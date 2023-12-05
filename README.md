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
