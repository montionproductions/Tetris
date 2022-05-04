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

    public PowerUpType powerUpType;

    public enum PowerUpType
    {
        CompleteRow = 0,
        DeleteColum = 1,
        DeleteColor = 3
    }

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = mouseOverColor;

        if(GameObject.FindObjectOfType<TouchGestureGrup>() != null)
            GameObject.FindObjectOfType<TouchGestureGrup>().enabled = false;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = originalColor;

        if (GameObject.FindObjectOfType<TouchGestureGrup>() != null)
            GameObject.FindObjectOfType<TouchGestureGrup>().enabled = true;
    }

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
        startPost = transform.position;
        Time.timeScale = .3f;
    }

    void OnMouseUp()
    {
        dragging = false;
        Time.timeScale = 1.0f;

        switch(powerUpType)
        {
            case PowerUpType.CompleteRow:
                PowerUp_CompleteRow();
                break;
            case PowerUpType.DeleteColum:
                PowerUp_DeleteColum();
                break;
            case PowerUpType.DeleteColor:
                PowerUp_DeleteColor();
                break;
        }  
    }

    private void PowerUp_CompleteRow()
    {
        if (PowerUps.IsValidGridPos(transform))
        {
            PowerUps.DeleteRow(transform);
            enabled = false;
        }
        else
        {
            transform.position = startPost;
        }
    }

    private void PowerUp_DeleteColum()
    {
        // Implements
        if (PowerUps.IsValidColum(transform))
        {
            PowerUps.DeleteColum(transform);
            Destroy(this.gameObject);
        }
        else
        {
            transform.position = startPost;
        }
    }

    private void PowerUp_DeleteColor()
    {
        // Implements
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
