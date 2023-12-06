# pressure drop

A [ server-side / host-only ] mod that adds configurable timed pressure plates *(Abandoned Aqueduct)*.

## why?

I wanted to make activating the pressure plates more accessible *(no more waiting for friends / pushing pots)*.

## balance

~~probably reduces some pressure~~

- How long a pressure plate stays pressed is configurable *(`pressurePlateTimer`)* as you see fit
    | value | behaviour |
    |  ---: | :---      |
    |     0 | disable *(vanilla)* |
    | *(any negative value)* | pressure plates stay down forever once pressed
    | *(any positive value)* | pressure plates stay down for the specified number of seconds

## issues

I don't know how this mod behaves when installed by both host and client (or just non-host).

# temp: todo: drop

- drop command: \<item\> \[@tp\]
    - except captain special and untiered/consumed
    - scrapper-like, but place in ring around player
        - if player dead (or arg specified), do at teleporter instead
            - if no tp (hidden realm, moon), display message (no abusing timer, good luck o7)
        - track transform + last position if nulled
- steal command: \<user\> \<item\> \[@tp\]
    - same as drop, but uses specified user's inventory instead
- configs
    - can drop
        - can drop when dead
        - can drop at tp
    - can recycle drop
        - red: false
        - yellow: false
        - lunar: false
        - white: true
        - green: true
        - void (all): false
        - equipment: true
        - lunar equipment: false
