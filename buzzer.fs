\ Buzzer lexicon.

#require ]B!

\res MCU: STM8S103
\res export PD_DDR PD_CR1 PD_ODR

2 constant _buz.PIN

nvm

: buzzer-on  ( -- )
    [ 1 PD_ODR _buz.PIN ]b! ;

: buzzer-off  ( -- )
    [ 0 PD_ODR _buz.PIN ]b! ;

: buzzer-init  ( -- )
    [ 1 PD_DDR _buz.PIN ]b!
    [ 1 PD_CR1 _buz.PIN ]b!
    buzzer-off ;

: buzz  ( times duration -- )
    swap 1- for
        buzzer-on
        dup ms-wait
        buzzer-off
        dup 2/ ms-wait
    next drop ;

ram
