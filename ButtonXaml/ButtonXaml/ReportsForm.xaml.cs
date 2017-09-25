using ButtonXaml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MotleyRunner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsForm : ContentPage
    {
        public ReportsForm(IntervalSettings intervals)
        {
            InitializeComponent();
            this.Intervals = intervals;
            BindingContext = this.Intervals;
        }

        public IntervalSettings Intervals { get; set; }

    }
}