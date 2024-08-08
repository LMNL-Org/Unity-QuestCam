using System;
using UnityEngine;
using UnityEngine.UI;

namespace QuestCam
{
    public class CameraTablet : MonoBehaviour
    {
        public Camera camera;
        public GameObject tabletMesh;
        public RectTransform canvasTransform;
        public RenderTexture landscapeRT;
        public RenderTexture portraitRT;
        public RawImage viewImage;

        public bool IsLandscape = true;

        public void ToggleCameraView()
        {
            camera.transform.Rotate(Vector3.up, 180);
        }

        public void ToggleLandscape()
        {
            Vector3 scale = tabletMesh.transform.localScale;
            tabletMesh.transform.localScale = new Vector3(scale.y, scale.x, scale.z);

            Vector2 sizeDelta = canvasTransform.sizeDelta;
            canvasTransform.sizeDelta = new Vector2(sizeDelta.y, sizeDelta.x);
            canvasTransform.ForceUpdateRectTransforms();

            if (camera.targetTexture == landscapeRT)
            {
                camera.targetTexture = portraitRT;
                viewImage.texture = portraitRT;
            }
            else
            {
                camera.targetTexture = landscapeRT;
                viewImage.texture = landscapeRT;
            }

            IsLandscape = !IsLandscape;
            
            camera.ResetProjectionMatrix();
        }
    }
}