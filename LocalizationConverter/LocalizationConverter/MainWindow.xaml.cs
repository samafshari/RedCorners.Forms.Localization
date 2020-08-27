using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RedCorners;
using RedCorners.Components;
using RedCorners.Models;

using System.IO;
using Path = System.IO.Path;
using Newtonsoft.Json;
using IronXL;

namespace LocalizationConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Settings
        {
            public string Folder { get; set; } = "";
            public string Extension { get; set; } = ".l.json";
            public string Excel { get; set; } = "l.xlsx";
        }

        readonly ObjectStorage<Settings> settingsStorage = new ObjectStorage<Settings>();

        public MainWindow()
        {
            InitializeComponent();
            txtFolder.Text = settingsStorage.Data.Folder;
            txtExtension.Text = settingsStorage.Data.Extension;
            txtExcel.Text = settingsStorage.Data.Excel;
        }

        void Save()
        {
            settingsStorage.Data.Folder = txtFolder.Text;
            settingsStorage.Data.Extension = txtExtension.Text;
            settingsStorage.Data.Excel = txtExcel.Text;
            settingsStorage.Save();
        }

        private void btnJsonToExcel_Click(object sender, RoutedEventArgs e)
        {
            Save();

            var folder = txtFolder.Text;
            var extension = txtExtension.Text;
            var excelPath = txtExcel.Text;
            if (Path.GetFullPath(excelPath) != excelPath)
                excelPath = Path.Combine(folder, excelPath);

            var jsonPaths = Directory.EnumerateFiles(folder, $"*{extension}");
            // languages[language[key]] = value
            var languages = new Dictionary<string, Dictionary<string, string>>();
            var keySet = new HashSet<string>();
            foreach (var jsonPath in jsonPaths)
            {
                var fileName = Path.GetFileName(jsonPath);
                var languageKey = fileName.Substring(0, fileName.Length - extension.Length);
                var json = File.ReadAllText(jsonPath);
                var language = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                languages[languageKey] = language;
                foreach (var item in language)
                    keySet.Add(item.Key);
            }

            var keys = keySet.ToArray();

            var workbook = WorkBook.Create();
            var workSheet = workbook.DefaultWorkSheet;
            if (workSheet == null) workSheet = workbook.CreateWorkSheet("Translations");

            { // Write Language Keys
                char col = 'B';
                foreach (var language in languages)
                { 
                    workSheet[$"{col}1"].StringValue = language.Key;
                    int row = 2;
                    foreach (var key in keys)
                    {
                        if (languages[language.Key].TryGetValue(key, out var val))
                            workSheet[$"{col}{row}"].StringValue = val;
                        else
                            workSheet[$"{col}{row}"].StringValue = null;

                        workSheet[$"A{row}"].StringValue = key;

                        row++;
                    }
                    col++;
                }
            }
            
            workbook.SaveAs(excelPath);
        }

        private void btnExcelToJson_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
    }
}
