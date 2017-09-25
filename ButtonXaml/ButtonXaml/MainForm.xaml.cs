using System;
using AudioManager;
using MotleyRunner;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ButtonXaml
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainForm : ContentPage
	{
        IntervalSettings intervals;

		public MainForm ()
		{
			InitializeComponent();

            this.Init();

            this.Intervals = new IntervalSettings();
            this.Intervals.StatusChanged += Intervals_StatusChanged;
            BindingContext = this.Intervals;

            //this.Content = new Xamarin.Forms.Maps.Map()
            //{
            //    IsShowingUser = true,
            //    HeightRequest = 100,
            //    WidthRequest = 960,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};

        }
        
        private void Intervals_StatusChanged(object sender, TimerStatusChangeEvent e)
        {
            string m = string.Empty;
            foreach (Rep r in this.Intervals.Program.Reps)
            {
                m += "Rep: " + r.Name + " " + r.TotalTime.TotalSeconds + "\r\n";
                foreach (UserActivity ua in r.UserActivities)
                { 
                    m += "Act: " + ua.Name + " " + ua.TotalTime.TotalSeconds + "\r\n";
                }
                m += "\r\n";

            }

            string msg = "Complete after " +  string.Format("{0:mm\\:ss}", this.Intervals.Program.TotalTime);       // + this.Intervals.Program.TotalTime.ToString();
            DisplayAlert("Timer", m, "Well Done!");

            NavigateToReports();

        }

        public IntervalSettings Intervals
        {
            get
            {
                if (this.intervals == null)
                {
                    this.intervals = new IntervalSettings();
                }
                return this.intervals;
            }
            set
            {
                this.intervals = value;
            }
        }

        async void NavigateToReports()
        {
            ReportsForm page = new ReportsForm(this.intervals);
            await this.Navigation.PushAsync(page);
        }

        async void NavigateToSettings()
        {
            SettingsPage page = new SettingsPage(this.intervals);
            await this.Navigation.PushAsync(page);
        }

        private void OnClick(object sender, EventArgs e)
        {
            this.NavigateToSettings();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void Init()
        {
            Audio.Manager.EffectsOn = true;
            float vol = Audio.Manager.EffectsVolume;
            Audio.Manager.EffectsVolume = 0;
            await Audio.Manager.PlaySound("single-beep.mp3");
            await Audio.Manager.PlaySound("double-beep.mp3");
            await Audio.Manager.PlaySound("harley-start.mp3");
            Audio.Manager.EffectsVolume = 1.0F;
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}