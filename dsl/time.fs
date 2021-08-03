\ Time managment words. Using TIM4 to count milliseconds.

nvm
#require >
ram

#require :NVM
#require ]B!

\res MCU: STM8S103
\res export TIM4_CR1 TIM4_IER TIM4_PSCR TIM4_ARR TIM4_SR
\res export INT_TIM4

nvm
variable millis
variable ms.backup

:nvm
    savec
    1 millis +!
    [ 0 TIM4_SR 0 ]b!   \ clear interrupt pending bit
    iret
;nvm INT_TIM4 !

: time-init  ( -- )
    7 TIM4_PSCR c!   \ presc. 128
    124 TIM4_ARR c!  \ period 125
    [ 0 TIM4_SR 0 ]b!   \ clear flag
    [ 1 TIM4_IER 0 ]b!  \ and enable interrupt
    ;

: time-start  ( -- )
    \ Start counting milliseconds.
    [ 1 TIM4_CR1 0 ]b! ;

: time-stop  ( -- )
    \ Stop counting milliseconds.
    [ 0 TIM4_CR1 0 ]b! ;

: time-store  ( -- )
    \ Backup milliseconds counter.
    millis @ ms.backup ! ;

: time-restore  ( -- )
    \ Restore milliseconds counter from backup.
    ms.backup @ millis ! ;

: ms-passed?  ( ms -- ? )
    \ Indicates that at least <ms> have passed.
    millis @ > not ;

: ms-wait  ( ms -- )
    0 millis !
    time-start
    begin dup ms-passed? until
    drop time-stop ;

: ms>seconds  ( ms -- seconds )
    \ Convert milliseconds value to seconds
    1000 / ;

ram
