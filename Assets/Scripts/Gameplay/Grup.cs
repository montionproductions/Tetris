using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grup : MonoBehaviour
{
    public Game gameController;
    public Transform boxSelected;
    public Transform trail;
    public bool isNoRatable = false;

    private Transform boxSelectedObj;

    float lastFall = 0f;
    float _pressTime = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();
    }

    void Start()
    {
        // Default position not valid? Then it's game over
        if (!IsValidGridPos(this.transform))
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
            gameController.StartCoroutine("GameOver");
        }

        boxSelectedObj = Instantiate(boxSelected);
        UpdateSelectedPosition();

        // Show trail
        InstantiateHardFallTrails();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.isPaused || Game.isGameOver)
            return;

        _pressTime += Time.deltaTime;

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftArrow) && _pressTime > gameController.HorizontalMovSpeed)
        {
            Move(Vector2.left);
            _pressTime = 0f;
        }// Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow) && _pressTime > gameController.HorizontalMovSpeed)
        {
            Move(Vector2.right);
            _pressTime = 0f;
        }// Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow) && _pressTime > gameController.RotateSpeed)
        {
            Rotate();
            _pressTime = 0f;
        }// Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= gameController.MovTime ||
           Input.GetKey(KeyCode.DownArrow) && _pressTime > gameController.VerticalMovSpeed)
        {
            MoveDown();
            _pressTime = 0f;
        } // Fall hard
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            FallHard();
        }

        UpdateSelectedPosition();
    }

    void Move(Vector2 dir)
    {
        ShowHardFallTrails(false);
        Vector2 norDir = dir;
        // Modify position
        transform.position += new Vector3(norDir.x, norDir.y, 0);

        // See if it's valid
        if (IsValidGridPos(this.transform))
            // It's valid. Update grid.
            UpdateGrid();
        else
            // Its not valid. revert.
            transform.position += new Vector3(-norDir.x, -norDir.y, 0);

        ShowHardFallTrails(true);
    }

    void Rotate()
    {
        if (isNoRatable)
            return;

        ShowHardFallTrails(false);
        transform.Rotate(0, 0, -90);

        // See if valid
        if (IsValidGridPos(this.transform))
        {
            // It's valid. Update grid.
            UpdateGrid();
        }
        else
        {
            if(CheckValidRotate(transform.position))
            {
                // It's valid. Update grid.
                UpdateGrid();
            } else
            {
                // Revert rotation
                transform.Rotate(0, 0, 90);
            }
        }

        ShowHardFallTrails(true);
    }

    bool CheckValidRotate(Vector2 boxPosition)
    {
        // Check if is valid move left and rotate
        this.transform.position = new Vector2(boxPosition.x + 1, boxPosition.y);
        if (IsValidGridPos(this.transform))
        {
            return true;
        }

        this.transform.position = new Vector2(boxPosition.x + 2, boxPosition.y);
        if (IsValidGridPos(this.transform))
        {
            return true;
        }

        this.transform.position = new Vector2(boxPosition.x - 1, boxPosition.y);
        if (IsValidGridPos(this.transform))
        {
            return true;
        }

        this.transform.position = new Vector2(boxPosition.x - 2, boxPosition.y);
        if (IsValidGridPos(this.transform))
        {
            return true;
        }

        this.transform.position = new Vector2(boxPosition.x, boxPosition.y);
        return false;

    }

    void MoveDown()
    {
        ShowHardFallTrails(false);
        // Modify position
        transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (IsValidGridPos(this.transform))
        {
            // It's valid. Update grid.
            UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(0, 1, 0);

            // Clear filled horizontal lines
            GridGenerator.DeleteFullRows();

            // Spawn next Group
            gameController.SpawnRandomFigure();

            // Delete trail
            DestroyHardFallTrails();

            // Disable script
            enabled = false;
            GetComponent<TouchGestureGrup>().enabled = false;
            Destroy(boxSelectedObj.gameObject);
        }

        lastFall = Time.time;
        ShowHardFallTrails(true);
    }

    void FallHard()
    {
        bool fall = true;
        while (fall)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (IsValidGridPos(this.transform))
            {
                // It's valid. Update grid.
                UpdateGrid();
            }
            else
            {
                // It's not valid. revert.
                transform.position += new Vector3(0, 1, 0);

                // Clear filled horizontal lines
                GridGenerator.DeleteFullRows();

                // Spawn next Group
                gameController.SpawnRandomFigure();

                DestroyHardFallTrails();

                // Disable script
                enabled = false;
                fall = false;
                GetComponent<TouchGestureGrup>().enabled = false;
                Destroy(boxSelectedObj.gameObject);
            }
        }
    }

    bool IsValidGridPos(Transform transformObj)
    {
        foreach (Transform child in transformObj)
        {
            Vector2 v = GridGenerator.RoundVec2(child.position);

            // Not inside Border?
            if (!GridGenerator.InsideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?

            if ((int)v.y == 20 || (int)v.x == 10)
            {
                Debug.Log((int)v.x + ", " + (int)v.y);
            }

            if (GridGenerator.grid[(int)v.x, (int)v.y] != null 
                && GridGenerator.grid[(int)v.x, (int)v.y].parent != transform 
                && GridGenerator.grid[(int)v.x, (int)v.y].parent != this.transform) {
                return false;
            }
                
        }
        return true;
    }

    void UpdateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < GridGenerator.rows; ++y)
            for (int x = 0; x < GridGenerator.colums; ++x)
                if (GridGenerator.grid[x, y] != null)
                    if (GridGenerator.grid[x, y].parent == transform)
                        GridGenerator.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = GridGenerator.RoundVec2(child.position);
            GridGenerator.grid[(int)v.x, (int)v.y] = child;
        }
    }

    void UpdateSelectedPosition()
    {
        _updateSelectedPosition();
        boxSelectedObj.rotation = this.transform.rotation;
    }

    void _updateSelectedPosition()
    {
        boxSelectedObj.position = transform.position;
        //boxSelectedObj.position += new Vector3(0, -4, 0);

        bool fall = true;
        while (fall)
        {
            // Modify position
            boxSelectedObj.position += new Vector3(0, -1, 0);

            // See if valid
            if (IsValidGridPos(boxSelectedObj))
            {
                
            }
            else
            {
                // Disable script
                fall = false;

                // It's not valid. revert.
                boxSelectedObj.position += new Vector3(0, 1, 0);
            }
        }

        


        boxSelectedObj.position = new Vector3(this.transform.position.x, boxSelectedObj.position.y, 0);
    }

    void InstantiateHardFallTrails()
    {
        foreach (Transform child in transform)
        {
            var instanceTrail = Instantiate(trail, child);

            var colorChild = child.GetComponent<SpriteRenderer>().color;
            instanceTrail.GetComponent<TrailRenderer>().startColor = colorChild;
            instanceTrail.GetComponent<TrailRenderer>().endColor = new Color(colorChild.r, colorChild.g, colorChild.b, 0);

            instanceTrail.gameObject.SetActive(true);
        }
    }

    void DestroyHardFallTrails()
    {
        foreach (Transform child in transform)
        {
            child.GetChild(0).GetComponent<BoxTrail>().DestroyTrail();
        }
    }

    void ShowHardFallTrails(bool active)
    {
        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.SetActive(active);
        }
    }
}
