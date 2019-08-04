using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    [Serializable]
    public struct ChessConfig
    {
        public Chess Chess;
        public Vector2 Position;
    }
    
    [CreateAssetMenu(fileName = "ChessPositions", menuName = "Chess/PositionConfig")]
    public class ChessPositionConfig : ScriptableObject
    {
        public List<ChessConfig> ChessConfigs = new List<ChessConfig>();
    }
}
