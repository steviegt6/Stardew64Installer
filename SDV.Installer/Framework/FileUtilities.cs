using System;
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

        /// <summary>Recursively copy a directory and wait until the copy operation complete.</summary>
        /// <param name="source">The file or folder to copy.</param>
        /// <param name="toPath">The absolute destination folder path.</param>
        public static void RecursiveCopyTo(this DirectoryInfo source, string toPath)
        {
            // create target folder
            DirectoryInfo targetFolder = new DirectoryInfo(toPath);
            if (!targetFolder.Exists)
                targetFolder.Create();

            // copy entries
            foreach (FileSystemInfo entry in source.GetFileSystemInfos())
            {
                string targetName = Path.Combine(targetFolder.FullName, entry.Name);

                switch (entry)
                {
                    case FileInfo sourceFile:
                        sourceFile.CopyToAndWait(targetName);
                        break;

                    case DirectoryInfo sourceDir:
                        sourceDir.RecursiveCopyTo(targetName);
                        break;

                    default:
                        throw new NotSupportedException($"Unknown filesystem info type '{source.GetType().FullName}'.");
                }
            }
        }
    }
}
