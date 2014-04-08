(define l (list  1 2 3 4))

(define (sum-list l)
    (if (equal? '() l)
    0
    (+ (car l) (sum-list (cdr l)))))

(if 
	(equal? 10 (sum-list l))
	(displayln 'Passed)
	(displayln 'Failed))