(begin
	(define pass #t)
	(set! pass (and pass (equal? .125  (/ 1 2 4))))
	(set! pass (and pass (equal? .5  (/ (/ 1 2) (/ 4 4)))))
	(set! pass (and pass (equal? .1250  (/ (/ 1 2) 4))))
	(if 
		pass
		(displayln 'Passed)
		(displayln 'Failed)))
(scall System.Console ReadKey)