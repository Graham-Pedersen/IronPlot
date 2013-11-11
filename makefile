all: desugar

desugar: desugar.rkt
		raco exe desugar.rkt

clean:
		rm -rf desugar *.exe

tests_racket = $(shell ls test/*.rkt)
tests_plot = $(shell ls test/*.plot)

tests: racket-tests-compile plot-tests-compile

racket-tests-compile:
		$(foreach test,$(tests_racket),raco exe $(test);)
plot-tests-compile:
		$(foreach test,$(tests_plot),cat $(test) | ./desugar > $(test).exe;)