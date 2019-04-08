using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using GKToy;
using GKToyTaskDialogue;
using System.Xml;

namespace GKToyTaskEditor
{
    class GKToyMakerTaskDataExporter : GKToyMakerDataExporter
    {
        static List<string> taskTextProps = new List<string>
        {
            "TaskName",
            "TaskTarget",
            "TaskDesc",
            "TaskCanSubmit",
            "TargetInfo",
            "TargetText"
        };
        static List<string> dfgProps = new List<string>
        {
            "AcceptDfgObject",
            "SubmitDfgObject",
            "InteractDfgObject"
        };
        static List<string> replaceProps = new List<string>
        {
            "TargetInfo",
            "TargetText"
        };
        /// <summary>
        /// 导出客户端数据
        /// </summary>
        /// <param name="data">要导出的数据源</param>
        /// <param name="destPath"></param>
        static public void ExportClientData(GKToyData data, string destPath, string fileName)
        {
            GameData mainTaskData = new GameData();
            GameData subTaskData = new GameData();
            GameData taskConfigData = new GameData();
            GameData npcTalkData = new GameData();
            GameData taskTextData = new GameData();
            List<GKToyData> dfgDatas = new List<GKToyData>();
            NodeElement tmpItem;
            Type exportType = typeof(ExportClientAttribute);
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectory(destPath);
            string subPath = string.Format("{0}/{1}", destPath, fileName);
            if (!Directory.Exists(subPath))
                GKFile.GKFileUtil.CreateDirectory(subPath);
            data.LoadNodes();
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType)
                    continue;
                if ("GKToyTaskEditor.GKToyTask" == node.className)
                {
                    tmpItem = new NodeElement();
                    tmpItem.attrs = _GetFieldsWithAttribute(((GKToyTask)node).TaskID.ToString(), node, exportType, taskTextData, dfgDatas, data);
                    mainTaskData.Add(tmpItem);
                    _FindSubTask(data, node, node, subTaskData, taskConfigData, exportType, 1, taskTextData, dfgDatas);
                }
            }
            // 导出任务主表lua.
            string filePath = string.Format("{0}/Task.lua", subPath);
            StreamWriter stream = new StreamWriter(filePath, false);
            try
            {
                stream.Write(_DataToLua("Task", mainTaskData));
            }
            finally
            {
                stream.Close();
            }
            // 导出任务配置表lua.
            filePath = string.Format("{0}/TaskConfig.lua", subPath);
            stream = new StreamWriter(filePath, false);
            try
            {
                stream.Write(_DataToLua("TaskConfig", taskConfigData));
            }
            finally
            {
                stream.Close();
            }
            // 导出子任务表lua.
            filePath = string.Format("{0}/TaskTarget.lua", subPath);
            stream = new StreamWriter(filePath, false);
            try
            {
                stream.Write(_DataToLua("TaskTarget", subTaskData));
            }
            finally
            {
                stream.Close();
            }
            // 导出taskText lua.
            filePath = string.Format("{0}/TaskText.lua", subPath);
            stream = new StreamWriter(filePath, false);
            try
            {
                stream.Write(_DataToLua("TaskText", taskTextData));
            }
            finally
            {
                stream.Close();
            }
            // 导出任务对话lua.
            string dfgPath = string.Format("{0}/TaskDfg", destPath);
            if (!Directory.Exists(dfgPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(dfgPath);
            foreach(GKToyData dfgData in dfgDatas)
            {
                npcTalkData.AddRange(GKToyMakerDialogueDataExporter.ExportClientData(dfgData, string.Format("{0}/{1}.lua", dfgPath, dfgData.name)));
            }
            // 导出npctalk lua.
            filePath = string.Format("{0}/NpcTalk.lua", subPath);
            stream = new StreamWriter(filePath, false);
            try
            {
                stream.Write(_DataToLua("NpcTalk", npcTalkData));
            }
            finally
            {
                stream.Close();
            }
            // prize.

        }

        /// <summary>
        /// 导出服务器数据
        /// </summary>
        /// <param name="dataType">数据类型：1-客户端，2-服务器</param>
        static public void ExportServerData(GKToyData data, string destPath, string fileName)
        {
            GameData mainTaskData = new GameData();
            GameData subTaskData = new GameData();
            GameData taskConfigData = new GameData();
            GameData npcTalkData = new GameData();
            GameData taskTextData = new GameData();
            List<GKToyData> dfgDatas = new List<GKToyData>();
            NodeElement tmpItem;
            Type exportType = typeof(ExportClientAttribute);
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectory(destPath);
            string subPath = string.Format("{0}/{1}", destPath, fileName);
            if (!Directory.Exists(subPath))
                GKFile.GKFileUtil.CreateDirectory(subPath);
            data.LoadNodes();
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType)
                    continue;
                if ("GKToyTaskEditor.GKToyTask" == node.className)
                {
                    tmpItem = new NodeElement();
                    tmpItem.attrs = _GetFieldsWithAttribute(((GKToyTask)node).TaskID.ToString(), node, exportType, taskTextData, dfgDatas, data);
                    mainTaskData.Add(tmpItem);
                    _FindSubTask(data, node, node, subTaskData, taskConfigData, exportType, 1, taskTextData, dfgDatas);
                }
            }
            // 导出任务主表lua.
            string filePath = string.Format("{0}/Task.xml", subPath);
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            FileStream stream = new FileStream(filePath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, mainTaskData);
            }
            finally
            {
                stream.Close();
            }
            // 导出任务配置表lua.
            filePath = string.Format("{0}/TaskConfig.xml", subPath);
            stream = new FileStream(filePath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, taskConfigData);
            }
            finally
            {
                stream.Close();
            }
            // 导出子任务表lua.
            filePath = string.Format("{0}/TaskTarget.xml", subPath);
            stream = new FileStream(filePath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, subTaskData);
            }
            finally
            {
                stream.Close();
            }
            // 导出taskText lua.
            filePath = string.Format("{0}/TaskText.xml", subPath);
            stream = new FileStream(filePath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, taskTextData);
            }
            finally
            {
                stream.Close();
            }
            // 导出任务对话lua.
            string dfgPath = string.Format("{0}/TaskDfg", destPath);
            if (!Directory.Exists(dfgPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(dfgPath);
            foreach (GKToyData dfgData in dfgDatas)
            {
                npcTalkData.AddRange(GKToyMakerDialogueDataExporter.ExportServerData(dfgData, string.Format("{0}/{1}.xml", dfgPath, dfgData.name)));
            }
            // 导出npctalk lua.
            filePath = string.Format("{0}/NpcTalk.xml", subPath);
            stream = new FileStream(filePath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, npcTalkData);
            }
            finally
            {
                stream.Close();
            }
            // prize.
        }

        static void _FindSubTask(GKToyData data, GKToyNode parenNode, GKToyNode curNode, GameData subTaskData, GameData taskConfigData, Type exportType, int layer, GameData taskTextData, List<GKToyData> dfgDatas)
        {
            foreach (Link link in curNode.links)
            {
                NodeElement tmpItem = new NodeElement();
                if (!data.nodeLst.ContainsKey(link.next))
                    return;
                curNode = (GKToyNode)data.nodeLst[link.next];
                if ("GKToyTaskEditor.GKToyTask" == curNode.className)
                    return;
                tmpItem.attrs = _GetFieldsWithAttribute(((GKToySubTask)curNode).TargetID.ToString(), curNode, exportType, taskTextData, dfgDatas, data);
                subTaskData.Add(tmpItem);
                taskConfigData.Add(_GenerateTaskConfigData(parenNode, curNode, layer));
                _FindSubTask(data, parenNode, curNode, subTaskData, taskConfigData, exportType, layer + 1, taskTextData, dfgDatas);
            }
        }

        static NodeElement _GenerateTaskConfigData(GKToyNode mainNode, GKToyNode subNode, int rank)
        {
            NodeElement taskConfig = new NodeElement();
            List<NodeAttr> fields = new List<NodeAttr>();
            fields.Add(new NodeAttr("TaskID", mainNode.LiteralId.ToString()));
            fields.Add(new NodeAttr("TargetID", subNode.LiteralId.ToString()));
            fields.Add(new NodeAttr("TarSeq", rank.ToString()));
            if (0 == subNode.links.Count)
                fields.Add(new NodeAttr("IfAdd", "0"));
            else
                fields.Add(new NodeAttr("IfAdd", "1"));
            taskConfig.attrs = fields;
            return taskConfig;
        }

        static NodeElement _GenerateTaskTextData(string id, string text)
        {
            NodeElement taskText = new NodeElement();
            List<NodeAttr> fields = new List<NodeAttr>();
            fields.Add(new NodeAttr("TextID", id));
            fields.Add(new NodeAttr("Text", text));
            taskText.attrs = fields;
            return taskText;
        }

        /// <summary>
        /// 在实例中读取带有特定Attribute的属性
        /// </summary>
        /// <param name="obj">读取的实例</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>属性列表</returns>
        protected static List<NodeAttr> _GetFieldsWithAttribute(string id, object obj, Type attribute, GameData taskTextData, List<GKToyData> dfgDatas, GKToyData nodeData)
        {
            if (!obj.GetType().IsClass)
                return null;
            Type type = obj.GetType();
            object[] attrs;
            List<NodeAttr> fields = new List<NodeAttr>();
            // 读取所有属性.
            type = obj.GetType();
            List<PropertyInfo> allProperties = new List<PropertyInfo>();
            do
            {
                allProperties.AddRange(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            } while (null != type || typeof(GKToyNode) == type);
            foreach (PropertyInfo property in allProperties)
            {
                attrs = property.GetCustomAttributes(attribute, true);
                if (0 != attrs.Length)
                {
                    object val = property.GetValue(obj, null);
                    Type propType = val.GetType();
                    // 存储译文.
                    if (taskTextProps.Contains(property.Name))
                    {
                        string textId = string.Format("{0}_{1}", property.Name, id);
                        string valStr = _DealWithAttributes(val, propType);
                        // id替换.
                        if (replaceProps.Contains(property.Name))
                        {
                            int start = valStr.IndexOf("[");
                            int end = valStr.IndexOf("]");
                            if (-1 < start && 0 < end - start - 1)
                            {
                                string name = valStr.Substring(start + 1, end - start - 1);
                                PropertyInfo innerPorp = obj.GetType().GetProperty(name);
                                if (null != innerPorp)
                                {
                                    object innerVal = innerPorp.GetValue(obj, null);
                                    Type innerType = innerVal.GetType();
                                    valStr = valStr.Replace(name, _DealWithAttributes(innerVal, innerType));
                                }
                            }
                        }
                        fields.Add(new NodeAttr(property.Name, textId));
                        taskTextData.Add(_GenerateTaskTextData(textId, valStr));
                    }
                    else if ("PreTask" == property.Name)
                    {
                        string valStr = "";
                        GKToyTask task = (GKToyTask)obj;
                        if (0 < task.preTaskIds.Count)
                        {
                            for (int i = 0; i < task.preTaskIds.Count; ++i)
                            {
                                if (i < task.preSeperator.Count)
                                {
                                    valStr = string.Format("{0}{1}{2}", valStr, ((GKToyTask)nodeData.nodeLst[task.preTaskIds[i]]).TaskID, task.preSeperator[i]);
                                }
                                else
                                {
                                    valStr = string.Format("{0}{1}", valStr, ((GKToyTask)nodeData.nodeLst[task.preTaskIds[i]]).TaskID);
                                }
                            }
                        }
                        fields.Add(new NodeAttr(property.Name, valStr));
                    }
                    else if ("NextTask" == property.Name)
                    {
                        string valStr = "";
                        GKToyTask task = (GKToyTask)obj;
                        bool seperator = false;
                        if (0 < task.nextTaskIds.Count)
                        {
                            foreach (int next in task.nextTaskIds)
                            {
                                if (seperator)
                                {
                                    valStr = string.Format("{0},{1}", valStr, ((GKToyTask)nodeData.nodeLst[next]).TaskID);
                                }
                                else
                                {
                                    valStr = string.Format("{0}{1}", valStr, ((GKToyTask)nodeData.nodeLst[next]).TaskID);
                                    seperator = true;
                                }
                            }
                        }
                        fields.Add(new NodeAttr(property.Name, valStr));
                    }
                    // 处理对话.
                    else if (dfgProps.Contains(property.Name))
                    {
                        dfgDatas.Add(((GKToySharedGameObject)val).Value.GetComponent<GKToyBaseOverlord>().internalData.data);
                    }
                    else
                    {
                        attrs = property.GetCustomAttributes(typeof(XmlElementAttribute), true);
                        if (0 != attrs.Length)
                        {
                            fields.Add(new NodeAttr(((XmlElementAttribute)attrs[0]).ElementName, _DealWithAttributes(val, propType)));
                        }
                        else
                        {
                            fields.Add(new NodeAttr(property.Name, _DealWithAttributes(val, propType)));
                        }
                    }
                }
            }
            if (0 == fields.Count)
                return null;
            else
                return fields;
        }
        /// <summary>
        /// 序列化结点属性值
        /// </summary>
        /// <param name="val"></param>
        /// <param name="propType"></param>
        /// <returns></returns>
        static string _DealWithAttributes(object val, Type propType)
        {
            // 提取自定义变量的值.
            if (propType.IsSubclassOf(typeof(GKToyVariable)))
            {
                var v = ((GKToyVariable)val).GetValue();
                if (v is IList)
                {
                    string valStr = "";
                    bool seperator = false;
                    foreach (var valElement in (IList)v)
                    {
                        if (seperator)
                        {
                            valStr = string.Format("{0};{1}", valStr, valElement.ToString());
                        }
                        else
                        {
                            valStr = string.Format("{0}{1}", valStr, valElement.ToString());
                            seperator = true;
                        }
                    }
                    return valStr;
                }
                else
                {
                    return v.ToString();
                }
            }
            // 序列化list.
            if (val is IList)
            {
                string valStr = "";
                bool seperator = false;
                foreach (var valElement in (IList)val)
                {
                    if (seperator)
                    {
                        valStr = string.Format("{0};{1}", valStr, valElement.ToString());
                    }
                    else
                    {
                        valStr = string.Format("{0}{1}", valStr, valElement.ToString());
                        seperator = true;
                    }
                }
                return valStr;
            }
            return val.ToString();
        }
    }
}
