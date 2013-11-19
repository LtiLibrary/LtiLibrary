using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Token
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}
