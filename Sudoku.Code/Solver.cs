using Sudoku.Code.Models;

namespace Sudoku.Code
{
    public class Solver
    {
        /* Board Pattern:
         * _ _ _ # _ _ _ # _ _ _ 
         * _ 1 _ # _ 2 _ # _ 3 _
         * _ _ _ # _ _ _ # _ _ _
         * # # # # # # # # # # #
         * _ _ _ # _ _ _ # _ _ _
         * _ 4 _ # _ 5 _ # _ 6 _
         * _ _ _ # _ _ _ # _ _ _
         * # # # # # # # # # # #
         * _ _ _ # _ _ _ # _ _ _
         * _ 7 _ # _ 8 _ # _ 9 _
         * _ _ _ # _ _ _ # _ _ _
         */

        private readonly int[,] _board;

        public Solver() => _board = new int[9, 9];

        public int GetValue(int x, int y) => _board[x, y];

        public void SetValue(int x, int y, int value)
        {
            if (x > 9 || x < 0 || y > 9 || y < 0)
                throw new IndexOutOfRangeException();

            if (value is > 9 or < 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            //if (!_board[x, y].Equals(default))
            //    throw new NotSupportedException($"Incorrect input, this place already contains value");

            _board[x, y] = value;
            if (!IsBoardValid(new Coordinate(x, y)))
            {
                _board[x, y] = default;
                throw new NotSupportedException($"Incorrect input, value ({value}) in this place breaks one of the sudoku rules");
            }
        }

        public bool SolveBoard(Coordinate? pos = null)
        {
            if (pos != null && !IsBoardValid(pos.Value))
                return false;

            pos = GetFirstFreeCoordinate();
            if (pos == null)
                return true;

            for (int i = 1; i < 10; i++)
            {
                _board[pos.Value.X, pos.Value.Y] = i;
                if (SolveBoard(pos))
                    return true;
            }

            _board[pos.Value.X, pos.Value.Y] = default;
            return false;
        }

        private Coordinate? GetFirstFreeCoordinate()
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (_board[x, y].Equals(default))
                        return new Coordinate(x, y);
                }
            }

            return null;
        }

        private bool IsBoardValid(Coordinate pos)
        {
            var value = _board[pos.X, pos.Y];

            //row
            for (int x = 0; x < 9; x++)
            {
                if (pos.X == x)
                    continue;

                if (value == _board[x, pos.Y])
                    return false;
            }

            //Block
            int boardX = pos.X / 3 * 3;
            int boardY = pos.Y / 3 * 3;

            for (int blockY = 0; blockY < 3; blockY++)
            {
                for (int blockX = 0; blockX < 3; blockX++)
                {
                    if (boardX + blockX == pos.X && boardY + blockY == pos.Y)
                        continue;

                    if (value == _board[boardX + blockX, boardY + blockY])
                        return false;
                }
            }

            //column
            for (int y = 0; y < 9; y++)
            {
                if (pos.Y == y)
                    continue;

                if (value == _board[pos.X, y])
                    return false;
            }

            return true;
        }
    }
}