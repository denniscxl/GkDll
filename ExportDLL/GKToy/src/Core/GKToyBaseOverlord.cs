using System.Collections.Generic;
using UnityEngine;
using GKBase;
using System;
using System.Linq;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

namespace GKToy
{
    [System.Serializable]
    public class GKToyBaseOverlord : MonoBehaviour
    {
        #region PublicField
        static protected ToyMakerSettings _settings;
        public static ToyMakerSettings Settings
        {
            get
            {
                if (_settings == null) 
                {
                    _settings = GK.LoadResource<ToyMakerSettings>("Settings/ToyMakerSettings");
                }
                return _settings;
            }
        }
        static public ToyMakerSettings.ToyMakerBase toyMakerBase = null;
        public GKToyExternalData internalData = null;
        // 外部数据.
        public List<GKToyExternalData> externalDatas = new List<GKToyExternalData>();
        public List<GKNodeStateMachine> stateMachines = new List<GKNodeStateMachine>();
		public bool isPlaying = true;
        #endregion

        #region PrivateField
        protected Dictionary<int, Dictionary<string, object>>  _varDict = new Dictionary<int, Dictionary<string, object>>();
        protected List<GKToyData> _tmpNewData = new List<GKToyData>();
        #endregion

        #region PublicMethod
        // 序列化节点数据并存储外部数据.
        public void Save()
        {
            Debug.Log("Save Overlord.");
            internalData.data.SaveNodes();
            internalData.data.SaveVariable();
            EditorUtility.SetDirty(internalData);

            foreach (var ed in externalDatas)
            {
                if (null != ed && null != ed.data)
                {
                    ed.data.SaveNodes();
                    ed.data.SaveVariable();
                    EditorUtility.SetDirty(ed);
                }
            }
        }
        /// <summary>
        /// 备份数据到文件
        /// 备份目录：内部数据源所在目录/OverlordBackup/
        /// </summary>
        public void Backup()
        {
            string dataPath = string.Format("{0}/OverlordBackup/", Path.GetDirectoryName(AssetDatabase.GetAssetPath(internalData)));
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string overlordPath = string.Format("{0}overlord_{1}_back", dataPath, internalData.name);
            OverlordBackData backData = new OverlordBackData(dataPath, this);
            File.WriteAllText(overlordPath, JsonConvert.SerializeObject(backData));
            File.WriteAllText(backData.internalData.backupPath, JsonConvert.SerializeObject(internalData.data));
            foreach (GKToyExternalData external in externalDatas)
            {
                File.WriteAllText(backData.externalDatas[external.name].backupPath, JsonConvert.SerializeObject(external.data));
            }
        }
        /// <summary>
        /// 从备份文件还原数据
        /// </summary>
        public bool Restore()
        {
            string filePath = string.Format("{0}/OverlordBackup/overlord_{1}_back", Path.GetDirectoryName(AssetDatabase.GetAssetPath(internalData)), internalData.name);
            if (!File.Exists(filePath))
            {
                filePath = EditorUtility.OpenFilePanel("Restore data", Application.dataPath, "");
            }
            try
            {
                OverlordBackData backData = JsonConvert.DeserializeObject<OverlordBackData>(File.ReadAllText(filePath));
                GKToyExternalData internData = AssetDatabase.LoadAssetAtPath<GKToyExternalData>(backData.internalData.resourcePath);
                internData.data = JsonConvert.DeserializeObject<GKToyData>(File.ReadAllText(backData.internalData.backupPath));
                internData.data.Init(this);
                List<GKToyExternalData> externDatas = new List<GKToyExternalData>();
                foreach (var external in backData.externalDatas)
                {
                    GKToyExternalData externData = AssetDatabase.LoadAssetAtPath<GKToyExternalData>(external.Value.resourcePath);
                    externData.data = JsonConvert.DeserializeObject<GKToyData>(File.ReadAllText(external.Value.backupPath));
                    externData.data.Init(this);
                    externDatas.Add(externData);
                }
                isPlaying = backData.isPlaying;
                internalData = internData;
                externalDatas = externDatas;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region ExternalData
        // 运行时新增外部数据.
        public void AddExternalData(GKToyExternalData d)
        {
            if (externalDatas.Contains(d))
                return;
            externalDatas.Add(d);
            _tmpNewData.Add(d.data);
        }
        #endregion

        #region Variable
        // 设置变量值.
        public void SetVariableValue(string name, object val, int idx = 0)
        {
            GKToyData d = null;
            if (0 != idx && idx - 1 < externalDatas.Count)
                d = externalDatas[idx - 1].data;
            else
                d = internalData.data;

            if (null == d)
                return;

            if (_varDict[idx].ContainsKey(name))
                ((GKToyVariable)(_varDict[idx][name])).SetValue(val);
        }

        // 获取变量值.
        public object GetVariableValue(string name, int idx = 0)
        {
            GKToyData d = null;
            if (0 != idx && idx - 1 < externalDatas.Count)
                d = externalDatas[idx - 1].data;
            else
                d = internalData.data;
            
            if (null == d)
                return null;

            if (_varDict[idx].ContainsKey(name))
                return ((GKToyVariable)(_varDict[idx][name])).GetValue();
            else
                return null;
        }

        // 获取变量.
        public object GetVariable(string name, int idx = 0)
        {
            GKToyData d = null;
            if (0 != idx && idx - 1 < externalDatas.Count)
                d = externalDatas[idx - 1].data;
            else
                d = internalData.data;

            if (null == d)
                return null;

            if (_varDict[idx].ContainsKey(name))
                return _varDict[idx][name];
            else
                return null;
        }

        // 根据对象类型获取相同类型变量名称列表.
        List<string> _tmpVarNames = new List<string>();
        public List<string> GetVariableNameListByType(Type type, int idx = 0)
        {
            _tmpVarNames.Clear();
            GKToyData d = null;
            if (0 != idx && idx - 1 < externalDatas.Count)
                d = externalDatas[idx - 1].data;
            else
                d = internalData.data;

            if (null == d)
                return _tmpVarNames;

            foreach (var v in d.variableLst)
            {
                if (type == v.Value[0].GetType())
                {
                    foreach (var ele in v.Value)
                        _tmpVarNames.Add(((GKToyVariable)ele).Name);
                    return _tmpVarNames;
                }
            }
            return _tmpVarNames;
        }

        // 根据对象类型获取相同类型变量列表.
        public List<object> GetVariableListByType(Type type, int idx = 0)
        {
            _tmpVarNames.Clear();
            GKToyData d = null;
            if (0 != idx && idx - 1 < externalDatas.Count)
                d = externalDatas[idx - 1].data;
            else
                d = internalData.data;

            if (null == d)
                return null;

            foreach (var v in d.variableLst)
            {
                if (type == v.Value[0].GetType())
                {
                    return v.Value;
                }
            }
            return null;
        }
        #endregion

        #endregion

        #region PrivateMethod
        // Use this for initialization.
        void Start()
        {
            toyMakerBase = Settings.toyMakerBase;
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
			{
                foreach(var sm in stateMachines)
                    sm.Update();
			}
            // 运行时新数据初始化.
            if(0 != _tmpNewData.Count)
            {
                foreach(var t in _tmpNewData)
                {
                    _InitData(t);
                }
                _tmpNewData.Clear();
                _InitVariable();
            }
		}

        protected void Init()
        {
            // 初始化内置数据.
            _InitData(internalData.data);
            // 初始化外部数据.
            foreach (var d in externalDatas)
                _InitData(d.data);
            _InitVariable();
        }

        // 反序列化及有限状态机初始化.
        protected void _InitData(GKToyData d)
        {
            d.Init(this);
            GKNodeStateMachine machine = new GKNodeStateMachine(d.nodeLst.Values.ToList());
#if UNITY_EDITOR
            List<GKStateListMachineBase<int>.NewStateStruct> groupIds = new List<GKStateListMachineBase<int>.NewStateStruct>();
            foreach (GKToyNode node in d.nodeLst.Values)
            {
                if (NodeType.Group == node.nodeType)
                    groupIds.Add(new GKStateListMachineBase<int>.NewStateStruct(node.id, "", null));
            }
            machine.GoToState(groupIds);
#endif
            stateMachines.Add(machine); 
        }

        // 添加数据源变量信息至宿主中, 使宿主可直接进行操作.
        protected void _AddVariable(GKToyData d, int idx)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var lst in d.variableLst.Values)
            {
                foreach (var obj in lst)
                {
                    if (!dict.ContainsKey(((GKToyVariable)obj).Name))
                        dict.Add(((GKToyVariable)obj).Name, obj);
                }
            }
            _varDict.Add(idx, dict);
        }

