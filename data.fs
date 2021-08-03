\ Data used by other components.

nvm
\ Helper constants.
0 constant NO-DOTS
-1 constant WITH-DOTS

\ Labels for displaying in different states.
create label.DASHES dp.- c,  dp.- c,  dp.- c,  dp.- c,
create label.BACK   dp.] c,  dp.NONE c,  dp.NONE c, dp.[ c,
create label.DONE   dp.d c,  dp.o c,  dp.n c,  dp.E c,

variable duration  \ Lighting duration in seconds

: millis-left  ( -- ms-left )
    \ Get time left in milliseconds.
    duration @ 1000 *  millis@  - ;

ram
