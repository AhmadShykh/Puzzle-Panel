using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyObject : MonoBehaviour
{
    public GameObject objToDestroy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(finishObj());
    }

    IEnumerator finishObj()
	{
        yield return new WaitForSeconds(3);
        Destroy(objToDestroy);
	}
}
