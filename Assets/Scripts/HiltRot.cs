using UnityEngine;
using System.Collections;

public class HiltRot : MonoBehaviour
{

    //Inputs

    [SerializeField] private string m_rightXAxis = "RightXAxis_P1";
    [SerializeField] private string m_rightYAxis = "RightYAxis_P1";
    [SerializeField] private bool m_printDebug = false;

    //Fancy lighting of saber (and hilt........)
    private bool m_drawingLaserSaber = false;
    private bool m_sheathingLaserSaber = true;
    private float m_currentLength = 0.00f;
    private float m_maxLength = 1.05f;
    private float m_lightSpeed = 1.5f;
    private AudioSource m_hiltAudio;
    [SerializeField] private float m_volume = 0.1f;
    [SerializeField] private AudioClip m_saberOpen = null;
    [SerializeField] private AudioClip m_saberClose = null;
    [SerializeField] private Light m_flashLight = null;
    private AudioClip[] m_clashSounds;


    //Rotation controls
    private bool m_inControl = true;
    private float m_accelFactor = 0f;
    private float m_lastRot = 0f;
    private float m_currentRot = 0f;
    private float m_rotDif = 0f;
    private float m_speed = 800f;
    private float m_suspendTime = 0.06f;

    private float m_colRotDif;
    private void Start(){
        m_hiltAudio = gameObject.AddComponent<AudioSource>();
        m_hiltAudio.volume = 0.0f;
        m_hiltAudio.panStereo = (gameObject.transform.root.tag == "Player1") ? -1.0f : 1.0f;
        m_rotDif = 0f;
        transform.localScale = new Vector3(0.3768654f, m_currentLength, 0.3768654f);

        MakeClashLibrary();
    }

    
    private void OnTriggerEnter(Collider clash){
        // Maybe add CollisionEnter for flashy sparkles and crap

        if (clash.tag == "LazerTag" && m_inControl){
            m_colRotDif = clash.GetComponent<HiltRot>().m_rotDif;

            if (m_printDebug){
                Debug.Log("Hilt rotDif: " + m_rotDif);
                Debug.Log("Hilt colRotDif: " + m_colRotDif);
            }

            m_inControl = false;
            StartCoroutine(SuspendControl(m_suspendTime));
            gameObject.SendMessageUpwards("SuspendRemote", m_suspendTime);
            transform.root.BroadcastMessage("ClashFlash");
            FlashLightFlash(8f);
            ClashSounds(clash.tag);
        }
        else if (m_inControl){
            ClashSounds("notLazerTag");
            FlashLightFlash(2f);
        }
    }

    private void FixedUpdate(){
        if (m_inControl){
            m_currentRot = transform.eulerAngles.z;
            m_rotDif = m_currentRot - m_lastRot;

            m_lastRot = transform.eulerAngles.z;

            float x = Input.GetAxis(m_rightXAxis);
            float y = Input.GetAxis(m_rightYAxis);
            float angle = Mathf.Atan2(x, y) * 180 / Mathf.PI;
            Quaternion quaternionSpin = Quaternion.Euler(0, 0, angle);

            float step = m_speed * Time.deltaTime * m_accelFactor;
            m_accelFactor = Mathf.Clamp((m_accelFactor += 0.05f), 0f, 1f);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternionSpin, step);
        }
        else{
            float signOfRot = (m_rotDif == 0) ? 0 : Mathf.Sign(m_rotDif);
            float recoil = transform.eulerAngles.z - 2f * signOfRot;

            Quaternion qRecoil = Quaternion.Euler(0, 0, recoil);
            float step = m_speed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, qRecoil, step);
        }


        // LasersaberDraw
        if (Input.GetButtonDown("StartButton_P1") && gameObject.transform.root.tag == "Player1"){
            LightLightsaber();
        }
        if (Input.GetButtonDown("StartButton_P2") && gameObject.transform.root.tag == "Player2"){
            LightLightsaber();
        }
        //LighLaserSaber anim :D
        if (m_drawingLaserSaber && m_currentLength != m_maxLength){
            LightingLightsaber();
            float intensity = Mathf.Lerp(0, 3, m_currentLength);
            GetComponentInChildren<Light>().intensity = intensity;
        }
        else if (m_sheathingLaserSaber && m_currentLength > 0f){
            SheathingLightsaber();
            float intensity = Mathf.Lerp(0, 3, m_currentLength);
            GetComponentInChildren<Light>().intensity = intensity;
        }
        
        if (m_flashLight.intensity != 0f){
            Mathf.Clamp(m_flashLight.intensity -= 40 * Time.deltaTime, 0, 8);
        }
    }

    public float RotDif{
        get { return m_rotDif; }
    }

    private IEnumerator SuspendControl(float time){
        yield return new WaitForSeconds(time);
        m_inControl = true;
        m_accelFactor = 0f;
    }

    public void EquipLasersaber(GameObject chosenLasersaber){
        GameObject lasersaber = (GameObject)Instantiate(chosenLasersaber, transform.position, transform.rotation);
        lasersaber.transform.parent = transform;
    }

    private void LightLightsaber(){
        m_hiltAudio.volume = m_volume;
        if (m_drawingLaserSaber){
            m_drawingLaserSaber = false;
            m_sheathingLaserSaber = true;
            m_hiltAudio.PlayOneShot(m_saberClose);

        }
        else if (m_sheathingLaserSaber){
            m_drawingLaserSaber = true;
            m_sheathingLaserSaber = false;
            m_hiltAudio.PlayOneShot(m_saberOpen);
        }
    }

    private void LightingLightsaber(){
        if (m_currentLength <= m_maxLength){
            m_currentLength += Mathf.Clamp(m_lightSpeed * Time.deltaTime, 0f, 1f);
            transform.localScale = new Vector3(0.3768654f, m_currentLength, 0.3768654f); 
        }
        else{
            m_currentLength = m_maxLength;  
        }
    }
    private void SheathingLightsaber(){
        if (m_currentLength >= 0f){
            m_currentLength -= Mathf.Clamp(m_lightSpeed * Time.deltaTime, 0f, 1f);
            transform.localScale = new Vector3(0.3768654f, m_currentLength, 0.3768654f);
        }
        else{
            m_currentLength = 0f;
        }
    }
    public void SheatheLightsaber(){
        transform.localScale = new Vector3(1f,0f,1f);
        GetComponentInChildren<Light>().intensity = 0f;
    }

    private void MakeClashLibrary(){
        m_clashSounds = new AudioClip[] { (AudioClip)Resources.Load("hit1"), (AudioClip)Resources.Load("hit2"), (AudioClip)Resources.Load("hit3") };
    }
    private void ClashSounds(string tag){
        float clashVolume = ((tag == "LazerTag") ? 1.5f : 1.0f);
        m_hiltAudio.pitch = Random.Range(0.4f, 1.8f);
        m_hiltAudio.PlayOneShot(m_clashSounds[Random.Range(0, m_clashSounds.Length)], clashVolume);
    }

    private void FlashLightFlash( float intensity){
        m_flashLight.intensity = intensity;
    }
}
