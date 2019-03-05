using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    None,
    Rocks,
    Knight,
    Bishop
};

public enum PlayerName
{
    None,
    Player1,
    Player2
};


public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject cube;
    public GameObject player1;
    public GameObject player2;
    public GameObject selectedPlayer;
    public GameObject invalidMove;
    public GameObject gameOver;

    public Material blackColorMat;
    public Material whiteColorMat;
    
    public Transform chessBoard;

    public Text turnText;
    public Text timeText;
    
    public int maxTime = 300;

    float time = 0;

    public Slider progressBar;
    public GameObject inputPanel;
    public InputField rows;
    public InputField column;


    [HideInInspector] public List<GameObject> cubeList;


    [Header("Chess Piece")]
    [Space(10)]
    [HideInInspector] public GameObject KnightBlack;
    [HideInInspector] public GameObject RockBlack;
    [HideInInspector] public GameObject BishopBlack;
    [HideInInspector] public GameObject KnightWhite;
    [HideInInspector] public GameObject RockWhite;
    [HideInInspector] public GameObject BishopWhite;


    [HideInInspector]
    public bool gameStarted = false;
    WWW www;


    

    private void Awake()
    {
        instance = this;
    }

    
    void Start () {

        StartCoroutine(DownloadAssetFromServer());
    }
	
	
	void Update () {

        // Updating progress bar
        if(www!=null && www.progress!=0 && progressBar.value !=1)
        {
            progressBar.value = www.progress;
            inputPanel.SetActive(true);
        }

        if (!gameStarted)
            return;

        time += Time.deltaTime;

        TimeSpan t = TimeSpan.FromSeconds(maxTime - time);
        string ans = string.Format("{0:D2} : {1:D2}", t.Minutes, t.Seconds);
        timeText.text = ans;

        if(t.Minutes==0 && t.Seconds==0)
        {
            OnGameOver();
        }

	}


    // Building Chess board 
    private void BuildChess(int x,int y)
    {
        for(int i=0;i<x;i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject go=GameManager.Instantiate(cube,new Vector3(i, 0, -j),Quaternion.identity,chessBoard);

                if((i+j)%2==0)
                    go.transform.GetChild(0).GetComponent<MeshRenderer>().material = whiteColorMat;
                else
                    go.transform.GetChild(0).GetComponent<MeshRenderer>().material = blackColorMat;

                go.GetComponent<CubeProperties>().position = new Vector2(i, j);
                cubeList.Add(go);
                go.hideFlags = HideFlags.HideInHierarchy;
            }

        }


    }


    //Disable all Highlight 
    public void DisableAllValidPoition()
    {
        foreach (GameObject go in cubeList)
            go.GetComponent<CubeProperties>().validationColor.SetActive(false);
    }


    // Highlight player movable position
    public void ShowValidPosition(GameObject player)
    {

        DisableAllValidPoition();

        PlayerProperties playerProperties = player.GetComponent<PlayerProperties>();
        Vector2 positon = playerProperties.playerPosition;
        Vector2 lastCubePosition = cubeList[cubeList.Count - 1].GetComponent<CubeProperties>().position;
        Vector2 player1Position = player1.GetComponent<PlayerProperties>().playerPosition;
        Vector2 player2Position = player2.GetComponent<PlayerProperties>().playerPosition;


        if (playerProperties.playerName==PlayerName.Player1)
        {
            playerProperties.movablePositionCount = 0;

            if (playerProperties.playerType==PlayerType.Rocks)
            {
                 for (float i =0; i <= lastCubePosition.x; i++)
                {
                    for (float j = positon.y; j <= lastCubePosition.y; j++)
                    { 
                        if ((i!= positon.x || j!= positon.y) && (i == positon.x || j == positon.y))
                        {
                            if (!playerProperties.isEligibleForAttack && lastCubePosition == new Vector2(i, j))
                                continue;


                            if (player1Position.y == player2Position.y)
                            {
                                if (player2Position.x > player1Position.x)
                                {
                                    if (i > player2Position.x)
                                        continue;
                                }
                                else
                                {
                                    if (i < player2Position.x)
                                        continue;
                                }
                            }

                            
                            if (player1Position.x == player2Position.x)
                            {
                                if (player2Position.y > player1Position.y)
                                {
                                    if (j > player2Position.y)
                                        continue;
                                }
                                else
                                {
                                    if (j < player2Position.y)
                                        continue;
                                }
                            }

                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++;
                        }
                    }

                }
            }


            if (playerProperties.playerType == PlayerType.Bishop)
            {
                for (float i = 0; i <= lastCubePosition.x; i++)
                {
                    for (float j = positon.y; j <= lastCubePosition.y; j++)
                    {
                        if ((i != positon.x || j != positon.y) && (Mathf.Abs(i - positon.x) == Mathf.Abs(j - positon.y)))
                        {
                            //print(i + ":" + j+" : "+ playerProperties.isEligibleForAttack + lastCubePosition);

                            if (!playerProperties.isEligibleForAttack && lastCubePosition == new Vector2(i,j))
                                continue;

                            bool isOpponentValid = Mathf.Abs(player1Position.x - player2Position.x) == Mathf.Abs(player1Position.y - player2Position.y);


                            if (isOpponentValid && player2Position.y>player1Position.y)
                            {
                                if(player2Position.x < player1Position.x)
                                {
                                    if (i < player2Position.x && j> player2Position.y)
                                        continue;
                                }
                                else
                                {
                                    if (i > player2Position.x && j > player2Position.y)
                                        continue;
                                }
                            }


                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++;
                        }
                    }

                }
            }


            if (playerProperties.playerType == PlayerType.Knight)
            {
                for (float i = 0; i <= lastCubePosition.x; i++)
                {
                    for (float j = positon.y; j <= lastCubePosition.y; j++)
                    {
                        if ((i != positon.x || j != positon.y) && (((Mathf.Abs(i- positon.x)==2) && (Mathf.Abs(j - positon.y) == 1)) || ((Mathf.Abs(i - positon.x) == 1) && (Mathf.Abs(j - positon.y) == 2))))
                        {
                            if (!playerProperties.isEligibleForAttack && lastCubePosition == new Vector2(i, j))
                                continue;

                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++;
                        }
                    }

                }
            }


            if (playerProperties.movablePositionCount == 0)
                OnGameOver();
        }


        if (playerProperties.playerName == PlayerName.Player2)
        {
            playerProperties.movablePositionCount = 0;

            if (playerProperties.playerType == PlayerType.Rocks)
            {
                for (float i = lastCubePosition.x; i >= 0; i--)
                {
                    for (float j = positon.y; j >= 0; j--)
                    {
                        if ((i != positon.x || j != positon.y) && (i == positon.x || j == positon.y))
                        {
                            if (!playerProperties.isEligibleForAttack && player1Position == new Vector2(i, j))
                                continue;


                            if (player1Position.y == player2Position.y)
                            {
                                if (player1Position.x > player2Position.x)
                                {
                                    if (i > player1Position.x)
                                        continue;
                                }
                                else
                                {
                                    if (i < player1Position.x)
                                        continue;
                                }
                            }


                            if (player1Position.x == player2Position.x)
                            {
                                if (player1Position.y > player2Position.y)
                                {
                                    if (j > player1Position.y)
                                        continue;
                                }
                                else
                                {
                                    if (j < player1Position.y)
                                        continue;
                                }
                            }


                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++;
                        }
                    }

                }


            }


            if (playerProperties.playerType == PlayerType.Bishop)
            {
                for (float i = lastCubePosition.x; i >= 0; i--)
                {
                    for (float j = positon.y; j >= 0; j--)
                    {
                        if ((i != positon.x || j != positon.y) && (Mathf.Abs(i-positon.x) == Mathf.Abs(j-positon.y)))
                        {
                            if (!playerProperties.isEligibleForAttack && player1Position == new Vector2(i, j))
                                continue;


                            bool isOpponentValid = Mathf.Abs(player1Position.x - player2Position.x) == Mathf.Abs(player1Position.y - player2Position.y);


                            if (isOpponentValid && player2Position.y > player1Position.y)
                            {
                                if (player1Position.x < player2Position.x)
                                {
                                    if (i < player1Position.x && j < player1Position.y)
                                        continue;
                                }
                                else
                                {
                                    if (i > player1Position.x && j < player1Position.y)
                                        continue;
                                }
                            }


                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++;
                        }
                    }

                }


            }


            if (playerProperties.playerType == PlayerType.Knight)
            {
                for (float i = lastCubePosition.x; i >= 0; i--)
                {
                    for (float j = positon.y; j >= 0; j--)
                    {
                        if ((i != positon.x || j != positon.y) && (((Mathf.Abs(i - positon.x) == 2) && (Mathf.Abs(j - positon.y) == 1)) || ((Mathf.Abs(i - positon.x) == 1) && (Mathf.Abs(j - positon.y) == 2))))
                        {
                            if (!playerProperties.isEligibleForAttack && player1Position == new Vector2(i, j))
                                continue;

                            GameObject g = cubeList.Find(x => x.GetComponent<CubeProperties>().position == new Vector2(i, j));
                            g.GetComponent<CubeProperties>().validationColor.SetActive(true);
                            playerProperties.movablePositionCount++; 
                        }
                    }

                }


            }


            if (playerProperties.movablePositionCount == 0)
                OnGameOver();

        }

    }


    public void OnGameOver()
    {
        gameStarted = false;
        chessBoard.gameObject.SetActive(false);
        gameOver.SetActive(true);
        timeText.gameObject.SetActive(false);
        turnText.gameObject.SetActive(false);
    }


    public void StartGame()
    {
        ShowValidPosition(player1);
        gameStarted = true;
    }


    public void RestartGame()
    {
        time = 0;
        chessBoard.gameObject.SetActive(true);
        gameOver.SetActive(false);
        timeText.gameObject.SetActive(true);
        turnText.gameObject.SetActive(true);
        player1.GetComponent<PlayerProperties>().ResetOnPosition();
        player2.GetComponent<PlayerProperties>().ResetOnPosition();
        ShowValidPosition(player1);
        selectedPlayer = player1;
        turnText.text = "Player 1";
        gameStarted = true;
    }


    // Show Text on click unvalid move
    public IEnumerator ShowInvalidMove()
    {
        invalidMove.SetActive(true);
        yield return new WaitForSeconds(1f);
        invalidMove.SetActive(false);
    }


    public IEnumerator DownloadAssetFromServer()
    {
        string serverUrl = "https://drive.google.com/open?id=1QpQoy5-aBZKprJDsuDASJSFWxfcS4RZw";
        string filePath= Path.Combine(Application.streamingAssetsPath, "chesspiece.unity3d");

        if (File.Exists(filePath))
        {
            yield return new WaitForSeconds(0f);
            LoadAssetBundle();
            progressBar.gameObject.SetActive(false);
            inputPanel.SetActive(true);
        }
        else
        {
            progressBar.gameObject.SetActive(true);
            inputPanel.SetActive(false);

            www = new WWW(serverUrl);

            yield return www;

            yield return new WaitUntil(() => www.isDone);

            File.WriteAllBytes(filePath, www.bytes);

            yield return new WaitForSeconds(1f);

            LoadAssetBundle();
        }

    }


    // Load Assets from Asset Bundle
    public void LoadAssetBundle()
    {
        if (!File.Exists(Path.Combine(Application.streamingAssetsPath, "chesspiece")))
            return;

        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "chesspiece"));

        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        GameObject prefab = myLoadedAssetBundle.LoadAsset<GameObject>("ChessPiece");
        //Instantiate(prefab);

        KnightBlack= prefab.transform.GetChild(1).gameObject; 
        RockBlack= prefab.transform.GetChild(2).gameObject; 
        BishopBlack=prefab.transform.GetChild(0).gameObject;
        KnightWhite= prefab.transform.GetChild(4).gameObject; 
        RockWhite= prefab.transform.GetChild(5).gameObject; 
        BishopWhite= prefab.transform.GetChild(3).gameObject; 

        myLoadedAssetBundle.Unload(false);
    }


    // Create chess board
    public void SetUpChessBoard()
    {
        if (Int32.Parse(column.text) < 2 || Int32.Parse(column.text) > 8)
            return;

        if (Int32.Parse(rows.text) < 2 || Int32.Parse(rows.text) > 8)
            return;

        BuildChess(Int32.Parse(column.text), Int32.Parse(rows.text));

        ShowValidPosition(player1);
        gameStarted = true;
    }

}
