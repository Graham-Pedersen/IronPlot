#lang racket

;; Helpers.

; partition-k : ('a -> boolean) 'a list ('a list 'a list -> 'a list 'a list)
(define (partition-k pred lst k)
  (if (not (pair? lst))
      (k '() '())
      (partition-k pred (cdr lst) (λ (in out)
        (if (pred (car lst))
            (k (cons (car lst) in) out)
            (k in (cons (car lst) out)))))))

; define? : term -> boolean
(define (define? sx)
  (match sx
    [`(define . ,_) #t]
    [else           #f]))

; not-define? : term -> boolean
(define (not-define? sx)
  (not (define? sx)))

; atomic? : term -> boolean
(define (atomic? exp)
  (match exp
    [`(λ . ,_)     #t]
    [(? number?)   #t]
    [(? string?)   #t]
    [(? boolean?)  #t]
    [`(quote . ,_) #t]
    ['(void)       #t]
    [else          #f]))

; atomic-define? : term -> boolean
(define (atomic-define? def)
  (match def
    [`(define ,v ,exp)  (atomic? exp)]
    [else               #f]))

; tops-to-defs : top list -> def list
(define (tops-to-defs tops)
  
  (define (top-to-def top)
    (match top
      [`(define (,f ,params ...) . ,body) 
       `(define ,f (λ ,params . ,body))]
    
      [`(define ,v ,exp)
       `(define ,v ,exp)]
    
      [exp
       `(define ,(gensym '_) ,exp)]))
  
  (map top-to-def tops))


      

;; Desugaring.

; desugar-define : define-term -> exp
(define (desugar-define def)
  (match def
    [`(define ,v ,exp)
     `(define ,v ,(desugar-exp exp))]
    
    [else
     (error (format "cannot desugar: ~s~n" def))]))

; desugar : program -> program
(define (desugar-program prog)
  
  (set! prog (tops-to-defs prog))
  (set! prog (map desugar-define prog))
  (set! prog
    (partition-k 
     atomic-define?
     prog
     (λ (atomic complex)
       (define bindings
         (for/list ([c complex])
           (match c
             [`(define ,v ,complex)
              `(,v (void))])))
       
       (define sets
         (for/list ([c complex])
           (match c
             [`(define ,v ,complex)
              `(set! ,v ,complex)])))
       
       (append atomic (list `(let ,bindings ,sets))))))
  
  prog)

; desugar-quote : sexp -> exp
(define (desugar-quote s-exp)
  (cond
    [(pair? s-exp)     `(cons ,(desugar-quote (car s-exp))
                              ,(desugar-quote (cdr s-exp)))]
    [(null? s-exp)     ''()]
    [(number? s-exp)   s-exp]
    [(string? s-exp)   s-exp]
    [(boolean? s-exp)  s-exp]
    [(symbol? s-exp)   `(quote ,s-exp)]
    [else 
     (error (format "strange value in quote: ~s~n" s-exp))]))

; desugar-body : body -> exp
(define (desugar-body body)
  (match body
    [`(,exp)
     (desugar-exp exp)]
    
    [`(,(and (? not-define?) exps) ...)
     `(begin ,@(map desugar-exp exps))]
    
    [`(,tops ... ,exp)
     (define defs (tops-to-defs tops))
     (desugar-exp (match defs
                    [`((define ,vs ,es) ...)
                     `(letrec ,(map list vs es) ,exp)]))]))
       
; desugar-qq : qqexp -> exp
(define (desugar-qq n qq-exp)
  (match qq-exp
    [(list 'unquote exp)
     (if (= n 1)
         (desugar-exp exp)
         (list 'list ''unquote 
               (desugar-qq (- n 1) exp)))]
    
    [`(quasiquote ,qq-exp)
     `(list 'quasiquote ,(desugar-qq (+ n 1) qq-exp))]
    
    [(cons (list 'unquote-splicing exp) rest)
     (if (= n 1)
         `(append ,exp ,(desugar-qq n rest))
         (cons (list 'unquote-splicing (desugar-qq (- n 1) exp))
               (desugar-qq n rest)))]
    
    [`(,qq-exp1 . ,rest)
     `(cons ,(desugar-qq n qq-exp1)
            ,(desugar-qq n rest))]
       
    [else 
     (desugar-quote qq-exp)]))

; desugar-exp : exp -> exp
(define (desugar-exp exp)
  (match exp
    [(? symbol?)      exp]
    [`(quote ,s-exp)  (desugar-quote s-exp)]

    [`(let ((,vs ,es) ...) . ,body)
     `((λ ,vs ,(desugar-body body)) 
       ,@(map desugar-exp es))]
    
    [`(letrec ((,vs ,es) ...) . ,body)
     (desugar-exp
      `(let ,(for/list ([v vs])
               (list v '(void)))
         ,@(map (λ (v e)
                  `(set! ,v ,e))
                vs es)
         ,@body))]
    
    [`(λ ,params . ,body)
     `(λ ,params ,(desugar-body body))]
    
    [`(cond)
     '(void)]
    
    [`(cond (else ,exp))
     (desugar-exp exp)]
        
    [`(cond (,test ,exp))
     `(if ,(desugar-exp test) 
          ,(desugar-exp exp) 
          (void))]
    
    [`(cond (,test ,exp) ,rest ...)
     `(if ,(desugar-exp test)
          ,(desugar-exp exp)
          ,(desugar-exp `(cond . ,rest)))]
    
    [`(and)   #t]
    [`(or)    #f]
    
    [`(or ,exp)
     (desugar-exp exp)]
    
    [`(and ,exp)
     (desugar-exp exp)]
    
    [`(or ,exp . ,rest)
     (define $t (gensym 't))
     (desugar-exp 
      `(let ((,$t ,exp))
         (if ,$t ,$t (or . ,rest))))]
    
    [`(and ,exp . ,rest)
     `(if ,(desugar-exp exp)
          ,(desugar-exp `(and . ,rest))
          #f)]
    
    [`(if ,test ,exp)
     `(if ,(desugar-exp test) ,(desugar-exp exp) (void))]
    
    [`(if ,test ,exp1 ,exp2)
     `(if ,(desugar-exp test) 
          ,(desugar-exp exp1) 
          ,(desugar-exp exp2))]
    
    [`(set! ,v ,exp)
     `(set! ,v ,(desugar-exp exp))]

    [`(quasiquote ,qq-exp)
     (desugar-qq 1 qq-exp)]
    
    [`(begin . ,body)
     (desugar-body body)]
    
    [(? atomic?)      exp]
    
    [`(,f . ,args)  
     `(,(desugar-exp f) ,@(map desugar-exp args))]
            
    [else 
     (printf "desugar fail: ~s~n" exp)
     exp]))
         
      
; reads all input from std in into a list
(define (read-input)
    (define line (read))
    (if (eof-object? line)
        '()
        (cons line (read-input))))

(displayln (pretty-format (desugar-program (read-input) ) 40))


(define (pp x) (display (pretty-format x)))


          
      