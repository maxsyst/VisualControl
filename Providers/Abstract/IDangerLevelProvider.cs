using System.Collections.Generic;
using VueExample.Models;

namespace VueExample.Providers.Abstract
{
    public interface IDangerLevelProvider
    {
        List<DangerLevel> GetDangerLevelFromDefectList(List<Defect> defectList);
    }
}