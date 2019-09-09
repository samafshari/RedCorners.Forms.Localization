using RedCorners.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using RedCorners;
using RedCorners.Forms.Localization;
using System.Linq;

namespace LocalizationDemo.ViewModels
{
    public class MainViewModel : BindableModel
    {
        public List<LanguageViewModel> Languages { get; set; } = new List<LanguageViewModel>();
        public List<TranslationViewModel> Translations { get; set; } = new List<TranslationViewModel>();

        public MainViewModel()
        {
            Languages = RL.GetLanguageKeys().Select(x => new LanguageViewModel(x)).ToList();
            Translations = RL.GetEffectiveKeys()[RL.CurrentLanguage].Select(x => new TranslationViewModel
            {
                Key = x.Key,
                Value = x.Value
            }).ToList();
        }
    }
}
