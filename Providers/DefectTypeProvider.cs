using System.Collections.Generic;
using System.Linq;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Providers.Abstract;
using VueExample.ResponseObjects;

namespace VueExample.Providers
{
    public class DefectTypeProvider : IDefectTypeProvider
    {
        private readonly VisualControlContext _visualControlContext;
        public DefectTypeProvider(VisualControlContext visualControlContext)
        {
            _visualControlContext = visualControlContext;
        }
        public List<DefectType> GetDefectTypes()
        {
            return _visualControlContext.DefectTypes.ToList();
        }

        public List<DefectType> GetDefectTypesFromDefectList(List<Defect> defectList)
        {
            var defectTypeList = new List<DefectType>();
            foreach (var defect in defectList)
            {
                defectTypeList.Add(defect.DefectType);
            }

            return defectTypeList;
        }

        public AfterDbManipulationObject<DefectType> AddDefectType(string description, string color)
        {
            var newDefectType = new DefectType { Description = description, Color = color };
            var obj = new AfterDbManipulationObject<DefectType>(newDefectType);
            if (_visualControlContext.DefectTypes.Any(x => x.Description == description))
            {
                obj.AddError(new Error(@"Существует тип дефекта с идентичным именем"));
            }

            if (_visualControlContext.DefectTypes.Any(x => x.Color == color))
            {
                obj.AddError(new Error(@"Существует тип дефекта с идентичным цветом"));
            }

            if (obj.HasErrors) return obj;
            _visualControlContext.DefectTypes.Add(newDefectType);
            _visualControlContext.SaveChanges();
            return obj;
        }

        public AfterDbManipulationObject<DefectType> DeleteDefectType(string description)
        {
            var obj = new AfterDbManipulationObject<DefectType>("DELETE");
            var defectType =  _visualControlContext.DefectTypes.FirstOrDefault(x => x.Description == description);
            if (defectType == null)
            {
                obj.AddError(new Error(@"Не найден тип с таким именем"));
                return obj;
            }

            if ( _visualControlContext.Defects.Any(x => x.DefectTypeId == defectType.DefectTypeId))
            {
                obj.AddError(new Error(@"Невозможно удалить тип, так как существуют дефекты с таким типом"));
                return obj;
            }

            obj.SetObject(defectType);
                _visualControlContext.DefectTypes.Remove(defectType);
                _visualControlContext.SaveChanges();
            return obj;
        }
    }
}
