using System.Threading.Tasks;
using VueExample.Contexts;
using VueExample.Entities;
using VueExample.Exceptions;
using VueExample.Models.SRV6;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class KurbatovParameterBordersProvider : IKurbatovParameterBordersProvider
    {
        private readonly Srv6Context _srv6Context;
        public KurbatovParameterBordersProvider(Srv6Context srv6Context)
        {
            _srv6Context = srv6Context;
        }
        public async Task<KurbatovParameterBordersEntity> Create(KurbatovParameterBordersModel kurbatovBordersModel)
        {
            var kurbatovBorders = new KurbatovParameterBordersEntity{Upper = kurbatovBordersModel.Upper, Lower = kurbatovBordersModel.Upper};
            _srv6Context.KurbatovBorders.Add(kurbatovBorders);
            await _srv6Context.SaveChangesAsync();
            return kurbatovBorders;
        }

        public async Task<KurbatovParameterBordersEntity> Update(KurbatovParameterBordersModel kurbatovBordersModel)
        {
            var kurbatovBorders = await _srv6Context.KurbatovBorders.FindAsync(kurbatovBordersModel.Id) ?? throw new RecordNotFoundException();
            kurbatovBorders.Lower = kurbatovBordersModel.Lower;
            kurbatovBorders.Upper = kurbatovBordersModel.Upper;
            await _srv6Context.SaveChangesAsync();
            return kurbatovBorders;
        }
    }
}