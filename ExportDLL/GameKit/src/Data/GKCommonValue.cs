using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GKMemory;
using System;

namespace GKData
{
    /// <summary>
    /// Common data structure.
    /// </summary>
    public class GKCommonValue
    {
        #region PublicField
        // Attribute purpose index. 
        public int index = 0;
        // Attribute type.
        public AttributeType type = AttributeType.Type_NoSet;
        static public GKObjectPool<GKCommonValue> commonValuePool = GKMemoryController.Instance().GetOrCreateObjectPool<GKCommonValue>(-1, true);
        // 属性变更回调.
        public delegate void OnAttributChanged(object obj, GKCommonValue attr);
        public event OnAttributChanged OnAttrbutChangedEvent = null;
        #endregion

        #region PrivateField
        // Current value.
        protected long _longValue = 0;
        protected float _floatValue = 0f;
        protected string _stringValue = null;
        protected byte[] _bufferValue = null;
        // last value.
        protected GKCommonValue _lastValue = null;
        #endregion

        #region PublicMethod
        // Get Current valuw.
        public int ValInt
        {
            get
            {
                return (int)_longValue;
            }
        }
        public long ValLong
        {
            get
            {
                return _longValue;
            }
        }
        public float ValFloat
        {
            get
            {
                return _floatValue;
            }
        }
        public string ValString
        {
            get
            {
                return (null == _stringValue) ? string.Empty : _stringValue;
            }
        }
        public byte[] ValBuffer
        {
            get
            {
                return _bufferValue;
            }
        }
        // Get last value.
        public int LastValInt
        {
            get
            {
                return (null == _lastValue) ? 0 : _lastValue.ValInt;
            }
        }
        public long LastValLong
        {
            get
            {
                return (null == _lastValue) ? 0 : _lastValue.ValLong;
            }
        }
        public float LastValFloat
        {
            get
            {
                return (null == _lastValue) ? 0f : _lastValue.ValFloat;
            }
        }
        public string LastValString
        {
            get
            {
                return (null == _lastValue) ? string.Empty : _lastValue.ValString;
            }
        }
        public byte[] LastValBuffer
        {
            get
            {
                return (null == _lastValue) ? null : _lastValue.ValBuffer;
            }
        }
        // 以下代码部分代码重复性高, 因为代用频率高. 为了性能， 牺牲部分美观.
        public void SetValue(int newValue)
        {
            type = AttributeType.Type_Int32;
            if(null != OnAttrbutChangedEvent)
            {
                if(null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._longValue = _longValue;
            }
            _longValue = newValue;
        }
        public void SetValue(long newValue)
        {
            type = AttributeType.Type_Int64;
            if (null != OnAttrbutChangedEvent)
            {
                if (null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._longValue = _longValue;
            }
            _longValue = newValue;
        }
        public void SetValue(float newValue)
        {
            type = AttributeType.Type_Float;
            if (null != OnAttrbutChangedEvent)
            {
                if (null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._floatValue = _floatValue;
            }
            _floatValue = newValue;
        }
        public void SetValue(string newValue)
        {
            type = AttributeType.Type_String;
            if (null != OnAttrbutChangedEvent)
            {
                if (null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._stringValue = _stringValue;
            }
            _stringValue = newValue;
        }
        public void SetValue(byte[] newValue)
        {
            type = AttributeType.Type_Blob;
            if (null != OnAttrbutChangedEvent)
            {
                if (null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._bufferValue = _bufferValue;
            }
            _bufferValue = newValue;
        }
        public void Clear()
        {
            ClearValueWithOutEvent();
            OnAttrbutChangedEvent = null;
            type = AttributeType.Type_NoSet;
        }
        // 只清除数据, 不清除事件. 调用频繁, 故使用新函数而不拓展参数.
        public void ClearValueWithOutEvent()
        {
            _longValue = 0;
            _floatValue = 0f;
            _stringValue = null;
            _bufferValue = null;
            if (null != _lastValue)
            {
                _lastValue.Clear();
                commonValuePool.Recycle(_lastValue);
                _lastValue = null;
            }
        }
        public void CopyVale(GKCommonValue src, bool bDoEvent = false)
        {
            if(null != OnAttrbutChangedEvent)
            {
                if(null == _lastValue)
                {
                    _lastValue = commonValuePool.Spawn(true);
                }
                _lastValue._longValue = _longValue;
                _lastValue._floatValue = _floatValue;
                _lastValue._stringValue = _stringValue;
                _lastValue._bufferValue = _bufferValue;
            }
            if(null != src)
            {
                _longValue = src._longValue;
                _floatValue = src._floatValue;
                _stringValue = src._stringValue;
                _bufferValue = src._bufferValue;
                type = src.type;
            }
        }
        public void DoEvent(object obj)
        {
            if(null != OnAttrbutChangedEvent)
            {
                try{
                    OnAttrbutChangedEvent(obj, this);
                }
                catch(System.Exception)
                {
                    
                }
            }
        }
        public bool HasEvent()
        {
            return (null != OnAttrbutChangedEvent);
        }
        public void ClearEvent()
        {
            OnAttrbutChangedEvent = null;
        }
        public string GetEventTarget()
        {
            if(null != OnAttrbutChangedEvent)
            {
                Delegate[] eventList = OnAttrbutChangedEvent.GetInvocationList();
                if(null != eventList)
                {
                    string result = string.Empty;
                    for (int i = 0, iCount = eventList.Length; i < iCount; i++)
                    {
                        Delegate oneEvent = eventList[i];
                        result += oneEvent.Target + ":" + oneEvent.Method + "\r\n";
                    }
                    return result;
                }
            }
            return string.Empty;
        }

        #endregion

        #region PrivateMethod

        #endregion
    }
}
