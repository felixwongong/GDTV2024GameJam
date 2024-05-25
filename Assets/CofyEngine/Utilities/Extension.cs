using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CofyEngine.Util
{
    public static class Extension
    {
        public static IEnumerator ToRoutine(this Task t)
        {
            while (!t.IsCompleted)
            {
                yield return null;
            }

            if (!t.IsCompletedSuccessfully)
            {
                FLog.LogError($"Task ({t}) failed or canceled!", t.Exception);
            }
        }


        public static Sprite ToSprite(this Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            // Get the texture width and height
            int width = texture.width;
            int height = texture.height;

            // Create a Rect object with the texture dimensions
            Rect rect = new Rect(0, 0, width, height);

            // Create a Sprite from the Texture2D
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

            return sprite;
        }

        public static double CalculateDistance(this Vector2 latlng1, Vector2 latlng2) 
        {
            double R = 6371e3; // Earth's radius in meters
            double lat1Rad = ToRadians(latlng1.x);
            double lat2Rad = ToRadians(latlng2.x);
            double deltaLat = ToRadians(latlng2.x - latlng1.x);
            double deltaLng = ToRadians(latlng2.y - latlng1.y);

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTName(this object obj)
        {
            return obj?.GetType().Name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isRefNull(this object obj)
        {
            return ReferenceEquals(obj, null);
        }
    }
}