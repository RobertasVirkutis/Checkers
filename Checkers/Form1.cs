using Checkers.Entities;
using System.Media;

namespace Checkers
{
    public partial class Form1 : Form
    {
        public PictureBox[,] GameBoard { get; set; } = new PictureBox[8, 8];
        public string PlayerToMove { get; set; } = "b";
        public Piece ActivePiece { get; set; }
        public List<Move> PossibleMoves { get; set; } = new List<Move>();
        public Player PlayerBlack { get; set; } = new Player { Color = "Black", CapturedPieces = 0 };
        public Player PlayerWhite { get; set; } = new Player { Color = "White", CapturedPieces = 0 };
        public bool MultiCaptureLockActivated { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadBoard(object sender, EventArgs e)
        {
            int top = 3;

            for (int i = 0; i < 8; i++)
            {
                var left = 3;

                var primaryColor = (i + 1) % 2 == 0 ? Color.White : Color.Black;
                var secondaryColor = (i + 1) % 2 != 0 ? Color.White : Color.Black;
                var playerColor = i < 3 ? "w" : i > 4 ? "b" : "-";

                for (int j = 0; j < 8; j++)
                {
                    GameBoard[i, j] = new PictureBox
                    {
                        BackColor = (j + 1) % 2 == 0 ? primaryColor : secondaryColor,
                        Size = new Size(90, 90),
                        Location = new Point(left, top)
                    };

                    left += 90;
                    GameBoard[i, j].AccessibleName = $"{i},{j},{playerColor}";
                    GameBoard[i, j].Click += (sender, e) =>
                    {
                        MakeMove(sender);
                    };
                    if (GameBoard[i, j].BackColor == Color.Black && !playerColor.Equals("-"))
                    {
                        GameBoard[i, j].Image = playerColor == "w" ? Resources.wpc : Resources.bpc;
                    }

                    GameBoard[i, j].SizeMode = PictureBoxSizeMode.CenterImage;
                    Board.Controls.Add(GameBoard[i, j]);
                }

                top += 90;
            }

            PlaySound(Resources.start);
        }

        private void MakeMove(object? sender)
        {
            var pieceClicked = sender as PictureBox;

            var location = pieceClicked.AccessibleName.Split(",");
            var row = int.Parse(location[0]);
            var column = int.Parse(location[1]);
            var pieceType = location[2];

            if (GameBoard[row, column].Image is null)
                CompleteMove(row, column);
            else
                ActivatePiece(row, column, pieceType, pieceClicked);
        }

        private void CompleteMove(int row, int column)
        {
            if (MultiCaptureLockActivated && !PossibleMoves.Any(x => x.Location.Row == row && x.Location.Column == column))
                return;

            if (ActivePiece is not null)
            {
                var move = PossibleMoves.FirstOrDefault(x => x.Location.Row == row && x.Location.Column == column);
                if (move is not null && ActivePiece.Type.EndsWith(PlayerToMove))
                {
                    GameBoard[row, column].Image = SelectPieceImage(row, ActivePiece.Type, GameBoard[ActivePiece.Row, ActivePiece.Column].Image, out bool pieceUpgraded);
                    GameBoard[ActivePiece.Row, ActivePiece.Column].Image = null;

                    var newPieceType = $"{(pieceUpgraded ? "k" : string.Empty)}{ActivePiece.Type}";
                    GameBoard[row, column].AccessibleName = $"{row},{column},{newPieceType}";
                    GameBoard[ActivePiece.Row, ActivePiece.Column].AccessibleName = $"{ActivePiece.Row},{ActivePiece.Column},-";

                    bool skipToAnotherMove = false;
                    if (move.IsCaptureMove)
                        CommitCapture(move.CapturedPieceLocation, move.Location, out skipToAnotherMove);

                    var soundType = move.IsCaptureMove ? Resources.capture : Resources.move;
                    if (pieceUpgraded)
                        soundType = Resources.upgrade;

                    PlaySound(soundType);
                    CheckForGameEnd();

                    if (skipToAnotherMove)
                        return;

                    SwitchTurn();
                }

                GameBoard[ActivePiece.Row, ActivePiece.Column].BackColor = Color.Black;
            }

            DisplayPossibleMoves(false);
            PossibleMoves.Clear();
            ActivePiece = null;
            MultiCaptureLockActivated = false;
        }

        private void CheckForGameEnd()
        {
            if (PlayerWhite.CapturedPieces < 12 && PlayerBlack.CapturedPieces < 12)
                return;

            var winnerColorFormatted = PlayerWhite.CapturedPieces > 12 ? "balta" : "juoda";
            var startNewDialog = new StartNewDialogForm(winnerColorFormatted);
            var dialogResult = startNewDialog.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                var checkersGameForm = new Form1();
                var newThread = new Thread(() => OpenForm(checkersGameForm));
                newThread.SetApartmentState(ApartmentState.STA);
                this.Close();
                newThread.Start();

                return;
            }

            this.Close();
        }

