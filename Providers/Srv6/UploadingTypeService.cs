using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VueExample.Models.SRV6;
using VueExample.Models.SRV6.Uploader;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class UploadingTypeService : IUploadingTypeService
    {
        private readonly IMongoCollection<UploadingType> _uploadingTypeCollection;
        private readonly ICodeProductProvider _codeProductProvider;
        private readonly ISRV6GraphicService _graphicService;
        public UploadingTypeService(IMongoClient mongoClient, ICodeProductProvider codeProductProvider, ISRV6GraphicService graphicService)
        {
            _codeProductProvider = codeProductProvider;
            _graphicService = graphicService;
            _uploadingTypeCollection = mongoClient.GetDatabase("srv6_graphic4").GetCollection<UploadingType>("GraphicsByElement");
        }

        public async Task<List<UploadingType>> GetAll()
        {
            return await _uploadingTypeCollection.Find(_ => true).ToListAsync();
        }

        public async Task<AvailableS2PGraphicsViewModel> GetAvailableS2PGraphics(string waferId)
        {
            var codeProduct = await _codeProductProvider.GetByWaferId(waferId);
            var graphicsNameList = new List<string>{"S21", "S22", "S11", "S12"};
            var graphicsList = await _graphicService.GetByCodeProduct(codeProduct.IdCp);
            var graphicsDictionary = new Dictionary<string, Graphic>();
            foreach (var graphicName in graphicsNameList)
            {
                var graphic = graphicsList.FirstOrDefault(x => x.Name == $"{graphicName}/Freq");
                graphicsDictionary.Add(graphicName, graphic);
            }
            return new AvailableS2PGraphicsViewModel{AvailableGraphics = graphicsList.Where(x => x.Name.Contains("Freq")).ToList(), CurrentGraphics = graphicsDictionary};
           
        }
    }
}