using UnityEngine;
using DG.Tweening;

public class CubeProperties : MonoBehaviour {

    [HideInInspector] public Vector2 position;
    public GameObject validationColor;


    private void OnMouseDown()
    {
        if (!validationColor.activeSelf)
        {
            StartCoroutine(GameManager.instance.ShowInvalidMove());
            return;
        }

        GameManager.instance.selectedPlayer.transform.DOMoveX(transform.position.x, 1f);
        GameManager.instance.selectedPlayer.transform.DOMoveZ(transform.position.z, 1f).OnComplete(ChangeTurn);
        GameManager.instance.selectedPlayer.GetComponent<PlayerProperties>().playerPosition=position;
    }


    private void ChangeTurn()
    {
        GameManager.instance.selectedPlayer.GetComponent<PlayerProperties>().isEligibleForAttack = true;

        if (GameManager.instance.selectedPlayer.GetComponent<PlayerProperties>().playerName == PlayerName.Player1)
        {
            GameManager.instance.turnText.text = "Player 2";
            GameManager.instance.ShowValidPosition(GameManager.instance.player2);
            GameManager.instance.selectedPlayer = GameManager.instance.player2;

        }
        else
        {
            GameManager.instance.turnText.text = "Player 1";
            GameManager.instance.ShowValidPosition(GameManager.instance.player1);
            GameManager.instance.selectedPlayer = GameManager.instance.player1;

        }
    }

}
