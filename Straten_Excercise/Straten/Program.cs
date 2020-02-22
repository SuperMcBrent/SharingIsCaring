using System;

// TODO: take taalcode into account, DEBUG test to disable lines of code, command line arg to use csv or bin; evaluate use of SortedList? Speedup? StringBuilder, Linq ...

namespace Straten {
    class Program {
        static int Main(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("Usage: Straten.exe <operation> <nl|fr> <city>");
                return 1;
            }

            int operation = int.Parse(args[0]);
            string taalCode = args[1];
            string gemeente = args[2];

            Land land = new Land(1, "Belgie", taalCode);
            var regio = new Regio(1, "Vlaanderen", land);
            land.Regios.Add(regio);

            switch (operation) {
                case 1:
                    //land.Read(gemeente);
                    land.ReadAll();
                    land.Persist();
                    land.MakeBLOB();
                    break;

                default:
                case 2:
                    land.LoadBLOB();
                    land.Persist();
                    break;

            }

            //Exporters.FileExporter fileExporter = new Exporters.FileExporter(land);
            //fileExporter.Export(gemeente);
            /*
            Exporters.ConsoleExporter consoleExporter = new Exporters.ConsoleExporter(land);
            consoleExporter.Export(gemeente);
            */

            return 0;
        }
    }
}