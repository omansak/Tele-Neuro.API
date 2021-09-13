using System;
using Service.Document.Model;

namespace Service.Document.Image.ImageSharp
{
    public interface IDocumentImageService : IDocumentService
    {
        IDocumentImageService SetBaseFolder(string baseFolder);
        IDocumentImageService SetDirectory(string directory);
        IDocumentImageService SetQuality(int quality);
        IDocumentImageService SetSaveFormat(ImageFormat format);
        IDocumentImageService SetCompletedAction(Action<DocumentResult> completedAction);
    }
}
