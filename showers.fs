\ Callbacks for showing time in different states.

nvm
\ Time data conversion words.
: to-printable  ( seconds -- printable )
    \ Convert <seconds> value to printable format.
    60 /mod  100 *  + ;

: from-printable  ( printable -- seconds )
    \ Convert <printable> number (0..9999) to seconds
    \ i.e 130 --> 1 min 30 seconds so 90 seconds
    100 /mod  60 *  + ;

\ Time showing callbacks.
: show-duration  ( -- )
    \ Callback for showing user configured lighting duration
    \ in CONFITM state.
    duration @ to-printable WITH-DOTS display-number ;

: show-time-left  ( -- )
    \ Callback for showing seconds left in PAUSE state.
    millis@ ms>seconds to-printable NO-DOTS display-number ;

ram
