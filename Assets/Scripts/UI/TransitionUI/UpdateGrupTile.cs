using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdateGrupTile : MonoBehaviour
{
    public Transform firstPosition;
    public Transform lastPosition;

    public bool updatePos = false;

    // Update is called once per frame
    void Update()
    {
        if (firstPosition != null && !updatePos)
        {
            transform.position = firstPosition.position;
        }

        if (lastPosition != null && updatePos)
        {
            transform.position = lastPosition.position;
        }
    }
}
