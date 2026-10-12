using System;
using System.Collections.Generic;
using UnityEngine;
namespace PatiAdventure
{
    [Serializable] public sealed class ExplorationProgress
    {
        public const string Key="Pati.Exploration.v1";
        public List<string> stars=new List<string>();
        public List<string> enemies=new List<string>();
        public static ExplorationProgress Load()
        {
            try { var data=JsonUtility.FromJson<ExplorationProgress>(PlayerPrefs.GetString(Key,""));
                if(data!=null&&data.stars!=null&&data.enemies!=null)return data; }
            catch(ArgumentException) { }
            return new ExplorationProgress();
        }
        public void Write(){PlayerPrefs.SetString(Key,JsonUtility.ToJson(this));PlayerPrefs.Save();}
    }
}
