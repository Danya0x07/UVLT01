\ Time managment words.

#require ]B!
#require ]B?
#require ]C!

\res MCU: STM8S103
\res export TIM4_CR1 TIM4_PSCR TIM4_ARR TIM4_SR TIM4_CNTR
\res export TIM1_PSCRH TIM1_PSCRL TIM1_CNTRH TIM1_CNTRL TIM1_CR1

nvm

: time-init  ( -- )
    \ Using TIM4 for milliseconds delay
    [ 7 TIM4_PSCR ]c!   \ presc. 128
    [ 124 TIM4_ARR ]c!  \ period 125

    \ and TIM1 for counting milliseconds.
    [ 15999 $100 / TIM1_PSCRH ]c!  \ Prescaler 16000
    [ 15999 $FF and TIM1_PSCRL ]c! ;

: time-start  ( -- )
    \ Start counting milliseconds.
    [ 1 TIM1_CR1 0 ]b! ;

: time-stop  ( -- )
    \ Stop counting milliseconds.
    [ 0 TIM1_CR1 0 ]b! ;

: time-reset
    \ Reset milliseconds counter.
    [ 0 TIM1_CNTRH ]c!
    [ 0 TIM1_CNTRL ]c! ;

: millis@
    \ Get value of milliseconds counter.
    TIM1_CNTRH c@ 8 lshift TIM1_CNTRL c@ or ;

: ms-wait  ( ms -- )
    \ Delay for <ms> milliseconds.
    [ 0 TIM4_CNTR ]c!
    [ 1 TIM4_CR1 0 ]b!
    1- for
        \ wait for 1 ms.
        begin [ TIM4_SR 0 ]b? until
        [ 0 TIM4_SR 0 ]b!
    next
    [ 0 TIM1_CR1 0 ]b! ;

: ms>seconds  ( ms -- seconds )
    \ Convert milliseconds value to seconds
    1000 / ;

ram wipe
