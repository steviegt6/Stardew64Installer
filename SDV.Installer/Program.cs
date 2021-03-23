using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SDV.Installer
{
    public static class Program
    {
        private static string ExePath =>
            Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        private static string LibPath => ExePath + Path.DirectorySeparatorChar + "SDVLibs";

        // TODO: Figure out which DLLs are actually needed
        private static readonly string[] DllsToCopy = {
            "BmFont",
            "GalaxyCSharp",
            "libSkiaSharp",
            "Lidgren.Network",
            "Mono.Posix",
            "Mono.Security",
            "MonoGame.Framework",
            "mscorlib",
            "SDL2",
            "SkiaSharp",
            "soft_oal",
            "StardewValley.GameData",
            "steam_api64",
            "Steamworks.NET",
            "System.Configuration",
            "System.Core",
            "System.Data",
            "System",
            "System.Drawing",
            "System.Runtime.Serialization",
            "System.Security",
            "System.Xml",
            "System.Xml.Linq",
            "WindowsBase",
            "xTile",
            "xTilePipeline",
            "MonoMod.Utils"
        };

        private static readonly string[] DirtyFiles = {
            "MONOMODDED_StardewValley.exe",
            "MONOMODDED_StardewValley.pdb",
            "StardewValley.exe"
        };

        private const string CorFlagsArgs = "/C CorFlags.exe MONOMODDED_StardewValley.exe /32BITREQ-";
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

            CopyRequiredDLLs(installationFolder);
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

        private static void CopyRequiredDLLs(string installPath)
        {
            Console.WriteLine("Copying required DLLs over to the installation location:");

            foreach (string dllName in DllsToCopy)
                try
                {
                    // TODO: Remove exec step
                    Console.WriteLine($" Copying {dllName}.dll -> exec directory...");
                    File.Copy(Path.Combine(LibPath, dllName + ".dll"), Path.Combine(ExePath, dllName + ".dll"), true);

                    Console.WriteLine($" Copying {dllName}.dll -> SDV directory...");
                    File.Copy(Path.Combine(LibPath, dllName + ".dll"), Path.Combine(installPath, dllName + ".dll"), true);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine($"Could not locate file: {e.FileName}");
                    Console.WriteLine("Falling back to previous prompt...");
                    Continue();
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine($"Could not locate directory: {e.Message}");
                    Console.WriteLine("Falling back to previous prompt...");
                    Continue();
                }

            // Is this separate loop required for waiting until all DLLs are copied?
            foreach (string dllName in DllsToCopy)
                while (!File.Exists(Path.Combine(installPath, dllName + ".dll")))
                    Console.ReadLine();

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

            RetryMMCopy:
            try
            {
                // Give it some time, honestly
                Thread.Sleep(1000 * 10);
                Console.WriteLine("Modifying MONOMODDED_StardewValley.exe flags with CorFlags..." +
                                  "\n(If this does not work, please re-launch with administrator privileges)");

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
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found... retrying...");

                goto RetryMMCopy;
            }

            Console.WriteLine("Copied the modified EXE over to the installation location!");
        }
    }
}
