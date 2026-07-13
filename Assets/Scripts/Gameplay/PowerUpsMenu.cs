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
            powerUpsCounter[i] = 0;
    }

    private void Start()
    {
        RefreshUnlockedPowerUps();
    }

    private void OnEnable()
    {
        PowerUpUnlockManager.OnPowerUpUnlocked += HandlePowerUpUnlocked;
    }

    private void OnDisable()
    {
        PowerUpUnlockManager.OnPowerUpUnlocked -= HandlePowerUpUnlocked;
    }

    private void HandlePowerUpUnlocked(PowerUpDefinition definition)
    {
        if (definition == null) return;
        DragAndDropElement.PowerUpType type = ToGameplayType(definition.type);
        if (powerUpsCounter[(int)type] == 0)
            powerUpsCounter[(int)type] = 1;
        _updatePowerUp(type);
    }

    private void RefreshUnlockedPowerUps()
    {
        if (PowerUpUnlockManager.I == null) return;
        foreach (PowerUpDefinition definition in PowerUpUnlockManager.I.PowerUps)
            if (definition != null && PowerUpUnlockManager.I.IsUnlocked(definition.id))
                HandlePowerUpUnlocked(definition);
    }

    private static DragAndDropElement.PowerUpType ToGameplayType(global::PowerUpType type)
    {
        if (type == global::PowerUpType.ClearColumn) return DragAndDropElement.PowerUpType.DeleteColum;
        if (type == global::PowerUpType.ClearRow) return DragAndDropElement.PowerUpType.DeleteRow;
        return DragAndDropElement.PowerUpType.CompleteRow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddPowerUp(DragAndDropElement.PowerUpType powerType)
    {
        if (!IsUnlocked(powerType)) return;
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
        else
        {
            notifications[idPowerUp].gameObject.SetActive(false);
            for (int i = spawnPointsPowerUps[idPowerUp].childCount - 1; i >= 0; i--)
                Destroy(spawnPointsPowerUps[idPowerUp].GetChild(i).gameObject);
        }
    }

    private static bool IsUnlocked(DragAndDropElement.PowerUpType type)
    {
        if (PowerUpUnlockManager.I == null) return false;
        string id = type == DragAndDropElement.PowerUpType.DeleteColum
            ? "powerup_clear_column"
            : type == DragAndDropElement.PowerUpType.DeleteRow
                ? "powerup_clear_row"
                : "powerup_fill_cell";
        return PowerUpUnlockManager.I.IsUnlocked(id);
    }
}
