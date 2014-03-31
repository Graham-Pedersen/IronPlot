(let ([p (lambda a (apply + a))])
  (p 3 2 (p) (p 1) (p 4 2 1)))

