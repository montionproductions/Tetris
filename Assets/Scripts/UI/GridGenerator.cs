using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
    public float displacement = .95f;
    public static int rows = 20;
    public static int colums = 10;
    public Vector2 gridPosition;
    public bool clearGrid = false;
    public bool updateGrid = false;
    public static Transform[,] grid;

    public Transform box;

    // Start is called before the first frame update
    private void Start()
    {
        grid = new Transform[colums, rows];
    }

    void OnEnable()
    {
        box = GameObject.Find("BoxBg").transform;

        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(clearGrid)
        {
            RemoveGrid();
        }
    }

    void CreateGrid()
    {
        if (!updateGrid)
            return;

        if(box == null)
        {
            Debug.Log("BoxBg not found!");
            return;
        }

        RemoveGrid();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                Vector3 boxPos = new Vector3(x * displacement, y * displacement, 0);
                Transform newBox = Instantiate(box, boxPos, Quaternion.identity);
                newBox.GetComponent<SpriteRenderer>().enabled = true;
                newBox.SetParent(this.transform, false);
            }
        }

        this.transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
    }

    void RemoveGrid()
    {
        var tempArray = new GameObject[this.transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = this.transform.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            DestroyImmediate(child);
        }
    }

    public static Vector2 RoundVec2(Vector2 v)
    {
       return new Vector2(Mathf.Round(v.x),
                          Mathf.Round(v.y));
    }

    public static bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < colums && (int)pos.y >= 0);
    }

    public static void DeleteRow(int row)
    {
        for (int elementRow = 0; elementRow < colums; ++elementRow)
        {
            //Destroy(grid[elementRow, row].gameObject);
            grid[elementRow, row].GetComponent<Box>().DeleteBox((elementRow * 0.01f), 0.35f + (elementRow * 0.01f));
            grid[elementRow, row] = null;
        }
    }

    public static void DecreaseRow(int row)
    {
        for (int elementRow = 0; elementRow < colums; ++elementRow)
        {
            if (grid[elementRow, row] != null)
            {
                // Move one towards bottom
                grid[elementRow, row - 1] = grid[elementRow, row];
                grid[elementRow, row] = null;

                // Update Block position
                grid[elementRow, row - 1].gameObject.SendMessage("MoveDown");
            }
        }
    }

    public static void DecreaseRowsAbove(int row)
    {
        for (int elementRow = row; elementRow < rows; ++elementRow)
        {
            DecreaseRow(elementRow);
        }
    }

    public static bool IsRowFull(int row)
    {
        for (int x = 0; x < colums; ++x)
            if (grid[x, row] == null)
                return false;
        return true;
    }

    public static void DeleteFullRows()
    {
        var linePosition = 0;

        for (int y = 0; y < rows; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                GameObject.FindObjectOfType<UIController>().AddLine(1);
                linePosition = y;
                Game._linesCounter++;
                --y;
            }
        }

        if (Game._linesCounter == 4)
        {
            Game.On4LinesWin(linePosition);
        }

        if (Game._linesCounter == 2)
        {
            Game.On2LinesWin(linePosition);
        }

        if (Game._linesCounter == 3)
        {
            Game.On3LinesWin(linePosition);
        }

        Game._linesCounter = 0;
    }

    public static void DeleteAllBoxes()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int elementColum = 0; elementColum < colums; elementColum++)
            {
                //Destroy(grid[elementRow, row].gameObject);
                if (grid[elementColum, y] != null)
                {
                    grid[elementColum, y].GetComponent<Box>().RemoveBox((elementColum * 0.01f), 0.35f + (elementColum * 0.01f));
                    grid[elementColum, y] = null;
                }
            }
        }
    }

    public static void DeleteColum(int elementRow)
    {
        for (int elementColum = 0; elementColum < rows; ++elementColum)
        {
            //Destroy(grid[elementRow, row].gameObject);
            if (grid[elementRow, elementColum] != null && grid[elementRow, elementColum].parent != Game.currentFigure)
            {
                grid[elementRow, elementColum].GetComponent<Box>().DeleteBox((elementColum * 0.01f), 0.35f + (elementColum * 0.01f));
                grid[elementRow, elementColum] = null;
            }
        }
    }
}
