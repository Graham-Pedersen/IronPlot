
(define (fizzbuzz x)
    (define (do_fizzbuzz current)
        (cond
            [(> current x) '()]
            [(equal? 0 (% current 15)) (cons 'fizzbuzz (do_fizzbuzz (+ current 1)))]
            [(equal? 0 (% current 3))  (cons 'fizz (do_fizzbuzz (+ current 1)))]
            [(equal? 0 (% current 5))  (cons 'buzz (do_fizzbuzz (+ current 1)))]
            [else (cons current (do_fizzbuzz (+ current 1)))]))
     (do_fizzbuzz 1))

(if 
	(equal? '(1 2 fizz 4 buzz fizz 7 8 fizz buzz 11 fizz 13 14 fizzbuzz) (fizzbuzz 15))
	(displayln 'Passed)
	(displayln 'Failed))