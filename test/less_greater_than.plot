(begin
	(define true (< 1 2))
	(define false (< 7 1))
	(define truet (> 7 1))
	(define falsf (> 0 2))
	(if (and (equal? true #t) (equal? false #f) (equal? truet #t) (equal? falsf #f))
		(displayln "Passed")
		(displayln "Failed")))