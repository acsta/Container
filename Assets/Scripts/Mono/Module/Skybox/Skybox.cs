using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class SkyboxMono : MonoBehaviour
{
    public float m_dayCycleProgress = 0f;
    public float m_dayCycleSpeed = 1f;
    public float m_dayLength = 180;
    public float m_nightLength = 180;
    public float m_currTime = 45f;
    public float m_starNebulaSpeed = 1;
    public GameObject m_Timer;
    public Texture2D m_skyDayTex;
    public Texture2D m_skyNightTex;
    public Texture2D m_skySunsetTex;
    public Texture2D m_skySunriseTex;
    void Start()
    {
        m_currTime = 45;
        
        Shader.SetGlobalTexture("_SkyDayTex", m_skyDayTex);
        Shader.SetGlobalTexture("_SkyNightTex", m_skyNightTex);
        Shader.SetGlobalTexture("_SkySunriseTex", m_skySunriseTex);
        Shader.SetGlobalTexture("_SkySunsetTex", m_skySunsetTex);
    }

    void Update()
    {
        m_currTime += Time.deltaTime * m_dayCycleSpeed;
        m_currTime %= 360;
        Shader.SetGlobalFloat("_CurrTime", m_currTime);
        
        m_dayCycleProgress = m_currTime / (m_dayLength + m_nightLength);
        m_Timer.transform.Rotate(Time.deltaTime * m_dayCycleSpeed, 0f, 0f);
        Shader.SetGlobalFloat("_DayCycleProgress", m_dayCycleProgress);
        
        Shader.SetGlobalFloat("_DayLength"       , m_dayLength);
        Shader.SetGlobalFloat("_NightLength"     , m_nightLength);
        Shader.SetGlobalFloat("_StarSpeed"       , m_starNebulaSpeed);
        Shader.SetGlobalFloat("_NebulaSpeed"     , m_starNebulaSpeed);
    }
}
