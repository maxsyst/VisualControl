namespace VueExample.ChartModels.ChartJs.Abstract
{
    public abstract class Options
    {
        public bool Responsive { get; set; }
        public bool ShowLines { get; set; }
        public bool MaintainAspectRatio { get; set; }
        public int ResponsiveAnimationDuration { get; set; }
        public ChartJs.Options.Animation Animation { get; set; }
        public ChartJs.Options.Hover Hover { get; set; }

        public Options()
        {
            Responsive = false;
            ShowLines = false;
            ResponsiveAnimationDuration = 0;
            MaintainAspectRatio = false;
            Animation = new ChartJs.Options.Animation();
            Hover = new ChartJs.Options.Hover();
        }
    }
}