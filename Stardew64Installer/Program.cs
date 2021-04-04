using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Stardew64Installer.Framework;

namespace Stardew64Installer
{
    /// <summary>The console app entry class.</summary>
    public static class Program
    {
        /*********
        ** Fields
        *********/
        /// <summary>The absolute path to the installer folder.</summary>
        private static readonly string InstallerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>The relative path within the installer and staging folders that contains DLLs to copy into the game folder.</summary>
        private static readonly string CopyToGameFolderRelativePath = Path.Combine("libs", "CopyToGameFolder");

        /// <summary>The absolute path to a temporary one-use folder in which to store intermediate files during installation.</summary>
        private static readonly string StagingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        /// <summary>The original filename for the Stardew Valley executable.</summary>
        private const string ExeName = "StardewValley.exe";

        /// <summary>The original filename for the <c>MonoGame.Framework</c> DLL.</summary>
        private const string MonoGameDllName = "MonoGame.Framework.dll";

        /// <summary>The filename prefix added by MonoMod to the modified version of an executable or DLL.</summary>
        private const string MonoModdedPrefix = "MONOMODDED_";


        /*********
        ** Public methods
        *********/
        /// <summary>The console app entry point.</summary>
        public static void Main()
        {
            Console.WriteLine("Welcome to the Stardew Valley 64-bit patcher!");
            Console.WriteLine(" Please note that this program requires a copy of the Linux version of Stardew Valley.");
            Console.WriteLine(" You will have to install this manually through DepotDownloader.");
            Console.WriteLine();
            Prompt();
        }


        /*********
        ** Private methods
        *********/
        private static void Prompt()
        {
            string option = WriteReadLine("[1] I don't have a copy of the Linux version!\n[2] I'm good to go!");

            if (!int.TryParse(option, out int optionNum))
                return;

            if (optionNum < 1 || optionNum > 2)
            {
                ParseFailure();
                return;
            }

            if (optionNum == 1)
                DepotDownloadMessage();
            else
                Continue();
        }

        private static void ParseFailure()
        {
            Console.WriteLine();
            Console.WriteLine("We failed to parse your response. Please choose a number between 1 and 2.");
            Prompt();
        }

        private static void DepotDownloadMessage()
        {
            // TODO: Integrate this into this program... eventually?
            Console.WriteLine();
            Console.WriteLine("Please download DepotDownloader through https://github.com/SteamRE/DepotDownloader");
            WriteReadKey("Press any key to exit...");
        }

        private static void Continue()
        {
            Console.WriteLine();

            string installPath = WriteReadLine("Please provide the location of the depot-downloaded copy of Stardew Valley:");
            Console.WriteLine();

            DirectoryInfo installDir = new DirectoryInfo(installPath);
            DirectoryInfo stagingDir = PrepareStagingFolder(installDir);
            ApplyPatches(stagingDir);
            InstallFiles(stagingDir, installDir);

            SetColor(ConsoleColor.Green, () =>
                Console.WriteLine($"Installation complete! Please launch {ExeName} from the game folder.")
            );
            WriteReadKey("Press any key to exit...");
        }

        private static string WriteReadLine(string value)
        {
            Console.WriteLine(value);
            return Console.ReadLine();
        }

        private static void WriteReadKey(string value)
        {
            Console.WriteLine(value);
            Console.ReadKey();
        }

