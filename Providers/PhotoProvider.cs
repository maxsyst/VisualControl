using System.Collections.Generic;
using System.Linq;
using VueExample.Contexts;
using VueExample.Models;

namespace VueExample.Providers
{
    public class PhotoProvider : IPhotoProvider
    {   
        private readonly VisualControlContext _visualControlContext;
        public PhotoProvider(VisualControlContext visualControlContext)
        {
            _visualControlContext = visualControlContext;
        }
        public string InsertPhoto(Photo photo)
        {
            _visualControlContext.Add(photo);
            _visualControlContext.SaveChanges();
            return photo.Guid;
        }

        public List<Photo> GetPhotosByDefectId(int defectId)
        {
            List<Photo> photosList;
            photosList = _visualControlContext.Photos.Where(x => x.DefectId == defectId).ToList();
            return photosList;
        }
    }
}
