(define (get-first pair)
	(let ([first (car pair)]
	      [rest (cdr pair)])
		first))

(if 
	(equal? (get-first (cons 1 2)) 1)
	(displayln 'Passed)
	(displayln "Failed"))