using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GKToy;
using System.Text;
 
public class TreeNode {

    #region PublicField
    public string name;
    public string key;
    public TreeNodeType nodeType = TreeNodeType.Item;
    public List<TreeNode> children = null;
    public bool isOpen = false;
    public Dictionary<string, string> pathData;
    #endregion

    #region PrivateField
    static TreeNode _instance = null;
    TreeNode _parent;
    #endregion

    #region PublicMethod
    static public TreeNode Get()
    {
        if (_instance == null)
        {
            _instance = new TreeNode();
        }
        return _instance;
    }

    public void InsertNode(TreeNode node)
    {
        if (this.children == null)
        {
            this.children = new List<TreeNode>();
        }
        children.Add(node);
        node._parent = this;
    }

    public void OpenAllNode(TreeNode node)
    {
        node.isOpen = true;
        if (node.children != null && node.children.Count > 0)
        {
            for (int i = 0; i < node.children.Count; i++)
            {
                OpenAllNode(node.children[i]);
            }
        }
    }

    public TreeNode GenerateFileTree(Dictionary<string, Dictionary<string, NodeTypeTreeAttribute>> data, string lang, List<string> invisibleNode)
    {
        TreeNode root = new TreeNode();
        pathData = data.Where(x => !invisibleNode.Contains(x.Key) && x.Value.ContainsKey(lang))
            .ToDictionary(x => string.Format("Root/{0}", x.Value[lang].treePath), x => x.Key);
        root = GenerateFileNode("", "Root/", pathData);
        root.isOpen = true;
        return root;
    }

    public TreeNode GenerateFileNode(string parentFullPath, string path, Dictionary<string, string> data)
    {
        TreeNode node = new TreeNode();
        string[] segment = path.Split('/');
        if (segment.Length > 1)
        {
            string name = segment[0];
            node.name = name;
            node.nodeType = TreeNodeType.Switch;
            string fullPath = string.Format("{0}{1}/", parentFullPath, name);
            List<string> allChildrenPath = data.Keys.Where(s => s.StartsWith(fullPath) && s != fullPath).ToList();
            List<string> dirList = new List<string>();
            for (int i = 0; i < allChildrenPath.Count; i++)
            {
                string childPath = allChildrenPath[i].Remove(0, fullPath.Length);
                string[] childPathSegment = childPath.Split('/');
                if (childPathSegment.Length > 1)
                {
                    string childDirPath = childPathSegment[0];
                    if (!dirList.Contains(childDirPath))
                    {
                        dirList.Add(childDirPath);
                        TreeNode childNode = GenerateFileNode(fullPath, string.Format("{0}/", childDirPath), data);
                        node.InsertNode(childNode);
                    }
                }
                else
                {
                    TreeNode childNode = GenerateFileNode(fullPath, childPath, data);
                    node.InsertNode(childNode);
                }
            }
        }
        else
        {
            node.name = path;
            string fullPath = string.Format("{0}{1}", parentFullPath, path);
            if (data.ContainsKey(fullPath))
            {
                node.key = data[fullPath];
            }
            node.nodeType = TreeNodeType.Item;
            data.Remove(path);
        }
        return node;
    }
    #endregion

    #region PrivateMethod
    #endregion
	
    public enum TreeNodeType
    {
        Item,
        Switch
    }
}
