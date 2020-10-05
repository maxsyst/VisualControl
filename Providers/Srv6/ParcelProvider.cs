using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models.SRV6;
using VueExample.Models.SRV6.NullObjects;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class ParcelProvider : IParcelProvider
    {
        private readonly Srv6Context _srv6Context;
        private readonly IMapper _mapper;
        public ParcelProvider(Srv6Context srv6Context, IMapper mapper) 
        {
            _srv6Context = srv6Context;
            _mapper = mapper;
        }
        public async Task<ParcelViewModel> GetById(int id)
        {
            return _mapper.Map<Parcel, ParcelViewModel>(await _srv6Context.Parcels.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<ParcelViewModel> GetByWaferId(string waferId)
        {
            var parcel = await _srv6Context.Parcels.Include(x => x.Wafers).FirstOrDefaultAsync(x => x.Id == x.Wafers.FirstOrDefault(w => w.WaferId == waferId).ParcelId);
            return parcel == null ?  _mapper.Map<Parcel, ParcelViewModel>(new NullParcelObject()) : _mapper.Map<Parcel, ParcelViewModel>(parcel);
        }
    }
}