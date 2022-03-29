using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GridController : MonoBehaviour
{
    RectTransform GridBackground;
    RectTransform Grid;
    RectTransform[] GridLines;

    public float min_with_box = 60;

    private void Awake()
    {
        GridBackground = GameObject.Find("MainBackground").GetComponent<RectTransform>();
        Grid = GameObject.Find("Grid").GetComponent<RectTransform>();

        var nLines = Grid.transform.childCount;
        GridLines = new RectTransform[nLines];
        for (int i = 0; i < nLines; i++)
        {
            GridLines[i] = Grid.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeHeightBox();
    }

    // Control box height
    void ChangeHeightBox()
    {
        foreach(RectTransform currentLine in GridLines) {
            //Debug.Log("Box rect: " + currentLine.GetChild(0).GetComponent<RectTransform>().sizeDelta);
            //Debug.Log("Line rect: " + currentLine.sizeDelta);

            Vector2 boxRect = currentLine.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            if(currentLine.sizeDelta.x > min_with_box && boxRect.x > min_with_box)
                currentLine.sizeDelta = new Vector2(currentLine.sizeDelta.x, boxRect.x);
        }
    }
}
