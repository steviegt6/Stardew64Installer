using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod;
using MonoMod.Utils;

// ReSharper disable once CheckNamespace
namespace StardewValley
{
    // ReSharper disable once ArrangeTypeModifiers
    [SuppressMessage("Style", "IDE1006:Naming Styles")]
    class patch_KeyboardDispatcher : KeyboardDispatcher
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public extern void orig_ctor(GameWindow window);

        // ReSharper disable once ArrangeTypeMemberModifiers
        const BindingFlags all = BindingFlags.NonPublic
                                    | BindingFlags.Public
                                    | BindingFlags.Instance
                                    | BindingFlags.Static;

        [MonoModConstructor]
        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public void ctor(GameWindow window)
        {
            _commandInputs = new List<char>();
            _keysDown = new List<Keys>();
            _charsEntered = new List<char>();
            _window = window;

            new DynData<KeyboardDispatcher>(this).Set("pastedResult", "");

            // https://stackoverflow.com/questions/1121441/addeventhandler-using-reflection
            // https://stackoverflow.com/questions/11120401/creating-delegate-from-methodinfo
            EventInfo windowTextInput = typeof(GameWindow).GetEvent("TextInput", all);
            Delegate handler = Delegate.CreateDelegate(windowTextInput.EventHandlerType, this, "Event_TextInput");
            windowTextInput.AddEventHandler(window, handler);

            KeyboardInput.Initialize(window);
        }

        public patch_KeyboardDispatcher(GameWindow window) : base(window) { }
    }
}
