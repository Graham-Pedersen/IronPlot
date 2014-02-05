(define fact (lambda (x) 
		(if (equal? 1 x) 
			1 
			(* x (fact (- x 1)))))) 
(define result (fact 5))
result