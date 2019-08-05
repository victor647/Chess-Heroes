using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Chess
{
    public class ChessPiece : MonoBehaviour
    {
        private Chess _chess;
        public int AttackDamage => _chess.AttackDamage;
        public ChessType Type => _chess.ChessType;
        public int MaxHealth => _chess.MaxHealth;
        public PlayerColor Color { get; private set; }
        public int Health { get; private set; }
        public Vector2 Position { get; private set; }
        private readonly List<Vector2> _availableMovePositions = new List<Vector2>();
        private readonly List<ChessPiece> _availableAttackTargets = new List<ChessPiece>();
        private readonly Dictionary<Vector2, Vector2> _nextToTargetPosition = new Dictionary<Vector2, Vector2>();

        private static readonly Vector2[] StepsLinear =
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };
        
        private static readonly Vector2[] StepsDiagonal =
        {
            new Vector2(1, 1), 
            new Vector2(1, -1),
            new Vector2(-1, 1),
            new Vector2(-1, -1)
        };

        public void Init(ChessConfig config, PlayerColor color)
        {
            _chess = config.Chess;
            Color = color;
            Health = _chess.MaxHealth;
            Position = color == PlayerColor.Red? config.Position : ChessBoard.GetMirrorPosition(config.Position);
            ChessBoard.Instance.ChessPiecesOnBoard[(int)Position.x, (int)Position.y] = this;
            transform.position = ChessBoard.GetWorldPosition(Position);
        }

        #region Interaction
        private void OnMouseDown()
        {
            if (ChessBoard.Instance.CurrentPlayer != Color) return;
            if (ChessBoard.Instance.SelectedChess == this) return;
            GetMovePositionsAndAttackTargets();
            ChessBoard.Instance.CreateDestinations(_availableMovePositions, _availableAttackTargets);
            ChessBoard.Instance.SelectedChess = this;
        }

        private void OnMouseEnter()
        {
            if (ChessBoard.Instance.SelectedChess != this)
                UIManager.Instance.ShowChessStatus(this);
        }

        private void OnMouseExit()
        {
            if (ChessBoard.Instance.SelectedChess != this)
                UIManager.Instance.HideChessStatus(this);
        }

        #endregion

        private void GetMovePositionsAndAttackTargets()
        {
            _availableMovePositions.Clear();
            _availableAttackTargets.Clear();
            switch (Type)
            {
                case ChessType.Soldier:
                    var stepForward = Position + StepsLinear[Color == PlayerColor.Red ? 0 : 1];
                    CheckPosition(stepForward);
                    if (Position.y > 4 && Color == PlayerColor.Red || Position.y < 5 && Color == PlayerColor.Black)
                    {
                        CheckPosition(Position + StepsLinear[2]);
                        CheckPosition(Position + StepsLinear[3]);
                    }
                    break;
                case ChessType.Chariot:
                    _nextToTargetPosition.Clear();
                    foreach (var step in StepsLinear)
                    {
                        var tempPosition = Position + step;
                        while (CheckPosition(tempPosition) == PositionStatus.Empty)
                        {
                            tempPosition += step;
                        }

                        if (CheckPosition(tempPosition, false, false) == PositionStatus.Enemy)
                            _nextToTargetPosition[tempPosition] = tempPosition - step;
                    }
                    break;
                case ChessType.Cannon:
                    foreach (var step in StepsLinear)
                    {
                        var tempPosition = Position + step;
                        while (CheckPosition(tempPosition, true, false) == PositionStatus.Empty)
                        {
                            tempPosition += step;
                        }

                        if (CheckPosition(tempPosition, false, false) == PositionStatus.OutOfBound) continue;
                        
                        tempPosition += step;
                        while (CheckPosition(tempPosition, false) == PositionStatus.Empty)
                        {
                            tempPosition += step;
                        }
                    }
                    break;
                case ChessType.Horse:
                    foreach (var step1 in StepsLinear)
                    {
                        var tempPosition1 = Position + step1;
                        if (!IsPositionInBoundary(tempPosition1)) continue;
                        if (CheckPosition(tempPosition1, false, false) != PositionStatus.Empty) continue;
                        foreach (var step2 in StepsDiagonal)
                        {
                            if ((step1 + step2).sqrMagnitude < 5) continue;
                            CheckPosition(tempPosition1 + step2);
                        }
                    }
                    break;
                case ChessType.Elephant:
                    foreach (var step in StepsDiagonal)
                    {
                        var tempPosition1 = Position + step;
                        if (!IsPositionInBoundary(tempPosition1)) continue;
                        if (CheckPosition(tempPosition1, false, false) != PositionStatus.Empty) continue;
                        CheckPosition(tempPosition1 + step);
                    }
                    break;
                case ChessType.Guard:
                    foreach (var step in StepsDiagonal)
                    {
                        CheckPosition(Position + step);
                    }
                    break;
                case ChessType.General:
                    foreach (var step in StepsLinear)
                    {
                        CheckPosition(Position + step);
                    }
                    break;
            }
        }

        private PositionStatus CheckPosition(Vector2 position, bool canMove = true, bool canAttack = true)
        {
            if (!IsPositionInBoundary(position)) return PositionStatus.OutOfBound;
            var target = ChessBoard.Instance.GetChessPieceAtPosition(position);
            if (target == null)
            {
                if (canMove)
                    _availableMovePositions.Add(position);
                return PositionStatus.Empty;
            }

            if (target.Color != Color)
            {
                if (canAttack)
                    _availableAttackTargets.Add(target);
                return PositionStatus.Enemy;
            }
            
            return PositionStatus.Self;
        }

        private bool IsPositionInBoundary(Vector2 position)
        {
            if (Color == PlayerColor.Black)
                position = ChessBoard.GetMirrorPosition(position);
            switch (Type)
            {
                case ChessType.Elephant:
                    if (position.y > 4) return false;
                    break;
                case ChessType.Guard:
                case ChessType.General:
                    if (position.y > 2 || position.x < 3 || position.x > 5) return false;
                    break;
            }
            return position.x >= 0 && position.y >= 0 && position.x <= 8 && position.y <= 9;
        }

        public void MoveOrAttack(Vector2 position)
        {
            var target = ChessBoard.Instance.GetChessPieceAtPosition(position);
            if (target) //attack
            {
                if (target.Color == Color) return;
                target.TakeDamage(this);
                if (target.Health <= 0 && Type != ChessType.Cannon)
                    MoveToPosition(position);
                else if (Type == ChessType.Chariot)
                    MoveToPosition(_nextToTargetPosition[position]);
            }
            else //just move
                MoveToPosition(position);
            ChessBoard.Instance.SwitchPlayer();
        }

        private void MoveToPosition(Vector2 position)
        {
            if (Position == position) return;
            transform.position = ChessBoard.GetWorldPosition(position);
            ChessBoard.Instance.ChessPiecesOnBoard[(int)Position.x, (int)Position.y] = null;
            ChessBoard.Instance.ChessPiecesOnBoard[(int)position.x, (int)position.y] = this;
            Position = position;
        }

        private void TakeDamage(ChessPiece source)
        {
            Health -= source._chess.AttackDamage;
            if (Health <= 0) 
                Die();
        }

        private void Die()
        {
            ChessBoard.Instance.KillChess(this);
            Destroy(gameObject);
        }
    }
}