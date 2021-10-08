using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VueExample.Models.SRV6.Export
{
    public class KurbatovXLS
    {
        [JsonProperty(Order = -4)]
        public string OperationNumber { get; set; }
        [JsonProperty(Order = -4)]
        public string ElementName { get; set; }
        public string StageName { get; set; }
        [JsonProperty(Order = -1)]
        public HashSet<string> DirtyCodesList = new HashSet<string>();
        [JsonProperty(Order = -2)]
        public int DieQuantity { get; set; }        
        [JsonProperty(Order = -2)]
        public int DirtyPercentage { get; set; }
        public bool IsAddedToCommonWorksheet { get; set; }
        public bool IsNeedToCopyS2P { get; set; }
        [JsonProperty(Order = 0)]
        public List<KurbatovParameter> kpList = new List<KurbatovParameter>();     

        public void FindDirty()
        {
            foreach (var kurbatovParameter in kpList)
            {
                foreach (var dieValue in kurbatovParameter.advList)
                {
                    if(dieValue.Status != "Годен")
                    {
                        DirtyCodesList.Add(dieValue.DieCode);
                    }
                }
            }
            this.DieQuantity = kpList.FirstOrDefault().advList.Count();
            this.DirtyPercentage = (int)Math.Ceiling((DirtyCodesList.Count * 100.0 / DieQuantity));
            foreach (var kurbatovParameter in kpList)
            {
                kurbatovParameter.FindGoodAverage(this.DirtyCodesList);
            }
        }        
    }
}