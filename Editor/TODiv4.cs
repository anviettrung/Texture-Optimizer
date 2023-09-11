using UnityEditor;
using UnityEngine;
using static AVT.TextureOptimizer.TOUtils;

namespace AVT.TextureOptimizer
{
    public static class TODiv4
    {
        #region Menu Item

        [MenuItem(menuItemScaleToDiv4)]
        public static void ScaleToDiv4()
        {
            TransformSelectedTextures((texture, importer) =>
            {
                var size = new Vector2Int(texture.width, texture.height);
                var newSize = 4 * new Vector2Int(
                    size.x / 4 + (size.x % 4 > 0 ? 1 : 0), 
                    size.y / 4 + (size.y % 4 > 0 ? 1 : 0));
                
                return ScaleTexture(texture, newSize);
            });
        }

        [MenuItem(menuItemResizeToDiv4)]
        public static void ResizeToDiv4()
        {
            TransformSelectedTextures((texture, importer) =>
            {
                var size = new Vector2Int(texture.width, texture.height);
                var newSize = 4 * new Vector2Int(
                    size.x / 4 + (size.x % 4 > 0 ? 1 : 0), 
                    size.y / 4 + (size.y % 4 > 0 ? 1 : 0));
                
                //resize nhung co vien alpha = 0 xung quanh 
                return ResizeTexture(texture, newSize - size);
            });
        }

        #endregion

        #region Core

        private static Texture2D ScaleTexture(Texture2D texture, Vector2Int size)
        {
            var output = new Texture2D(size.x, size.y);
            var pixels = output.GetPixels(0);
            var incX = (float)1 / texture.width * ((float)texture.width / size.x);
            var incY = (float)1 / texture.height * ((float)texture.height / size.y);
            for (var px = 0; px < pixels.Length; px++)
            {
                pixels[px] = texture.GetPixelBilinear(
                    incX * ((float)px % size.x),
                    incY * Mathf.Floor(px / (float)size.x));
            }
            
            output.SetPixels(pixels);
            output.Apply();
            
            return output;
        }

        private static Texture2D ResizeTexture(Texture2D texture, Vector2Int outline)
        {
            // left - right
            var pixelWidth = new Vector2Int(
                outline.x / 2, 
                outline.x + texture.width - outline.x / 2 - outline.x % 2);
            
            // top - bot
            var pixelHeight = new Vector2Int(
                outline.y / 2, 
                outline.y + texture.height - outline.y / 2 - outline.y % 2);
            
            var output = new Texture2D(
                texture.width + outline.x, 
                texture.height + outline.y);
            
            var colors = new Color[output.width * output.height];

            var pixels = texture.GetPixels(
                (int)texture.texelSize.x, 
                (int)texture.texelSize.y, 
                texture.width, 
                texture.height);

            for (var i = 0; i < colors.Length; i++)
            {
                var width = i % (texture.width + outline.x);
                var height = i / (texture.width + outline.x);

                if (width >= pixelWidth.x && width < pixelWidth.y && height >= pixelHeight.x && height < pixelHeight.y)
                    colors[i] = pixels[ConvertGetTexPointFromText(i, outline, texture.width, height)];
                else
                    colors[i] = Color.clear;
            }

            output.SetPixels(colors);
            output.Apply();
            
            return output;
        }
        
        private static int ConvertGetTexPointFromText(int point, Vector2Int outline, int texWidth, int texHeight)
        {
            return point - outline.x / 2 - outline.y / 2 * (texWidth + outline.x) - (texHeight - outline.y / 2) * outline.x;
        }

        #endregion
    }
}
