using UnityEngine;
using System.Collections;

public class LazerHum : MonoBehaviour
{
    private AudioSource m_lowSource;
    private AudioSource m_midSource;
    private float m_humPitch;
    [SerializeField] private AudioClip m_lowHum;
    [SerializeField] private AudioClip m_midHum;
    private float m_volume = 0.15f;

    private void Start(){
        m_humPitch = Random.Range(0.0f, 0.3f);

        m_lowSource = gameObject.AddComponent<AudioSource>();

        m_lowSource.loop = true;
        m_lowSource.playOnAwake = false;
        m_lowSource.clip = m_lowHum;
        m_lowSource.pitch = 1.0f - m_humPitch;
        m_lowSource.volume = 0.0f;
        m_lowSource.panStereo = (gameObject.transform.root.tag == "Player1") ? -1.0f : 1.0f;

        m_midSource = gameObject.AddComponent<AudioSource>();

        m_midSource.loop = true;
        m_midSource.playOnAwake = false;
        m_midSource.clip = m_midHum;
        m_midSource.pitch = 1.25f - m_humPitch;
        m_midSource.volume = 0.0f;
        m_midSource.panStereo = (gameObject.transform.root.tag == "Player1") ? -1.0f : 1.0f;
    }

    private void Update(){
        if(!m_lowSource.isPlaying){
            m_lowSource.Play();
        }
        if (!m_midSource.isPlaying){
            m_midSource.Play();
        }
        
        float filterFactor = Mathf.InverseLerp(0, 180, Mathf.Abs(transform.rotation.eulerAngles.z - 180));
        float drawnSaberVolume = Mathf.Clamp(transform.localScale.y, 0f, 1f);

        m_lowSource.volume = Mathf.Clamp(drawnSaberVolume - filterFactor, 0f, 1f) * m_volume;
        m_midSource.volume = Mathf.Clamp(drawnSaberVolume - Mathf.Abs(filterFactor-1), 0f, 1f) * m_volume; 
    }
}
