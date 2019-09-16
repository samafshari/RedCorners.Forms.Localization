using RedCorners.Forms;
using RedCorners.Forms.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace LocalizationDemo
{
    public class App : Application2
    {
        public enum Languages
        {
            En,
            De,
            Lu
        }

        public override void InitializeSystems()
        {
            base.InitializeSystems();
            RL.Load(typeof(App), "LocalizationDemo.", ".trans.json");

            RL.SetLanguageKeys(Enum.GetNames(typeof(Languages)));
            RL.SetLanguage(RL.GetLanguageKeys().FirstOrDefault());
            RL.OnLanguageChange += (o, e) => ShowFirstPage();
        }

        public override Page GetFirstPage() =>
            new Views.MainPage();
    }
}
