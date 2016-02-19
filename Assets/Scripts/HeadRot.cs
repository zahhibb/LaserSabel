using UnityEngine;
using System.Collections;

public class HeadRot : MonoBehaviour{
    [SerializeField] private string m_leftXAxis = "LeftXAxis_P1";
    [SerializeField] private string m_leftYAxis = "LeftYAxis_P1";

    private void Update (){
        float x = Input.GetAxis(m_leftXAxis);
        float y = Input.GetAxis(m_leftYAxis);
        float angle = Mathf.Atan2(x, y) * 180f / Mathf.PI;

        Quaternion quaternionRotate = Quaternion.Euler(0f, angle, 0f);

        if (angle < -80f || angle > 80f){
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternionRotate, 1000f * Time.deltaTime);
        }
        else{
            Quaternion quaternionNeutralRotate = Quaternion.Euler(0f, 0f, 0f);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternionNeutralRotate, 1200f * Time.deltaTime);
        }
    }
}
