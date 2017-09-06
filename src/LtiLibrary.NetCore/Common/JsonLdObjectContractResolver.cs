﻿using System;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// DefaultContractResolver with null Converter for JsonLdObjects
    /// to prevent recusive calls to JsonLdObjectConverter.
    /// </summary>
    internal class JsonLdObjectContractResolver : DefaultContractResolver
    {
        private readonly bool _unsetConverter;

        public JsonLdObjectContractResolver()
         : this( false )
        {
        }

        public JsonLdObjectContractResolver( bool unsetConverter )
        {
            _unsetConverter = unsetConverter;
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (typeof(JsonLdObject).GetTypeInfo().IsAssignableFrom(type))
            {
                var contract = base.ResolveContract(type);
                
                // Set the Convert to null to prevent recursive call to JsonLdObjectConverter
                if ( _unsetConverter )
                {
                    contract.Converter = null;
                }
                
                return contract;
            }
            return base.ResolveContract(type);
        }

    }
}
