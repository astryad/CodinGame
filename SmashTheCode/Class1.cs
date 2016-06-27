using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
namespace SmashTheCode
{
    public class Player
    {
        public static void Main(string[] args)
        {
            var game = new Game();

            while (true)
            {
                Console.WriteLine(game.ResolveTurn());
            }
        }
    }

    public class Game
    {
        private const int BoardHeight = 12;
        private readonly IConsole _console;

        public Game() : this(new SystemConsole())
        {

        }

        public Game(IConsole console)
        {
            _console = console;
        }

        public string ResolveTurn()
        {
            ReadNextTurns();

            _console.Debug("Next turn color: " + NextTurns[0].Top);

            _console.ReadLine();
            ReadPlayerBoard();
            _console.ReadLine();
            ReadOpponentBoard();

            var board = new Board(PlayerBoard);

            var plays = new[]
            {
                new {Column = 0, Rotation = 0},
                new {Column = 0, Rotation = 1},
                new {Column = 0, Rotation = 2},
                new {Column = 0, Rotation = 3},
                new {Column = 1, Rotation = 0},
                new {Column = 1, Rotation = 1},
                new {Column = 1, Rotation = 2},
                new {Column = 1, Rotation = 3},
                new {Column = 2, Rotation = 0},
                new {Column = 2, Rotation = 1},
                new {Column = 2, Rotation = 2},
                new {Column = 2, Rotation = 3},
                new {Column = 3, Rotation = 0},
                new {Column = 3, Rotation = 1},
                new {Column = 3, Rotation = 2},
                new {Column = 3, Rotation = 3},
                new {Column = 4, Rotation = 0},
                new {Column = 4, Rotation = 1},
                new {Column = 4, Rotation = 2},
                new {Column = 4, Rotation = 3},
                new {Column = 5, Rotation = 0},
                new {Column = 5, Rotation = 1},
                new {Column = 5, Rotation = 2},
                new {Column = 5, Rotation = 3},
            };

            var playScore = plays.Select(
                play =>
                {
                    var nextBoard = board.Play(NextTurns[0], play.Column, play.Rotation);
                    var score = nextBoard.ResolveCombos();

                    var secondTurnPlays = plays.Select(
                        p =>
                        {
                            var secondTurnBoard = nextBoard.Play(NextTurns[1], p.Column, p.Rotation);
                            var secondTurnScore = secondTurnBoard.ResolveCombos();
                            var result = new
                            {
                                Play = p,
                                Score = secondTurnScore,
                                Value = secondTurnBoard.Value
                            };
                            return result;
                        });

                    score += secondTurnPlays.OrderByDescending(p => p.Score).ThenByDescending(p => p.Value).First().Score;

                    return new
                    {
                        Play = play,
                        Score = score,
                        Value = nextBoard.Value
                    };
                });

            var bestPlay = playScore.OrderByDescending(play => play.Score).ThenByDescending(play => play.Value).First();

            return string.Format("{0} {1}", bestPlay.Play.Column, bestPlay.Play.Rotation);
        }

        private void ReadPlayerBoard()
        {
            PlayerBoard = new string[BoardHeight];

            for (var i = 0; i < BoardHeight; i++)
            {
                PlayerBoard[i] = _console.ReadLine();
            }
        }

        private void ReadOpponentBoard()
        {
            OpponentBoard = new string[BoardHeight];

            for (var i = 0; i < BoardHeight; i++)
            {
                // One line of the map ('.' = empty, '0' = skull block, '1' to '5' = colored block)
                OpponentBoard[i] = _console.ReadLine();
            }
        }

        private void ReadNextTurns()
        {
            NextTurns = new TurnBlocks[8];

            for (int i = 0; i < 8; i++)
            {
                string[] inputs = _console.ReadLine().Split(' ');

                var color = new TurnBlocks
                {
                    Top = inputs[0][0],
                    Bottom = inputs[1][0]
                };

                NextTurns[i] = color;
            }
        }

        public TurnBlocks[] NextTurns { get; private set; }
        public string[] PlayerBoard { get; set; }
        public string[] OpponentBoard { get; set; }
    }

    public interface IBoard
    {
        string[] BoardData { get; }
        int Value { get; set; }
        int ResolveCombos();
        int CalculateCombo(int row, int column);
        void UpdateBoard();
        bool IsVisited(int row, int column);
        char GetBlockType(int row, int column);
        IBoard Play(TurnBlocks turnBlocks, int column, int rotation);
    }

    public class Board : IBoard
    {
        private readonly Block[][] _blocks;

        public string[] BoardData
        {
            get { return _blocks.Select(row => string.Join("", row.Select(block => block.BlockType))).ToArray(); }
        }

        public Board(string[] boardData)
        {
            _blocks = new Block[12][];
            for (var i = 0; i < 12; i++)
            {
                _blocks[i] = new Block[6];
                for (var j = 0; j < 6; j++)
                {
                    _blocks[i][j] = new Block(boardData[i][j], i, j);
                }
            }
        }

        private class Block
        {
            public char BlockType { get; set; }
            public int Row { get; private set; }
            public int Column { get; private set; }

            public Block(char blockType, int row, int column)
            {
                Visited = false;
                BlockType = blockType;
                Row = row;
                Column = column;
            }

