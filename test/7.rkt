#lang racket/base

(define (boring_fact x)
    (if 
        (<= x 1)
        1
        (* x (boring_fact (- x 1)))))



; fact implimented with scoping using lambda
(define (fact x)
    (define f (lambda (x) (if (<= x 1) 1 (* x (fact (- x 1))))))

    (define neg 1)
    (if (< x 0)
        (begin
            (set! x (* -1 x))
            (set! neg -1))
        '())
    (* neg (f x)))

(define x 5)
(displayln (fact x))
(set! x 8)
(displayln (fact x))
(boring_fact 5)

