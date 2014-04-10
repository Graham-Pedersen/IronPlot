
(define (create_new_board c1 c2 c3 c4) 
	(list c1 c2 c3 c4))

(define (create_empty_board)
	(create_new_board 
						(list 0 0 0 0) 
						(list 0 0 0 0) 
						(list 0 0 0 0) 
						(list 0 0 0 0)))


(define (place_in_row num y board j)
	(cond 
           [(equal? y j)     (cons num (cdr board))]
           [else        (cons (car board) (place_in_row num y (cdr board) (+ j 1)))]))



(define	(place_at_i_j num ij board i j)
	(define x (car ij))
	(define y (cdr ij))	
	(cond
		[(equal? x i)  (cons (place_in_row num y (car board) 0) (cdr board))]
		[else     (cons (car board) (place_at_i_j num ij (cdr board) (+ i 1) j))]))



(define (append-element lst elem)
	(foldr cons (list elem) lst))



	

(define (shuffle_right row)
	(reverse(shuffle_left  row)))




(define (shuffle_left row)
	(cond
		[(null? row) '()] 	
		[(equal? (car row) 0)   (append-element (shuffle_left (cdr row)) 0)]
		[else (cons (car row) (shuffle_left (cdr row)))]))



(define (row_left list_row)
	(set! list_row (shuffle_left list_row))
	(define f (car list_row))
	(define s (car (cdr list_row)))
	(define thrd (car (cdr (cdr list_row))))
	(define fth (car (cdr (cdr (cdr list_row)))))
	(cond 
		[(equal?  f s) 
			(cond
				[(equal? thrd fth)   (list (+ f s) (+ thrd fth) 0 0)]
				[else                (list (+ f s) thrd fth 0)])]
		[(equal? s thrd)	         (list f (+ s thrd) fth 0)]
		[(equal? thrd fth)           (list f s (+ thrd fth) 0)]
		[else                        list_row]))
	    				

(define (row_right list_row)
	(set! list_row (shuffle_right list_row))
	(define f (car list_row))
	(define s (car (cdr list_row)))
	(define thrd (car (cdr (cdr list_row))))
	(define fth (car (cdr (cdr (cdr list_row)))))
	(cond
		[(equal? thrd fth)
			(cond
				[(equal? f s)	   (list 0 0 (+ f s) (+ thrd fth))]
				[else              (list 0 f s (+ thrd fth))])]
		[(equal? s thrd)           (list 0 f (+ s thrd) fth)]
		[(equal? f s)              (list 0 (+ f s) thrd fth)]
                [else                      list_row]))

	

(define (move_board_left board)
	(cond
		[(null? board)  '()]
		[else       (cons (row_left (car board)) (move_board_left (cdr board)))]))


(define (move_board_right board)
	(cond 
		[(null? board) '()]
		[else    (cons (row_right (car board)) (move_board_right (cdr board)))]))



(define (move_board_down board)
	(cond 
		[(equal? board '(() () () ())) board]
		[else 
			(begin
				(define f (car (car board)))
				(define s (car (car (cdr board))))
				(define thrd (car (car (cdr (cdr board)))))
				(define fth (car (car (cdr (cdr (cdr board))))))
				(define tp_list (list f s thrd fth)) 
				(set! tp_list (row_right tp_list)) ;;Shuffle right

				(define temp_list (list
					(cdr (car board))
					(cdr (car (cdr board)))
					(cdr (car (cdr (cdr board))))
					(cdr (car (cdr (cdr (cdr board)))))))

				(set! temp_list (move_board_down temp_list))
					
						
				(set! f (car tp_list))
				(set! s (car (cdr tp_list)))
				(set! thrd (car (cdr (cdr tp_list))))
				(set! fth (car (cdr (cdr (cdr tp_list)))))

				(define temp_list2 (list
					(cons f (car temp_list))
                                        (cons s  (car (cdr temp_list)))
					(cons thrd (car (cdr (cdr temp_list))))
					(cons fth (car(cdr (cdr (cdr temp_list)))))))
                                temp_list2)]))



(define (move_board_up board)
  (cond 
		[(equal? board '(() () () ())) board]
		[else 
			(begin
				(define f (car (car board)))
				(define s (car (car (cdr board))))
				(define thrd (car (car (cdr (cdr board)))))
				(define fth (car (car (cdr (cdr (cdr board))))))
				(define tp_list (list f s thrd fth)) 
				(set! tp_list (row_left tp_list)) ;;Shuffle right

				(define temp_list (list
					(cdr (car board))
					(cdr (car (cdr board)))
					(cdr (car (cdr (cdr board))))
					(cdr (car (cdr (cdr (cdr board)))))))

				(set! temp_list (move_board_up temp_list))
					
						
				(set! f (car tp_list))
				(set! s (car (cdr tp_list)))
				(set! thrd (car (cdr (cdr tp_list))))
				(set! fth (car (cdr (cdr (cdr tp_list)))))

				(define temp_list2 (list
					(cons f (car temp_list))
                                        (cons s  (car (cdr temp_list)))
					(cons thrd (car (cdr (cdr temp_list))))
					(cons fth (car(cdr (cdr (cdr temp_list)))))))
                                temp_list2)]))



(define (debug_display list)
  (displayln (car list))
  (displayln (car (cdr list)))
  (displayln (car (cdr (cdr list))))
  (displayln (car (cdr (cdr (cdr list)))))
  (displayln ""))


(define test_board (create_empty_board))
(debug_display test_board)
(set! test_board (place_at_i_j 2 (cons 2 0) test_board 0 0))
(debug_display test_board)
(set! test_board (place_at_i_j 2 (cons 2 2) test_board 0 0))
(set! test_board (place_at_i_j 4 (cons 2 1) test_board 0 0))
(debug_display test_board)
(set! test_board (move_board_right test_board))
(debug_display test_board)
(set! test_board (place_at_i_j 2(cons 2 0) test_board 0 0))
(debug_display test_board)
(set! test_board (move_board_left test_board))
(debug_display test_board)
(set! test_board (move_board_down test_board))
(debug_display test_board)
(set! test_board (place_at_i_j 4(cons 2 0) test_board 0 0))
(set! test_board (move_board_down test_board))
(debug_display test_board)
(set! test_board (move_board_up test_board))
(debug_display test_board)
(set! test_board (place_at_i_j 8 (cons 3 0) test_board 0 0))
(set! test_board (move_board_up test_board))
(debug_display test_board)
(set! test_board (place_at_i_j 16 (cons 3 0) test_board 0 0))
(set! test_board (move_board_down test_board))
(debug_display test_board)
