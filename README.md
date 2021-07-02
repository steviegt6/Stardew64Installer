**Stardew64Installer** is a handy piece of software that turns the Linux version of Stardew Valley
into a working 64-bit Windows version!

# For players
### Install guide
Follow these instructions closely! This needs a special version of the game.

1. Update all your mods to their latest versions. (Newer versions may add 64-bit support.)
2. Download the Linux version of the game:
   1. Go to `steam://nav/console` in your browser to open the Steam console.
   2. Run this command: `download_depot 413150 413153`
   3. Wait for the download to finish. This may take a long time and won't show any download
      progress. Eventually the console will say
      "Depot download complete" and show the downloaded folder path.
   4. **For the rest of the instructions, "depot folder" means this downloaded folder.**
3. Make Stardew Valley 64-bit:
   1. Download and unzip [Stardew64Installer from the releases page](https://github.com/Steviegt6/Stardew64Installer/releases).
   2. Double-click `Stardew64Installer.exe` and follow the on-screen instructions.
4. Install SMAPI 64-bit:
   1. Download and unzip [SMAPI 3.10 or later](https://smapi.io/).  
   2. Run the SMAPI installer to install it (it will detect 64-bit mode automatically).  
      _Tip: if it chooses a different game folder, rename the detected folder temporarily and try
      it again; the installer should then ask for the game path._
5. Run `StardewModdingAPI.exe` in the depot folder and the game should be 64-bit!

### Troubleshooting
* Antiviruses may flag/block/delete the installer since it moves/copies/modifies files in your game
  folder. In that case you may need to whitelist the installer in your antivirus program.

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

The project also includes these DLLs in `libs\CopyToGameFolder`, which are copied into the game
folder to fix 64-bit compatibility:

file | description
---- | -----------
`Galaxy64.dll`<br />`GalaxyCSharp.dll`<br />`GalaxyCSharpGlue.dll` | The 64-bit [GOG Galaxy SDK](https://docs.gog.com/sdk/). `GalaxyCSharp.dll` replaces a 32-bit version, and the others are added to support it.
`libSkiaSharp.dll` | The [SkiaSharp](https://github.com/mono/SkiaSharp) 2D graphics library. This replaces a 32-bit version in the game folder. Taken from the [2.80.2 NuGet package](https://www.nuget.org/packages/SkiaSharp) (`runtimes/win-x64/native/libSkiaSharp.dll`).
`SDL2.dll`<br />`soft_oal.dll` | These are [Simple DirectMedia Layer](https://www.libsdl.org/) and [OpenAL](https://openal.org/), audio dependencies used by the game and needed to run the Linux version on Windows. Added files (they'd normally be provided by the OS).
`steam_api64.dll`<br />`Steamworks.NET.dll` | The 64-bit Windows [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET). These replace 32-bit versions in the game folder. Taken from the [15.0.0 release](https://github.com/rlabrecque/Steamworks.NET/releases).

### Contributing
Contributing is simple. I don't have a set style guide or any real guidelines to follow, feel free
to submit pull requests and explain your reasoning. We'll discuss further once a pull request is
open (or you can contact me directly beforehand).

All contributors are credited in the release notes.

### Preparing a release
To prepare a release build:

1. Update `common.targets`, `RELEASE-NOTES.md`, and `Stardew64Installer.Framework/Constants` for the new version.
2. Build the solution in Release mode.
3. Go to `Stardew64Installer/bin/Release/net452`.
4. Zip the `Stardew64Installer *` folder for the current version.
4. Upload or share that file.

## See also
* [Release notes](RELEASE-NOTES.md)
