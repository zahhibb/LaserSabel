using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {


    [SerializeField] private string[] playerChoices;

    void Awake () {
        DontDestroyOnLoad(transform.gameObject);
        Destroy(GameObject.FindGameObjectWithTag("StartPos2"));
        gameObject.tag = "Database";
    }
    public void SetSaberPlayer1(string saber) {
        playerChoices[0] = saber;
    }

    public void SetSaberPlayer2(string saber) {
        playerChoices[1] = saber;
    }

    public string[] GetSaber() {
        return playerChoices;
    }
}
