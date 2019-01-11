using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace GKUI
{
    /**
     * UGUI.
     * */
    [RequireComponent(typeof(Image))]
    public class GKUISpriteAnimation : MonoBehaviour
    {
        public float FPS = 5;
        public List<Sprite> SpriteFrames;
        public bool IsPlaying = false;
        public bool Foward = true;
        public bool AutoPlay = false;
        public bool Loop = false;

        Image _imageSource;
        int _curFrame = 0;
        float _delta = 0;

        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        void Awake()
        {
            _imageSource = GetComponent<Image>();
        }

        void Start()
        {
            if (AutoPlay)
            {
                Play();
            }
            else
            {
                IsPlaying = false;
            }
        }

        void SetSprite(int idx)
        {
            _imageSource.sprite = SpriteFrames[idx];
            //		_imageSource.SetNativeSize();
        }

        public void Play()
        {
            IsPlaying = true;
            Foward = true;
        }

        public void PlayReverse()
        {
            IsPlaying = true;
            Foward = false;
        }

        void Update()
        {
            if (!IsPlaying || 0 == FrameCount)
            {
                return;
            }

            _delta += Time.deltaTime;
            if (_delta > 1 / FPS)
            {
                _delta = 0;
                if (Foward)
                {
                    _curFrame++;
                }
                else
                {
                    _curFrame--;
                }

                if (_curFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        _curFrame = 0;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }
                else if (_curFrame < 0)
                {
                    if (Loop)
                    {
                        _curFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }

                SetSprite(_curFrame);
            }
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            _curFrame = 0;
            SetSprite(_curFrame);
            IsPlaying = false;
        }

        public void Rewind()
        {
            _curFrame = 0;
            SetSprite(_curFrame);
            Play();
        }
    }
}