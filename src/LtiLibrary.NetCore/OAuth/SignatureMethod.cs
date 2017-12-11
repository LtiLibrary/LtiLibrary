using System;
using System.Collections.Generic;
using System.Text;

namespace LtiLibrary.NetCore.OAuth
{
    public enum SignatureMethod
    {
        HmacSha1,
        HmacSha256,
        HmacSha384,
        HmacSha512,
    }
}
