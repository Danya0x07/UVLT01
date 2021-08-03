\ Relay lexicon.

#require ]B!
#require LSHIFT

\res MCU: STM8S103
\res export PD_DDR PD_CR1 PD_ODR

3 constant _rel.PIN

nvm

: relay-on  ( -- )
    [ 1 PD_ODR _rel.PIN ]b! ;

: relay-off  ( -- )
    [ 0 PD_ODR _rel.PIN ]b! ;

: relay-init  ( -- )
    [ 1 PD_DDR _rel.PIN ]b!
    [ 1 PD_CR1 _rel.PIN ]b!
    relay-off ;

: relay-toggle  ( -- )
    [ 1 _rel.PIN lshift ] literal PD_ODR c@ xor
    PD_ODR c! ;

ram wipe
