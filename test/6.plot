  (define fiblist '(0 1 1 2 3 5))
    
(define (f x)
  
  (cond 
	[(< x 0)  (error "fail!")]
	[(= x 0)  1]
	[(> x 0)  (* x (f (- x 1)))]
	[else     'universe-broke]))
	
	
(if 
	(equal? 3628800 (f 10))
	(displayln 'Passed)
	(displayln "Failed"))