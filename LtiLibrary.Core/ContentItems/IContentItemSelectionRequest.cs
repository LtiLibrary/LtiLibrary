namespace LtiLibrary.Core.ContentItems
{
    /// <summary>
    /// LTI Content-Item Message 1.0 ContentItemSelectionRequest interface (3 Feb 2015 draft)
    /// </summary>
    public interface IContentItemSelectionRequest
    {
        string AcceptMediaTypes { get; set; }
        bool? AcceptMultiple { get; set; }
        string AcceptPresentationDocumentTargets { get; set; }
        bool? AcceptUnsigned { get; set; }
        bool? AutoCreate { get; set; }
        bool? CanConfirm { get; set; }
        string ContentItemReturnUrl { get; set; }
        string Data { get; set; }
        string Text { get; set; }
        string Title { get; set; }
    }
}