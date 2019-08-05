using Chess;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public Text CurrentPlayerRed;
        public Text CurrentPlayerBlack;
        public Text ChessStatusRed;
        public Text ChessStatusBlack;
        public GameObject WinBanner;

        private void Awake()
        {
            Instance = this;
        }

        public void SwitchPlayer(PlayerColor nextPlayer)
        {
            CurrentPlayerRed.enabled = nextPlayer == PlayerColor.Red;
            CurrentPlayerBlack.enabled = nextPlayer != PlayerColor.Red;
        }

        public void EndGame(PlayerColor winner)
        {
            WinBanner.SetActive(true);
            WinBanner.GetComponent<EndGameController>().SetWinnerText(winner);
        }

        public void ShowChessStatus(ChessPiece chess)
        {
            var text = $"生命：{chess.Health}/{chess.MaxHealth}\n攻击：{chess.AttackDamage}";
            if (chess.Color == PlayerColor.Black)
                ChessStatusBlack.text = text;
            else
                ChessStatusRed.text = text;
        }

        public void HideChessStatus(ChessPiece chess)
        {
            const string text = "生命：\n攻击：";
            if (chess.Color == PlayerColor.Black)
                ChessStatusBlack.text = text;
            else
                ChessStatusRed.text = text;
        }
    }
}
