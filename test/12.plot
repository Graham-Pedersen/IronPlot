(using System.Collections.Generic)
(define list (new System.Collections.Generic.List (typelist System.Int32)))
(call list Add 1)
(call list Add 5)
(call list Add 7)
(define count (call list Count))

(define (sumList l index max)
	(if (not (equal? index max))
		(+ (call l Item index) (sumList l (+ index 1) max))
		0))

(if 
	(equal? (sumList list 0 count) 13)
	(displayln 'Passed)
	(displayln 'Failed))