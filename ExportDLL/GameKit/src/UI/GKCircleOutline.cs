using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GKUI
{
    public class GKCircleOutline : GKModifiedShadow
    {
        [SerializeField]
        int _circleCount = 2;
        [SerializeField]
        int _firstSample = 4;
        [SerializeField]
        int _sampleIncrement = 2;

#if UNITY_EDITOR
        override protected void OnValidate()
        {
            base.OnValidate();
            CircleCount = _circleCount;
            FirstSample = _firstSample;
            SampleIncrement = _sampleIncrement;
        }
#endif

        public int CircleCount
        {
            get
            {
                return _circleCount;
            }

            set
            {
                _circleCount = Mathf.Max(value, 1);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public int FirstSample
        {
            get
            {
                return _firstSample;
            }

            set
            {
                _firstSample = Mathf.Max(value, 2);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        public int SampleIncrement
        {
            get
            {
                return _sampleIncrement;
            }

            set
            {
                _sampleIncrement = Mathf.Max(value, 1);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        override public void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
                return;

            var total = (_firstSample * 2 + _sampleIncrement * (_circleCount - 1)) * _circleCount / 2;
            var neededCapacity = verts.Count * (total + 1);
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;
            var original = verts.Count;
            var count = 0;
            var sampleCount = _firstSample;
            var dx = effectDistance.x / _circleCount;
            var dy = effectDistance.y / _circleCount;
            for (int i = 1; i <= _circleCount; i++)
            {
                var rx = dx * i;
                var ry = dy * i;
                var radStep = 2 * Mathf.PI / sampleCount;
                var rad = (i % 2) * radStep * 0.5f;
                for (int j = 0; j < sampleCount; j++)
                {
                    var next = count + original;
                    ApplyShadow(verts, effectColor, count, next, rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
                    count = next;
                    rad += radStep;
                }
                sampleCount += _sampleIncrement;
            }
        }
    }
}