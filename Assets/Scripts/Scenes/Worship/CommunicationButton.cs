using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommunicationButton : MonoBehaviour {
  public void OnClick() {
    SceneManager.LoadScene("Communication");
  }
}
