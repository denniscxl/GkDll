using UnityEngine;
using GKBase;

namespace GKController
{
    public class GKGame : MonoBehaviour
    {

        #region Data
        #endregion

        #region PublicField
        public GameObject overlord;
        #endregion

        #region PrivateField
        #endregion

        #region PublicMethod
        public virtual void Init()
        {
            InitRoot();
        }

        public virtual void InitRoot()
        {
            overlord = new GameObject("Overlord");
            GK.SetParent(overlord, gameObject, false);
        }
        #endregion

        #region PrivateMethod

        #endregion
    }

    public class SingletonGame<T> : GKGame where T : GKGame
    {
        static protected T _instance = null;
        static public T Instance
        {
            get
            {
                if (null == _instance)
                {
                    var o = GameObject.Find("Game");
                    if (!o)
                    {
                        _instance = GK.GetOrAddComponent<T>(GK.TryLoadGameObject(string.Format("Prefabs/Manager/Game")));
                    }
                    else
                    {
                        _instance = GK.GetOrAddComponent<T>(o);
                    }
                    _instance.gameObject.name = "Game";
                }
                return _instance;
            }
        }
    }
}
