using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerControler : MonoBehaviour
{
    public static PlayerControler instance;

    public int moveSpeed;
    public bool lockedMove;

    Animator _Am;
    Rigidbody2D _Rd;
    SpriteRenderer _Sr;

    float inputX;

    private void Start()
    {
        instance = this;

        lockedMove = true;

        _Am = transform.GetChild(0).GetComponent<Animator>();
        _Rd = GetComponent<Rigidbody2D>();
        _Sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lockedMove)
        if (SceneManager.GetSceneByName("bbb").name.Equals("bbb")) lockedMove = false;

        if (!lockedMove)
        {
            inputX = Input.GetAxis("Horizontal");

            _Rd.velocity = new Vector2(moveSpeed * inputX, _Rd.velocity.y);

            if (inputX != 0)
            {
                Flip(inputX);
                _Am.SetBool("isRun", true);
            }
            else _Am.SetBool("isRun", false);
        }
    }


    void Flip(float axis)
    {
        if (axis < 0 ? true : false)
            _Sr.flipX = true;
        else _Sr.flipX = false;
    }


}
