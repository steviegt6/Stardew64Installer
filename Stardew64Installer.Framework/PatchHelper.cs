using System;
using System.Reflection;

namespace Stardew64Installer.Framework
{
    /// <summary>Provides utility methods to simplify patches.</summary>
    internal static class PatchHelper
    {
        /*********
        ** Fields
        *********/
        /// <summary>Binding flags which match all members.</summary>
        private const BindingFlags All = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;


        /*********
        ** Public methods
        *********/
        /// <summary>Get an event and assert that it's present.</summary>
        /// <param name="type">The type whose members to search.</param>
        /// <param name="name">The event name.</param>
        public static EventInfo RequireEvent(Type type, string name)
        {
            return type.GetEvent(name, PatchHelper.All) ?? throw new InvalidOperationException($"Can't find event '{name}' on {type.FullName}.");
        }

        /// <summary>Get a field and assert that it's present.</summary>
        /// <param name="type">The type whose members to search.</param>
        /// <param name="name">The field name.</param>
        public static FieldInfo RequireField(Type type, string name)
        {
            return type.GetField(name, PatchHelper.All) ?? throw new InvalidOperationException($"Can't find field '{name}' on {type.FullName}.");
        }

        /// <summary>Get a method and assert that it's present.</summary>
        /// <param name="type">The type whose members to search.</param>
        /// <param name="name">The method name.</param>
        public static MethodInfo RequireMethod(Type type, string name)
        {
            return type.GetMethod(name, PatchHelper.All) ?? throw new InvalidOperationException($"Can't find method '{name}' on {type.FullName}.");
        }

        /// <summary>Get a type and assert that it's present.</summary>
        /// <param name="name">The type and assembly name.</param>
        public static Type RequireType(string name)
        {
            return Type.GetType(name, throwOnError: false) ?? throw new InvalidOperationException($"Can't find type '{name}'.");
        }

        /// <summary>Attempts to get a type without asserting that it's present.</summary>
        /// <param name="name">The type and assembly name.</param>
        /// <param name="type">The requested type.</param>
        /// <returns>True if the type requests actually exists.</returns>
        public static bool RequestType(string name, out Type type)
        {
            type = Type.GetType(name, throwOnError: false);

            return type != null;
        }

        /// <summary>Get a nested type and assert that it's present.</summary>
        /// <param name="type">The type whose members to search.</param>
        /// <param name="name">The nested type name.</param>
        public static Type RequireNestedType(Type type, string name)
        {
            return type.GetNestedType(name, PatchHelper.All) ?? throw new InvalidOperationException($"Can't find nested type '{name}' on {type.FullName}.");
        }
    }
}
