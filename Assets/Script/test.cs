using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{

    [SerializeField] Sprite mashroom;
    [SerializeField] Sprite leaf;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
		{
            //GetComponent<Animator>().Play("tilesAnim1");
            GetComponent<Image>().sprite = mashroom;
		}
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //GetComponent<Animator>().Play("tilesAnim2");
            GetComponent<Image>().sprite = leaf;
        }
    }
}
