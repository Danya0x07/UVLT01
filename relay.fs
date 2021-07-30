\ Relay lexicon.

#require ]B!
#require LSHIFT

\res MCU: STM8S103
\res export PD_DDR PD_CR1 PD_ODR

3 constant RELAY_PIN

NVM

: relay-on  ( -- )
    [ 1 PD_ODR RELAY_PIN ]B! ;

: relay-off  ( -- )
    [ 0 PD_ODR RELAY_PIN ]B! ;

: relay-init  ( -- )
    [ 1 PD_DDR RELAY_PIN ]B!
    [ 1 PD_CR1 RELAY_PIN ]B!
    relay-off ;

: relay-toggle  ( -- )
    [ 1 RELAY_PIN LSHIFT ] LITERAL PD_ODR C@ xor
    PD_ODR C! ;

RAM
