← [README](README.md)

## Upcoming release
* Fixed more errors due to inconsistent asset key normalization (thanks to Pathoschild!).
* Internal refactoring (thanks to Pathoschild!).

## 1.1.1
Released 27 March 2021.

* Fixed errors due to inconsistent asset key normalization (thanks to Pathoschild!).

## 1.1
Released 23 March 2021.

* Improved error-handling.
* Removed an unneeded `if` check in `KeyboardDispatcher` (thanks to Mellozx and Dradonhunter11!).
* Heavily reduced the number of bundled DLLs (thanks to Pathoschild!).
* Improved installer speed by replacing forced wait times with checked operations (thanks to Pathoschild!).
* The installer now uses a temporary folder instead of directly changing the installer folder (thanks to Pathoschild!).
* Fixed pointer overflow error in `KeyboardInput` initialization (thanks to Pathoschild!).
* Beautified the installer (thanks to Pathoschild!).
* `MonoMod` and `CorFlags` now log directly to the installer window for easier troubleshooting (thanks to Pathoschild!).
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
