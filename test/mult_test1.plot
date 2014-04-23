(begin
	(define twf (* 1 2 3 4))
	(define twf_ (* (*  1 2)(* 3 4)))
	(define twf__ (* 1 (* 2 (* 3 4))))
	(if (and (equal? twf 24) (equal? twf_ 24) (equal? twf__ 24))
		(displayln "Passed")
		(displayln "Failed")))