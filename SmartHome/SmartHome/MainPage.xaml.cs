using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome
{
    public partial class MainPage : TabbedPage
    {
        String paalle = "Hälytys on kytketty päälle!";
        String pois = "Hälytys on pois päältä.";

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
        }


              private void HalytysSwitch_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
                if (HalytysSwitch.IsToggled == false)
                //En tajua miksi se toimii näin päin, mutta näin se toimii.
                //Eli kun togggled on false sanotaan hälytys on kytketty päälle.
                {
                Halytys_Label.Text = paalle;
                }
                else
                {
                Halytys_Label.Text = pois;
                }
        }
    }
}
