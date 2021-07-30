\ Buzzer lexicon.

#require ]B!

\res MCU: STM8S103
\res export PD_DDR PD_CR1 PD_ODR

2 constant BUZZER_PIN

NVM

: buzzer-on  ( -- )
    [ 1 PD_ODR BUZZER_PIN ]B! ;

: buzzer-off  ( -- )
    [ 0 PD_ODR BUZZER_PIN ]B! ;

: buzzer-init  ( -- )
    [ 1 PD_DDR BUZZER_PIN ]B!
    [ 1 PD_CR1 BUZZER_PIN ]B!
    buzzer-off ;

: buzz  ( times duration -- )
    swap 1- for
        buzzer-on
        dup ms-wait
        buzzer-off
        dup 2/ ms-wait
    next
    drop ;

RAM
