using System.Diagnostics.CodeAnalysis;
using MonoMod;

namespace StardewValley.Patches.mm
{
    // ReSharper disable once ArrangeTypeModifiers
    [MonoModPatch("global::StardewValley.Game1.Initialize")]
    class InitializeGame1Patch : Game1
    {
        [Game1Initialize]
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public extern void orig_Initialize();

        protected override void Initialize() => orig_Initialize();
    }
}
