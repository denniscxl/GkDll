using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

namespace GKToyDialogue
{
    public class GKToyDialogueConditionTypeData : GKGameData
    {
        #region Condition
        [System.Serializable]
        public class ConditionTypeData
        {
            public int id;
            public string conditionType;
        }

        Dictionary<string, ConditionTypeData> _conditionDict = new Dictionary<string, ConditionTypeData>();

        [SerializeField]
        public ConditionTypeData[] _conditionTypeData;
        public ConditionTypeData GetConditionTypeData(int id)
        {
            if (id < 0 || id >= _conditionTypeData.Length)
            {
                Debug.LogError(string.Format("Get condition data faile. id: {0}", id));
                return null;
            }
            return _conditionTypeData[id];
        }

        List<string> _strConditionTypeLst = new List<string>();

        public string [] GetConditionTypeArray()
        {
            if (0 < _strConditionTypeLst.Count)
                return _strConditionTypeLst.ToArray();

            foreach (var ct in _conditionTypeData)
                _strConditionTypeLst.Add(GKToyDialogueMaker._GetDialogueLocalization(ct.conditionType));

            return _strConditionTypeLst.ToArray();
        }

        public ConditionTypeData GetConditionTypeData(string key)
        {
            if (_conditionDict.ContainsKey(key))
                return _conditionDict[key];

            foreach (var d in _conditionTypeData)
            {
                if (d.conditionType.Equals(key))
                {
                    _conditionDict.Add(key, d);
                    return d;
                }
            }

            return null;
        }

        public void IniConditionTypeProperty(ref SerializedProperty p, int idx)
        {
            p.FindPropertyRelative("id").intValue = _conditionTypeData[idx].id;
            p.FindPropertyRelative("conditionType").stringValue = _conditionTypeData[idx].conditionType;
        }
        public void ResetConditionTypeDataTypeArray(int length)
        {
            _conditionDict.Clear();
            ResetDataArray<ConditionTypeData>(length, ref _conditionTypeData);
        }

        #endregion
    }
}
