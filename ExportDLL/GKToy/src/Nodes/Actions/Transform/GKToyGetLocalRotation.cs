using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/本地旋转角度")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Rotation.png")]
    [NodeDescription("返回物体相对其父物体的旋转角度。")]
    [NodeTypeTree("Action/Transform/LocalRotation", "English")]
    [NodeDescription("Return the rotation of the object relative to the parent object's rotation.", "English")]
    public class GKToyGetLocalRotation : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetLocalRotation(int _id) : base(_id) { }

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
                _output.SetValue(_transform.localRotation.eulerAngles);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
