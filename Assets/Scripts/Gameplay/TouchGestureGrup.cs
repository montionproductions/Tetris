using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGestureGrup : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float HORIZONTAL_SWIPE_THRESHOLD = 20f;
    public float VERTICAL_SWIPE_THRESHOLD = 10f;

    Game gameController;

    private float _pressTime = 0;
    private float _movingTime = 0;
    private bool _isPressed = false;
    private bool _isMoving = false;

    void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.isPaused)
            return;

        _pressTime += Time.deltaTime;

        if(_isMoving)
        {
            _movingTime += Time.deltaTime;
        }
        

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;

                _isPressed = true;
                _pressTime = 0;
                _movingTime = 0;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                _isMoving = true;

                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                _isPressed = false;
                _isMoving = false;

                fingerDown = touch.position;
                checkSwipeReleased();
            }
        }

    }

    void checkSwipeReleased()
    {
        //Check if Vertical swipe
        if (verticalMove() > VERTICAL_SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y < 0 && _pressTime < .25f && _movingTime < .15f)//Down swipe
            {
                Debug.Log(_pressTime);
                OnSwipeDownReleased();
                _pressTime = 0;
            }
            else if (fingerDown.y - fingerUp.y > 0)//Up swipe
            {
                OnSwipeUp();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > HORIZONTAL_SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }
    }

    void checkSwipe()
    {
        //Check if Vertical swipe
        if (verticalMove() > VERTICAL_SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0 && _pressTime > gameController.RotateSpeed)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0 && _pressTime > gameController.VerticalMovSpeed)//Down swipe
            {
                OnSwipeDown();
            }
            //fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > HORIZONTAL_SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0 && _pressTime > gameController.HorizontalMovSpeed)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0 && _pressTime > gameController.HorizontalMovSpeed)//Left swipe
            {
                OnSwipeLeft();
            }
            //fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        gameObject.SendMessage("Rotate");
        _pressTime = 0;
    }

    void OnSwipeDown()
    {
        gameObject.SendMessage("MoveDown");
        _pressTime = 0;
    }

    void OnSwipeDownReleased()
    {
        gameObject.SendMessage("FallHard");
        _movingTime = 0;
    }

    void OnSwipeLeft()
    {
        gameObject.SendMessage("Move", Vector2.left);
        _pressTime = 0;
    }

    void OnSwipeRight()
    {
        //Debug.Log("Swipe Right");
        gameObject.SendMessage("Move", Vector2.right);
        _pressTime = 0;
    }
}
