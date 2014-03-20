(letrec ([f (lambda (x) (g x))]
	[g (lambda (y) y)])
	(f 10))