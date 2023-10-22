using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class levelControl : MonoBehaviour
{

    [Header("Canvas")]
    [SerializeField] Canvas topCanvas; 
    [SerializeField] Canvas bottomCanvas;
    [SerializeField] GameObject pauseCanva;

    [Header("Grid Properties")]
    [SerializeField] int gridMinSize = 4;
    [SerializeField] int gridMaxSize = 6;

    [Header("Tile GameObjects")]
    [SerializeField] GameObject cellObj;
    [SerializeField] Sprite leafSprite;
    [SerializeField] Sprite mashroomSprite;

    [Header("Level Elements")]
    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject turnsText;
    [SerializeField] GameObject[] heartImages;
    int heartInd = 0;

    [Header("Win Lose Canva Settings")]
    [SerializeField] GameObject winLoseCanvas;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite winSprite;

    [Header("Sounds")]
    [SerializeField]  Animator soundPlayingCell;
    [SerializeField] AudioClip WrongMove;
    [SerializeField] AudioClip gameOver;

    [Header("Animation Objects")]
    [SerializeField] Animator sideStatues;

    //Max and minimum percentage of tiles that can fliped in top panel
    int minflipTitles = 50;
    int maxflipTitles = 100;
  
    int cellPixel = 32, gridSize, difficulty, playerTurns ;

    //Saving sprite to reset level
    Sprite[] bottomCanvaSprites;


    AudioSource mapAudioSource;

    void Start()
    {
        if (minflipTitles <= maxflipTitles)
        {
            mapAudioSource = GetComponent<AudioSource>();
            gridSize = gridMinSize;
            difficulty = 0;
            bottomCanvaSprites = new Sprite[gridSize * gridSize];
            //Setting the score to zero 
            scoreText.GetComponent<TextMeshProUGUI>().text = "0";


            //Creating the top panel 
            createMap(topCanvas);
            soundPlayingCell.Play("playSound");

        }
        else
            Debug.Log("Mainimum tiles to flip is smaller than maximum tiles to flip.");

    }

    // Update is called once per frame
    void Update()
    {
		
    }

    void createMap(Canvas panelCanvas)
	{
        
        panelCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cellPixel * gridSize, cellPixel * gridSize);
        for(int i = 0; i<  gridSize; i++)
		{
            for(int j =0; j < gridSize; j++)
			{
                GameObject cell = Instantiate(cellObj, panelCanvas.transform);
                cell.GetComponent<Button>().interactable = false;
                cell.GetComponent<Grid>().enabled = false;
            }
        }
        Animator[] allCells = panelCanvas.GetComponentsInChildren<Animator>();
        int flipCellPercentage = Random.Range(minflipTitles, maxflipTitles);
        int cellsToFlip = (flipCellPercentage * gridSize * gridSize) / 100;

        int cellNum;
        for (int i = 0; i < cellsToFlip; i++)
		{
            
            do
            {
                cellNum = Random.Range(0, gridSize * gridSize - 1);
            } while (allCells[cellNum].GetComponent<Image>().sprite == mashroomSprite);
            allCells[cellNum].SetInteger("Anim",2);
            allCells[cellNum].SetTrigger("Trig");
            allCells[cellNum].GetComponent<Image>().sprite = mashroomSprite;
        }

        createBottomPanel();

    }


    bool cellMatch(int i, int[] cells)
    {
        bool match = true;
        if (i == 0)
            match = false;
        else if (i == 1)
        {
            if (cells[i] != cells[i - 1])
                match = false;
        }
        else if (i == 2)
        {
            if (cells[i] != cells[i - 1] && cells[i] != cells[i - 2])
                match = false;
        }
        return match;

    }


    void flipInBottomPanel()
	{
        int[] cells;
        cells = new int[playerTurns];
        Animator[] allCells = bottomCanvas.GetComponentsInChildren<Animator>();
        for(int i = 0; i < playerTurns; i++)
		{
            do
            {
                cells[i] = Random.Range(0, gridSize * gridSize - 1);
            } while (cellMatch(i,cells));
            flipRespCells(cells[i], allCells, false);

        }
        savePanel(true);
	}

    void savePanel(bool savePanel)  //true = save panel, false = reset panel
	{
        Image[] cells = bottomCanvas.GetComponentsInChildren<Image>();
        for(int row =0;row < gridSize; row++)
		{
            for(int col = 0; col < gridSize; col++)
			{
                if(savePanel)
                    bottomCanvaSprites[row * gridSize + col] = cells[row*gridSize+col].sprite;
                else
				{
                    cells[row * gridSize + col].sprite = bottomCanvaSprites[row * gridSize + col];
                    if (bottomCanvaSprites[row * gridSize + col] == mashroomSprite)
                        cells[row * gridSize + col].GetComponent<Animator>().SetInteger("Anim",1);
                    else
                        cells[row * gridSize + col].GetComponent<Animator>().SetInteger("Anim", 0);
                    cells[row * gridSize + col].GetComponent<Animator>().SetTrigger("Trig");
                }
                    
            }
		}
	}
    void flipRespCells(int cellNum, Animator[] allCells, bool playAnim)
	{
        int col = cellNum % gridSize;
        int row = (cellNum / gridSize) % gridSize;
        flipRespCell(allCells[(row ) * gridSize + (col )], playAnim);

        if (row -1 >= 0 && col-1 >= 0)
		{
            flipRespCell(allCells[(row - 1) *gridSize +(col - 1)],playAnim);
		}
        if (row + 1 < gridSize && col + 1 < gridSize)
        {
            flipRespCell(allCells[(row + 1) * gridSize + (col + 1)], playAnim);
        }
        if (row - 1 >= 0 )
        {
            flipRespCell(allCells[(row - 1) * gridSize + (col )], playAnim);
        }
        if (col - 1 >= 0)
        {
            flipRespCell(allCells[(row ) * gridSize + (col - 1)], playAnim);
        }
        if (row - 1 >= 0 && col + 1 < gridSize)
        {
            flipRespCell(allCells[(row - 1) * gridSize + (col + 1)], playAnim);
        }
        if (row + 1 < gridSize && col - 1 >= 0)
        {
            flipRespCell(allCells[(row + 1) * gridSize + (col - 1)], playAnim);
        }
        if (row +1 < gridSize)
        {
            flipRespCell(allCells[(row + 1) * gridSize + (col )], playAnim);
        }
        if (col + 1 < gridSize)
        {
            flipRespCell(allCells[(row ) * gridSize + (col + 1)], playAnim);
        }

    }

    void flipRespCell(Animator cell, bool playAnim)
	{
        if (playAnim) playCellAnim(cell);
        else playIdleAnim(cell);
        flipCellSprite(cell);
    }
    void flipCellSprite(Animator cell)
	{
        if (cell.GetComponent<Image>().sprite == mashroomSprite)
            cell.GetComponent<Image>().sprite = leafSprite;
        else
            cell.GetComponent<Image>().sprite = mashroomSprite;

    }
    int dicideTurns()
	{
        int turns = 0;
        if (difficulty == 0 || difficulty == 1)
            turns = 1;
        else if (difficulty == 2 || difficulty == 3)
            turns = 2;
        else if (difficulty == 4 || difficulty == 5)
            turns = 3;
        return turns;
    }

    void createBottomPanel()
	{
        bottomCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cellPixel * gridSize, cellPixel * gridSize);
        Image[] allCells = topCanvas.GetComponentsInChildren<Image>();
        for (int row = 0; row < gridSize; row++)
		{
            for(int col =0; col < gridSize; col++)
			{
                GameObject cell = Instantiate(cellObj, bottomCanvas.transform);
                cell.GetComponent<Grid>().x = row;
                cell.GetComponent<Grid>().y = col;
                if(allCells[row*gridSize + col].sprite == mashroomSprite)
				{
                    cell.GetComponent<Image>().sprite = mashroomSprite;
                    cell.GetComponent<Animator>().SetInteger("Anim", 1);
                    cell.GetComponent<Animator>().SetTrigger("Trig");
                }
            }
		}
        playerTurns = dicideTurns();

        //Setting the turns
        turnsText.GetComponent<TextMeshProUGUI>().text = playerTurns.ToString();

        flipInBottomPanel();
	}
    void playCellAnim(Animator cell)
	{
        soundPlayingCell.Play("playSound");
        cell.SetTrigger("Trig");
        if (cell.GetComponent<Image>().sprite == mashroomSprite)
            cell.SetInteger("Anim", 3);
        else
            cell.SetInteger("Anim", 2);


    }
    void playIdleAnim(Animator cell)
    {
        cell.SetTrigger("Trig");
        if (cell.GetComponent<Image>().sprite == mashroomSprite)
            cell.SetInteger("Anim", 0);
        else
            cell.SetInteger("Anim", 1);
            
    }
    public void flipCellsFunc(int x, int y)
	{
        Animator[] allCells = bottomCanvas.GetComponentsInChildren<Animator>();
        flipRespCells(x * gridSize + y, allCells, true);
        checkWinLose();
	}

    void checkWinLose()
	{
        Debug.Log(heartInd);
        playerTurns--;
        //Setting the turns
        turnsText.GetComponent<TextMeshProUGUI>().text = playerTurns.ToString();
        //Pausing Input
        winLoseCanvas.SetActive(true);


        if (playerTurns == 0)
        {
            Image[] topPanelImages = topCanvas.GetComponentsInChildren<Image>();
            Image[] bottomPanelImages = bottomCanvas.GetComponentsInChildren<Image>();
            
            for(int row = 0; row < gridSize; row++ )
			{
                for (int col =0; col < gridSize; col++)
				{
                    if(topPanelImages[row*gridSize+col].sprite != bottomPanelImages[row * gridSize + col].sprite)
					{
                        begingLoseSequence();
                        return;
					}
				}
			}

        }
		else
		{
            StartCoroutine(disWinLoseCanva(false));
		}
    }
    void begingLoseSequence()
	{
        heartImages[heartInd].SetActive(false);
        heartInd++;
        if(heartInd < 3)
		{
            
            StartCoroutine(disWinLoseCanva(true));
		}
		else
		{
            StartCoroutine(startGameOverSequence());
		}
	}

    void resetLevel()
	{
        playerTurns = dicideTurns();
        //Setting the turns
        turnsText.GetComponent<TextMeshProUGUI>().text = playerTurns.ToString();
        savePanel(false);
    }
    IEnumerator disWinLoseCanva(bool reset)
	{
        yield return new WaitForSeconds(0.95f);
        winLoseCanvas.SetActive(false);
        if (reset)
        {
            mapAudioSource.clip = WrongMove;
            sideStatues.Play("resetAnim");
            mapAudioSource.Play();
            resetLevel();
        }
    }

    public void setPanelsActive(bool active)
	{
        int offset = 900;
        if (active)
            offset *= 1;
        else
            offset *= -1;
        topCanvas.GetComponent<RectTransform>().localPosition += Vector3.right * offset;
        bottomCanvas.GetComponent<RectTransform>().localPosition += Vector3.right * offset;
    }

    IEnumerator startGameOverSequence()
	{
        yield return new WaitForSeconds(0.95f);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = loseSprite;
        GetComponent<pauseScript>().enabled = false;
        setPanelsActive(false);

        mapAudioSource.clip = gameOver;
        mapAudioSource.Play();

        yield return new WaitForSeconds(2);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = emptySprite;
        pauseCanva.SetActive(true);
    }

    
    public void onclickPlayAgain()
	{
        gridSize = gridMinSize;
        difficulty = 0;

        //Setting the score to zero 
        scoreText.GetComponent<TextMeshProUGUI>().text = "0";

        //Destorying all child objects in panels
        destroyChildObjs();

        //Restoring all lives
        heartInd = 0;
        heartImages[0].SetActive(true);
        heartImages[1].SetActive(true);
        heartImages[2].SetActive(true);

        setPanelsActive(true);

        //Creating the top panel 
        createMap(topCanvas);
        soundPlayingCell.Play("playSound");
    }

	void destroyChildObjs()
	{
        Transform topTransform = topCanvas.transform;
        Transform bottomTransform = bottomCanvas.transform;
        foreach (Transform child in topTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in bottomTransform)
        {
            Destroy(child.gameObject);
        }
    }

}
