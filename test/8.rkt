#lang racket/base

(define (make_adder x)
    (lambda (y) (+ x y)))

(define (plus_fun x y)
    (define + (lambda (x y) (* x y)))
    (+ x y))


(define add_three (make_adder 3))
;should print 8
(displayln (add_three 5))


;should print 20
(displayln (plus_fun 4 5))