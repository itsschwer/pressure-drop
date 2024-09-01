# for developers

## implementing your own chat commands

it might be more helpful to first view an [example](https://github.com/itsschwer/ror2-experimental/commit/8c4b492ec8548a48143157dbe69ea15c82746762) before reading ahead.

### Add dependency

Add the dependency string for this plugin to your `manifest.json`
```diff
"dependencies": [
-    "bbepis-BepInExPack-5.4.2113"
+    "bbepis-BepInExPack-5.4.2113",
+    "itsschwer-PressureDrop-1.1.1"
]
```

Add a reference to `PressureDrop.dll` in your `.csproj`
1. Install [PressureDrop](https://thunderstore.io/package/itsschwer/PressureDrop/) *(using [r2modman](https://thunderstore.io/package/ebkr/r2modman/))*
2. Find `PressureDrop.dll` *(e.g. AppData\Roaming\r2modmanPlus-local\RiskOfRain2\cache\itsschwer-PressureDrop\1.1.1\plugins\PressureDrop\PressureDrop.dll)*
3. Copy `PressureDrop.dll` to your project *(e.g. a [`libs`](https://github.com/risk-of-thunder/R2Boilerplate/tree/0a4ff42595674f4d3beb7b01cb782e46c5e93341/ExamplePlugin/libs) folder in your project root)*
4. Add the following to your `.csproj`
```xml
    <ItemGroup>
        <Reference Include="PressureDrop">
            <HintPath>libs\PressureDrop.dll</HintPath>
            <Private>false</Private>
        </Reference>
    </ItemGroup>
```

Add a dependency in your plugin class
```diff
+   [BepInDependency(PressureDrop.Plugin.GUID)]
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
```

Add the `using` as necessary
```cs
using PressureDrop;
```

### Create commands

- Command methods have the parameters `NetworkUser user` and `string[] args`
    - `user` is the `NetworkUser` which called the command
    - `args` are the arguments that have been passed with the command
        - `args[0]` will always be the `string` used to call the command
        - Check `args.Length` to avoid `IndexOutOfRangeException`

### Register commands

- Use `ChatCommander.Register()` to add a chat command
    - It takes two parameters, `string token` and `System.Action<NetworkUser, string[]> action`
        - `token` is the string that will be used in-game to activate your command
        - `action` is the method that will be called upon activation
    - `Register()` returns a `bool` representing whether the command was successfully registered or not.
- Use `ChatCommander.Unregister()` to remove a chat command *(same parameters — need to match!)*

### Suggested syntax for displaying command usage

Entry | Meaning
---   | ---
`plain text` | Enter this literally, exactly as shown
`<argument-name>` | Placeholder, replace `<argument-name>` with an appropriate value
`[optional]` | Optional, `optional` can be omitted
`[choice-1 \| choice-2]` | Optional choice, can omit or pick either `choice-1` or `choice-2`
`(choice-1 \| choice-2)` | Required choice, must pick either `choice-1` or `choice-2`

#### Other styles:
- [Minecraft](https://minecraft.wiki/w/Commands#Syntax)
- [GitHub](https://github.com/cli/cli/blob/trunk/docs/command-line-syntax.md)
