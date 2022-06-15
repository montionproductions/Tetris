using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject powerUpMenu;

    public Transform[] powerUps = new Transform[3];
    public Transform[] spawnPointsPowerUps = new Transform[3];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // Show PowerUps Menu
        ShowPowerUpsMenu(true);
    }

    public void ShowPowerUpsMenu(bool active)
    {
        powerUpMenu.SetActive(active);

        GetComponent<SpriteRenderer>().enabled = !active;
    }

    public void InstantiatePowerUp(DragAndDropElement.PowerUpType powerTime)
    {
        _addPowerUp(powerTime);
    }

    private void _addPowerUp(DragAndDropElement.PowerUpType powerTime)
    {
        int idPowerUp = (int)powerTime;

        if (spawnPointsPowerUps[idPowerUp].childCount > 0)
        {
            Debug.Log("PowerUpsMenu: Add power up");
        } else
        {
            var obj = Instantiate(powerUps[idPowerUp], spawnPointsPowerUps[idPowerUp]);
            //obj.transform.position = new Vector3(0f, 0f, 0f);
        }

    }
}
