(define assembly_info ", System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")

(define cons_qual (lambda (name) 
		(scall System.String Concat name assembly_info)))

(define do_setup (lambda ()
	(define sname (cons_qual "System.Net.Sockets.Socket"))
	(define ipHostInfo
			(scall (cons_qual "System.Net.Dns") GetHostEntry
				(scall (cons_qual "System.Net.Dns") GetHostName)))
	(displayln "input the ip of the host you want to connect to")
	(define ip (scall (cons_qual "System.Net.IPAddress") Parse (scall System.Console ReadLine)))
	(define port 11000)
	(displayln  (cons '(connecting) (cons ip (cons '(and port) (cons port '())))))
	(define family (call ip AddressFamily))
	(define stype (scall (cons_qual "System.Net.Sockets.SocketType") Stream))
	(define protocol (scall (cons_qual "System.Net.Sockets.ProtocolType") Tcp))
	(define endpoint (new (cons_qual "System.Net.IPEndPoint") ip port))
	(define sock (new sname family stype protocol))
	(call sock Connect endpoint)
	(call sock Blocking set #f)
	(displayln "made connection")
	sock))
	
(define grabAllInput 
	(lambda ()
		(define return "")
		(define key "")
		(define char "")
		(while (scall System.Console KeyAvailable) 
			(begin
				(set! key (scall System.Console ReadKey #t))
				(set! char (call key KeyChar))
				(set! return (scall System.String Concat return char))))
		return))

(define check_sock (lambda () '(not done)))
(define send_message (lambda () '(not done)))
(define thread_sleep (lambda () 
	'hello))
	
(define workloop 
	(lambda ()
		(define message "")
		(while #t (begin
			(check_sock)
			(set! message (grabAllInput))
			(if (equal? "" message)
				(thread_sleep)
				(displayln message))))))
	
(define server_con (do_setup))
(displayln (workloop))
	

	