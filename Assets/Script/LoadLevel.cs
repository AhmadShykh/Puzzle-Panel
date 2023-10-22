using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadInstructions : MonoBehaviour
{
    public void loadNextLevel(int num)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + num);
	}
}
