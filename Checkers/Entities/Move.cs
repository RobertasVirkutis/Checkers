namespace Checkers.Entities
{
    public class Move
    {
        public Location Location { get; set; }
        public MoveType Type { get; set; }
        public bool IsCaptureMove { get; set; }
        public Location? CapturedPieceLocation { get; set; }

        public Move() { }

        public Move(Location location, MoveType type, bool isCaptureMove = false)
        {
            Location = location;
            Type = type;
            IsCaptureMove = isCaptureMove;
        }
    }
}
