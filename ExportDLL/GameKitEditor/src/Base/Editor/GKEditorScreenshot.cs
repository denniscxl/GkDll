using System;
using UnityEditor;
using UnityEngine;

namespace GKBase
{
	public class GKEditorScreenshot
	{
		public int frameCount;
		int _width;
		int _height;
		Rect sourceRect = new Rect();
		object[] parameters;

		public void SaveParams(int width, int height, Rect _sourceRect, params object[] _parameters)
		{
            Debug.Log(_sourceRect);
            if (RuntimePlatform.OSXEditor == Application.platform)
            {
                _width = width * 2;
                _height = height * 2;
                _sourceRect.x = _sourceRect.x * 2;
                _sourceRect.y = _sourceRect.y * 2;
                _sourceRect.width = _sourceRect.width * 2;
                _sourceRect.height = _sourceRect.height * 2;
            }
            else
            {
                _width = width;
                _height = height;
            }
            sourceRect = _sourceRect;
            parameters = _parameters;
            frameCount = 1;
        }

		public object[] TakeScreenshot()
		{

            Debug.Log(sourceRect);
            Texture2D texture = new Texture2D(_width, _height, TextureFormat.RGB24, false);
			texture.ReadPixels(sourceRect, 0, 0, false);
			GK.SaveTextureToFile(texture, String.Format("{0}/Screenshot-{1:yyMMddHHmmssfff}.png", Application.dataPath, DateTime.Now));
			return parameters;
		}

		public void RestoreState()
		{
			frameCount = 0;
			AssetDatabase.Refresh();
		}
	}
}
