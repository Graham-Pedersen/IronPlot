(define l (list  1 2 3 4))

(define (print-list l)
    (if (equal? '() l)
    '()
    (begin 
        (displayln (car l))
        (print-list (cdr l)))))

(displayln (print-list l))