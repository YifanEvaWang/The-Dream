using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MainSceneData
{
    public string[] URL;
}


[System.Serializable]
public class FlappyCandleData
{
    //varibales for json file
    //public StartSceneData startSceneData;
    public string gameTitle, startButtonText;
    //public GameSceneData gameSceneData;
    public int wicksNeededToWin;
    public float upThrust, sideVelocity, forceOfGravity, wickHeight, stickHeight;
    //public WinSceneData winSceneData;
    public string winSceneHeading, winButtonText;
    //public GameOverSceneData gameOverSceneData;
    public string gameOverSceneHeading, gameOverbuttonText;
    //public ColumnPoolData columnPoolData;
    public int candlePoolSize, currentCandle, spawnRate;
    public float candleMin, candleMax, objectPoolPosition1, objectPoolPosition2, timeSinceLastSpawn, spawnXPos, CandleSpeed;
    public string firstScene;
    public int PlayerXPos, PlayerYPos;
    public int winScore;
    public float backgroundLength;
}


