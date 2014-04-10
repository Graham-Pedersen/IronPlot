(let ([x 5])
	(let ([f (lambda () x)])
		(let ([x 3])
			(if
				(equal? (f) 5)
				(displayln 'Passed)
				(displayln "Failed")))))
