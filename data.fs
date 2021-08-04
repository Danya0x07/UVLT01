\ Data used by other components.

nvm
\ Helper constants.
0 constant NO-DOTS
-1 constant WITH-DOTS

\ Labels for displaying in different states.
create label.DASHES dp.- c,  dp.- c,  dp.- c,  dp.- c,
create label.BACK   dp.] c,  dp.NONE c,  dp.NONE c, dp.[ c,
create label.DONE   dp.d c,  dp.o c,  dp.n c,  dp.E c,
create label.BAD  dp.- c,  dp.b c,  dp.A c,  dp.d c,

variable duration  \ Lighting duration in seconds

: seconds-left  ( -- left )
    \ Get time left in seconds.
    duration @  seconds @  - ;

: elapsed?  ( ms-left -- flag)
    \ Indicate seconds are elapsed
    0> not ;

ram
