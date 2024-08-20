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

            RectTransform viewTransform = viewImage.GetComponent<RectTransform>();
            viewTransform.localScale = new Vector3(viewTransform.localScale.x * -1.0f, viewTransform.localScale.y,
                viewTransform.localScale.z);
        }

        public void ToggleLandscape()
        {
            Vector2 sizeDelta = canvasTransform.sizeDelta;
            canvasTransform.sizeDelta = new Vector2(sizeDelta.y, sizeDelta.x);
            canvasTransform.ForceUpdateRectTransforms();

            if (camera.targetTexture == landscapeRT)
            {
                camera.targetTexture = portraitRT;
                viewImage.texture = portraitRT;
                
                tabletMesh.transform.Rotate(new Vector3(0, 0, 1), 90);
            }
            else
            {
                camera.targetTexture = landscapeRT;
                viewImage.texture = landscapeRT;

                tabletMesh.transform.rotation = new Quaternion();
            }

            IsLandscape = !IsLandscape;
            
            camera.ResetProjectionMatrix();
        }
    }
}