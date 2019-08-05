using System;
using UI;
using UnityEngine;

namespace Chess
{
    public class ChessDestination : MonoBehaviour
    {
        public Vector2 Position;
        public ChessPiece Chess;

        private void OnMouseEnter()
        {
            if (Chess)
                UIManager.Instance.ShowChessStatus(Chess);
        }

        private void OnMouseUp()
        {
            ChessBoard.Instance.SelectedChess.MoveOrAttack(Position);
        }
    }
}
