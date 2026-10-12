using System;
using UnityEngine;
namespace PatiAdventure
{
    public static class PatiLocalization
    {
        public static string Language { get; private set; } = "tr";
        public static event Action Changed;
        public static void SetLanguage(string value)
        { Language=value=="en"?"en":"tr"; PlayerPrefs.SetString("Pati.Language",Language); PlayerPrefs.Save(); Changed?.Invoke(); }
        public static string Pick(string turkish,string english) => Language=="en"?english:turkish;
    }
}
