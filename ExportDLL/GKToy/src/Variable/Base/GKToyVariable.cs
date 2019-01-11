using UnityEngine;

namespace GKToy
{
    [System.Serializable]
    public abstract class GKToyVariable
    {
        
        [SerializeField]
        bool _isGlobal;
        public bool IsGlobal
        {
            get { return _isGlobal;}
            set { _isGlobal = value; }
        }

        [SerializeField]
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [SerializeField]
        string _propertyMapping;
        public string PropertyMapping
        {
            get { return _propertyMapping; }
            set { _propertyMapping = value; }
        }

        GKToyBaseOverlord _propertyMappingOwner;
        public GKToyBaseOverlord PropertyMappingOwner
        {
            get { return _propertyMappingOwner; }
            set { _propertyMappingOwner = value; }
        }

        GKToyData _propertyDataOwner;
        public GKToyData PropertyDataOwner
        {
            get { return _propertyDataOwner; }
            set { _propertyDataOwner  = value; }
        }

        [SerializeField]
        bool _isLst;
        public bool IsLst
        {
            get { return _isLst; }
            set { _isLst = value; }
        }

        public void ValueChanged()
        {
            if(null != PropertyMappingOwner && null != PropertyDataOwner)
            {
                PropertyDataOwner.variableChanged = true;
            }
        }

        public virtual void InitializePropertyMapping(GKToyBaseOverlord overlord, GKToyData data)
        {
            PropertyMappingOwner = overlord;
            PropertyDataOwner = data;
            PropertyMapping = GetType().ToString();
        }

        abstract public object GetValue();
        abstract public  void SetValue(object value);

        // 获取对应索引元素, 适用于List中获取对应元素.
        virtual public object GetValue(int idx)
        {
            return null;
        }
        // 设置对应索引元素, 适用于List中设置对应元素.
        virtual public void SetValue(int idx, object obj){}
        // 删除对应索引元素, 适用于List中删除对应元素.
        virtual public void RemoveAt(int idx) { }
        // 扩展元素容量. 适用于List扩展新元素 。
        virtual public void AddCapacity() { }
        // 获取元素数, 适用于List。
        virtual public int GetValueCount()
        {
            return 0;
        }
    }
}
