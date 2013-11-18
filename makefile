all: desugar matt-desugar

desugar: desugar.rkt
		raco exe desugar.rkt

matt_desugar: matt_desugar.rkt
		raco exe matt_desugar.rkt

clean:
		rm -rf desugar *.exe

test_desugar_r: racket_tests_compile run_r_tests_desugar
test_desugar_p: plot_tests_compile run_p_tests_desugar

desugar-tests: desugar racket_tests_desugar plot_tests_desugar


tests_racket = $(shell ls test/*.rkt)
racket_tests_desugar:
		$(foreach test,$(tests_racket),raco exe $(test);)


tests_plot = $(shell ls test/*.plot)
plot_tests_desugar:
		$(foreach test,$(tests_plot),cat $(test) | ./desugar > $(test).exe;)


tests_c_racket = $(shell ls --ignore=*.* test/)
run_r_tests_desugar:
		$(foreach test,$(tests_c_racket),./test/$(test);)


run_p_tests_desugar: convert-plot-to-rkt compile-plot-rkt
		$(foreach test,$(tests_c_plot),./$(test);)

tests_ds_plot = $(shell ls test/*.exe)

convert_plot_to_rkt:
		$(foreach test,$(tests_ds_plot),sed -i '$a #lang racket/base' $(test);)

compile_plot_rkt: