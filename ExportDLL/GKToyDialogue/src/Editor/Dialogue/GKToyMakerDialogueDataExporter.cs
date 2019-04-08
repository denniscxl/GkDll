using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using GKToy;
using UnityEngine;

namespace GKToyDialogue
{
    class GKToyMakerDialogueDataExporter : GKToyMakerDataExporter
    {
        /// <summary>
        /// 导出客户端数据
        /// </summary>
        /// <param name="data">要导出的数据源</param>
        /// <param name="destPath"></param>
        static public new void ExportClientData(GKToyData data, string destPath)
        {
            GameData gameData = new GameData();
            NodeElement tmpItem;
            int endID = 0;
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                if ("GKToy.GKToyEnd" == node.className)
                {
                    endID = node.id;
                    break;
                }
            }
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(destPath);
            bool isSpecialNode;
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                isSpecialNode = false;
                tmpItem = new NodeElement();
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType)
                    continue;
                if ("GKToyDialogue.GKToyDialogueCondition" == node.className && 0 == ((GKToyDialogueCondition)node).OutPutType.Value)
                {
                    isSpecialNode = true;
                }
                tmpItem.attrs = _GetFieldsWithAttribute(node, typeof(ExportClientAttribute), endID, node.id == endID, isSpecialNode);
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
        }

        /// <summary>
        /// 导出服务器数据
        /// </summary>
        /// <param name="dataType">数据类型：1-客户端，2-服务器</param>
        static public new void ExportServerData(GKToyData data, string destPath)
        {
            GameData gameData = new GameData();
            NodeElement tmpItem;
            int endID = 0;
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                if ("GKToy.GKToyEnd" == node.className)
                {
                    endID = node.id;
                    break;
                }
            }
            if (!Directory.Exists(destPath))
                GKFile.GKFileUtil.CreateDirectoryFromFileName(destPath);
            bool isSpecialNode;
            foreach (GKToyNode node in data.nodeLst.Values)
            {
                isSpecialNode = false;
                tmpItem = new NodeElement();
                if (NodeType.Group == node.nodeType || NodeType.VirtualNode == node.nodeType)
                    continue;
                if ("GKToy.GKToyDialogueCondition" == node.className && 0 == ((GKToyDialogueCondition)node).OutPutType.Value)
                {
                    isSpecialNode = true;
                }
                tmpItem.attrs = _GetFieldsWithAttribute(node, typeof(ExportServerAttribute), endID, node.id == endID, isSpecialNode);
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
        }


        /// <summary>
        /// 在实例中读取带有特定Attribute的属性
        /// </summary>
        /// <param name="obj">读取的实例</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>属性列表</returns>
        static List<NodeAttr> _GetFieldsWithAttribute(object obj, Type attribute, int endID, bool isEnd, bool isSpecialNode)
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
            bool isSpecialProp = false;
            List<NodeAttr> fields = new List<NodeAttr>();
            foreach (FieldInfo field in allFields)
            {
                attrs = field.GetCustomAttributes(attribute, true);
                if (0 != attrs.Length)
                {
                    object val = field.GetValue(obj);
                    Type propType = val.GetType();
                    attrs = field.GetCustomAttributes(typeof(XmlElementAttribute), true);
                    if (0 != attrs.Length)
                    {
                        fields.Add(new NodeAttr(((XmlElementAttribute)attrs[0]).ElementName, _DealWithAttributes(val, propType, endID, isEnd, isSpecialProp)));
                    }
                    else
                    {
                        fields.Add(new NodeAttr(field.Name, _DealWithAttributes(val, propType, endID, isEnd, isSpecialProp)));
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
                isSpecialProp = false;
                attrs = property.GetCustomAttributes(attribute, true);
                if (0 != attrs.Length)
                {
                    object val = property.GetValue(obj, null);
                    Type propType = val.GetType();
                    attrs = property.GetCustomAttributes(typeof(XmlElementAttribute), true);
                    if (isSpecialNode)
                    {
                        if ("IfYesNode" == property.Name || "IfNoNode" == property.Name)
                        {
                            isSpecialProp = true;
                        }
                    }
                    if (0 != attrs.Length)
                    {
                        fields.Add(new NodeAttr(((XmlElementAttribute)attrs[0]).ElementName, _DealWithAttributes(val, propType, endID, isEnd, isSpecialProp)));
                    }
                    else
                    {
                        fields.Add(new NodeAttr(property.Name, _DealWithAttributes(val, propType, endID, isEnd, isSpecialProp)));
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
        /// <param name="val">属性值</param>
        /// <param name="propType">属性类型</param>
        /// <param name="endId">结束节点id</param>
        /// <param name="isEnd">当前是否结束节点的属性</param>
        /// <param name="isSpecialProp">特殊链接属性：false-非特殊属性/不需要链接到结束节点;true-需要将值修改为结束节点的id</param>
        /// <returns></returns>
        static string _DealWithAttributes(object val, Type propType, int endId, bool isEnd, bool isSpecialProp)
        {
            // 提取自定义变量的值.
            if (propType.IsSubclassOf(typeof(GKToyVariable)))
            {
                var v = ((GKToyVariable)val).GetValue();
                string name = ((GKToyVariable)val).Name;
                if (isSpecialProp && 0 == (int)v)
                {
                    return endId.ToString();
                }
                else if (v is IList)
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
                if (0 == links.Count && !isEnd)
                {
                    valStr = string.Format("{0}{1}", valStr, endId);
                }
                else
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
            // 序列化其他list.
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
