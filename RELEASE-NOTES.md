← [README](README.md)

## 1.1.7
Released 8 May 2021. Updated by Steviegt6.

* Fixed issue with split-screen resulting from unchecked `KeyboardDispatcher` initialization.

## 1.1.6
Released 24 April 2021. Updated by Pathoschild.

* Changed _press any key to exit_ to _press enter to exit_, to avoid accidentally closing window.
* Fixed assembly load error for some players.
* Fixed installer window closing immediately if an unhandled error occurs.

## 1.1.5
Released 23 April 2021. Updated by Pathoschild.

* Simplified installer file structure for players.
* Fixed `libSkiaSharp` error when taking in-game screenshots.

## 1.1.4
Released 22 April 2021. Updated by Pathoschild.

* Added the version number to the installer window title to simplify troubleshooting.
* Fixed errors initializing the GOG Galaxy and Steam SDKs.
* Removed the `MonoMod.Utils.dll` dependency added to the game folder.

## 1.1.3
Released 14 April 2021. Updated by Pathoschild.

* Added support for patching the same game folder more than once.
* Added `Game1.Stardew64InstallerVersion` field so SMAPI can log the patch tool version.
* Improved error-handling for game path.
* Fixed issue where a failed install would ask for a new game path, install to that folder, then try to resume the previous failed install.

## 1.1.2
Released 03 April 2021. Updated by Pathoschild.

* Fixed `MonoGame.Framework` patches not being applied.
* Internal refactoring.

## 1.1.1
Released 27 March 2021. Updated by Pathoschild.

* Fixed errors due to inconsistent asset key normalization.

## 1.1
Released 23 March 2021.

* Improved error-handling.
* Improved installer speed by replacing forced wait times with checked operations (thanks to Pathoschild!).
* The installer now uses a temporary folder instead of directly changing the installer folder (thanks to Pathoschild!).
* The installer now shows `MonoMod` and `CorFlags` output directly for easier troubleshooting (thanks to Pathoschild!).
* Heavily reduced the number of bundled and overwritten DLLs (thanks to Pathoschild!).
* Removed an unneeded `if` check in `KeyboardDispatcher` (thanks to Mellozx and Dradonhunter11!).
* Fixed pointer overflow error in `KeyboardInput` initialization (thanks to Pathoschild!).
* Internal refactoring (thanks to Pathoschild!).

## 1.0.1
Released 17 March 2021.

* Added a bundled 64-bit `Steamworks.NET.dll` instead of applying `CorFlags` to the 32-bit version.
* Added `CorFlags` to `StardewValley.exe` to fix `BadImageFormatException` errors.
* Fixed missing steamapi DLL error.

## 1.0
Released 16 March 2021.

* Initial release.

Credits for the initial work:
* [@Dradonhunter11](https://github.com/Dradonhunter11) for figuring out how to make Stardew Valley 64-bit actually work.
* [@Steviegt6](https://github.com/Steviegt6) for the implementation.
* CiscoRamon for putting Steviegt6 and Dradonhunter11 in touch with [@Pathoschild](https://github.com/Pathoschild) to start this project.
