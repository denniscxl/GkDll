using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/本地坐标")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Move.png")]
    [NodeDescription("返回物体相对其父物体的坐标。")]
    [NodeTypeTree("Action/Transform/LocalPosition", "English")]
    [NodeDescription("Return position of the object relative to the parent object.", "English")]
    public class GKToyGetLocalPosition : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetLocalPosition(int _id) : base(_id) { }

        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedVector3();
            outputObject = _output;
            _transform = ovelord.gameObject.GetComponent<Transform>();
        }

        public override int Update()
        {
            if (_bLock)
                return 0;

            base.Update();
            if (null != _transform)
            {
                _output.SetValue(_transform.localPosition);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
