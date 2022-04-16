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

    private IEnumerator UpdatePosition(float time)
    {
        yield return new WaitForSeconds(time);
        transform.position += new Vector3(0, -1, 0);
    }

    public void DeleteBox(float timeToDestroy)
    {
        StartCoroutine("DisoveAnimation", 0.1f);
        Destroy(this.gameObject, timeToDestroy);
        Debug.Log("Box destroyed");
    }

    public void MoveDown()
    {
        StartCoroutine("UpdatePosition", 1.0f);
    }
}
