using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VueExample.Contexts;
using VueExample.Models;
namespace VueExample.Providers
{
    public class DefectProvider : IDefectProvider
    {
        private readonly VisualControlContext _visualControlContext;
        public DefectProvider(VisualControlContext visualControlContext)
        {
            _visualControlContext = visualControlContext;
        }
        public int InsertNewDefect(Defect defect)
        {
            _visualControlContext.Defects.Add(defect);
            _visualControlContext.SaveChanges();
            return defect.DefectId;
        }
        public void DeleteById(int defectId)
        {
            var deletedDefect = _visualControlContext.Defects.FirstOrDefault(x => x.DefectId == defectId);
            if (deletedDefect != null) _visualControlContext.Defects.Remove(deletedDefect);
            _visualControlContext.SaveChanges();
        }

        public int GetDuplicate(long dieId, int stageId, int defectTypeId)
        {
            Defect duplicate;
            duplicate = _visualControlContext.Defects.FirstOrDefault(x =>
                    x.DieId == dieId && x.DefectTypeId == defectTypeId && x.StageId == stageId);

            return duplicate?.DefectId ?? 0;
        }

        public List<Defect> GetByWaferId(string waferId)
        {
            return _visualControlContext.Defects.Where(x => x.WaferId == waferId).ToList();
        }

        public List<Defect> GetByDieId(long dieId)
        {
            return _visualControlContext.Defects.Where(x => x.DieId == dieId).ToList();
        }

        public List<Defect> GetByWaferIdWithIncludes(string waferId)
        {
            return _visualControlContext.Defects.Include(x=>x.DangerLevel).Include(x=>x.DefectType).Where(x => x.WaferId == waferId).ToList();
        }

        public Defect GetById(int defectId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Defect> GetByIdAsync(int defectId)
        {
            throw new System.NotImplementedException();
        }
    }
}
