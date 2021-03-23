using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SDV.Installer.Framework;

namespace SDV.Installer
{
    public static class Program
    {
        /*********
        ** Fields
        *********/
        private static readonly string ExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string CopyToGameFolderPath = Path.Combine(ExePath, "libs", "CopyToGameFolder");

        private const string ExeName = "StardewValley.exe";
        private const string ModifiedExeName = "MONOMODDED_" + ExeName;

        private static readonly string[] DirtyFiles = {
            ExeName,
            ModifiedExeName,
            "MONOMODDED_StardewValley.pdb",
        };

        /*********
        ** Public methods
        *********/
        public static void Main()
        {
            Console.WriteLine("Cleaning out any potentially dirty files...");

            foreach (string fileName in DirtyFiles)
            {
                FileInfo file = new FileInfo(Path.Combine(ExePath, fileName));
                if (file.DeleteAndWait())
                    Console.WriteLine($"Deleted {fileName}!");
            }

            for (int i = 0; i < 2; i++)
                Console.WriteLine();

            Console.WriteLine(" Welcome to the Stardew Valley 64bit patcher!");
            Console.WriteLine("  Please note that this program requires a copy of the Linux version of Stardew Valley.");
            Console.WriteLine("  You will have to install this manually through DepotDownloader.");
            Console.WriteLine();
            Prompt();
        }


        /*********
        ** Private methods
        *********/
        private static void Prompt()
        {
            string option = WriteReadLine(" [1] I don't have a copy of the Linux version!" +
                                          "\n [2] I'm good to go!");

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
            WriteReadKey(" Please download DepotDownloader through https://github.com/SteamRE/DepotDownloader" +
                         "\n Press any key to exit...");
        }

        private static void Continue()
        {
            Console.WriteLine();

            string installationFolder = WriteReadLine("Please provide the location of the depot-downloaded copy of Stardew Valley:");
            Console.WriteLine();

            CopyRequiredDlls(installationFolder);
            ApplyMonoModPatches(installationFolder);

            Console.WriteLine();
            WriteReadKey($" Installation complete! Please launch {ExeName} from the depot-download folder!" +
                         "\n Press any key to exit...");
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

        private static void CopyRequiredDlls(string installPath)
        {
            Console.WriteLine("Copying required DLLs over to the installation location:");

            // copy game DLLs into execution folder
            foreach (FileInfo dll in new DirectoryInfo(installPath).GetFiles("*.dll"))
            {
                Console.WriteLine($" Copying {dll.Name} -> exec directory...");
                dll.CopyToAndWait(Path.Combine(ExePath, dll.Name));
            }

            // copy files into game folder
            foreach (FileInfo dll in new DirectoryInfo(CopyToGameFolderPath).GetFiles("*.dll"))
            {
                string dllName = dll.Name;

                try
                {
                    // TODO: Remove exec step
                    Console.WriteLine($" Copying {dllName} -> exec directory...");
                    dll.CopyToAndWait(Path.Combine(ExePath, dllName));

                    Console.WriteLine($" Copying {dllName} -> SDV directory...");
                    dll.CopyToAndWait(Path.Combine(installPath, dllName));
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Could not locate directory: {e.Message}");
                    Console.WriteLine("Falling back to previous prompt...");
                    Continue();
                }
            }

            Console.WriteLine();
        }

        private static void ApplyMonoModPatches(string installPath)
        {
            Console.WriteLine($"Copying over {ExeName} to patch...");

            try
            {
                FileInfo file = new FileInfo(Path.Combine(installPath, ExeName));
                file.CopyToAndWait(Path.Combine(ExePath, ExeName));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Could not locate file: {e.FileName}");
                Console.WriteLine("Falling back to previous prompt...");
                Continue();
            }

            Console.WriteLine("Applying MonoMod patches...");

            RunCommand($"MonoMod.exe {ExeName}");

            while (true)
            {
                try
                {
                    Console.WriteLine($"Modifying {ModifiedExeName} flags with CorFlags...\n(If this does not work, please re-launch with administrator privileges)");

                    RunCommand($"libs\\CorFlags.exe {ModifiedExeName} /32BITREQ-");

                    Console.WriteLine("Copying the modified EXE over to the installation location...");

                    var file = new FileInfo(Path.Combine(ExePath, ModifiedExeName));
                    file.CopyToAndWait(Path.Combine(installPath, ExeName));
                    break;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found... retrying...");
                }
            }

            Console.WriteLine("Copied the modified EXE over to the installation location!");
        }

        /// <summary>Run a command through <c>cmd.exe</c> and wait for it to finish.</summary>
        /// <param name="command">The command to run.</param>
        private static void RunCommand(string command)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",

                        // run within the same window
                        UseShellExecute = false
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
