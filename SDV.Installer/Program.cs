using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SDV.Installer
{
    public static class Program
    {
        private static readonly string ExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string CopyToGameFolderPath = Path.Combine(ExePath, "libs", "CopyToGameFolder");

        private static readonly string[] DirtyFiles = {
            "MONOMODDED_StardewValley.exe",
            "MONOMODDED_StardewValley.pdb",
            "StardewValley.exe"
        };

        private const string CorFlagsArgs = "/C libs\\CorFlags.exe MONOMODDED_StardewValley.exe /32BITREQ-";
        private const string MonoModArgs = "/C MonoMod.exe StardewValley.exe";
        private const string ExeName = "StardewValley.exe";
        private const string ModifiedExeName = "MONOMODDED_" + ExeName;

        private static void Main()
        {
            Console.WriteLine("Cleaning out any potentially dirty files...");

            foreach (string file in DirtyFiles.Where(file => File.Exists(Path.Combine(ExePath, file))))
            {
                File.Delete(Path.Combine(ExePath, file));
                Console.WriteLine($"Deleted {file}!");
            }

            for (int i = 0; i < 2; i++)
                Console.WriteLine();

            Console.WriteLine(" Welcome to the Stardew Valley 64bit patcher!");
            Console.WriteLine("  Please note that this program requires a copy of the Linux version of Stardew Valley.");
            Console.WriteLine("  You will have to install this manually through DepotDownloader.");
            Console.WriteLine();
            Prompt();
        }

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
            WriteReadKey(" Installation complete! Please launch StardewValley.exe from the depot-download folder!" +
                         "\n Press any key to exit...");
        }

        private static string WriteReadLine(string value)
        {
            Console.WriteLine(value);
            return Console.ReadLine();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static string WriteReadKey(string value)
        {
            Console.WriteLine(value);
            return Console.ReadKey().Key.ToString();
        }

        private static void CopyRequiredDlls(string installPath)
        {
            Console.WriteLine("Copying required DLLs over to the installation location:");

            List<string> waitForFiles = new List<string>();

            // copy game DLLs into execution folder
            foreach (FileInfo dll in new DirectoryInfo(installPath).GetFiles("*.dll"))
            {
                Console.WriteLine($" Copying {dll.Name} -> exec directory...");
                File.Copy(dll.FullName, Path.Combine(ExePath, dll.Name), true);
                waitForFiles.Add(Path.Combine(ExePath, dll.Name));
            }

            // copy files into game folder
            foreach (FileInfo dll in new DirectoryInfo(CopyToGameFolderPath).GetFiles("*.dll"))
            {
                string dllName = dll.Name;

                try
                {
                    // TODO: Remove exec step
                    Console.WriteLine($" Copying {dllName} -> exec directory...");
                    File.Copy(dll.FullName, Path.Combine(ExePath, dllName), true);
                    waitForFiles.Add(Path.Combine(ExePath, dllName));

                    Console.WriteLine($" Copying {dllName} -> SDV directory...");
                    File.Copy(dll.FullName, Path.Combine(installPath, dllName), true);
                    waitForFiles.Add(Path.Combine(installPath, dllName));
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Could not locate directory: {e.Message}");
                    Console.WriteLine("Falling back to previous prompt...");
                    Continue();
                }
            }

            // Is this separate loop required for waiting until all DLLs are copied?
            foreach (string path in waitForFiles)
            {
                while (!File.Exists(path))
                    Console.ReadLine();
            }

            Console.WriteLine();
        }

        private static void ApplyMonoModPatches(string installPath)
        {
            Console.WriteLine("Copying over StardewValley.exe to patch...");

            try
            {
                File.Copy(Path.Combine(installPath, ExeName), Path.Combine(ExePath, ExeName), true);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Could not locate file: {e.FileName}");
                Console.WriteLine("Falling back to previous prompt...");
                Continue();
            }

            while (!File.Exists(Path.Combine(ExePath, ExeName)))
                Console.ReadLine();

            Console.WriteLine("Applying MonoMod patches...");

            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = MonoModArgs
                }
            }.Start();

            while (true)
            {
                try
                {
                    // Give it some time, honestly
                    Thread.Sleep(1000 * 10);
                    Console.WriteLine("Modifying MONOMODDED_StardewValley.exe flags with CorFlags...\n(If this does not work, please re-launch with administrator privileges)");

                    new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = CorFlagsArgs
                        }
                    }.Start();

                    Thread.Sleep(1000 * 5);
                    Console.WriteLine("Copying the modified EXE over to the installation location...");
                    File.Copy(Path.Combine(ExePath, ModifiedExeName), Path.Combine(installPath, ExeName), true);
                    break;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found... retrying...");
                }
            }

            Console.WriteLine("Copied the modified EXE over to the installation location!");
        }
    }
}
