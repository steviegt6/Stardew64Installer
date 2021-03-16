using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Stardew64Installer
{
    public static class Program
    {
        private static string ExePath =>
            Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        private static string LibPath => ExePath + Path.DirectorySeparatorChar + "SDVLibs";

        private const string SDL2Name = "SDL2.dll";
        private const string soft_oalName = "soft_oal.dll";
        private const string SteamworksDLLName = "Steamworks.NET.dll";
        private const string CMDCorFlagsInfo = "/C CorFlags.exe Steamworks.NET.dll /32BITREQ-";
        private const string CMDMMInfo = "/C MonoMod.dll StardewValley.exe";
        private const string MMExeName = "MONOMODDED_StardewValley.exe";
        private const string ExeName = "Stardew Valley.exe";

        private static void Main()
        {
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
                
            Console.WriteLine();
        }

        private static void ParseFailure()
        {
            Console.WriteLine();
            Console.WriteLine("We failed to parse your response. Please choose a number between 1 and 2.");
            Prompt();
        }

        private static void DepotDownloadMessage()
        {
            Console.WriteLine();
            Console.WriteLine("Please download DepotDownloader through https://gyazo.com/98e79051032c7efe3d22feb50110a29d");
            Console.ReadLine();
        }

        private static void Continue()
        {
            if (!File.Exists("StardewValley.exe"))
            {
                Console.WriteLine();
                Console.WriteLine("No executable file with the name \"StardewValley.exe\" was found in the same directory as Stardew64Installer!");
                Console.ReadLine();
                return;
            }

            string installationFolder = WriteReadLine("Please provide the location of the Steam installation for Stardew Valley:");

            CorFlagSteamworks(installationFolder);
            CopyRequiredDLLs(installationFolder);
            ApplyMonoModPatches(installationFolder);
            Console.ReadLine();
        }

        private static string WriteReadLine(string value)
        {
            Console.WriteLine(value);
            Console.WriteLine();
            return Console.ReadLine();
        }

        private static void CorFlagSteamworks(string installPath)
        {
            Console.WriteLine("Copying Steamworks.NET.dll...");
            string dllPath = Path.Combine(installPath, SteamworksDLLName);
            string newPath = Path.Combine(ExePath, SteamworksDLLName);

            File.Copy(dllPath, newPath, true);

            Console.WriteLine("Modifying DLL flags with CorFlags..." +
                              "\n(This comes pre-installed with Visual Studio, if you do not have it installed, please download it.)");
            new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden, 
                    FileName = "cmd.exe",
                    Arguments = CMDCorFlagsInfo
                }
            }.Start();

            Console.WriteLine("Copying the modified DLL over to the installation location...");
            File.Copy(newPath, dllPath, true);
        }

        private static void CopyRequiredDLLs(string installPath)
        {
            Console.WriteLine("Copying required DLLs over to the installation location...");
            File.Copy(Path.Combine(LibPath, soft_oalName), Path.Combine(installPath, soft_oalName), true);
            File.Copy(Path.Combine(LibPath, SDL2Name), Path.Combine(installPath, SDL2Name), true);
        }

        private static void ApplyMonoModPatches(string installPath)
        {
            Console.WriteLine("Applying MonoMod patches...");

            new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = CMDMMInfo
                }
            }.Start();

            while (!File.Exists(Path.Combine(ExePath, MMExeName))) 
                Console.ReadLine();

            Console.WriteLine("Copying the modified EXE over to the installation location...");
            File.Copy(Path.Combine(ExePath, MMExeName), Path.Combine(installPath, ExeName), true);
        }
    }
}
