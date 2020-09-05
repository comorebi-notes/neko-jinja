using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
  // 再生アニメーションのResourcesフォルダ内のサブパス
  [SerializeField]
  public Object[] AnimationList;

  // 再生アニメーション指定用 
  private enum AnimationPattern : int {
    Wait = 7,      // 待機
    Attack = 4,     // 攻撃 
    Run = 6,       // 走り 
    Count
  }

  // キャラクター管理用 
  private GameObject m_goCharacter = null;
  private GameObject m_goCharPos = null;
  private Vector3 m_vecCharacterPos;      // キャラクター位置 
  private Vector3 m_vecCharacterScale;    // キャラクタースケール 

  // 処理ステップ用 
  private enum Step : int {
    Init = 0,   // 初期化 
    Title,      // タイトル 
    Wait,       // 待機 
    Move,       // 移動 
    Attack,     // 攻撃
    End
  }

  // 処理ステップ管理用 
  private Step m_Step = Step.Init;

  // 汎用
  // いろいろ使いまわす用変数
  private int m_Count = 0;
  private bool m_SW = true;

  // Use this for initialization
  void Start () {
    // キャラクターパラメータ関連を設定 

    // 座標設定 
    m_vecCharacterPos.x = 0.0f;
    m_vecCharacterPos.y = -72.0f;
    m_vecCharacterPos.z = 0.0f;

    // スケール設定 
    m_vecCharacterScale.x = 0.16f;
    m_vecCharacterScale.y = 0.16f;
    m_vecCharacterScale.z = 1.0f;
  }

  // Update is called once per frame
  void Update () {
    switch (m_Step) {
    // 初期化
    case Step.Init:
      m_Count = 0;
      m_SW = true;
      m_Step = Step.Title;
      break;

    // タイトル
    case Step.Title:
      if (++m_Count > 15) {
        m_SW = !m_SW;
        m_Count = 0;
      }
      // if (Input.GetKeyDown(KeyCode.Space) == true ) {
        AnimationStart();   // アニメーション開始処理(設定)
        m_Step = Step.Wait;
      // }
      break;

    // 待機
    case Step.Wait:
      if (Input.GetKeyDown(KeyCode.Z) == true) { // 攻撃
        // 攻撃に変更 
        AnimationChange(AnimationPattern.Attack);
        m_Step = Step.Attack;
      } else if (Input.GetKeyDown(KeyCode.LeftArrow) == true) { // 左移動 
        if (m_vecCharacterScale.x < 0) m_vecCharacterScale.x *= -1; // 左向きにします

        m_goCharPos.transform.localScale = m_vecCharacterScale; // 向き設定 
        // 走りに変更
        AnimationChange(AnimationPattern.Run);
        m_Step = Step.Move;
      } else if (Input.GetKeyDown(KeyCode.RightArrow) == true) { // 右移動
        if (m_vecCharacterScale.x > 0) m_vecCharacterScale.x *= -1; // 右向きにします
        m_goCharPos.transform.localScale = m_vecCharacterScale; // 向き設定
        // 走りに変更 
        AnimationChange(AnimationPattern.Run);
        m_Step = Step.Move;
      }
      break;

    // 移動
    case Step.Move:
      if (Input.GetKey(KeyCode.LeftArrow) == true) { // 左移動 
        if (m_vecCharacterPos.x > -560.0f) m_vecCharacterPos.x -= .16f;
      } else if (Input.GetKey(KeyCode.RightArrow) == true) { // 右移動 
        if (m_vecCharacterPos.x < 560.0f) m_vecCharacterPos.x += .16f;
      } else { // 待機に変更 
        AnimationChange(AnimationPattern.Wait);
        m_Step = Step.Wait;
      }
      m_goCharPos.transform.localPosition = m_vecCharacterPos; // 座標反映 
      break;

    // 攻撃中 
    case Step.Attack:
      if (IsAnimationPlay() == false) {
        // 待機に変更 
        AnimationChange(AnimationPattern.Wait);
        m_Step = Step.Wait;
      }
      break;

    default:
      break;
    }
  }

  private void OnGUI() {
  // GUI変更
  GUIStyle guiStyle = new GUIStyle();
  GUIStyleState styleState = new GUIStyleState();

  switch (m_Step) {
    // タイトル
    case Step.Title:
      if (m_SW == true) {
        styleState.textColor = Color.black; // 文字色 黒 
        guiStyle.normal = styleState; // スタイルの設定。
        // GUI.Label(new Rect(420, 180, 100, 50), "PRESS SPACE", guiStyle);
      }
      break;
    default:
      break;
    }
  }

  // アニメーション開始 
  private void AnimationStart() {
    Script_SpriteStudio6_Root scriptRoot = null;    // SpriteStudio Anime を操作するためのクラス
    int listLength = AnimationList.Length;

    // すでにアニメーション生成済 or リソース設定無い場合はreturn
    if (m_goCharacter != null || listLength < 1) return;

    // 再生するリソース名をリストから取得して再生する
    Object resourceObject = AnimationList[0];
    if (resourceObject != null) {
      // アニメーションを実体化
      m_goCharacter = Instantiate(resourceObject, Vector3.zero, Quaternion.identity) as GameObject;
      if (m_goCharacter != null) {
        scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(m_goCharacter);
        if (scriptRoot != null) {
          // 座標設定するためのGameObject作成
          m_goCharPos = new GameObject();
          if (m_goCharPos == null) {
            // 作成できないケース対応 
            Destroy(m_goCharacter);
            m_goCharacter = null;
          } else {
            // Object名変更 
            m_goCharPos.name = "Comipo";

            // 座標設定 
            m_goCharacter.transform.parent = m_goCharPos.transform;

            // 自分の子に移動して座標を設定
            m_goCharPos.transform.parent = this.transform;
            m_goCharPos.transform.localPosition = m_vecCharacterPos;
            m_goCharPos.transform.localRotation = Quaternion.identity;
            m_goCharPos.transform.localScale = m_vecCharacterScale;

            //アニメーション再生
            AnimationChange(AnimationPattern.Wait);
          }
        }
      }
    }
  }

  // アニメーション 再生/変更 
  private void AnimationChange(AnimationPattern pattern) {
    Script_SpriteStudio6_Root scriptRoot = null;    // SpriteStudio Anime を操作するためのクラス
    int iTimesPlaey = 0;

    if (m_goCharacter == null) return;

    scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(m_goCharacter);
    if (scriptRoot != null) {
      switch (pattern) {
        case AnimationPattern.Wait:
          iTimesPlaey = 0;    // ループ再生 
          break;
        case AnimationPattern.Attack:
          iTimesPlaey = 1;    // 1回だけ再生 
          break;
        case AnimationPattern.Run:
          iTimesPlaey = 0;    // ループ再生 
          break;
        default:
          break;
      }
      scriptRoot.AnimationPlay(-1, (int)pattern, iTimesPlaey);
    }
  }

  // アニメーションが再生中か停止中(エラー含)か取得します
  private bool IsAnimationPlay() {
    bool ret = false;

    Script_SpriteStudio6_Root scriptRoot = null;    // SpriteStudio Anime を操作するためのクラス

    if (m_goCharacter != null) {
      scriptRoot = Script_SpriteStudio6_Root.Parts.RootGet(m_goCharacter);
      if (scriptRoot != null) {
        // 再生回数を取得して、プレイ終了かを判断します
        int Remain = scriptRoot.PlayTimesGetRemain(0);
        if (Remain >= 0) ret = true;
      }
    }

    return ret;
  }
}
