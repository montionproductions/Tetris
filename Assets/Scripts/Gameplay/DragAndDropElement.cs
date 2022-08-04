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
        DeleteRow = 2
    }

    void OnMouseEnter()
    {
        originalColor = GetComponent<Renderer>().material.color;
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
        Time.timeScale = .15f;
    }

    void OnMouseUp()
    {
        dragging = false;
        Time.timeScale = 1.0f;

        if (GameObject.FindObjectOfType<TouchGestureGrup>() != null)
            GameObject.FindObjectOfType<TouchGestureGrup>().enabled = true;

        switch (powerUpType)
        {
            case PowerUpType.CompleteRow:
                PowerUp_CompleteRow();
                break;
            case PowerUpType.DeleteColum:
                PowerUp_DeleteColum();
                break;
            case PowerUpType.DeleteRow:
                PowerUp_DeleteRow();
                break;
        }  
    }

    private void PowerUp_CompleteRow()
    {
        if (PowerUps.IsValidGridPos(transform))
        {
            PowerUps.CompleteRow(transform);
            transform.parent = null;

            GameObject.FindObjectOfType<PowerUpsMenu>().InstantiatePowerUp(PowerUpType.CompleteRow);

            Game.currentFigure.GetComponent<Grup>().UpdateGrup();

            Destroy(this);
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
            transform.parent = null;

            //Game.powerUpsMenu.InstantiatePowerUp(PowerUpType.DeleteColum);
            GameObject.FindObjectOfType<PowerUpsMenu>().InstantiatePowerUp(PowerUpType.DeleteColum);

            Game.currentFigure.GetComponent<Grup>().UpdateGrup();

            Destroy(this.gameObject);
        }
        else
        {
            transform.position = startPost;
        }
    }

    private void PowerUp_DeleteRow()
    {
        if(PowerUps.IsValidRow(transform))
        {
            PowerUps.DeleteRow(transform);
            transform.parent = null;

            //Game.powerUpsMenu.InstantiatePowerUp(PowerUpType.DeleteRow);
            GameObject.FindObjectOfType<PowerUpsMenu>().InstantiatePowerUp(PowerUpType.DeleteRow);

            Game.currentFigure.GetComponent<Grup>().UpdateGrup();

            Destroy(this.gameObject);
        }
        else
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
            transform.position = new Vector3(rayPoint.x, rayPoint.y, 0.0f);
        }
    }

    private void OnDestroy()
    {
        //GameObject.FindObjectOfType<TouchGestureGrup>().enabled = true;
    }
}
