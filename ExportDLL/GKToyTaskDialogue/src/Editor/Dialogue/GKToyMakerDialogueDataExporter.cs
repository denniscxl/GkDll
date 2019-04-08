using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using GKToy;
using UnityEngine;

namespace GKToyTaskDialogue
{
    public class GKToyMakerDialogueDataExporter : GKToyMakerDataExporter
    {
        static List<string> npcTalkProps = new List<string>
        {
            "MenuText",
            "SpeakText",
            "SpeakText2"
        };
        static List<string> npcTalkNames = new List<string>
        {
            "MT",
            "ST",
            "ST2"
        };
        /// <summary>
        /// 导出客户端数据
        /// </summary>
        /// <param name="data">要导出的数据源</param>
        /// <param name="destPath"></param>
        static public new List<NodeElement> ExportClientData(GKToyData data, string destPath)
        {
            GameData gameData = new GameData();
            List<NodeElement> npcTalkText = new List<NodeElement>();
            NodeElement tmpItem;
            int startId = 0;
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(destPath);
            data.LoadNodes();
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                tmpItem = new NodeElement();
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType || "GKToy.GKToyEnd" == node.className)
                    continue;
                if("GKToy.GKToyStart" == node.className)
                {
                    if (0 < node.links.Count)
                    {
                        startId = node.links[0].next;
                    }
                    continue;
                }
                tmpItem.attrs = _GetFieldsWithAttribute(((GKToyDialogue)node).NodeID.ToString(), data.name, node, typeof(ExportClientAttribute), npcTalkText, node.id == startId);
                gameData.Add(tmpItem);
            }
            // 导出lua.
            StreamWriter stream = new StreamWriter(destPath, false);
            try
            {
                stream.Write(_DataToLua(data.name, gameData));
            }
            finally
            {
                stream.Close();
            }
            return npcTalkText;
        }

        /// <summary>
        /// 导出服务器数据
        /// </summary>
        /// <param name="dataType">数据类型：1-客户端，2-服务器</param>
        static public new List<NodeElement> ExportServerData(GKToyData data, string destPath)
        {
            GameData gameData = new GameData();
            List<NodeElement> npcTalkText = new List<NodeElement>();
            NodeElement tmpItem;
            int startId = 0;
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(destPath);
            data.LoadNodes();
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                tmpItem = new NodeElement();
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType || "GKToy.GKToyEnd" == node.className)
                    continue;
                if ("GKToy.GKToyStart" == node.className)
                {
                    if (0 < node.links.Count)
                    {
                        startId = node.links[0].next;
                    }
                    continue;
                }
                tmpItem.attrs = _GetFieldsWithAttribute(((GKToyDialogue)node).NodeID.ToString(),data.name, node, typeof(ExportServerAttribute), npcTalkText, node.id == startId);
                gameData.Add(tmpItem);
            }
            // 导出xml.
            XmlSerializer serializer = new XmlSerializer(typeof(GameData));
            FileStream stream = new FileStream(destPath, FileMode.Create);
            try
            {
                serializer.Serialize(stream, gameData);
            }
            finally
            {
                stream.Close();
            }
            return npcTalkText;
        }


        /// <summary>
        /// 在实例中读取带有特定Attribute的属性
        /// </summary>
        /// <param name="obj">读取的实例</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>属性列表</returns>
        static List<NodeAttr> _GetFieldsWithAttribute(string id, string dataName, object obj, Type attribute, List<NodeElement> npcTalkText, bool isStartNode)
        {
            if (!obj.GetType().IsClass)
                return null;
            Type type = obj.GetType();
            List<FieldInfo> allFields = new List<FieldInfo>();
            // 读取所有属性.
            do
            {
                allFields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            } while (null != type);
            object[] attrs;
            List<NodeAttr> fields = new List<NodeAttr>();
            foreach (FieldInfo field in allFields)
            {
                attrs = field.GetCustomAttributes(attribute, true);
                if (0 != attrs.Length)
                {
                    object val = field.GetValue(obj);
                    Type propType = val.GetType();
                    if ("id" == field.Name)
                        continue;
                    attrs = field.GetCustomAttributes(typeof(XmlElementAttribute), true);
                    if (0 != attrs.Length)
                    {
                        fields.Add(new NodeAttr(((XmlElementAttribute)attrs[0]).ElementName, _DealWithAttributes(val, propType)));
                    }
                    else
                    {
                        fields.Add(new NodeAttr(field.Name, _DealWithAttributes(val, propType)));
                    }
                }
            }
            // 读取所有属性.
            type = obj.GetType();
            List<PropertyInfo> allProperties = new List<PropertyInfo>();
            do
            {
                allProperties.AddRange(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            } while (null != type);
            foreach (PropertyInfo property in allProperties)
            {
                attrs = property.GetCustomAttributes(attribute, true);
                if (0 != attrs.Length)
                {
                    object val = property.GetValue(obj, null);
                    Type propType = val.GetType();
                    if (npcTalkProps.Contains(property.Name))
                    {
                        string textId = string.Format("{0}_{1}_{2}", npcTalkNames[npcTalkProps.IndexOf(property.Name)], dataName, id);
                        fields.Add(new NodeAttr(property.Name, textId));
                        npcTalkText.Add(_GenerateNpcTextData(textId, _DealWithAttributes(val, propType)));
                    }
                    else if ("IsStartNode" == property.Name && isStartNode)
                    {
                        fields.Add(new NodeAttr(property.Name, "1"));
                    }
                    else if("NodeID" == property.Name)
                    {
                        fields.Add(new NodeAttr(property.Name, string.Format("{0}_{1}", dataName, _DealWithAttributes(val, propType))));
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
        static NodeElement _GenerateNpcTextData(string id, string text)
        {
            NodeElement npcText = new NodeElement();
            List<NodeAttr> fields = new List<NodeAttr>();
            fields.Add(new NodeAttr("TextID", id));
            fields.Add(new NodeAttr("Text", text));
            npcText.attrs = fields;
            return npcText;
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
            // 序列化link.
            if (propType == typeof(List<Link>))
            {
                string valStr = "";
                bool seperator = false;
                List<Link> links = (List<Link>)val;
                if (0 < links.Count)
                {
                    foreach (Link link in links)
                    {
                        if (seperator)
                        {
                            valStr = string.Format("{0};{1}", valStr, link.next);
                        }
                        else
                        {
                            valStr = string.Format("{0}{1}", valStr, link.next);
                            seperator = true;
                        }
                    }
                }
                return valStr;
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
