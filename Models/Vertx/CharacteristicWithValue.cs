namespace VueExample.Models.Vertx
{
    public class CharacteristicWithValue
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        public CharacteristicWithValue()
        {

        }
        public CharacteristicWithValue(string name, string unit, double value)
        {
            Unit = unit;
            Name = name;
            Value = value;
        }
    }
}