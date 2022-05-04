using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Game game;

    public bool isStatic = false;
    public float disolveVelocity = 0.1f;

    private void Awake()
    {
        game = GameObject.FindObjectOfType<Game>();
        isStatic = false;
    }

    private IEnumerator DisoveAnimation(float disolveValue)
    {
        var currentColor = GetComponent<SpriteRenderer>().color;

        for (float alpha = 1; alpha > 0; alpha -= disolveValue)
        {
            GetComponent<SpriteRenderer>().color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return new WaitForSeconds(disolveVelocity);
        }
    }

    private IEnumerator ScaleAnimation(float scaleValue)
    {
        for (float scale = 1; scale < 1.2f; scale += scaleValue)
        {
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForSeconds(disolveVelocity);
        }
    }

    private IEnumerator UpdatePosition(float time)
    {
        yield return new WaitForSeconds(time);
        transform.position += new Vector3(0, -1, 0);
    }

    private IEnumerator StartAnimation(float timeToStart)
    {
        yield return new WaitForSeconds(timeToStart);
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GetComponent<SpriteRenderer>().sortingOrder = 3;
        StartCoroutine("DisoveAnimation", 0.1f);
        StartCoroutine("ScaleAnimation", 0.025f);

    }

    public void DeleteBox(float timeToStart, float timeToDestroy)
    {
        StartCoroutine("StartAnimation", timeToStart);
        Destroy(this.gameObject, timeToDestroy);
        //Debug.Log("Box destroyed");
    }

    public void MoveDown()
    {
        StartCoroutine("UpdatePosition", 0.35f);
    }
}
