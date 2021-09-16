using System;
using Service.Document.Model;

namespace Service.Document.Image.ImageSharp
{
    public interface IDocumentImageService : IDocumentService
    {
        DocumentImageService SetBaseFolder(string baseFolder);
        DocumentImageService SetDirectory(string directory);
        DocumentImageService SetQuality(int quality);
        DocumentImageService SetSaveFormat(ImageFormat format);
        DocumentImageService SetCompletedAction(Action<DocumentResult> completedAction);
    }
}
