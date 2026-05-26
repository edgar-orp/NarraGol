using System;
using System.IO;
using System.Runtime.InteropServices;

namespace dev.xrlab.NarraGol.Utility
{
    public static class WindowsFolders
    {
        private static readonly Guid DownloadsFolderGuid =
            new Guid("374DE290-123F-4565-9164-39C4925E467B");

        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr ppszPath
        );

        public static string GetDownloadsFolder()
        {
            int result = SHGetKnownFolderPath(
                DownloadsFolderGuid,
                0,
                IntPtr.Zero,
                out IntPtr outPath
            );

            if (result != 0)
            {
                UnityEngine.Debug.LogWarning("Could not resolve Windows Downloads folder. Using fallback path.");

                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );
            }

            string path = Marshal.PtrToStringUni(outPath);

            Marshal.FreeCoTaskMem(outPath);

            return path;
        }
    }
}