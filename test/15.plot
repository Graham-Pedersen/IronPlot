(define (get-first pair)
	(let ([first (car pair)]
	      [rest (cdr pair)])
		first))

(displayln (get-first (cons 1 2)))