#lang racket/base

(define % modulo)

(define (fizzbuzz x)
    (define (do_fizzbuzz current)
        (cond
            [(> current x) '()]
            [(eq? 0 (% current 15)) (cons 'fizzbuzz (do_fizzbuzz (+ current 1)))]
            [(eq? 0 (% current 3))  (cons 'fizz (do_fizzbuzz (+ current 1)))]
            [(eq? 0 (% current 5))  (cons 'buzz (do_fizzbuzz (+ current 1)))]
            [else (cons current (do_fizzbuzz (+ current 1)))]))
     (do_fizzbuzz 1))


(displayln (fizzbuzz 15))