using UnityEngine;

namespace BRIJ
{
    public class AndroidExtensions
    {
        private static AndroidJavaObject _activity;

        private static AndroidJavaObject Activity
        {
            get
            {
                if (_activity != null)
                    return _activity;

                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                return _activity;
            }
        }

        private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";

        public static string SaveImageToGallery(Texture2D texture2D, string title, string description)
        {
            using var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass);
            using var cr = Activity.Call<AndroidJavaObject>("getContextResolver");
            var image = Texture2DToAndroidBitmap(texture2D);
            var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, title, description);
            return imageUrl;
        }

        private static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
        {
            var encoded = texture2D.EncodeToPNG();
            using var bf = new AndroidJavaClass("android.graphics.BitmapFactory");
            return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
        }
    }
}