using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VueExample.Services.Abstract;
using VueExample.ViewModels;

namespace VueExample.Services
{
    public class DieWithCodeService : IDieWithCodeService
    {
        private readonly IMongoCollection<DieWithCodeViewModel> _dieWithCodesCollection;
        public DieWithCodeService(IMongoClient mongoClient)
        {
            _dieWithCodesCollection = mongoClient.GetDatabase("srv6_graphic4").GetCollection<DieWithCodeViewModel>("DieWithCodes");
        }
        public async Task<List<ObjectId>> CreateDieWithCodes(List<DieWithCode> dieWithCodes)
        {
            var dieWithCodeViewModelList = dieWithCodes.Select(x => new DieWithCodeViewModel(x)).ToList(); 
            await _dieWithCodesCollection.InsertManyAsync(dieWithCodeViewModelList);
            return dieWithCodeViewModelList.Select(x => x.Id).ToList();
        }

        public async Task<bool> DeleteDieWithCodes(List<ObjectId> dieWithCodesIds)
        {
            var filter = Builders<DieWithCodeViewModel>.Filter.In("_id", dieWithCodesIds);
            var deleteResult = await _dieWithCodesCollection.DeleteManyAsync(filter);
            return deleteResult.DeletedCount == dieWithCodesIds.Count;
        }

        public async Task<DieWithCode> GetById(ObjectId id)
        {
            var dieWithCodeViewModel = await _dieWithCodesCollection.Find(Builders<DieWithCodeViewModel>.Filter.Eq(x => x.Id, id)).FirstOrDefaultAsync();
            return new DieWithCode {
                DieId = dieWithCodeViewModel.DieId,
                DieCode = dieWithCodeViewModel.DieCode,
                AbscissList = dieWithCodeViewModel.AbscissList.ToList(),
                ValueListWithState = dieWithCodeViewModel.ValueListWithState.ToList()
            };
        }
    }
}