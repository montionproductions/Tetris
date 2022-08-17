using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PointGizmo : MonoBehaviour
{
    public Transform firstPoint;
    public Transform lastPoint;
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(firstPoint.position, 1);
        Gizmos.DrawSphere(lastPoint.position, 1);

        if (firstPoint != null && lastPoint != null)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firstPoint.position, lastPoint.position);
        }
    }
}
