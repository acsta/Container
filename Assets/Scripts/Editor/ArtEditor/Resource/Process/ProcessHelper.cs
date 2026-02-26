using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TaoTie
{
    public enum AssetType
    {
        Unit,
        SceneObject,
        Effect,
        AnimatorClip,
    }

    public static class ProcessHelper
    {
        public static MD5 md5 = MD5.Create();
        public const string BasePath = "Assets/AssetsPackage/";
        public const string BuildInPath = "Library/unity default resources";
        
        public static GameObject ProcessPrefab(GameObject instance, GameObject prefab, AssetType assetType, string subPath, string typeName = null,
            string label = null)
        {
            if (prefab == null) return null;
            if (string.IsNullOrEmpty(subPath))
            {
                subPath = prefab.name;
            }
            

            CreateDir(assetType, subPath);
            if (assetType == AssetType.AnimatorClip)
            {
                CreateAnimatorClip(prefab, subPath);
            }
            else
            {
                GameObject obj = CreatePrefab(prefab, assetType, subPath);
                CreateComponentMesh(obj, assetType, subPath);
                CreateComponentMaterials(instance, obj, assetType, subPath);
                CreateComponentAvatar(obj, assetType, subPath);
                if (string.IsNullOrEmpty(typeName)) typeName = "场景内资源";
                if (string.IsNullOrEmpty(label)) label = subPath;
                if (assetType == AssetType.SceneObject)
                {
                    var config = AssetDatabase.LoadAssetAtPath<AssetsManagerConfig>(AssetsManagerConfig.ConfigPath);
                    for (int i = 0; i < config.Labels.Count; i++)
                    {
                        if (config.Labels[i].Label == typeName)
                        {
                            for (int j = 0; j < config.Labels[i].Collects.Count; j++)
                            {
                                if (config.Labels[i].Collects[j].Label == label)
                                {
                                    config.Labels[i].Collects[j].Objects.Add(obj);
                                    OnSaveConfig(config);
                                    return obj;
                                }
                            }

                            var collect = new CollectConfig()
                            {
                                Label = label,
                                Objects = new List<Object>()
                            };
                            collect.Objects.Add(obj);
                            config.Labels[i].Collects.Add(collect);
                            OnSaveConfig(config);
                            return obj;
                        }
                    }

                    var labelConfig = new LabelConfig()
                    {
                        Collects = new List<CollectConfig>(),
                        Label = typeName
                    };
                    var collect2 = new CollectConfig()
                    {
                        Label = label,
                        Objects = new List<Object>()
                    };
                    collect2.Objects.Add(obj);
                    labelConfig.Collects.Add(collect2);
                    config.Labels.Add(labelConfig);
                    OnSaveConfig(config);
                }

                return obj;
            }

            return null;
        }

        public static GameObject[] ProcessPrefabs(GameObject[] prefabs, AssetType assetType, string subPath,
            string typeName = null, string label = null)
        {
            GameObject[] res = new GameObject[prefabs.Length];
            List<GameObject> objs = new List<GameObject>();
            for (int i = 0; i < prefabs.Length; i++)
            {
                var prefab = prefabs[i];
                if (prefab == null) continue;
                if (string.IsNullOrEmpty(subPath))
                {
                    subPath = prefab.name;
                }

                CreateDir(assetType, subPath);
                if (assetType == AssetType.AnimatorClip)
                {
                    CreateAnimatorClip(prefab, subPath);
                }
                else
                {
                    GameObject obj = CreatePrefab(prefab, assetType, subPath);
                    CreateComponentMesh(obj, assetType, subPath);
                    CreateComponentMaterials(obj, obj, assetType, subPath);
                    
                    CreateComponentAvatar(obj, assetType, subPath);
                    objs.Add(obj);
                    res[i] = obj;
                }
            }

            if (assetType == AssetType.SceneObject)
            {
                if (string.IsNullOrEmpty(typeName)) typeName = "场景内资源";
                if (string.IsNullOrEmpty(label)) label = subPath;
                var config = AssetDatabase.LoadAssetAtPath<AssetsManagerConfig>(AssetsManagerConfig.ConfigPath);
                for (int i = 0; i < config.Labels.Count; i++)
                {
                    if (config.Labels[i].Label == typeName)
                    {
                        for (int j = 0; j < config.Labels[i].Collects.Count; j++)
                        {
                            if (config.Labels[i].Collects[j].Label == label)
                            {
                                config.Labels[i].Collects[j].Objects.AddRange(objs);
                                OnSaveConfig(config);
                                return res;
                            }
                        }

                        var collect = new CollectConfig()
                        {
                            Label = label,
                            Objects = new List<Object>()
                        };
                        collect.Objects.AddRange(objs);
                        config.Labels[i].Collects.Add(collect);
                        OnSaveConfig(config);
                        return res;
                    }
                }

                var labelConfig = new LabelConfig()
                {
                    Collects = new List<CollectConfig>(),
                    Label = typeName
                };
                var collect2 = new CollectConfig()
                {
                    Label = label,
                    Objects = new List<Object>()
                };
                collect2.Objects.AddRange(objs);
                labelConfig.Collects.Add(collect2);
                config.Labels.Add(labelConfig);
                OnSaveConfig(config);

            }

            return res;
        }

        public static void OnSaveConfig(AssetsManagerConfig config)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssetIfDirty(config);
            AssetDatabase.Refresh();
        }

        public static void CreateDir(AssetType assetType, string subPath)
        {
            var path = BasePath + assetType + "/" + subPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            FileHelper.CreateArtSubFolder(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static GameObject CreatePrefab(GameObject prefab, AssetType assetType, string subPath)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            if (path.Contains(BasePath + assetType))
            {
                return prefab;
            }

            var basePath = BasePath + assetType + "/" + subPath + "/Prefabs/" + prefab.name + ".prefab";
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = prefab.name;
            CleanupMissingScripts(obj);
            var res = PrefabUtility.SaveAsPrefabAsset(obj, basePath);
            GameObject.DestroyImmediate(obj);

            return res;
        }
        public static void CleanupMissingScripts(GameObject gameObject)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(true);
            for (int j = 0; j < transforms.Length; j++)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transforms[j].gameObject);
            }
        }
        public static void CreateComponentMesh(GameObject gameObject, AssetType assetType, string subPath)
        {
            bool dirty = false;
            var meshs = gameObject.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < meshs.Length; i++)
            {
                if(meshs[i].sharedMesh == null) continue;
                if (AssetDatabase.GetAssetPath(meshs[i].sharedMesh).Contains(BuildInPath)) continue;
                var newMesh = FindAssets(meshs[i].sharedMesh, assetType, subPath, "Models", ".asset");
                dirty = meshs[i].sharedMesh != newMesh;
                meshs[i].sharedMesh =  newMesh;
            }

            var colliders = gameObject.GetComponentsInChildren<MeshCollider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                if(colliders[i].sharedMesh == null) continue;
                if (AssetDatabase.GetAssetPath(colliders[i].sharedMesh).Contains(BuildInPath)) continue;
                var newMesh = FindAssets(colliders[i].sharedMesh, assetType, subPath, "Models", ".asset");
                dirty = colliders[i].sharedMesh != newMesh;
                colliders[i].sharedMesh =  newMesh;
            }

            var skinedMeshs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < skinedMeshs.Length; i++)
            {
                if(skinedMeshs[i].sharedMesh == null) continue;
                if (AssetDatabase.GetAssetPath(skinedMeshs[i].sharedMesh).Contains(BuildInPath)) continue;
                var newMesh = FindAssets(skinedMeshs[i].sharedMesh, assetType, subPath, "Models", ".asset");
                dirty = skinedMeshs[i].sharedMesh != newMesh;
                skinedMeshs[i].sharedMesh =  newMesh;
            }

            if (dirty)
            {
                EditorUtility.SetDirty(gameObject);
            }
        }

        public static void CreateComponentMaterials(GameObject instance, GameObject gameObject, AssetType assetType, string subPath)
        {
            bool dirty = false;
            var meshs = gameObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshs.Length; i++)
            {
                bool isdirty = false;
                List<Material> materials = new List<Material>();
                for (int j = 0; j < meshs[i].sharedMaterials.Length; j++)
                {
                    Material instanceMat = null;
                    if (instance.transform.childCount != 0)
                    {
                        instanceMat = instance.GetComponentsInChildren<MeshRenderer>()[i].sharedMaterials[j];   
                    }
                    else
                    {
                        instanceMat = instance.GetComponent<MeshRenderer>().sharedMaterials[j];   
                    }
                    //instanceMat = instance.GetComponent<MeshRenderer>().sharedMaterials[j];   
                    //meshs[i].sharedMaterials[j] = meshs[i].sharedMaterials[j] != instanceMat ? instanceMat : meshs[i].sharedMaterials[j];
                    
                    var newMat = FindAssets(instanceMat, assetType, subPath, "Materials", ".mat");
                    isdirty = meshs[i].sharedMaterials[j] != newMat;
                    materials.Add(newMat);
                }

                if (isdirty)
                {
                    meshs[i].SetSharedMaterials(materials);
                    dirty = true;
                }
            }

            var skinedMeshs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < skinedMeshs.Length; i++)
            {
                bool isdirty = false;
                List<Material> materials = new List<Material>();
                for (int j = 0; j < skinedMeshs[i].sharedMaterials.Length; j++)
                {
                    var newMat = FindAssets(skinedMeshs[i].sharedMaterials[j], assetType, subPath, "Materials", ".mat");
                    isdirty = skinedMeshs[i].sharedMaterials[j] != newMat;
                    materials.Add(newMat);
                }

                if (isdirty)
                {
                    skinedMeshs[i].SetSharedMaterials(materials);
                    dirty = true;
                }
            }

            if (dirty)
            {
                EditorUtility.SetDirty(gameObject);
            }
        }
        
        private static void CreateMaterialsTexture(Material newMat, AssetType assetType, string subPath)
        {
            Shader shader = newMat.shader;
            for (int ii = 0; ii < ShaderUtil.GetPropertyCount(shader); ++ii)
            {
                if (ShaderUtil.GetPropertyType(shader, ii) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, ii);
                    Texture tex = newMat.GetTexture(propertyName);
                    var newtex = FindAssets(tex, assetType, subPath, "Textures");
                    newMat.SetTexture(propertyName, newtex);
                }
            }
        }

        public static void CreateComponentAvatar(GameObject gameObject, AssetType assetType, string subPath)
        {
            bool dirty = false;
            var animators = gameObject.GetComponentsInChildren<Animator>(true);
            for (int i = 0; i < animators.Length; i++)
            {
                if (animators[i].avatar != null)
                {
                    var newAvatar = FindAssets(animators[i].avatar, assetType, subPath, "Animations", ".asset");
                    dirty = newAvatar != animators[i].avatar;
                    animators[i].avatar = newAvatar;
                }
            }

            if (dirty)
            {
                EditorUtility.SetDirty(gameObject);
            }
        }

        private static void CreateAnimatorClip(GameObject prefab, string subPath)
        {
            var basePath = BasePath + AssetType.Unit + "/" + subPath + "/Animations/";
            FbxHelperWindow.HandleFBX(AssetDatabase.GetAssetPath(prefab), basePath);
        }

        public static string GetExportName(Object obj)
        {
            byte[] input = Encoding.Default.GetBytes(AssetDatabase.GetAssetPath(obj) + obj.name);
            byte[] output = md5.ComputeHash(input);
            string md5Str = BitConverter.ToString(output).Replace("-", "");
            return md5Str;
        }

        public static T FindAssets<T>(T oldObj, AssetType assetType, string subPath, string assetsType, string ext = null) where T : Object
        {
            if (oldObj == null) return oldObj;
            var assetPath = AssetDatabase.GetAssetPath(oldObj);
            if( assetPath.Contains(BuildInPath)) return oldObj;
            if (assetPath.Contains(BasePath)) return oldObj;
            if (ext == null)
            {
                var vs = assetPath.Split('.');
                ext = "."+vs[vs.Length-1];
            }
            var basePath = BasePath + assetType;
            var dir = new DirectoryInfo(basePath);
            var subs = dir.GetDirectories();
            Object newOther = null;
            var commonPath = basePath + "/Common/" + assetsType +"/"+GetExportName(oldObj)+ext;
            for (int i = 0; i < subs.Length && newOther ==null; i++)
            {
                var exportName = basePath + "/" + subs[i].Name + "/" + assetsType +"/"+GetExportName(oldObj)+ext;
                newOther = AssetDatabase.LoadAssetAtPath<T>(exportName);
                if (newOther != null)
                {
                    if (subs[i].Name != subPath && subs[i].Name != "Common")
                    {
                        AssetDatabase.MoveAsset(exportName, commonPath);
                        if (oldObj is Material mat)
                        {
                            Shader shader = mat.shader;
                            for (int ii = 0; ii < ShaderUtil.GetPropertyCount(shader); ++ii)
                            {
                                if (ShaderUtil.GetPropertyType(shader, ii) == ShaderUtil.ShaderPropertyType.TexEnv)
                                {
                                    string propertyName = ShaderUtil.GetPropertyName(shader, ii);
                                    Texture tex = mat.GetTexture(propertyName);
                                    var path = AssetDatabase.GetAssetPath(tex);
                                    if (path.Contains("/Textures/") && !path.Contains("/Common/"))
                                    {
                                        var commonPath2 = basePath + "/Common/Textures/" + GetExportName(tex) + ext;
                                        AssetDatabase.MoveAsset(path, commonPath2);
                                    }
                                }
                            }
                        }
                        AssetDatabase.Refresh();
                    }
                }
            }
            
            if (newOther == null)
            {
                var exportName = basePath + "/" + subPath + "/" + assetsType + "/" + GetExportName(oldObj) + ext;
                if (oldObj is Texture)
                {
                    AssetDatabase.CopyAsset(assetPath, exportName);
                    newOther = AssetDatabase.LoadAssetAtPath<T>(exportName);
                }
                else if (oldObj is Material)
                {
                    newOther = Object.Instantiate(oldObj);
                    CreateMaterialsTexture(newOther as Material, assetType, subPath);
                    AssetDatabase.CreateAsset(newOther, exportName);
                }
                else
                {
                    newOther = Object.Instantiate(oldObj);
                    AssetDatabase.CreateAsset(newOther, exportName);
                }
            }
            return newOther as T;
        }
    }
}