(let ([x 5])
	(let ([f (lambda () x)])
		(let ([x 3])
			(displayln (f)))))