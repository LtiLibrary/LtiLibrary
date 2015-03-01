namespace LtiLibrary.Core.Common
{
    public interface IJsonLdObject
    {
        object Context { get; set; }
        string Id { get; set; }
        string Type { get; }
    }
}