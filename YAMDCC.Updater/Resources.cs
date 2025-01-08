using System.Globalization;
using System.Resources;

namespace YAMDCC.Updater;

internal static class Resources
{
    private static ResourceManager _resMan;

    private static ResourceManager ResMan
    {
        get
        {
            _resMan ??= new ResourceManager(typeof(Resources));
            return _resMan;
        }
    }

    public static byte[] GetObject(string name)
    {
        return (byte[])ResMan.GetObject(name, CultureInfo.InvariantCulture);
    }

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
    public static string GetString(string name)
    {
        return ResMan.GetString(name, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets a string from the underlying resource file, and
    /// replaces format objects with their string representation.
    /// </summary>
    /// <param name="name">
    /// The name of the string to find.
    /// </param>
    /// <param name="arg0">
    /// The object to format the string with.
    /// </param>
    /// <returns>
    /// <para>
    /// The formatted string corresponding to
    /// the specified string name, if found.
    /// </para>
    /// <para><c>null</c> if the string couldn't be found.</para>
    /// </returns>
    public static string GetString(string name, object arg0)
    {
        string temp = GetString(name);
        return temp is null
            ? null
            : string.Format(CultureInfo.InvariantCulture, temp, arg0);
    }

    /// <summary>
    /// Gets a string from the underlying resource file, and
    /// replaces format objects with their string representation.
    /// </summary>
    /// <param name="name">
    /// The name of the string to find.
    /// </param>
    /// <param name="args">
    /// The objects to format the string with.
    /// </param>
    /// <returns>
    /// <para>
    /// The formatted string corresponding to
    /// the specified string name, if found.
    /// </para>
    /// <para><c>null</c> if the string couldn't be found.</para>
    /// </returns>
    public static string GetString(string name, params object[] args)
    {
        string temp = GetString(name);
        return temp is null
            ? null
            : string.Format(CultureInfo.InvariantCulture, temp, args);
    }
}
