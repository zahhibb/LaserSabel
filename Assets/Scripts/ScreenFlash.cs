using UnityEngine;
using System.Collections;

/*
 * Curtesy of Jeff Kasselman
*/

public class ScreenFlash : MonoBehaviour {

    private Texture2D pixel;
    public Color color = Color.white;
    public float startAlpha = 0.0f;
    public float maxAlpha = 0.2f;
    public float rampUpTime = 0.001f;
    public float holdTime = 0.001f;
    public float rampDownTime = 0.001f;
    private float chanceForFLash = 3f; // the higher the lower :D

    enum FLASHSTATE { OFF, UP, HOLD, DOWN }

    Timer timer;

    FLASHSTATE state = FLASHSTATE.OFF;


    // Use this for initialization
    void Start()
    {
        pixel = new Texture2D(1, 1);
        color.a = startAlpha;
        pixel.SetPixel(0, 0, color);
        pixel.Apply();
        // for testing
        //TookDamage(new DamagePacket(1));
    }

    public void Update()
    {
        switch (state)
        {
            case FLASHSTATE.UP:
                if (timer.UpdateAndTest())
                {
                    state = FLASHSTATE.HOLD;
                    timer = new Timer(Mathf.Clamp(holdTime - Random.Range(0f,holdTime* chanceForFLash),0f,holdTime));
                }
                break;
            case FLASHSTATE.HOLD:
                if (timer.UpdateAndTest())
                {
                    state = FLASHSTATE.DOWN;
                    timer = new Timer(Mathf.Clamp(rampDownTime - Random.Range(0f, holdTime * chanceForFLash), 0f, rampDownTime));
                }
                break;
            case FLASHSTATE.DOWN:
                if (timer.UpdateAndTest())
                {
                    state = FLASHSTATE.OFF;
                    timer = null;
                }
                break;
        }
        /*
        *
        if (Input.GetButtonDown("MoveForwardButton_P1"))
        {
            ClashFlash();
            Debug.Log("Flash!");
        }
        */
    }

    private void SetPixelAlpha(float a)
    {
        color.a = a;
        pixel.SetPixel(0, 0, color);
        pixel.Apply();
    }

    public void OnGUI()
    {
        switch (state)
        {
            case FLASHSTATE.UP:
                SetPixelAlpha(Mathf.Lerp(startAlpha, maxAlpha, timer.Elapsed));
                break;
            case FLASHSTATE.DOWN:
                SetPixelAlpha(Mathf.Lerp(maxAlpha, startAlpha, timer.Elapsed));
                break;
            case FLASHSTATE.OFF:
                SetPixelAlpha(0f);
                break;
        }
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), pixel);
    }

    public void ClashFlash()
    {
        timer = new Timer(Mathf.Clamp(rampUpTime - Random.Range(0f, rampUpTime * chanceForFLash), 0f, rampUpTime));
        state = FLASHSTATE.UP;
        //Debug.Log("Flash Called from Clash");
    }

}

