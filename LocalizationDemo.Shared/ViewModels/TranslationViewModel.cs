using System;
using System.Text;
using System.Linq;
using RedCorners.Forms;
using RedCorners.Models;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocalizationDemo.ViewModels
{
    public class TranslationViewModel : BindableModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public TranslationViewModel()
        {
            Status = TaskStatuses.Success;
        }
    }
}
