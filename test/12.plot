(define x (netcons DLR_Compiler.primeGener 101))

(define getPrimes (lambda (primeList)
	(define prime (netcall primeList getNext))
	(if (not (equal? -1 prime))
		(begin (displayln prime) (getPrimes primeList))
		'())))
		
(getPrimes x)
