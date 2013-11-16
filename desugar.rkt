; This is a very simple desugarer for a simple scheme like language based on the article at http://matt.might.net/articles/desugaring-scheme/ over time we will grow this into a consideral subset of the racket language which we will grow out of this file by adding and changing functionality

; TODO remove language dependances from racket

#lang racket


; Entry Point

(define (desugar-input)
    (define program (read-input))
    (set! program (tops-to-defs program)) ;; convert tops into defines  
    (set! program (map desugar-define program)) ;; desugar the defines now
    (set! program (partition-k 
                    atomic-define?
                    program
                    (lambda (atomic complex)
                        (define bindings
                            (for/list ([c complex])
                                (match c
                                    [`(define ,v ,complex)
                                     1(,v (void))])))
                        (define sets
                            (for/list ([c complex])
                                (match c
                                    [`(define ,v ,complex)
                                    `(set! ,v ,complex)])))
                    (append atomic (list `(let ,bidings ,sets))))))
    (displayln (pretty-format program 40)))


; Helper Functions

; reads all input from std in into a list
(define (read-input)
    (define line (read))
    (if (eof-object? line)
        '()
        (cons line (read-input))))
    

; Desugaring Functions

(define (tops-to-defs tops)
    (define (top-to-def top)
        (cond 
            [(function-def? top) (function-def->var-def top)]
            [(var-def? top) top]
            [else `(define ,(gensym '_) ,top)]))
    (map top-to-def tops))


; desugar define statements of the form (define ,v ,exp)
(define (desugar-define def)
    (cond
        [(var-def? def) `(define ,(cadr def) ,(desugar-exp (caddr def)))]
        [else `(cannot desugar define ,def)]))

(define (desugar-exp exp)
    (cond
        [(symbol? exp) exp]
        [(atomic? exp) exp]
        [(quote?  exp) (desugar-quote exp)]
        [(let?    exp) (desugar-let exp)]
	    [(letrec? exp) (desugar-letrec exp)]
	    [(lambda? exp) (desugar-lambda exp)]
        [(and? exp) (desugar-and exp)]
        [(or? exp) (desugar-or exp)] 
        [(if? exp) (desugar-if exp)]
        [(set!? exp) (desugar-set! exp)]
        [(quasi-quote? exp) ] ;; need to do
        [(begin? exp) ]
        [(function-call? exp) (desugar-func exp)]
        [else (displayln `(could not desugar expression ,exp))]))
 
;; --------- desugar helpers -------------
   
(define (desugar-body body) body)
#|
;; desugar body of a lambda or begin
(define (desugar-body body) 
  (cond ;; or should it just be body?
    [(null? (cdr body)) (desugar-exp (car body))]  ;; case 1, just an expression, I think this is a valid check
    [ ]  ;; case 2, a begin
    [ ])) ;; case 3, contains definitions

|#

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


(define (desugar-quasi-quote s-exp) s-exp)

(define (desugar-let exp)
    (define vars (cadr exp))
    (define body (caddr exp))

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
      ,@(map (lambda (v) `(set! ,(get-var v) ,(get-exp v))) vars)
      ,@body)))

;; ------ desugaring lambda ------

(define (desugar-lambda exp) ;; `(lambda ,params . ,body)
  (define body (cdr (cdr exp)))
  `(lambda ,(car (cdr exp)) ,(desugar-body body)))

;; --------- functions ---------
                            
(define (function-def->var-def def)
    `(define ,(caadr def) (lambda ,(cdadr def) ,(caddr def))))


;; ------------- transform function call -----

(define (desugar-func exp)
  `(,(desugar-exp (car exp))
    ,@(map desugar-exp (cdr exp))))

;; ----------- desugaring and/or ------------

(define (desugar-or exp)
  (match exp
    [`(or) #f]
    [`(or ,exp) (desugar-exp exp)]
    [`(or ,exp . ,rest) 
        (define $t (gensym 't))
        (desugar-exp 
            `(let ((,$t ,exp))
                (if ,$t #t (or . ,rest))))])) 


(define (desugar-and exp)
  (match exp
    [`(and) #t]
    [`(and ,exp) (desugar-exp exp)]
    [`(and ,exp . ,rest) `(if ,(desugar-exp exp) ,(desugar-exp `(and . ,rest)) #f)]))

;; ---------- desugaring if ---------------

(define (desugar-if exp)
  (cond
    [(null? (cdr (cdr (cdr exp)))) ;; `(if ,test ,exp)
     `(if ,(desugar-exp (car (cdr exp)))
          ,(desugar-exp (cdr (cdr exp)))
          (void))]
    [else ;; `(if ,test ,exp1 ,exp2)
     `(if ,(desugar-exp (car (cdr exp))) ;; test
          ,(desugar-exp (car (cdr (cdr exp)))) ;; exp1
          ,(desugar-exp (cdr (cdr (cdr exp)))))]))


;; ------------ desugaring/transforming set! ---------

(define (desugar-set! exp)
  `(set! ,(car (cdr exp)) ,(desugar-exp (cdr (cdr exp)))))

; ----- matching helper functions -----


; matches `(define (,f ,params ...) . ,body)
(define (function-def? top)
    (and (eq? 'define (car top)) (list? (cadr top))))

; matches `(define ,v ,exp)
(define (var-def? exp)
    (and (eq? 'define (car exp)) (not (list? (cadr exp)))))

; matchs any quoted list
(define (quote? exp)
        (eq? 'quote (car exp)))

(define (let? exp)
  (and (eq? 'let (car exp))
       (list? (car (cdr exp)))
       (not (null? (cdr (cdr exp))))))

(define (letrec? exp)
        (and (eq? 'letrec (car exp)) (list? (cadr exp))))

;; matches lambda I think
(define (lambda? exp)
  (and (eq? 'lambda (car exp))
       (list? (car (cdr exp)))
       (not (null? (cdr (cdr exp))))))

(define (begin? exp)
  (and (eq? 'begin (car exp))
       (not (null? (cdr exp)))))

(define (quasi-quote? exp)
  (and (eq? 'quasi-quote (car exp))
       (not (null? (cdr exp)))))

(define (and? exp) ;; check that the first element of the list is and
  (eq? 'and (car exp)))

(define (or? exp)
  (eq? 'or (car exp)))

(define (if? exp)
  (or (and (eq? 'if (car exp)) ;; matches `(if ,test ,exp)
          (not (null? (cdr exp)))
          (not (null? (cdr (cdr exp))))
          (null? (cdr (cdr (cdr exp))))) 
      
      (and (eq? 'if (car exp)) ;; matches `(if ,test ,exp ,exp2)
          (not (null? (cdr exp)))
          (not (null? (cdr (cdr exp))))
          (not (null? (cdr (cdr (cdr exp)))))
          (null? (cdr (cdr (cdr (cdr exp))))))))

(define (set!? exp)
  (and (eq? 'set! (car exp))
       (not (null? (cdr exp)))
       (not (null? (cdr (cdr exp))))
       (null? (cdr (cdr (cdr exp))))))


;; not sure what else to check for
(define (function-call? exp)
  (and (not (null? (car exp)))
       (not (null? (cdr exp)))))
    
    

; matches any atomic
(define (atomic? exp)
  (cond
    [(number? exp)   #t]
    [(string? exp)   #t]
    [(boolean? exp)  #t]
    [(eq? 'lambda (car exp))  #t]
    [(eq? 'quote (car exp)) #t]
    [(eq? 'void (car exp))       #t]
    [else          #f]))

(define (partition-k pred list k)
  (if (not (pair? list))
      (k '() '())
      (partition-k pred (cdr list) (lambda (in out)
        (if (pred (car list))
            (k (cons (car list) in) out)
            (k in (cons (car list) out)))))))

(desugar-input)