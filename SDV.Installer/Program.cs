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
        private static string ExePath =>
            Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        private static string LibPath => ExePath + Path.DirectorySeparatorChar + "SDVLibs";

        // TODO: Figure out which DLLs are actually needed
        private static readonly List<string> dllsToCopy = new List<string>
        {
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
            //"Steamworks.NET",
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

        private static readonly List<string> dirtyFiles = new List<string>
        {
            "MONOMODDED_StardewValley.exe",
            "MONOMODDED_StardewValley.pdb"
        };

        private const string SteamworksDLLName = "Steamworks.NET.dll";
        private const string CMDCorFlagsInfo = "/C CorFlags.exe Steamworks.NET.dll /32BITREQ-";
        private const string CMDMMInfo = "/C MonoMod.exe StardewValley.exe";
        private const string ExeName = "StardewValley.exe";
        private const string MMExeName = "MONOMODDED_" + ExeName;

        private static void Main()
        {
            Console.WriteLine("Cleaning out any MONOMODDED files...");

            foreach (string file in dirtyFiles.Where(file => File.Exists(Path.Combine(ExePath, file))))
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
            Console.WriteLine(" Please download DepotDownloader through https://github.com/SteamRE/DepotDownloader" +
                              "\n Press enter to exit...");
            Console.ReadLine();
        }

        private static void Continue()
        {
            Console.WriteLine();

            // TODO: Auto-detect install locations
            string installationFolder = WriteReadLine("Please provide the location of the depot-downloaded copy of Stardew Valley:");

            CorFlagSteamworks(installationFolder);
            CopyRequiredDLLs(installationFolder);
            ApplyMonoModPatches(installationFolder);
            Console.WriteLine();
            Console.WriteLine(" Installation complete! Please launch StardewValley.exe from the depot-download folder!" +
                              "\n Press enter to exit...");
            Console.ReadLine();
        }

        private static string WriteReadLine(string value)
        {
            Console.WriteLine(value);
            // Console.WriteLine();
            return Console.ReadLine();
        }

        private static void CorFlagSteamworks(string installPath)
        {
            Console.WriteLine("Copying Steamworks.NET.dll to executable directory...");
            string dllPath = Path.Combine(installPath, SteamworksDLLName);
            string newPath = Path.Combine(ExePath, SteamworksDLLName);

            File.Copy(dllPath, newPath, true);

            Console.WriteLine("Modifying Steamworks.NET.dll flags with CorFlags..." +
                              "\n(If this does not work, please re-launch with administrator privileges)");
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden, 
                    FileName = "cmd.exe",
                    Arguments = CMDCorFlagsInfo
                }
            }.Start();

            Console.WriteLine("Copying the modified Steamworks.NET.dll over to the installation location...");
            File.Copy(newPath, dllPath, true);
        }

        private static void CopyRequiredDLLs(string installPath)
        {
            Console.WriteLine("Copying required DLLs over to the installation location:");

            foreach (string dllName in dllsToCopy)
            {
                // TODO: Remove exec step
                Console.WriteLine($" Copying {dllName}.dll -> exec directory...");
                File.Copy(Path.Combine(LibPath, dllName + ".dll"), Path.Combine(ExePath, dllName + ".dll"), true);

                Console.WriteLine($" Copying {dllName}.dll -> SDV directory...");
                File.Copy(Path.Combine(LibPath, dllName + ".dll"), Path.Combine(installPath, dllName + ".dll"), true);
            }

            // Is this separate loop required for waiting until all DLLs are copied?
            foreach (string dllName in dllsToCopy)
                while (!File.Exists(Path.Combine(installPath, dllName + ".dll")))
                    Console.ReadLine();

            Console.WriteLine();
        }

        private static void ApplyMonoModPatches(string installPath)
        {
            Console.WriteLine("Applying MonoMod patches...");

            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = CMDMMInfo
                }
            }.Start();

            Console.WriteLine("Copying the modified EXE over to the installation location...");

            RetryMMCopy:
            try
            {
                // Give it some time, honestly
                Thread.Sleep(1000 * 15);
                File.Copy(Path.Combine(ExePath, MMExeName), Path.Combine(installPath, ExeName), true);
            }
            catch (FileNotFoundException)
            {
                goto RetryMMCopy;
            }

            Console.WriteLine("Copied the modified EXE over to the installation location!");
        }
    }
}
