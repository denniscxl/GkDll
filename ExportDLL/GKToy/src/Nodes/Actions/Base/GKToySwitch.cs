using System.Collections.Generic;
using System.Linq;
using GKStateMachine;
using UnityEngine;

namespace GKToy
{
	[NodeTypeTree("行为/基本/整型开关")]
	[NodeTypeTree("Action/Base/Switch", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Switch.png")]
	[NodeDescription("根据输入整型, 选择对应链接. \n如果超过链接数, 默认选择链接0.")]
    [NodeDescription("Select corresponding links by int. \nDefault link 0.", "English")]
	public class GKToySwitch : GKToyNode
	{
		[SerializeField]
        GKToySharedInt _index = 0;
        public GKToySharedInt Index
        {
            get { return _index; }
			set 
            {
                _index = value;
                outputObject = value;
            }
        }

        // 链表存储链接关系, 依据Index来链接指定链接.
        [SerializeField]
        GKToySharedIntLst _indexLst = new GKToySharedIntLst();
        public GKToySharedIntLst IndexLst
        {
            get { return _indexLst; }
            set { _indexLst = value; }
        }

        public GKToySwitch(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
            outputObject = Index;
        }

        override public int Update()
		{
            if (_bLock)
                return 0;
            
            base.Update();

            int idx = IndexLst.Value[Index.Value];
            if(idx >= 0 && idx < IndexLst.GetValueCount())
            {
                Next(idx);
            }

			return 0;
		}
	}
}
