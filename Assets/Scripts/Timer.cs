using UnityEngine;
using System.Collections;

/***
 * This class provides an in game loop timer mechanism
 * Curtesy of Jeff Kasselman
 ***/

public class Timer {
    float m_timeElapsed;
    float m_totalTime;

    public Timer(float timeToCountInSec){
        m_totalTime = timeToCountInSec;
    }

    public bool UpdateAndTest(){
        m_timeElapsed += Time.deltaTime;
        return m_timeElapsed >= m_totalTime;
    }

    public float Elapsed{
        get { return Mathf.Clamp(m_timeElapsed / m_totalTime, 0, 1); }
    }
}
