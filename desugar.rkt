; This is a very simple desugarer for a simple scheme like language based on the article at http://matt.might.net/articles/desugaring-scheme/ over time we will grow this into a consideral subset of the racket language which we will grow out of this file by adding and changing functionality

; TODO remove language dependances from racket


#lang racket


; Entry Point

(define (desugar-input)
    (define program (car (read-input)))

    ; convert all the top leves forms into defines
    (set! program (tops-to-defs program))
    ; now that everthing is defines we can desugar defines
    (set! program (map desugar-define program))
    (displayln (pretty-format program)))


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
        (displayln top)
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
        [(let?    exp   ) (desugar-let exp)]
        [else (displayln `(could not desugar expression ,exp))]))
    
(define (desugar-body body) body)
(define (desugar-quote s-exp) s-exp)
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

                            
(define (function-def->var-def def)
    `(define ,(caadr def) (lambda ,(cdadr def) ,(caddr def))))

; ----- matching helper functions -----


; matches `(define (,f ,params ...) . ,body)
(define (function-def? top)
    (and (eq? 'define (car top)) (list? (cadr top))))

; matches `(define ,v ,exp)
(define (var-def? exp)
    (and (eq? 'define (car exp)) (not (list? (cadr exp)))))

; matchs any quoted list
(define (quote? exp)
    (cond
        [(eq? 'quote (car exp)) #t]
        [else #f]))

(define (let? exp)
    (cond
        [(and (eq? 'let (car exp)) (list? (cadr exp)))  #t]
        [else #f]))

(define (letrec? exp)
    (cond
        [(and (eq? 'letrec (car exp)) (list? (cadr exp)))  #t]
        [else #f]))

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


(desugar-input)