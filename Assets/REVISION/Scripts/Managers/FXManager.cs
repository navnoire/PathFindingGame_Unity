using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavnoireCoding.Managers
{
    public class FXManager : Singleton<FXManager>
    {

        public Shader blurShader;

        public Sprite BackgroundBlurSprite()
        {
            Camera _camera = Camera.main;
            int resWidth = _camera.pixelWidth;
            int resHeight = _camera.pixelHeight;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 0);
            _camera.targetTexture = rt;
            _camera.Render();

            Blur(rt, ref rt, 4);
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            tex.Apply();


            _camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            return Sprite.Create(tex, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));
        }

        private void Blur(RenderTexture source, ref RenderTexture destination, int iterationsCount)
        {
            if (blurShader == null)
            {
                Debug.LogError("[FXManager] Blur shader is not assigned properly.");
                return;
            }

            const int BoxDownPass = 0;
            const int BoxUpPass = 1;
            RenderTexture[] textures = new RenderTexture[4];


            Material bloom = new Material(blurShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };


            int width = source.width / 2;
            int height = source.height / 2;
            RenderTextureFormat format = source.format;

            RenderTexture currentDestination = textures[0] =
                RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(source, currentDestination, bloom, BoxDownPass);
            RenderTexture currentSource = currentDestination;


            int i = 1;
            for (; i < iterationsCount; i++)
            {
                width /= 2;
                height /= 2;
                if (height < 2)
                {
                    break;
                }
                currentDestination = textures[i] =
                    RenderTexture.GetTemporary(width, height, 0, format);
                Graphics.Blit(currentSource, currentDestination, bloom, BoxDownPass);
                currentSource = currentDestination;

            }

            for (i -= 2; i >= 0; i--)
            {
                currentDestination = textures[i];
                currentDestination.DiscardContents();
                Graphics.Blit(currentSource, currentDestination, bloom, BoxUpPass);
                RenderTexture.ReleaseTemporary(currentSource);
                currentSource = currentDestination;
            }

            Graphics.Blit(currentSource, destination, bloom, BoxUpPass);
            currentSource.DiscardContents();
            RenderTexture.ReleaseTemporary(currentDestination);
        }

    }
}
