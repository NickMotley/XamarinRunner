using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ButtonXaml
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainForm : ContentPage
	{
		public MainForm ()
		{
			InitializeComponent ();
		}

        async void NavigateToSettings()
        {
            await this.Navigation.PushAsync(new SettingsPage());
        }
        private void OnClick(object sender, EventArgs e)
        {
            this.NavigateToSettings();
        }

        async void NavigateToSession()
        {
            await this.Navigation.PushAsync(new Session());
        }
        private void OnClick2(object sender, EventArgs e)
        {
            this.NavigateToSession();
        }
    }
}