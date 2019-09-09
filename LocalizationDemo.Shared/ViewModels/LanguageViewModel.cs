using System;
using System.Text;
using System.Linq;
using RedCorners.Forms;
using RedCorners.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using RedCorners.Forms.Localization;

namespace LocalizationDemo.ViewModels
{
    public class LanguageViewModel : BindableModel
    {
        public string Language { get; private set; }
        public LanguageViewModel(string lang)
        {
            Status = TaskStatuses.Success;
            this.Language = lang;
        }

        public Command SwitchCommand => new Command(() =>
        {
            RL.SetLanguage(Language);
        });
    }
}
