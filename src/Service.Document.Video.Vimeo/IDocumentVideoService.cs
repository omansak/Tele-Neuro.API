using System;
using Service.Document.Model;

namespace Service.Document.Video.Vimeo
{
    public interface IDocumentVideoService : IDocumentService
    {
        DocumentVideoService SetCompletedAction(Action<DocumentResult> completedAction);
    }
}
