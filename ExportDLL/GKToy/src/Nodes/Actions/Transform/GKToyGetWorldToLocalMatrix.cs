using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界转本地矩阵")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Matrix.png")]
    [NodeDescription("返回把点从世界空间转换到本地空间的矩阵。")]
    [NodeTypeTree("Action/Transform/WorldToLocalMatrix", "English")]
    [NodeDescription("Return matrix that transforms a point from world space into local space.", "English")]
    public class GKToyGetWorldToLocalMatrix : GKToyNode
    {
        Transform _transform;
        GKToySharedMatrix4x4 _output = Matrix4x4.zero;
        public GKToyGetWorldToLocalMatrix(int _id) : base(_id) { }

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
                _output.SetValue(_transform.worldToLocalMatrix);
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}
