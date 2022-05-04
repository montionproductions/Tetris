using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject powerUpMenu;

    public Transform[] powerUps = new Transform[3];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            ShowPowerUpsMenu(false);
        }
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
}
