using UnityEngine;
using System.Collections;

public class ProceedToLoading : MonoBehaviour {
	[SerializeField] private float m_waitTime = 5f;
	[SerializeField] private GameObject m_loadingObject;
	[SerializeField] private GameObject m_removeObject0;
	[SerializeField] private GameObject m_removeObject1;
	
	private void Update () {
		StartCoroutine ("LoadLoading");
	}

	private IEnumerator LoadLoading(){
		yield return new WaitForSeconds(m_waitTime);
        
        m_loadingObject.SetActive(true);
		m_removeObject0.SetActive (false);
		m_removeObject1.SetActive (false);

	}
}
