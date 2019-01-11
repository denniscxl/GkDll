using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GKUI
{
    /// <summary>
    /// 动态格子矩形
    /// </summary>
    public class DynamicRect
    {
        /// <summary>
        /// 矩形数据
        /// </summary>
        Rect _rect;
        /// <summary>
        /// 格子索引
        /// </summary>
        public int Index;
        public DynamicRect(float x, float y, float width, float height, int index)
        {
            this.Index = index;
            _rect = new Rect(x, y, width, height);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="otherRect"></param>
        /// <returns></returns>
        public bool Overlaps(DynamicRect otherRect)
        {
            return _rect.Overlaps(otherRect._rect);
        }

        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="otherRect"></param>
        /// <returns></returns>
        public bool Overlaps(Rect otherRect)
        {
            return _rect.Overlaps(otherRect);
        }
        public override string ToString()
        {
            return string.Format("index:{0},x:{1},y:{2},w:{3},h:{4}", Index, _rect.x, _rect.y, _rect.width, _rect.height);
        }


    }

    /// <summary>
    /// 动态无限渲染器
    /// @panhaijie
    /// </summary>
    public class DynamicInfinityItem : UIBase
    {
        public delegate void OnSelect(DynamicInfinityItem item);
        public delegate void OnUpdateData(DynamicInfinityItem item);
        public OnSelect OnSelectHandler;
        public OnUpdateData OnUpdateDataHandler;
        /// <summary>
        /// 动态矩形
        /// </summary>
        protected DynamicRect _dRect;
        /// <summary>
        /// 动态格子数据
        /// </summary>
        protected object _data;
        public DynamicRect DRect
        {
            set
            {
                _dRect = value;
                gameObject.SetActive(value != null);
            }
            get { return _dRect; }
        }

        void Start()
        {

        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        public void SetData(object data)
        {
            if (data == null)
            {
                return;
            }

            _data = data;
            if (null != OnUpdateDataHandler)
                OnUpdateDataHandler(this);
            OnRenderer();
        }

        /// <summary>
        /// 复写
        /// </summary>
        protected virtual void OnRenderer()
        {

        }
        /// <summary>
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        public object GetData()
        {
            return _data;
        }

        public T GetData<T>()
        {
            return (T)_data;
        }
    }
}