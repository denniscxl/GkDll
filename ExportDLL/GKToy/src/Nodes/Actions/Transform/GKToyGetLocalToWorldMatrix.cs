using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/本地转世界矩阵")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Matrix.png")]
    [NodeDescription("返回把点从本地空间转换到世界空间的矩阵。")]
    [NodeTypeTree("Action/Transform/LocalToWorldMatrix", "English")]
    [NodeDescription("Return matrix that transforms a point from local space into world space.", "English")]
    public class GKToyGetLocalToWorldMatrix : GKToyNode
    {
        Transform _transform;
        GKToySharedMatrix4x4 _output = Matrix4x4.zero;
        public GKToyGetLocalToWorldMatrix(int _id) : base(_id) { }

        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            _output = new GKToySharedMatrix4x4();
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
                _output.SetValue(_transform.localToWorldMatrix);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
