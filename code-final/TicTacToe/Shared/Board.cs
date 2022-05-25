using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TicTacToe.Shared
{
    public class Board
    {
        public static Board Initialize() =>
            new Board(ImmutableList<Cell>.Empty);

        private Board(ImmutableList<Cell> moves)
        {
            this.Moves = moves;
        }

        public IEnumerable<Cell> HomeMoves =>
            this.Moves.WhereEvenOffset();

        public IEnumerable<Cell> AwayMoves =>
            this.Moves.WhereOddOffset();

        public IEnumerable<Line> WinningLines =>
            Line.FullLines(this.HomeMoves)
                .Concat(Line.FullLines(this.AwayMoves));

        public IEnumerable<Cell> PlayableCells =>
            this.WinningLines.Any() ? Enumerable.Empty<Cell>()
            : Cell.FullBoard().Except(this.Moves);

        private ImmutableList<Cell> Moves { get; }

        public Board Restart() =>
            new Board(ImmutableList<Cell>.Empty);

        public Board Play(int row, int col) =>
            this.Play(new Cell(row, col));
            
        private Board Play(Cell at) =>
            new(this.Moves.Add(this.Playable(at)));

        private Cell Playable(Cell at) =>
            this.PlayableCells.First(playable => playable.Equals(at));

        public IEnumerable<Move> LegalMoves =>
            this.PlayableCells.Select(cell => new Move(cell, () => this.Play(cell)));
    }

    public class Move
    {
        public Cell At { get; }
        private Lazy<Board> Result { get; }
        public Move(Cell at, Func<Board> boardFactory)
        {
            this.At = at;
            this.Result = new(boardFactory);
        }

        public Board Make() => this.Result.Value;
    }
}
