using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public Dictionary<Node, string> nodeAnswers;
    public string text;
    public DBScene JsonInfo;

    private void Awake()
    {
        nodeAnswers = new Dictionary<Node, string>();
    }

    public void addAnswers(string answer, Node tailNode)
    {
        if (nodeAnswers.ContainsKey(tailNode))
        {
            nodeAnswers[tailNode] = answer;
        }
        else
        {
            Debug.Log("can't add text, error");
        }
    }

    public void fillJson()
    {
        List<SceneAnswerPair> pairs = new List<SceneAnswerPair>();
        JsonInfo.description = text;
        JsonInfo.id = transform.GetSiblingIndex();
        foreach(Node node in nodeAnswers.Keys)
        {
            SceneAnswerPair onePair = new SceneAnswerPair();
            onePair.nextSceneID = node.transform.GetSiblingIndex();
            onePair.answer = nodeAnswers[node];
            pairs.Add(onePair);
        }
        JsonInfo.next = pairs.ToArray();
    }

    public void setText(string text)
    {
        this.text = text;
    }

    public string getText()
    {
        return text;
    }

    public Dictionary<Node,string> getNodeAnswer()
    {
        if (nodeAnswers.Keys.Count != 0)
        {
            return nodeAnswers;
        }
        else
        {
            return null;
        }
    }

    public void addChild(Node anotherNode)
    {
        if (!nodeAnswers.ContainsKey(anotherNode))
        {
            nodeAnswers.Add(anotherNode, null);
        }
    }

    
}
