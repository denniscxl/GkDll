using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

public class ToyMakerData : GKGameData
{
    #region Localization
    [System.Serializable]
    public class LocalizationData
    {
        public int id;
        public string english;
        public string chinese;
    }

    Dictionary<string, LocalizationData> _localizationDict = new Dictionary<string, LocalizationData>();

    [SerializeField]
    public LocalizationData[] _localizationData;
    public LocalizationData GetLocalizationData(int id)
    {
        if (id < 0 || id >= _localizationData.Length)
        {
            Debug.LogError(string.Format("Get localization data faile. id: {0}", id));
            return null;
        }
        return _localizationData[id];
    }

    public LocalizationData GetLocalizationData(string key)
    {
        if (_localizationDict.ContainsKey(key))
            return _localizationDict[key];

        foreach(var d in _localizationData)
        {
            if(d.english.Equals(key))
            {
                _localizationDict.Add(key, d);
                return d;
            }
        }

        return null;
    }

    public void InitLocalizationProperty(ref SerializedProperty p, int idx)
    {
        p.FindPropertyRelative("id").intValue = _localizationData[idx].id;
        p.FindPropertyRelative("english").stringValue = _localizationData[idx].english;
        p.FindPropertyRelative("chinese").stringValue = _localizationData[idx].chinese;
    }
    public void ResetLocalizationDataTypeArray(int length) 
    {
        _localizationDict.Clear();
        ResetDataArray<LocalizationData>(length, ref _localizationData); 
    }

    #endregion
}
