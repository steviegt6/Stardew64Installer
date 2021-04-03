using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using MonoMod;
using Stardew64Installer.Shared;

// ReSharper disable once CheckNamespace
namespace Stardew64Installer.Patches.MonoGameFramework
{
    /// <summary>Patches <see cref="ContentManager.Load{T}"/> to normalize assets in the Windows format.</summary>
    [MonoModPatch("global::Microsoft.Xna.Framework.Content.ContentManager")]
    internal class ContentManagerPatches : ContentManager
    {
        /// <inheritdoc />
        public ContentManagerPatches(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        /// <inheritdoc />
        public ContentManagerPatches(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory) { }

        /// <inheritdoc />
        public override T Load<T>(string assetName)
        {
            // get private fields
            bool disposed = (bool)PatchHelper.RequireField(typeof(ContentManager), "disposed").GetValue(this);
            var loadedAssets = (Dictionary<string, object>)PatchHelper.RequireField(typeof(ContentManager), "loadedAssets").GetValue(this);

            // replicate game logic
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));
            if (disposed)
                throw new ObjectDisposedException(nameof(ContentManager));

            // change key normalization
            string key;
            {
                Type type = PatchHelper.RequireType("StardewModdingAPI.Utilities.PathUtilities, StardewModdingAPI");
                MethodInfo method = PatchHelper.RequireMethod(type, "NormalizePath");
                key = (string)method.Invoke(null, new object[] { assetName });
            }

            // replicate game logic
            if (loadedAssets.TryGetValue(key, out var value) && value is T result)
                return result;
            T val = this.ReadAsset<T>(assetName, null);
            loadedAssets[key] = val;
            return val;
        }
    }
}
