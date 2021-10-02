using System;

namespace Service.Document.Model
{
    public class DocumentResult
    {
        /// <summary>
        /// GUID
        /// </summary>
        public string Guid { get; init; }
        /// <summary>
        /// File Physical Name
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// File Provider Name
        /// </summary>
        public string FileName { get; init; }
        /// <summary>
        /// File Extension Name
        /// </summary>
        public string Extension { get; init; }
        /// <summary>
        /// Content Type
        /// </summary>
        public string ContentType { get; init; }
        /// <summary>
        /// Paths
        /// </summary>
        public DocumentPath DocumentPath { get; init; }
        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; init; }
        /// <summary>
        /// Document Type
        /// </summary>
        public DocumentType Type { get; init; }
    }
    public class DocumentPath
    {
        /// <summary>
        /// Root Path
        /// --->  C://
        /// </summary>
        public string Base { get; init; }
        /// <summary>
        /// Directory
        /// ----> /Uploads
        /// </summary>
        public string Directory { get; init; }
        /// <summary>
        /// Directory/{FileName}.{Extension}
        /// ----> /Uploads/fileName.ext
        /// </summary>
        public string Path { get; init; }
        /// <summary>
        /// Base/Directory/{FileName}.{Extension}
        /// ----> C:/Uploads/fileName.ext
        /// </summary>
        public string FullPath { get; init; }
    }
}