            public bool Visited { get; set; }
        }

        public int ResolveCombos()
        {
            var score = 0;

            int turnScore = 0;
            do
            {
                turnScore = 0;
                for (int i = 11; i >= 0; i--)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        turnScore += CalculateCombo(i, j);
                    }
                }

                UpdateBoard();
                score += turnScore;
            } while (turnScore != 0);

            return score;
        }

        public int CalculateCombo(int row, int column)
        {
            var start = _blocks[row][column];

            if (start.BlockType == '.' || start.BlockType == '0')
                return 0;

            var startType = start.BlockType;

            var blocksToCheck = new Stack<Block>();
            blocksToCheck.Push(start);

            var comboBlocks = new List<Block>();

            while (blocksToCheck.Count > 0)
            {
                var block = blocksToCheck.Pop();

                if (!block.Visited && block.BlockType == startType)
                {
                    comboBlocks.Add(block);
                    if (block.Column > 0) blocksToCheck.Push(_blocks[block.Row][block.Column - 1]);
                    if (block.Column < 5) blocksToCheck.Push(_blocks[block.Row][block.Column + 1]);
                    if (block.Row > 0) blocksToCheck.Push(_blocks[block.Row - 1][block.Column]);
                    block.Visited = true;
                }
            }
            Value += Enumerable.Range(1, comboBlocks.Count).Sum();
            if (comboBlocks.Count > 3)
            {
                comboBlocks.ForEach(block => block.BlockType = '.');
                return comboBlocks.Count;
            }

            return 0;
        }

        public int Value { get; set; }

        public void UpdateBoard()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 11; j >= 0; j--)
                {
                    _blocks[j][i].Visited = false;

                    if (_blocks[j][i].BlockType == '.')
                    {
                        int shift = j;
                        while (shift >= 0 && _blocks[shift][i].BlockType == '.')
                            shift--;

                        int row = j;
                        while (shift >= 0)
                        {
                            _blocks[row][i].BlockType = _blocks[shift][i].BlockType;
                            row--;
                            shift--;
                        }

                        while (row >= 0)
                        {
                            _blocks[row][i].BlockType = '.';
                            row--;
                        }
                    }
                }
            }

            Value = 0;
        }

        public bool IsVisited(int row, int column)
        {
            return _blocks[row][column].Visited;
        }

        public char GetBlockType(int row, int column)
        {
            return _blocks[row][column].BlockType;
        }

        public IBoard Play(TurnBlocks turnBlocks, int column, int rotation)
        {
            if (!IsValidPlay(column, rotation))
                return new InvalidBoard();

            int[] columnTops = new int[6];

            for (int i = 0; i < 6; i++)
                columnTops[i] = -1;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (_blocks[i][j].BlockType == '.')
                        columnTops[j] = i;
                }
            }

            var newBoard = new Board(BoardData);

            if (rotation == 0)
            {
                if (columnTops[column] == -1) return new InvalidBoard();
                if (columnTops[column + 1] == -1) return new InvalidBoard();

                newBoard._blocks[columnTops[column]][column].BlockType = turnBlocks.Top;
                newBoard._blocks[columnTops[column + 1]][column + 1].BlockType = turnBlocks.Bottom;
            }

            if (rotation == 1)
            {
                var row = columnTops[column];
                if (row <= 0)
                    return new InvalidBoard();

                newBoard._blocks[row][column].BlockType = turnBlocks.Top;
                newBoard._blocks[row - 1][column].BlockType = turnBlocks.Bottom;
            }

            if (rotation == 2)
            {
                if (columnTops[column] == -1) return new InvalidBoard();
                if (columnTops[column - 1] == -1) return new InvalidBoard();

                newBoard._blocks[columnTops[column]][column].BlockType = turnBlocks.Top;
                newBoard._blocks[columnTops[column - 1]][column - 1].BlockType = turnBlocks.Bottom;
            }

            if (rotation == 3)
            {
                var row = columnTops[column];
                if (row <= 0)
                    return new InvalidBoard();

                newBoard._blocks[row][column].BlockType = turnBlocks.Bottom;
                newBoard._blocks[row - 1][column].BlockType = turnBlocks.Top;
            }

            return newBoard;
        }

        private bool IsValidPlay(int column, int rotation)
        {
            if (column == 0 && rotation == 2)
                return false;
            if (column == 5 && rotation == 0)
                return false;

            return true;
        }
    }

    public class InvalidBoard : IBoard
    {
        public string[] BoardData { get; private set; }
        public int Value { get; set; }

        public int ResolveCombos()
        {
            return -100;
        }

        public int CalculateCombo(int row, int column)
        {
            throw new NotImplementedException();
        }

        public void UpdateBoard()
        {
            throw new NotImplementedException();
        }

        public bool IsVisited(int row, int column)
        {
            throw new NotImplementedException();
        }

        public char GetBlockType(int row, int column)
        {
            throw new NotImplementedException();
        }

        public IBoard Play(TurnBlocks turnBlocks, int column, int rotation)
        {
            return this;
        }
    }

    public struct TurnBlocks
    {
        public char Top { get; set; }
        public char Bottom { get; set; }
    }

    public interface IConsole
    {
        string ReadLine();

        void Debug(string message);
    }

    public class SystemConsole : IConsole
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Debug(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}