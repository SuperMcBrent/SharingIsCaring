using System;
using System.Linq;

namespace Mastermind {
    class Program {
        static Board _board;

        static void Main() {
            //GenerateBoard();
            //RenderBoard();
            //EventLoop();
            string code = "ABC";
            string guess = "BBC";
            (int good, int almost) output = Compare(guess, code);
            Console.WriteLine($"Code '{code}' en Guess '{guess}' hebben {output.good} overeenkomsten en {output.almost} letters op de verkeerde plaats");
        }

        static void GenerateBoard() {
            _board = new Board();
        }

        static void RenderBoard() {

        }

        static void EventLoop() {

        }

        static (int good, int almost) Compare(string guess, string code) {
            int good = 0, almost = 0;
            foreach (char guesspiece in guess) {
                if (!code.Contains(guesspiece)) {
                    continue;
                }
                int count = guess.Count(e => e == guesspiece);
                Console.WriteLine($"De letter {guesspiece} komt {count} keer voor in {guess}");
                // checkt enkel de eerste occurence van guesspiece in de code
                if (Array.IndexOf(code.ToCharArray(), guesspiece) == Array.IndexOf(guess.ToCharArray(), guesspiece)) { 
                    good++;
                    continue;
                }

                almost++;
            }
            return (good, almost);
        }
    }
}
