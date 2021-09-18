using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Cecil;
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

        /// <summary>The relative path within the installer folder that contains DLLs and other files used by the installer.</summary>
        private const string InternalsDirName = "internal";

        /// <summary>The relative path within the staging folder that contains DLLs to copy into the game folder.</summary>
        private const string CopyToGameDirName = "CopyToGameFolder";

        /// <summary>The relative path withing the internal folder that contains DepotDownloader resources.</summary>
        private const string DepotDownloaderPath = "DepotDownloader";

        /// <summary>The relative path in relation to the base release directory.</summary>
        private const string DownloadedDepotsPath = "depots";

        /// <summary>The absolute path to a temporary one-use folder in which to store intermediate files during installation.</summary>
        private static readonly string StagingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        /// <summary>The original filename for the Stardew Valley executable.</summary>
        private const string ExeName = "StardewValley.exe";

        /// <summary>The original filename for the <c>MonoGame.Framework</c> DLL.</summary>
        private const string MonoGameDllName = "MonoGame.Framework.dll";

        /// <summary>The filename prefix added by MonoMod to the modified version of an executable or DLL.</summary>
        private const string MonoModdedPrefix = "MONOMODDED_";

        /// <summary>The filename suffix added to backup files.</summary>
        private const string BackupSuffix = ".original";


        /*********
        ** Public methods
        *********/
        /// <summary>The console app entry point.</summary>
        public static void Main()
        {
            Console.Title = $"Stardew64Installer {Constants.Stardew64InstallerVersion} - {Console.Title}";

            AppDomain.CurrentDomain.AssemblyResolve += Program.CurrentDomain_AssemblyResolve;

            Console.WriteLine("Welcome to the Stardew Valley 64-bit patcher!");
            Console.WriteLine(" Please note that this program requires a copy of the Linux version of Stardew Valley.");
            Console.WriteLine(" You will have to install this manually through DepotDownloader.");
            Console.WriteLine();

            try
            {
                Start();
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                WriteReadLine("Press enter to exit...");
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Method called when assembly resolution fails, which may return a manually resolved assembly.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            try
            {
                AssemblyName name = new AssemblyName(e.Name);
                foreach (FileInfo dll in new DirectoryInfo(Path.Combine(Program.InstallerPath, Program.InternalsDirName)).EnumerateFiles("*.dll"))
                {
                    if (name.Name.Equals(AssemblyName.GetAssemblyName(dll.FullName).Name, StringComparison.OrdinalIgnoreCase))
                        return Assembly.LoadFrom(dll.FullName);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resolving assembly: {ex}");
                return null;
            }
        }

        private static void Start()
        {
            while (true)
            {
                string option = WriteReadLine("[1] I don't have a copy of the Linux version!\n[2] I'm good to go!").Trim();

                if (option == "1")
                {
                    PromptDepotDownload();
                    break;
                }
                if (option == "2")
                {
                    Install();
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("We failed to parse your response. Please choose a number between 1 and 2.");
            }
        }

        private static void PromptDepotDownload()
        {
            Console.WriteLine();

            while (true)
            {
                string option = WriteReadLine("Would you like to use DepotDownloader to install the Linux version?" +
                                              "\n[1] Yes. (prompts for your Steam login credentials!)" +
                                              "\n[2] No. (you can still use the official Steam depot downloader)").Trim();

                if (option == "1")
                {
                    DepotDownload();
                    break;
                }

                if (option == "2")
                    break;
            }
        }

        private static void DepotDownload()
        {
            Console.WriteLine();
            Console.WriteLine("Proceeding with depot download... (note that any freezes are normal!)");

            Console.WriteLine();
            string username = WriteReadLine("Please enter your Steam username:");

            Console.WriteLine();
            string password = WriteReadLine("Please enter your Steam password:");

            RunCommand($"dotnet {InternalsDirName}\\{DepotDownloaderPath}\\DepotDownloader.dll -app {413150} -depot {413153} -username {username} -password {password}");

            Console.WriteLine();

            while (true)
            {
                string option = WriteReadLine("Depot downloading process should have succeeded. Would you like to use these files to install 64bit?" +
                                              "\n[1] Yes, do not exit program." +
                                              "\n[2] No, exit program.").Trim();

                if (option == "1")
                {
                    Install(Path.Combine(DownloadedDepotsPath, "413153", "6125897"));
                    break;
                }

                if (option == "2")
                    break;
            }
        }

        /// <summary>Interactively install to the game folder.</summary>
        private static void Install(string depotDownload = "")
        {
            while (true)
            {
                // get install path
                Console.WriteLine();
                string installPath = string.IsNullOrEmpty(depotDownload)
                    ? WriteReadLine("Please provide the location of the depot-downloaded copy of Stardew Valley:")
                    : Path.Combine(InstallerPath, depotDownload);
                if (string.IsNullOrWhiteSpace(installPath))
                    continue;

                // get directory
                DirectoryInfo installDir;
                try
                {
                    installDir = new DirectoryInfo(installPath);
                }
                catch (Exception ex)
                {
                    LogError($"Could not access that folder: {ex.Message}");
                    continue;
                }
                if (!installDir.Exists)
                {
                    LogError("That folder doesn't seem to exist.");
                    continue;
                }

                // install
                Console.WriteLine();
                bool installed =
                    TryPrepareStagingFolder(installDir, out DirectoryInfo stagingDir)
                    && TryApplyPatches(stagingDir)
                    && TryInstallFiles(stagingDir, installDir);

                if (installed)
                    break;
            }

            SetColor(ConsoleColor.Green, () => Console.WriteLine($"Installation complete! Please launch {ExeName} from the game folder."));
            WriteReadLine("Press enter to exit...");
        }

        /// <summary>Write a message and wait for the user to hit enter.</summary>
        /// <param name="message">The message to log.</param>
        private static string WriteReadLine(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        /// <summary>Create the staging folder with all the files needed to run the patcher.</summary>
        /// <param name="installDir">The game install folder.</param>
        /// <param name="stagingDir">The staging folder to patch.</param>
        private static bool TryPrepareStagingFolder(DirectoryInfo installDir, out DirectoryInfo stagingDir)
        {
            Console.WriteLine($"Copying files to temporary folder ({StagingPath})...");
            stagingDir = new DirectoryInfo(StagingPath);

            // copy installer files
            new DirectoryInfo(Path.Combine(InstallerPath, InternalsDirName)).RecursiveCopyTo(stagingDir.FullName);

            // copy game DLLs
            foreach (FileInfo dll in installDir.GetFiles("*.dll"))
                dll.CopyToAndWait(Path.Combine(stagingDir.FullName, dll.Name));

            // copy executable
            if (TryGetMonoModOriginal(installDir, ExeName, out FileInfo exeFile, out string error))
                exeFile.CopyToAndWait(Path.Combine(stagingDir.FullName, ExeName));
            else
            {
                LogError(error);
                return false;
            }

            // copy MonoGame DLL
            if (TryGetMonoModOriginal(installDir, MonoGameDllName, out FileInfo monogameFile, out error))
                monogameFile.CopyToAndWait(Path.Combine(stagingDir.FullName, MonoGameDllName));
            else
            {
                LogError(error);
                return false;
            }

            // merge overwrite files into staging folder
            foreach (FileInfo dll in new DirectoryInfo(Path.Combine(stagingDir.FullName, CopyToGameDirName)).GetFiles("*.dll"))
                dll.CopyToAndWait(Path.Combine(stagingDir.FullName, dll.Name));

            Console.WriteLine();
            return true;
        }

        /// <summary>Patch the files in the staging folder.</summary>
        /// <param name="stagingDir">The staging folder to patch.</param>
        private static bool TryApplyPatches(DirectoryInfo stagingDir)
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
            RunCommand($"CorFlags.exe {modifiedExeName} /32BITREQ-", workingPath: stagingDir.FullName);
            Console.WriteLine();

            return true;
        }

        /// <summary>Copy the modified files into the game folder.</summary>
        /// <param name="stagingDir">The staging folder which was patched.</param>
        /// <param name="installDir">The game install folder.</param>
        private static bool TryInstallFiles(DirectoryInfo stagingDir, DirectoryInfo installDir)
        {
            // copy override files
            Console.WriteLine("Copying override files...");
            DirectoryInfo overridesFolder = new DirectoryInfo(Path.Combine(stagingDir.FullName, CopyToGameDirName));
            foreach (FileInfo dll in overridesFolder.GetFiles("*.dll"))
            {
                string dllName = dll.Name;
                dll.CopyToAndWait(Path.Combine(installDir.FullName, dllName));
            }

            // copy modified executable
            Console.WriteLine($"Copying patched {ExeName}...");
            {
                var original = new FileInfo(Path.Combine(stagingDir.FullName, ExeName));
                var patched = new FileInfo(Path.Combine(stagingDir.FullName, MonoModdedPrefix + ExeName));

                original.CopyToAndWait(Path.Combine(installDir.FullName, ExeName + BackupSuffix));
                patched.CopyToAndWait(Path.Combine(installDir.FullName, ExeName));
            }

            // copy modified MonoGame
            Console.WriteLine($"Copying patched {MonoGameDllName}...");
            {
                var original = new FileInfo(Path.Combine(stagingDir.FullName, MonoGameDllName));
                var patched = new FileInfo(Path.Combine(stagingDir.FullName, MonoModdedPrefix + MonoGameDllName));

                original.CopyToAndWait(Path.Combine(installDir.FullName, MonoGameDllName + BackupSuffix));
                patched.CopyToAndWait(Path.Combine(installDir.FullName, MonoGameDllName));
            }

            Console.WriteLine();
            return true;
        }

        /// <summary>Try to get the original version of a file without MonoMod patches.</summary>
        /// <param name="gameDir">The game install folder.</param>
        /// <param name="filename">The original filename.</param>
        /// <param name="file">The file found, if any.</param>
        /// <param name="error">An error message indicating why the file couldn't be found, if applicable.</param>
        /// <returns>Returns whether the file was found.</returns>
        private static bool TryGetMonoModOriginal(DirectoryInfo gameDir, string filename, out FileInfo file, out string error)
        {
            // get base file
            file = new FileInfo(Path.Combine(gameDir.FullName, filename));
            if (!file.Exists)
            {
                error = $"Couldn't find {file.Name} in the game folder.";
                return false;
            }
            if (!IsMonoModded(file.FullName))
            {
                error = null;
                return true;
            }

            // get MonoMod backup
            file = new FileInfo(Path.Combine(gameDir.FullName, filename + BackupSuffix));
            if (!file.Exists)
            {
                error = $"The patch tool was previously applied to this game folder, but no backup of the original {filename} file exists. Please delete and reinstall the game to fix this.";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>Get whether an assembly has been patched by MonoMod.</summary>
        /// <param name="path">The assembly file path to check.</param>
        private static bool IsMonoModded(string path)
        {
            // read assembly
            byte[] assemblyBytes = File.ReadAllBytes(path);
            using Stream readStream = new MemoryStream(assemblyBytes);
            using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(readStream, new ReaderParameters(ReadingMode.Immediate) { InMemory = true });

            // check for MonoMod marker
            return assembly.MainModule.GetType("MonoMod.WasHere") != null;
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

        /// <summary>Write an error message to the console.</summary>
        /// <param name="text">The message to write.</param>
        private static void LogError(string text)
        {
            SetColor(ConsoleColor.Red, () => Console.WriteLine(text));
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
