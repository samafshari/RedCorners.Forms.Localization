NuGet: [https://www.nuget.org/packages/RedCorners.Forms.Localization](https://www.nuget.org/packages/RedCorners.Forms.Localization)

`RedCorners.Forms.Localization` is a lightweight and easy to use library for facilitating translations and localizations in Xamarin.Forms. It is available as a .NET Standard library and works on all of the platforms you can install Xamarin.Forms on.

It stores translations as a `Dictionary` of `Dictionary`s, where the parent `Dictionary` holds the mappings between language keys and their translations, and the child `Dictionary`s hold the mappings between translation keys and their values in each language.

This concept allows the translation engine to work perfectly with JSON data structures:

```js
{
    "En": {
        "Hello": "Hello!",
        "HelloX": "Hello, {0}!",
        "Bye": "Bye!"
    },
    "Fr": {
        "Hello": "Salut!",
        "HelloX": "Salut, {0}!",
        "Bye": "Au revoir!"
    },
    "De": {
        "Hello": "Hallo!",
        "HelloX": "Hallo, {0}!",
        "Bye": "Tschüss!"
    }
}
```

## Getting Started

Everything is located under the `RedCorners.Forms.Localization` namespace inside the `RL` static class:

```cs
using RedCorners.Forms.Localization;
```

Full Documentation: [http://redcorners.com/localization/](http://redcorners.com/localization/)

After installing the `RedCorners.Forms.Localization` package and its dependencies, you have a few different ways to load the translations. The most straightforward way is to call the `Load` method with a `Dictionary<string, Dictionary<string, string>>`, containing a structure similar to the one above:

```cs
RL.Load(map);
```

If you have a dictionary for each language, you can load them individually like this:

```cs
Dictionary<string, string> english, french, german;

RL.Load("En", english);
RL.Load("Fr", french);
RL.Load("De", german);
```

The `RL` class provides an additional overload of the `Load` method for automatically loading translations from embedded resources. Assume the scenario where three JSON files are set as embedded resources inside a project with the default namespace `LocalizationDemo` and the `.trans.json` file extensions: 

<img src="https://redcorners.com/images/localization_000.PNG" class="free-width" />

You can automatically load all the files by calling this special overload of `RL`:

<img src="https://redcorners.com/images/localization_001.PNG" class="free-width" />

```cs
namespace LocalizationDemo
{
    public class App : Application2
    {
        public override void InitializeSystems()
        {
            base.InitializeSystems();
            RL.Load(typeof(App), "LocalizationDemo.", ".trans.json");
        }
    }
}
```

### Changing the Language

You can change the current language by calling the `SetLanguage` method. `RL` provides an `OnLanguageChange` event, which is triggered whenever the language is changed. You can use this event to restart or update your UI layer to reflect the new language:

```cs
RL.SetLanguage("En");
RL.OnLanguageChange += (o, e) => ShowFirstPage();
```

The `GetLanguageKeys()` method returns a list of all loaded language keys (e.g. `En`, `Fr` or `De`). This is derived from the file names, based on the parameters of the `Load` method.

### Performing Localizations

The `L` method provides the values of the specified key in the selected language:

```cs
Console.WriteLine(RL.L("Hello"));
```

In case the translation value requires a parameter, you can provide them to the `L` method:

```cs
Console.WriteLine(RL.L("HelloX", "Alice"));
//Prints "Hello, Alice!"
```

### XAML Syntax

You can use the `RL` tools in Xamarin.Forms XAML. To do this, first include the namespace:

```xml
xmlns:rl="clr-namespace:RedCorners.Forms.Localization;assembly=RedCorners.Forms.Localization"
```

Once you have this, you can use the `RL` XAML extension to quickly provide localized strings:

```xml
<Label Text="{rl:RL Hello}" />
```