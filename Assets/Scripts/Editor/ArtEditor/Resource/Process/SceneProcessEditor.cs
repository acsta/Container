#if ODIN_INSPECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace TaoTie
{

    public class SceneProcessEditor: OdinEditorWindow
    {
        private const string AssetsPackage = "Assets/AssetsPackage/";
        private const string BasePath = AssetsPackage + "Scenes/";

        
        [MenuItem("Tools/工具/TA/场景入库", false, 161)]
        public static void GeneratingAtlas()
        {
            GetWindow(typeof(SceneProcessEditor));
        }
        
        public SceneAsset[] Scenes = new SceneAsset[0];

        [Button("入库")]
        public void Process()
        {
            for (int i = 0; i < Scenes.Length; i++)
            {
                if(Scenes[i] == null) continue;
                CopyScene(Scenes[i]);
            }
        }

        static void CopyScene(SceneAsset sceneAsset)
        {
            var path = AssetDatabase.GetAssetPath(sceneAsset);
            string subPath = sceneAsset.name + "Scene";
            var dir = BasePath + subPath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string target = dir + "/" + sceneAsset.name + ".unity";
            File.Copy(path,target,true);
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(target);
            
            Dictionary<GameObject,GameObject> scenePrefabs = new Dictionary<GameObject,GameObject>();
            var deps = AssetDatabase.GetDependencies(target);
            for (int i = 0; i < deps.Length; i++)
            {
                if (deps[i].StartsWith(BasePath)) continue;
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(deps[i]);
                if (prefab != null && PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab)
                {
                    scenePrefabs.Add(prefab, null);
                }
                    
            }
            
            ProcessHelper.CreateDir(AssetType.SceneObject, subPath);
            var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            var count = roots.Length;
            
            for (int i = 0; i < count; i++)
            {
                ProcessPrefab(roots[i].transform, scenePrefabs, subPath);
            }
            
            List<Transform> npcs = new List<Transform>();
            for (int i = 0; i < count; i++)
            {
                ProcessSceneObj(roots[i].transform, scenePrefabs, subPath, npcs);
            }

            int hoster = 0;
            var rc = FindObjectOfType<ReferenceCollector>();
            if (rc != null)
            {
                for (int i = 0; i < npcs.Count; i++)
                {
                    var euler = npcs[i].transform.rotation.eulerAngles;
                    bool ishoster = euler.y > 145 && euler.y < 225;
                    if (ishoster) hoster++;
                    rc.Add(ishoster ? "Host" : "Npc_" + (i - hoster), npcs[i]);
                }
                
            }
            
            if (Lightmapping.lightingSettings != null)
            {
                LightingSettings asset = GameObject.Instantiate(Lightmapping.lightingSettings);
                string lightDataPath = dir + "/" + sceneAsset.name + "/" + Lightmapping.lightingSettings.name + ".lighting";
                
                if (!Directory.Exists(dir + "/" + sceneAsset.name))
                {
                    Directory.CreateDirectory(dir + "/" + sceneAsset.name);
                }
                AssetDatabase.CreateAsset(asset, lightDataPath);
                Lightmapping.SetLightingSettingsForScene(scene,asset);
                
            }
            Lightmapping.ClearLightingDataAsset();
            
            if (RenderSettings.skybox != null)
            {
                RenderSettings.skybox = ProcessHelper.FindAssets(RenderSettings.skybox, AssetType.SceneObject, subPath,
                    "Materials", ".mat");
            }
            
            Lightmapping.BakeAsync();

            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveOpenScenes();
        }

        private static void ProcessPrefab(Transform transform, Dictionary<GameObject, GameObject> map, string subPath)
        {
            if (transform.gameObject.name == "1111")
            {
                return;
            }
            if (!transform.gameObject.activeSelf)
            {
                return;
            }
            if (transform.GetComponent<Canvas>() != null ||
                transform.GetComponent<EventSystem>() != null)
            {
                return;
            }
            if (transform.GetComponent<LensFlareComponentSRP>() is LensFlareComponentSRP lensFlare && 
                lensFlare != null && lensFlare.lensFlareData != null)
            {
                return;
            }
            
            if (transform.GetComponent<Volume>() is Volume volume && volume != null)
            {
                return;
            }

            if (transform.GetComponent<Camera>() is Camera aCamera && aCamera != null)
            {
                return;
            }
            
            if (transform.GetComponent<Animator>() is Animator animator && animator != null && animator.avatar != null)
            {
                return;
            }

            var type = PrefabUtility.GetPrefabAssetType(transform.gameObject);
            if (type != PrefabAssetType.NotAPrefab)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(transform.gameObject);
                if (prefab != null && map.TryGetValue(prefab, out var newPrefab))
                {
                    if (newPrefab == null)
                    {
                        newPrefab = ProcessHelper.ProcessPrefab(transform.gameObject, prefab, AssetType.SceneObject, subPath);
                        if (newPrefab == null)
                        {
                            map.Remove(prefab);
                            return;
                        }
                        map[prefab] = newPrefab;
                    }
                }
            }
            else
            {
                var count = transform.childCount;
                for (int i = count - 1; i>=0; i--)
                {
                    ProcessPrefab(transform.GetChild(i), map, subPath);
                }
            }
        }

        private static void ProcessSceneObj(Transform transform, Dictionary<GameObject, GameObject> map, string subPath,
            List<Transform> npcs)
        {
            if (transform.gameObject.name == "1111")
            {
                DestroyImmediate(transform.gameObject);
                return;
            }
            if (!transform.gameObject.activeSelf)
            {
                DestroyImmediate(transform.gameObject);
                return;
            }
            if (transform.GetComponent<Canvas>() != null ||
                transform.GetComponent<EventSystem>() != null)
            {
                GameObject.DestroyImmediate(transform.gameObject);
                return;
            }
            
            if (transform.GetComponent<LensFlareComponentSRP>() is LensFlareComponentSRP lensFlare && 
                lensFlare != null && lensFlare.lensFlareData != null)
            {
                lensFlare.lensFlareData = 
                    ProcessHelper.FindAssets(lensFlare.lensFlareData, AssetType.SceneObject, subPath, "Others");
                return;
            }
            
            if (transform.GetComponent<Skybox>() is Skybox skybox)
            {
                GameObject.DestroyImmediate(skybox);
            }
            
            if (transform.GetComponent<Volume>() is Volume volume && volume != null)
            {
                volume.sharedProfile =
                    AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/AssetsPackage/RenderAssets/Global Volume Profile.asset");
                return;
            }

            if (transform.GetComponent<Camera>() is Camera aCamera && aCamera != null)
            {
                if (aCamera.GetUniversalAdditionalCameraData()?.renderType == CameraRenderType.Base)
                {
                    aCamera.GetUniversalAdditionalCameraData()?.SetRenderer(1);
                    aCamera.tag = "MainCamera";
                    if (transform.GetComponent<ReferenceCollector>() == null)
                    {
                        transform.gameObject.AddComponent<ReferenceCollector>();
                    }
                }
                else
                {
                    GameObject.DestroyImmediate(transform.gameObject);
                }
                return;
            }

            
            if (transform.name == "CAR" || transform.name == "car") return;
            if (transform.GetComponent<Animator>() is Animator animator && animator != null && animator.avatar != null)
            {
                var index = transform.GetSiblingIndex();
                GameObject instance = new GameObject("Npc_" + index);
                instance.transform.SetParent(transform.parent);
                instance.transform.SetSiblingIndex(index);
                instance.transform.localPosition = transform.localPosition;
                instance.transform.localRotation = transform.localRotation;
                instance.transform.localScale = transform.localScale;
                transform.SetAsLastSibling();
                instance.gameObject.SetActive(transform.gameObject.activeSelf);
                DestroyImmediate(transform.gameObject);
                npcs.Add(instance.transform);
                return;
            }

            var type = PrefabUtility.GetPrefabAssetType(transform.gameObject);
            if (type != PrefabAssetType.NotAPrefab)
            {
                var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(transform.gameObject);
                if (prefab != null && map.TryGetValue(prefab, out var newPrefab))
                {
                    GameObject instance = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;
                    var index = transform.GetSiblingIndex();
                    instance.transform.SetParent(transform.parent);
                    instance.transform.SetSiblingIndex(index);
                    instance.transform.localPosition = transform.localPosition;
                    instance.transform.localRotation = transform.localRotation;
                    instance.transform.localScale = transform.localScale;
                    transform.SetAsLastSibling();
                    PrecessBakedShadowData(instance.transform, transform);
                    instance.gameObject.SetActive(transform.gameObject.activeSelf);
                    DestroyImmediate(transform.gameObject);
                }
            }
            else
            {
                ProcessHelper.CreateComponentMesh(transform.gameObject, AssetType.SceneObject, subPath);
                ProcessHelper.CreateComponentMaterials(transform.gameObject, transform.gameObject, AssetType.SceneObject, subPath);
                var count = transform.childCount;
                for (int i = count - 1; i>=0; i--)
                {
                    ProcessSceneObj(transform.GetChild(i), map, subPath,npcs);
                }
            }
        }

        private static void PrecessBakedShadowData(Transform newObj, Transform oldObj)
        {
            Renderer rendererOld = oldObj.GetComponent<Renderer>();
            Renderer rendererNew = newObj.GetComponent<Renderer>();
            
            if (rendererOld != null && rendererNew != null)
            {
                rendererNew.receiveShadows = rendererOld.receiveShadows;
                rendererNew.shadowCastingMode = rendererOld.shadowCastingMode;
                rendererNew.rendererPriority = rendererOld.rendererPriority;
                rendererNew.renderingLayerMask = rendererOld.renderingLayerMask;
                rendererNew.sortingOrder = rendererOld.sortingOrder;
                rendererNew.sortingLayerID = rendererOld.sortingLayerID;
                rendererNew.motionVectorGenerationMode = rendererOld.motionVectorGenerationMode;
                rendererNew.lightProbeProxyVolumeOverride = rendererOld.lightProbeProxyVolumeOverride;
                rendererNew.staticShadowCaster = rendererOld.staticShadowCaster;
                rendererNew.reflectionProbeUsage = rendererOld.reflectionProbeUsage;
                var flags = GameObjectUtility.GetStaticEditorFlags(oldObj.gameObject);
                GameObjectUtility.SetStaticEditorFlags(newObj.gameObject, flags);
            }
            
            for (int i = 0; i < newObj.childCount; i++)
            {
                PrecessBakedShadowData(newObj.GetChild(i), oldObj.GetChild(i));
            }
        }
    }
    
    
}

#endif