using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grup : MonoBehaviour
{
    public Game gameController;

    float lastFall = 0;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = GameObject.FindObjectOfType<Game>();
    }

    void Start()
    {
        // Default position not valid? Then it's game over
        if (!IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
            gameController.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.isPaused)
            return;

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
        }// Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2.right);
        }// Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }// Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1)
        {
            MoveDown();
        } // Fall hard
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            FallHard();
        }
    }

    void Move(Vector2 dir)
    {
        Vector2 norDir = dir;
        // Modify position
        transform.position += new Vector3(norDir.x, norDir.y, 0);

        // See if it's valid
        if (IsValidGridPos())
            // It's valid. Update grid.
            UpdateGrid();
        else
            // Its not valid. revert.
            transform.position += new Vector3(-norDir.x, -norDir.y, 0);
    }

    void Rotate()
    {
        transform.Rotate(0, 0, -90);

        // See if valid
        if (IsValidGridPos())
            // It's valid. Update grid.
            UpdateGrid();
        else
            // It's not valid. revert.
            transform.Rotate(0, 0, 90);

    }

    void MoveDown()
    {
        // Modify position
        transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (IsValidGridPos())
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

            // Disable script
            enabled = false;
            GetComponent<TouchGestureGrup>().enabled = false;
        }

        lastFall = Time.time;
    }

    void FallHard()
    {
        bool fall = true;
        while (fall)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if valid
            if (IsValidGridPos())
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

                // Disable script
                enabled = false;
                fall = false;
                GetComponent<TouchGestureGrup>().enabled = false;
            }
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
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

            if (GridGenerator.grid[(int)v.x, (int)v.y] != null && GridGenerator.grid[(int)v.x, (int)v.y].parent != transform) {
                return false;
            }
                
        }
        return true;
    }

    void UpdateGrid()
    {
        //Debug.Log("Update grup");
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
}
