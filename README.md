# Pocket-Portal-Guide
BepInEx plugin for Valheim. Adds toggle to show Portals on the map with their tag.

# Building
Fix the references. You'll need to reference the DLLs from your Valheim install, as well as the DLLs that is included with BepInEx.
Once that is done, you should be able to build.

# Installing
Drop the DLL into your BepInEx/Plugins folder.

# Usage
By default the plugin will not show map pins. To toggle them on the default key is F8. When toggled on, any portal will appear on the minimap and large map as a portal icon with the portal tag as the pin name
Any connected portal pair will get a randomly assigned color (this can be turned off in the INI file).

The plugin will remove pins before mapdata is saved (this happens periodically and on logout/shutdown), to avoid polluting the save game data, and avoid adding pins on top of existing pins.

# Detailed Configuration
The plugin comes with the standard INI file

```
[Minimap]
Show Pins = false
Untagged Portal Label = -untagged-
Use Color Coding = true
```
The ``Minimap`` section deals with map pins. ``Show Pins`` determines if the plugin should show Portal pins on start or not.
``Untagged Portal Label`` is the pin name given to Portals witn no tag. Pins with an empty string for a name won't show up on the map, so setting this to an empty string disables showing untagged portals
If ``Use Color Coding`` is true, the plugin will assign a random color to connected portal pairs
```
[Toggle Show Pins]
Key = F8
Modifier = None
```
The ``Toggle Show Pins`` section deals with the hotkey to toggle whether to show pins or not. The ``Key`` entry is just that, which key to press to toggle.
For more advanced combinations you can use the ``Modifier`` to create key combinations for trigger the toggle (Such as LeftControl+P)
