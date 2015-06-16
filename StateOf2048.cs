using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace AIAssignment1.G2048
{
        public class StateOf2048
    {
        public readonly byte BOARD_SIZE = 4;

        public readonly byte[,] stateTable;

        public byte[,] sTable;

        public readonly int Score = 0;

        public int diem;


        public byte cache_EmptyCells;

        /// <Separate row>
        ///Gets or sets the character separating each rows of state represented in string. Default is ';'.
        /// </Separate row>
        public static char RowSeparator = ';';

        /// <Separate Colum>
        /// Gets or sets the character separating each column of state represented in string. Default is ','.
        /// </Separate Colum>
        public static char ColSeparator = ',';

        /// <Get State>
        /// Gets the state represented in string.
        /// </Get State>
        public readonly string State;

        public StateOf2048(String stateString)
        {
            this.stateTable = new byte[BOARD_SIZE, BOARD_SIZE];
            int i = 0;
            foreach (var row in stateString.Split(RowSeparator))
            {
                int j = 0;
                foreach (var col in row.Split(ColSeparator))
                {

                    stateTable[i, j] = byte.Parse(col);
                    j++;
                }
                i++;
            }

            this.State = stateString;
        }

        public StateOf2048(byte[,] stateTable)
        {
            // TODO: Complete member initialization
            this.stateTable = stateTable;
            this.State = convertToString(this.stateTable);
            this.Score = diem;
        }

        public string convertToString(byte[,] sTable)
        {
            var colSep = "" + ColSeparator;
            var rowSep = "" + RowSeparator;
            var rowList = new List<string>();

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                var colList = new List<string>();
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    colList.Add("" + stateTable[i, j]);
                }
                rowList.Add(string.Join(colSep, colList));
            }
            return string.Join(rowSep, rowList);
        }

        /// <Gets a collection of states which can be generated from this state>
        /// <returns>A collection contains all generated states.</returns>
        public IEnumerable<StateOf2048> GetNextStates()
        {
            StateOf2048[] Listxt = new StateOf2048[4];
            for (int i = 0; i < 4; i++)
            {
                var nextState = move((MoveDirection)i);
                var nt = new StateOf2048(addRandomCell(move((MoveDirection)i)));
                nt.diem = diem;
                if (convertToString(move((MoveDirection)i)) == State && nt!= null) yield return nt;
            }
        }

        public enum MoveDirection { Right = 0, Up, Down, Left };

        public byte[,] move(MoveDirection direction)
        {
            sTable = stateTable;
            diem = Score;
            int point = 0;

            //rotate the board to make simplify the merging algorithm
            if (direction == MoveDirection.Up)
            {
                rotateLeft();
            }
            else if (direction == MoveDirection.Right)
            {
                rotateLeft();
                rotateLeft();
            }
            else if (direction == MoveDirection.Down)
            {
                rotateRight();
            }

            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                int lastMergePosition = 0;
                for (int j = 1; j < BOARD_SIZE; ++j)
                {
                    if (sTable[i, j] == 0)
                    {
                        continue; //skip moving zeros
                    }

                    int previousPosition = j - 1;
                    while (previousPosition > lastMergePosition && sTable[i, previousPosition] == 0)
                    { //skip all the zeros
                        --previousPosition;
                    }

                    if (previousPosition == j)
                    {
                        //we can't move this at all
                    }
                    else if (sTable[i, previousPosition] == 0)
                    {
                        //move to empty value
                        sTable[i, previousPosition] = sTable[i, j];
                        sTable[i, j] = 0;
                    }
                    else if (sTable[i, previousPosition] == sTable[i, j])
                    {
                        //merge with matching value
                        sTable[i, previousPosition] +=1;
                        sTable[i, j] = 0;
                        point += sTable[i, previousPosition];
                        lastMergePosition = previousPosition + 1;

                    }
                    else if (sTable[i, previousPosition] != sTable[i, j] && previousPosition + 1 != j)
                    {
                        sTable[i, previousPosition + 1] = sTable[i, j];
                        sTable[i, j] = 0;
                    }
                }
            }

            diem += point;

            //reverse back the board to the original orientation
            if (direction == MoveDirection.Up)
            {
                rotateRight();
            }
            else if (direction == MoveDirection.Right)
            {
                rotateRight();
                rotateRight();
            }
            else if (direction == MoveDirection.Down)
            {
                rotateLeft();
            }

            return sTable;
        }

        private void rotateLeft()
        {
            byte[,] rotatedBoard = new byte[BOARD_SIZE, BOARD_SIZE];

            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    rotatedBoard[BOARD_SIZE - j - 1, i] = sTable[i, j];
                }
            }

            sTable = rotatedBoard;
        }
        private void rotateRight()
        {
            byte[,] rotatedBoard = new byte[BOARD_SIZE, BOARD_SIZE];

            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    rotatedBoard[i, j] = sTable[BOARD_SIZE - j - 1, i];
                }
            }

            sTable = rotatedBoard;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var state = obj as StateOf2048;
            if (state == null)
                return false;

            return (this.State == state.State);
        }

        public override int GetHashCode()
        {
            return this.State.GetHashCode() ^ BOARD_SIZE;
        }

        public List<byte> getEmptyCellIds()
        {
            List<byte> cellList = new List<byte>();

            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    if (stateTable[i, j] == 0)
                    {
                        cellList.Add((byte)(BOARD_SIZE * i + j));
                    }
                }
            }

            return cellList;
        }

        public int getNumberOfEmptyCells()
        {
            if (cache_EmptyCells == 0)
            {
                cache_EmptyCells = (byte)(getEmptyCellIds().Count);
            }
            return cache_EmptyCells;
        }

        public Boolean hasWon()
        {
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    if (stateTable[i, j] >= 11)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Boolean isGameOver()
        {
            return isGridFull();
        }

        private Boolean isGridFull()
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    if (stateTable[x,y]==0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void setEmptyCell(byte[,] sTable, int i, int j, byte value)
        {
            if (sTable[i, j] == 0)
            {
                sTable[i, j] = value;
                sTable = null;
            }
        }


        public byte[,] addRandomCell(byte[,] sTable)
        {
            List<byte> emptyCells = getEmptyCellIds();

            byte listSize = (byte)(emptyCells.Count);

            if (listSize == 0)
            {
                return null;
            }

            int randomCellId = emptyCells[randomGenerator.Next(listSize)];
            byte randomValue = (byte)((randomGenerator.NextDouble() < 0.9) ? 1 : 2);

            int i = randomCellId / BOARD_SIZE;
            int j = randomCellId % BOARD_SIZE;

            sTable[i, j] = randomValue;

            return sTable;
        }

        public Random randomGenerator = new Random();

        /// <summary>
        /// Calculates the Manhattan distance from a start state to a goal state.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="goalState">The goal state.</param>
        /// <returns>The Manhattan distance calculated.</returns>
        public void write()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(stateTable[i, j] + " ");
                    if (j == 3) Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
