using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    // Inputs (Bumpers)
    [SerializeField] private string m_rightBumper = "MoveForward_P1";
    [SerializeField] private string m_leftBumper = "MoveBackward_P1";

    // Inputs (D-Pad)
    [SerializeField] private string m_DPadUp = "MoveForwardDPad_P1";
    [SerializeField] private string m_DPadDown = "MoveBackwardDPad_P1";
    [SerializeField] private string m_DPadLeft = "MoveLeftDPad_P1";
    [SerializeField] private string m_DPadRight = "MoveRightDPad_P1";

    //Input (ABXY-buttons)
    [SerializeField] private string m_YButton = "MoveForwardButton_P1";
    [SerializeField] private string m_AButton = "MoveBackwardButton_P1";
    [SerializeField] private string m_XButton = "MoveLeftButton_P1";
    [SerializeField] private string m_BButton = "MoveRightButton_P1";

    // Strafe checks
    private bool m_rightFirst = false;
    private bool m_leftFirst = false;
    private bool m_inControl = true;

    // Dash speed, direction & calculations
    private Vector3 m_moveDirection;
    private float m_maxDashTime = 2.0f;
    private float m_dashSpeed = 10.0f;
    private float m_dashStoppingSpeed = 0.1f;
    private float m_currentDashTime;
    private bool m_isDying = false;
    private float m_suspendTime = 0.1f;

    // Defined speed for D-Pad movement
    private float m_dashSpeedDPad = 0.5f;

    // Player check at distance calculation
    private Transform m_otherPlayer = null;
    private GameObject m_gameController = null;
    private SceneManager m_SceneManager = null;

    private void Start() {
        m_currentDashTime = m_maxDashTime;
        m_otherPlayer = (gameObject.tag == "Player1") ? GameObject.FindGameObjectWithTag("Player2").transform : GameObject.FindGameObjectWithTag("Player1").transform;
        m_gameController = GameObject.FindGameObjectWithTag("GameController");
        m_SceneManager = m_gameController.GetComponent<SceneManager>();
    }

    // träff och reset
    private void OnTriggerEnter(Collider clash){
        if (clash.tag == "LazerTag"){
            Debug.Log(gameObject.tag + "Took a hit!");
            m_SceneManager.PlayerDefeated(gameObject.tag);
        }
    }

    private void Update() {
        if (m_isDying){
            float newY = Mathf.Clamp(transform.localScale.y - 2 * Time.deltaTime, 0.05f, 1f);
            transform.localScale = new Vector3(1f, newY, 1f);
        }
        else{ 
            PlayerFocus();

            PlayerMovementBumper();
            PlayerMovementDPad();
            PlayerMovementButtons();


            RaycastHit rHit;
            float range = 2f;
            Vector3 traceOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            Ray ray = new Ray(traceOrigin, m_moveDirection);
            Debug.DrawRay(traceOrigin, m_moveDirection * range);

            if (Physics.Raycast(ray, out rHit, range)){
                if (rHit.collider.tag == "Vall"){
                    m_moveDirection = m_moveDirection * 0;
                }
            }
            if (!m_isDying && m_inControl){
                DashMovement(m_moveDirection);
            }
        }
    }

    // Players cameras focus on each other
    private void PlayerFocus() {
        if (gameObject.tag == "Player1") {
            transform.LookAt(GameObject.FindGameObjectWithTag("Player2").transform);
        }
        if (gameObject.tag == "Player2") {
            transform.LookAt(GameObject.FindGameObjectWithTag("Player1").transform);
        }
    }

    // Player input & move direction for Bumpers
    private void PlayerMovementBumper() {
        float distanceToPlayer = Vector3.Distance(m_otherPlayer.position, transform.position);
        float minDistance = 2.2f;

        float dashForwardAxis = Mathf.Sign(Input.GetAxis(m_rightBumper));
        float dashBackwardAxis = Mathf.Sign(Input.GetAxis(m_leftBumper));
        float dashHorizontalAxis = Mathf.Sign(Input.GetAxis(m_rightBumper));

        Vector3 dashMovementForward = transform.forward * dashForwardAxis;
        Vector3 dashMovementBackward = transform.forward * dashBackwardAxis;
        Vector3 dashMovementRight = transform.right * dashHorizontalAxis;
        Vector3 dashMovementLeft = -transform.right * dashHorizontalAxis;

        // Assign strafe priority
        if (Input.GetButtonDown(m_rightBumper)) {
            if (Input.GetButton(m_rightBumper) && m_leftFirst == false) {
                m_rightFirst = true;
            } else {
                m_rightFirst = false;
            }
        } else {
            m_rightFirst = false;
        }

        if (Input.GetButtonDown(m_leftBumper)) {
            if (Input.GetButton(m_leftBumper) && m_rightFirst == false) {
                m_leftFirst = true;
            } else {
                m_leftFirst = false;
            }
        } else {
            m_leftFirst = false;
        }

        // Perform movement axis/direction based on priority
        if (Input.GetButton(m_rightBumper) && Input.GetButton(m_leftBumper)) {
            // Strafe left
            if (m_leftFirst) {
                m_currentDashTime = 0.0f;
                m_moveDirection = dashMovementLeft;                
            }
            // Strafe right
            else if (m_rightFirst) {
                m_currentDashTime = 0.0f;                
                m_moveDirection = dashMovementRight;
            }
        // Move forward
        } else if (Input.GetButtonDown(m_rightBumper)) {
            m_currentDashTime = 0.0f;
                                          
            if (distanceToPlayer <= minDistance) {
                m_moveDirection = new Vector3(0, 0, 0);
            }else {
                m_moveDirection = dashMovementForward;
            }
        // Move backward
        } else if (Input.GetButtonDown(m_leftBumper)) {
            m_currentDashTime = 0.0f;
            m_moveDirection = dashMovementBackward;
        }

        if (m_moveDirection == dashMovementForward && distanceToPlayer <= minDistance) {
            m_currentDashTime = m_maxDashTime;
        }        
    }

    // Player input & move direction for D-Pad
    private void PlayerMovementDPad() {
        float distanceToPlayer = Vector3.Distance(m_otherPlayer.position, transform.position);
        float minDistance = 2.2f;

        float dashForwardAxisDPad = Input.GetAxis(m_DPadUp);
        float dashBackwardAxisDPad = Input.GetAxis(m_DPadDown);
        float dashLeftAxisDPad = Input.GetAxis(m_DPadLeft);
        float dashRightAxisDPad = Input.GetAxis(m_DPadRight);

        Vector3 dashMovementForward = transform.forward * dashForwardAxisDPad;
        Vector3 dashMovementBackward = transform.forward * dashBackwardAxisDPad;
        Vector3 dashMovementLeft = transform.right * dashLeftAxisDPad;
        Vector3 dashMovementRight = transform.right * dashRightAxisDPad;

        if (Input.GetAxis(m_DPadUp) > 0) {
            m_currentDashTime = 0.0f;

            if (distanceToPlayer <= minDistance) {
                m_moveDirection = new Vector3(0, 0, 0);
            } else {
                m_moveDirection = dashMovementForward * m_dashSpeedDPad;
            }
        }
        else if (Input.GetAxis(m_DPadDown) < 0) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementBackward * m_dashSpeedDPad;
        } 
        else if (Input.GetAxis(m_DPadLeft) < 0) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementLeft * m_dashSpeedDPad;
        } 
        else if (Input.GetAxis(m_DPadRight) > 0) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementRight * m_dashSpeedDPad;
        }
    }

    // Player input & move direction for Button ( A, B, X, Y )
    private void PlayerMovementButtons() {
        float distanceToPlayer = Vector3.Distance(m_otherPlayer.position, transform.position);
        float minDistance = 2.2f;

        float dashForwardAxisButton = Mathf.Sign(Input.GetAxis(m_YButton));
        float dashBackwardAxisButton = Mathf.Sign(Input.GetAxis(m_AButton));
        float dashLeftAxisButton = Mathf.Sign(Input.GetAxis(m_XButton));
        float dashRightAxisButton = Mathf.Sign(Input.GetAxis(m_BButton));

        Vector3 dashMovementForward = transform.forward * dashForwardAxisButton;
        Vector3 dashMovementBackward = transform.forward * dashBackwardAxisButton;
        Vector3 dashMovementLeft = transform.right * dashLeftAxisButton;
        Vector3 dashMovementRight = transform.right * dashRightAxisButton;

        if (Input.GetButtonDown(m_YButton)) {
            m_currentDashTime = 0.0f;

            if (distanceToPlayer <= minDistance) {
                m_moveDirection = new Vector3(0, 0, 0);
            } else {
                m_moveDirection = dashMovementForward;
            }
        }
        else if (Input.GetButtonDown(m_AButton)) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementBackward;
        } 
        else if (Input.GetButtonDown(m_XButton)) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementLeft;
        } 
        else if (Input.GetButtonDown(m_BButton)) {
            m_currentDashTime = 0.0f;

            m_moveDirection = dashMovementRight;
        }
    }

    // Actual movement transform
    private void DashMovement(Vector3 dashDir) {
        Vector3 moveDir;
        
        if (m_currentDashTime < m_maxDashTime) {
            moveDir = dashDir * m_dashSpeed;
            m_currentDashTime += m_dashStoppingSpeed;
        } else {
            moveDir = Vector3.zero;
        }
        transform.position += moveDir * Time.deltaTime;
    }

    // Become more powerful than you could possibly imagine
    public void DieLikeObi(float time){
        m_isDying = true;
        StartCoroutine(RiseAgain(time));
        BroadcastMessage("SheatheLightsaber");
    }

    private IEnumerator RiseAgain(float time){
        yield return new WaitForSeconds(time);
        m_isDying = false;
        
    }

    void SuspendRemote(float collideRot){
        m_inControl = false;
        StartCoroutine(SuspendControl(m_suspendTime));
    }

    private IEnumerator SuspendControl(float time){
        yield return new WaitForSeconds(time);
        m_inControl = true;
        m_currentDashTime = m_maxDashTime;
    }
}
