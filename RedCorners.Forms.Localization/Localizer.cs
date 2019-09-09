using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace RedCorners.Forms.Localization
{
    public static class RL
    {
        public static string LDayOfWeek(this DateTime dt)
        {
            return L(dt.DayOfWeek.ToString());
        }

        static readonly Dictionary<string, Dictionary<string, string>> keys
            = new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, Dictionary<string, string>> DynamicKeys =
            new Dictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, Dictionary<string, string>> GetEmbeddedKeys() => keys;

        public static Dictionary<string, Dictionary<string, string>> GetEffectiveKeys()
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            if (DynamicKeys != null)
            {
                foreach (var item in DynamicKeys)
                {
                    result[item.Key] = new Dictionary<string, string>();
                    foreach (var pair in item.Value)
                        result[item.Key][pair.Key] = pair.Value;
                }
            }

            foreach (var item in keys)
            {
                if (!result.ContainsKey(item.Key))
                    result[item.Key] = new Dictionary<string, string>();
                foreach (var pair in item.Value)
                {
                    if (!result[item.Key].ContainsKey(item.Key))
                        result[item.Key][pair.Key] = pair.Value;
                }
            }

            return result;
        }

        public static event EventHandler<string> OnLanguageChange;

        static IEnumerable<string> overrideKeys = null;
        public static void SetLanguageKeys(IEnumerable<string> keys)
        {
            overrideKeys = keys;
        }

        public static IEnumerable<string> GetLanguageKeys()
        {
            if (overrideKeys != null) return overrideKeys;

            var langs = keys.Keys;
            if (DynamicKeys == null) return langs;
            return new HashSet<string>(langs.Union(DynamicKeys.Keys));
        }

        static bool _isLoaded = false;

        public static string CurrentLanguage { get; private set; }

        public static void SetLanguage(string language)
        {
            CurrentLanguage = language;
            OnLanguageChange?.Invoke(language, language);
        }

        public static void SetLanguage(Enum language) => SetLanguage(language.ToString());

        public static void Load(Type rootType, string prefix, string extension = ".l.json")
        {
            if (_isLoaded) return;
            _isLoaded = true;
            var assembly = rootType.GetTypeInfo().Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resource in resourceNames.Where(x => x.ToLowerInvariant().EndsWith(extension)))
            {
                var lang = resource.Substring(0, resource.Length - extension.Length);
                lang = lang.Substring(prefix.Length, lang.Length - prefix.Length);
                keys[lang] = new Dictionary<string, string>();

                var suffix = $"{lang}{extension}";
                var r = resourceNames.FirstOrDefault(x => x.EndsWith(suffix));
                if (r == null) continue;

                Stream stream = assembly.GetManifestResourceStream(r);
                string text = "{}";
                using (var reader = new StreamReader(stream))
                    text = reader.ReadToEnd();

                keys[lang] = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            }
        }

        /// <summary>
        /// Returns Translation for the current language, or the fallback language, or the key itself.
        /// </summary>
        /// <param name="key">The key to return translation for</param>
        /// <returns></returns>
        public static string L(string key, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(key)) return key;
            var value = GetValue(CurrentLanguage, key, args);
            if (value != null) return value;
            foreach (string lang in GetLanguageKeys())
            {
                value = GetValue(lang, key, args);
                if (!string.IsNullOrWhiteSpace(value)) return value;
                //if (value != null) return string.Format(value, args);
            }
            return key;
        }

        public static string Ln(string key0, string key1, string keyx, int count)
        {
            if (count == 0) return L(key0, count.ToString());
            if (count == 1) return L(key1, count.ToString());
            return L(keyx, count.ToString());
        }

        static string GetValue(string lang, string key, params string[] args)
        {
            try
            {
                if (DynamicKeys != null)
                {
                    //Try get remote key
                    if (DynamicKeys.ContainsKey(lang) && (DynamicKeys[lang]?.ContainsKey(key) ?? false))
                    {
                        var val = DynamicKeys[lang][key];
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            if (args == null || args.Length == 0) return val;
                            return string.Format(val, args);
                        }
                    }
                }

                if (keys.ContainsKey(lang) && keys[lang].TryGetValue(key, out var value))
                {
                    if (args == null || args.Length == 0) return value;
                    return string.Format(value, args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetValue: {lang}, {key}, {ex}");
            }
            return null;
        }
    }

    [ContentProperty("Key")]
    public class RLExtension : IMarkupExtension
    {
        public string Key { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return RL.L(Key);
        }
    }

    [ContentProperty("Key")]
    public class RLuExtension : IMarkupExtension
    {
        public string Key { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return RL.L(Key)?.ToUpper();
        }
    }
}