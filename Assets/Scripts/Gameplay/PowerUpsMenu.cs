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

    public void InstantiatePowerUp(DragAndDropElement.PowerUpType powerType)
    {
        if (powerUpsCounter[(int)powerType] > 0)
            _addPowerUp(powerType);

        powerUpsCounter[(int)powerType]--;

        if (powerUpsCounter[(int)powerType] < 0)
            powerUpsCounter[(int)powerType] = 0;
    }

    private void _addPowerUp(DragAndDropElement.PowerUpType powerTime)
    {
        int idPowerUp = (int)powerTime;

        if (spawnPointsPowerUps[idPowerUp].childCount >= 1)
        {
            // Show circle
            notifications[idPowerUp].gameObject.SetActive(true);
            // Update counter
            powerUpsCounter[idPowerUp]++;
            notifications[idPowerUp].GetChild(0).GetComponent<TMP_Text>().text = powerUpsCounter[idPowerUp].ToString();
        } if (spawnPointsPowerUps[idPowerUp].childCount <= 0)
        {
            // Hide circle
            notifications[idPowerUp].gameObject.SetActive(false);

            var obj = Instantiate(powerUps[idPowerUp], spawnPointsPowerUps[idPowerUp]);
        }

    }
}
