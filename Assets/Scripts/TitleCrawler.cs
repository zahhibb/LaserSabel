using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleCrawler : MonoBehaviour {

    [SerializeField] private AudioSource m_crawlerMusic;
    [SerializeField] private Text m_farAwayText;
    [SerializeField] private Text m_crawlerTitle;
    [SerializeField] private Text m_crawlerText;    

    private float m_farAwayTime = 9f;
    private float m_titleCrawlerSpeed;
    private float m_crawlerFadeTime = 1.2f;
    private float m_crawlerAlpha = 0f;
    private float m_crawlerTime = 88f;
    private float m_textCrawlerSpeed = 2f;

    private Color m_colorToFadeTo;
    private Color m_farAwayFadeColor;    
    private Color m_screenFade;

    private GameObject panel;
    
	private void Update () {
        if (Input.GetButtonDown("StartButton_P1")) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
        } else {
            m_crawlerTime -= Time.deltaTime;
            if (m_crawlerTime > 0) {
                FarFarAway();
            }else if (m_crawlerTime <= 0) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StartMenu");
            }
        }
    }

    private void FarFarAway() {
        m_farAwayTime -= Time.deltaTime;
        if (m_farAwayTime > 0f) {
            // Fade in FarFarAway text
            m_farAwayFadeColor = m_farAwayText.color;            
            m_farAwayFadeColor.a = Mathf.Lerp(0, 0.75f, Time.time / 1.5f);
            m_farAwayText.color = m_farAwayFadeColor;
            
            if (m_farAwayTime <= 3f) {
                // Fade out FarFarAway text
                m_farAwayText.GetComponent<Text>().CrossFadeAlpha(0f, 0.5f, false);
            }
        } else {            
            if (!m_crawlerMusic.isPlaying) {
                m_crawlerMusic.Play();
            }
            StartCoroutine(AudioTimer());
        }
    }

    private void StarCrawler() {
        Destroy(GameObject.FindGameObjectWithTag("FFABackground"));
        // Move Title crawler
        m_crawlerTitle.enabled = true;
        m_titleCrawlerSpeed += 0.7f;
        m_crawlerTitle.transform.Translate(new Vector3(0, 0, 1) * m_titleCrawlerSpeed * Time.deltaTime);

        if (m_crawlerTitle.transform.position.z >= 0) {
            // Fade Title crawler
            m_colorToFadeTo = new Color(1f, 1f, 1f, m_crawlerAlpha);
            m_crawlerTitle.CrossFadeColor(m_colorToFadeTo, m_crawlerFadeTime, true, true);
            
            // Move Text crawler            
            m_crawlerText.transform.Translate(new Vector3(0f, 5f, 1f) * m_textCrawlerSpeed * Time.deltaTime);
        }
    }

    // More precisely times the Titlecrawl with the audio
    private IEnumerator AudioTimer() {
        yield return new WaitForSeconds(0.3f);
        StarCrawler();
    }

}
