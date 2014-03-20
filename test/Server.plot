(define assembly_info ", System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
(define list "System.Collections.Generic.List")

(define do_setup (lambda ()
	(define cons_qual (lambda (name method) 
			(scall System.String.Concat name assembly_info method)))

	(define sname (cons_qual "System.Net.Sockets.Socket" ""))
			
	(define buffer (new list (typelist System.Byte)))
	(define socks (new list (typelist sname)))

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
	(displayln  (cons '(starting on) (cons ip '())))
	(define family (scall (cons_qual "System.Net.Sockets.AddressFamily" ".InterNetwork")))
	(define stype (scall (cons_qual "System.Net.Sockets.SocketType" ".Stream")))
	(define protocol (scall (cons_qual "System.Net.Sockets.ProtocolType" ".Tcp")))
	(define endpoint (new (cons_qual "System.Net.IPEndPoint" "") ip 11000))
	(define server_sock (new sname family stype protocol))
	(call server_sock Bind endpoint)
	(call server_socket Listen 10)
	(displayln "server started...")
	server_sock))

(define server_sock (do_setup))



