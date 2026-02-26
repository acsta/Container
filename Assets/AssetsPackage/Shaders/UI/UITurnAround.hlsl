#ifndef UI_TurnAround_h
#define UI_TurnAround_h

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

SamplerState _Sampler_ClampU_ClampV_Linear;
SamplerState _Sampler_ClampU_ClampV_Point;
SamplerState _Sampler_RepeatU_RepeatV_Linear;
SamplerState _Sampler_RepeatU_ClampV_Linear;
SamplerState _Sampler_ClampU_RepeatV_Point;
SamplerState _Sampler_RepeatU_RepeatV_Point;
SamplerState _Sampler_RepeatU_ClampV_Point;
Texture2D<half4> _MainTex;
Texture2D<half4> _NoiseTex;
float4 _NoiseTex_ST;
half _RotateNoise;
half _MaskRadius;
half _OutlineRadius;
half _DisturbIntensity;
half _DisturbSpeed;
half3 _OutlineColor;

struct VSInput
{
    float2 uv : TEXCOORD0;
    
    float4 positionOS : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct PSInput
{
    float2 uv : TEXCOORD0;

    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_OUTPUT_STEREO
};
struct PSOutput
{
    float4 target0 : SV_TARGET0;
};

float3 Unity_RotateAboutAxis_Degrees_float(float3 In, float3 Axis, float Rotation)
{
    Rotation = radians(Rotation);
    float s = sin(Rotation);
    float c = cos(Rotation);
    float one_minus_c = 1.0 - c;

    Axis = normalize(Axis);
    float3x3 rot_mat = 
    {   one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
        one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
        one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
    };
    float3 Out = mul(rot_mat,  In);
                
    return Out;
}

void VSTurnAround(VSInput i, out PSInput o)
{
    o = (PSInput)0;
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.positionCS = TransformObjectToHClip(i.positionOS);

    o.uv = i.uv;
    #if defined (UNITY_UV_STARTS_AT_TOP)
    if(_ProjectionParams.x < 0.f)
    {
        //o.uv.y = 1 - o.uv.y;
    }
    #endif
}

void PSTurnAround(PSInput i, out PSOutput o)
{
    o = (PSOutput)0;

    half2 center = half2(0.5, 0.5);
    half2 uvC = i.uv;
    half cosAngle = cos(_RotateNoise);
    half sinAngle = sin(_RotateNoise);
    half2x2 rot = half2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
    uvC -= center;
    float2 uv = mul(rot, uvC);
    uv += center;

    float3 noiseUV = _NoiseTex.Sample(_Sampler_RepeatU_RepeatV_Linear, i.uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw + _SinTime.y * 0.1 * _DisturbSpeed) * _DisturbIntensity;
    uv += float2(noiseUV.x, noiseUV.y);

    half4 mainColor = _MainTex.Sample(_Sampler_ClampU_ClampV_Linear, i.uv);

    float mask = distance(float2(0.5, 0.5), uv);
    float maskRadius = _MaskRadius * 1.1 - 0.1;
    float maskOutline = step(maskRadius, mask) - step(maskRadius + _OutlineRadius, mask);
    half4 outline = float4(maskOutline * _OutlineColor, maskOutline);
    mask = 1 - step(maskRadius, mask);
    
    o.target0 = float4(mainColor.rgb + outline, saturate(1 - mask - maskOutline * 0.1));
}
#endif