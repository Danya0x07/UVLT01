\ Time managment words. Using TIM4 to count milliseconds.

#require :NVM
#require ]B!
#require WIPE

NVM
#require >
RAM

\res MCU: STM8S103
\res export TIM4_CR1 TIM4_IER TIM4_PSCR TIM4_ARR TIM4_SR
\res export INT_TIM4

NVM
variable millis

:NVM
	SAVEC
	1 millis +!
	[ 0 TIM4_SR 0 ]B!	\ clear interrupt pending bit
	IRET
;NVM INT_TIM4 !

: time-init  ( -- )
	7 TIM4_PSCR C!   \ presc. 128
	124 TIM4_ARR C!  \ period 125
	[ 0 TIM4_SR 0 ]B!	\ clear flag
	[ 1 TIM4_IER 0 ]B! 	\ and enable interrupt
	;

: time-start  ( -- )
	\ Start counting milliseconds from 0.
	0 millis !
	[ 1 TIM4_CR1 0 ]B! ;

: time-stop  ( -- )
	\ Stop counting milliseconds.
	[ 0 TIM4_CR1 0 ]B! ;

: ms-passed?  ( ms -- ? )
	\ Indicates that at least <ms> have passed.
	millis @ > not ;

: ms-wait  ( ms -- )
	time-start
	begin dup ms-passed? until
	drop time-stop ;

RAM WIPE
