using UnityEngine;
using UnityEditor;

namespace AVT.TextureOptimizer
{
    public static class OptimizeUISprite
    {
        #region Define
        private struct Outline
        {
            public int top;
            public int bot;
            public int left;
            public int right;
        }

        #endregion

        #region Menu Item

        [MenuItem(TOUtils.menuItem9Slices)]
        public static void OptimizeSprite9Slices()
        {
            TOUtils.TransformSelectedTextures((texture, importer) =>
            {
                var outline = Calculate9SlicesOutline(texture);
                importer.spriteBorder = new Vector4(
                    outline.left + 1,
                    outline.bot + 1,
                    outline.right - 1,
                    outline.top - 1);
                return Trim(texture, outline);
            });
        }

        #endregion

        #region Core

        private static Texture2D Trim(Texture2D texture, Outline outline)
        {
            var pixels = texture.GetPixels(
                (int)texture.texelSize.x,
                (int)texture.texelSize.y,
                texture.width,
                texture.height);

            var newWidth = outline.left + outline.right;
            var newHeight = outline.top + outline.bot;
            newWidth += 4 - newWidth % 4;
            newHeight += 4 - newHeight % 4;

            var res = new Texture2D(newWidth, newHeight);
            var newPixels = new Color[newWidth * newHeight];

            for (var i = 0; i < newPixels.Length; i++)
            {
                var posX = i % newWidth;
                var posY = i / newWidth;
                var mapX = posX <= outline.left ? posX : texture.width - (newWidth - posX);
                var mapY = posY <= outline.bot ? posY : texture.height - (newHeight - posY);

                newPixels[i] = pixels[texture.width * mapY + mapX];
            }

            res.name = texture.name;
            res.SetPixels(newPixels);
            res.Apply();

            return res;
        }

        private static Outline Calculate9SlicesOutline(Texture2D texture)
        {
            var res = new Outline();

            var pixels = texture.GetPixels(
                (int)texture.texelSize.x,
                (int)texture.texelSize.y,
                texture.width,
                texture.height);


            var midV = texture.height / 2;
            var midH = texture.width / 2;

            res.top = res.bot = midV;
            res.left = res.right = 0;

            while (res.top < texture.height)
            {
                if (!IsEqualRow(ref pixels, texture.width, res.top, midV))
                    break;
                res.top++;
            }

            res.top = texture.height - res.top - 1; // inverse

            while (res.bot > 0)
            {
                if (!IsEqualRow(ref pixels, texture.width, res.bot, midV))
                    break;
                res.bot--;
            }

            res.bot += 1;

            // Find Left and Right
            var minLeft = 0;
            var minRight = texture.width;

            for (var y = 0; y < texture.height; y++)
            {
                var left = midH;
                var right = midH;
                var iRow = y * texture.width;

                while (left > 1)
                {
                    if (pixels[iRow + left] != pixels[iRow + midH])
                        break;
                    left--;
                }

                while (right < texture.width)
                {
                    if (pixels[iRow + right] != pixels[iRow + midH])
                        break;
                    right++;
                }

                if (minLeft < left)
                    minLeft = left;

                if (minRight > right)
                    minRight = right;
            }

            res.left = minLeft;
            res.right = texture.width - minRight;

            return res;
        }

        private static bool IsEqualRow(ref Color[] pixels, int width, int a, int b)
        {
            var la = a * width;
            var lb = b * width;
            for (var i = 0; i < width; i++)
            {
                if (pixels[la + i] != pixels[lb + i])
                    return false;
            }

            return true;
        }


        #endregion
    }
}
