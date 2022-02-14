using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using KKSpeech;

using Random = System.Random;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleVoiceSystem:MonoBehaviour {
  public BattleState battleState;
  public GameObject battle;
  public TMP_Text dialogueText;
  public Button resetButton;
  public Button recordAttackButton;

  Animator playerAnimator;
  Animator enemyAnimator;

  Unit playerUnit;
  Unit enemyUnit;

  Random rnd = new Random();

  bool isDead = false;
  string[] wordsArray = {"Attack", "Charge"};
  string spell = "";

  public void OnResetButton() {
    SceneManager.LoadScene("MainScene");
  }

  void Start() {
    battleState = BattleState.START;
    SetupVoice();
    SetupBattle();
  }

  void SetupVoice() {
    if (SpeechRecognizer.ExistsOnDevice()) {
      SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
      listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
      listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
      listener.onErrorDuringRecording.AddListener(OnError);
      listener.onErrorOnStartRecording.AddListener(OnError);
      listener.onFinalResults.AddListener(OnFinalResult);
      listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
      SpeechRecognizer.RequestAccess();
    } else {
      dialogueText.text = "This device doesn't support speech recognition";
      recordAttackButton.gameObject.SetActive(false);
    }
  }

  void SetupBattle() {
    GameObject playerGO = battle.transform.Find("DogKnight").gameObject;
    playerUnit = playerGO.GetComponent<Unit>();
    playerAnimator = playerGO.GetComponent<Animator>();

    GameObject enemyGO = battle.transform.Find("Slime").gameObject;
    enemyUnit = enemyGO.GetComponent<Unit>();
    enemyAnimator = enemyGO.GetComponent<Animator>();

    int index = rnd.Next(wordsArray.Length);
    spell = wordsArray[index];

    battleState = BattleState.PLAYERTURN;

    dialogueText.text = $"Say the word {spell}";
  }

  void EndBattle() {
    dialogueText.text = "You won the battle!";
  }

  public void OnFinalResult(string result) {
    StartCoroutine(BattleProcess(result));
  }

  IEnumerator BattleProcess (string result) {
    if (battleState == BattleState.PLAYERTURN && result == spell) {
      playerAnimator.SetBool("isScreenTouch", true);

      dialogueText.text = "Correct Spell!";

      yield return new WaitForSeconds(1f);

      playerAnimator.SetBool("isScreenTouch", false);

      isDead = enemyUnit.TakeDamage(playerUnit.damage);

    } else {
      dialogueText.text = $"Say the word {spell} again.";

      recordAttackButton.GetComponentInChildren<Text>().text = "Try again!";
      recordAttackButton.gameObject.SetActive(true);
    }

    if (isDead) {
      enemyAnimator.SetBool("isScreenTouch", true);
      battleState = BattleState.WON;
      EndBattle();
    }
  }

  public void OnPartialResult(string result) {
    dialogueText.text = $"Say the word {spell} again.";
  }

  public void OnAvailabilityChange(bool available) {
    recordAttackButton.gameObject.SetActive(available);

    if (!available) {
      dialogueText.text = "Speech Recognition not available";
    } else {
      dialogueText.text = "Say something";
    }
  }

  public void OnAuthorizationStatusFetched(AuthorizationStatus status) {
    switch (status) {
      case AuthorizationStatus.Authorized:
        recordAttackButton.gameObject.SetActive(true);
        break;
      default:
        recordAttackButton.gameObject.SetActive(false);
        dialogueText.text = "Cannot use Speech Recognition, authorization status is " + status;
        break;
    }
  }

  public void OnEndOfSpeech() {
    recordAttackButton.GetComponentInChildren<Text>().text = "Start Recording";
  }

  public void OnError(string error) {
    recordAttackButton.GetComponentInChildren<Text>().text = "Error. Try again!";
    recordAttackButton.gameObject.SetActive(true);
  }

  public void OnStartRecordingPressed() {
    if (SpeechRecognizer.IsRecording()) {
      #if ((UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR))
        SpeechRecognizer.StopIfRecording();
        recordAttackButton.GetComponentInChildren<Text>().text = "Start Recording";
        recordAttackButton.gameObject.SetActive(false);
      #endif
    } else {
      SpeechRecognizer.StartRecording(true);
      recordAttackButton.GetComponentInChildren<Text>().text = "Stop Recording";
    }
  }
}
