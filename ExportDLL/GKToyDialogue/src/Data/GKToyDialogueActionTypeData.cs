using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

namespace GKToyDialogue
{
    public class GKToyDialogueActionTypeData : GKGameData
    {
        #region ActionType
        [System.Serializable]
        public class ActionTypeData
        {
            public int id;
            public string actionType;
        }

        Dictionary<string, ActionTypeData> _actionTypeDict = new Dictionary<string, ActionTypeData>();

        List<string> _strActionTypeLst = new List<string>();

        public string[] GetActionTypeArray()
        {
            if (0 < _strActionTypeLst.Count)
                return _strActionTypeLst.ToArray();

            foreach (var ct in _actionTypeData)
                _strActionTypeLst.Add(GKToyDialogueMaker._GetDialogueLocalization(ct.actionType));

            return _strActionTypeLst.ToArray();
        }

        [SerializeField]
        public ActionTypeData[] _actionTypeData;
        public ActionTypeData GetActionTypeData(int id)
        {
            if (id < 0 || id >= _actionTypeData.Length)
            {
                Debug.LogError(string.Format("Get actionType data faile. id: {0}", id));
                return null;
            }
            return _actionTypeData[id];
        }

        public ActionTypeData GetActionTypeData(string key)
        {
            if (_actionTypeDict.ContainsKey(key))
                return _actionTypeDict[key];

            foreach (var d in _actionTypeData)
            {
                if (d.actionType.Equals(key))
                {
                    _actionTypeDict.Add(key, d);
                    return d;
                }
            }

            return null;
        }

        public void InitActionTypeProperty(ref SerializedProperty p, int idx)
        {
            p.FindPropertyRelative("id").intValue = _actionTypeData[idx].id;
            p.FindPropertyRelative("actionType").stringValue = _actionTypeData[idx].actionType;
        }
        public void ResetActionTypeDataTypeArray(int length)
        {
            _actionTypeDict.Clear();
            ResetDataArray<ActionTypeData>(length, ref _actionTypeData);
        }

        #endregion
    }
}
