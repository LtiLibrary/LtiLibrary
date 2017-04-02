using System;
using System.Collections.Generic;

namespace LtiLibrary.NetCore.Common
{
    public interface IJsonLdObject
    {
        Uri ExternalContextId { get; set; }
        IDictionary<string, string> Terms { get; }
        Uri Id { get; set; }
        string Type { get; set; }
    }
}