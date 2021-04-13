← [README](README.md)

## Upcoming release
* Improved error-handling for game path (thanks to Pathoschild!).
* Fixed issue where a failed install would ask for a new game path, install to that folder, then try to resume the previous failed install (thanks to Pathoschild!).

## 1.1.2
Released 03 April 2021.

* Fixed `MonoGame.Framework` patches not being applied (thanks to Pathoschild!).
* Internal refactoring (thanks to Pathoschild!).

## 1.1.1
Released 27 March 2021.

* Fixed errors due to inconsistent asset key normalization (thanks to Pathoschild!).

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

* Initial implementation.  
  Thanks to [@Dradonhunter11](https://github.com/Dradonhunter11) for figuring out how to make Stardew Valley 64-bit actually work! The installer automates that process.
