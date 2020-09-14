using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorshipButton : MonoBehaviour {
  public void OnClick() {
    SceneManager.LoadScene("Worship");
  }
}
