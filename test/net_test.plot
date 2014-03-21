(define list (new System.Collections.Generic.List (typelist System.Int32)))
(call list Add 1)
(call list Add 5)
(call list Add 7)
(define count (call list Count))

(define printList (lambda (l index max)
	(if (not (equal? index max))
		(begin 
			(displayln (call l Item index))
			(printList l (+ index 1) max))
		(displayln '(end of list)))))
		
(displayln (printList list 0 count))
