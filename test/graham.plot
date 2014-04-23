(define l '(1 2 3 4))

(define suml (lambda (list) 
	(if 
		(equal? '() list)
		0
		(+ (car list) (suml (cdr list))))))

(if (equal? 10 (suml l))
	(displayln "Passed")
	(displayln "Failed"))