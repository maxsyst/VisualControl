
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace VueExample.Services.Abstract
{
    public interface IDieWithCodeService
    {
        Task<DieWithCode> GetById(ObjectId objectId);
        Task<List<ObjectId>> CreateDieWithCodes(List<DieWithCode> dieWithCodes);
        Task<bool> DeleteDieWithCodes(List<ObjectId> dieWithCodesIds);
    }
}