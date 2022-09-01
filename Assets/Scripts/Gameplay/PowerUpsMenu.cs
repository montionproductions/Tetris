using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] powerUps = new Transform[3];
    public Transform[] spawnPointsPowerUps = new Transform[3];
    public Transform[] notifications = new Transform[3];
    static public int[] powerUpsCounter = new int[3];

    void Awake()
    {
        for(int i = 0; i < powerUpsCounter.Length; i++)
            powerUpsCounter[i] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddPowerUp(DragAndDropElement.PowerUpType powerType)
    {
        powerUpsCounter[(int)powerType]++;
        _updatePowerUp(powerType);
    }
    public void RemovePowerUp(DragAndDropElement.PowerUpType powerType)
    {
        powerUpsCounter[(int)powerType]--;

        if (powerUpsCounter[(int)powerType] < 0)
            powerUpsCounter[(int)powerType] = 0;

        _updatePowerUp(powerType);
    }

    private void _updatePowerUp(DragAndDropElement.PowerUpType powerType)
    {
        int idPowerUp = (int)powerType;

        if (powerUpsCounter[idPowerUp] > 1) // Spawn power up with the badged
        {
            // Show circle
            notifications[idPowerUp].gameObject.SetActive(true);
            // Update counter
            notifications[idPowerUp].GetChild(0).GetComponent<TMP_Text>().text = powerUpsCounter[idPowerUp].ToString();
            // Spawn powerup
            if (spawnPointsPowerUps[idPowerUp].childCount == 0)
                Instantiate(powerUps[idPowerUp], spawnPointsPowerUps[idPowerUp]);
        } else if(powerUpsCounter[idPowerUp] == 1)
        {
            // Hide circle
            notifications[idPowerUp].gameObject.SetActive(false);
            // Spawn powerup
            if (spawnPointsPowerUps[idPowerUp].childCount == 0)
                Instantiate(powerUps[idPowerUp], spawnPointsPowerUps[idPowerUp]);
        }
    }
}