        /// <summary>Create the staging folder with all the files needed to run the patcher.</summary>
        /// <param name="installDir">The game install folder.</param>
        private static DirectoryInfo PrepareStagingFolder(DirectoryInfo installDir)
        {
            Console.WriteLine($"Copying files to temporary folder ({StagingPath})...");
            var stagingDir = new DirectoryInfo(StagingPath);

            // copy installer files
            new DirectoryInfo(InstallerPath).RecursiveCopyTo(stagingDir.FullName);

            // copy game DLLs
            foreach (FileInfo dll in installDir.GetFiles("*.dll"))
                dll.CopyToAndWait(Path.Combine(stagingDir.FullName, dll.Name));

            // copy game executable
            {
                FileInfo file = installDir.GetFiles(ExeName).FirstOrDefault();
                if (file == null)
                {
                    Console.WriteLine($"Could not locate {ExeName}");
                    Console.WriteLine("Falling back to previous prompt...");
                    Continue();
                }

                file.CopyToAndWait(Path.Combine(stagingDir.FullName, ExeName));
            }

            // copy overwrite files
            foreach (FileInfo dll in new DirectoryInfo(Path.Combine(InstallerPath, CopyToGameFolderRelativePath)).GetFiles("*.dll"))
                dll.CopyToAndWait(Path.Combine(stagingDir.FullName, dll.Name));

            Console.WriteLine();
            return stagingDir;
        }

        /// <summary>Patch the files in the staging folder.</summary>
        /// <param name="stagingDir">The staging folder to patch.</param>
        private static void ApplyPatches(DirectoryInfo stagingDir)
        {
            // apply MonoMod patches
            Console.WriteLine($"Applying MonoMod patches to {ExeName}...");
            RunCommand($"MonoMod.exe {ExeName}", workingPath: stagingDir.FullName);
            Console.WriteLine($"Applying MonoMod patches to {MonoGameDllName}...");
            RunCommand($"MonoMod.exe {MonoGameDllName}", workingPath: stagingDir.FullName);
            Console.WriteLine();

            // apply CorFlags
            string modifiedExeName = MonoModdedPrefix + ExeName;
            Console.WriteLine($"Patching {modifiedExeName} flags with CorFlags... (If this doesn't work, please relaunch with administrator privileges.)");
            RunCommand($"{Path.Combine("libs", "CorFlags.exe")} {modifiedExeName} /32BITREQ-", workingPath: stagingDir.FullName);
            Console.WriteLine();
        }

        /// <summary>Copy the modified files into the game folder.</summary>
        /// <param name="stagingDir">The staging folder which was patched.</param>
        /// <param name="installDir">The game install folder.</param>
        private static void InstallFiles(DirectoryInfo stagingDir, DirectoryInfo installDir)
        {
            // copy override files
            Console.WriteLine("Copying override files...");
            DirectoryInfo overridesFolder = new DirectoryInfo(Path.Combine(stagingDir.FullName, CopyToGameFolderRelativePath));
            foreach (FileInfo dll in overridesFolder.GetFiles("*.dll"))
            {
                string dllName = dll.Name;
                dll.CopyToAndWait(Path.Combine(installDir.FullName, dllName));
            }

            // copy modified executable
            Console.WriteLine($"Copying patched {ExeName}...");
            {
                var file = new FileInfo(Path.Combine(stagingDir.FullName, MonoModdedPrefix + ExeName));
                file.CopyToAndWait(Path.Combine(installDir.FullName, ExeName));
            }

            // copy modified MonoGame
            Console.WriteLine($"Copying patched {MonoGameDllName}...");
            {
                var file = new FileInfo(Path.Combine(stagingDir.FullName, MonoModdedPrefix + MonoGameDllName));
                file.CopyToAndWait(Path.Combine(installDir.FullName, MonoGameDllName));
            }

            Console.WriteLine();
        }

        /// <summary>Run a command through <c>cmd.exe</c> and wait for it to finish.</summary>
        /// <param name="command">The command to run.</param>
        /// <param name="workingPath">The absolute path to the working directory for the command, if any.</param>
        private static void RunCommand(string command, string workingPath = null)
        {
            SetColor(ConsoleColor.DarkGray, () =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    UseShellExecute = false // run within the same window
                };
                if (workingPath != null)
                    startInfo.WorkingDirectory = workingPath;

                var process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();
            });
        }

        /// <summary>Write text with a given console color.</summary>
        /// <param name="color">The color to set.</param>
        /// <param name="action">The action which writes console text.</param>
        private static void SetColor(ConsoleColor color, Action action)
        {
            Console.ForegroundColor = color;
            try
            {
                action();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
