using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using System.Security.Cryptography;
using System;
using System.Text;
using Random = UnityEngine.Random;

public class ReplaceShader : EditorWindow
{
    static int m_goCount;
    static int m_materialCount;
    
    [MenuItem("GameObject/Replace Shader")]
    static void Apply()
    {
        var beginTime = System.DateTime.UtcNow.Ticks;
        m_goCount = 0;
        m_materialCount = 0;
        List<Material> materials = new List<Material>();
            
        foreach (var go in Selection.gameObjects)
            search(go, ref materials);
        
        var newShader = Shader.Find("Elysia/S_ForwardLightPixel");
        if (newShader == null)
        {
            Debug.LogError("new shader not found");
        }
        var oldShader = Shader.Find("Universal Render Pipeline/Lit");
        if (oldShader == null)
        {
            Debug.LogError("old shader not found");
        }

        for (int i = 0; i < materials.Count; ++i)
        {
            if (materials[i] == null)
            {
                materials[i] = new Material(newShader);
                AssetDatabase.CreateAsset(materials[i],
                             "Assets/Materials/" + 
                             CalculateMD5Hash("Packages/com.p4.artresource/Material/" + System.DateTime.Now) + ".asset");
                EditorUtility.SetDirty(materials[i]);
                AssetDatabase.SaveAssetIfDirty(materials[i]);
                continue;
            }
             
            if (materials[i].shader == oldShader)
            {
                var baseTex = materials[i].GetTexture("_MainTex");
                var normalTex = materials[i].GetTexture("_BumpMap");
                var emissionTex = materials[i].GetTexture("_EmissionMap");
                
                var BaseColorTint = materials[i].GetColor("_BaseColor");
                var EmissionTint = materials[i].GetColor("_EmissionColor");

                var alphaCut = materials[i].GetFloat("_Cutoff");
                var RoughnessScale = 1f - materials[i].GetFloat("_Smoothness");
                var MetallicScale = materials[i].GetFloat("_Metallic");
                var SpecularScale = materials[i].GetFloat("Specular");
                var NormalScale = materials[i].GetFloat("_BumpScale");
                var AOScale = materials[i].GetFloat("_OcclusionStrength");
                
                materials[i].shader = newShader;
                materials[i].SetTexture("_BaseColorTex", baseTex);
                materials[i].SetTexture("_NormalTex", normalTex);
                materials[i].SetTexture("_EmissionTex", emissionTex);
                
                materials[i].SetColor("_BaseColorTint", BaseColorTint);
                materials[i].SetColor("_EmissionTint", EmissionTint);
                
                materials[i].SetFloat("_AlphaCut", alphaCut);
                materials[i].SetFloat("_RoughnessScale", RoughnessScale);
                materials[i].SetFloat("_MetallicScale", MetallicScale);
                materials[i].SetFloat("_SpecularScale", SpecularScale);
                materials[i].SetFloat("_NormalScale", NormalScale);
                materials[i].SetFloat("_AOScale", AOScale);

                EditorUtility.SetDirty(materials[i]);
                AssetDatabase.SaveAssetIfDirty(materials[i]);
            }
        }
            
        var endTime = System.DateTime.UtcNow.Ticks;
        var deltaTime = endTime - beginTime;

        Debug.Log($"Searched in {m_goCount} GameObjects, found and replace {m_materialCount} materials. Took {deltaTime / 10000.0} ms.");

            
        //EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();
    }
    
    static void search(GameObject go, ref List<Material> materials)
    {
        m_goCount++;
        var renderers = go.GetComponentsInChildren<Renderer>();
        List<Material> currMaterials = new List<Material>();
        foreach (var renderer in renderers)
        {
            currMaterials.AddRange(renderer.sharedMaterials);
        }

        m_materialCount += currMaterials.Count;
        materials.AddRange(currMaterials);
        foreach (Transform child in go.transform)
            search(child.gameObject, ref materials);
    }
    
    static string ComputeAssetHash(string assetPath)
    {
        if (!File.Exists(assetPath))
        return null;
        
        List<byte> list = new List<byte>();
        
        //读取资源及其meta文件为字节数组
        list.AddRange(GetAssetBytes(assetPath));
        
        //读取资源的依赖项及其meta文件为字节数组(依赖项本质也是资源的路径)
        string[] dependencies = AssetDatabase.GetDependencies(assetPath);
        
        for (int i = 0, iMax = dependencies.Length; i < iMax; ++i)
            list.AddRange(GetAssetBytes(dependencies[i]));
        
        //如果资源有其他依赖项的话，也需要将其对应的字节数组读取到 list 中，然后再进行 哈希码 的计算
        //返回资源 hash
        return ComputeHash(list.ToArray());
    }
    
    static byte[] GetAssetBytes(string assetPath)
    {
        if (!File.Exists(assetPath))
            return null;

        List<byte> list = new List<byte>();

        var assetBytes = File.ReadAllBytes(assetPath);
        list.AddRange(assetBytes);

        string metaPath = assetPath + ".meta";
        var metaBytes = File.ReadAllBytes(metaPath);
        list.AddRange(metaBytes);
        return list.ToArray();
    }
    
    static MD5 md5 = null;
    static MD5 MD5
    {
        get
        {
            if (null == md5)
            md5 = MD5.Create();
            return md5;
        }
    }

     static string ComputeHash(byte[] buffer)
     {
         if (null == buffer || buffer.Length < 1)
                 return "";
 
         byte[] hash = MD5.ComputeHash(buffer);
         StringBuilder sb = new StringBuilder();

         foreach (var b in hash)
                 sb.Append(b.ToString("x2"));
 
         return sb.ToString();
     }
     
     public static string CalculateMD5Hash(string input)
     {
         // 创建一个MD5实例
         using (MD5 md5 = MD5.Create())
         {
             // 将输入字符串转换为字节数组并计算哈希值
             byte[] inputBytes = Encoding.UTF8.GetBytes(input);
             byte[] hashBytes = md5.ComputeHash(inputBytes);

             // 将哈希字节数组转换为十六进制字符串
             StringBuilder sb = new StringBuilder();
             for (int i = 0; i < hashBytes.Length; i++)
             {
                 sb.Append(hashBytes[i].ToString("x2"));
             }
             return sb.ToString();
         }
     }
}
