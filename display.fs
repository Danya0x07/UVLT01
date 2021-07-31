\ Display lexicon (TM1637).

#require ]B!
#require ]B?
#require LSHIFT

\res MCU: STM8S103
\res export PA_DDR PA_ODR PA_IDR

2 constant _dp.CLK
3 constant _dp.DIO

\ The segment map.
\     A
\    ---
\ F |   | B
\    -G-
\ E |   | C
\    ---
\     D
\ For the 2nd (counting from left, [12:34]) digit of the display
\ the X bit controls the dots.

nvm
create _dp.digits  \ Array for converting numbers to TM1637 digits.
\    XGFEDCBA
    %00111111 c,  \ 0
    %00000110 c,  \ 1
    %01011011 c,  \ 2
    %01001111 c,  \ 3
    %01100110 c,  \ 4
    %01101101 c,  \ 5
    %01111101 c,  \ 6
    %00000111 c,  \ 7
    %01111111 c,  \ 8
    %01101111 c,  \ 9

\ Some other symbol codes.
%00000000 constant dp.NONE
%10000000 constant dp.:
%01000000 constant dp.-
%00111001 constant dp.[
%00001111 constant dp.]
ram

\ Command sets
1 6 lshift constant _dp.DATA
2 6 lshift constant _dp.DISPLAY
3 6 lshift constant _dp.ADDRESS

\ Display switch settings bit shift.
3 constant _dp.DSS

nvm
\ Buffer for number to symbol codes conversion.
\ (create doesn't work properly in NVM for now, so using
\ variable instead)
variable _dp.numbuff 2 allot

\ Pin manipulation words.
: _clk-low   ( -- )   [ 0 PA_ODR _dp.CLK ]b! ;
: _clk-high  ( -- )   [ 1 PA_ODR _dp.CLK ]b! ;
: _dio-low   ( -- )   [ 0 PA_ODR _dp.DIO ]b! ;
: _dio-high  ( -- )   [ 1 PA_ODR _dp.DIO ]b! ;
: _dio-high? ( -- ? )   [ PA_IDR _dp.DIO ]b? ;

\ Pseudo I2C operatoins.
: _bit-delay  ( -- )  1 ms-wait ;

: _tr-start  ( -- )
    _clk-high _dio-high
    _bit-delay
    _dio-low ;

: _tr-stop  ( -- )
    _clk-low _bit-delay
    _dio-low _bit-delay
    _clk-high _bit-delay
    _dio-high ;

: _check-ack  ( -- )
    _clk-low
    _bit-delay
    _dio-high
    _dio-high? not if
        _dio-low then
    _bit-delay
    _clk-high _bit-delay 
    _clk-low ;

\ Display memory managment.
: _write-byte  ( b -- )
    7 for
        _clk-low
        dup 1 and  PA_ODR _dp.DIO b!
        _bit-delay
        ( b ) 2/
        _clk-high
        _bit-delay
    next drop
    _check-ack ;

: _write-cmd  ( b -- )
    _tr-start _write-byte _tr-stop ;

\ Display control words.

: _brightness!  ( n -- )
    \ 0 <= n <= 7
    _dp.DISPLAY or  
    [ 1 _dp.DSS lshift ] literal or
    _write-cmd ;

: display-string  ( a -- )
    \ Display a string of 4 symbol codes starting at <a> address.
    _dp.DATA _write-cmd  \ cursor auto increment
    _tr-start
    _dp.ADDRESS _write-byte
    3 for
        dup c@ _write-byte
        1+
    next drop
    _tr-stop ;

: display-number  ( n dots? -- )
    \ Display decimal number 0..9999
    swap  \ Hide dots flag.
    3 for 
        10 /mod ( digit n/10 ) 
    next drop ( d0 d1 d2 d3 )
    _dp.numbuff
    3 for  ( d3 addr )
        dup rot 
        ( addr addr digit ) _dp.digits + c@  \ digit>code
        swap c!
        ( addr ) 1+
    next drop
    
    ( flag ) if  \ Add dots if needed
        [ 1 _dp.numbuff 1+ ( 1 2nd-seg-addr ) 7 ]b! then
    _dp.numbuff display-string ;

: display-clear  ( -- )
    \ Clear the display segments.
    _dp.numbuff 4 erase 
    _dp.numbuff display-string ;

: display-init  ( -- )
    \ Configure GPIOs in open-drain mode
    [ 1 PA_DDR _dp.CLK ]b!
    [ 1 PA_DDR _dp.DIO ]b!
    
    _clk-high _dio-high
    display-clear
    7 _brightness! ;

ram
