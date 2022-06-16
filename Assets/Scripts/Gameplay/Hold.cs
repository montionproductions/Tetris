using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    // Start is called before the first frame update
    Grup.FigureType currentHold;
    Transform currentPreview;

    public Transform spawnPoint;
    public Transform[] previews = new Transform[7];

    private void OnMouseUp()
    {
        Grup.FigureType type = Game.currentFigure.GetComponent<Grup>().figureType;
        ChangeHoldFigure(type);
    }
    void ChangeHoldFigure(Grup.FigureType figure)
    {
        if(currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
            currentPreview = Instantiate(previews[(int)figure], spawnPoint);
            Game.currentFigure.GetComponent<Grup>().RemoveFigureFromGrid();
            Game.gameInstance.SpawnFigure(currentHold);

        } else
        {
            currentPreview = Instantiate(previews[(int)figure], spawnPoint);
            Game.currentFigure.GetComponent<Grup>().RemoveFigureFromGrid();
            Game.gameInstance.SpawnRandomFigure();
        }

        currentHold = figure;            
    }
}
