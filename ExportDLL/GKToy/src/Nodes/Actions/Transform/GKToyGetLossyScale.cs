using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界缩放")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Scale.png")]
    [NodeDescription("返回物体的世界缩放。")]
    [NodeTypeTree("Action/Transform/LossyScale", "English")]
    [NodeDescription("Return the global scale of the object.", "English")]
    public class GKToyGetLossyScale : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetLossyScale(int _id) : base(_id) { }

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
                _output.SetValue(_transform.lossyScale);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
