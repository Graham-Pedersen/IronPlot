This software is OPEN SOURCE under CRAPL: http://matt.might.net/articles/crapl/

This is the reopository for IronPlot a scheme-like language

This language and compiler are still heavily under construction do not yet provide the ease of access we would like to building and running the project

How it works

.plot file -> (parser) -> parse tree -> (desugaring) -> core scheme parse tree -> (Dynamic Language Runtime conversion) -> .NET intermediate language .exe file

input grammer: 
<prog> ::= <top>*

<top> ::= <def> 
	   |  <exp>
       |  <use>

<def> ::= (define (<var> <var>*) <body>)
       |  (define <var> <exp>)
	   
<use> ::= (using <net_name_space | net_dll_path>)

<exp> ::= <var>
       |  <literal>
       |  <prim>
	   |  <netcall>
	   |  (map <exp> <list>*)
       |  (lambda (<var>*) <body>)
       |  (let ((<var> <exp>)*) <body>)
       |  (letrec ((<var> <exp>)*) <body>) 
	   |  (while <exp> <exp>)
	   |  (displayln <exp>)
       |  (cond (<exp> <exp>)* [(else <exp>)])
       |  (if <exp> <exp> [ <exp> ])
       |  (set! <var> <exp>)
       |  (begin <body>)
       |  (quote <s-exp>)
       |  (quasiquote <qq-exp 1>)
	   |  <list-exp>
	   |  <math-exp>
	   |  <comp-exp>
	   
<list-exp> ::= (list <exp>*)
	   |  (cons <exp> <exp>)
	   |  (car <list>)
	   |  (cdr <list>)

	   
<math-exp> ::=  (+ <exp> <exp>)
	   |  (- <exp> <exp>)
	   |  (* <exp> <exp>)
	   |  (/ <exp> <exp>)
	   |  (% <exp> <exp>)
	   
<comp-exp>   ::= (equals <exp> <exp>)
	   |  (< <exp> <exp>)
	   |  (<= <exp> <exp>)
	   |  (> <exp> <exp>)
	   |  (>= <exp> <exp>)
	   |  (and <exp>*)
       |  (or <exp>*)
	   
	   
	   
<netcall> ::=  (new <nettype> [<typelist>] exp*) ;; this represent a constructor to a .net OBJCET if a typelist is included it calls a generic constructor
		  ::=  (call <obj> <method_name> exp*) ;; represents a call to a .net object with exp* arguments
          ::=  (scall <nettype> <method_name> exp*) ;; represents a static .net call with exp* arguments
		
<typelist> ::= (typelist <nettype>*



To compile you need a copy of racket to compile the desugarer located in the root directory
You will also need a c# compiler (provided by .net or by MONO on linux) to compile the compile project DLR_Compiler

Invoking the compiler: If you are using windows you can use our Visual Studio plugin that allows you to produce IronRacket projects and provides intelesense and syntax highlighting by compiling that project 
To invoke it manually on linux systems you should use:
cat filename.plot | desugar > output.tmp  
compiler.exe {output filename} {run | compile} {output file name}

As we use this more we will probably make a script or change the input/output to make this easier to chain together on unix systems

Creating .dll librarys: This feature has not been implimented yet sorry!


Examples!:
