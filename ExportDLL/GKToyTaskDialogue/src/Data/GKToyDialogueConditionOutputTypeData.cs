using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

namespace GKToyTaskDialogue
{
    public class GKToyDialogueConditionOutputTypeData : GKGameData
    {
        #region ConditionOutputType
        [System.Serializable]
        public class ConditionOutputTypeData
        {
            public int id;
            public string type;
        }

        Dictionary<string, ConditionOutputTypeData> _dict = new Dictionary<string, ConditionOutputTypeData>();

        [SerializeField]
        public ConditionOutputTypeData[] _typeData;
        public ConditionOutputTypeData GetData(int id)
        {
            if (id < 0 || id >= _typeData.Length)
            {
                Debug.LogError(string.Format("Get condition output data faile. id: {0}", id));
                return null;
            }
            return _typeData[id];
        }

        List<string> _strTypeLst = new List<string>();

        public string [] GetArray()
        {
            if (0 < _strTypeLst.Count)
                return _strTypeLst.ToArray();

            foreach (var ct in _typeData)
                _strTypeLst.Add(GKToyDialogueMaker._GetDialogueLocalization(ct.type));

            return _strTypeLst.ToArray();
        }

        public ConditionOutputTypeData GetData(string key)
        {
            if (_dict.ContainsKey(key))
                return _dict[key];

            foreach (var d in _typeData)
            {
                if (d.type.Equals(key))
                {
                    _dict.Add(key, d);
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
        public void ReseDataTypeArray(int length)
        {
            _dict.Clear();
            ResetDataArray<ConditionOutputTypeData>(length, ref _typeData);
        }

        #endregion
    }
}
