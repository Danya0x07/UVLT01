\ Builtin LED control lexicon.
nvm
#require [']

: led-on  ( -- )    1 OUT! ;
: led-off  ( -- )   0 OUT! ;

: _led-update  ( -- )   $80 TIM over and = OUT! ;
: led-blink  ( -- )     ['] _led-update BG ! ;
: led-noblink  ( -- )   0 BG !  led-off ;

ram
