using UnityEngine;

namespace GKBase
{
    public class GKSingleton<T> where T : class, new()
    {
        static T _instance = null;

        static public T Instance()
        {
            if (_instance == null)
            {
                _instance = new T();
                if (_instance == null)
                {
                    Debug.LogError("Failed to create the instance of " + typeof(T) + " as singleton!");
                }
            }

            return _instance;
        }

        static public void Release()
        {
            if (_instance != null)
            {
                _instance = null;
            }
        }
    }
}