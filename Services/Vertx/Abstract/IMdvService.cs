using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using VueExample.Models.Vertx;

namespace VueExample.Services.Vertx.Abstract
{
    public interface IMdvService
    {
        Task<Mdv> CreateMdv(Mdv mdv);

        Task<bool> Delete(ObjectId id);

        Task<List<Mdv>> GetByWafer(string waferId);

        Task<Mdv> GetById(ObjectId id);

        Task<Mdv> GetByWaferAndCode(string waferId, string code);
    }
}