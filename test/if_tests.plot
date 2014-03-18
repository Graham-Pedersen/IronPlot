(if #t
    (begin (display "hi")
    	   (displayln " there"))
    (void))


(if #t (void) (begin (display "should not print")))

(if #f (void) (begin (display "should print")))

(if #f (display "1") (display "2"))