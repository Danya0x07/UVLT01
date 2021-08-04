\ Copyright (C) 2021 Daniel Efimenko
\   github.com/Danya0x07
\
\ The UV automatic lamp program.
\

\ Entry point.
nvm

: startup  ( -- )
    \ Initialize system components and enter main loop.
    time-init
    encoder-init
    buzzer-init
    relay-init
    display-init

    \ Continuously execute current state and go to the next one.
    ['] setup 'current-state !
    begin
        'current-state @ execute
        transit
    again ;

ram    
