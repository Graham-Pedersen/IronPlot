(define (boring_fact x)
    (if 
        (<= x 1)
        1
        (* x (boring_fact (- x 1)))))



(define (fact x)
    (define f (lambda (x) (if (<= x 1) 1 (* x (fact (- x 1))))))

    (define neg 1)
    (if (< x 0)
        (begin
            (set! x (* -1 x))
            (set! neg -1))
        '())
    (* neg (f x)))

(define x 5)
(set! x 8)
(if 
	(and (= (boring_fact 8) (fact x))  (= 40320 (fact x)))
	(displayln 'Passed)
	(displayln 'Failed))
	

