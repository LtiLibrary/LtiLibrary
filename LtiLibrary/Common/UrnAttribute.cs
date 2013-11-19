using System;

namespace LtiLibrary.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class UrnAttribute : Attribute
    {
        public UrnAttribute(string urn)
        {
            Urn = urn;
        }

        public string Urn { get; set; }
    }
}
