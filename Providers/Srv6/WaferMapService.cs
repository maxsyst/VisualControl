using System.Linq;
using System.Threading.Tasks;
using VueExample.Exceptions;
using VueExample.Providers.Srv6.Interfaces;
using VueExample.Services;
using VueExample.ViewModels;

namespace VueExample.Providers.Srv6
{
    public class WaferMapService : IWaferMapService
    {
        private readonly IWaferMapProvider _waferMapProvider;
        private readonly IDieProvider _dieProvider;
        public WaferMapService(IWaferMapProvider waferMapProvider, IDieProvider dieProvider)
        {
            _dieProvider = dieProvider;
            _waferMapProvider = waferMapProvider;
        }
        public async Task<FormedMapViewModel> GetFormedMap(WaferMapFieldViewModel waferMapFieldViewModel)
        {
            var diesList = await _dieProvider.GetDiesByWaferId(waferMapFieldViewModel.WaferId);
            if (diesList.Count == 0)
            {
                throw new CollectionIsEmptyException();
            }
            var orientation = (await _waferMapProvider.GetByWaferId(waferMapFieldViewModel.WaferId)).Orientation;
            var waferMapFormed = new WaferMapFormationService(waferMapFieldViewModel.FieldHeight,
                waferMapFieldViewModel.FieldWidth, waferMapFieldViewModel.StreetSize, diesList).GetFormedWaferMap();
            return new FormedMapViewModel { WaferMapDies = waferMapFormed.ToList(), Orientation = orientation};
        }
    }
}