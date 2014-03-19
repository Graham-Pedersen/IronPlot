(define x (new CompilerLib.primeGener 101))

(define getPrimes (lambda (primeList)
	(define prime (call primeList getNext))
	(if (not (equal? -1 prime))
		(begin (displayln prime) (getPrimes primeList))
		'())))
		
(getPrimes x)
