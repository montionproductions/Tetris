using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResponsiveUI : MonoBehaviour
{
    public float maxSize;
    public float minSize;

    private Camera mainCamera;
    private void OnEnable()
    {
        mainCamera = GameObject.FindObjectOfType<Camera>();

        Debug.Log("Width: " + Screen.width.ToString() + ", Height: " + Screen.height.ToString());
    }
}
