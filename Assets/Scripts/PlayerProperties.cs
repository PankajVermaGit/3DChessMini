using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour {


    public PlayerName playerName;
    public PlayerType playerType;
    public Vector2 playerPosition;
    public bool isEligibleForAttack;
    public int movablePositionCount; 

    // Use this for initialization
    void OnEnable()
    {
        ResetOnPosition();
        SpawnPlayer();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name=="Player1" || other.gameObject.name == "Player2")
        {
            GameManager.instance.OnGameOver();
        }
    }



    public void ResetOnPosition()
    {
        if (playerName == PlayerName.Player1)
        {
            transform.position = new Vector3(GameManager.instance.cubeList[0].transform.position.x, transform.position.y, GameManager.instance.cubeList[0].transform.position.z);
            playerPosition = GameManager.instance.cubeList[0].transform.GetComponent<CubeProperties>().position;
        }
        else
        {
            transform.position = new Vector3(GameManager.instance.cubeList[GameManager.instance.cubeList.Count - 1].transform.position.x, transform.position.y, GameManager.instance.cubeList[GameManager.instance.cubeList.Count - 1].transform.position.z);
            playerPosition = GameManager.instance.cubeList[GameManager.instance.cubeList.Count -1].transform.GetComponent<CubeProperties>().position;
        }
    }


    public void SpawnPlayer()
    {
        GameObject go=new GameObject();

        if (playerName == PlayerName.Player1)
        {
            if (playerType == PlayerType.Rocks)
            {
                go=Instantiate(GameManager.instance.RockWhite, transform) as GameObject;
            }

            if (playerType == PlayerType.Bishop)
            {
                go = Instantiate(GameManager.instance.BishopWhite, transform) as GameObject;

            }


            if (playerType == PlayerType.Knight)
            {
                go = Instantiate(GameManager.instance.KnightWhite, transform) as GameObject;
            }

        }

        if (playerName == PlayerName.Player2)
        {
            if (playerType == PlayerType.Rocks)
            {
                go = Instantiate(GameManager.instance.RockBlack, transform) as GameObject;


            }


            if (playerType == PlayerType.Bishop)
            {
                go = Instantiate(GameManager.instance.BishopBlack, transform) as GameObject;


            }


            if (playerType == PlayerType.Knight)
            {
                go=Instantiate(GameManager.instance.KnightBlack, transform) as GameObject;


            }

        }

        go.transform.localPosition = new Vector3(0, -1f, 0);
        go.transform.localScale = new Vector3(1, 1, 1);
    }



    public void SetPlayerName(Dropdown dropDown)
    {
        switch (dropDown.value)
        {
            case 0:
                playerType = PlayerType.Rocks;
                break;
            case 1:
                playerType = PlayerType.Knight;
                break;
            case 2:
                playerType = PlayerType.Bishop;
                break;
        }        
    }


}
