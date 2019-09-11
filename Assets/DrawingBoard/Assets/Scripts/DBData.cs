using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DBScene
{
    public int id;
    public string mainPngAddr;
    public string[] pngAddress;
    public line[] lines;
    public string description;
    public SceneAnswerPair[] next;
}

[System.Serializable]
public class SceneAnswerPair
{
    public int nextSceneID;
    public string answer;
}

[System.Serializable]
public class slideShow
{
    public string name;
    public DBScene[] sceneSet;
    public DBScene Firstslide;
}

[System.Serializable]
public class line
{
    public point[] points;
    public int penWidth;
    public bool isAnim;
}

[System.Serializable]
public class point
{
    public float x;
    public float y;
    public float timeStamp;
}