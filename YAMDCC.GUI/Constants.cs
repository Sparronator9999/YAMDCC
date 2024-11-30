using System;
using System.IO;

namespace YAMDCC.GUI
{
    internal static class Constants
    {
        /// <summary>
        /// The path where program data is stored.
        /// </summary>
        /// <remarks>
        /// (C:\ProgramData\Sparronator9999\YAMDCC on Windows)
        /// </remarks>
        public static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Sparronator9999", "YAMDCC");
    }
}
