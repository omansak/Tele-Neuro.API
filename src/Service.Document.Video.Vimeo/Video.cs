using System;

namespace Service.Document.Video.Vimeo
{
    public class Video
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public Uri Uri => new Uri(this.Url);
    }
}
