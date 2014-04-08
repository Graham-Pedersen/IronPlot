(letrec ([f (lambda (x) (g x))]
	[g (lambda (y) y)])
	(if 
		(equal? (f 10) 10)
		(displayln 'Passed)
		(displayln "Failed")))
	
