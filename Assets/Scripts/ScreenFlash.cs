using UnityEngine;
using System.Collections;

/*
 * Curtesy of Jeff Kasselman
*/

public class ScreenFlash : MonoBehaviour {

    private Texture2D m_pixel;
    private Color m_color = Color.white;
    private float m_startAlpha = 0.0f;
    private float m_maxAlpha = 0.2f;
    private float m_rampUpTime = 0.001f;
    private float m_holdTime = 0.001f;
    private float m_rampDownTime = 0.001f;
    private float m_chanceForFLash = 3f; // the higher the lower :D

    private enum FLASHSTATE { OFF, UP, HOLD, DOWN }

    Timer m_timer;

    FLASHSTATE m_state = FLASHSTATE.OFF;

    void Start(){
        m_pixel = new Texture2D(1, 1);
        m_color.a = m_startAlpha;
        m_pixel.SetPixel(0, 0, m_color);
        m_pixel.Apply();
    }

    public void Update(){
        switch (m_state){
            case FLASHSTATE.UP:
                if (m_timer.UpdateAndTest()){
                    m_state = FLASHSTATE.HOLD;
                    m_timer = new Timer(Mathf.Clamp(m_holdTime - Random.Range(0f,m_holdTime* m_chanceForFLash),0f,m_holdTime));
                }
                break;
            case FLASHSTATE.HOLD:
                if (m_timer.UpdateAndTest()){
                    m_state = FLASHSTATE.DOWN;
                    m_timer = new Timer(Mathf.Clamp(m_rampDownTime - Random.Range(0f, m_holdTime * m_chanceForFLash), 0f, m_rampDownTime));
                }
                break;
            case FLASHSTATE.DOWN:
                if (m_timer.UpdateAndTest()){
                    m_state = FLASHSTATE.OFF;
                    m_timer = null;
                }
                break;
        }
    }

    private void SetPixelAlpha(float a){
        m_color.a = a;
        m_pixel.SetPixel(0, 0, m_color);
        m_pixel.Apply();
    }

    private void OnGUI(){
        switch (m_state){
            case FLASHSTATE.UP:
                SetPixelAlpha(Mathf.Lerp(m_startAlpha, m_maxAlpha, m_timer.Elapsed));
                break;
            case FLASHSTATE.DOWN:
                SetPixelAlpha(Mathf.Lerp(m_maxAlpha, m_startAlpha, m_timer.Elapsed));
                break;
            case FLASHSTATE.OFF:
                SetPixelAlpha(0f);
                break;
        }
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_pixel);
    }

    public void ClashFlash(){
        m_timer = new Timer(Mathf.Clamp(m_rampUpTime - Random.Range(0f, m_rampUpTime * m_chanceForFLash), 0f, m_rampUpTime));
        m_state = FLASHSTATE.UP;
        //Debug.Log("Flash Called from Clash");
    }

}

