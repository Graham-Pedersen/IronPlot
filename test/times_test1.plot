(define return #f)
(begin
	(set! return(equal? 6 (* 1 2 3)))
	(set! return (equal? 24 (* (* 1 2) (* 3 4))))
	(set! return (equal? 6 (* 1 (* 2 3))))
	(if return
		(displayln "Passed")
		(displayln "Failed")))