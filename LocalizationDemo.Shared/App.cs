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
#if __ANDROID__
            RL.Load(typeof(App), "LocalizationDemo.Droid.", ".trans.json");
#elif __IOS__
            RL.Load(typeof(App), "LocalizationDemo.iOS.", ".trans.json");
#endif

            RL.SetLanguageKeys(Enum.GetNames(typeof(Languages)));
            RL.SetLanguage(RL.GetLanguageKeys().FirstOrDefault());
            RL.OnLanguageChange += (o, e) => ShowFirstPage();
        }

        public override Page GetFirstPage() =>
            new Views.MainPage();
    }
}
