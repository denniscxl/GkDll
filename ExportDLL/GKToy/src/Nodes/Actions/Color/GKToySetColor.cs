using UnityEngine;
using System.Linq;

namespace GKToy
{
    [NodeTypeTree("行为/颜色/设置颜色")]
	[NodeTypeTree("Action/Color/SetColor", "English")]
	[NodeIcon("Assets/Utilities/GKToy/Textures/Icon/Color.png")]
	[NodeDescription("设置物体的材质颜色。")]
	[NodeDescription("Set the color of object.", "English")]
	public class GKToySetColor : GKToyNode
    {
		[SerializeField]
		private Renderer _renderer;
		[SerializeField]
        private GKToySharedColor _color = new GKToySharedColor();
		public GKToySharedColor SetColor
		{
            get { return _color; }
			set { _color = value; }
		}

        public GKToySetColor(int _id) : base(_id) { }

        override public void Init(GKToyBaseOverlord ovelord)
		{
			base.Init(ovelord);
			_renderer = ovelord.gameObject.GetComponentInChildren<Renderer>();
            outputObject = Color.white;
		}

        override public int Update()
		{
            if (_bLock)
                return 0;

            base.Update();
            if (_renderer != null && SetColor != null)
			{
                _renderer.material.color = SetColor.Value;
                outputObject = SetColor;
			}
            NextAll();
			return 0;
		}
	}
}
