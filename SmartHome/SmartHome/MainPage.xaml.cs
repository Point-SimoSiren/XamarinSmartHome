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
        string halypaalle = "Hälytys on kytketty päälle!";
        string halypois = "Hälytys on pois päältä.";

        string kiuasPaalle = "Kiuas on päällä.";
        string kiuasPois = "Kiuas on sammutettu.";

        public MainPage()
        {
            InitializeComponent();

            Keittio_slider.Minimum = 0;
            Olohuone_slider.Minimum = 0;
            Makuuhuone_slider.Minimum = 0;
            Tyohuone_slider.Minimum = 0;
            
            Keittio_slider.Maximum = 10;
            Olohuone_slider.Maximum = 10;
            Makuuhuone_slider.Maximum = 10;
            Tyohuone_slider.Maximum = 10;

            GetHalytyksenTila();
                                   
        }

        private async void GetHalytyksenTila()
        {
            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/halytys/1");

            Halytys halytysItem = JsonConvert.DeserializeObject<Halytys>(response);

            int h = halytysItem.HalytinStatus;
        
            if (h == 0)
            {
                HalytysSwitch.IsToggled = false;
                Halytys_Label.Text = halypois;
            }
            else
            {
                HalytysSwitch.IsToggled = true;
                Halytys_Label.Text = halypaalle;
            };
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

        public async void HalytysSwitch_PropertyChanged_1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (HalytysSwitch.IsToggled == false)

            { 
                        
                Halytys_Label.Text = halypois;

                //päivitystoiminto API:n kautta tietokantaan

                int id = 1;
                int stat = 0;
                Halytys halytys = new Halytys()
                {
                    HalytinId = id,
                    HalytinStatus = stat
                };

                var json = JsonConvert.SerializeObject(halytys);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var result = await client.PutAsync
                (string.Concat("https://kotiapi.azurewebsites.net/api/halytys/", halytys.HalytinId), content);

            }
            else
            {
                Halytys_Label.Text = halypaalle;

                //päivitystoiminto API:n kautta tietokantaan

                int id = 1;
                int stat = 1;
                Halytys halytys = new Halytys()
                {
                    HalytinId = id,
                    HalytinStatus = stat
                };

                var json = JsonConvert.SerializeObject(halytys);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var result = await client.PutAsync
                (string.Concat("https://kotiapi.azurewebsites.net/api/halytys/", halytys.HalytinId), content);

            }
        }

        

    }
}


