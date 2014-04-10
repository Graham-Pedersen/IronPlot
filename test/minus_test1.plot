(begin 
      (define (test) 
		  (+  
			(- 1 2 4)
			(- (- 1 2) (- 5 6))
			(- (- 1 2) 4)))
		(displayln (test))
		(if (equal? (test) (- 0 10))
			(displayln 'Passed)
			(displayln 'Failed)))