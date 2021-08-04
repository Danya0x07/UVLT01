\ Time managment words.

#require ]B!
#require ]B?
#require ]C!
#require :NVM

\res MCU: STM8S103
\res export TIM4_CR1 TIM4_PSCR TIM4_ARR TIM4_SR TIM4_CNTR
\res export TIM1_PSCRH TIM1_PSCRL TIM1_CNTRH TIM1_CNTRL TIM1_ARRH TIM1_ARRL 
\res export TIM1_CR1 TIM1_SR1 TIM1_IER
\res export INT_TIM1

nvm
variable seconds

:nvm
    savec
    1 seconds +!
    [ 0 TIM1_SR1 0 ]b!
    iret
;nvm INT_TIM1 !

: time-init  ( -- )
    \ Using TIM4 for milliseconds delay
    [ 7 TIM4_PSCR ]c!   \ presc. 128
    [ 124 TIM4_ARR ]c!  \ period 125

    \ and TIM1 for counting seconds.
    [ 15999 $100 / TIM1_PSCRH ]c!  \ Prescaler 16000
    [ 15999 $FF and TIM1_PSCRL ]c!
    [ 1000 $100 / TIM1_ARRH ]c!  \ Period 1000 ms
    [ 1000 $FF and TIM1_ARRL ]c!
    [ 1 TIM1_IER 0 ]b!
    [ 0 TIM1_SR1 0 ]b! ;

: time-start  ( -- )
    \ Start counting seconds.
    [ 1 TIM1_CR1 0 ]b! ;

: time-stop  ( -- )
    \ Stop counting seconds.
    [ 0 TIM1_CR1 0 ]b! 
    [ 0 TIM1_CNTRH ]c!
    [ 0 TIM1_CNTRL ]c! ;

: time-reset
    \ Reset seconds counter. 
    0 seconds ! ;

: ms-wait  ( ms -- )
    \ Delay for <ms> milliseconds.
    [ 0 TIM4_CNTR ]c!
    [ 1 TIM4_CR1 0 ]b!
    1- for
        \ wait for 1 ms.
        begin [ TIM4_SR 0 ]b? until
        [ 0 TIM4_SR 0 ]b!
    next
    [ 0 TIM4_CR1 0 ]b! ;

ram wipe
