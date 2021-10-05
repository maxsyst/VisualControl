using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VueExample.Services.Vertx.Abstract;
using VueExample.ViewModels.Vertx.InputModels;

namespace VueExample.Controllers.Vertx
{
    [Route("api/vertx/[controller]")]
    public class MeasurementSetPlusUnitController : Controller
    {
        private readonly IMeasurementSetPlusUnitService _measurementSetPlusUnitService;
        public MeasurementSetPlusUnitController(IMeasurementSetPlusUnitService measurementPlusUnitService)
        {
            _measurementSetPlusUnitService = measurementPlusUnitService;
        }

        [HttpPost("updateCharacteristicUnit")]
        public async Task<IActionResult> UpdateCharacteristicUnit([FromBody] CharacteristicInputModel characteristicInputModel)
        {
           var result = 
            await _measurementSetPlusUnitService.ChangeCharacteristicUnit(
                characteristicInputModel.CharacteristicName, 
                characteristicInputModel.CharacteristicUnit, 
                new MongoDB.Bson.ObjectId(characteristicInputModel.MeasurementId));
           return result ? Ok() : (IActionResult)BadRequest();
        }
    }
}