using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using MonoMod;

namespace StardewValley.Patches.mm
{
    // ReSharper disable once ArrangeTypeModifiers
    [MonoModPatch("global::StardewValley.KeyboardDispatcher.ctor")]
    class CtorKeyboardDispatcherPatch : KeyboardDispatcher
    {
        [KeyboardDispatcherCtor]
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public extern void orig_ctor();

        public CtorKeyboardDispatcherPatch(GameWindow window) : base(window) => orig_ctor();
    }
}
