using System.Collections.Generic;
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
        private readonly IProcessProvider _processProvider;
        public ParcelProvider(Srv6Context srv6Context, IProcessProvider processProvider, IMapper mapper) 
        {
            _srv6Context = srv6Context;
            _processProvider = processProvider;
            _mapper = mapper;
        }
        public async Task<ParcelViewModel> GetById(int id)
        {
            return _mapper.Map<Parcel, ParcelViewModel>(await _srv6Context.Parcels.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<List<ParcelWithWafersViewModel>> GetByProcessId(int processId)
        {
            var parcelWithWafersViewModelsList = await _srv6Context.Processes.Join(_srv6Context.CodeProducts, p => p.ProcessId, c => c.ProcessId, (p,c) => new {ProcessId = p.ProcessId, CodeProductId = c.IdCp})
                                                 .Join(_srv6Context.Wafers, p => p.CodeProductId, c => c.CodeProductId, (p,c) => new {WaferId = c.WaferId, Parcel = c.Parcel, ProcessId = p.ProcessId})
                                                 .Join(_srv6Context.Parcels, p => p.Parcel.Id, c => c.Id, (p,c) => new {Parcel = p.Parcel, ProcessId = p.ProcessId})
                                                 .Where(x => x.ProcessId == processId)
                                                 .Select(x => new ParcelWithWafersViewModel {
                                                                ParcelName = x.Parcel.Name, 
                                                                ParcelId = x.Parcel.Id, 
                                                                ChildrenWafers = 
                                                                        x.Parcel.Wafers.Select(w => new WaferViewModel {WaferId = w.WaferId, CodeProductId = w.CodeProductId})
                                                                                       .ToList()})
                                                 .AsNoTracking()
                                                 .ToListAsync();
                                               
            return parcelWithWafersViewModelsList.GroupBy(x => x.ParcelId).Select(g => g.FirstOrDefault()).ToList();;                                

        }

        public async Task<ParcelViewModel> GetByWaferId(string waferId)
        {
            var parcel = await _srv6Context.Parcels.Include(x => x.Wafers).FirstOrDefaultAsync(x => x.Id == x.Wafers.FirstOrDefault(w => w.WaferId == waferId).ParcelId);
            return parcel == null ?  _mapper.Map<Parcel, ParcelViewModel>(new NullParcelObject()) : _mapper.Map<Parcel, ParcelViewModel>(parcel);
        }
    }
}