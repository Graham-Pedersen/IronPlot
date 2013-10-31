#lang racket
(define (add-one x) (+ x 1))
(define y 3)
(define (plus x y) (+ x y))
(plus y (add-one 2))
