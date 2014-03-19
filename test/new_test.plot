 (define (fizzbuzz x)
    (cond
        [(equal? 0 (% x 15 )) (displayln 'fizzbuzz)]
        [(equal? 0 (% x 5 )) (displayln 'fizz)]
        [(equal? 0 (% x 3 )) (displayln 'buzz)]
        [else (displayln x)]))

(define (iter current end)
    (if (equal? current end)
        (displayln 'done)
        (begin (fizzbuzz current)
        (iter (+ current 1) end))))

(iter 1 100)
