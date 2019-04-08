using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

namespace GKToyTaskDialogue
{
    public class GKToyDialogueSoundTypeData : GKGameData
    {
        #region Sound
        [System.Serializable]
        public class SoundTypeData
        {
            public int id;
            public string type;
        }

        Dictionary<string, SoundTypeData> _soundDict = new Dictionary<string, SoundTypeData>();

        [SerializeField]
        public SoundTypeData[] _typeData;
        public SoundTypeData GetTypeData(int id)
        {
            if (id < 0 || id >= _typeData.Length)
            {
                Debug.LogError(string.Format("Get camera data faile. id: {0}", id));
                return null;
            }
            return _typeData[id];
        }

        List<string> _strTypeLst = new List<string>();

        public string [] GeTypeArray()
        {
            if (0 < _strTypeLst.Count)
                return _strTypeLst.ToArray();

            foreach (var ct in _typeData)
                _strTypeLst.Add(GKToyDialogueMaker._GetDialogueLocalization(ct.type));

            return _strTypeLst.ToArray();
        }

        public SoundTypeData GetTypeData(string key)
        {
            if (_soundDict.ContainsKey(key))
                return _soundDict[key];

            foreach (var d in _typeData)
            {
                if (d.type.Equals(key))
                {
                    _soundDict.Add(key, d);
                    return d;
                }
            }

            return null;
        }

        public void IniConditionTypeProperty(ref SerializedProperty p, int idx)
        {
            p.FindPropertyRelative("id").intValue = _typeData[idx].id;
            p.FindPropertyRelative("type").stringValue = _typeData[idx].type;
        }
        public void ResetTypeDataTypeArray(int length)
        {
            _soundDict.Clear();
            ResetDataArray<SoundTypeData>(length, ref _typeData);
        }

        #endregion
    }
}
