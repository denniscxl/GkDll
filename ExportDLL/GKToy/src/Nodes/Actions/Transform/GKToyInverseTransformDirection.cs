﻿using UnityEngine;

namespace GKToy
{
    [NodeTypeTree("行为/变换/世界转本地-方向")]
    [NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Conver.png")]
    [NodeDescription("将方向从世界坐标系转换到本地坐标系。")]
    [NodeTypeTree("Action/Transform/WorldToLocal-Direction", "English")]
    [NodeDescription("Transforms direction from world space to local space.", "English")]
    public class GKToyInverseTransformDirection : GKToyNode
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

        public GKToyInverseTransformDirection(int _id) : base(_id) { }

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
                _output.SetValue(_transform.InverseTransformDirection(Input.Value));
                outputObject = _output;
            }
            NextAll();
            return 0;
        }
    }
}