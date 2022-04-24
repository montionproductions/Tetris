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

    private float _timePressed = 0;
    private bool _isPressed = false;
    private bool _isMoving = false;

    void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;

                _isPressed = true;
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
                _timePressed = 0f;
            }
        }

        if(_isPressed)
        {
            _timePressed += Time.deltaTime;
            Debug.Log(_timePressed);
        }
    }

    void checkSwipeReleased()
    {
        //Check if Vertical swipe
        if (verticalMove() > VERTICAL_SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDownReleased();
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
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
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
    }

    void OnSwipeDown()
    {
        gameObject.SendMessage("MoveDown");
    }

    void OnSwipeDownReleased()
    {
        gameObject.SendMessage("FallHard");
    }

    void OnSwipeLeft()
    {
        gameObject.SendMessage("Move", Vector2.left);
    }

    void OnSwipeRight()
    {
        //Debug.Log("Swipe Right");
        gameObject.SendMessage("Move", Vector2.right);
    }
}
