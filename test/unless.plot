(unless #t 
	(display "should not print"))

(unless #t
	(display "should")
	(display " not")
	(display " print"))

(unless #f
	(display "should print"))