namespace LtiLibrary.Core.ContentItems
{
    public interface IContentItemSelectionRequest
    {
        string AcceptMediaTypes { get; set; }
        bool? AcceptMultiple { get; set; }
        string AcceptPresentationDocumentTargets { get; set; }
        bool? AcceptUnsigned { get; set; }
        bool? AutoCreate { get; set; }
        bool? CanConfirm { get; set; }
        string ContentItemReturnUrl { get; set; }
        string ContentItemServiceUrl { get; set; }
        string Data { get; set; }
        string Text { get; set; }
        string Title { get; set; }
    }
}