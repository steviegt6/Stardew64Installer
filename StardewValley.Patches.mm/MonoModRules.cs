using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using StardewValley;

// ReSharper disable once CheckNamespace
namespace MonoMod
{
    [MonoModCustomMethodAttribute("Initialize")]
    public class Game1InitializeAttribute : Attribute { }

    [MonoModCustomMethodAttribute("Ctor")]
    public class KeyboardDispatcherCtorAttribute : Attribute { }

    // ReSharper disable once ArrangeTypeModifiers
    static class MonoModRules
    {
        public static void Initialize(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
                return;
            return;
            ILCursor c = new ILCursor(new ILContext(method));

            for (int i = 0; i < 3; i++)
                c.TryGotoNext(il => il.MatchLdstr("FarmerSounds.xgs"));

            c.Index += 5;
            c.RemoveRange(25);

            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Action<string>>(rootDirectory =>
                                           {
                                               Game1.soundBank = new SoundBankWrapper(
                                                   new SoundBank(Game1.audioEngine.Engine,
                                                       Path.Combine(rootDirectory, "XACT", "Sound Bank.xsb")));
                                               Game1.waveBank = new WaveBank(Game1.audioEngine.Engine,
                                                   Path.Combine(rootDirectory, "XACT", "Wave Bank.xwb"));
                                               Game1.waveBank1_4 = new WaveBank(Game1.audioEngine.Engine,
                                                   Path.Combine(rootDirectory, "XACT", "Wave Bank(1.4).xwb"));
                                           });

            // TODO: c.TryGotoNext(il => il.MatchNewobj("StardewValley.DummyAudioEngine"));
        }

        public static void Ctor(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
                return;
            return;
            ILCursor c = new ILCursor(new ILContext(method));

            c.TryGotoNext(il => il.MatchRet());
            c.Index++;

            c.Emit(OpCodes.Ldarg_1);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldftn, typeof(KeyboardDispatcher).GetMethod("Event_TextInput", BindingFlags.Instance | BindingFlags.NonPublic));
            c.Emit(OpCodes.Newobj, typeof(TextInputEventArgs).GetConstructor(BindingFlags.Public, null, new[] { typeof(object), typeof(
                Keys) }, null));
            c.Emit(OpCodes.Callvirt, typeof(GameWindow).GetMethod("add_TextInput", BindingFlags.Instance | BindingFlags.NonPublic));

            c.TryGotoNext(il => il.MatchLdarg(0));
            c.Emit(OpCodes.Ret);
        }
    }
}
