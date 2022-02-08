using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToAttack : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("Touch.Began");
                    animator.SetBool("isScreenTouch", true);
                break;

                case TouchPhase.Ended:
                    Debug.Log("Touch.Ended");
                    animator.SetBool("isScreenTouch", false);
                break;
            }

        }
    }
}
