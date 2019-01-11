using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界旋转角度")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Rotation.png")]
    [NodeDescription("返回物体的世界旋转角度。")]
    [NodeTypeTree("Action/Transform/WorldRotation", "English")]
    [NodeDescription("Return the rotation of the object in world space.", "English")]
    public class GKToyGetWorldRotation : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetWorldRotation(int _id) : base(_id) { }

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
                _output.SetValue(_transform.rotation.eulerAngles);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
