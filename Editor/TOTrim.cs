using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AVT.TextureOptimizer.TOUtils;

namespace AVT.TextureOptimizer
{
    public static class TOTrim
    {
        
        
        #region Menu Item

        [MenuItem(menuItemIsAutoTrimCheck)]
        private static void ToggleIsAutoTrim() => IsAutoTrim = !IsAutoTrim;

        [MenuItem(menuItemTrimTexture)]
        public static void TrimTex()
        {
            TransformSelectedTextures((texture, importer) => Trim(texture));
        }
        
        #endregion

        #region Core

        public static Texture2D Trim(Texture2D texture)
        {
            var pixels = texture.GetPixels(
                (int)texture.texelSize.x, 
                (int)texture.texelSize.y, 
                texture.width, 
                texture.height);
            
            var outlineBot = CalculateOutlineBot(pixels, texture);
            var outlineTop = CalculateOutlineTop(pixels, texture);
            var outlineLeft = CalculateOutlineLeft(pixels, texture);
            var outlineRight = CalculateOutlineRight(pixels, texture);

            var newTexSize = new Vector2Int(
                texture.width - (outlineLeft + outlineRight), 
                texture.height - (outlineBot + outlineTop));
            
            var output = new Texture2D(newTexSize.x, newTexSize.y);
            var colors = new Color[newTexSize.x * newTexSize.y];

            for (var i = 0; i < colors.Length; i++)
            {
                var width = i % newTexSize.x;
                var height = i / newTexSize.x;
                colors[i] = pixels[texture.width * (height + outlineBot) + outlineLeft + width];
            }

            output.SetPixels(colors);
            output.Apply();
            
            return output;
        }

        #endregion

        #region Utils
        
        private static int CalculateOutlineBot(IList<Color> pixels, Texture texture)
        {
            var outline = 0;
            for (var i = 0; i < texture.height; i++)
            {
                for (var j = 0; j < texture.width; j++)
                {
                    if (pixels[i * texture.width + j].a > 0) 
                        return outline;
                }
                outline++;
            }
            return outline;
        }
        
        private static int CalculateOutlineTop(IList<Color> pixels, Texture texture)
        {
            var outline = 0;
            for (var i = texture.height - 1; i > 0; i--)
            {
                for (var j = 0; j < texture.width; j++)
                {
                    if (pixels[i * texture.width + j].a > 0)
                        return outline;
                }
                outline++;
            }
            return outline;
        }
        
        private static int CalculateOutlineLeft(IList<Color> pixels, Texture texture)
        {
            var outline = 0;
            for (var i = 0; i < texture.width; i++)
            {
                for (var j = 0; j < texture.height; j++)
                {
                    if (pixels[i + j * texture.width].a > 0)
                        return outline;
                }
                outline++;
            }
            return outline;
        }
        
        private static int CalculateOutlineRight(IList<Color> pixels, Texture texture)
        {
            var outline = 0;
            for (var i = texture.width - 1; i > 0; i--)
            {
                for (var j = 0; j < texture.height; j++)
                {
                    if (pixels[i + j * texture.width].a > 0)
                        return outline;
                }
                outline++;
            }
            return outline;
        }

        #endregion
    }
}
