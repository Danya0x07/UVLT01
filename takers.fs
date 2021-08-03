\ User values/options taking routine.

nvm

variable previous  \ Buffer for last user value got (for lazy display updates).
: previous!  ( data -- )    previous ! ;

: data-new?!  ( data -- data was-new? )   
    dup previous @ <> ( data flag )
    over previous! ;

\ Buffer for parsing number into digits when setting duration.
\ ("create" isn't working properly in NVM for now, so using "variable" instead)
variable digit-buffer 2 allot
\ Digits are counted from left to right i.e. in 1234 the <3> is 1-st digit, <4> is 0-th.
: digit@  ( i -- ith-digit )    digit-buffer + c@ ;
: digit!  ( c i -- )    digit-buffer + c! ;


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

ram
