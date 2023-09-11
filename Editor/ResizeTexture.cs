using AVT.TextureOptimizer;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ResizeTexture
{
    #region Settings

    private static bool IsAutoTrim
    {
        get => EditorPrefs.GetBool("ResizeTexture_isAutoTrim", false);
        set
        {
            EditorPrefs.SetBool("ResizeTexture_isAutoTrim", value);
            Menu.SetChecked(TOUtils.pluginFolderPath + "Is Auto Trim", value);
        }
    }

    #endregion

    #region Menu Item

    [MenuItem(TOUtils.pluginFolderPath + "Is Auto Trim", priority = -100)]
    private static void ToggleIsAutoTrim()
    {
        IsAutoTrim = !IsAutoTrim;
    }
    
    [MenuItem(TOUtils.pluginFolderPath + "ResizeToDiv4")]
    public static void ResizeTexToDiv4()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Texture2D tex = obj as Texture2D;
            string name = tex.name;

            if (tex == null) continue;

            importer.isReadable = true;
            importer.SaveAndReimport();

            //trim xung quanh
            if (IsAutoTrim)
                tex = CaculateTrim(tex);

            //tinh size
            Vector2Int size = new Vector2Int(tex.width, tex.height);
            Vector2Int newSize = new Vector2Int(size.x / 4 + (size.x % 4 > 0 ? 1 : 0), size.y / 4 + (size.y % 4 > 0 ? 1 : 0)) * 4;

            //resize nhung co vien alpha = 0 xung quanh 
            tex = CaculateResize(tex, newSize - size);
            //delete old
            //AssetDatabase.DeleteAsset(path);
            //save new
            SaveSubSprite(tex, path ,name);

            importer.isReadable = false;
            importer.SaveAndReimport();
        }

    }
    
    [MenuItem("Auto/Resize/ScaleTextureToDiv4 &s", false, -101)]

    public static void ScaleTexToDiv4()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Texture2D tex = obj as Texture2D;
            string name = tex.name;

            if (tex == null) continue;

            importer.isReadable = true;
            importer.SaveAndReimport();

            //trim xung quanh
            if (IsAutoTrim)
                tex = CaculateTrim(tex);
            
            if (tex.width % 4 == 0 && tex.height % 4 == 0)
                continue;
            
            //tinh size
            Vector2Int size = new Vector2Int(tex.width, tex.height);
            Vector2Int newSize = new Vector2Int(size.x / 4 + (size.x % 4 > 0 ? 1 : 0), size.y / 4 + (size.y % 4 > 0 ? 1 : 0)) * 4;

            //scale deu
            tex = ScaleTexture(tex, newSize.x, newSize.y);
            //delete old
            //AssetDatabase.DeleteAsset(path);
            //save new
            SaveSubSprite(tex, path ,name);

            importer.isReadable = false;
            importer.SaveAndReimport();
        }
        Debug.Log("<color=green><b>Complete Resize!</b></color>");
    }

    [MenuItem("Auto/Resize/TrimTexture", false, -101)]

    public static void TrimTex()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            Texture2D tex = obj as Texture2D;
            string name = tex.name;

            if (tex == null) continue;

            importer.isReadable = true;
            importer.SaveAndReimport();

            //trim xung quanh
            tex = CaculateTrim(tex);

            //save new
            SaveSubSprite(tex, path, name);

            importer.isReadable = false;
            importer.SaveAndReimport();
        }

    }

    #endregion

    #region Core

    // Since a sprite may exist anywhere on a tex2d, this will crop out the sprite's claimed region and return a new, cropped, tex2d.
    private static Texture2D CaculateResize(Texture2D texture, Vector2Int outline)
    {
        //left, right
        Vector2Int pixelWidth = new Vector2Int(outline.x / 2, outline.x + texture.width - outline.x / 2 - outline.x % 2);
        Vector2Int pixelHeight = new Vector2Int(outline.y / 2, outline.y + texture.height - outline.y / 2 - outline.y % 2);


        var output = new Texture2D((int)texture.width + outline.x, (int)texture.height + outline.y);
        Color[] colors = new Color[(texture.width + outline.x) * (texture.height + outline.y)];

        Color[] pixels = texture.GetPixels((int)texture.texelSize.x, (int)texture.texelSize.y, (int)texture.width, (int)texture.height);

        for (int i = 0; i < colors.Length; i++)
        {
            int width = i % (texture.width + outline.x);
            int height = i / (texture.width + outline.x);

            if (width >= pixelWidth.x && width < pixelWidth.y && height >= pixelHeight.x && height < pixelHeight.y)
            {
                colors[i] = pixels[ConvertGetTexPointFromText(i, outline, texture.width, height)];
            }
            else
            {
                colors[i] = Color.clear;
            }
        }

        output.SetPixels(colors);
        output.Apply();
        output.name = texture.name;
        return output;
    }

    private static Texture2D CaculateTrim(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels((int)texture.texelSize.x, (int)texture.texelSize.y, (int)texture.width, (int)texture.height);
        Vector2Int outline = Vector2Int.zero;
        var outlineLeft = 0;
        var outlineRight = 0;
        var outlineTop = 0;
        var outlineBot = 0;

        //caculate space

        //down
        bool isBreak = false;
        for (int i = 0; i < texture.height; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                if (pixels[i * texture.width + j].a > 0)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak)
            {
                isBreak = false;
                break;
            }

            outlineBot++;
        }

        //left
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                if (pixels[i + j * texture.width].a > 0)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak)
            {
                isBreak = false;
                break;
            }

            outlineLeft++;
        }

        //top
        for (int i = texture.height - 1; i > 0; i--)
        {
            for (int j = 0; j < texture.width; j++)
            {
                if (pixels[i * texture.width + j].a > 0)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak)
            {
                isBreak = false;
                break;
            }

            outlineTop++;
        }

        //right
        for (int i = texture.width - 1; i > 0; i--)
        {
            for (int j = 0; j < texture.height; j++)
            {
                if (pixels[i + j * texture.width].a > 0)
                {
                    isBreak = true;
                    break;
                }
            }

            if (isBreak)
            {
                isBreak = false;
                break;
            }

            outlineRight++;
        }

        Vector2Int newTexSize = new Vector2Int(texture.width - (outlineLeft + outlineRight), texture.height - (outlineBot + outlineTop));
        var output = new Texture2D(newTexSize.x, newTexSize.y);
        Color[] colors = new Color[newTexSize.x * newTexSize.y];

        for (int i = 0; i < colors.Length; i++)
        {
            int width = i % newTexSize.x;
            int height = i / newTexSize.x;
            colors[i] = pixels[texture.width * (height + outlineBot) + outlineLeft + width];
        }

        output.SetPixels(colors);
        output.Apply();
        output.name = texture.name;
        return output;
    }

    private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                              incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    private static int ConvertGetTexPointFromText(int point, Vector2Int outline, int texWidth, int texHeight)
    {
        return point - outline.x / 2 - (outline.y / 2) * (texWidth + outline.x) - (texHeight - outline.y / 2) * outline.x;
    }

    #endregion

    #region Utils

    static ResizeTexture()
    {
        EditorApplication.delayCall += () => { IsAutoTrim = IsAutoTrim; };
    }
    
    private static void SaveSubSprite(Texture2D tex, string saveToDirectory, string name)
    {
        saveToDirectory = SplitPath(saveToDirectory);
        if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, name + ".png"), tex.EncodeToPNG());
        Debug.Log("Save success file : " + saveToDirectory +"/"+ name);
    }

    private static string SplitPath(string path = "")
    {
        for (int i = path.Length - 1; i >= 0; i--)
        {
            char c = path[i];
            path = path.Remove(i);
            if (c == '/')
            {
                break;
            }
        }

        return path;
    }

    #endregion
}