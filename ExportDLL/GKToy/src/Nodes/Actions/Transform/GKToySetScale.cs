using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/设置缩放")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Scale.png")]
    [NodeDescription("设置物体的本地缩放。")]
    [NodeTypeTree("Action/Transform/SetLocalScale", "English")]
    [NodeDescription("Set the local scale of a object.", "English")]
    public class GKToySetScale : GKToyNode
    {
        [SerializeField]
        GKToySharedVector3 _scale = new GKToySharedVector3();
        public GKToySharedVector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        GKToySharedVector3 _output = Vector3.zero;
        Transform _transform;

        public GKToySetScale(int _id) : base(_id) { }

        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = Scale;
            _transform = ovelord.gameObject.GetComponent<Transform>();
        }

        public override int Update()
        {
            if (_bLock)
                return 0;

            base.Update();
            if (null != _transform)
            {
                _transform.localScale = Scale.Value;
                outputObject = Scale;
            }
            NextAll();
            return 0;
        }
    }
}
