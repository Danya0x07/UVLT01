\ State machine implementation.

nvm
\ State IDs.
0 constant state.SETUP
1 constant state.DIRECT
2 constant state.CONFIRM
3 constant state.COUNTDOWN
4 constant state.PAUSE
5 constant state.FINISH

: setup  ( -- nxstate )
    \ Refresh the system, get user input and go further.
    led-blink
    duration @ to-printable
    take-number from-printable
    led-noblink
    dup 0= if
        drop  state.DIRECT
    else
        duration !  state.CONFIRM
    then ;

: direct-control  ( -- nxstate )
    \ Enter direct control mode, stay here until get doublepress.
    label.DASHES display-string
    begin
        0
        button-pressed? if
            pressed-again? if
                drop -1
            else
                relay-toggle
                300 ms-wait
            then
        then
        10 ms-wait
    until
    relay-off
    state.SETUP
    2 100 buzz ;

: confirm  ( -- nxstate )
    \ This state allows user to start countdown procedure or return to SETUP.
    led-on
    ['] show-duration take-option
    led-off
    0= if
        2 100 buzz
        state.SETUP
    else
        state.COUNTDOWN
    then ;

ram
