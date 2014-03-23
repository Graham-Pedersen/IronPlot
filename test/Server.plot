(define assembly_info ", System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
(define list "System.Collections.Generic.List")


(define cons_qual (lambda (name method) 
		(scall System.String.Concat name assembly_info method)))

(define do_setup (lambda ()
	(define sname (cons_qual "System.Net.Sockets.Socket" ""))	
	(define get_input (lambda () (call '() System.Console.ReadLine)))
	(define ipHostInfo
			(scall (cons_qual "System.Net.Dns" ".GetHostEntry")
				(scall (cons_qual "System.Net.Dns" ".GetHostName"))))
	(define ip 
		(call 
			(new list 
				(typelist (cons_qual "System.Net.IPAddress" ""))
				(call ipHostInfo AddressList))
			Item
			0))
	(define port 11000)
	(displayln  (cons '(starting on) (cons ip (cons '(and port) (cons port '())))))
	(define family (call ip AddressFamily))
	(define stype (scall (cons_qual "System.Net.Sockets.SocketType" ".Stream")))
	(define protocol (scall (cons_qual "System.Net.Sockets.ProtocolType" ".Tcp")))
	(define endpoint (new (cons_qual "System.Net.IPEndPoint" "") ip port))
	(define server_sock (new sname family stype protocol))
	(call server_sock Bind endpoint)
	(call server_sock Blocking set #f)
	(call server_sock Listen 10)
	(displayln "server started...")
	server_sock))
	
(define decode (lambda (bytes) (displayln "got a message who knows what though!")))
(define send (lambda (message) 'hello))

(define get_message 
	(lambda (client) 
		(if (call client Poll 0 (scall (cons_qual "System.Net.Sockets.SelectMode" ".SelectRead")))
			(begin 
				(define count (call client Available))
				(define bytes 
					(scall 
						System.Array 
						CreateInstance 
						(scall System.Type GetType "System.Byte" )
						count))
				(call client Receive bytes)
				(decode bytes))
			'())))
	
(define accept_client 
	(lambda (server) 
		(if (call server Poll 0 (scall (cons_qual "System.Net.Sockets.SelectMode" ".SelectRead")))
			(begin 
				(define newbie (call server Accept))
				(displayln "Client connected!")
				(call newbie Blocking set #f)
				(set! clist (cons newbie clist)))
			'())))

(define work (lambda (clients)
	(if (not (equal? '() clients))
		(begin 
			(get_message (car clients))
			(work (cdr clients)))
		(accept_client server_sock))))
	
(define clist '())
(define server_sock (do_setup))
(while #t (work clist))






