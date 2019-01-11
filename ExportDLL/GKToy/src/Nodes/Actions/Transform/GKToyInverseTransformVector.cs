using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界转本地-向量")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("将向量从世界坐标系转换到本地坐标系。")]
    [NodeTypeTree("Action/Transform/WorldToLocal-Vector", "English")]
    [NodeDescription("Transforms vector from world space to local space.", "English")]
    public class GKToyInverseTransformVector : GKToyNode
    {
        [SerializeField]
        GKToySharedVector3 _input = Vector3.zero;
        public GKToySharedVector3 Input
        {
            get { return _input; }
            set { _input = value; }
        }

        GKToySharedVector3 _output = Vector3.zero;
        Transform _transform;

        public GKToyInverseTransformVector(int _id) : base(_id) { }

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
                _output.SetValue(_transform.InverseTransformVector(Input.Value));
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}