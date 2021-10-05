namespace VueExample.Models.Vertx
{
    public class Characteristic
    {
        public string Name { get; set; }
        public string Unit { get; set; }

        public Characteristic()
        {

        }

        public Characteristic(string name, string unit)
        {
            Name = name;
            Unit = unit;
        }
    }
}