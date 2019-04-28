using UnityEngine;
using UnityEditor;
using System.IO;

namespace GKFile
{
    /*
     * From 一只飞鸟的自白 - CSDN.NET
     */

    public class GKSeperateRGBAndAlpha
    {

        static string _path = string.Format("{0}/SeparateTextureFolder/", Application.dataPath);
        static string defaultWhiteTexPath_relative = string.Format("Assets/SeparateTextureFolder/Default/Default_Alpha.png");
        static Texture2D defaultWhiteTex = null;

        [MenuItem("GK/Seperate ETC1 RGB and Alpha Channel")]
        static void _SeperateAllTexturesRGBandAlphaChannel()
        {
            Debug.Log("Start Seperate ETC1.");
            if (!GetDefaultWhiteTexture())
            {
                return;
            }
            string[] paths = Directory.GetFiles(_path, "*.*", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path) && _IsTextureFile(path) && !_IsTextureConverted(path))   //full name    
                {
                    _SeperateRGBAandlphaChannel(path);
                }
            }
            AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
            Debug.Log("Finish Departing.");
        }

        #region process texture  

        static void _SeperateRGBAandlphaChannel(string _texPath)
        {
            string assetRelativePath = _GetRelativeAssetPath(_texPath);
            _SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor  
            Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
            if (!sourcetex)
            {
                Debug.LogError("Load Texture Failed : " + assetRelativePath);
                return;
            }

            TextureImporter ti = null;
            try
            {
                ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + assetRelativePath);
                return;
            }
            if (ti == null)
            {
                return;
            }
            bool bGenerateMipMap = ti.mipmapEnabled;    //same with the texture import setting        

            Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, bGenerateMipMap);
            rgbTex.SetPixels(sourcetex.GetPixels());

            Texture2D mipMapTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);  //Alpha Channel needed here  
            mipMapTex.SetPixels(sourcetex.GetPixels());
            mipMapTex.Apply();
            Color[] colors2rdLevel = mipMapTex.GetPixels(1);   //Second level of Mipmap  
            Debug.Log(string.Format("colors2rdLevel.Length: {0}, mipMapTex.width: {1}, mipMapTex.heigh: {2}", colors2rdLevel.Length, mipMapTex.width, mipMapTex.height));
            Color[] colorsAlpha = new Color[colors2rdLevel.Length];

            if (colors2rdLevel.Length != (mipMapTex.width) / 2 * (mipMapTex.height) / 2)
            {
                Debug.LogError(string.Format("Size error: mipMapTex.width * mipMapTex.heigh {0}", mipMapTex.width * mipMapTex.height));
                return;
            }
            for (int i = 0; i < colors2rdLevel.Length; ++i)
            {
                colorsAlpha[i].r = colors2rdLevel[i].a;
                colorsAlpha[i].g = colors2rdLevel[i].a;
                colorsAlpha[i].b = colors2rdLevel[i].a; 
            }
            Texture2D alphaTex = null; 
            alphaTex = new Texture2D((sourcetex.width) / 2, (sourcetex.height) / 2, TextureFormat.RGB24, bGenerateMipMap); 
            alphaTex.SetPixels(colorsAlpha);
            rgbTex.Apply();
            alphaTex.Apply();

            byte[] bytes = rgbTex.EncodeToPNG();
            string rgbTexRelativePath = _GetRGBTexPath(_texPath);
            File.WriteAllBytes(rgbTexRelativePath, bytes);
            byte[] alphabytes = alphaTex.EncodeToPNG();
            string alphaTexRelativePath = _GetAlphaTexPath(_texPath);
            File.WriteAllBytes(alphaTexRelativePath, alphabytes);

            _ReImportAsset(assetRelativePath, rgbTex.width, rgbTex.height);
            _ReImportAsset(alphaTexRelativePath, alphaTex.width, alphaTex.height);
            Debug.Log("Succeed Departing : " + assetRelativePath);
        }

        static void _ReImportAsset(string path, int width, int height)
        {
            try
            {
                AssetDatabase.ImportAsset(path);
            }
            catch
            {
                Debug.LogError("Import Texture failed: " + path);
                return;
            }

            TextureImporter importer = null;
            try
            {
                importer = (TextureImporter)TextureImporter.GetAtPath(path);
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + path);
                return;
            }
            if (importer == null)
            {
                return;
            }
            importer.maxTextureSize = Mathf.Max(width, height);
            importer.anisoLevel = 0;
#if UNITY_2017_1_OR_NEWER
            importer.textureType = TextureImporterType.Default;
#else
            importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
            importer.textureType = TextureImporterType.Advanced;
#endif
            if (path.Contains("/UI/"))
            {
                importer.textureType = TextureImporterType.GUI;
            }
            AssetDatabase.ImportAsset(path);
        }


        static void _SetTextureReadableEx(string _relativeAssetPath)    //set readable flag and set textureFormat TrueColor  
        {
            TextureImporter ti = null;
            try
            {
                ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + _relativeAssetPath);
                return;
            }
            if (ti == null)
            {
                return;
            }
            ti.isReadable = true;
            ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;      //this is essential for departing Textures for ETC1. No compression format for following operation.  
            AssetDatabase.ImportAsset(_relativeAssetPath);
        }

        static bool GetDefaultWhiteTexture()
        {
            defaultWhiteTex = AssetDatabase.LoadAssetAtPath(defaultWhiteTexPath_relative, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
            if (!defaultWhiteTex)
            {
                Debug.LogError("Load Texture Failed : " + defaultWhiteTexPath_relative);
                return false;
            }
            return true;
        }

#endregion

#region string or path helper    

        static bool _IsTextureFile(string _path)
        {
            string path = _path.ToLower();
            return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
        }

        static bool _IsTextureConverted(string _path)
        {
            return _path.Contains("_RGB.") || _path.Contains("_Alpha.");
        }

        static string _GetRGBTexPath(string _texPath)
        {
            return _GetTexPath(_texPath, "_RGB.");
        }

        static string _GetAlphaTexPath(string _texPath)
        {
            return _GetTexPath(_texPath, "_Alpha.");
        }

        static string _GetTexPath(string _texPath, string _texRole)
        {
            string dir = System.IO.Path.GetDirectoryName(_texPath);
            string filename = System.IO.Path.GetFileNameWithoutExtension(_texPath);
            string result = dir + "/" + filename + _texRole + "png";
            return result;
        }

        static string _GetRelativeAssetPath(string _fullPath)
        {
            _fullPath = _GetRightFormatPath(_fullPath);
            int idx = _fullPath.IndexOf("Assets");
            string assetRelativePath = _fullPath.Substring(idx);
            return assetRelativePath;
        }

        static string _GetRightFormatPath(string _path)
        {
            return _path.Replace("\\", "/");
        }

#endregion
    }
}