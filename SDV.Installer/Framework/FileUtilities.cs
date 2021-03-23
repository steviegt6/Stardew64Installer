using System.IO;
using System.Threading;

namespace SDV.Installer.Framework
{
    /// <summary>Provides utility methods for file operations.</summary>
    internal static class FileUtilities
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Copy a file and wait until the copy operation completes.</summary>
        /// <param name="file">The file to copy.</param>
        /// <param name="toPath">The absolute destination file path.</param>
        public static void CopyToAndWait(this FileInfo file, string toPath)
        {
            file.CopyTo(toPath, overwrite: true);
            while (!File.Exists(toPath))
                Thread.Sleep(100);
        }

        /// <summary>Delete a file and wait until the deletion completes.</summary>
        /// <param name="file">The file to copy.</param>
        /// <returns>Returns whether the file existed and was deleted.</returns>
        public static bool DeleteAndWait(this FileInfo file)
        {
            if (!file.Exists)
                return false;

            file.Delete();
            while (File.Exists(file.FullName))
                Thread.Sleep(100);
            return true;
        }
    }
}
