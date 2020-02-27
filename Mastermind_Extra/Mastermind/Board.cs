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
    }
}
