using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MineSweeper_ConsoleApp {
    class Program {

        #region fields
        static readonly string gameName = "MineSweeper";
        const int _mine = -1;
        static int _height = Console.WindowHeight;
        static int _width = Console.WindowWidth;
        static readonly Random _random = new Random();
        static readonly float _mineRatio = .1f;
        static int _mineCount = (int)(_width * _height * _mineRatio);
        static (int value, bool visible)[,] _board;
        static (int column, int row) _position = (_width / 2, _height / 2);
        static StringBuilder stringBuilder = new StringBuilder(_width * _height);
        static ConsoleKey pressedKey;
        //StringBuilder.Append();

        private enum Gamestate { Win, Lose, Quit, Menu, Playing };
        static Gamestate gamestate = Gamestate.Playing;
        #endregion

        static void Main() {
            Console.CursorSize = 100;
            GenerateBoard();
            RenderBoard();
            EventLoop();
        }

        private static bool IsConsoleResized() {
            if (Console.WindowHeight != _height) {
                System.Diagnostics.Debug.WriteLine($"DEBUG - De hoogte van het venster is gewijzigd van {_height} naar {Console.WindowHeight}");
                return true;
            }
            if (Console.WindowWidth != _width) {
                System.Diagnostics.Debug.WriteLine($"DEBUG - De breedte van het venster is gewijzigd van {_width} naar {Console.WindowWidth}");
                return true;
            }
            return false;
        }

        private static void EventLoop() {
            while (gamestate.Equals(Gamestate.Playing) || gamestate.Equals(Gamestate.Menu)) {

                //checken of er nog veldn zijn die visible false zijn
                // if not, game is gewonnen.
                // gamestate win en break;

                if (gamestate.Equals(Gamestate.Playing)) {
                    if (IsConsoleResized()) {
                        _width = Console.WindowWidth;
                        _height = Console.WindowHeight;
                        _mineCount = (int)(_width * _height * _mineRatio);
                        _position = (_width / 2, _height / 2);
                        stringBuilder.Clear();
                        Console.Clear();
                        System.Diagnostics.Debug.WriteLine($"DEBUG - Generating board with new dimensions w:{_width} h:{_height} with {_mineCount} bombs");
                        GenerateBoard();
                        RenderBoard();
                    }
                    Console.SetCursorPosition(_position.column, _position.row);
                    pressedKey = Console.ReadKey(true).Key;
                    switch (pressedKey) {
                        case ConsoleKey.DownArrow:
                            if (_position.row < _height - 1) {
                                _position.row++;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (_position.row > 0) {
                                _position.row--;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (_position.column > 0) {
                                _position.column--;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (_position.column < _width - 1) {
                                _position.column++;
                            }
                            break;
                        case ConsoleKey.Enter:
                            if (!_board[_position.column, _position.row].value.Equals(_mine)) {
                                Reveal(_position.column, _position.row);
                            } else {
                                for (int x = 0; x < _width; x++) {
                                    for (int y = 0; y < _height; y++) {
                                        _board[x, y].visible = true;
                                    }
                                }
                                RenderBoard();
                                //gamestate = Gamestate.Lose;
                                //Music();
                                Console.Beep();
                                Console.Beep();
                                Console.Beep();
                            }
                            // else game = finished
                            break;
                        case ConsoleKey.Escape:
                            gamestate = Gamestate.Menu;
                            break;
                        default:
                            break;
                    }
                } else if (gamestate == Gamestate.Menu) {
                    Console.Clear();
                    Console.CursorVisible = false;
                    Console.SetCursorPosition((_width / 2) - (gameName.Length / 2), (_height / 2) - 5);
                    Console.Write(gameName);    
                    List<string> menuItems = new List<string>() { "Restart", "Easy", "Quit" };
                    List<string> difficulties = new List<string>() { "Easy", "Medium", "Hard" };
                    for (int i = 0; i < menuItems.Count; i++) {
                        Console.SetCursorPosition((_width / 2) - (menuItems[i].Length / 2), (_height / 2) - 2 + (2 * i));
                        Console.Write(menuItems[i]);
                    }
                    pressedKey = Console.ReadKey(true).Key;
                    switch (pressedKey) {
                        case ConsoleKey.Escape:
                            gamestate = Gamestate.Playing;
                            Console.Clear();
                            Console.SetCursorPosition(0, 0);
                            Console.CursorVisible = true;
                            RenderBoard();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void RenderBoard() {
            stringBuilder.Clear();
            for (int row = 0; row < _height; row++) {
                for (int column = 0; column < _width; column++) {
                    //wtf
                    if (_board[column, row].visible == false) {
                        stringBuilder.Append("█");
                    } else if (_board[column, row].value.Equals(0)) {
                        stringBuilder.Append(" ");
                    } else if (_board[column, row].value.Equals(_mine)) {
                        stringBuilder.Append("■");
                    } else {
                        stringBuilder.Append(_board[column, row].value);
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(stringBuilder.ToString());
        }

        private static void GenerateBoard() {
            //het bord: een array van int/bool tuples
            _board = new (int value, bool visible)[_width, _height];
            var coordinates = new List<(int Row, int Column)>();
            //lijst van coordinaten/velden aanmaken
            for (int column = 0; column < _width; column++) {
                for (int row = 0; row < _height; row++) {
                    coordinates.Add((column, row));
                }
            }
            //Console.WriteLine($"Minecount: {_mineCount} van de {_width * _height}");
            for (int i = 0; i < _mineCount; i++) {
                //op dit coordinaat wordt een mijn geplaatst...
                int randomIndex = _random.Next(0, coordinates.Count);
                (int Column, int Row) = coordinates[randomIndex];
                coordinates.RemoveAt(randomIndex);
                // mijn plaatsen en visible is false; cen onzichtbaar bij opstarten...
                _board[Column, Row] = (_mine, false);

                for (int x = -1; x < 2; x++) {
                    for (int y = -1; y < 2; y++) {
                        if (x + Column < _width && y + Row < _height && x + Column >= 0 && y + Row >= 0) {
                            //Console.WriteLine(_board[Column, Row].value);
                            if (_board[Column + x, Row + y].value != _mine) {
                                //Console.WriteLine($"Vakje {Column + x},{Row + y} krijgt +1 omdat het naast een bom {Column},{Row} ligt (totaal: {_board[Column + x, Row + y].value + 1})");
                                _board[Column + x, Row + y].value++;
                            }
                        }
                    }
                }
                //Console.WriteLine("----- NEXT -----");
            }
        }

        private static int SideQuestSum(int min, int val) {
            if (val == min) {
                return val;
            }
            return val + SideQuestSum(min, val - 1);
        }

        ////while(true) {};
        //private static IEnumerable<(int Row, int Column)>
        //private static char Render(int value);

        private static void Reveal(int column, int row) {
            Console.CursorVisible = false;
            if (_board[column, row].value == 0) {
                for (int x = -1; x < 2; x++) {
                    for (int y = -1; y < 2; y++) {
                        if (x + column < _width && y + row < _height && x + column >= 0 && y + row >= 0) {
                            if (!_board[column + x, row + y].value.Equals(_mine)) {
                                if (!_board[column + x, row + y].visible) {
                                    _board[column + x, row + y].visible = true;
                                    Console.SetCursorPosition(column + x, row + y);
                                    Console.Write(_board[column + x, row + y].value.Equals(0) ? " " : _board[column + x, row + y].value.ToString());
                                    if (_board[column + x, row + y].value == 0) {
                                        Reveal(column + x, row + y);
                                    }
                                }
                            }
                        }
                    }
                }
            } else if (!_board[column, row].visible) {
                _board[column, row].visible = true;
                Console.SetCursorPosition(column, row);
                Console.Write(_board[column, row].value);
            }
            Console.CursorVisible = true;
        }

        private static void Music() {
            Console.Beep(659, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(523, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(375); Console.Beep(392, 125); Thread.Sleep(375); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(375); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(698, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(698, 125); Thread.Sleep(625); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(622, 125); Thread.Sleep(250); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(523, 125); Thread.Sleep(1125); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(698, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(698, 125); Thread.Sleep(625); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(622, 125); Thread.Sleep(250); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(523, 125);
        }
    }
}

/*
  __  __ _          ___                              
 |  \/  (_)_ _  ___/ __|_ __ _____ ___ _ __  ___ _ _ 
 | |\/| | | ' \/ -_)__ \ V  V / -_) -_) '_ \/ -_) '_|
 |_|  |_|_|_||_\___|___/\_/\_/\___\___| .__/\___|_|  
                                      |_|            
*/
