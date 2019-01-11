using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GKUI
{
    /// <summary>
    /// 动态无限列表
    /// @panhaijie
    /// 2016年3月22日 10:27:51
    /// </summary>
    public class DynamicInfinityListRenderer : MonoBehaviour
    {
        /// <summary>
        /// 单元格尺寸（宽，高）
        /// </summary>
        public Vector2 cellSize;
        /// <summary>
        /// 单元格间隙（水平，垂直）
        /// </summary>
        public Vector2 spacingSize;
        /// <summary>
        /// 列数
        /// </summary>
        public int columnCount;
        /// <summary>
        /// 单元格渲染器prefab
        /// </summary>
        public GameObject renderGO;
        /// <summary>
        /// 渲染格子数
        /// </summary>
        protected int _rendererCount;
        /// <summary>
        /// 父节点蒙版尺寸
        /// </summary>
        Vector2 _maskSize;
        /// <summary>
        /// 蒙版矩形
        /// </summary>
        Rect _rectMask;
        protected ScrollRect _scrollRect;
        /// <summary>
        /// 转换器
        /// </summary>
        protected RectTransform _rectTransformContainer;
        /// <summary>
        /// 渲染脚本集合
        /// </summary>
        protected List<DynamicInfinityItem> _listItems;
        /// <summary>
        /// 渲染格子字典
        /// </summary>
        Dictionary<int, DynamicRect> _dictDRect;
        /// <summary>
        /// 数据提供者
        /// </summary>
        protected IList _dataProviders;
        protected bool _hasInited = false;
        /// <summary>
        /// 初始化渲染脚本
        /// </summary>
        virtual public void InitRendererList(DynamicInfinityItem.OnSelect OnSelect, DynamicInfinityItem.OnUpdateData OnUpdate)
        {
            if (_hasInited) return;
            //转换器
            _rectTransformContainer = transform as RectTransform;
            //获得蒙版尺寸
            _maskSize = transform.parent.GetComponent<RectTransform>().rect.size;
            _scrollRect = transform.parent.GetComponent<ScrollRect>();
            //通过蒙版尺寸和格子尺寸计算需要的渲染器个数
            _rendererCount = columnCount * (Mathf.CeilToInt(_maskSize.y / _GetBlockSizeY()) + 1);
            _UpdateDynmicRects(_rendererCount);
            _listItems = new List<DynamicInfinityItem>();
            RectTransform rectTran = null;
            for (int i = 0; i < _rendererCount; ++i)
            {
                GameObject child = GameObject.Instantiate(renderGO);
                rectTran = child.GetComponent<RectTransform>();
                rectTran.SetParent(transform);
                rectTran.localRotation = Quaternion.identity;
                rectTran.localScale = Vector3.one;
                child.layer = gameObject.layer;
                DynamicInfinityItem dfItem = child.GetComponent<DynamicInfinityItem>();
                if (dfItem == null)
                    throw new Exception("Render must extend DynamicInfinityItem");
                _listItems.Add(dfItem);
                _listItems[i].DRect = _dictDRect[i];
                _listItems[i].OnSelectHandler = OnSelect;
                _listItems[i].OnUpdateDataHandler = OnUpdate;
                child.SetActive(false);
                _UpdateChildTransformPos(child, i);
            }
            _SetListRenderSize(_rendererCount);
            _hasInited = true;
        }

        /// <summary>
        /// 设置渲染列表的尺寸
        /// 不需要public
        /// </summary>
        /// <param name="count"></param>
        void _SetListRenderSize(int count)
        {
            _rectTransformContainer.sizeDelta = new Vector2(_rectTransformContainer.sizeDelta.x, Mathf.CeilToInt((count * 1.0f / columnCount)) * _GetBlockSizeY());
            _rectMask = new Rect(0, -_maskSize.y, _maskSize.x, _maskSize.y);
            _scrollRect.vertical = _rectTransformContainer.sizeDelta.y > _maskSize.y;
        }

        /// <summary>
        /// 更新各个渲染格子的位置
        /// </summary>
        /// <param name="child"></param>
        /// <param name="index"></param>
        void _UpdateChildTransformPos(GameObject child, int index)
        {
            int row = index / columnCount;
            int column = index % columnCount;
            Vector2 v2Pos = new Vector2();
            v2Pos.x = column * _GetBlockSizeX();
            v2Pos.y = -cellSize.y - row * _GetBlockSizeY();
            ((RectTransform)child.transform).anchoredPosition3D = Vector3.zero;
            ((RectTransform)child.transform).anchoredPosition = v2Pos;
        }

        /// <summary>
        /// 获得格子块尺寸
        /// </summary>
        /// <returns></returns>
        protected float _GetBlockSizeY() { return cellSize.y + spacingSize.y; }
        protected float _GetBlockSizeX() { return cellSize.x + spacingSize.x; }

        /// <summary>
        /// 更新动态渲染格
        /// 不需要public
        /// </summary>
        /// <param name="count"></param>
        void _UpdateDynmicRects(int count)
        {
            _dictDRect = new Dictionary<int, DynamicRect>();
            for (int i = 0; i < count; ++i)
            {
                int row = i / columnCount;
                int column = i % columnCount;
                DynamicRect dRect = new DynamicRect(column * _GetBlockSizeX(), -row * _GetBlockSizeY() - cellSize.y, cellSize.x, cellSize.y, i);
                _dictDRect[i] = dRect;
            }
        }

        /// <summary>
        /// 设置数据提供者
        /// </summary>
        /// <param name="datas"></param>
        public void SetDataProvider(IList datas)
        {
            _UpdateDynmicRects(datas.Count);
            _SetListRenderSize(datas.Count);
            _dataProviders = datas;
            _ClearAllListRenderDr();
        }

        /// <summary>
        /// 清理可复用渲染格
        /// 不需要public
        /// </summary>
        void _ClearAllListRenderDr()
        {
            if (_listItems != null)
            {
                int len = _listItems.Count;
                for (int i = 0; i < len; ++i)
                {
                    DynamicInfinityItem item = _listItems[i];
                    item.DRect = null;
                }
            }
        }

        /// <summary>
        /// 获得数据提供者
        /// </summary>
        /// <returns></returns>
        public IList GetDataProvider() { return _dataProviders; }

        /// <summary>
        /// 数据发生变化 供外部调用刷新列表
        /// </summary>
        [ContextMenu("RefreshDataProvider")]
        public void RefreshDataProvider()
        {
            if (_dataProviders == null)
                throw new Exception("dataProviders 为空！请先使用SetDataProvider ");
            _UpdateDynmicRects(_dataProviders.Count);
            _SetListRenderSize(_dataProviders.Count);
            _ClearAllListRenderDr();
        }

        #region 移动至数据
        /// <summary>
        /// 移动列表使之能定位到给定数据的位置上
        /// </summary>
        /// <param name="target"></param>
        virtual public void LocateRenderItemAtTarget(object target, float delay)
        {
            LocateRenderItemAtIndex(_dataProviders.IndexOf(target), delay);
        }
        virtual public void LocateRenderItemAtIndex(int index, float delay)
        {
            if (index < 0 || index > _dataProviders.Count - 1)
                throw new Exception("Locate Index Error " + index);
            index = Math.Min(index, _dataProviders.Count - _rendererCount + 2);
            index = Math.Max(0, index);
            Vector2 pos = _rectTransformContainer.anchoredPosition;
            int row = index / columnCount;
            Vector2 v2Pos = new Vector2(pos.x, row * _GetBlockSizeY());
            _coroutine = StartCoroutine(_TweenMoveToPos(pos, v2Pos, delay));
        }
        protected IEnumerator _TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
        {
            bool running = true;
            float passedTime = 0f;
            while (running)
            {
                yield return new WaitForEndOfFrame();
                passedTime += Time.deltaTime;
                Vector2 vCur;
                if (passedTime >= delay)
                {
                    vCur = v2Pos;
                    running = false;
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
                else
                {
                    vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
                }
                _rectTransformContainer.anchoredPosition = vCur;
            }

        }
        protected Coroutine _coroutine = null;
        #endregion

        Dictionary<int, DynamicRect> _inOverlaps = new Dictionary<int, DynamicRect>();
        protected void _UpdateRender()
        {
            _rectMask.y = -_maskSize.y - _rectTransformContainer.anchoredPosition.y;
            _inOverlaps.Clear();
            foreach (DynamicRect dR in _dictDRect.Values)
            {
                if (dR.Overlaps(_rectMask))
                {
                    _inOverlaps.Add(dR.Index, dR);
                }
            }
            int len = _listItems.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicInfinityItem item = _listItems[i];
                if (item.DRect != null && !_inOverlaps.ContainsKey(item.DRect.Index))
                    item.DRect = null;
            }
            foreach (DynamicRect dR in _inOverlaps.Values)
            {
                if (_GetDynmicItem(dR) == null)
                {
                    DynamicInfinityItem item = _GetNullDynmicItem();
                    item.DRect = dR;
                    _UpdateChildTransformPos(item.gameObject, dR.Index);

                    if (_dataProviders != null && dR.Index < _dataProviders.Count)
                    {
                        item.SetData(_dataProviders[dR.Index]);
                    }
                }
            }
        }

        /// <summary>
        /// 获得待渲染的渲染器
        /// </summary>
        /// <returns></returns>
        DynamicInfinityItem _GetNullDynmicItem()
        {
            int len = _listItems.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicInfinityItem item = _listItems[i];
                if (item.DRect == null)
                    return item;
            }
            throw new Exception("Error");
        }

        /// <summary>
        /// 通过动态格子获得动态渲染器
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        DynamicInfinityItem _GetDynmicItem(DynamicRect rect)
        {
            int len = _listItems.Count;
            for (int i = 0; i < len; ++i)
            {
                DynamicInfinityItem item = _listItems[i];
                if (item.DRect == null)
                    continue;
                if (rect.Index == item.DRect.Index)
                    return item;
            }
            return null;
        }

        void Update()
        {
            if (_hasInited)
                _UpdateRender();
        }
    }
}