using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrail : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyTrail()
    {
        Destroy(this.gameObject, 0.2f);
    }
}
