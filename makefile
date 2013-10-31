all: desugar

desugar: desugar.rkt
		raco exe desugar.rkt

clean:
		rm -rf desugar *.exe

tests = $(shell ls test/*.plot)
tests: racket-tests-compile plot-tests-compile

racket-tests-compile:
		$(foreach test,$(tests),echo test/$(test);)
plot-tests-compile:
