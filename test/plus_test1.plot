(begin 
       (define ten (+ 1 2 3 4))
       (define twentytwo (+ (+ 4 5) (+ 6 7)))
       (define sixteen (+ 1 (+ 3 (+ 5 7))))
	   (if (and (equal? ten 10) (equal? twentytwo 22) (equal? sixteen 16))
		(displayln "Passed")
		(displayln "Failed")))