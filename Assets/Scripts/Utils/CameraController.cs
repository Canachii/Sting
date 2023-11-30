using System;
using DG.Tweening;
using UnityEngine;

namespace Utils
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;
        private Camera thisCamera;
        public float shakeDuration = .1f;
        public float shakePower = .5f;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            thisCamera = GetComponent<Camera>();
        }

        public void ShakeCamera()
        {
            thisCamera.DOShakePosition(shakeDuration, shakePower);
        }
    }
}