﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/******************************************
 * 
 * Class with a collection of image processing utilities
 * 
 * @author Esteban Gallardo
 */
public class ImageUtils
{
    // -------------------------------------------
    /* 
	 * LoadImage
	 */
    public static void LoadImage(string _pathFile, Image _image, int _height, int _maximumHeightAllowed)
    {
        if (System.IO.File.Exists(_pathFile))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(_pathFile);
            Texture2D textureOriginal = new Texture2D(1, 1);
            textureOriginal.LoadImage(bytes);
            TransformTexture(textureOriginal, _image, _height, _pathFile, _maximumHeightAllowed);
        }
    }

    // -------------------------------------------
    /* 
	 * TransformTexture
	 */
    public static void TransformTexture(Texture2D _textureOriginal, Image _image, int _height, string _pathFile, int _maximumHeightAllowed)
    {
        if ((_textureOriginal.width > 100) && (_textureOriginal.height > 100))
        {
            float factorScale = ((float)_maximumHeightAllowed / (float)_textureOriginal.height);
            Texture2D textureScaled = ImageUtils.ScaleTexture(_textureOriginal, (int)(_textureOriginal.width * factorScale), (int)_maximumHeightAllowed);
            _image.overrideSprite = ToSprite(textureScaled);
            float finalWidth = textureScaled.width * ((float)_height / (float)textureScaled.height);
            _image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, _height);
        }
        BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_CONFIRMATION_FROM_SYSTEM, _pathFile, _textureOriginal.width, _textureOriginal.height);
    }

    // -------------------------------------------
    /* 
	 * LoadTexture2D
	 */
    public static Texture2D LoadTexture2D(string _pathFile, int _height)
    {
        Texture2D textureRead = null;
        if (System.IO.File.Exists(_pathFile))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(_pathFile);
            Texture2D textureOriginal = new Texture2D(1, 1);
            textureOriginal.LoadImage(bytes);
            textureRead = ScaleTexture2D(textureOriginal, _height);
        }
        return textureRead;
    }

    // -------------------------------------------
    /* 
	 * ScaleTexture2D
	 */
    public static Texture2D ScaleTexture2D(Texture2D _texture, int _height)
    {
        float factorScale = ((float)_height / (float)_texture.height);
        return ImageUtils.ScaleTexture(_texture, (int)(_texture.width * factorScale), (int)_height);
    }

    // -------------------------------------------
    /* 
	 * LoadBytesRawImage
	 */
    public static void LoadBytesRawImage(Image _image, byte[] _pvrtcBytes)
    {
        Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
        tex.LoadRawTextureData(_pvrtcBytes);
        tex.Apply();
        _image.material.mainTexture = tex;
    }

    // -------------------------------------------
    /* 
	 * LoadBytesImage
	 */
    public static void LoadBytesImage(Image _image, byte[] _pvrtcBytes)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(_pvrtcBytes);
        _image.material.mainTexture = tex;
    }

    // -------------------------------------------
    /* 
	 * LoadBytesImage
	 */
    public static void LoadBytesImage(Image _image, byte[] _pvrtcBytes, int _height, int _maximumHeightAllowed)
    {
        try
        {
            Texture2D textureOriginal = new Texture2D(1, 1);
            textureOriginal.LoadImage(_pvrtcBytes);
            float factorScale = ((float)_maximumHeightAllowed / (float)textureOriginal.height);
            Texture2D textureScaled = ImageUtils.ScaleTexture(textureOriginal, (int)(textureOriginal.width * factorScale), (int)_maximumHeightAllowed);
            _image.overrideSprite = ToSprite(textureScaled);
            float finalWidth = textureScaled.width * ((float)_height / (float)textureScaled.height);
            _image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, _height);
            _image.gameObject.SetActive(true);
        }
        catch (Exception err)
        {
            if (ScreenController.Instance.DebugMode) Debug.Log(err.StackTrace);
        };
    }

    // -------------------------------------------
    /* 
	 * GetBytesImage
	 */
    public static byte[] GetBytesImage(Image _image)
    {
        return _image.sprite.texture.GetRawTextureData();
    }

    // -------------------------------------------
    /* 
	 * GetBytesPNG
	 */
    public static byte[] GetBytesPNG(Image _image)
    {
        return _image.overrideSprite.texture.EncodeToPNG();
    }

    // -------------------------------------------
    /* 
	 * GetBytesJPG
	 */
    public static byte[] GetBytesJPG(Image _image)
    {
        return _image.overrideSprite.texture.EncodeToJPG(75);
    }

    // -------------------------------------------
    /* 
	 * GetBytesPNG
	 */
    public static byte[] GetBytesPNG(Sprite _image)
    {
        return _image.texture.EncodeToPNG();
    }

    // -------------------------------------------
    /* 
	 * GetBytesJPG
	 */
    public static byte[] GetBytesJPG(Sprite _image)
    {
        return _image.texture.EncodeToJPG(75);
    }

    // -------------------------------------------
    /* 
	 * GetFiles
	 */
    public static string[] GetFiles(string _path, string _searchPattern, SearchOption _searchOption)
    {
        string[] searchPatterns = _searchPattern.Split('|');
        List<string> files = new List<string>();
        foreach (string sp in searchPatterns)
        {
            files.AddRange(System.IO.Directory.GetFiles(_path, sp, _searchOption));
        }            
        files.Sort();
        return files.ToArray();
    }

    // -------------------------------------------
    /* 
	 * GetFiles
	 */
    public static FileInfo[] GetFiles(DirectoryInfo _path, string _searchPattern, SearchOption _searchOption)
    {
        string[] searchPatterns = _searchPattern.Split('|');
        List<FileInfo> files = new List<FileInfo>();
        foreach (string sp in searchPatterns)
        {
            files.AddRange(_path.GetFiles(sp, _searchOption));
        }
        return files.ToArray();
    }

    // -------------------------------------------
    /* 
	 * ScaleTexture
	 */
    public static Texture2D ScaleTexture(Texture2D _source, int _targetWidth, int _targetHeight)
    {
        Texture2D result = new Texture2D(_targetWidth, _targetHeight, _source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / _source.width) * ((float)_source.width / _targetWidth);
        float incY = ((float)1 / _source.height) * ((float)_source.height / _targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = _source.GetPixelBilinear(incX * ((float)px % _targetWidth),
                              incY * ((float)Mathf.Floor(px / _targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    // -------------------------------------------
    /* 
	 * ToSprite
	 */
    public static Sprite ToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
