using System.Collections.Generic;
using VueExample.Contexts;
using VueExample.Models;
using VueExample.Providers.Abstract;

namespace VueExample.Providers
{
    public class DangerLevelProvider : IDangerLevelProvider
    {
        private readonly VisualControlContext _visualControlContext;
        public DangerLevelProvider(VisualControlContext visualControlContext)
        {
            _visualControlContext = visualControlContext;    
        }
        public List<DangerLevel> GetDangerLevelFromDefectList(List<Defect> defectList)
        {
            var dangerLevelList = new List<DangerLevel>();
            foreach (var defect in defectList)
            {
                dangerLevelList.Add(defect.DangerLevel);
            }

            return dangerLevelList;
        }
    }
}
