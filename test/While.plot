(define i 0)
(while 
	(not (equal? i 10)) 
	(begin 
		(set! i (+ i 1))))
(displayln i)
	