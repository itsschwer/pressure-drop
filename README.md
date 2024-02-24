# pressure drop

A \[ server-side / host-only \] mod that adds configurable timed pressure plates *(Abandoned Aqueduct)* and a `/drop` item *(`/drop <itemnamenospaces> [@]`)* chat command.

## why?

I wanted to make activating the pressure plates more accessible *(no more waiting for friends / pushing pots)*.

---

sometimes I \[ take the wrong item / want to help others catch up \].

## balance

~~probably reduces some pressure~~

---

- how long a pressure plate stays pressed can be configured as you see fit *(default: 30s)*

> timed pressure plate *(config: 3s)*
![gif demonstration of timed pressure plate](https://github.com/itsschwer/pressure-drop/blob/main/xtra/demo-pressure-plate-timed.gif?raw=true)

---

~~strategic! a typing challenge!~~

- sacrifice more time than at a Scrapper to reduce the risk of a Printer or Cauldron taking the wrong item
- more opportunities to use the Recycler
- leave items for your allies:
    - to help distribute the wealth
    - to help them win against the Teleporter boss *(you died)*
- can remove items you are not having fun with
- can spread void items between players

> `/drop` item command
![gif demonstration of /drop item command](https://github.com/itsschwer/pressure-drop/blob/main/xtra/demo-drop-item.gif?raw=true)

## configurable

> `/reload` can be used to apply changes made to the configuration file without needing to restart the game *(must be hosting a run to use)*

---

- how long a pressure plate stays pressed can be configured as you see fit *(default: 30s)*
    | value | behaviour |
    |  ---: | :---      |
    |     0 | disable *(vanilla)* |
    | *(any negative value)* | pressure plates stay down forever once pressed
    | *(any positive value)* | pressure plates stay down for the specified number of seconds

---

- the drop command can be disabled
- the ability to send items to the Teleporter can be disabled
- the ability for dead players to drop items can be disabled
- the ability to drop void items can be enabled
- which item tiers should be recyclable when dropped can be configured as you see fit *(default: only white and green)*

## extra

- the option to show chat history when the scoreboard is open can be enabled
- the option to change void items to always require confirmation before being picked up can be enabled
    - intended to encourage sharing void items in multiplayer when the ability to drop void items is disabled
- the option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated *(rather than on entry)* can be enabled
    - intended to make it easier to find and obtain the *Mysterious Cuff / Condemned Bond* from [**Starstorm 2**](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)

## see also

- [DropItem](https://thunderstore.io/package/Thrayonlosa/DropItem/) by [Thrayonlosa](https://thunderstore.io/package/Thrayonlosa/) — similar `/drop` functionality, inspired this implementation
    - can drop equipment
    - can drop from another player's inventory
    - only drops around the executing player *(must be alive)*
    - not configurable *(e.g. max drop stack size, recyclability)*
    - drops \[ *consumed* / *broken* \] items
    - drops all shoot in same direction — scatters messily once landed
        - drop direction is fixed *(rather than using aim direction)*

## developers

If you'd like to implement your own chat commands, you can include this plugin as a dependency and register your commands using `ChatCommander` — please refer to [this guide](https://github.com/itsschwer/pressure-drop/blob/main/xtra/developers.md)!

## end
- **a tip**: `/d .` can be used to drop the newest item type in your inventory — experiment!
