using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome
{
    public partial class MainPage : TabbedPage
    {
        String halypaalle = "Hälytys on kytketty päälle!";
        String halypois = "Hälytys on pois päältä.";

        string kiuasPaalle = "Kiuas on päällä.";
        string kiuasPois = "Kiuas on sammutettu.";

        public MainPage()
        {
            InitializeComponent();
            Slider1.Minimum = 0;
            Slider2.Minimum = 0;
            Slider3.Minimum = 0;
            Slider4.Minimum = 0;
            
            Slider1.Maximum = 10;
            Slider2.Maximum = 10;
            Slider3.Maximum = 10;
            Slider4.Maximum = 10;

            GetHalytyksenTila();


            
        }

        private async void GetHalytyksenTila()
        {
            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync("http://kotiapi.azurewebsites.net/api/halytys/1");

            Halytys halytysItem = JsonConvert.DeserializeObject<Halytys>(response);

            int h = halytysItem.HalytinStatus;
        
            if (h == 0)
            {
                HalytysSwitch.IsToggled = false;
            }
            else
            {
                HalytysSwitch.IsToggled = true;
            }
        }




        private void Sauna_Switch_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (Sauna_Switch.IsToggled == false)

            {
                Sauna_Label.Text = kiuasPaalle;
            }
            else
            {
                Sauna_Label.Text = kiuasPois;
            }
        }

        private void HalytysSwitch_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (HalytysSwitch.IsToggled == false)
            //Jännä että se toimii näin päin, mutta näin se toimii.
            //Eli kun togggled on false sanotaan hälytys on kytketty päälle.
            {
                Halytys_Label.Text = halypaalle;
            }
            else
            {
                Halytys_Label.Text = halypois;
            }
        }
    }
}

