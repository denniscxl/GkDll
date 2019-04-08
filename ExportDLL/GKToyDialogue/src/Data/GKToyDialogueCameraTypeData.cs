using UnityEngine;
using GKData;
using UnityEditor;
using System.Collections.Generic;

namespace GKToyDialogue
{
    public class GKToyDialogueCameraTypeData : GKGameData
    {
        #region Condition
        [System.Serializable]
        public class CameraTypeData
        {
            public int id;
            public string type;
        }

        Dictionary<string, CameraTypeData> _cameraDict = new Dictionary<string, CameraTypeData>();

        [SerializeField]
        public CameraTypeData[] _typeData;
        public CameraTypeData GetTypeData(int id)
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

        public CameraTypeData GetTypeData(string key)
        {
            if (_cameraDict.ContainsKey(key))
                return _cameraDict[key];

            foreach (var d in _typeData)
            {
                if (d.type.Equals(key))
                {
                    _cameraDict.Add(key, d);
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
            _cameraDict.Clear();
            ResetDataArray<CameraTypeData>(length, ref _typeData);
        }

        #endregion
    }
}
