namespace LtiLibrary.ContentItems
{
    public interface IContentItemSelectionRequest
    {
        bool? AcceptCopyAdvice { get; set; }
        string AcceptMediaTypes { get; set; }
        bool? AcceptMultiple { get; set; }
        string AcceptPresentationDocumentTargets { get; set; }
        bool? AcceptUnsigned { get; set; }
        string ContentItemReturnUrl { get; set; }
        string ContentItemServiceUrl { get; set; }
        string Data { get; set; }
        string Text { get; set; }
        string Title { get; set; }
    }
}