\ State machine implementation.

\ State IDs.
0 constant state.SETUP
1 constant state.DIRECT
2 constant state.CONFIRM
3 constant state.COUNTDOWN
4 constant state.PAUSE
5 constant state.FINISH

\ Helper constants.
0 constant NO-DOTS
-1 constant WITH-DOTS

nvm
\ Labels for displaying in different states.
create label.DASHES dp.- c,  dp.- c,  dp.- c,  dp.- c,
create label.BACK   dp.] c,  dp.NONE c,  dp.NONE c, dp.[ c,
create label.DONE   dp.d c,  dp.o c,  dp.n c,  dp.E c,

\ Variables
variable duration  \ Lighting duration in seconds
variable previous  \ Buffer for last user value got (for lazy display updates).
\ Buffer for parsing number into digits when setting duration.
\ ("create" isn't working properly in NVM for now, so using "variable" instead)
variable digit-buffer 2 allot

\ Digit buffer R/W shortcuts. Digits are counted from left to right
\ i.e. in 1234 the <3> is 1-st digit, <4> is 0-th.
: digit@  ( i -- ith-digit )    digit-buffer + c@ ;
: digit!  ( c i -- )    digit-buffer + c! ;

\ User value update handlers.
: previous!  ( data -- )    previous ! ;

: data-new?!  ( data -- data was-new? )   
    dup previous @ <> ( data flag )
    over previous! ;

\ Digit managment words.
: parse-digits  ( n -- )
    \ Parse digits of n (0..9999) into digit-buffer.
    4 0 do
        10 /mod swap  i digit!
    loop drop ;

: build-number  ( -- n )
    \ Build number from 4 digits in digit-buffer.
    3 digit@ 1000 *
    2 digit@ 100 *
    1 digit@ 10 *
    0 digit@ + + + ;

\ Time data conversion words.
: to-printable  ( seconds -- printable )
    \ Convert <seconds> value to printable format.
    60 /mod  100 *  + ;

: from-printable  ( printable -- seconds )
    \ Convert <printable> number (0..9999) to seconds
    \ i.e 130 --> 1 min 30 seconds so 90 seconds
    100 /mod  60 *  + ;

: millis-left  ( -- ms-left )
    \ Get time left in milliseconds.
    duration @ 1000 *  millis @  - ;

\ User values takers.
: take-digit  ( pos max -- )
    \ Take user digit 0..<max> and store it on position <pos> in digit-buffer.
    over digit@  ( pos max dig )
    0 rot rot  ( pos 0 max dig )
    encoder-set
    1 encoder-irq!
    begin button-pressed? not while
        ( pos )
        encoder-get data-new?! if
            ( pos curr ) over digit!
            build-number WITH-DOTS display-number
        else drop then ( pos )
        20 ms-wait
    repeat drop
    0 encoder-irq! ;

: take-number  ( prev -- new )
    \ Take user number editing digits of <prev>.
    parse-digits
    9 5 9 9  \ limits for time value [99:59]
    3 for
        i swap take-digit
        1 30 buzz
    next
    build-number ;

: take-option  ( 'displayer -- option )
    \ Take user binary choice(option): 1 or 0.
    
    \ When user through encoder selects option 0, the label.BACK is displayed, 
    \ and when on position 1, word <displayer> is executed.
    0 1 1 encoder-set
    0 previous!
    1 encoder-irq!
    begin button-pressed? not while
        encoder-get data-new?! if
            ( 'displayer option ) 0= if
                label.BACK display-string
            else
                dup execute
            then
        else drop then
        10 ms-wait
    repeat drop
    0 encoder-irq!
    encoder-get ( option ) ;

\ Time showing callbacks.
: show-duration  ( -- )
    \ Callback for showing user configured lighting duration
    \ in CONFITM state.
    duration @ to-printable WITH-DOTS display-number ;

: show-time-backup  ( -- )
    \ Callback for showing seconds left in PAUSE state.
    ms.backup @ ms>seconds to-printable NO-DOTS display-number ;

\ State processor words.
: setup  ( -- nxstate )
    \ SETUP state processor word.
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
    \ DIRECT state processor word.
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
    \ CONFIRM state processor word.
    \ This state allows user to start countdown procedure
    \ or return to SETUP.
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
