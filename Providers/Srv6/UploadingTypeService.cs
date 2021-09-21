using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VueExample.Models.SRV6.Uploader;
using VueExample.Providers.Srv6.Interfaces;

namespace VueExample.Providers.Srv6
{
    public class UploadingTypeService : IUploadingTypeService
    {
        private readonly IMongoCollection<UploadingType> _uploadingTypeCollection;
        public UploadingTypeService(IMongoClient mongoClient)
        {
            _uploadingTypeCollection = mongoClient.GetDatabase("srv6_graphic4").GetCollection<UploadingType>("GraphicsByElement");
        }

        public async Task<List<UploadingType>> GetAll()
        {
            return await  _uploadingTypeCollection.Find(_ => true).ToListAsync();
        }
    }
}