- Add configuration option to change void tier pickup rule from `ConfirmFirst` to `ConfirmAll`
- Add configuration option to change *Void Fields*' fog effect to start when a *Cell Vent* is activated, rather than on entry
- Refactor config generation

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
