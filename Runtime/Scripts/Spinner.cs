using System;
using UnityEngine;

namespace QuestCam
{
    public class Spinner : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private float _angle;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        private void Update()
        {
            _rectTransform.rotation = Quaternion.Euler(0f, 0f, _angle);
            _angle += Time.deltaTime * 10f;
        }
    }
}