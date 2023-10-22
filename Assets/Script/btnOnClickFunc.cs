using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btnOnClickFunc : MonoBehaviour
{

    levelControl mainCanva;
    // Start is called before the first frame update
    void Start()
    {
        mainCanva = FindObjectOfType<levelControl>();
        if(GetComponent<Button>() != null) GetComponent<Button>().onClick.AddListener(delegate { callCanvaFlipFunc(); });
    }

    void callCanvaFlipFunc()
	{
        Grid cellGrid = GetComponent<Grid>();
        mainCanva.flipCellsFunc(cellGrid.x, cellGrid.y);
    }
    
    void playSound()
	{
        GetComponent<AudioSource>().Play();
	}

}
