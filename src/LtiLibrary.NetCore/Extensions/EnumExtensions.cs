using System;
using System.Linq;
using System.Reflection;
using LtiLibrary.NetCore.Common;

namespace LtiLibrary.NetCore.Extensions
{
    /// <summary>
    /// Enum extensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Return the URN associated with this Enum value.
        /// </summary>
        public static string GetUrn(this Enum value)
        {
            return 
                value.GetType()
                .GetRuntimeFields()
                .SingleOrDefault(f => f.Name == value.ToString())
                ?.GetCustomAttribute<UrnAttribute>()
                ?.Urn;
        }
    }
}
