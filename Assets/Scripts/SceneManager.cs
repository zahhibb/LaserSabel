using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneManager : MonoBehaviour {

    private int m_player1score = 0;
    private int m_player2score = 0;

    [SerializeField] private GameObject m_lasersabel1 = null;
    [SerializeField] private GameObject m_lasersabel2 = null;
    [SerializeField] private GameObject m_lasersabel3 = null;
    [SerializeField] private GameObject m_lasersabel4 = null;

    private string m_p1Choice; //att få från meny
    private string m_p2Choice;

    [SerializeField] private Camera m_fancyCam = null;
    [SerializeField] private Text m_canvasText = null;

    private float m_fancyCamHeight = 2.5f;
    private bool m_cutsceneRunning = false;
    private GameObject m_fancyCamFocus = null;
    private float m_goodTime = 3f;

    private GameObject m_startPos1 = null;
    private GameObject m_startPos2 = null;

    private GameObject m_player1 = null;
    private GameObject m_player2 = null;
    private GameObject m_database = null;    

    [SerializeField] private GameObject m_playerPrefab1 = null;
    [SerializeField] private GameObject m_playerPrefab2 = null;

    private float m_gameTime = 0f;

    void Awake() {
        m_database = GameObject.FindGameObjectWithTag("Database");
    }

    private void Start (){
        m_fancyCam.enabled = false;

        m_gameTime = 0f;

        m_startPos1 = GameObject.FindGameObjectWithTag("StartPos1");
        m_startPos2 = GameObject.FindGameObjectWithTag("StartPos2");

        m_player1 = (GameObject)Instantiate(m_playerPrefab1, m_startPos1.transform.position, m_startPos1.transform.rotation);
        m_player2 = (GameObject)Instantiate(m_playerPrefab2, m_startPos2.transform.position, m_startPos1.transform.rotation);

        m_fancyCamFocus = transform.gameObject;

        m_p1Choice = m_database.GetComponent<PlayerInfo>().GetSaber()[0];
        m_p2Choice = m_database.GetComponent<PlayerInfo>().GetSaber()[1];

        m_player1.GetComponentInChildren<HiltRot>().EquipLasersaber(GetSabers(m_p1Choice));
        m_player2.GetComponentInChildren<HiltRot>().EquipLasersaber(GetSabers(m_p2Choice));

        m_canvasText.text = "0" + "\t\t" + "0";
    }

    private void Update () {
        m_gameTime += (1f * Time.deltaTime);

        // Camera and Listener Animations
        transform.position = (m_player1.transform.position + (m_player2.transform.position - m_player1.transform.position) / 2f) - new Vector3 (0f, -m_fancyCamHeight, 0f);
        if (m_cutsceneRunning) {
            AnimateCamera(m_fancyCamFocus.transform);
        }
    }

    public GameObject GetSabers(string pChoice){
        switch (pChoice){
            case "Red":
                return m_lasersabel1;
            case "Blue":
                return m_lasersabel2;
            case "Green":
                return m_lasersabel3;
            case "Purple":
                return m_lasersabel4;
            default:
                return m_lasersabel1;
        }
    }

    public float GameTime{
        get {return m_gameTime;}
        set { m_gameTime = value;}
    }


    public void ResetPositions(){
        m_player1.transform.position = m_startPos1.transform.position;
        m_player2.transform.position = m_startPos2.transform.position;
        m_player1.transform.localScale = new Vector3(1f, 1f, 1f);
        m_player2.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void AnimateCamera(Transform focus){
        if (m_cutsceneRunning){
            m_fancyCam.transform.RotateAround(focus.position, Vector3.up, 45f * Time.deltaTime);
            m_fancyCam.transform.LookAt(focus.transform);
        }        
        else{
            m_fancyCam.transform.RotateAround(focus.position, Vector3.up, 45f * Time.deltaTime);
            m_fancyCam.transform.LookAt(focus.transform);
        }
    }

    public void PlayerDefeated(string loser){
        if (!m_cutsceneRunning){
            Debug.Log(loser + "called as loser from Manager");
            m_fancyCamFocus = (loser == "Player1") ? m_player2 : m_player1;
            StartCoroutine(Celebrate(m_goodTime));
            m_cutsceneRunning = true;
            m_fancyCam.enabled = true;

            if (loser == m_player1.tag){
                m_player2score++;
                m_player1.GetComponentInChildren<PlayerInput>().DieLikeObi(m_goodTime);
            }
            else if (loser == m_player2.tag){
                m_player1score++;
                m_player2.GetComponentInChildren<PlayerInput>().DieLikeObi(m_goodTime);
            }

            string p1Score = m_player1score.ToString();
            string p2Score = m_player2score.ToString();

            if (m_player1score < 5 && m_player2score < 5){
                
                m_canvasText.text = p1Score + "\t\t" +p2Score;
            }
            else{
                m_canvasText.text = m_fancyCamFocus.tag + " best at LaserSabel.";
                StartCoroutine(loadMenu(3));
            }
        }
    }

    private IEnumerator Celebrate(float time){
        if (m_player1score < 5 && m_player2score < 5){
            yield return new WaitForSeconds(time);
            ResetPositions();
            m_cutsceneRunning = false;
            m_fancyCam.enabled = false;
        }
    }

    private IEnumerator loadMenu(float time){
        yield return new WaitForSeconds(time);
        Destroy(GameObject.FindGameObjectWithTag("Database"));
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
