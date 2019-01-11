using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GKUI
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        Color32 _topColor = Color.white;
        [SerializeField]
        Color32 _bottomColor = Color.black;

        override public void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }

        override public void ModifyMesh(Mesh mesh)
        {
            if (!IsActive())
            {
                return;
            }

            Vector3[] vertexList = mesh.vertices;
            int count = mesh.vertexCount;
            if (count > 0)
            {
                float _bottomColorY = vertexList[0].y;
                float _topColorY = vertexList[0].y;

                for (int i = 1; i < count; i++)
                {
                    float y = vertexList[i].y;
                    if (y > _topColorY)
                    {
                        _topColorY = y;
                    }
                    else if (y < _bottomColorY)
                    {
                        _bottomColorY = y;
                    }
                }
                List<Color32> colors = new List<Color32>();
                float uiElementHeight = _topColorY - _bottomColorY;
                for (int i = 0; i < count; i++)
                {
                    colors.Add(Color32.Lerp(_bottomColor, _topColor, (vertexList[i].y - _bottomColorY) / uiElementHeight));
                }
                mesh.SetColors(colors);
            }
        }

        public void ModifyVertices(List<UIVertex> vertexList)
        {
            if (!IsActive() || vertexList.Count < 4)
            {
                return;
            }
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
            if (vertexList.Count == 4)
            {
                SetVertexColor(vertexList, 0, _bottomColor);
                SetVertexColor(vertexList, 1, _topColor);
                SetVertexColor(vertexList, 2, _topColor);
                SetVertexColor(vertexList, 3, _bottomColor);
#else //This if has to be changed if you are using version 5.2.1p3 or later patches of 5.2.1 Use the _bottomColor code for it to work.
            if (vertexList.Count == 6)
            {
                _SetVertexColor(vertexList, 0, _bottomColor);
                _SetVertexColor(vertexList, 1, _topColor);
                _SetVertexColor(vertexList, 2, _topColor);
                _SetVertexColor(vertexList, 3, _topColor);
                _SetVertexColor(vertexList, 4, _bottomColor);
                _SetVertexColor(vertexList, 5, _bottomColor);
#endif
            }
            else
            {
                float _bottomColorPos = vertexList[vertexList.Count - 1].position.y;
                float _topColorPos = vertexList[0].position.y;

                float height = _topColorPos - _bottomColorPos;

                for (int i = 0; i < vertexList.Count; i++)
                {
                    UIVertex v = vertexList[i];
                    v.color *= Color.Lerp(_topColor, _bottomColor, ((v.position.y) - _bottomColorPos) / height);
                    vertexList[i] = v;
                }
            }
        }

        void _SetVertexColor(List<UIVertex> vertexList, int index, Color color)
        {
            UIVertex v = vertexList[index];
            v.color = color;
            vertexList[index] = v;
        }
    }
}