using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TouchScript.Gestures;

public class CharacterControl2 : MonoBehaviour {
  // 再生アニメーションのResourcesフォルダ内のサブパス
  [SerializeField]
  public UnityEngine.Object[] animationList;
  public float walkSpeed;

  // キャラクター管理用 
  private GameObject _goCharacter = null;
  private GameObject _goCharacterPosition = null;
  private Vector3 _vecCharacterPosition; 
  private Vector3 _vecCharacterScale; 

  // 再生アニメーション指定用 
  private enum AnimationPattern : int {
    Wait    = 7, // 待機 
    Walk    = 6, // 歩き
    Stretch = 3  // のび
  }

  // 処理ステップ用 
  private enum State : int {
    Init,   // 初期化 
    Wait,   // 待機 
    Walk,   // 移動 
    Stretch // のび
  }

  // 処理ステップ管理用 
  private State _currentState = State.Init;

  // Use this for initialization
  private void Start () {
    // 座標設定 
    _vecCharacterPosition.x = 30.0f;
    _vecCharacterPosition.y = -72.0f;
    _vecCharacterPosition.z = 0.0f;

    // スケール設定 
    _vecCharacterScale.x = 0.16f;
    _vecCharacterScale.y = 0.16f;
    _vecCharacterScale.z = 1.0f;

    GetComponent<TapGesture>().Tapped += (object sender, EventArgs e) => HandleStretch();
    GetComponent<FlickGesture>().Flicked += (object sender, EventArgs e) => {
      var gesture = sender as FlickGesture;
      if (gesture.ScreenFlickVector.x < 0) {
        HandleWalkLeft();
      } else if (gesture.ScreenFlickVector.x > 0) {
        HandleWalkRight();
      }
    };
  }

  // Update is called once per frame
  private void Update () {
    switch (_currentState) {
    // 初期化
    case State.Init:
      AnimationStart();
      _currentState = State.Wait;
      break;

    // 待機
    case State.Wait:
      if (Input.GetKeyDown(KeyCode.X)) {
        HandleStretch(); // のび
      } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
        HandleWalkLeft(); // 左移動
      } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
        HandleWalkRight(); // 右移動
      }
      break;

    // 移動
    case State.Walk:
      var screenWidthHalf = Screen.width / 2;

      if (Input.GetKey(KeyCode.UpArrow)) {
        // 左移動
        if (_vecCharacterPosition.x > -screenWidthHalf) _vecCharacterPosition.x -= walkSpeed;

      } else if (Input.GetKey(KeyCode.DownArrow)) {
        // 右移動 
        if (_vecCharacterPosition.x < screenWidthHalf) _vecCharacterPosition.x += walkSpeed;

      } else {
        // 待機に変更 
        AnimationChange(AnimationPattern.Wait);
        _currentState = State.Wait;
      }
      _goCharacterPosition.transform.localPosition = _vecCharacterPosition; // 座標反映 
      break;

    // のび 
    case State.Stretch:
      if (!IsAnimationPlay()) {
        AnimationChange(AnimationPattern.Wait);
        _currentState = State.Wait;
      }
      break;
    
    default:
      break;
    }
  }

  private void HandleStretch() {
    if (_currentState != State.Wait) return;

    AnimationChange(AnimationPattern.Stretch);
    _currentState = State.Stretch;
  }

  private void HandleWalkLeft() {
    if (_currentState != State.Wait) return;

    if (_vecCharacterScale.x < 0) _vecCharacterScale.x *= -1;
    _goCharacterPosition.transform.localScale = _vecCharacterScale; 
    AnimationChange(AnimationPattern.Walk);
    _currentState = State.Walk;
  }

  private void HandleWalkRight() {
    if (_currentState != State.Wait) return;

    if (_vecCharacterScale.x > 0) _vecCharacterScale.x *= -1;
    _goCharacterPosition.transform.localScale = _vecCharacterScale;
    AnimationChange(AnimationPattern.Walk);
    _currentState = State.Walk;
  }

  // アニメーション開始 
  private void AnimationStart() {
    // すでにアニメーション生成済 or リソース設定無い場合はreturn
    if (_goCharacter || animationList.Length < 1) return;

    // 再生するリソース名をリストから取得
    var resourceObject = animationList[0];
    if (!resourceObject) return;

    // アニメーションを実体化
    _goCharacter = Instantiate(resourceObject, Vector3.zero, Quaternion.identity) as GameObject;
    if (_goCharacter == null) return;

    var scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(_goCharacter);
    if (!scriptRoot) return;

    // 座標設定するためのGameObject作成
    _goCharacterPosition = new GameObject();
    if (!_goCharacterPosition) {
      Destroy(_goCharacter);
      _goCharacter = null;
    } else {
      // 座標設定 
      _goCharacter.transform.parent = _goCharacterPosition.transform;

      // 自分の子に移動して座標を設定
      _goCharacterPosition.transform.parent = this.transform;
      _goCharacterPosition.transform.localPosition = _vecCharacterPosition;
      _goCharacterPosition.transform.localRotation = Quaternion.identity;
      _goCharacterPosition.transform.localScale = _vecCharacterScale;

      //アニメーション再生
      AnimationChange(AnimationPattern.Wait);
    }
  }

  // アニメーション再生/変更 
  private void AnimationChange(AnimationPattern pattern) {
    if (!_goCharacter) return;

    var timesPlay = 0;
    var scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(_goCharacter);

    if (!scriptRoot) return;
    switch (pattern) {
      case AnimationPattern.Wait:
        timesPlay = 0; // ループ再生 
        break;
      case AnimationPattern.Stretch:
        timesPlay = 1; // 1回だけ再生 
        break;
      case AnimationPattern.Walk:
        timesPlay = 0; // ループ再生 
        break;
      default:
        break;
    }
    scriptRoot.AnimationPlay(-1, (int)pattern, timesPlay);
  }

  // アニメーションが再生中か停止中(エラー含)か取得します
  private bool IsAnimationPlay() {
    if (!_goCharacter) return false;

    var scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(_goCharacter);
    if (!scriptRoot) return false;

    // 再生回数を取得してプレイ終了かどうか判定
    var remain = scriptRoot.PlayTimesGetRemain(0);
    return remain >= 0;
  }
}
