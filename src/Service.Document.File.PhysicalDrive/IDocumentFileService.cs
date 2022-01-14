using Service.Document.Model;
using System;

namespace Service.Document.File.PhysicalDrive
{
    public interface IDocumentFileService : IDocumentService
    {
        DocumentFileService SetBaseFolder(string baseFolder);
        DocumentFileService SetDirectory(string directory);
        DocumentFileService SetCompletedAction(Action<DocumentResult> completedAction);
    }
}
