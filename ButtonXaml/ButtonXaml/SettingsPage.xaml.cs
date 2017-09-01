using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public partial class SettingsPage : ContentPage
    {
        private IntervalSettings intervals;

        public SettingsPage(IntervalSettings intervals)
        {
            InitializeComponent();
            this.Intervals = intervals;
            BindingContext = this.Intervals;
        }

        public IntervalSettings Intervals
        {
            get
            {
                //if (this.intervals == null)
                //{
                //    this.intervals = new IntervalSettings();
                //}
                return this.intervals;
            }
            set
            {
                this.intervals = value;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}
