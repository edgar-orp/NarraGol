using System.IO;
using UnityEngine;

namespace dev.xrlab.NarraGol.Utility
{
    public static class NarraGolPaths
    {
        public static string GetAppRootFolder()
        {
#if UNITY_EDITOR
            return Directory.GetParent(Application.dataPath).FullName;
#else
            return Directory.GetParent(Application.dataPath).FullName;
#endif
        }

        public static string GetVideosFolder()
        {
            string path = Path.Combine(
                GetAppRootFolder(),
                "Videos"
            );

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Created Videos folder: {path}");
            }

            return path;
        }

        public static string GetVideoPath(string fileName)
        {
            return Path.Combine(
                GetVideosFolder(),
                fileName
            );
        }
    }
}