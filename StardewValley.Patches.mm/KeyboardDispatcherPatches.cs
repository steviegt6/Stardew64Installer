using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod;
using Stardew64Installer.Framework;
using StardewValley;

namespace Stardew64Installer.Patches.StardewValley
{
    /// <summary>A MonoMod patch that reimplements the <see cref="KeyboardDispatcher"/> constructor to remove a Linux-only check around the <see cref="GameWindow.TextInput"/> event set, ensures that <see cref="KeyboardInput.Initialize"/> is called, and removes the unneeded <see cref="KeyboardInput.CharEntered"/> and <see cref="KeyboardInput.KeyDown"/> events.</summary>
    [MonoModPatch("global::StardewValley.KeyboardDispatcher")]
    internal class KeyboardDispatcherPatches : KeyboardDispatcher
    {
        [MonoModConstructor]
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public void ctor(GameWindow window)
        {
            _commandInputs = new List<char>();
            _keysDown = new List<Keys>();
            _charsEntered = new List<char>();
            _window = window;

            PatchHelper.RequireField(typeof(KeyboardDispatcherPatches), "_pasteResult").SetValue(this, "");

            // https://stackoverflow.com/questions/1121441/addeventhandler-using-reflection
            // https://stackoverflow.com/questions/11120401/creating-delegate-from-methodinfo
            EventInfo windowTextInput = PatchHelper.RequireEvent(typeof(GameWindow), "TextInput");
            Delegate handler = Delegate.CreateDelegate(windowTextInput.EventHandlerType, this, "Event_TextInput");
            windowTextInput.AddEventHandler(window, handler);

            KeyboardInput.Initialize(window);
        }

        /// <inheritdoc />
        public KeyboardDispatcherPatches(GameWindow window)
            : base(window) { }
    }
}
