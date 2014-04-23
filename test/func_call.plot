(define (add x y)
	(+ x y))

(if (equal? 3 (add 1 2))
	(displayln "Passed")
	(displayln "Failed"))