using System;
using System.Linq;
using System.Text;

namespace Mastermind {
    class Program {
        static Board _board;

        static void Main() {
            _board = new Board();
            _board.GenerateCode();
            while (true) {
                string guess = Console.ReadLine();
                (int good, int almost) output = _board.Compare(guess, _board.Code); 
                Console.WriteLine($"De code en jouw guess '{guess}' hebben {output.good} overeenkomsten en {output.almost} letters op de verkeerde plaats");
            }



            //GenerateBoard();
            //RenderBoard();
            //EventLoop();
        }

        static void GenerateBoard() {

        }

        static void RenderBoard() {

        }

        static void EventLoop() {

        }
    }
}
