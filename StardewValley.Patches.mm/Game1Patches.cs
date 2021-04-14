using MonoMod;
using Stardew64Installer.Framework;

namespace Stardew64Installer.Patches.StardewValley
{
    /// <summary>A MonoMod patch that adds a patch version field.</summary>
    [MonoModPatch("global::StardewValley.Game1")]
    internal static class Game1Patches
    {
        /// <summary>The version of this patch tool.</summary>
        public static string Stardew64InstallerVersion => Constants.Stardew64InstallerVersion;
    }
}
