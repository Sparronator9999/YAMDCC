using System.Globalization;
using System.Resources;

namespace MSIFanControl.GUI
{
    /// <summary>
    /// A resource class for retrieving strings.
    /// </summary>
    internal static class Strings
    {
        private static ResourceManager resMan = null;

        /// <summary>
        /// Gets a string from the underlying resource file.
        /// </summary>
        /// <remarks>
        /// This function internally calls
        /// <see cref="ResourceManager.GetString(string)"/> to retrieve the string.
        /// </remarks>
        /// <param name="name">
        /// The name of the string to find.
        /// </param>
        /// <returns>
        /// <para>The value of the specified string name, if found.</para>
        /// <para><c>null</c> if the string couldn't be found.</para>
        /// </returns>
        internal static string GetString(string name)
        {
            if (resMan is null)
            {
                resMan = new ResourceManager(typeof(Strings));
            }
            return resMan.GetString(name, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a string from the underlying resource file, and replaces format
        /// items with the specified object's string representation.
        /// </summary>
        /// <param name="name">
        /// The name of the string to find.
        /// </param>
        /// <param name="arg0">
        /// The object to format the string with.
        /// </param>
        /// <returns>
        /// <para>The formatted string corresponding to the specified string name, if found.</para>
        /// <para><c>null</c> if the string couldn't be found.</para>
        /// </returns>
        internal static string GetString(string name, object arg0)
        {
            string temp = GetString(name);
            return temp is null
                ? null
                : string.Format(temp, arg0);
        }

        /// <inheritdoc cref="GetString(string)"/>
        /// <param name="args">
        /// The objects to format the string with.
        /// </param>
        internal static string GetString(string name, params object[] args)
        {
            string temp = GetString(name);
            return temp is null
                ? null
                : string.Format(temp, args);
        }
    }
}
