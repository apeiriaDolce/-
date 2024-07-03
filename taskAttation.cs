using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class taskAttation : MonoBehaviour
{
    public static taskAttation Instance;

    public float moveSize;
    public Coroutine moveCoro;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        moveCoro = StartCoroutine(objectMove());
    }

    IEnumerator objectMove()
    {
        int i = 0;
        Vector3 startPos = transform.localPosition;
        while (true)
        {
            if (i <= 1)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - moveSize, transform.localPosition.z);
                i++;
            }
            else
            {
                i = 0;
                transform.localPosition = startPos;
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
    public void stop()
    {
        StopCoroutine(moveCoro);
    }
    public void start()
    {
        moveCoro= StartCoroutine(objectMove());
    }
}
