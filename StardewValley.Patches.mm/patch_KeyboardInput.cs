using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using StardewValley.Patches.mm.Framework;

// ReSharper disable once CheckNamespace
namespace StardewValley
{
    /// <summary>A MonoMod patch that swaps 'SetWindowLong' (which is 32-bit only) to 'SetWindowLongPtr'.</summary>
    static class patch_KeyboardInput
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);

        public static void Initialize(GameWindow window)
        {
            // fetch private members
            Type type = typeof(KeyboardInput);
            FieldInfo initialized = PatchHelper.RequireField(type, "initialized");
            FieldInfo prevWndProc = PatchHelper.RequireField(type, "prevWndProc");
            FieldInfo hookProcDelegate = PatchHelper.RequireField(type, "hookProcDelegate");
            FieldInfo himc = PatchHelper.RequireField(type, "hIMC");
            Delegate hookProc = Delegate.CreateDelegate(type: PatchHelper.RequireNestedType(type, "WndProc"), firstArgument: null, method: PatchHelper.RequireMethod(type, "HookProc"), throwOnBindFailure: true);

            // replicate game logic, but swap out SetWindowLong
            if ((bool)initialized.GetValue(null))
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");
            hookProcDelegate.SetValue(null, hookProc);
            prevWndProc.SetValue(null, (IntPtr)SetWindowLongPtr(window.Handle, -4, Marshal.GetFunctionPointerForDelegate((Delegate)hookProcDelegate.GetValue(null))));
            himc.SetValue(null, ImmGetContext(window.Handle));
            initialized.SetValue(null, true);
        }
    }
}
