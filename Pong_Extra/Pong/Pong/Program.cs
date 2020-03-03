using System;

namespace Pong {
    class Program {
        static readonly string gameName = "Pong";
        static int _windowHeigth;
        static int _windowWidth;
        static ConsoleKey pressedKey;
        static int _palateSize;
        static readonly float difficulty = .2f;
        static int _leftPalateTop;
        static int _rightPalateTop;
        static (int top, int left) _ballPosition;
        static (int xPart, int yPart) _ballVector;

        // de while eventloop loop zo snel als die kan
        // ballspeed variable maken en update van pixel pas toelaten als er bv 20-50 millis gepasseerd zijn

        public static void Main(string[] args) {
            Setup();
            while (true) {
                EventLoop();
            }
        }

        private static void Setup() {
            Console.Clear();
            Console.CursorVisible = false;
            Console.Title = gameName;
            _windowHeigth = Console.WindowHeight;
            _windowWidth = Console.WindowWidth;
            _palateSize = (int)Math.Floor(_windowHeigth * difficulty);
            _leftPalateTop = 0;
            _rightPalateTop = 0;
            _ballPosition.left = (int)Math.Floor(_windowWidth / 2.0);
            _ballPosition.top = (int)Math.Floor(_windowHeigth / 2.0);
        }

        private static void EventLoop() {
            if (IsConsoleResized()) {
                Setup();
            }

            ClearPalates();
            DrawPalates();
            DrawBall();

            pressedKey = Console.ReadKey(true).Key;
            switch (pressedKey) {
                case ConsoleKey.Q:
                    if (_leftPalateTop > 0) {
                        _leftPalateTop--;
                    }
                    break;
                case ConsoleKey.A:
                    if (_leftPalateTop + _palateSize < _windowHeigth) {
                        _leftPalateTop++;
                    }
                    break;
                case ConsoleKey.NumPad9:
                    if (_rightPalateTop > 0) {
                        _rightPalateTop--;
                    }

                    break;
                case ConsoleKey.NumPad6:
                    if (_rightPalateTop + _palateSize < _windowHeigth) {
                        _rightPalateTop++;
                    }
                    break;
                default:
                    break;
            }
        }

        private static bool IsConsoleResized() {
            if (Console.WindowHeight != _windowHeigth) {
                System.Diagnostics.Debug.WriteLine($"DEBUG - De hoogte van het venster is gewijzigd van {_windowHeigth} naar {Console.WindowHeight}");
                return true;
            }
            if (Console.WindowWidth != _windowWidth) {
                System.Diagnostics.Debug.WriteLine($"DEBUG - De breedte van het venster is gewijzigd van {_windowWidth} naar {Console.WindowWidth}");
                return true;
            }
            return false;
        }

        private static void DrawPalates() {
            for (int i = 0; i < _palateSize; i++) {
                Console.SetCursorPosition(0, _leftPalateTop + i);
                Console.Write("█");
                Console.SetCursorPosition(_windowWidth - 1, _rightPalateTop + i);
                Console.Write("█");
            }
        }

        private static void ClearPalates() {
            //enkel de kolommen van de palates clearen, console.clear te traag
            for (int i = 0; i < _windowHeigth; i++) {
                Console.SetCursorPosition(0, i);
                Console.Write(" ");
                Console.SetCursorPosition(_windowWidth - 1, i);
                Console.Write(" ");
            }
        }

        public static void DrawBall() {
            ClearBall();

            Console.SetCursorPosition(_ballPosition.left, _ballPosition.top);
            Console.Write("██");
        }

        public static void ClearBall() {
            //alle mogelijke vorige posities van 'de pixel' opkuisen
            for (int i = -2; i < 4; i++) {
                for (int j = -1; j < 2; j++) {
                    if (_ballPosition.left + i < 0 || _ballPosition.left + i > _windowWidth) {
                        continue;
                    }
                    if (_ballPosition.top + j < 0 || _ballPosition.top + j > _windowHeigth) {
                        continue;
                    }
                    Console.SetCursorPosition(_ballPosition.left + i, _ballPosition.top + j);
                    Console.Write(" ");
                }
            }
        }

        public static void UpdateBallPos() {
            // botsen met rand toevoegen

            // botsen = vector updaten

            _ballPosition.top += _ballVector.yPart;
            _ballPosition.left += _ballVector.xPart;
        }
    }
}