        private void OpenForm<T>(T form) where T : Form => Application.Run(form);

        private void PlaySound(UnmanagedMemoryStream soundSource)
        {
            var sound = new SoundPlayer(soundSource);
            sound.Play();
        }

        private void CommitCapture(Location capturedPieceLocation, Location moveLocation, out bool skipToAnotherMove)
        {
            skipToAnotherMove = false;
            GameBoard[capturedPieceLocation.Row, capturedPieceLocation.Column].Image = null;
            GameBoard[capturedPieceLocation.Row, capturedPieceLocation.Column].AccessibleName = $"{capturedPieceLocation.Row},{capturedPieceLocation.Column},-";

            if (PlayerToMove.Equals("b"))
                PlayerBlack.CapturedPieces++;
            else
                PlayerWhite.CapturedPieces++;

            DisplayPossibleMoves(false);
            PossibleMoves.Clear();
            GameBoard[ActivePiece.Row, ActivePiece.Column].BackColor = Color.Black;

            ActivePiece.Row = moveLocation.Row;
            ActivePiece.Column = moveLocation.Column;
            GameBoard[moveLocation.Row, moveLocation.Column].BackColor = Color.GreenYellow;

            RecalculatePossibleMoves();
            var otherCaptureMoves = PossibleMoves.Where(x => x.IsCaptureMove)?.ToList();
            if (otherCaptureMoves?.Any() == true)
            {
                PossibleMoves = otherCaptureMoves;
                DisplayPossibleMoves(true);
                skipToAnotherMove = true;
                MultiCaptureLockActivated = true;
            }
        }

        private Image SelectPieceImage(int row, string pieceType, Image pieceImage, out bool pieceUpgraded)
        {
            pieceUpgraded = true;

            if (pieceType == "b" && row == 0)
                return Resources.blackpc;
            if (pieceType == "w" && row == 7)
                return Resources.whitepc;

            pieceUpgraded = false;
            return pieceImage;
        }

        private void SwitchTurn() => PlayerToMove = PlayerToMove == "b" ? "w" : "b";


        private void ActivatePiece(int row, int column, string pieceType, PictureBox pieceClicked)
        {
            if (MultiCaptureLockActivated)
                return;

            DisplayPossibleMoves(false);
            PossibleMoves.Clear();

            if (ActivePiece is not null)
                GameBoard[ActivePiece.Row, ActivePiece.Column].BackColor = Color.Black;

            if (!pieceType.EndsWith(PlayerToMove))
                return;

            GameBoard[row, column].BackColor = Color.GreenYellow;
            ActivePiece = new Piece { Row = row, Column = column, Type = pieceType };
            RecalculatePossibleMoves();
            DisplayPossibleMoves(true);
        }

