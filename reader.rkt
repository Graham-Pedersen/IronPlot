; TODO after compiling remove raket dependences
#lang racket

; simply reads a file and pipes it as input to the next stage 
; seperated out so it can be modified 

; common library needs 
; 	 file open
;	 ability to read command line args
;	 ability to output to stdout 
; 	 by the time we target windows we will need the ability to import racket files (to have them all call each other without pipes)

(require racket/cmdline)
(define (get-file-path)
    (command-line
        #:args (filename)
        filename))

(define (read-lines file)
    (define line (read file))
    (if (eof-object? line)
        '()
        (cons line (read-lines file))))

(define (read-input-file path)
    (define file (open-input-file path))
    (write  (read-lines file)))


(read-input-file (get-file-path))