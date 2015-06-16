using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace AIAssignment1.G2048
{
    public class G2048
    {
        private StateOf2048 initState;

        public enum SolvingMethod { DFS, BFS, HillCliming };

        public StateOf2048 InitState { get { return initState; } }

        public static char RowSeparator { get; set; }

        public static char ColSeparator { get; set; }

        public void SetInitState(string state)
        {
            initState = new StateOf2048(state);
        }


        public IEnumerable<StateOf2048> Solve(SolvingMethod method)
        {
            if (initState == null)
            {
                Console.Write("\nPlease set initial state before solving the puzzle.");
                return null;
            }

            switch (method)
            {
                case SolvingMethod.DFS:
                    return depthFirstSearch();
                case SolvingMethod.BFS:
                    return breadthFirstSearch();
                case SolvingMethod.HillCliming:
                    return HillCliming();
                default:
                    return null;
            }
        }



        public IEnumerable<StateOf2048> traceBack(StateOf2048 startState, Dictionary<string, StateOf2048> tracer)
        {
            while (startState != null)
            {
                yield return startState;
                startState = tracer[startState.State];
            }
        }

        private IEnumerable<StateOf2048> depthFirstSearch()
        {
            var stack = new Stack<StateOf2048>();
            var tracer = new Dictionary<string, StateOf2048>();
            var generatedStates = new HashSet<string>();
            stack.Push(initState);
            tracer.Add(initState.State, null);
            generatedStates.Add(initState.State);

            while (stack.Any())
            {
                var state = stack.Pop();
                //state.write();

                if (state.hasWon())
                {
                    state.write();
                    return traceBack(state, tracer).Reverse();
                }
                else if (!state.isGameOver())
                {
                    var nextStates = state.GetNextStates();
                    foreach (var s in nextStates)
                    {
                        if ((s.State) != null && !generatedStates.Contains(s.State))
                        {
                            tracer.Add(s.State, state);
                            stack.Push(s);
                            generatedStates.Add(s.State);
                        }
                        else continue;
                    }
                }
                else continue;
            }
            return null;
        }

        public IEnumerable<StateOf2048> breadthFirstSearch()
        {
            var queue = new Queue<StateOf2048>();
            var tracer = new Dictionary<string, StateOf2048>();
            var generatedStates = new HashSet<string>();
            queue.Enqueue(initState);
            tracer.Add(initState.State, null);
            generatedStates.Add(initState.State);

            while (queue.Any())
            {
                var state = queue.Dequeue();
                //state.write();
                if (state.hasWon())
                {
                    state.write();
                    return traceBack(state, tracer).Reverse();
                }
                else if (!state.isGameOver())
                {
                    var nextStates = state.GetNextStates();
                    foreach (var s in nextStates)
                    {
                        if (!generatedStates.Contains(s.State) && s.State != null)
                        {
                            tracer.Add(s.State, state);
                            queue.Enqueue(s);
                            generatedStates.Add(s.State);
                        }
                        else continue;
                    }
                }
                else continue;
            }
            return null;
        }

        private IEnumerable<StateOf2048> HillCliming()
        {
            var stack = new Stack<StateOf2048>();
            var tracer = new Dictionary<string, StateOf2048>();
            var generatedStates = new HashSet<string>();
            stack.Push(initState);
            tracer.Add(initState.State, null);
            generatedStates.Add(initState.State);

            while (stack.Any())
            {
                var state = stack.Pop();
                //state.write();

                if (state.hasWon())
                {
                    state.write();
                    return traceBack(state, tracer).Reverse();
                }
                else if (!state.isGameOver())
                {
                    var nextStates = state.GetNextStates();
                    StateOf2048 xxx= state;
                    foreach (var s in nextStates)
                    {
                        if ((s.State) != null && !generatedStates.Contains(s.State))
                        {
                            if (s.diem >= xxx.diem)
                            {
                                xxx = s;
                                tracer.Add(xxx.State, state);
                                stack.Push(xxx);
                                generatedStates.Add(xxx.State);
                                continue;
                            }

                        }
                        else continue;

                    }
                }
               else continue;
            }
            return null;
        }


        private static string getMethodName(SolvingMethod method)
        {
            switch (method)
            {
                case SolvingMethod.BFS:
                    return "Breadth First Search";
                case SolvingMethod.DFS:
                    return "Depth First Search";
                case SolvingMethod.HillCliming:
                    return "HillClimbing";
                default:
                    return "";
            }
        }


        /// <summary>
        /// Runs the test in specified input file and write the result to specified output file.
        /// </summary>
        /// <param name="inputFile">The input file contains the test.</param>
        /// <param name="outputFile">The output file to write result to.</param>
        public static void RunTest(string inputFile, string outputFile, int m, bool openOutput)
        {
            if (!File.Exists(inputFile))
            {
                Console.Write("Input file does not exist.");
            }

            try
            {
                StreamReader reader = new StreamReader(inputFile);
                string initState = reader.ReadLine();
                reader.Close();
                SolvingMethod method = ((SolvingMethod)m);

                Console.Write(string.Format("\nSolving G2048 using {0}...\n", G2048.getMethodName(method)));
                var puzzle = new G2048();
                puzzle.SetInitState(initState);

                FileStream output = new FileStream(outputFile, FileMode.Create);
                StreamWriter writer = new StreamWriter(output);

                Stopwatch timer = new Stopwatch();
                timer.Start();
                var result = puzzle.Solve((SolvingMethod)m);
                timer.Stop();

                if (result != null)
                {
                    foreach (var state in result)
                    {
                        writer.WriteLine(state.State);
                        writer.WriteLine();
                    }
                }
                else writer.Write("Not solvable!");
                writer.Close();

                Console.Write("\nDone!");
                Console.WriteLine(string.Format("\nTime: {0}ms\nMemory: {1}MB", timer.ElapsedMilliseconds, ((float)Process.GetCurrentProcess().WorkingSet64 / 1024) / 1024));

                if (openOutput)
                {
                    Console.Write("Press enter to view results...");
                    while (Console.ReadKey().Key != ConsoleKey.Enter) ;
                    Process.Start(outputFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.Write(ex.Message);
                Console.Read();
            }
        }

        public static void RunAll(int m)
        {
            const string input = "Inputs\\G2048\\g2048_test{0}.txt";
            const string output = "Outputs\\G2048\\g2048_output{0}.txt";

            Directory.CreateDirectory("Outputs");

            for (int i = 1; i <= 10; i++)
            {
                try
                {
                    G2048.RunTest(string.Format(input, i), string.Format(output, i), m, false);
                }
                catch { continue; }
            }
            Console.Read();
        }
    }
}