        // 初始化变量.
        protected void _InitVariable()
        {
            _varDict.Clear();
            _AddVariable(internalData.data, 0);
            for (int i = 0; i < externalDatas.Count; i++)
            {
                if (null == externalDatas[i])
                    continue;

                _AddVariable(externalDatas[i].data, i + 1);
            }
        }
        #endregion
    }

    public enum NodeType
    {
        Node = 0,
        Action,
        Condition,
		Decoration,
		Group = 10,
        VirtualNode
    }
    /// <summary>
    /// overlord备份数据类
    /// </summary>
    [Serializable]
    public class OverlordBackData
    {
        public BackupDataItem internalData;
        public Dictionary<string, BackupDataItem> externalDatas;
        public bool isPlaying;
        /// <summary>
        /// 从json反序列化需要的无参构造函数
        /// </summary>
        public OverlordBackData() { }

        public OverlordBackData(string dataPath, GKToyBaseOverlord overlord)
        {
            internalData = new BackupDataItem(AssetDatabase.GetAssetPath(overlord.internalData), string.Format("{0}{1}_back", dataPath, overlord.internalData.name));
            externalDatas = new Dictionary<string, BackupDataItem>();
            foreach (GKToyExternalData external in overlord.externalDatas)
            {
                externalDatas.Add(external.name, new BackupDataItem(AssetDatabase.GetAssetPath(external), string.Format("{0}{1}_back", dataPath, external.name)));
            }
            isPlaying = overlord.isPlaying;
        }
    }
    /// <summary>
    /// GKToyExternalData路径数据类
    /// </summary>
    [Serializable]
    public class BackupDataItem
    {
        public string resourcePath;
        public string backupPath;
        public BackupDataItem() { }
        public BackupDataItem(string _resourcePath,string _backupPath)
        {
            resourcePath = _resourcePath;
            backupPath = _backupPath;
        }
    }
}
