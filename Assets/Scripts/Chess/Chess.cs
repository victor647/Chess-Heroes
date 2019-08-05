using UnityEngine;

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
}
