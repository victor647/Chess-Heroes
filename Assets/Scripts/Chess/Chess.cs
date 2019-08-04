using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Chess
{
    public enum ChessType
    {
        Soldier,
        Chariot,
        Horse,
        Cannon,
        Elephant,
        Guard,
        General
    }

    public enum PlayerColor
    {
        Red,
        Black
    }

    [CreateAssetMenu(fileName = "New Chess", menuName = "Chess/Chess")]
    public class Chess : ScriptableObject
    {
        public int MaxHealth;
        public int AttackDamage;
        public ChessType ChessType;
        public Sprite SpriteBlack;
        public Sprite SpriteRed;
    }

    public class ChessPiece : MonoBehaviour
    {
        private Chess _chess;
        private PlayerColor _color;
        private int _health;
        public Vector2 Position { get; private set; }
        private List<Vector2> _availableMovePositions = new List<Vector2>();

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
            _color = color;
            _health = _chess.MaxHealth;
            Position = color == PlayerColor.Red? config.Position : ChessBoard.GetMirrorPosition(config.Position);
            ChessBoard.Instance.ChessPiecesOnBoard.Add(this);
            transform.position = ChessBoard.GetWorldPosition(Position);
        }

        private void OnMouseDown()
        {
            GetAvailableMovePositions();
            foreach (var position in _availableMovePositions)
            {
                
            }
        }

        private void GetAvailableMovePositions()
        {
            _availableMovePositions.Clear();
            switch (_chess.ChessType)
            {
                case ChessType.Soldier:
                    var stepForward = Position + StepsLinear[_color == PlayerColor.Red ? 0 : 1];
                    TryAddMovePosition(stepForward);
                    if (Position.y > 4 && _color == PlayerColor.Red || Position.y < 5 && _color == PlayerColor.Black)
                    {
                        TryAddMovePosition(Position + StepsLinear[2]);
                        TryAddMovePosition(Position + StepsLinear[3]);
                    }
                    break;
                case ChessType.Chariot:
                case ChessType.Cannon:
                    foreach (var step in StepsLinear)
                    {
                        var tempPosition = Position + step;
                        while (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition) == null)
                        {
                            if (!TryAddMovePosition(tempPosition)) break;
                            tempPosition += step;
                        } 
                    }
                    break;
                case ChessType.Horse:
                    foreach (var step1 in StepsLinear)
                    {
                        var tempPosition1 = Position + step1;
                        if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition1) != null) continue;
                        foreach (var step2 in StepsDiagonal)
                        {
                            if ((step1 + step2).sqrMagnitude < 5) continue;
                            var tempPosition2 = tempPosition1 + step2;
                            if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition1) != null)
                                TryAddMovePosition(tempPosition2);
                        }
                    }
                    break;
                case ChessType.Elephant:
                    foreach (var step in StepsDiagonal)
                    {
                        var tempPosition1 = Position + step;
                        if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition1) != null) continue;
                        var tempPosition2 = tempPosition1 + step;
                            if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition1) != null)
                                TryAddMovePosition(tempPosition2);
                    }
                    break;
                case ChessType.Guard:
                    foreach (var step in StepsDiagonal)
                    {
                        var tempPosition = Position + step;
                        if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition) == null)
                            TryAddMovePosition(tempPosition);
                    }
                    break;
                case ChessType.General:
                    foreach (var step in StepsLinear)
                    {
                        var tempPosition = Position + step;
                        if (ChessBoard.Instance.GetChessPieceAtPosition(tempPosition) == null)
                            TryAddMovePosition(tempPosition);
                    }
                    break;
            }
        }

        private bool TryAddMovePosition(Vector2 position)
        {
            if (!IsPositionInBoundary(position)) return false;
            _availableMovePositions.Add(position);
            return true;
        }

        private bool IsPositionInBoundary(Vector2 position)
        {
            if (_color == PlayerColor.Black)
                position = ChessBoard.GetMirrorPosition(position);
            switch (_chess.ChessType)
            {
                case ChessType.Elephant:
                    if (position.y > 4) return false;
                    break;
                case ChessType.Guard:
                case ChessType.General:
                    if (position.y > 2 || position.x < 3 || position.x > 5) return false;
                    break;
            }
            return position.x > 0 && position.y > 0 && position.x <= 8 && position.y <= 9;
        }

        public void Move(Vector2 position)
        {
            var targetChess = ChessBoard.Instance.GetChessPieceAtPosition(position);
            if (targetChess)
                Attack(targetChess);
            else
                transform.position = ChessBoard.GetWorldPosition(position);
            
            ChessBoard.Instance.SwitchPlayer();
        }

        private void Attack(ChessPiece target)
        {
            if (target._color == _color) return;
            var position = target.Position;
            target.TakeDamage(this);
            if (!target)
                transform.position = ChessBoard.GetWorldPosition(position);
        }

        private void TakeDamage(ChessPiece source)
        {
            _health -= source._chess.AttackDamage;
            if (_health <= 0) 
                Die();
        }

        private void Die()
        {
            ChessBoard.Instance.KillChessPiece(this);
            Destroy(gameObject);
        }
    }
}
