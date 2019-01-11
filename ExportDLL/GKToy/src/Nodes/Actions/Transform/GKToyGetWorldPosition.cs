using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界坐标")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Move.png")]
    [NodeDescription("返回物体的世界坐标。")]
    [NodeTypeTree("Action/Transform/WorldPosition", "English")]
    [NodeDescription("Return the world space position of the object.", "English")]
    public class GKToyGetWorldPosition : GKToyNode
    {
        Transform _transform;
        GKToySharedVector3 _output = Vector3.zero;
        public GKToyGetWorldPosition(int _id) : base(_id) { }

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
                _output.SetValue(_transform.position);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
