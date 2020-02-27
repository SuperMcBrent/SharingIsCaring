using System;
using System.Collections.Generic;
using System.Text;

namespace Mastermind {
    class Board {
        public string Code { get; set; }

        public Board() {
            this.Code = GenerateCode();
        }

        public string GenerateCode() {
            string code = "";
            Random r = new Random();
            for (int i = 0; i < Config.CodeLength; i++) {
                code += ((char)r.Next(Config.AsciiValueA, Config.AsciiValueA + Config.Difficulty)).ToString();
            }
            return code;
        }

        public (int good, int almost) Compare(string guess, string code) {
            StringBuilder guessString = new StringBuilder(guess);
            StringBuilder codeString = new StringBuilder(code);
            int good = 0, almost = 0;
            if (!guess.Length.Equals(code.Length)) {
                return (0, 0);
            }
            // rechttegenoverstaande gelijken tellen en eruithalen om double hits tegen te gaan
            for (int i = 0; i < code.Length; i++) {
                if (codeString[i].Equals(guessString[i])) {
                    guessString[i] = '_';
                    codeString[i] = '_';
                    good++;
                    continue;
                }
            }
            // verkeerde plaatsjes tellen en eruithalen om double hits tegen te gaan
            for (int i = 0; i < code.Length; i++) {
                if (codeString[i].Equals('_')) {
                    continue;
                }
                if (guessString.ToString().Contains(codeString[i])) {
                    var a = Array.IndexOf(guessString.ToString().ToCharArray(), codeString[i]);
                    guessString[a] = '_';
                    codeString[i] = '_';
                    almost++;
                }
            }
            return (good, almost);
        }
    }
}
