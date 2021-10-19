using System.Collections.Generic;

namespace VueExample.ViewModels.FileNameUploader
{
    public class FileNameUploaderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int ProcessId { get; set; }
        public List<GraphicNameUploaderViewModel> GraphicNames { get; set; }
    }
}