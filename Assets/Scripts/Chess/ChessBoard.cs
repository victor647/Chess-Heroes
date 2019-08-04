using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
    public class ChessBoard : MonoBehaviour
    {
        public static ChessBoard Instance;
        public GameObject ChessPrefab;
        private PlayerColor _currentPlayer = PlayerColor.Red;

        public List<ChessPiece> ChessPiecesOnBoard = new List<ChessPiece>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InstantiateChessPieces();
        }

        private void InstantiateChessPieces()
        {
            var config = Resources.Load<ChessPositionConfig>("Configs/ChessPositions");
            if (!config) return;
            foreach (var positionConfig in config.ChessConfigs)
            {
                CreateChessPiece(positionConfig, PlayerColor.Red);
                CreateChessPiece(positionConfig, PlayerColor.Black);
            }
        }

        private void CreateChessPiece(ChessConfig config, PlayerColor color)
        {
            var chess = Instantiate(ChessPrefab, transform);
            var chessPiece = chess.AddComponent<ChessPiece>();
            
            chess.name = config.Chess.ChessType + "_" + color;
            chess.GetComponent<SpriteRenderer>().sprite = color == PlayerColor.Red ? config.Chess.SpriteRed : config.Chess.SpriteBlack;
            chessPiece.Init(config, color);
        }
        
        public void KillChessPiece(ChessPiece chess)
        {
            ChessPiecesOnBoard.Remove(chess);
        }

        private void CheckWinStatus()
        {
            
        }

        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PlayerColor.Red ? PlayerColor.Black : PlayerColor.Red;
        }

        public static Vector2 GetMirrorPosition(Vector2 originalPosition)
        {
            return new Vector2(8 - originalPosition.x,  9 - originalPosition.y);
        }
        
        public static Vector2 GetWorldPosition(Vector2 boardPosition)
        {
            return new Vector2(boardPosition.x - 4, boardPosition.y - 4.5f) * 0.9f;
        }

        public ChessPiece GetChessPieceAtPosition(Vector2 position)
        {
            return ChessPiecesOnBoard.First(cp => cp.Position == position);
        }
    }
}
