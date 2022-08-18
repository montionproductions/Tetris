using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    public Transform grup;
    public Transform firstPos;
    public Transform lastPos;

    public float speed = 1.5f;

    private bool isPlaying = false;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    PlayExitAnimation();
        //}

        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    PlayEnterAnimation();
        //}
    }

    public void PlayEnterAnimation()
    {
        grup.transform.position = firstPos.position;
        grup.GetComponent<UpdateGrupTile>().enabled = false;
        isPlaying = true;

        target = lastPos;
    }
    public void PlayExitAnimation()
    {
        grup.transform.position = lastPos.position;
        grup.GetComponent<UpdateGrupTile>().enabled = false;
        isPlaying = true;

        target = firstPos;
    }

    void Move()
    {
        if (!isPlaying)
            return;

        // Move our position a step closer to the target.
        grup.position = Vector3.MoveTowards(grup.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(grup.position, target.position) < 0.001f)
        {
            isPlaying = false;
            grup.position = target.position;

            if(target == firstPos) // Hide grup when animation exit plays
            {
                grup.gameObject.SetActive(false);
            }
        }
    }
}
