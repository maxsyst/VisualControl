using System;

namespace VueExample.ViewModels
{
    public class StandartWaferViewModel
    {
        public int Id { get; set; }
        public Nullable<Int16> X { get; set; }
        public Nullable<Int16> Y { get; set; }
        public Nullable<Int16> Flag { get; set; }
        public string Code { get; set; }
        public int? CodeProductId { get; set; }
    }
}