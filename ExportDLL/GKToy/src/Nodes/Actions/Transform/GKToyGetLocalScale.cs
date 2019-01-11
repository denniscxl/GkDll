using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/本地缩放")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Scale.png")]
    [NodeDescription("返回物体相对其父物体的缩放。")]
    [NodeTypeTree("Action/Transform/LocalScale", "English")]
    [NodeDescription("Get the scale of the object relative to its parent.", "English")]
    public class GKToyGetLocalScale : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetLocalScale(int _id) : base(_id) { }

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
                _output.SetValue(_transform.localScale);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
