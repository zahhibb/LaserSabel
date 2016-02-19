using UnityEngine;
using System.Collections;

public class FakeLoading : MonoBehaviour {

	[SerializeField] private float m_loadingTime = 1f;

	void Start () {
		StartCoroutine ("LoadTheScene");
	}

	private IEnumerator LoadTheScene(){
		yield return new WaitForSeconds(m_loadingTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene("battle", UnityEngine.SceneManagement.LoadSceneMode.Single);	}
}
