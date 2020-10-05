namespace VueExample.Models.SRV6.NullObjects
{
    public class NullParcelObject : Parcel
    {
        public NullParcelObject() : base()
        {
            this.Id = 0;
            this.Name = "Неизвестно";
        }
    }
}