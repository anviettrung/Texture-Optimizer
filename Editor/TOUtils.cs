using System;
using UnityEditor;
using UnityEngine;

namespace AVT.TextureOptimizer
{
    internal static class TOUtils // Stand for Texture Optimizer Utilities
    {
        #region Hotkey
        
        internal const string pluginFolderPath = "Create/Texture Optimizer/";

        private const string hotkey9Slices = " &q";
        private const string hotkeyScaleToDiv4 = " &s";
        private const string hotkeyResizeToDiv4 = " &r";

        internal const string menuItem9Slices = pluginFolderPath + "9-Slices" + hotkey9Slices;
        internal const string menuItemScaleToDiv4 = pluginFolderPath + "Scale to Div 4" + hotkeyScaleToDiv4;
        internal const string menuItemResizeToDiv4 = pluginFolderPath + "Resize to Div 4" + hotkeyResizeToDiv4;
        
        #endregion
        
        public static void TransformSelectedTextures(Func<Texture2D, TextureImporter, Texture2D> transform)
        {
            foreach (var obj in Selection.objects)
            {
                var texture = obj as Texture2D;
                if (texture == null) continue;

                var path = AssetDatabase.GetAssetPath(obj);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                var readableStatus = importer.isReadable;
                importer.isReadable = true;
                importer.SaveAndReimport();
                
                texture = transform(texture, importer);
                
                SaveSubSprite(texture, path);

                importer.isReadable = readableStatus;
                importer.SaveAndReimport();

                Debug.Log($"<color=green><b>Complete Optimize:</b></color> {texture.name}");
            }
        }
        
        private static void SaveSubSprite(Texture2D texture, string filePath)
        {
            System.IO.File.WriteAllBytes(filePath, texture.EncodeToPNG());
        }
    }
}

