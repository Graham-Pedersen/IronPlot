(define add-two (lambda (x) (+ x 2)))

(displayln (add-two 4))

(displayln (
	(lambda (a b c) 
		(+ 
			(* a a)
			(+
				(* b b)
				(* c c))))  1 2 3))

(define sub (lambda (a b) (- a b)))				
				
(define crazy-town 
	(lambda (x y) 
		(define lambda sub)
		(lambda x y)))
		
(displayln (crazy-town 10 6))

(using System.Collections.Generic)

(displayln (+ 3 2))
(displayln (+ .1 .4))
(displayln (+ .2 5))

(displayln (- 1 2))
(displayln (- .1 .4))
(displayln (- .2 5))

(displayln (* 1 2))
(displayln (* .1 .4))
(displayln (* .2 5))

(displayln (/ 1 2))
(displayln (/ .1 .4))
(displayln (/ .2 5))

(displayln (% 3 2))
(displayln (% .4 .1))
(displayln (% 5 .2))

(define list (new System.Collections.Generic.List (typelist System.Int32)))
(call list Add 1)
(call list Add 5)
(call list Add 7)
(call list Add 100)
(define count (call list Count))

(define printList (lambda (l index max)
	(if (not (equal? index max))
		(begin 
			(displayln (call l Item index))
			(printList l (+ index 1) max))
		(displayln '(end of list)))))
(displayln (cons '(count is ) (cons count '())))
(printList list 0 count)
(scall System.Console ReadKey )
				

