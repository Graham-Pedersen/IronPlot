(define x 2)
(set! x '(hello ^^))
(if 
	(equal? x '(hello ^^))
	(displayln 'Passed)
	(displayln "Failed"))