using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropElement : MonoBehaviour
{
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.yellow;
    private bool dragging = false;
    private float distance;
    private Vector3 startPost;

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = mouseOverColor;
        GameObject.FindObjectOfType<TouchGestureGrup>().enabled = false;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = originalColor;
        GameObject.FindObjectOfType<TouchGestureGrup>().enabled = true;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
        startPost = transform.position;
    }

    void OnMouseUp()
    {
        dragging = false;
        if(PowerUps.IsValidGridPos(transform)) {
            Vector2 v = PowerUps.CompleteRow(transform);
            transform.position = new Vector3(v.x, v.y, 0);

            // Clear filled horizontal lines
            GridGenerator.DeleteFullRows();

            enabled = false;

        } else
        {
            transform.position = startPost;
        }
    }

    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<TouchGestureGrup>().enabled = true;
    }
}
