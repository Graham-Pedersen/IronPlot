;This is a very simple desugarer for a simple scheme like language based on the article at http://matt.might.net/articles/desugaring-scheme/ over time we will grow this into a consideral subset of the racket language which we will grow out of this file by adding and changing functionality

; TODO remove language dependances from racket

#lang racket

 
; Entry Point
 ;; add begin to top level
(define (desugar-input)
;; (define program (read-input))
  (define out (void))
  (define program (read-input (open-input-file (command-line
                   #:args (filename output)
                   (begin
                     (set! out (open-output-file output #:exists 'replace))
                   filename)) #:mode 'text)))
  (set! program (desugar-tops program)) ;; desugar-tops
  (set! program (map desugar-define program)) ;; desugar the defines now
  (set! program (partition-k 
                 atomic-define?
                 program
                 (lambda (atomic complex)
                   (define bindings
                     (for/list ([c complex])
                       (match c
                         [`(define ,v ,complex)
                          `(,v (void))]
                         [else (displayln "cannot desugar begin")])))
                   (define sets
                     (for/list ([c complex])
                       (match c
                         [`(define ,v ,complex)
                          `(set! ,v ,complex)]
                         [else (displayln "cannot desguar begin")])))
                   (append atomic (list (desugar-exp `(let ,bindings ,@sets)))))))
  (write program out))
;; (displayln (pretty-format program 40)))



; reads all input from std in into a list
(define (read-input [file (current-input-port)])
    (define line (read file))
    (if (eof-object? line)
        '()
        (cons line (read-input file))))
    

; desugar top level begins, hope this is correct
(define (splice-begin-forms tops)
  (define (splice-begin top)
    (match top
      [`(begin . ,forms) `(begin-top . ,forms)]
      [else (displayln `(cannot desugar begin))]))
  (map splice-begin tops))
       ; Desugaring Functions

(define (desugar-tops tops)
    (define (top-to-def top)
        (match top
            [`(define (,name ,params ...) . ,body)
             (function-def->var-def top)]
            [`(define ,var ,exp)
             `(define ,var ,exp)]
            [`(begin . ,forms)
             `(begin-top ,@forms)]
            [exp `(define ,(gensym '_) ,exp)]))
    (map top-to-def tops)) ;; map is putting in list...... problem with program wrapped in parens


; desugar define statements of the form (define ,v ,exp)
(define (desugar-define def)
    (match def
      [`(define ,v ,exp) `(define ,v ,(desugar-exp exp))]
      [`(begin-top . ,forms) `(begin-top ,@(map desugar-exp forms))]
      [else (displayln `(cannot desugar define ,def))]))

(define (desugar-exp exp)
    (match exp
      [(? symbol?) exp]
      [`(quote ,s-exp) (desugar-quote s-exp)]
      [`(let ((,vs ,es) ...) . ,body)
       `((lambda ,vs ,(desugar-body body)) ,@(map desugar-exp es))]
      
      [`(letrec ((,vs ,es) ...) . ,body) 
       (desugar-exp `(let ,(for/list ([v vs])
                             (list v '(void)))
                       ,@(map (λ (v e)
                                `(set! ,v ,e))
                              vs es)
                       ,@body))]
      [`(lambda ,params . ,body) `(lambda ,params ,(desugar-body body))]
      [`(cond) '(void)]
      [`(cond (else ,exp)) (desugar-exp exp)]
      [`(cond (,test ,exp)) 
       `(if ,(desugar-exp test) ,(desugar-exp exp) (void))]
      [`(cond (,test ,exp) ,rest ...) 
       `(if ,(desugar-exp test) ,(desugar-exp exp) ,(desugar-exp `(cond . ,rest)))]
      [`(and)     #t]
      [`(or)      #f]
      [`(and ,exp) (desugar-exp exp)]
      [`(or ,exp) (desugar-exp exp)]
      [`(and ,exp . ,rest) 
       `(if ,(desugar-exp exp)
            ,(desugar-exp `(and . ,rest)) #f)]
      [`(or ,exp . ,rest) 
       (define $t (gensym 't))
       (desugar-exp `(let ((,$t ,exp)) 
                       (if ,$t ,$t (or . ,rest))))]
      [`(if ,test ,exp)
       `(if ,(desugar-exp test) ,(desugar-exp exp) (void))]
      [`(if ,test ,exp1 ,exp2)
       `(if ,(desugar-exp test) ,(desugar-exp exp1) ,(desugar-exp exp2))]
      [`(set! ,v ,exp) `(set! ,v ,(desugar-exp exp))]
      [`(quasiquote ,qq-exp) (desugar-quasi-quote 1 qq-exp)]
      [`(begin . ,body) (desugar-body body)]
      [`(first ,exp) `(car ,exp)]
      [`(rest ,exp) `(cdr ,exp)]
      [`(+ ,farg ,sarg . ,rest) (if (null? rest) ;; added today, need to test
                                    `(+ ,farg ,sarg)
                                    `(+ ,farg ,(desugar-exp `(+ ,sarg ,@rest))))]
      [`(* ,farg ,sarg . ,rest) (if (null? rest) ;; added today , need to test
                                    `(* ,farg ,sarg)
                                    `(* ,farg ,(desugar-exp `(* ,sarg ,@rest))))]
      [`(/ ,farg ,sarg . ,rest) ;;(if (null? rest) ;; added today , need to test
                                  ;;   `(/ ,farg ,sarg)
                                     (foldl (lambda (v a) `(/ ,a
                                                              ,(desugar-exp v)))
                                            `(/ ,(desugar-exp farg)
                                                ,(desugar-exp sarg))
                                            rest)]
      [`(- ,farg ,sarg . ,rest) ;;(if (null? rest) ;; added today, need to test
                                 ;;  `(- ,farg ,sarg)
                                   (foldl (lambda (v a) `(- ,a
                                                              ,(desugar-exp v)))
                                            `(- ,(desugar-exp farg)
                                                ,(desugar-exp sarg))
                                            rest)]
      [`(list . ,rest) (foldr (lambda (x a) `(cons ,(desugar-exp x) ,a)) '() rest)] ;; needs testing 
                                   
      [(? atomic?) exp]
      [`(,function . ,args) 
       `(,(desugar-exp function) ,@(map desugar-exp args))]
      [else (displayln `(could not desugar expression ,@exp))]))

;; --------- desugar helpers -------------



;; desugar body of a lambda or begin
(define (desugar-body body)
  (match body
    [`(,exp)
     (desugar-exp exp)]
    
    [`(,(and (? not-define?) exps) ...)
     `(begin ,@(map desugar-exp exps))]
    
    [`(,tops ... ,exp)
     (define defs (desugar-tops tops))
     (desugar-exp (match defs
                    [`((define ,vs ,es) ...)
                     `(letrec ,(map list vs es) ,exp)]))]))



(define (desugar-quote s-exp)
  (cond
    [(pair? s-exp) `(cons ,(desugar-quote (car s-exp))
                          ,(desugar-quote (cdr s-exp)))]
    [(null? s-exp) ''()]
    [(number? s-exp) s-exp]
    [(string? s-exp) s-exp]
    [(boolean? s-exp) s-exp]
    [(symbol? s-exp) `(quote, s-exp)]
    [else 
     (error (format "strange value in quote: ~s~n" s-exp))]))


(define (desugar-quasi-quote n s-exp)
  (match s-exp
    [(list 'unquote exp)
     (if (= n 1)
         (desugar-exp exp)
         (list 'list ''unquote 
               (desugar-quasi-quote (- n 1) exp)))]
    [`(quasiquote ,s-exp)
     `(list 'quasiquote ,(desugar-quasi-quote (+ n 1) s-exp))]   
    [(cons (list 'unquote-splicing exp) rest)
     (if (= n 1)
         `(append ,exp ,(desugar-quasi-quote n rest))
         (cons (list 'unquote-splicing (desugar-quasi-quote (- n 1) exp))
               (desugar-quasi-quote n rest)))]    
    [`(,qq-exp1 . ,rest)
     `(cons ,(desugar-quasi-quote n qq-exp1)
            ,(desugar-quasi-quote n rest))]
    [else 
     (desugar-quote s-exp)]))

(define (desugar-let exp) ;; `(let ((,vs ,es) ...) . ,body)
    (define vars (cadr exp))
    (define body (cddr exp))
        (define var-getter 
            (lambda (x)
                (if (null? x)
                    '()
                    (cons (caar x) (var-getter (cdr x))))))
    (define exp-getter 
            (lambda (x)
                (if (null? x)
                    '()
                    (cons (cadar x) (exp-getter (cdr x))))))

    (define var-names (var-getter vars))
    (define exps (exp-getter vars))
    `((lambda ,var-names ,(desugar-body body))
        ,@(map desugar-exp exps)))


;; i love me some functional programming grahamammm

(define (desugar-letrec exp) ;; `(letrec ((,vs ,es) ..) . ,body)
  (define vars (car (cdr exp)))
  (define body (cdr (cdr exp)))
  (define get-var
    (lambda (v)
      (if (null? v)
          '()
          (car v))))
  (define get-exp
    (lambda (e)
      (if (null? e)
          '()
          (cdr e))))
  (desugar-exp
   `(let ,(map (lambda (v) (list (get-var v) '(void))) vars)
      ,@(map (lambda (v) `(set! ,(get-var v) ,@(get-exp v))) vars)
      ,@body)))

;; ------ desugaring lambda ------

(define (desugar-lambda exp) ;; `(lambda ,params . ,body)
  (match exp
    [`(lambda ,params . ,body)
     (displayln `(in lambda ,body))
     `(lambda ,params ,(desugar-body body))]))

     
; ----- helper functions -----
                            
(define (function-def->var-def def)
    `(define ,(caadr def) (lambda ,(cdadr def) ,(caddr def))))


(define (not-define? symbol)
    (not (define? symbol)))

(define (define? symbol)
    (match symbol
        [`(define . ,_) #t]
        [else           #f]))

; aotmic-define? : term -> boolean
(define (atomic-define? def)
  (match def
    [`(define ,v ,exp) (atomic? exp)]
    [`(begin-top . ,forms) #t] ;; for now just do this
    [else #f]))


; matches `(define (,f ,params ...) . ,body)
(define (function-def? top)
    (and (eq? 'define (car top)) (list? (cadr top))))

; matches `(define ,v ,exp)
(define (var-def? exp)
    (and (eq? 'define (car exp)) (not (list? (cadr exp)))))    
    

; matches any atomic
(define (atomic? exp)
  (match exp
    [`(lambda . ,_)#t]
    [(? number?)   #t]
    [(? string?)   #t]
    [(? boolean?)  #t]
    [`(quote . ,_) #t]
    ['(void)       #t]
    [else          #f]))

(define (partition-k pred list k)
  (if (not (pair? list)) ;; check if empty
      (k '() '())
      (partition-k pred (cdr list) 
                   (lambda (in out)
                     (if (pred (car list))
                         (k (cons (car list) in) out)
                         (k in (cons (car list) out)))))))

(desugar-input)