(letrec (
		[f (lambda (x) (g x))]
        [g (lambda (y) y)])
			(if 
				(equal? 10 (f 10))
				(displayln 'Passed)
				(displayln "Failed")))