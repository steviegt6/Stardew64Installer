using System;
using System.Diagnostics.CodeAnalysis;
using MonoMod;

namespace StardewValley.Patches.mm
{
    // ReSharper disable once ArrangeTypeModifiers
    class patch_Game1 : Game1
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public extern void orig_Initialize();

        protected override void Initialize()
        {
            Console.WriteLine("AAAAAAAAAAAAAAAAAAA");
            //orig_Initialize();
        }
    }
}