        private void RecalculatePossibleMoves(int? rowToCalculate = null, int? columnToCalculate = null)
        {
            var row = rowToCalculate ?? ActivePiece.Row;
            var column = columnToCalculate ?? ActivePiece.Column;
            var pieceType = ActivePiece.Type;

            var moves = GetPossibleMovesLocations(row, column, pieceType);

            foreach (var move in moves)
            {
                if (ValidatePossibleMove(move.Location.Row, move.Location.Column, out bool tileOccupiedByOpponent))
                    PossibleMoves.Add(new Move(move.Location, move.Type));

                if (tileOccupiedByOpponent)
                    HandlePossibleCapture(move.Location, move.Type);
            }
        }

        private void HandlePossibleCapture(Location location, MoveType moveType)
        {

            var whiteOrKingPieceMoving = ActivePiece.Type.Equals("w") || ActivePiece.Type.Contains("k");
            var blackOrKingPieceMoving = ActivePiece.Type.Equals("b") || ActivePiece.Type.Contains("k");

            Location? possibleCaptureLocation = moveType switch
            {
                MoveType.TopLeft => blackOrKingPieceMoving ? GetTopLeftMoveLocation(location.Row, location.Column) : null,
                MoveType.TopRight => blackOrKingPieceMoving ? GetTopRightMoveLocation(location.Row, location.Column) : null,
                MoveType.BottomLeft => whiteOrKingPieceMoving ? GetBottomLeftMoveLocation(location.Row, location.Column) : null,
                MoveType.BottomRight => whiteOrKingPieceMoving ? GetBottomRightMoveLocation(location.Row, location.Column) : null,
            };

            if (possibleCaptureLocation is not null && ValidatePossibleMove(possibleCaptureLocation.Row, possibleCaptureLocation.Column, out _))
                PossibleMoves.Add(new Move { Location = possibleCaptureLocation, Type = moveType, IsCaptureMove = true, CapturedPieceLocation = location });
        }

        private List<Move> GetPossibleMovesLocations(int row, int column, string pieceType)
        {
            var moves = new List<Move>();

            if (pieceType.Equals("b") || pieceType.Contains("k"))
            {
                var topLeftMoveLocation = GetTopLeftMoveLocation(row, column);
                moves.Add(new Move(topLeftMoveLocation, MoveType.TopLeft));

                var topRightMoveLocation = GetTopRightMoveLocation(row, column);
                moves.Add(new Move(topRightMoveLocation, MoveType.TopRight));
            }


            if (pieceType.Equals("w") || pieceType.Contains("k"))
            {
                var bottomLeftMoveLocation = GetBottomLeftMoveLocation(row, column);
                moves.Add(new Move(bottomLeftMoveLocation, MoveType.BottomLeft));

                var bottomRightMoveLocation = GetBottomRightMoveLocation(row, column);
                moves.Add(new Move(bottomRightMoveLocation, MoveType.BottomRight));
            }

            return moves;
        }

        private Location GetTopLeftMoveLocation(int row, int column) => new() { Row = row - 1, Column = column - 1 };
        private Location GetTopRightMoveLocation(int row, int column) => new() { Row = row - 1, Column = column + 1 };
        private Location GetBottomLeftMoveLocation(int row, int column) => new() { Row = row + 1, Column = column - 1 };
        private Location GetBottomRightMoveLocation(int row, int column) => new() { Row = row + 1, Column = column + 1 };

        private void DisplayPossibleMoves(bool showActive)
        {
            foreach (var move in PossibleMoves)
                GameBoard[move.Location.Row, move.Location.Column].BackColor = showActive ? Color.LightSkyBlue : Color.Black;
        }

        private bool ValidatePossibleMove(int row, int column, out bool tileOccupiedByOpponent)
        {
            tileOccupiedByOpponent = false;

            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;

            if (GameBoard[row, column].Image is not null)
            {
                if (!GameBoard[row, column].AccessibleName.Contains(PlayerToMove))
                    tileOccupiedByOpponent = true;

                return false;
            }

            return true;
        }
    }
}