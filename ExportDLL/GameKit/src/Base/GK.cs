﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace GKBase
{
    public class GK
    {
        static public bool isGamePlaying
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlaying;
#else
			return true;		
#endif
            }
        }

        static public T[] ReverseArray<T>(T[] arr)
        {
            if (arr == null) return null;
            var n = arr.Length;
            var o = new T[n];
            for (int i = 0; i < n; i++)
            {
                o[n - i - 1] = arr[i];
            }
            return o;
        }

        static public List<T> ReverseArray<T>(List<T> arr)
        {
            if (arr == null) return null;
            var n = arr.Count;
            var o = new List<T>(n);
            for (int i = 0; i < n; i++)
            {
                o.Add(arr[n - i - 1]);
            }
            return o;
        }

        static public void NewObjectArray<T>(ref T[] arr, int size) where T : new()
        {
            arr = NewObjectArray<T>(size);
        }

        static public T[] NewObjectArray<T>(int size) where T : new()
        {
            var o = new T[size];
            for (int i = 0; i < size; i++)
            {
                o[i] = new T();
            }
            return o;
        }

        static public T[,] NewObjectArray<T>(int width, int height) where T : new()
        {
            var o = new T[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    o[x, y] = new T();
                }
            }
            return o;
        }

        static public void SetArrayElement<T>(T[,] arr, T value)
        {
            for (int x = 0; x < arr.GetLength(0); ++x)
                for (int y = 0; y < arr.GetLength(1); ++y)
                    arr[x, y] = value;
        }

        static public T SafeGetElement<T>(T[] arr, int element)
        {
            if (arr == null) return default(T);
            if (element < 0 || element >= arr.Length) return default(T);
            return arr[element];
        }

        static public T SafeGetElement<T>(List<T> arr, int element)
        {
            if (arr == null) return default(T);
            if (element < 0 || element >= arr.Count) return default(T);
            return arr[element];
        }

        static public T SafeGetLastElement<T>(T[] arr, int element)
        {
            if (arr == null) return default(T);
            if (element < 0 || element >= arr.Length) return default(T);
            return arr[arr.Length - 1 - element];
        }

        static public T SafeGetLastElement<T>(List<T> arr, int element)
        {
            if (arr == null) return default(T);
            if (element < 0 || element >= arr.Count) return default(T);
            return arr[arr.Count - 1 - element];
        }

        static public void RemoveElementBySwapLast<T>(List<T> arr, int element)
        {
            if (arr == null) return;
            if (element < 0 || element >= arr.Count) return;
            var last = arr.Count - 1;

            if (element < last)
            {
                arr[element] = arr[last];
            }
            arr.RemoveAt(last);
        }

        static public void RemoveLastElement<T>(List<T> arr)
        {
            if (arr == null) return;
            if (arr.Count <= 0) return;
            arr.RemoveAt(arr.Count - 1);
        }

        static public int GetElementIdxByList<T>(T[] arr, T element)
        {
            if (arr == null) return -1;
            if (arr.Length <= 0) return -2;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(element))
                {
                    return i;
                }
            }

            return -4;
        }

        static public int GetElementIdxByList<T>(List<T> arr, T element)
        {
            if (arr == null) return -1;
            if (arr.Count <= 0) return -2;
            if (!arr.Contains(element)) return -3;

            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i].Equals(element))
                {
                    return i;
                }
            }

            return -4;
        }

        static public bool IsNullOfEmpty<T>(T[] arr)
        {
            return (arr == null || arr.Length == 0);
        }

        static public bool IsNullOfEmpty<T>(List<T> arr)
        {
            return (arr == null || arr.Count == 0);
        }

        static public void AddPtr(ref System.IntPtr p, int offset)
        {
            p = new System.IntPtr(p.ToInt64() + offset);
        }

        static public T TryLoadResource<T>(string path) where T : UnityEngine.Object
        {
            //Debug.Log("Load resource " + typeof(T).Name + " \"" + path + "\"");
            return Resources.Load(path, typeof(T)) as T;
        }

        static public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            //Debug.Log("Load resource " + typeof(T).Name + " \"" + path + "\"");
            var o = TryLoadResource<T>(path);
            if (o == null)
            {
                Debug.LogWarning("Cannot load resource " + typeof(T).Name + " \"" + path + "\"");
            }
            return o;
        }

        static public GameObject LoadGameObject(string path, bool objectUsingPrefabName = false)
        {
            var prefab = LoadResource<GameObject>(path);
            if (!prefab) return null;
            var obj = GameObject.Instantiate(prefab) as GameObject;
            if (objectUsingPrefabName && obj)
            {
                obj.name = prefab.name;
            }
            return obj;
        }

        static public GameObject TryLoadGameObject(string path)
        {
            var prefab = TryLoadResource<GameObject>(path);
            if (!prefab) return null;
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = go.name.Replace("(Clone)", "");
            return go;
        }

        static public GameObject LoadPrefab(string path) { return LoadResource<GameObject>(path); }
        static public Texture LoadTexture(string path) { return LoadResource<Texture>(path); }
        static public Texture2D LoadTexture2D(string path) { return LoadResource<Texture2D>(path); }
        static public Material LoadMaterial(string path) { return LoadResource<Material>(path); }
        static public AnimationClip LoadAnimClip(string path) { return LoadResource<AnimationClip>(path); }

        static public GameObject TryLoadPrefab(string path) { return TryLoadResource<GameObject>(path); }
        static public Texture TryLoadTexture(string path) { return TryLoadResource<Texture>(path); }
        static public Texture2D TryLoadTexture2D(string path) { return TryLoadResource<Texture2D>(path); }
        static public Material TryLoadMaterial(string path) { return TryLoadResource<Material>(path); }
        static public AnimationClip TryLoadAnimClip(string path) { return TryLoadResource<AnimationClip>(path); }

        static public void DestroyAllChildren(GameObject o) { DestroyAllChildren(o.transform); }
        static public void DestroyAllChildren(Transform t)
        {
            if (t.childCount == 0) return;
            var c = new Transform[t.childCount];

            int i = 0;
            foreach (Transform ct in t)
            {
                c[i] = ct;
                i++;
            }

            foreach (var p in c)
            {
                GK.Destroy(p.gameObject);
            }
        }
        static public void DestroyAllChildren2D(RectTransform t)
        {
            if (t.childCount == 0) return;
            var c = new RectTransform[t.childCount];

            int i = 0;
            foreach (RectTransform ct in t)
            {
                c[i] = ct;
                i++;
            }

            foreach (var p in c)
            {
                GK.Destroy(p.gameObject);
            }
        }

        static public bool HasBits<T>(T val, T bits) where T : struct
        {
            int v = (int)(object)val;
            int b = (int)(object)bits;
            return (v & b) == b;
        }

        static public void Swap<T>(ref T lhs, ref T rhs) { T temp = lhs; lhs = rhs; rhs = temp; }

        static public string Fullname(GameObject o)
        {
            if (o == null) return "null";
            if (o.transform.parent)
            {
                return Fullname(o.transform.parent.gameObject) + "/" + o.name;
            }
            else
            {
                return o.name;
            }
        }

        static public string Fullname(Component o)
        {
            if (o == null) return "null";
            return Fullname(o.gameObject) + "." + o.GetType().Name;
        }

        static public void SetPos(Transform t, Vector3 p)
        {
            if (t == null) return;
            if (t.position == p) return;    //avoid dirty transformation if nothing changed
            t.position = p;
        }

        static public void SetLocalPos(Transform t, Vector3 p)
        {
            if (t == null) return;
            if (t.localPosition == p) return;   //avoid dirty transformation if nothing changed
            t.localPosition = p;
        }

        static public void SetParent(GameObject t, GameObject parent, bool keepWorldPos)
        {
            if (!t) return;
            SetParent(t.transform, parent ? parent.transform : null, keepWorldPos);
        }

        static public void SetParent(Transform t, Transform parent, bool keepWorldPos)
        {
            if (!t) return;
            if (keepWorldPos)
            {
                t.transform.parent = parent;
            }
            else
            {
                var pos = t.localPosition;
                var rot = t.localRotation;
                var s = t.localScale;

                t.transform.SetParent(parent);

                t.localPosition = pos;
                t.localRotation = rot;
                t.localScale = s;
            }
        }

        static public GameObject FindChild(GameObject root, string name, bool ignoreCase = true, bool recursive = true)
        {
            if (!root) return null;
            foreach (Transform t in root.transform)
            {
                if (t.name.Equals(name, ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal))
                {
                    return t.gameObject;
                }

                if (recursive)
                {
                    var o = FindChild(t.gameObject, name, ignoreCase, recursive);
                    if (o) return o;
                }
            }
            return null;
        }

        static public GameObject[] FindAllChild(GameObject root, string name, bool ignoreCase = true, bool recursive = true)
        {
            var outList = new List<GameObject>();
            _FindAllChildWithPrefix(ref outList, root, name, ignoreCase, recursive);
            return outList.ToArray();
        }

        static void _FindAllChild(ref List<GameObject> outList, GameObject root, string name, bool ignoreCase, bool recursive)
        {
            if (!root) return;
            foreach (Transform t in root.transform)
            {
                if (t.name.Equals(name, ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal))
                {
                    outList.Add(t.gameObject);
                }

                if (recursive)
                {
                    _FindAllChild(ref outList, t.gameObject, name, ignoreCase, recursive);
                }
            }
        }

        static public void FindAllChild<T>(ref List<T> outList, GameObject root, bool recursive = true)
        {
            if (!root) return;
            foreach (Transform t in root.transform)
            {
                T target = t.gameObject.GetComponent<T>();
                if (null != target)
                {
                    outList.Add(target);
                }
                if (recursive)
                {
                    FindAllChild(ref outList, t.gameObject);
                }
            }
        }

        static public GameObject FindChildWithPrefix(GameObject root, string name, bool ignoreCase = true, bool recursive = true)
        {
            if (!root) return null;
            foreach (Transform t in root.transform)
            {
                if (t.name.StartsWith(name, ignoreCase, null))
                {
                    return t.gameObject;
                }

                if (recursive)
                {
                    var o = FindChild(t.gameObject, name, ignoreCase, recursive);
                    if (o) return o;
                }
            }
            return null;
        }

        static public GameObject FindChildWithSuffix(GameObject root, string name, bool ignoreCase = true, bool recursive = true)
        {
            if (!root) return null;
            foreach (Transform t in root.transform)
            {
                if (t.name.EndsWith(name, ignoreCase, null))
                {
                    return t.gameObject;
                }

                if (recursive)
                {
                    var o = FindChild(t.gameObject, name, ignoreCase, recursive);
                    if (o) return o;
                }
            }
            return null;
        }

        static public GameObject[] FindAllChildWithPrefix(GameObject root, string prefix, bool ignoreCase = true, bool recursive = true)
        {
            var outList = new List<GameObject>();
            _FindAllChildWithPrefix(ref outList, root, prefix, ignoreCase, recursive);
            return outList.ToArray();
        }

        static void _FindAllChildWithPrefix(ref List<GameObject> outList, GameObject root, string prefix, bool ignoreCase, bool recursive)
        {
            if (!root) return;
            foreach (Transform t in root.transform)
            {
                if (t.name.StartsWith(prefix, ignoreCase, null))
                {
                    outList.Add(t.gameObject);
                }

                if (recursive)
                {
                    _FindAllChildWithPrefix(ref outList, t.gameObject, prefix, ignoreCase, recursive);
                }
            }
        }

        static public T FindChildOfType<T>(GameObject root, bool recursive = true) where T : Component
        {
            if (!root) return null;
            foreach (Transform t in root.transform)
            {
                T c = t.gameObject.GetComponent<T>();
                if (c) return c;

                if (recursive)
                {
                    c = FindChildOfType<T>(t.gameObject, recursive);
                    if (c) return c;
                }
            }
            return null;
        }

        static public void PrintArray2D<T>(T[] arr, int dim1, int dim2)
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            var fmt = "{0:X" + size * 2 + "} ";
            var msg = "";
            for (int y = 0; y < dim2; ++y)
            {
                for (int x = 0; x < dim1; ++x)
                {
                    msg += string.Format(fmt, arr[x + y * dim1]);
                }
                msg += '\n';
            }
            Debug.Log(msg);
        }

        static public void Assert(bool expr, params string[] msg)
        {
            if (!expr)
            {
                string finalMsg = "";
                foreach (string s in msg)
                    finalMsg += s;
                Debug.LogError("ASSERT FAILED! Msg: " + finalMsg);
            }
        }

        static public void ResetLocalTransform(GameObject o) { ResetLocalTransform(o.transform); }

        static public void ResetLocalTransform(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        static public int LayerBits(params string[] name)
        {
            int m = 0;
            foreach (var p in name)
            {
                m |= 1 << LayerMask.NameToLayer(p);
            }
            return m;
        }

        static public int LayerId(string name)
        {
            return LayerMask.NameToLayer(name);
        }

        static public void SetLayerRecursively(GameObject o, string layer)
        {
            SetLayerRecursively(o, LayerMask.NameToLayer(layer));
        }

        static public void SetLayerRecursively(GameObject o, int layer)
        {
            if (!o) return;
            o.layer = layer;
            foreach (Transform t in o.transform)
            {
                SetLayerRecursively(t.gameObject, layer);
            }
        }

        static public Dictionary<string, GameObject> IterateTransformHierarchy(GameObject o, Dictionary<string, GameObject> objDict, bool ignoreCase)
        {
            if (!o) return null;
            if (null == objDict)
                objDict = new Dictionary<string, GameObject>();
            objDict.Add(ignoreCase ? o.name.ToUpper() : o.name, o);
            foreach (Transform t in o.transform)
            {
                IterateTransformHierarchy(t.gameObject, objDict, ignoreCase);
            }
            return objDict;
        }

        static public T GetOrAddComponent<T>(GameObject o) where T : Component
        {
            var c = o.GetComponent<T>();
            if (c) return c;
            return o.AddComponent<T>();
        }

        static public GameObject GetOrAddGameObject(GameObject o, string name)
        {
            if (!o) return null;
            var f = o.transform.Find(name);
            if (f) return f.gameObject;

            var c = new GameObject(name);
            c.transform.parent = o.transform;
            GK.ResetLocalTransform(c);
            return c;
        }

        static public void Destroy(UnityEngine.Object obj)
        {
            if (!obj) return;
            if (Application.isEditor && !Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                // NGUI require remove parent instantly
                var go = obj as GameObject;
                if (go)
                {
                    // Parent of RectTransform is being set with parent property. 
                    // Consider using the SetParent method instead, with the worldPositionStays argument set to false. 
                    // This will retain local orientation and scale rather than world orientation and scale, 
                    // which can prevent common UI scaling issues.
                    // go.transform.parent = null;
                }
                UnityEngine.Object.Destroy(obj);
            }
        }

        static public int EnumCount<T>() { return System.Enum.GetNames(typeof(T)).Length; }
        static public string[] EnumNames<T>() { return System.Enum.GetNames(typeof(T)); }
        static public T[] EnumValues<T>()
        {
            var arr = System.Enum.GetValues(typeof(T));
            var v = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                v[i] = (T)arr.GetValue(i);
            }

            return v;
        }

        static public bool TryParseEnum<T>(out T o, string str, bool ignoreCase) where T : System.IConvertible, new()
        {
            try
            {
                o = (T)System.Enum.Parse(typeof(T), str, ignoreCase);
                return true;
            }
            catch
            {
                o = new T();
                return false;
            }
        }

        // Get Outer net ip, if use Network.player.ipAddress Function, ip is intranet ip.
        static public string GetOuterNetIP()
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.Default;
            // Is vaild url.
            string response = client.UploadString("http://www.3322.org/dyndns/getip", "");
            return response.Trim();
        }

        // Compare date time.
        static public int CompareDateTime(string time1, string time2)
        {
            DateTime t1 = Convert.ToDateTime(time1);
            DateTime t2 = Convert.ToDateTime(time2);

            return DateTime.Compare(t1, t2);
        }
        static public int CompareDateTime(DateTime time1, string time2)
        {
            DateTime t2 = Convert.ToDateTime(time2);

            //		Debug.Log (string.Format("{0} | {1}", time1, t2));
            return DateTime.Compare(time1, t2);
        }

        /*
         * Rectangle recognition in picture.
         * A two pixel buffer is created. One buffer is the picture pixel buffer, and the other is the read flag buffer.
         * The pixels that have not been operated in the picture buffer have been read. 
         * If the pixel color value is not the default color. the element recognition logic is carried out 
         * and the operation of all the involved pixels in the recognition process is marked.
         * return: x left-up x, y left-up y, z width, w height.
         * */
        static public List<Vector4> RectangleRecognition(Texture2D tex)
        {
            List<Vector4> l = new List<Vector4>();
            bool[,] opt = new bool[tex.height, tex.width];
            bool finished = false;

            while (!finished)
            {
                var v = _GetRectangleLTByColor(tex, ref opt);
                if (0.9 < v.z)
                {
                    int w = _GetRectangleWidthByColor(tex, (int)v.x, (int)v.y);
                    int h = _GetRectangleHeightByColor(tex, (int)v.x, (int)v.y);

                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            opt[(int)v.y + i, (int)v.x + j] = true;
                        }
                    }

                    Vector4 v4 = new Vector4(v.x, v.y, w, h);
                    l.Add(v4);
                }
                else
                {
                    finished = true;
                }
            }

            return l;
        }

        // Get the left upper corner of the color based rectangle in the picture.
        static private Vector3 _GetRectangleLTByColor(Texture2D tex, ref bool[,] array)
        {
            Vector3 v = new Vector3();

            for (int i = 0; i < tex.height; i++)
            {
                for (int j = 0; j < tex.width; j++)
                {
                    if (!array[i, j])
                    {
                        array[i, j] = true;
                        if (UnityEngine.Color.white != tex.GetPixel(j, i))
                        {
                            v.x = j;
                            v.y = i;
                            v.z = 1;
                            return v;
                        }
                    }
                }
            }

            v.z = 0;
            return v;
        }

        // Gets the color based rectangle width in the picture.
        static int _GetRectangleWidthByColor(Texture2D tex, int widthIdx, int heightIdx)
        {
            for (int i = widthIdx; i < tex.width; i++)
            {
                UnityEngine.Color c = tex.GetPixel(i, heightIdx);
                if (c == UnityEngine.Color.white)
                {
                    return i - widthIdx;
                }
            }
            return tex.width - widthIdx;
        }

        // Gets the color based rectangle height in the picture.
        static int _GetRectangleHeightByColor(Texture2D tex, int widthIdx, int heightIdx)
        {
            for (int i = heightIdx; i < tex.height; i++)
            {
                UnityEngine.Color c = tex.GetPixel(widthIdx, i);
                if (c == UnityEngine.Color.white)
                {
                    return i - heightIdx;
                }
            }
            return tex.height - heightIdx;
        }

        public delegate void DelegateType();

        // Default get texture method.
        static public Texture2D GetTextureByName(string name)
        {
            return LoadResource<Texture2D>(string.Format("Textures/{0}", name));
        }

        // Default get sprite method.
        static public Sprite GetSpriteByName(string name)
        {
            return LoadResource<Sprite>(string.Format("Textures/{0}", name));
        }

        static public  DateTime GetDateTimeFrom1970Ticks(long curSeconds)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.AddSeconds(curSeconds);
        }

        static public DateTime GetDateTime(long curSeconds)
        {
            return new DateTime(curSeconds);
        }

        static public bool ExistLocationInList(int x, int y, List<Vector2> list)
        {
            foreach (var l in list)
            {
                if ((int)l.x == x && (int)l.y == y)
                    return true;
            }
            return false;
        }

        static public int CalcPointDistance(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        static public void ShuffleByList<T>(ref List<T> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                int rand = UnityEngine.Random.Range(i + 1, list.Count);
                var temp = list[rand];
                list[rand] = list[i];
                list[i] = temp;
            }
        }

        /// <summary>
        /// Clones the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">The list.</param>
        /// <returns>List{``0}.</returns>
        static public List<T> CloneList<T>(object List)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, List);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as List<T>;
            }
        }

        static public void ReplaceShader(List<Renderer> renders, Shader replaceShader, string condition)
        {
            if (null == renders)
                return;
            for (int i = 0; i < renders.Count; i++)
            {
                Renderer r = renders[i];
                if (null == r)
                    continue;
                Material[] mats = r.materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    Material m = mats[j];
                    if (null != m && m.shader.name.StartsWith(condition))
                    {
                        m.shader = replaceShader;
                    }
                }
                r.materials = mats;
            }
        }

        static public void SetMatColor(List<Renderer> renders, UnityEngine.Color color, GameObject gameObject, float rimPower = 0.5f)
        {
            if (null != renders && !ReferenceEquals(gameObject, null) && gameObject.activeSelf)
            {
                for (int i = 0; i < renders.Count; i++)
                {
                    try
                    {
                        Renderer r = renders[i];
                        if (ReferenceEquals(r, null))
                            continue;
                        SetMatColor(r, color, rimPower);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        // 边缘色及边缘强度定义随Shader定义而不同. 但为保证复用率, 尽可能确保项目内统一.
        static readonly int _rimColorID = Shader.PropertyToID("_RimColor");
        static readonly int _rimPower = Shader.PropertyToID("_RimPower");
        static readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");
        static readonly int _dstBlend = Shader.PropertyToID("_DstBlend");
        static readonly int _renderQueue = 3100;
        const string RIMLIGHT = "RIMLIGHT";
        const string TRANSPARENT = "TRANSPARENT";
        const string PROJECTION_ON = "PROJECTION_ON";
        const string ALPHAOFFSET = "_AlphaOffset";
        static public void SetMatColor(Renderer render, UnityEngine.Color color, float rimPower = 0.5f)
        {
            if (null != render)
            {
                Material[] mats = render.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Material m = mats[i];
                    if (ReferenceEquals(m, null))
                        continue;
                    if (m.HasProperty(_rimColorID))
                    {
                        UnityEngine.Color c = m.GetColor(_rimColorID);
                        if (c != color)
                        {
                            m.SetColor(_rimColorID, color);
                            m.SetFloat(_rimPower, rimPower);
                            // 判断是否启用了着色器关键字.
                            if (rimPower > 0.5f)
                            {
                                if (!m.IsKeywordEnabled(RIMLIGHT))
                                {
                                    m.EnableKeyword(RIMLIGHT);
                                }
                            }
                            else
                            {
                                if (m.IsKeywordEnabled(RIMLIGHT))
                                {
                                    m.DisableKeyword(RIMLIGHT);
                                }
                            }
                        }
                    }
                }
                render.materials = mats;
            }
        }
        // 设置透明度.
        static Dictionary<int, int> _alphaRenderQueue = new Dictionary<int, int>();
        static public void SetMatAlpha(float alpha, List<Renderer> renders)
        {
            if (null == renders)
                return;
            for (int i = 0; i < renders.Count; i++)
            {
                Renderer r = renders[i];
                if (null == r)
                    continue;
                Material[] mats = r.materials;
                if (null == mats)
                    continue;
                for (int j = 0; j < mats.Length; j++)
                {
                    Material m = mats[j];
                    if (null == m)
                        continue;
                    if (alpha > 0f)
                    {
                        m.EnableKeyword(TRANSPARENT);
                        m.DisableKeyword(PROJECTION_ON);
                        // 修改渲染队列至Alpha级.
                        int id = m.GetInstanceID();
                        if (!_alphaRenderQueue.ContainsKey(id) && _renderQueue != m.renderQueue)
                            _alphaRenderQueue[id] = m.renderQueue;
                        m.renderQueue = _renderQueue;
                        m.SetInt(_dstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
                        m.SetFloat(ALPHAOFFSET, alpha);
                    }
                    else
                    {
                        m.DisableKeyword(TRANSPARENT);
                        m.EnableKeyword(PROJECTION_ON);
                        int id = m.GetInstanceID();
                        if (_alphaRenderQueue.ContainsKey(id))
                            m.renderQueue = _alphaRenderQueue[id];
                        m.SetInt(_dstBlend, 0);
                        m.SetFloat(ALPHAOFFSET, alpha);
                    }
                }
            }
            if (alpha <= 0f)
            {
                _alphaRenderQueue.Clear();
            }
        }

        static public void FindControls<T>(Component comp, ref T controls) where T : new()
        {
            FindControls(comp.gameObject, ref controls);
        }

        static public void FindControls<T>(GameObject obj, ref T controls) where T : new()
        {
            if (controls == null)
            {
                controls = new T();
            }

            var type = controls.GetType();
            var fields = type.GetFields();
            foreach (var f in fields)
            {
                var attrs = f.GetCustomAttributes(typeof(System.NonSerializedAttribute), false);
                if (attrs.Length != 0) continue;
                f.GetCustomAttributes(typeof(System.NonSerializedAttribute), false);
                var w = GK.FindChild(obj, f.Name, true);

                if (w == null)
                {
                    Debug.LogError("Cannot find widget [" + f.Name + "] in " + GK.Fullname(obj));
                    f.SetValue(controls, null);
                    continue;
                }

                if (f.FieldType == typeof(GameObject))
                {
                    f.SetValue(controls, w);

                }
                else if (f.FieldType.IsSubclassOf(typeof(Component)))
                {
                    var c = w.GetComponent(f.FieldType);
                    f.SetValue(controls, c);
                }
            }
        }

        static public void CrossFadeColorInChildren(GameObject root, float alpha, float duration, bool ignoreTimeScale)
        {
        }
		// 不能处于行首的符号.
		private static List<char> nonconvertibleWord = new List<char> { '，', '。', '“', '”', '…', '！', '？', '、', ',', '.', '"' };
		// 所使用字体英文字母占中文文字的宽度比例
		private const float letterPercent = 0.56f;
		/// <summary>
		/// 中文文字自动换行，含中英混排、符号处理
		/// </summary>
		/// <param name="word">要处理的文字</param>
		/// <param name="lineLength">一行的字数</param>
		/// <returns>处理后的行数</returns>
        static public int AutoLineFeed(string word, out string afterWord, int lineLength)
		{
			afterWord = word;
			int index = 0;
			int lineCount = 0;
			int breakCount = 0;
			float curLineWord = 0;
			while (index < word.Length)
			{
				if (word[index] == '\n')
				{
					++lineCount;
					++breakCount;
					curLineWord = 0;
				}
				else
				{
					curLineWord += (int)word[index] > 255 ? 1 : letterPercent;
					if (curLineWord >= lineLength && index + 1 < word.Length)
					{
						if (nonconvertibleWord.IndexOf(word[index + 1]) >= 0)
						{
							--index;
						}
						if (word[index + 1] != '\n')
						{
							afterWord = afterWord.Insert(index + lineCount - breakCount + 1, "\n");
							++lineCount;
							curLineWord = 0;
						}
					}
				}
				++index;
			}
			return lineCount;
		}

        // 计算折线节点.\
        static List<Vector2> _points = new List<Vector2>();
        static public List<Vector2> ClacLinePoint(Vector2 src, Vector2 dest, out bool vertical, int height, bool bSelected = false)
        {
            _points.Clear();
            vertical = true;

            if(src.x > dest.x && !bSelected)
            {
                // 6 points. 180.
                // | ————————— |
                // | —— d s —— |
                _points.Add(src);
                Vector2 point = new Vector2(src.x + 6, src.y);
                _points.Add(point);
                if(dest.y >=  src.y)
                {
                    point = new Vector2(src.x + 6, dest.y + height);
                    _points.Add(point);
                    point = new Vector2(dest.x - 6, dest.y + height);
                    _points.Add(point);
                }
                else
                {
                    point = new Vector2(src.x + 6, dest.y - height);
                    _points.Add(point);
                    point = new Vector2(dest.x - 6, dest.y - height);
                    _points.Add(point);
                }
                point = new Vector2(dest.x - 6, dest.y);
                _points.Add(point);
                _points.Add(dest);
            }
            else
            {
                // 4 points.
                //      | —— d
                // s —— |
                _points.Add(src);
                Vector2 point = new Vector2(src.x + 6, src.y);
                _points.Add(point);
                point = new Vector2(src.x + 6, dest.y);
                _points.Add(point);
                _points.Add(dest);
            }

            //float distanceX = Mathf.Abs(src.x - dest.x);
            //float distanceY = Mathf.Abs(src.y - dest.y);

            // 2 points.
            //if (distanceX < 8 || distanceY < 8)
            //{
            //    vertical = distanceX < distanceY;
            //    _points.Add(src);
            //    _points.Add(dest);
            //    return points;
            //}

            return _points;
        }

        static public void SaveTextureToFile(Texture2D texture, string imagePath)
		{
			byte[] bytes = texture.EncodeToPNG();
			FileStream file = new FileStream(imagePath, FileMode.Create);
			BinaryWriter binary = new BinaryWriter(file);
			binary.Write(bytes);
			file.Close();
		}

        static public string [] TypesToString(Type [] types)
        {
            if (null == types)
                return null;
            List<string> lst = new List<string>();
            foreach(var t in types)
            {
                lst.Add(t.ToString());
            }
            return lst.ToArray();
        }

        // 深拷贝.
        static public T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        /// <summary>
        /// 通过给定的文件路径，判断文件的编码类型
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="skipByte">读取跳过的字符数</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetEncoding(string fileName, out int skipByte)
        {
            skipByte = 0;
            Encoding reVal = Encoding.Default;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(4);
            if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                skipByte = 3;
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                skipByte = 3;
                reVal = Encoding.Unicode;
            }
            else
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    skipByte = 3;
                    reVal = Encoding.UTF8;
                }
                else
                {
                    int i;
                    int.TryParse(fs.Length.ToString(), out i);
                    ss = r.ReadBytes(i);

                    if (IsUTF8Bytes(ss))
                    {
                        reVal = Encoding.UTF8;
                    }
                }
            }
            r.Close();
            fs.Close();
            return reVal;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式!");
            }
            return true;
        }

        /// <summary>
        /// Gif转换图片. 由于System.Drawing.dll的部分功能只支持PC平台.
        /// 故如果需要使用此函数需要删除Unity中精简版dll， 并放置全平台dll
        /// 于Plugins中.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        static public List<Texture2D> GifToTextureByCS(Image image)
        {
            List<Texture2D> texture2D = null;
            if (null != image)
            {
                texture2D = new List<Texture2D>();
                //Debug.LogError(image.FrameDimensionsList.Length);
                //image.FrameDimensionsList.Length = 1;
                //根据指定的唯一标识创建一个提供获取图形框架维度信息的实例;
                FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
                //获取指定维度的帧数;
                int framCount = image.GetFrameCount(frameDimension);
                for (int i = 0; i < framCount; i++)
                {
                    //选择由维度和索引指定的帧;
                    image.SelectActiveFrame(frameDimension, i);
                    var framBitmap = new Bitmap(image.Width, image.Height);
                    //从指定的Image 创建新的Graphics,并在指定的位置使用原始物理大小绘制指定的 Image;
                    //将当前激活帧的图形绘制到framBitmap上;
                    System.Drawing.Graphics.FromImage(framBitmap).DrawImage(image, Point.Empty);
                    var frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height);
                    for (int x = 0; x < framBitmap.Width; x++)
                    {
                        for (int y = 0; y < framBitmap.Height; y++)
                        {
                            //获取当前帧图片像素的颜色信息;
                            System.Drawing.Color sourceColor = framBitmap.GetPixel(x, y);
                            //设置Texture2D上对应像素的颜色信息;
                            frameTexture2D.SetPixel(x, framBitmap.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                        }
                    }
                    frameTexture2D.Apply();
                    texture2D.Add(frameTexture2D);
                }
            }
            return texture2D;
        }
    }
}



