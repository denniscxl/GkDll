﻿using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using GKBase;

namespace GKToy
{
    [System.Serializable]
    public class GKToyData : ICloneable
    {
        #region PublicField
        public string name = "Hello";
        public string comment = "";
        // 启动时运行.
        public bool startWhenEnable = false;
        // 完成后删除.
        public bool destoryWhenCompleted = true;
        public int nodeGuid = 0;
        public int linkGuid = 0;
        public int minLiteralId = 0;
        public int maxLiteralId = 0;
        public int curLiteralId = 0;
        // Node链表.
        public List<string> nodeData = new List<string>();
        public List<string> nodeTypeData = new List<string>();
        [JsonIgnore]
        public Dictionary<int, object> nodeLst = new Dictionary<int, object>();
        public bool variableChanged = false;
        public List<string> variableData = new List<string>();
        public List<string> variableTypeData = new List<string>();
        [JsonIgnore]
        public Dictionary<string, List<object>> variableLst = new Dictionary<string, List<object>>();
        #endregion

        #region PrivateField
        GKToyBaseOverlord _overlord;
        #endregion

        #region PublicMethod
        public void Init(GKToyBaseOverlord overlord)
        {
            _overlord = overlord;
            LoadVariable(overlord, this);
            LoadNodes();
        }

        // Clone 深拷贝.
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        // 变量元素.
        public void RemoveVariable(string key, object val)
        {
            if (variableLst.ContainsKey(key))
            {
                variableLst[key].Remove(val);

                if (0 == variableLst[key].Count)
                    variableLst.Remove(key);
            }
        }

        // 变量转化为Json存储.
        public void SaveVariable()
        {
            if (GKEditor.isUnityEditor())
            {
                // 设置场景有更新.
                if (!Application.isPlaying)
                {
                    variableData.Clear();
                    variableTypeData.Clear();
                    foreach (var objs in variableLst)
                    {
                        foreach (var obj in objs.Value)
                        {
                            //// 规避反序列化变量内容过载. 序列化时不对序列化Data处理.
                            //var tPropertyMappingOwner = ((GKToyVariable)obj).PropertyMappingOwner;
                            //((GKToyVariable)obj).PropertyMappingOwner = null;
                            //var tPropertyDataOwner = ((GKToyVariable)obj).PropertyDataOwner;
                            //((GKToyVariable)obj).PropertyDataOwner = null;

                            variableData.Add(JsonUtility.ToJson(obj, true));
                            variableTypeData.Add(objs.Key);

                            //((GKToyVariable)obj).PropertyMappingOwner = tPropertyMappingOwner;
                            //((GKToyVariable)obj).PropertyDataOwner = tPropertyDataOwner;
                        }
                    }
                    if (null != _overlord)
                    {
                        if (_overlord.gameObject.scene.name == null)
                        {
                            // prefab
                            GKEditor.SetPrefabDirty(_overlord.gameObject);
                        }
                        else
                        {
                            // scene gameobject
                            GKEditor.SetActiveSceneDirty();
                        }
                    }
                }
            }
        }

        // Json转化为变量.
        public void LoadVariable(GKToyBaseOverlord overlord, GKToyData data)
        {
            variableLst.Clear();
            int i = 0;
            foreach (var d in variableData)
            {
                Type t = Type.GetType(variableTypeData[i]);
                var v = JsonUtility.FromJson(d, t) as GKToyVariable;

                // 序列化后引用为堆内存, 需要重新指定.
                if (null != v)
                    v.InitializePropertyMapping(overlord, data);
                
                if (variableLst.ContainsKey(v.PropertyMapping))
                {
                    variableLst[v.PropertyMapping].Add(v);
                }
                else
                {
                    List<object> lst = new List<object>();
                    lst.Add(v);
                    variableLst.Add(v.PropertyMapping, lst);
                }
                i++;
            }
        }

        // 节点转化为Json存储.
        public void SaveNodes()
        {
            if (GKEditor.isUnityEditor())
            {
                // 设置场景有更新.
                if (!Application.isPlaying)
                {
                    nodeTypeData.Clear();
                    List<string> tmpNodeData = new List<string>();
                    foreach (var obj in nodeLst.Values)
                    {
                        tmpNodeData.Add(JsonUtility.ToJson(obj, true));
                        nodeTypeData.Add(((GKToyNode)obj).className);
                    }
                    bool isChanged = false;
                    if (tmpNodeData.Count == nodeData.Count)
                    {
                        for (int i = 0; i < tmpNodeData.Count; ++i)
                        {
                            if (!tmpNodeData[i].Equals(nodeData[i]))
                                isChanged = true;
                        }
                    }
                    else
                        isChanged = true;
                    if (isChanged && null != _overlord)
                    {
                        if (_overlord.gameObject.scene.name == null)
                        {
                            GKEditor.SetPrefabDirty(_overlord.gameObject);
                        }
                        else
                        {
                            GKEditor.SetActiveSceneDirty();
                        }

                    }
                    nodeData = tmpNodeData;
                }
            }
        }

        // Json转化为节点.
        public void LoadNodes()
        {
            nodeLst.Clear();
            int i = 0;
            bool isEditor = GKEditor.isUnityEditor();
            foreach (var d in nodeData)
            {
                Type t = GKToyMakerTypeManager.Instance().typeAssemblyDict[nodeTypeData[i]].GetType(nodeTypeData[i]);
                var n = (GKToyNode)JsonUtility.FromJson(d, t);
                n.Init(_overlord);
                if (isEditor && typeof(GKToyNodeGroup) == t)
                    ((GKToyNodeGroup)n).data = this;
                nodeLst.Add(n.id, n);
                i++;
            }
        }
        /// <summary>
        /// 还原数据
        /// </summary>
        /// <param name="overlord"></param>
        /// <returns>数据结构是否改变</returns>
        public bool RestoreData(GKToyBaseOverlord overlord)
        {
            _overlord = overlord;
            LoadVariable(overlord, this);
            nodeLst.Clear();
            int i = 0;
            bool isEditor = GKEditor.isUnityEditor();
            bool dataChanged = false;
            foreach (var d in nodeData)
            {
                Type t = GKToyMakerTypeManager.Instance().typeAssemblyDict[nodeTypeData[i]].GetType(nodeTypeData[i]);
                var n = (GKToyNode)JsonUtility.FromJson(d, t);
                if (n.Restore(d) && !dataChanged)
                {
                    dataChanged = true;
                }
                if (isEditor && typeof(GKToyNodeGroup) == t)
                    ((GKToyNodeGroup)n).data = this;
                nodeLst.Add(n.id, n);
                i++;
            }
            return dataChanged;
        }
        #endregion

        #region PrivateMethod
        #endregion
    }
}
