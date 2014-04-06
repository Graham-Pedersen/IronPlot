(define (add-one x) (+ x 1))
(define y 3)
(define (plus x y) (+ x y))
(+ '1 '2)
(if 
	(equal? 6 (plus y (add-one 2)))
	(displayln 'Passed)
	(displayln "Failed"))

