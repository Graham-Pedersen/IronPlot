(let ((x 5) (y 6))
		(define result (+ x y))
		(if 
			(equal? result 11)
			(displayln 'Passed)
			(displayln "Failed")))