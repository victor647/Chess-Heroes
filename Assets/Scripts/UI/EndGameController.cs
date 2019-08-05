using Chess;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndGameController : MonoBehaviour
    {
        public Text WinnerText;
        
        public void SetWinnerText(PlayerColor winner)
        {
            if (winner == PlayerColor.Black)
            {
                WinnerText.text = "黑方获胜！";
                WinnerText.color = Color.black;
            }
            else
            {
                WinnerText.text = "红方获胜！";
                WinnerText.color = Color.red;
            }
        }
        
        public void ResetGame()
        {
            Destroy(ChessBoard.Instance.gameObject);
            var chessBoard = Resources.Load<GameObject>("Prefabs/ChessBoard");
            if (chessBoard)
                Instantiate(chessBoard);
            UIManager.Instance.SwitchPlayer(PlayerColor.Red);
            gameObject.SetActive(false);
        }
    }
}
