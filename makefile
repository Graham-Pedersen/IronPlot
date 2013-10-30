all: desugar reader

desugar:
		raco exe desugar.rkt

reader:
		raco exe reader.rkt

clean:
		rm -rf reader desugar *.exe

tests = $(shell ls test/*.plot)
tests: racket-tests-compile plot-tests-compile

racket-tests-compile:
		$(foreach test,$(tests),echo test/$(test);)
plot-tests-compile:
