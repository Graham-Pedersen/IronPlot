(if #t
    (begin (displayln "hi")
    	   (displayln " there"))
    (void))


(if #t (void) (begin (displayln "should not print")))

(if #f (void) (begin (displayln "should print")))

(if #f (displayln "1") (displayln "2"))