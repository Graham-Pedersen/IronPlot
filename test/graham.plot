(define l '(1 2 3 4))

(define suml (lambda (list) 
	(if 
		(equal? '() list)
		0
		(+ (car list) (suml (cdr list))))))

(displayln (suml l))