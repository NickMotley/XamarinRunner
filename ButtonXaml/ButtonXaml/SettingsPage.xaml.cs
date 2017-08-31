using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ButtonXaml
{
    public partial class SettingsPage : ContentPage
    {
        private IntervalSettings interval;

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = this.Intervals;
            //this.GoToCommand = new Command<Type>(NavigateTo);

        }

        public IntervalSettings Intervals
        {
            get
            {
                if (this.interval == null)
                {
                    this.interval = new IntervalSettings();
                }
                return this.interval;
            }
            set
            {
                this.interval = value;
            }
        }

        //public ICommand GoToCommand { private set; get; }

        //async void NavigateTo(Type obj)
        //{
        //            await this.Navigation.PushAsync(new Session());
        //}

        //private void Button_Clicked(object sender, EventArgs e)
        //{
        //    this.NavigateTo(null);
        //}
    }
}
