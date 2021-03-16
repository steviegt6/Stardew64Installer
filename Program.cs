using System;

namespace Stardew64Installer
{
    public class Program
    {
        private static void Main()
        {
            Console.WriteLine(" Do you want to use DepotDownloader through this program to install the Linux version of Stardew Valley?");
            Console.WriteLine("  Please note that this will require your Steam login credentials");
            Console.WriteLine("  If you do not feel comfortable providing this information, please download it manually with DepotDownloader manually.");

            DepotPrompt:
            Console.WriteLine();
            Console.WriteLine(" [1] Install through DepotDownloader" +
                              "\n [2] Skip DepotDownloader (provide the EXE in the same folder as this executable)");

            string option = Console.ReadLine();

            if (int.TryParse(option, out int optionNum))
            {
                if (optionNum < 1 || optionNum > 2)
                    goto ParseFailure;

                Console.WriteLine();
                return;
            }

            ParseFailure:
            Console.WriteLine();
            Console.WriteLine("We failed to parse your response. Please choose a number between 1 and 2.");
            goto DepotPrompt;
        }
    }
}
