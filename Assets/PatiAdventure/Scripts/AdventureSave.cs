using System;
using System.Collections.Generic;
using UnityEngine;
namespace PatiAdventure
{
    [Serializable]
    public sealed class AdventureSaveData
    {
        public int version=2, level;
        public bool trainingComplete;
        public bool hasCheckpoint;
        public Vector3 checkpoint;
        public List<string> stars=new List<string>(), friends=new List<string>(), beacons=new List<string>(), chests=new List<string>(), guides=new List<string>();
        public void Normalize()
        {
            level=Mathf.Clamp(level,0,36);
            if(stars==null)stars=new List<string>(); if(friends==null)friends=new List<string>();
            if(beacons==null)beacons=new List<string>(); if(chests==null)chests=new List<string>(); if(guides==null)guides=new List<string>();
            if(!Finite(checkpoint.x)||!Finite(checkpoint.y)||!Finite(checkpoint.z)||checkpoint.sqrMagnitude>1000000)hasCheckpoint=false;
        }
        private static bool Finite(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value);
        public static bool AddOnce(List<string> list,string id)
        { if(string.IsNullOrEmpty(id)||list.Contains(id))return false; list.Add(id); return true; }
        public bool CanFinish(int chapter,int stage)
        {
            if(stars.Count<5)return false;
            if(chapter==0&&stage==0)return trainingComplete&&friends.Count>=1&&beacons.Count>=1&&guides.Count>=1;
            return stage==0 || (stage==1 ? friends.Count>=3 : beacons.Count>=3);
        }
    }
    public static class AdventureSave
    {
        public const string Key="Pati.Save.v2";
        public static AdventureSaveData Decode(string json)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(json))return new AdventureSaveData();
                var data=JsonUtility.FromJson<AdventureSaveData>(json);
                if(data==null||data.version!=2)return new AdventureSaveData();
                data.Normalize(); return data;
            }
            catch(ArgumentException) { return new AdventureSaveData(); }
        }
        public static AdventureSaveData Load()
        {
            if(PlayerPrefs.HasKey(Key))return Decode(PlayerPrefs.GetString(Key));
            return new AdventureSaveData { level=Mathf.Clamp(PlayerPrefs.GetInt("Pati.Progress",0),0,36) };
        }
        public static void Write(AdventureSaveData data)
        { data.Normalize(); PlayerPrefs.SetString(Key,JsonUtility.ToJson(data)); PlayerPrefs.Save(); }
    }
}
