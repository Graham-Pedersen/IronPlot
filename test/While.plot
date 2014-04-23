(define i 0)
(while 
	(not (equal? i 10)) 
	(begin 
		(set! i (+ i 1))))
		
(if (equal? i 10)
	(displayln "Passed")
	(displayln "Failed"))
	