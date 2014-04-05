(letrec ([f (lambda (x) (g x))]
               [g (lambda (y) y)])
        (displayln (f 10)))