using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*
     * Few methods from FlappyPlayer like restart Game should be here 
     * but i don't want to change all orginal script
     */

    public static GameManager instance;
    public Transform playerTransform;
    public Text scoreTxt;
    int score = 0;
    public int Score { get { return score; } set { score = value; scoreTxt.text = "Score: " + value.ToString(); } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);
    }
}
