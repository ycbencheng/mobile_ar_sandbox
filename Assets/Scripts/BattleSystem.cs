using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject battle;
    public TMP_Text dialogueText;
    public Button resetButton;

    Animator playerAnimator;
    Animator enemyAnimator;

    Unit playerUnit;
    Unit enemyUnit;

    bool isDead = false;

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        GameObject playerGO = battle.transform.Find("DogKnight").gameObject;
        playerUnit = playerGO.GetComponent<Unit>();
        playerAnimator = playerGO.GetComponent<Animator>();

        GameObject enemyGO = battle.transform.Find("Slime").gameObject;
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyAnimator = enemyGO.GetComponent<Animator>();

        state = BattleState.PLAYERTURN;
    }


    void Update()
    {       
        if (state == BattleState.PLAYERTURN && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    playerAnimator.SetBool("isScreenTouch", true);
                    break;

                case TouchPhase.Ended:
                    playerAnimator.SetBool("isScreenTouch", false);
                    enemyAnimator.SetBool("isScreenTouch", true);

                    isDead = enemyUnit.TakeDamage(playerUnit.damage);
                    break;
            }

        }
        Debug.Log(isDead);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
    }

    public void OnResetButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    void EndBattle()
    {
        dialogueText.text = "You won the battle!";
    }
}
