using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseScript : MonoBehaviour
{
    [SerializeField] GameObject PauseCanva;
    bool active;

	void Start()
	{
        active = true;
	}


	// Update is called once per frame
	void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
		{
            if(active)
			{
                PauseCanva.SetActive(true);
                GetComponent<levelControl>().setPanelsActive(false);
                active = false;
            }
			else
			{
                PauseCanva.SetActive(false);
                GetComponent<levelControl>().setPanelsActive(true);
                active = true;
            }
		}
        
    }
}
