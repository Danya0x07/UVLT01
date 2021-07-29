\ Encoder and button lexicon.

NVM
#require >
#require <>
RAM

#require :NVM
#require ]B!
#require ]B?
#require WIPE

#require time.fs

\res MCU: STM8S103
\res export PC_IDR PC_CR1 PC_CR2
\res export EXTI_CR1
\res export INT_EXTI2

\ Pin definitions.
7 constant ENCODER_BTN
6 constant ENCODER_CLK
5 constant ENCODER_DT

NVM
variable _enc.min
variable _enc.max
variable _enc.curr
variable _?btn.prev

: encoder-init  ( -- )
	\ Ininialize GPIOs of encoder and its button.
	[ 1 PC_CR1 ENCODER_BTN ]B!  \ internal pullup on
	[ $9B C, ]    \ ASM("SIM") - disable interrupts
	$30 EXTI_CR1 C!  \ PortC interrupt on
	[ $9A C, ] ;  \ ASM("RIM") - enable interrupts

: encoder-irq!  ( 1|0 -- )
	\ Manage encoder interrupt
	PC_CR2 ENCODER_CLK B! ;

\ Relationship between encoder pin statuses and its steps
\ looks like that: (high: -1, low: 0)
\
\			CLK
\         /     \ 
\		 -1		 0
\		 DT		DT
\       /  \   /  \
\	   -1   0 -1   0
\      |    | |    |
\      -1  +1 +1   -1
\
\ So the formula can be: -(CLK xor DT), if 0 then -1 .

: _enc.calc-step  ( CLK DT -- step)
	\ Calculate encoder step (+1 | -1) from its pins statuses
	xor negate 
	dup 0= if drop -1 then ;

: _enc.constrain  ( updated -- constrained)
	\ Constrain encoder current value according to bounds.
	dup _enc.max @ > if drop _enc.max @ then
	dup _enc.min @ < if drop _enc.min @ then ;

:NVM  \ Encoder ISR.
	SAVEC
	0 encoder-irq!
	
	[ PC_IDR ENCODER_CLK ]B?  \ get CLK
	[ PC_IDR ENCODER_DT ]B?   \ get DT
	_enc.calc-step 	_enc.curr @  +  \ this will be new encoder value
	\ Check it and then write.
	_enc.constrain	_enc.curr !
	
	1 encoder-irq!
	IRET
;NVM INT_EXTI2 !

: encoder-set  ( min max curr -- )
	\ Set encoder bounds and current value.
	_enc.curr !  _enc.max !  _enc.min ! ;

: encoder-get  ( -- curr )
	\ Get current encoder value.
	_enc.curr @ ;

: button-is-down?  ( -- ? )
	\ Check if button is being pressed now.
	[ PC_IDR ENCODER_BTN ]B? not ;

: button-pressed?  ( -- ? )
	\ Check if there was button pressed after it was released.
	_?btn.prev @  button-is-down?  2dup <> if
		5 ms-wait
		drop
		button-is-down?
	then  ( prev new )
    dup rot not and  ( new ? )
    swap _?btn.prev ! ;

RAM
