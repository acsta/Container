Shader "Custom/S_UITurnAround"
{
    Properties
    {
        [IntRange] _StencilRef("Stencil Ref", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilTestCompare("Stencil Test Compare", Int) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPassOp("Stencil Pass Operator", Int) = 0
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    	_NoiseTex("Noise Tex", 2D) = "black" {}
    	_RotateNoise("Rotate Noise", Range(0, 4)) = 0
    	_DisturbIntensity("Disturb Intensity", Range(0, 1)) = 0.1
    	_DisturbSpeed("Disturb Speed", Range(0, 1)) = 0.1
    	_MaskRadius("Mask Radius", Range(0, 1)) = 1
    	_OutlineRadius("Outline Radius", Range(0, 1)) = 0.1
    	[HDR]_OutlineColor("Outline Color", Color) = (1,1,1)
    }
    SubShader
    {
        Tags
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
		}
        Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilTestCompare]
                Pass [_StencilPassOp]
            }
            
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex VSTurnAround
            #pragma fragment PSTurnAround

            #include "UITurnAround.hlsl"
            
            ENDHLSL
        }
    }
}
