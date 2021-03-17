//using System;
using System.Diagnostics.CodeAnalysis;
/*using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;*/

namespace StardewValley
{
    // ReSharper disable once ArrangeTypeModifiers
    [SuppressMessage("Style", "IDE1006:Naming Styles")]
    class patch_Game1 : Game1
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        //const BindingFlags all = BindingFlags.NonPublic
        //                         | BindingFlags.Public
        //                         | BindingFlags.Static
        //                         | BindingFlags.Instance;

        // ReSharper disable once ArrangeTypeMemberModifiers
        //static Type GameType => typeof(Game1);

        // ReSharper disable once ArrangeTypeMemberModifiers
        //readonly MethodInfo OnFadedBackInComplete = GameType.GetMethod("onFadedBackInComplete", all);

        // ReSharper disable once ArrangeTypeMemberModifiers
        //static bool OnFadeToBlackComplete(Game1 instance) =>
        //    (bool) GameType.GetMethod("onFadeToBlackComplete", all)?.Invoke(instance, null);

        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        public extern void orig_Initialize();

        protected override void Initialize()
        {
            // I don't see any reason to actually make any patches here
            // I have not encountered any issues with the current audio loading
            orig_Initialize();
            
            /*
            keyboardDispatcher = new KeyboardDispatcher(Window);

            bool OnFadeToBlack() => OnFadeToBlackComplete(this);

            void OnFadeIn()
            {
                OnFadedBackInComplete.Invoke(null, null);
            }

            GameType.GetField("screenFade",
                    all)
                ?.SetValue(
                    null,
                    new ScreenFade(
                        OnFadeToBlack,
                        OnFadeIn)
                );

            options = new Options
            {
                musicVolumeLevel = 1f,
                soundVolumeLevel = 1f
            };

            otherFarmers = new NetRootDictionary<long, Farmer>
            {
                Serializer = SaveGame.farmerSerializer
            };

            viewport = new Rectangle(
                new Size(graphics.PreferredBackBufferWidth,
                    graphics.PreferredBackBufferHeight)
            );

            string rootDirectory = Content.RootDirectory;

            if (!File.Exists(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "..",
                        "Resources",
                        rootDirectory,
                        "XACT",
                        "FarmerSounds.xgs")
                )
            )
            {
                File.Exists(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        rootDirectory,
                        "XACT",
                        "FarmerSounds.xgs")
                );
            }

            if (IsMainInstance)
            {
                try
                {
                    audioEngine = GameType.Assembly.GetType("StardewValley.AudioEngineWrapper")
                        .GetConstructor(all,
                            null,
                            new[]
                            {
                                typeof(AudioEngine)
                            },
                            null)
                        ?.Invoke(all,
                            null,
                            new object[]
                            {
                                new AudioEngine(Path.Combine(
                                    rootDirectory,
                                    "XACT",
                                    "FarmerSounds.xgs")
                                )
                            },
                            CultureInfo.CurrentCulture) as IAudioEngine;

                    soundBank = new SoundBankWrapper(new SoundBank(audioEngine.Engine,
                            Path.Combine(
                                rootDirectory,
                                "XACT",
                                "Sound Bank.xsb")
                        )
                    );

                    waveBank = new WaveBank(audioEngine.Engine,
                        Path.Combine(
                            rootDirectory,
                            "XACT",
                            "Wave Bank.xwb"));

                    waveBank1_4 = new WaveBank(audioEngine.Engine,
                        Path.Combine(
                            rootDirectory,
                            "XACT",
                            "Wave Bank(1.4).xwb")
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine("Game.Initialize() caught exception initializing XACT:\n{0}", e);

                    if (audioEngine == null)
                        audioEngine = GameType.Assembly.GetType("StardewValley.DummyAudioEngine")
                            .GetConstructor(
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |
                                BindingFlags.Instance,
                                null,
                                new Type[0],
                                null)
                            ?.Invoke(BindingFlags.NonPublic,
                                null,
                                new object[0],
                                CultureInfo.CurrentCulture) as IAudioEngine;

                    if (soundBank == null)
                        soundBank = new DummySoundBank();
                }
            }

            audioEngine?.Update();

            musicCategory = audioEngine?.GetCategory("Music");
            soundCategory = audioEngine?.GetCategory("Sound");
            ambientCategory = audioEngine?.GetCategory("Ambient");
            footstepCategory = audioEngine?.GetCategory("Footsteps");
            currentSong = null;

            if (soundBank != null)
            {
                wind = soundBank.GetCue("wind");
                chargeUpSound = soundBank.GetCue("toolCharge");
            }

            int width = graphics.GraphicsDevice.Viewport.Width;
            int height = graphics.GraphicsDevice.Viewport.Height;
            screen = new RenderTarget2D(graphics.GraphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            GameType.GetMethod(
                    "allocateLightMap",
                    BindingFlags.Static
                    | BindingFlags.NonPublic)
                ?.Invoke(
                    null,

                    new object[]
                    {
                        width,
                        height
                    });

            AmbientLocationSounds.InitShared();

            previousViewportPosition = Vector2.Zero;

            PushUIMode();
            PopUIMode();
            setRichPresence("menus");*/
        }
    }
}
