using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GKToy
{
	public class GKToyMakerTypeManager
	{
        #region PublicField         // 节点类型，Key-类型名称，Value-节点树路径(多语言列表).
        public Dictionary<string, Dictionary<string, NodeTypeTreeAttribute>> typeAttributeDict;
        // 节点描述，Key-类型名称，Value-节点描述(多语言列表).
        public Dictionary<string, Dictionary<string, NodeDescriptionAttribute>> desAttributeDict;
        // 节点图标，Key-类型名称，Value-图标路径.
        public Dictionary<string, NodeIconAttribute> iconAttributeDict;
        // 在节点树中隐藏的节点列表.
        public List<string> hideNodes;
        #endregion

        #region PrivateField
        static GKToyMakerTypeManager _instance;
        #endregion 
        #region PublicMethod
        static public GKToyMakerTypeManager Instance()
        {
            if (_instance == null)
            {
                _instance = new GKToyMakerTypeManager();
            }
            return _instance;
        }
        public GKToyMakerTypeManager()
        {
            Type parent = typeof(GKToyNode);
            if (typeAttributeDict != null)
                return;
            typeAttributeDict = new Dictionary<string, Dictionary<string, NodeTypeTreeAttribute>>();
            desAttributeDict = new Dictionary<string, Dictionary<string, NodeDescriptionAttribute>>();
            iconAttributeDict = new Dictionary<string, NodeIconAttribute>();
            hideNodes = new List<string>();
            List<Type> subTypes = parent.Assembly.GetTypes().Where(type => type.IsSubclassOf(parent)).ToList();
            foreach (Type type in subTypes)
            {
                var attributes = type.GetCustomAttributes(typeof(NodeTypeTreeAttribute), false);
                if (attributes.Length > 0)
                {
                    typeAttributeDict.Add("GKToy." + type.Name, ((NodeTypeTreeAttribute[])attributes).ToDictionary(x => x.lang));
                }
                var attributes2 = type.GetCustomAttributes(typeof(NodeDescriptionAttribute), false);
                if (attributes2.Length > 0)
                {
                    desAttributeDict.Add("GKToy." + type.Name, ((NodeDescriptionAttribute[])attributes2).ToDictionary(x => x.lang));
                }
                var attributes3 = type.GetCustomAttributes(typeof(NodeIconAttribute), false);
                if (attributes3.Length > 0)
                {
                    iconAttributeDict.Add("GKToy." + type.Name, (NodeIconAttribute)attributes3[0]);
                }
                var attributes4 = type.GetCustomAttributes(typeof(InvisibleNodeAttribute), false);
                if (attributes4.Length > 0)
                {
                    hideNodes.Add("GKToy." + type.Name);
                }
            }
        }
        #endregion 
        #region PrivateMethod
        #endregion
	}
	/// <summary>
	/// 节点类型定义Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class NodeTypeTreeAttribute : Attribute
	{
		// 节点树路径.
		public string treePath;
		// 语言，默认中文.
		public string lang;

		public NodeTypeTreeAttribute(string _treePath, string _lang = "Chinese")
		{
			treePath = _treePath;
			lang = _lang;
		}
	}
	/// <summary>
	/// 节点描述Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class NodeDescriptionAttribute : Attribute
	{
		// 描述.
		public string desc;
		// 语言，默认中文.
		public string lang;
		public NodeDescriptionAttribute(string _desc, string _lang = "Chinese")
		{
			desc = _desc;
			lang = _lang;
		}
	}
	/// <summary>
	/// 节点图标定义Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NodeIconAttribute : Attribute
	{
		// 自定义图标路径.
		public string iconPath;

		public NodeIconAttribute(string _iconPath) { iconPath = _iconPath; }
	}
	/// <summary>
	/// 关闭节点在节点树中的显示Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class InvisibleNodeAttribute : Attribute{}
}
