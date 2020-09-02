using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoroAnimation : MonoBehaviour {
  private Vector2 _position;

  private void Start() {
    _position = transform.position;
  }

  private void Update() {
    var time = Time.time / 1.5f;
    var deg = time * Mathf.Rad2Deg % 360;
    var scale = transform.localScale;
    var scaleXAbs = Mathf.Abs(scale.x);

    transform.position = new Vector2(Mathf.Sin(time) * 60.0f + _position.x, _position.y);

    scale.x = deg > 90 && deg < 270 ? scaleXAbs : -scaleXAbs;
    transform.localScale = scale;
  }
}
