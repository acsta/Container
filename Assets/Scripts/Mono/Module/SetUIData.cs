using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaoTie
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    [RequireComponent(typeof(Material))]
    [ExecuteAlways]
    public class SetUIData : MonoBehaviour
    {
        private Vector4 AltasData;
        private static int ID = Shader.PropertyToID("_AltasData");
        public void Start()
        {
            var Image = this.GetComponent<UnityEngine.UI.Image>();
            if(Image==null) return;
            var Material = Image.material;
            if (Material == null) return;
            // Vector4 UVRect = UnityEngine.Sprites.DataUtility.GetOuterUV(Image.sprite);
            // Rect originRect = Image.sprite.rect;
            // Rect textureRect = Image.sprite.textureRect;
            // float scaleX = textureRect.width / originRect.width;
            // float scaleY = textureRect.height / originRect.height;
            if(Image.sprite == null) return;
            AltasData = new Vector4(
                Image.sprite.textureRect.x,
                Image.sprite.textureRect.y,
                Image.sprite.textureRect.width,
                Image.sprite.textureRect.height
            );
            
            Material.SetVector(ID, AltasData);
        }

        public void Update()
        {
            var Image = this.GetComponent<UnityEngine.UI.Image>();
            if(Image==null) return;
            var Material = Image.material;

            if (Material == null) return;
            // Vector4 UVRect = UnityEngine.Sprites.DataUtility.GetOuterUV(Image.sprite);
            // Rect originRect = Image.sprite.rect;
            // Rect textureRect = Image.sprite.textureRect;
            // float scaleX = textureRect.width / originRect.width;
            // float scaleY = textureRect.height / originRect.height;
            if(Image.sprite == null) return;
            AltasData = new Vector4(
                Image.sprite.textureRect.x,
                Image.sprite.textureRect.y,
                Image.sprite.textureRect.width,
                Image.sprite.textureRect.height
            );
            
            Material.SetVector(ID, AltasData);
        }
    }
}