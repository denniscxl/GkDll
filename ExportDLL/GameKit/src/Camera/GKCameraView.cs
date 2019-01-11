using UnityEngine;
using System.Collections;

namespace GKCamera
{
    public class GKCameraView : MonoBehaviour
    {
        // 距离摄像机8.5米 用黄色表示.
        public float upperDistance = 8.5f;
        // 距离摄像机12米 用红色表示.
        public float lowerDistance = 12.0f;

        Camera _theCamera;
        Transform _tx;


        void Start()
        {
            if (!_theCamera)
            {
                _theCamera = Camera.main;
            }
            _tx = _theCamera.transform;
        }


        void Update()
        {
            _FindUpperCorners();
            _FindLowerCorners();
        }


        void _FindUpperCorners()
        {
            Vector3[] corners = _GetCorners(upperDistance);

            // for debugging
            Debug.DrawLine(corners[0], corners[1], Color.yellow); // UpperLeft -> UpperRight
            Debug.DrawLine(corners[1], corners[3], Color.yellow); // UpperRight -> LowerRight
            Debug.DrawLine(corners[3], corners[2], Color.yellow); // LowerRight -> LowerLeft
            Debug.DrawLine(corners[2], corners[0], Color.yellow); // LowerLeft -> UpperLeft
        }


        void _FindLowerCorners()
        {
            Vector3[] corners = _GetCorners(lowerDistance);

            // for debugging
            Debug.DrawLine(corners[0], corners[1], Color.red);
            Debug.DrawLine(corners[1], corners[3], Color.red);
            Debug.DrawLine(corners[3], corners[2], Color.red);
            Debug.DrawLine(corners[2], corners[0], Color.red);
        }


        Vector3[] _GetCorners(float distance)
        {
            Vector3[] corners = new Vector3[4];

            float halfFOV = (_theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            float aspect = _theCamera.aspect;

            float height = distance * Mathf.Tan(halfFOV);
            float width = height * aspect;

            // UpperLeft
            corners[0] = _tx.position - (_tx.right * width);
            corners[0] += _tx.up * height;
            corners[0] += _tx.forward * distance;

            // UpperRight
            corners[1] = _tx.position + (_tx.right * width);
            corners[1] += _tx.up * height;
            corners[1] += _tx.forward * distance;

            // LowerLeft
            corners[2] = _tx.position - (_tx.right * width);
            corners[2] -= _tx.up * height;
            corners[2] += _tx.forward * distance;

            // LowerRight
            corners[3] = _tx.position + (_tx.right * width);
            corners[3] -= _tx.up * height;
            corners[3] += _tx.forward * distance;

            return corners;
        }
    }
}