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

        string slup = "Saunan lämpötila: 37 astetta";
        string sldn = "Saunan lämpötila: 23 astetta";

        static int arvottu()
        {
            Random rnd = new Random();
            return rnd.Next(20, 23);
        }


        public MainPage()
        {
            InitializeComponent();

            // Valaisin slidereiden minimi ja maksimi arvojen asettaminen ohjelman alussa

            Keittio_slider.Minimum = 0;
            Olohuone_slider.Minimum = 0;
            Makuuhuone_slider.Minimum = 0;
            Tyohuone_slider.Minimum = 0;
            
            Keittio_slider.Maximum = 100;
            Olohuone_slider.Maximum = 100;
            Makuuhuone_slider.Maximum = 100;
            Tyohuone_slider.Maximum = 100;
            

            // Tässä kutsutaan funktiot joka hakevat valojen yms laitteiden tilan kannasta kun ohjelma aukeaa
            GetValojenTilat();
            GetHalytyksenTila();
            GetSaunanTila();
            GetLammitysLukemat();
                                                                      
        }

        
            
        
        private async void GetLammitysLukemat()
        {
            //Haetaan lämpötilanasetus stepperin talletettu arvo tietokannasta
            HttpClient client = new HttpClient();        
            string response = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/lammitys/1");
            Lammitys Lam = JsonConvert.DeserializeObject<Lammitys>(response);
            lampoStepper.Value = Lam.LampotilaAsetus;
            
            //Käytetään aiemmin arvottua lukemaa vallitsevaan huoneistonlämpötilaan
            huoneistonLampotila.Text = arvottu().ToString();

        }
        


        private async void GetValojenTilat()
        {
            HttpClient client = new HttpClient();

            string response1 = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/huone/2");
            Huone k = JsonConvert.DeserializeObject<Huone>(response1);
            Keittio_slider.Value = k.Valostatus;

            string response2 = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/huone/3");
            Huone o = JsonConvert.DeserializeObject<Huone>(response2);
            Olohuone_slider.Value = o.Valostatus;

            string response3 = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/huone/4");
            Huone m = JsonConvert.DeserializeObject<Huone>(response3);
            Makuuhuone_slider.Value = m.Valostatus;

            string response4 = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/huone/5");
            Huone t = JsonConvert.DeserializeObject<Huone>(response4);
            Tyohuone_slider.Value = t.Valostatus;

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

        private async void GetSaunanTila()
        {
            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync("https://kotiapi.azurewebsites.net/api/sauna/1");

            Sauna saunaItem = JsonConvert.DeserializeObject<Sauna>(response);

            int s = saunaItem.VirtaStatus;

            if (s == 0)
            {
                Sauna_Switch.IsToggled = false;
                Sauna_Label.Text = kiuasPois;
                Saunanlampo_Label.Text = sldn;
            }
            else
            {
                Sauna_Switch.IsToggled = true;
                Sauna_Label.Text = kiuasPaalle;
                Saunanlampo_Label.Text = slup;
            };
        }


        // Hälytyksen tilamuutos PUT

        public async void HalytysSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

        // ------------ Saunan tilamuutos PUT -------------------

        private async void Sauna_Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Sauna_Switch.IsToggled == false)

            {

                Sauna_Label.Text = kiuasPois;
                Saunanlampo_Label.Text = sldn;

                //päivitystoiminto API:n kautta tietokantaan

                int id = 1;
                int stat = 0;
                Sauna sauna = new Sauna()
                {
                    SaunaId = id,
                    VirtaStatus = stat
                };

                var json = JsonConvert.SerializeObject(sauna);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var result = await client.PutAsync
                (string.Concat("https://kotiapi.azurewebsites.net/api/sauna/", sauna.SaunaId), content);

            }
            else
            {
                Sauna_Label.Text = kiuasPaalle;
                Saunanlampo_Label.Text = slup;

                //päivitystoiminto API:n kautta tietokantaan

                int id = 1;
                int stat = 1;
                Sauna sauna = new Sauna()
                {
                    SaunaId = id,
                    VirtaStatus = stat
                };

                var json = JsonConvert.SerializeObject(sauna);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();
                var result = await client.PutAsync
                (string.Concat("https://kotiapi.azurewebsites.net/api/sauna/", sauna.SaunaId), content);

            }

        }


        // Keittiön valo PUT
                    
        private async void Keittio_slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            
            int id = 2;
            string huonenimi = "Keittiö";
            int valostatus = (int)Keittio_slider.Value;
            Huone keit = new Huone()
            {
                HuoneId = id,
                Huonenimi = huonenimi,
                Valostatus = valostatus
            };
            
            var json = JsonConvert.SerializeObject(keit);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var result = await client.PutAsync
            (string.Concat("https://kotiapi.azurewebsites.net/api/huone/", keit.HuoneId), content);
        }

        // Olohuoneen valo PUT

        private async void Olohuone_slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int id = 3;
            string huonenimi = "Olohuone";
            int valostatus = (int)Olohuone_slider.Value;
            Huone oloh = new Huone()
            {
                HuoneId = id,
                Huonenimi = huonenimi,
                Valostatus = valostatus
            };
            
            var json = JsonConvert.SerializeObject(oloh);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var result = await client.PutAsync
            (string.Concat("https://kotiapi.azurewebsites.net/api/huone/", oloh.HuoneId), content);
        }

        // Makuuhuoneen valo PUT

        private async void Makuuhuone_slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int id = 4;
            string huonenimi = "Makuuhuone";
            int valostatus = (int)Makuuhuone_slider.Value;
            Huone mak = new Huone()
            {
                HuoneId = id,
                Huonenimi = huonenimi,
                Valostatus = valostatus
            };

            var json = JsonConvert.SerializeObject(mak);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var result = await client.PutAsync
            (string.Concat("https://kotiapi.azurewebsites.net/api/huone/", mak.HuoneId), content);

        }

        // Työhuoneen valo PUT

        private async void Tyohuone_slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int id = 5;
            string huonenimi = "Työhuone";
            int valostatus = (int)Tyohuone_slider.Value;
            Huone tyoh = new Huone()
            {
                HuoneId = id,
                Huonenimi = huonenimi,
                Valostatus = valostatus
            };

            var json = JsonConvert.SerializeObject(tyoh);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var result = await client.PutAsync
            (string.Concat("https://kotiapi.azurewebsites.net/api/huone/", tyoh.HuoneId), content);

        }

        private async void LampoStepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int id = 1;
            int nykyinen = int.Parse(huoneistonLampotila.Text);
            int lampAsetus = (int)lampoStepper.Value;
            Lammitys asetettu = new Lammitys()
            {
                LammitinId = id,
                NykyLampotila = nykyinen,
                LampotilaAsetus = lampAsetus
            };

            var json = JsonConvert.SerializeObject(asetettu);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var result = await client.PutAsync
            (string.Concat("https://kotiapi.azurewebsites.net/api/Lammitys/", asetettu.LammitinId), content);
        }
    }
}


