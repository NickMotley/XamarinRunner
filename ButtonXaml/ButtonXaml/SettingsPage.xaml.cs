using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public partial class SettingsPage : TabbedPage
    {

        public SettingsPage(IntervalSettings intervals)
        {
            InitializeComponent();
            this.Intervals = intervals;
            BindingContext = this.Intervals;
        }

        public IntervalSettings Intervals { get; set; }

        //protected override bool OnBackButtonPressed()
        //{
        //    return base.OnBackButtonPressed();
        //}
    }
}
