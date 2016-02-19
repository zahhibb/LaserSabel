using UnityEngine;
using System.Collections;

public class ShoulderRot : MonoBehaviour
{
    private float m_suspendTime = 0.06f;
    private bool m_inControl = true;

    [SerializeField] private bool m_printDebug = true;
    [SerializeField] private float m_speed = 600f;
    [SerializeField] private string m_leftXAxis = "LeftXAxis_P1";
    [SerializeField] private string m_leftYAxis = "LeftYAxis_P1";
    [SerializeField] private string m_triggerAxis = "TriggerAxis_P1";

    private float m_arc = 0;
    private float m_accelFactor = 0f;
    private float m_lastRot = 0f;
    private float m_currentRot = 0f;
    private float m_lastArc = 0f;
    private float m_currentArc = 0f;
    private float m_rotDif = 0f;
    private float m_arcDif = 0f;

    private void Update (){
        if (m_inControl){
            m_currentArc = transform.eulerAngles.x;
            m_arcDif = m_currentArc - m_lastArc;
            m_currentRot = transform.eulerAngles.y;
            m_rotDif = m_currentRot - m_lastRot;           
            m_lastArc = transform.eulerAngles.x;
            m_lastRot = transform.eulerAngles.y;

            float x = Input.GetAxis(m_leftXAxis);
            float y = Input.GetAxis(m_leftYAxis);
            float angle = Mathf.Atan2(x, y) * 180f / Mathf.PI;
            float triggerAxis = Input.GetAxis(m_triggerAxis);            

            if (triggerAxis < 0f){
                m_arc = Mathf.Lerp(-50f, 0f, Mathf.Abs(triggerAxis+1));
            }
            else if (triggerAxis > 0){
                m_arc = Mathf.Lerp(0f, 50f, Mathf.Abs(triggerAxis));
            }
            else{
                m_arc = 0.0f;
            }

            Quaternion quaternionRotate = Quaternion.Euler(0f, angle, 0f);
            Quaternion quaternionArc = Quaternion.AngleAxis((m_arc), Vector3.left);
            Quaternion quaternionCombined = quaternionRotate * quaternionArc;

            float step = m_speed * Time.deltaTime * m_accelFactor;
            m_accelFactor = Mathf.Clamp((m_accelFactor += 0.05f), 0f, 1f);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternionCombined, step);
        }
        else{            
            float recoil = transform.eulerAngles.y - m_rotDif;            
            float arcRecoil = transform.eulerAngles.x - m_arcDif;
            
            Quaternion qRecoil = Quaternion.Euler(0f, recoil, 0f);
            Quaternion qArc = Quaternion.Euler(arcRecoil, 0f, 0f);
            Quaternion qCombined = qRecoil * qArc;
            float step = m_speed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, qCombined, step);

            if (m_printDebug){
                Debug.Log("shoulderRotDIf: " + m_rotDif);
                Debug.Log("Shoulder recoil: " + recoil);
                Debug.Log("shoulder qCombined: " + qCombined);
            }
        }
	}

    public float RotDif{
        get {return m_rotDif; }
    }

    public float ArcDif{
        get { return m_arcDif; }
    }

    public void SuspendRemote(float collideRot){
        if (m_printDebug){
            Debug.Log("shoulderCurrentAngle: " + transform.rotation.y);
            Debug.Log("Shoulder rotDif: " + m_rotDif);
            Debug.Log("Shoulder remote collide rotDif: " + collideRot);
        }

        m_inControl = false;
        StartCoroutine(SuspendControl(m_suspendTime));
    }

    private IEnumerator SuspendControl(float time){
        yield return new WaitForSeconds(time);
        m_inControl = true;
        m_accelFactor = 0f;
    }
}

