using System.Collections.Generic;

namespace VueExample.ViewModels.FileNameUploader
{
    public class FileNameUploaderUViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProcessId { get; set; }
        public string SelectedGraphicNames { get; set; }
        public List<string> GraphicNames { get; set; }
    }
}