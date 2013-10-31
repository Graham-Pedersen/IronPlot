#lang racket
    

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

(define (make_adder x)
    (lambda (y) (+ x y)))

(define (plus_fun x y)
    (define + (lambda (x y) (* x y)))
    (+ x y))
     


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



(define x 5)
(displayln (fact x))
(set! x 8)
(displayln (fact x))
(boring_fact 5)

(define add_three (make_adder 3))
;should print 8
(displayln (add_three 5))

;should print 20
(displayln (plus_fun 4 5))

(define l (list  1 2 3 4))

(define (print-list l)
    (if (eq? '() l)
    '()
    (begin 
        (displayln (car l))
        (print-list (cdr l)))))

(print-list l)
