using Service.Document.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Document.Image.ImageSharp
{
    public class DocumentImageService : IDocumentImageService
    {
        public string BaseFolder { get; private set; }
        public string Directory { get; private set; }
        public ImageFormat SaveFormat { get; private set; }
        public int Quality { get; private set; }
        public Action<DocumentResult> CompletedAction { get; private set; }
        public string FolderPath => Path.Join(BaseFolder, Directory);

        public DocumentImageService(string baseFolder = "", string directory = "", ImageFormat saveFormat = ImageFormat.Default, int quality = 100, Action<DocumentResult> completedAction = null)
        {
            BaseFolder = baseFolder;
            Directory = directory;
            SaveFormat = saveFormat;
            Quality = quality;
            CompletedAction = completedAction;
        }

        public DocumentImageService SetBaseFolder(string baseFolder)
        {
            BaseFolder = baseFolder;
            return this;
        }
        public DocumentImageService SetDirectory(string directory)
        {
            Directory = directory;
            return this;
        }
        public DocumentImageService SetSaveFormat(ImageFormat format)
        {
            SaveFormat = format;
            return this;
        }
        public DocumentImageService SetCompletedAction(Action<DocumentResult> completedAction)
        {
            CompletedAction = completedAction;
            return this;
        }
        public DocumentImageService SetQuality(int quality)
        {
            Quality = quality;
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
        private async Task<DocumentResult> UploadAsync(Stream stream, string fileName = null, string mimeType = null, Action<DocumentResult> completed = null)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(Configuration.Default, stream, out IImageFormat format))
            {
                string extension = format?.FileExtensions?.FirstOrDefault();
                string guid = Guid.NewGuid().ToString();
                string name = $"{guid}.{extension}";
                string path = GetFilePath(name);
                System.IO.Directory.CreateDirectory(FolderPath);
                // Image Processing --> image.Mutate();
                switch (SaveFormat)
                {
                    case ImageFormat.Jpeg:
                        await image.SaveAsJpegAsync(path, new JpegEncoder { Quality = Quality });
                        break;
                    case ImageFormat.Bmp:
                        await image.SaveAsBmpAsync(path);
                        break;
                    case ImageFormat.Png:
                        await image.SaveAsPngAsync(path);
                        break;
                    case ImageFormat.Gif:
                        await image.SaveAsGifAsync(path);
                        break;
                    case ImageFormat.Tga:
                        await image.SaveAsTgaAsync(path);
                        break;
                    default:
                        await image.SaveAsync(path);
                        break;
                }

                var result = new DocumentResult
                {
                    Guid = guid,
                    Name = name,
                    FileName = fileName ?? name,
                    Extension = extension,
                    ContentType = format?.DefaultMimeType,
                    Type = DocumentType.Image,
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
        }

        private string GetFilePath(string fileName)
        {
            return Path.Join(FolderPath, fileName);
        }
    }
}
