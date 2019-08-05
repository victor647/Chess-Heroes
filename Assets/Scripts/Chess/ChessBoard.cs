using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Chess
{
    public enum PositionStatus
    {
        Empty,
        Self,
        Enemy,
        OutOfBound
    }
    
    public class ChessBoard : MonoBehaviour
    {
        public static ChessBoard Instance;
        public GameObject ChessPrefab;
        public GameObject MovablePositionSprite;
        public GameObject AttackTargetSprite;
        public PlayerColor CurrentPlayer { get; private set; }
        public ChessPiece SelectedChess;

        public ChessPiece[,] ChessPiecesOnBoard = new ChessPiece[9, 10];
        private readonly List<GameObject> _movablePositions = new List<GameObject>();
        private readonly List<GameObject> _attackTargets = new List<GameObject>();
        private byte _totalChessCountRed;
        private byte _totalChessCountBlack;

        private void Awake()
        {
            Instance = this;
            CurrentPlayer = PlayerColor.Red;
        }

        private void Start()
        {
            InstantiateChessPieces();
        }

        private void Update()
        {
            if (!SelectedChess) return;
            if (Input.GetMouseButtonDown(1))
            {
                ClearDestinations();
                SelectedChess = null;
            }
        }
        
        private void InstantiateChessPieces()
        {
            var config = Resources.Load<ChessPositionConfig>("Configs/ChessPositions");
            if (!config) return;
            foreach (var positionConfig in config.ChessConfigs)
            {
                CreateChessPiece(positionConfig, PlayerColor.Red);
                CreateChessPiece(positionConfig, PlayerColor.Black);
                _totalChessCountBlack++;
                _totalChessCountRed++;
            }
        }

        private void CreateChessPiece(ChessConfig config, PlayerColor color)
        {
            var chess = Instantiate(ChessPrefab, transform);
            var chessPiece = chess.GetComponent<ChessPiece>();
            chess.name = config.Chess.ChessType + "_" + color;
            chess.GetComponent<SpriteRenderer>().sprite = color == PlayerColor.Red ? config.Chess.SpriteRed : config.Chess.SpriteBlack;
            chessPiece.Init(config, color);
        }

        private void ClearDestinations()
        {
            foreach (var position in _movablePositions)
            {
                Destroy(position);
            }
            _movablePositions.Clear();
            foreach (var target in _attackTargets)
            {
                Destroy(target);
            }
            _attackTargets.Clear();
        }
        
        public void CreateDestinations(List<Vector2> positions, List<ChessPiece> targets)
        {
            ClearDestinations();   
            foreach (var newPosition in positions)
            {
                var positionSprite = Instantiate(MovablePositionSprite, GetWorldPosition(newPosition), Quaternion.identity);
                var destination = positionSprite.GetComponent<ChessDestination>();
                destination.Position = newPosition;
                _movablePositions.Add(positionSprite);
            }
            foreach (var target in targets)
            {
                var attackSprite = Instantiate(AttackTargetSprite, GetWorldPosition(target.Position), Quaternion.identity);
                var destination = attackSprite.GetComponent<ChessDestination>();
                destination.Position = target.Position;
                destination.Chess = target;
                _attackTargets.Add(attackSprite);
            }
        }

        public void KillChess(ChessPiece chess)
        {
            if (chess.Type == ChessType.General)
            {
                UIManager.Instance.EndGame(chess.Color == PlayerColor.Black ? PlayerColor.Red : PlayerColor.Black);
                return;
            }
            if (_totalChessCountBlack == 1)
                UIManager.Instance.EndGame(PlayerColor.Red);
            if (_totalChessCountRed == 1)
                UIManager.Instance.EndGame(PlayerColor.Black);
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == PlayerColor.Red ? PlayerColor.Black : PlayerColor.Red;
            ClearDestinations();
            UIManager.Instance.SwitchPlayer(CurrentPlayer);
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
            return ChessPiecesOnBoard[(int)position.x, (int)position.y];
        }
    }
}
