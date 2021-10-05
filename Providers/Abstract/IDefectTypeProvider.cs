using System.Collections.Generic;
using VueExample.Models;
using VueExample.ResponseObjects;

namespace VueExample.Providers.Abstract
{
    public interface IDefectTypeProvider
    {
        List<DefectType> GetDefectTypes();
        List<DefectType> GetDefectTypesFromDefectList(List<Defect> defectList);
        AfterDbManipulationObject<DefectType> AddDefectType(string description, string color);
        AfterDbManipulationObject<DefectType> DeleteDefectType(string description);
    }
}