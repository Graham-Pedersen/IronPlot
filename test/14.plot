(define make-addr (lambda (x) (lambda (y) (+ x y)))) 
(define 5-addr (make-addr 5))
(if 
	(equal? 9 (5-addr 4))
	(displayln 'Passed)
	(displayln "Failed"))