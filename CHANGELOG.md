## 2.1.0
- Move features to [ServerSider](https://thunderstore.io/package/itsschwer/ServerSider/) <sup>[***src***](https://github.com/itsschwer/ror2-serversider)</sup>
    - Configuration option to change void tier pickup rule from `ConfirmFirst` to `ConfirmAll`
- Change visibility of various members from `public` to `internal`
- Code refactoring

### 2.0.1
- Fix changelog for version 2.0.0
    - Document missing removal of pressure plate component
    - Restructure for readability

# 2.0.0
- Move features to [ServerSider](https://thunderstore.io/package/itsschwer/ServerSider/) <sup>[***src***](https://github.com/itsschwer/ror2-serversider)</sup>
    - Configuration option to adjust pressure plates *(Abandoned Aqueduct)* to stay pressed down for a duration
    - Configuration option to send a chat notification listing the items that are consumed when a Scrapper, 3D Printer, Cleansing Pool, or Cauldron is used
        - Configuration option to include Item Scrap in the printed list
    <!--  -->
    - *Note: **PressureDrop** may eventually be completely merged into **ServerSider** one day*
        - *This mod now only implements the chat command for dropping items (and the "void pick up confirm all rule")*
- Move features to [HUDdleUP](https://thunderstore.io/package/itsschwer/HUDdleUP/) <sup>[***src***](https://github.com/itsschwer/ror2-huddle-up)</sup>
    - Configuration option to show chat history when the scoreboard is open

### 1.5.1
- Fix Longstanding Solitude not being matched by the /drop command
    - *Allows Longstanding Solitude to be dropped*

## 1.5.0
- Add configuration option to send a chat notification listing the items that are consumed when a Scrapper, 3D Printer, Cleansing Pool, or Cauldron is used
    - Add a configuration to include Item Scrap in the printed list
- Rework tweak to show chat history when the scoreboard is open
    - Now doesn't reset the fade out progress
    - Change default value of configuration option `scoreboardShowChat` to `true`
- Code refactoring

### 1.4.1
- Start assembly versioning
- Add suggested syntax section to `developers.md`
- \[Dropping items\] Try optimise Devotion Update compatibility
    - *(Cache the reflection method)*

## 1.4.0
- Change default value of configuration option `voidPickupConfirmAll` to `true`
- \[`ChatCommander`\] Remove empty arguments when parsing chat commands
    - *e.g. <span style="white-space:pre;">`/d      kja`</span> is now equivalent to `/d kja`*
- \[Dropping items\] Fix attempt to make compatible with pre-Devotion Update, Devotion Update, and Seekers of the Storm
    - Tested on Devotion Update and Seekers of the Storm patch
    - Untested on pre-Devotion Update

### 1.3.2
- \[Dropping items\] Attempt to make compatible with pre-Devotion Update, Devotion Update, and Seekers of the Storm \[SotS\]
    - Haven't tried rolling back to a pre-SotS patch to test if this works, please report any issues to the [GitHub repository](https://github.com/itsschwer/pressure-drop/issues)

### 1.3.1
- Fix `MissingMethodException` when calling `PickupDropletController.CreatePickupDroplet` *(introduced by the Devotion Update)*
    - *Fix items being removed from the inventory but not being droppped when trying to use the /drop command*
    - Not sure how this affects previous versions of the game, please report any issues to the [GitHub repository](https://github.com/itsschwer/pressure-drop/issues)

## 1.3.0
- Add configuration option to show chat history when the scoreboard is open
- Remove configuration option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated, rather than on entry
    - Use [ServerSider](https://thunderstore.io/package/itsschwer/ServerSider/) <sup>[***src***](https://github.com/itsschwer/ror2-serversider)</sup> instead

## 1.2.0
- Add configuration option to change void tier pickup rule from `ConfirmFirst` to `ConfirmAll`
- Add configuration option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated, rather than on entry

### 1.1.1
- Add demo gifs to readme
- Create `developers.md`
- \[`ChatCommander`\] Log source assembly when registering command
- Mark `Drop` as public

## 1.1.0
- Add configuration option `dropInvertDirection`
    - Controls whether items should be dropped opposite the aim direction or not
- Rework how chat commands are handled
    - *(Hook `Console.RunCmd` instead of listening to `Chat.onChatChanged`*)
    - Prevent propagation of 'say' command if a chat command is matched
        - Hopefully avoids triggering any "unable to find command" messages from other mods implementing chat commands

### 1.0.4
- Refactor code
- Prevent generating extraneous `RoR2BepInExPack` 'Hook removed' messages
- Mark `Config` as public

### 1.0.3
- Include syntax for `/drop` in readme

### 1.0.2
- Thunderstore release
- Add log message on awake

### 1.0.1
- Use GitHub Actions to generate releases
- Tiny code clean up

# 1.0.0
- Initial release
