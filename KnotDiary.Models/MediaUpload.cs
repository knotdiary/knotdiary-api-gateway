using System.Collections.Generic;
using System.IO;

namespace KnotDiary.Models
{
    public class BaseMediaUpload
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
    }

    public class MediaUpload : BaseMediaUpload
    {
        public Stream File { get; set; }
    }

    public class MultipleMediaUpload : BaseMediaUpload
    {
        public List<Stream> Files { get; set; }
    }
}
