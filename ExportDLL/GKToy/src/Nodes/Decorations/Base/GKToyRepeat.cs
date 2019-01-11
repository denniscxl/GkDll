using System.Collections;
using System.Linq;

namespace GKToy
{
	[NodeTypeTree("辅助/循环")]
    [NodeTypeTree("Decoration/Repeater", "English")]
	[NodeDescription("用于实现单个节点的循环.")]
	[NodeDescription("Repeat single node.", "English")]
	public class GKToyRepeat : GKToyNode
	{
        public GKToyRepeat(int _id) : base(_id) { }

        public override void Init(GKToyBaseOverlord ovelord)
        {
            base.Init(ovelord);
        }

		public override int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            NextAll();
			return 0;
		}
	}
}
