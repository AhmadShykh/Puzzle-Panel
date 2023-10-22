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
    [SerializeField] GameObject startObject;
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
    [SerializeField] AudioClip clearLevelAudio;

    [Header("Animation Objects")]
    [SerializeField] Animator sideStatues;

    //Max and minimum percentage of tiles that can fliped in top panel
    int minflipTitles = 50;
    int maxflipTitles = 100;
  
    int cellPixel = 32, gridSize, difficulty, playerTurns, level, score ;

    //Checking If the panels are hidden or not
    bool panelsActive = true ;
    
    //Saving sprite to reset level
    Sprite[] bottomCanvaSprites;


    AudioSource mapAudioSource;

    void Start()
    {
        if (minflipTitles <= maxflipTitles)
        {
            
            mapAudioSource = GetComponent<AudioSource>();
            
            
            
            //Set them each time playAgain or win
            level = 1;
            gridSize = gridMaxSize;
            difficulty = 0;
            score = 15;
            heartInd = 0;

            //Setting size of the save panel
            bottomCanvaSprites = new Sprite[gridSize * gridSize];

            startLevel();

        }
        else
            Debug.Log("Mainimum tiles to flip is smaller than maximum tiles to flip.");

    }
    //Creating everthing to display them on screen
    void startLevel()
	{
        GetComponent<pauseScript>().enabled = false;
        setLevelsAndTurns();
        startObject.SetActive(true);
        setPanelsActive(false);
        setHeartActive(false);
        scoreText.SetActive(false);
        turnsText.SetActive(false);


        //Creating the top panel 
        createPanels();

        StartCoroutine(displayLevel());
    }
    //Displaying the level after everything set
    IEnumerator displayLevel()
    {
        yield return new WaitForSeconds(2);
        flipTopBottomCells();

        //Setting the score to zero 
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
        //Setting the turns
        turnsText.GetComponent<TextMeshProUGUI>().text = playerTurns.ToString();

        soundPlayingCell.Play("playSound");

        startObject.SetActive(false);
        setPanelsActive(true);
        setHeartActive(true);
        scoreText.SetActive(true);
        turnsText.SetActive(true);
        GetComponent<pauseScript>().enabled = true;
    }

    void setLevelsAndTurns()
	{
        TextMeshProUGUI[] levelTexts = startObject.GetComponentsInChildren<TextMeshProUGUI>();
        levelTexts[0].text = "Level" + level.ToString();
        levelTexts[1].text = dicideTurns().ToString();
	}
    void setHeartActive(bool active)
	{
        if(heartInd < 3)
		{
            heartImages[2].SetActive(active);
            if (heartInd < 2)
			{
                heartImages[1].SetActive(active);
                if(heartInd < 1)
                    heartImages[0].SetActive(active);
            }
        }
            
        
        
        
        
        if(active)
		{

		}
    }

    
    // Update is called once per frame
    void Update()
    {
		
    }

    void createPanels()
	{
        
        topCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cellPixel * gridSize, cellPixel * gridSize);
        bottomCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(cellPixel * gridSize, cellPixel * gridSize);
        for (int row = 0; row <  gridSize; row++)
		{
            for(int col =0; col < gridSize; col++)
			{
                GameObject topCell = Instantiate(cellObj, topCanvas.transform);
                topCell.GetComponent<Button>().interactable = false;
                topCell.GetComponent<Grid>().enabled = false;
                GameObject bottomCell = Instantiate(cellObj, bottomCanvas.transform);
                bottomCell.GetComponent<Grid>().x = row;
                bottomCell.GetComponent<Grid>().y = col;
            }
        }

    }
    void flipTopBottomCells()
    {
        flipTopCanvasTiles();

        flipInBottomPanel();
    }
    void flipTopCanvasTiles()
	{
        Animator[] allCells = topCanvas.GetComponentsInChildren<Animator>();
        int flipCellPercentage = Random.Range(minflipTitles, maxflipTitles);
        int cellsToFlip = (flipCellPercentage * gridSize * gridSize) / 100;

        int cellNum;
        for (int i = 0; i < cellsToFlip; i++)
        {

            do
            {
                cellNum = Random.Range(0, gridSize * gridSize - 1);
            } while (allCells[cellNum].GetComponent<Image>().sprite == mashroomSprite);

            allCells[cellNum].SetInteger("Anim", 2);
            allCells[cellNum].SetTrigger("Trig");
            allCells[cellNum].GetComponent<Image>().sprite = mashroomSprite;
        }
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
        
        Image[] allTopCells = topCanvas.GetComponentsInChildren<Image>();
        Image[] allBottomCells = bottomCanvas.GetComponentsInChildren<Image>();
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (allTopCells[row * gridSize + col].sprite == mashroomSprite)
                {
                    allBottomCells[row*gridSize+col].GetComponent<Image>().sprite = mashroomSprite;
                    allBottomCells[row*gridSize+col].GetComponent<Animator>().SetInteger("Anim", 1);
                    allBottomCells[row*gridSize+col].GetComponent<Animator>().SetTrigger("Trig");
                }
            }
        }
        playerTurns = dicideTurns();
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

            Debug.Log(cells[i]%gridSize + " " + ((cells[i]/gridSize) % gridSize));
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
        playerTurns--;
        //Setting the turns
        turnsText.GetComponent<TextMeshProUGUI>().text = playerTurns.ToString();
        //Pausing Input
        winLoseCanvas.SetActive(true);


        if (playerTurns == 0)
        {
            Image[] topPanelImages = topCanvas.GetComponentsInChildren<Image>();
            Image[] bottomPanelImages = bottomCanvas.GetComponentsInChildren<Image>();

            bool lose = false;
            for (int row = 0; row < gridSize; row++ )
			{
                for (int col =0; col < gridSize; col++)
				{
                    if(topPanelImages[row*gridSize+col].sprite != bottomPanelImages[row * gridSize + col].sprite)
					{
                        if(!lose) begingLoseSequence();
                        StartCoroutine(wrongTileAnim(bottomPanelImages[row * gridSize + col].GetComponent<Animator>()));
                        lose = true;
					}
				}
			}
            if (lose) return;
            StartCoroutine( startWinSequence());
        }
		else
		{
            StartCoroutine(disWinLoseCanva(false));
		}
    }
    IEnumerator wrongTileAnim(Animator cell)
	{
        yield return new WaitForSeconds(0.95f);
        cell.Play("wrongTile");
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
        GetComponent<pauseScript>().enabled = false;
        yield return new WaitForSeconds(0.95f);
        winLoseCanvas.SetActive(false);
        if (reset)
        {
            yield return new  WaitForSeconds(1.6f);
            mapAudioSource.clip = WrongMove;
            mapAudioSource.Play();
            sideStatues.Play("resetAnim");
            
            resetLevel();
        }
        GetComponent<pauseScript>().enabled = true;
    }

    public void setPanelsActive(bool active)
	{
        int offset = 900;
        if (active)
        {
            if (panelsActive)
                return;
            offset *= 1;
        }
        else
        {
            if (!panelsActive)
                return;
            offset *= -1;
            
        }
        panelsActive = active;
        topCanvas.GetComponent<RectTransform>().localPosition += Vector3.right * offset;
        bottomCanvas.GetComponent<RectTransform>().localPosition += Vector3.right * offset;
    }
    IEnumerator startWinSequence()
	{
        yield return new WaitForSeconds(0.95f);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = winSprite;
        GetComponent<pauseScript>().enabled = false;

        mapAudioSource.clip = clearLevelAudio;
        mapAudioSource.Play();
        sideStatues.Play("winAnim");

        yield return new WaitForSeconds(1);
        //To play again need to destroy the present chlidren
        destroyChildObjs();
        

        yield return new WaitForSeconds(1);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = emptySprite;
        startNextLevel();
    }
    void startNextLevel()
	{

        //Set them each time playAgain or win
        level++;
        difficulty++;
        score++;
        if(difficulty == 6)
		{
            gridSize++;
            difficulty = 0;
            if (gridSize == gridMaxSize + 1)
                gridSize = gridMinSize;
            bottomCanvaSprites = new Sprite[gridSize * gridSize];

        }

        startLevel();

        winLoseCanvas.SetActive(false);
    }
    IEnumerator startGameOverSequence()
	{
        yield return new WaitForSeconds(2.25f);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = loseSprite;
        GetComponent<pauseScript>().enabled = false;
        setPanelsActive(false);

        //To play again need to destroy the present chlidren
        destroyChildObjs();

        mapAudioSource.clip = gameOver;
        mapAudioSource.Play();

        yield return new WaitForSeconds(2);
        winLoseCanvas.GetComponentInChildren<Image>().sprite = emptySprite;
        pauseCanva.SetActive(true);

    }

    public void playAgainFunc()
	{
        //Deleting already present objects if played again from pause 
        if (GetComponent<pauseScript>().enabled)
            destroyChildObjs();
        //Set them each time playAgain or win
        level = 1;
        gridSize = gridMinSize;
        difficulty = 0;
        score = 0;
        heartInd = 0;

        pauseCanva.SetActive(false);

        startLevel();

        winLoseCanvas.SetActive(false);

    }

	void destroyChildObjs()
	{
        Image[] topImage = topCanvas.GetComponentsInChildren<Image>();
        Image[] bottomImage = bottomCanvas.GetComponentsInChildren<Image>();
        foreach (Image child in topImage)
        {
            Destroy(child.gameObject);
        }
        foreach (Image child in bottomImage)
        {
            Destroy(child.gameObject);
        }
    }

   

}
