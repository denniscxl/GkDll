using UnityEngine;
using UnityEngine.EventSystems;
using GKBase;

namespace GKUI
{
    public class GKUIEventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public delegate void DragDelegate(PointerEventData eventData);
        public VoidDelegate OnClick;
        public DragDelegate OnDrag;
        public VoidDelegate OnDown;
        public VoidDelegate OnEnter;
        public VoidDelegate OnExit;
        public VoidDelegate OnUp;
        public VoidDelegate OnSelectd;
        public VoidDelegate OnUpdateSelect;

        static public GKUIEventTriggerListener Get(GameObject go)
        {
            GKUIEventTriggerListener listener = GK.GetOrAddComponent<GKUIEventTriggerListener>(go);
            return listener;
        }
        override public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null) OnClick(gameObject);
        }
        override public void OnPointerDown(PointerEventData eventData)
        {
            if (OnDown != null) OnDown(gameObject);
        }
        override public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnEnter != null) OnEnter(gameObject);
        }
        override public void OnPointerExit(PointerEventData eventData)
        {
            if (OnExit != null) OnExit(gameObject);
        }
        override public void OnPointerUp(PointerEventData eventData)
        {
            if (OnUp != null) OnUp(gameObject);
        }
        override public void OnSelect(BaseEventData eventData)
        {
            if (OnSelectd != null) OnSelectd (gameObject);
        }
        override public void OnUpdateSelected(BaseEventData eventData)
        {
            if (OnUpdateSelect != null) OnUpdateSelect(gameObject);
        }
    }
}