**Stardew64Installer** is a handy piece of software that turns the Linux version of Stardew Valley
into a working 64-bit Windows version!

# For players
### Install guide
Follow these instructions closely! This needs a special version of the game.

1. Update all your mods to their latest versions. (Newer versions may add 64-bit support.)
2. Download the Linux version of the game:
   1. Go to `steam://nav/console` in your browser to open the Steam console.
   2. Run this command:
      ```
      download_depot 413150 413153
      ```
   3. Wait for the download to finish. This may take a long time and won't show any download
      progress. Eventually the console will say
      "Depot download complete" and show the downloaded folder path.
   4. Make a backup of the downloaded folder! You'll need a clean copy of the game if you need to
      reapply the patch tool.
   5. **For the rest of the instructions, "depot folder" means this downloaded folder.**
3. Make Stardew Valley 64-bit:
   1. Download and unzip [Stardew64Installer from the releases page](https://github.com/Steviegt6/Stardew64Installer/releases).
   2. Double-click the `Stardew64Installer.exe` file.
   3. When it asks, paste the full path to the depot folder.
   4. Wait for it to say "Installation complete!".
4. Install SMAPI 64-bit:
   1. ~~Download and unzip the [64-bit version of SMAPI](https://smapi.io/).~~  
      ***Note:*** 64-bit SMAPI is in development and not publicly available yet.
   2. Copy the files into the depot folder, so `StardewModdingAPI.exe` is in the same folder as
      `StardewValley.exe`. (There's no installer currently.)
5. Run `StardewModdingAPI.exe` in the depot folder and it should be 64-bit!

# For developers
### How Stardew64Installer works
The solution is split into four projects:

project | purpose
------- | -------
`Stardew64Installer` | A simple installer console app that runs on players' computers and interactively patches their game.
`Stardew64Installer.Framework` | Internal code shared between the patch projects.
`MonoGame.Framework.Patches.mm`<br />`StardewValley.Patches.mm` | [MonoMod](https://github.com/MonoMod/MonoMod) patches which rewrite `MonoGame.Framework.dll` and `StardewValley.exe`. These are bundled into the installer.

These work together to automatically patch the game files. Behind the scenes, `CorFlags.exe` is
used to modify `StardewValley.exe` and MonoMod is used to rewrite the assemblies.

### Contributing
Contributing is simple. I don't have a set style guide or any real guidelines to follow, feel free
to submit pull requests and explain your reasoning. We'll discuss further once a pull request is
open (or you can contact me directly beforehand).

Credit to [@Dradonhunter11](https://github.com/Dradonhunter11) for figuring out how to make Stardew
Valley 64-bit actually work! This program automates that process.
