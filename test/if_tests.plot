(define passed #f)

(if #t
    (set! passed #t)
    (void))
	
(if #f
	(set! passed #f)
	(void))
	
(if
	passed
	(displayln 'Passed)
	(displayln "Failed"))

