using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPowerUp : MonoBehaviour
{
    public enum PowerUpType {
        CompleteRow = 0,
        CompleteColum = 1,
        RemoveColor
    };

    public PowerUpType powerUpType;

    private PowerUpsMenu menu;

    private void Awake()
    {
        menu = GameObject.FindObjectOfType<PowerUpsMenu>();
    }

    private void OnMouseEnter()
    {
        // Instantiate powerup
        var mousePos = GameObject.FindObjectOfType<Camera>().ScreenToWorldPoint( Input.mousePosition );
        Instantiate(menu.powerUps[(int)powerUpType], new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);

        // Hide menu
        //menu.ShowPowerUpsMenu(false);
    }
}
