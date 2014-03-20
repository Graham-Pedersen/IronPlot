  (define fiblist '(0 1 1 2 3 5))
    
    (define (f x) 
      
      (displayln "f: ~s ~n")
      (define (help x) x)
      
      (cond 
        [(< x 0)  (error "fail!")]
        [(= x 0)  1]
        [(> x 0)  (* x (f (- x 1)))]
        [else     'universe-broke]))
      
    (define x 20)
    
    (displayln (and 1 2 3))
    
    (displayln (or 1 2))
    
    (displayln `(foo bar 3 ,(+ 1 2) ,@(list 'a 'b)))
    
    (displayln (f x))