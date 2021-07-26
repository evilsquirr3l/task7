using System;

namespace xCloud.Task7.Models
{
    public class ImageMetadataModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime UpdatedOn { get; set; }

        public long SizeInBytes { get; set; }

        public string FileExtension { get; set; }
    }
}