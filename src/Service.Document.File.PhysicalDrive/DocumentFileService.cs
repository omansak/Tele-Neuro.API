using Service.Document.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Service.Document.File.PhysicalDrive
{
    public class DocumentFileService : IDocumentFileService
    {
        public string BaseFolder { get; private set; }
        public string Directory { get; private set; }
        public Action<DocumentResult> CompletedAction { get; private set; }
        public string FolderPath => Path.Join(BaseFolder, Directory);

        public DocumentFileService(string baseFolder = "", string directory = "", Action<DocumentResult> completedAction = null)
        {
            BaseFolder = baseFolder;
            Directory = directory;
            CompletedAction = completedAction;
        }

        public DocumentFileService SetBaseFolder(string baseFolder)
        {
            BaseFolder = baseFolder;
            return this;
        }

        public DocumentFileService SetDirectory(string directory)
        {
            Directory = directory;
            return this;
        }

        public DocumentFileService SetCompletedAction(Action<DocumentResult> completedAction)
        {
            CompletedAction = completedAction;
            return this;
        }

        public async Task<DocumentResult> SaveAsync(string filePath, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            using (var stream = new StreamReader(filePath))
            {
                return await UploadAsync(stream.BaseStream, fileName, mimeType, completed);
            }
        }

        public async Task<DocumentResult> SaveAsync(byte[] data, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            using (var stream = new MemoryStream(data))
            {
                return await UploadAsync(stream, fileName, mimeType, completed);
            }
        }

        public async Task<DocumentResult> SaveAsync(Stream stream, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            return await UploadAsync(stream, fileName, mimeType, completed);
        }

        public async Task<DocumentResult> SaveAsync(Uri uri, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            using (var wc = new System.Net.WebClient())
            {
                return await SaveAsync(wc.DownloadData(uri.AbsoluteUri), fileName, mimeType, completed);
            }
        }

        private async Task<DocumentResult> UploadAsync(Stream stream, string fileName, string mimeType, Action<DocumentResult> completed = null)
        {
            mimeType ??= MimeTypeAssistant.GetMimeType(fileName);

            if (string.IsNullOrWhiteSpace(mimeType))
                throw new ArgumentNullException(nameof(mimeType));

            string guid = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(fileName).ToLowerInvariant().Replace(".", "");
            string name = $"{guid}.{extension}";
            string path = GetFilePath(name);

            if (!System.IO.Directory.Exists(FolderPath))
                System.IO.Directory.CreateDirectory(FolderPath);

            await using (FileStream fileStream = System.IO.File.Create(path, (int)stream.Length))
            {
                byte[] bytesInStream = new byte[stream.Length];
                await stream.ReadAsync(bytesInStream, 0, bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }

            var result = new DocumentResult
            {
                Guid = guid,
                Name = name,
                FileName = fileName ?? name,
                Extension = extension,
                ContentType = mimeType,
                Type = MimeTypeAssistant.GetDocumentType(mimeType), // TODO Detect type
                DocumentPath = new DocumentPath
                {
                    Base = BaseFolder,
                    Directory = Directory,
                    Path = Path.Join(Directory, name),
                    FullPath = path,
                },
                CreatedDate = DateTime.Now
            };

            if (completed != null)
            {
                completed(result);
            }

            if (CompletedAction != null)
            {
                CompletedAction(result);
            }

            return result;
        }

        private string GetFilePath(string fileName)
        {
            return Path.Join(FolderPath, fileName);
        }
    }
}
