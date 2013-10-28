; TODO remove language dependances from racket
#lang racket

;move to library this is not the right place
(define gen-var 
    ((lambda (init)
        (define x init)
        (lambda (sym) 
            (set! x (+ 1 x))
            `(,sym x)))0))

; reads all input from std in into a list
(define (read-input)
    (define line (read))
    (if (eof-object? line)
        '()
        (cons line (read-input))))

(define (desugar-input)
    (define program (car (read-input)))
    (displayln '(this is the input program:))
    (displayln (pretty-format input))
    (displayln '(output program:))
    ;convert all the top leves into defines
    (set! program (tops-to-defs input))
    ;now that everthing is defines we can do this
    (set! program (map desugar-define program))
    (displayln (pretty-format program)))

;desugar all our define statments
(define (desugar-define def) 
    (cond
        []
        [else `(cannot desugar define, def)

(define (desugar-define def) def)
(define (desugar-exp exp) exp)
(define (desugar-body body) body)
(define (desugar-quote s-exp) s-exp)
(define (desugar-quasi-quote s-exp) s-exp)


;converts all top level forms into defines
(define (tops-to-defs tops)
    (define (top-to-def top)
        (displayln top)
        (cond 
            [(function-def? top) `(define ,(caadr top) (lambda ,(cdadr top) . ,(caddr top)))]
            [(var-def? top) top]
            [else `(define ,(gensym '_) ,(cdr top))]))

    (map top-to-def tops))


; ----- matching helper functions -----


;matches `(define (,f ,params ...) . ,body)
(define (function-def? top)
    (if (and (eq? 'define (car top)) (list? (cadr top)))
        #t
        #f))

; matches `(define ,v ,exp)
(define (var-def? top)
    (if (and (eq? 'define (car top)) (not (list? (cadr top))))
        #t
        #f))


(desugar-input)