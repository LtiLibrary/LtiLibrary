using System.Collections.Specialized;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.OAuth;
using Xunit;

namespace LtiLibrary.NetCore.Tests
{
    public class NameValueCollectionShould
    {
        private readonly NameValueCollection _collection;

        private readonly string _expected;

        /// <summary>
        /// A NameValueCollection is used to collect LTI request parameters
        /// and ToNormalizedString is used convert that parameters into
        /// a normalized string for the purpose of constructing a signature
        /// base string per RFC5849.
        /// </summary>
        public NameValueCollectionShould()
        {
            _collection =
                new NameValueCollection
                {
                    {"b5", "=%3D"},
                    {"a3", "a"},
                    {"c@", ""},
                    {"a2","r b"},
                    {OAuthConstants.ConsumerKeyParameter, "9djdj82h48djs9d2"},
                    {OAuthConstants.TokenParameter, "kkk9d7dh3k39sjv7"},
                    {OAuthConstants.SignatureMethodParameter, OAuthConstants.SignatureMethodHmacSha1},
                    {OAuthConstants.TimestampParameter, "137131201"},
                    {OAuthConstants.NonceParameter, "7d8f3e4a"},
                    {"c2", ""},
                    {"a3", "2 q"}
                };

            _expected =
                "a2=r%20b&a3=2%20q&a3=a&b5=%3D%253D&c%40=&c2=&oauth_consumer_key=9dj"
                + "dj82h48djs9d2&oauth_nonce=7d8f3e4a&oauth_signature_method=HMAC-SHA1"
                + "&oauth_timestamp=137131201&oauth_token=kkk9d7dh3k39sjv7";
        }

        [Fact]
        // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.2
        // Sort the parameters by name, then value
        public void ReturnNormalizedStringPerRfc5849()
        {
            Assert.Equal(_expected, _collection.ToNormalizedString());
        }

        [Fact]
        // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.1
        // Exclude the OAuth signature or realm in the signature base string
        public void NotIncludeSignatureOrRealmInNormalizedString()
        {
            var collection = new NameValueCollection(_collection)
            {
                {OAuthConstants.SignatureParameter, "signature"},
                {OAuthConstants.RealmParameter, "realm"}
            };
            Assert.Equal(_expected, collection.ToNormalizedString());
        }

        [Fact]
        public void ShouldIncludeParametersWithNullValues()
        {
            var collection = new NameValueCollection(_collection)
            {
                // These create a key, but do not add values
                {"z", null}, 
                {"z", null},
                {"z", null}
            };
            Assert.Equal(_expected, collection.ToNormalizedString());
        }

        [Fact]
        public void ShouldIncludeParametersWithEmptyAndWhiteSpaceValues()
        {
            var collection = new NameValueCollection(_collection)
            {
                // This creates a Key, but does not add a value
                {"z", null},
                // This adds the first value to the existing "z" key
                {"z", string.Empty},
                // This adds the second value to the existing "z" key
                {"z", " "}
            };
            // Note that NameValueCollection does keep track of the null
            // value. It looks like there are two values for the "z" key
            var expected = _expected + "&z=&z=%20";
            Assert.Equal(expected, collection.ToNormalizedString());
        }
    }
}
