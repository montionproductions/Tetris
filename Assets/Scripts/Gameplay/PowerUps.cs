using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    private GridGenerator gridGenerator;

    private void Awake()
    {
        gridGenerator = GameObject.FindObjectOfType<GridGenerator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static Vector2 PutBox(Transform box)
    {
        // Add new children to grid
        Vector2 v = GridGenerator.RoundVec2(box.position);
        GridGenerator.grid[(int)v.x, (int)v.y] = box;

        return v;
    }

    public static void DeleteRow(Transform box)
    {
        // Add box to grid
        Vector2 v = PutBox(box);
        box.position = new Vector3(v.x, v.y, 0);

        // Clear filled horizontal lines
        GridGenerator.DeleteFullRows();
    }

    public static void DeleteColum(Transform box)
    {
        Vector2 v = GridGenerator.RoundVec2(box.position);
        GridGenerator.DeleteColum((int)v.x);
    }

    public static bool IsValidGridPos(Transform box)
    {

        Vector2 v = GridGenerator.RoundVec2(box.position);

        if (!GridGenerator.InsideBorder(v))
            return false;

        if (box.position.y > 19)
            return false;

        // Block in grid cell (and not part of same group)?
        if (GridGenerator.grid[(int)v.x, (int)v.y] != null) {
            return false;
        }

        return true;
    }

    public static bool IsValidColum(Transform box)
    {
        Vector2 v = GridGenerator.RoundVec2(box.position);

        if (!GridGenerator.InsideBorder(v))
            return false;

        if (box.position.y > 19 || box.position.y < 0)
            return false;
    
        return true;
    }
}
