using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using GKBase;

namespace GKUI
{
    public class GKUIEditor
    {

        #region PublicField
        public enum UIAncherType
        {
            FullScree = 0,
            MiddleCenter,
        }
        #endregion

        #region privateField
        #endregion

        #region PublicMethod
        // Create UI node.
        static public GameObject CreateNode(string name, GameObject parent, string layerName)
        {
            GameObject go = new GameObject(name);
            GK.SetParent(go, parent, false);
            go.layer = GK.LayerId(layerName);
            return go;
        }

        static public void AdjustNodeData(GameObject go, Vector3 pos, Vector4 offest, Vector2 size, UIAncherType ancher = UIAncherType.MiddleCenter)
        {
            var tran = GK.GetOrAddComponent<RectTransform>(go);
            if (null != tran)
            {
                tran.sizeDelta = size;
                tran.pivot = new Vector2(0.5f, 0.5f);
                tran.localPosition = Vector3.zero;

                if (UIAncherType.MiddleCenter == ancher)
                {

                    tran.anchorMax = new Vector2(0.5f, 0.5f);
                    tran.anchorMin = new Vector2(0.5f, 0.5f);

                    float x = pos.x - (UIController.width - size.x) * 0.5f;
                    float y = UIController.height * 0.5f - (UIController.height - pos.y - size.y * 0.5f);

                    tran.localPosition = new Vector3(x, y, 0);


                }
                else if (UIAncherType.FullScree == ancher)
                {

                    tran.anchorMax = new Vector2(1, 1);
                    tran.anchorMin = new Vector2(0, 0);
                    tran.offsetMin = new Vector2(offest.x, offest.y);
                    tran.offsetMax = new Vector2(offest.z, offest.w);
                }
            }
        }
        #endregion

        #region PrivateMethod
        #endregion
    }
}