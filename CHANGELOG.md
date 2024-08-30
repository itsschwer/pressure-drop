- Change default value of configuration option `voidPickupConfirmAll` to `true`

### 1.3.2
- \[Dropping items\] Attempt to make compatible with pre-Devotion Update, Devotion Update, and Seekers of the Storm \[SotS\]
    - Haven't tried rolling back to a pre-SotS patch to test if this works, please report any issues to the [GitHub repository](https://github.com/itsschwer/pressure-drop/issues)

### 1.3.1
- Fix `MissingMethodException` when calling ` PickupDropletController.CreatePickupDroplet` *(introduced by the Devotion Update)*
    - *Fix items being removed from the inventory but not being droppped when trying to use the /drop command*
    - Not sure how this affects previous versions of the game, please report any issues to the [GitHub repository](https://github.com/itsschwer/pressure-drop/issues)

## 1.3.0
- Add configuration option to show chat history when the scoreboard is open
- Removed configuration option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated, rather than on entry
    - Use [ServerSider](https://thunderstore.io/package/itsschwer/ServerSider/) <sup>[***src***](https://github.com/itsschwer/ror2-serversider)</sup> instead

## 1.2.0
- Add configuration option to change void tier pickup rule from `ConfirmFirst` to `ConfirmAll`
- Add configuration option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated, rather than on entry

### 1.1.1
- Add demo gifs to readme
- Create developers.md
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
