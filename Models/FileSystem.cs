using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorage.Models
{
    public class FileSystem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public double Size { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Url { get; set; }

    }
}
