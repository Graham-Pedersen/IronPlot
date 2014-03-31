(using CompilerLib)

(define x (new primeGener 1001))

(define (getPrimesprimeList)
	(define prime (call primeList getNext))
	(if (not (equal? -1 prime))
		(begin (displayln prime) (getPrimes primeList))
		'()))
		
(displayln (getPrimes x))
