namespace VueExample.Models.SRV6
{
    public class KurbatovParameterModel
    {
        public int Id { get; set; }
        public StandartParameterModel StandartParameter { get; set; }
        public KurbatovParameterBordersModel KurbatovParameterBorders { get; set; }
        public int SmpId { get; set; }
    }
}