using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_WPF_Natif.Model.ML
{
    /// <summary>
    ///     Simon Dery - 11 décembre 2025
    ///     Gestion des move possible à l'entrainement
    /// </summary>
    public static class MoveHandler
    {
        private static Dictionary<Moves, Move> _availableMoves;

        static MoveHandler()
        {
            _availableMoves = new Dictionary<Moves, Move>()
            {
                { Moves.None, new Move() { MoveId = Moves.None, DisplayName = "Aucun", Label = "None", ImagePath = $"{Constants.InitialMoveImagePath}/None.png"}},

                { Moves.Pushup_Up, new Move() { MoveId = Moves.Pushup_Up, DisplayName = "Pushup Haut", Label = "Pushup_Up", ImagePath = $"{Constants.InitialMoveImagePath}/Pushup_Active.png"}},
                { Moves.Pushup_Down, new Move() { MoveId = Moves.Pushup_Down, DisplayName = "Pushup bas", Label = "Pushup_Down", ImagePath = $"{Constants.InitialMoveImagePath}/Pushup_Rest.png"}},

                { Moves.Squat_Up, new Move() { MoveId = Moves.Squat_Up, DisplayName = "Squat haut", Label = "Squat_Up", ImagePath = $"{Constants.InitialMoveImagePath}/Squat_Active.png"}},
                { Moves.Squat_Down, new Move() { MoveId = Moves.Squat_Down, DisplayName = "Squat bas", Label = "Squat_Down", ImagePath = $"{Constants.InitialMoveImagePath}/Squat_Rest.png"}},

                { Moves.JumpingJack_Up, new Move() { MoveId = Moves.JumpingJack_Up, DisplayName = "Jumping jack haut", Label = "JumpingJack_Up", ImagePath = $"{Constants.InitialMoveImagePath}/JumpingJack_Active.png"}},
                { Moves.JumpingJack_Down, new Move() { MoveId = Moves.JumpingJack_Down, DisplayName = "Jumping jack bas", Label = "JumpingJack_Down", ImagePath = $"{Constants.InitialMoveImagePath}/JumpingJack_Rest.png"}},

            };
        }

        public enum Moves
        {
            None = 0,
            Pushup_Up,
            Pushup_Down,
            Squat_Up,
            Squat_Down,
            JumpingJack_Up, 
            JumpingJack_Down
        }

        /// <summary>
        ///     Simon Dery - 11 décembre 2025
        ///     Retourne le move selon l'enum
        /// </summary>
        public static Move GetMove(Moves moveEnum) => _availableMoves[moveEnum];

        /// <summary>
        ///     Simon Dery - 11 décembre 2025
        ///     Retourne tous les moves disponibles
        /// </summary>
        public static List<Move> GetAllMoves() => _availableMoves.Select(m => m.Value).ToList();

        /// <summary>
        ///     Simon Dery - 11 décembre 2025
        ///     Move possible à l'entrainement     
        /// </summary>
        public class Move
        {
            public Moves MoveId { get; set; }

            public string DisplayName { get; set; }

            public string Label { get; set; }

            public string ImagePath { get; set; }
        }
    }
}
