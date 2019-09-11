
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class FlappyCandleController : MonoBehaviour
{
    //Game Scene Variables
    public GameObject startUI, gameUI, winUI, gameOverUI;
    public GameObject player;
    public GameObject background;
    public GameObject Candle;
    public Transform stick, wick;
    //PLayer Varaibles
    public int score;
    //GameUI Variables
    public Text scoreText;
    public Text ScoreNumber;

    private Rigidbody2D candleRb;
    //private static float sideVelocity, upThrust, forceOfGravity;
    private Rigidbody2D playerRb;
    //Data Controller
    //private string gameDataFileName = "data.json";
    //candle pool
    private GameObject[] candles;
    private Vector2 objectPoolPosition;

    void Start()
    {
        setVariables();

        candles = new GameObject[DataController.gameData.candlePoolSize];
        stick.localScale.Set(stick.localScale.x, DataController.gameData.stickHeight, stick.localScale.z);
        wick.localScale.Set(wick.localScale.x, DataController.gameData.wickHeight, wick.localScale.z);
        for (int i = 0; i < DataController.gameData.candlePoolSize; i++)
        {
            candles[i] = (GameObject)Instantiate(Candle, objectPoolPosition, Quaternion.identity);
            candleRb = candles[i].GetComponent<Rigidbody2D>();
            candleRb.velocity = new Vector2(DataController.gameData.CandleSpeed, 0);
        }

        DataController.gameData.backgroundLength = background.transform.localScale.x;
        Time.timeScale = 0;
        startUI.SetActive(true);
        gameUI.SetActive(false);
        winUI.SetActive(false);
        gameOverUI.SetActive(false);
        playerRb = player.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        playerMove();
        poolCandle();
    }

    public void poolCandle()
    {
        DataController.gameData.timeSinceLastSpawn += Time.deltaTime;

        if (DataController.gameData.timeSinceLastSpawn > DataController.gameData.spawnRate)
        {
            DataController.gameData.timeSinceLastSpawn = 0;
            float spawnYPos = Random.Range(DataController.gameData.candleMin, DataController.gameData.candleMax);
            candles[DataController.gameData.currentCandle].transform.position = new Vector2(DataController.gameData.spawnXPos, spawnYPos);
            GameObject wickObj = candles[DataController.gameData.currentCandle].transform.Find("Target").gameObject;
            wickObj.SetActive(true);
            wickObj.transform.parent.GetChild(2).gameObject.SetActive(false);
            DataController.gameData.currentCandle++;

            if (DataController.gameData.currentCandle >= DataController.gameData.candlePoolSize)
            {
                DataController.gameData.currentCandle = 0;
            }
        }
    }

    public void playerMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))//Input.touchCount > 0)//
        {
            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(new Vector2(0, DataController.gameData.upThrust));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Goal")
        {
            score++;
            ScoreNumber.text = score.ToString();
            GameObject fire = other.transform.parent.gameObject.transform.GetChild(2).gameObject;
            fire.SetActive(true);
            other.gameObject.SetActive(false);
        }

        if (score == DataController.gameData.winScore)
        {
            gameUI.SetActive(false);
            Time.timeScale = 0;
            PlayerPrefs.SetInt("levelPassed", SceneManager.GetActiveScene().buildIndex-1);
            winUI.SetActive(true);
            SceneManager.LoadScene(0);
        }

        if (other.gameObject.tag == "Obstacle")
        {
            gameUI.SetActive(false);
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
        }
    }


    

    public void startGame()
    {
        startUI.SetActive(false);
        Time.timeScale = 1;
        gameUI.SetActive(true);
    }

    public void winRestart()
    {
        
        //winUI.SetActive(false);
        Reset();
        SceneManager.LoadScene(0);
        //startUI.SetActive(true);
    }

    public void gameOverRestart()
    {
        gameOverUI.SetActive(false);
        Reset();
        startUI.SetActive(true);
    }
    private void setVariables()
    {
        startUI.transform.GetChild(0).GetComponent<Text>().text = DataController.gameData.gameTitle;
        startUI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = DataController.gameData.startButtonText;
        winUI.transform.GetChild(0).GetComponent<Text>().text = DataController.gameData. winSceneHeading;
        winUI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = DataController.gameData. winButtonText;
        gameOverUI.transform.GetChild(0).GetComponent<Text>().text = DataController.gameData.gameOverSceneHeading;
        gameOverUI.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = DataController.gameData.gameOverbuttonText;

        //player settings
        score = 0;
        ScoreNumber.text = score.ToString();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.gravityScale = DataController.gameData.forceOfGravity;
        player.transform.localPosition = new Vector3(DataController.gameData.PlayerXPos, DataController.gameData.PlayerYPos, 0);

        //candle settings
        objectPoolPosition = new Vector2(DataController.gameData.objectPoolPosition1, DataController.gameData.objectPoolPosition2);

    }

    private void RepositionBackground()
    {
        Vector2 groundOffset = new Vector2(gameObject.transform.localScale.x, 0);
        background.transform.position = (Vector2)transform.position + groundOffset;
    }

    void OnDisable()
    {
        for (int i = 0; i < DataController.gameData.candlePoolSize; i++)
        {
            Debug.Log("Enable running");
            Destroy(candles[i]);
        }
    }

    void Reset()
    {
        for (int i = 0; i < DataController.gameData.candlePoolSize; i++)
        {
            Destroy(candles[i]);
        }

        setVariables();

        candles = new GameObject[DataController.gameData.candlePoolSize];
        stick.localScale.Set(stick.localScale.x, DataController.gameData.stickHeight, stick.localScale.z);
        wick.localScale.Set(wick.localScale.x, DataController.gameData.wickHeight, wick.localScale.z);
        for (int i = 0; i < DataController.gameData.candlePoolSize; i++)
        {
            candles[i] = (GameObject)Instantiate(Candle, objectPoolPosition, Quaternion.identity);
            candleRb = candles[i].GetComponent<Rigidbody2D>();
            candleRb.velocity = new Vector2(DataController.gameData.CandleSpeed, 0);
        }
    }

    public GameObject spriteBar;
    public void editMode()
    {
        if (spriteBar.activeSelf)
        {
            Time.timeScale = 1;
            spriteBar.SetActive(false);
        } else
        {
            Time.timeScale = 0;
            spriteBar.SetActive(true);
        }
    }

}