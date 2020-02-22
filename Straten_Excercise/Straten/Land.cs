using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Straten {
    class Land {

        public int Id { get; set; }
        public string Naam { get; set; }
        public string TaalCode { get; set; }

        public Regios Regios { get; set; }

        public Land(int id, string naam, string taalcode) {
            this.Id = id;
            this.Naam = naam;
            this.TaalCode = taalcode;
            this.Regios = new Regios();
        }

        private const string repo = @"C:\School\SharingIsCaring\Straten_Excercise\Straten\repository";

        private readonly string provincieInfo = Path.Combine(repo, "ProvincieInfo.csv");
        private readonly string straatnaamID_gemeenteID = Path.Combine(repo, "StraatnaamID_gemeenteID.csv");
        private readonly string WRGemeentenaam = Path.Combine(repo, "WRGemeentenaam.csv");
        private readonly string WRstraatnamen = Path.Combine(repo, "WRstraatnamen.csv");

        public void ReadAll() {
            long totalTimeElapsed = 0;
            StreamReader reader;
            // READ PROVINCIEDATA
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            reader = new StreamReader(provincieInfo);
            var provincies = this.Regios[0].Provincies;
            while (!reader.EndOfStream) {
                // leest enkel nl in, geen fr
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                if (!provincies.Exists(int.Parse(values[1]))) { // hier taalcode aan toevoegen
                    provincies.Add(new Provincie(line, this.Regios[0]));
                }
                Provincie huidigeProvincie = provincies.Get(int.Parse(values[1]));
                if (huidigeProvincie.Gemeentes.Exists(int.Parse(values[0]),values[2])) {
                    continue;
                }
                huidigeProvincie.Gemeentes.Add(new Gemeente(int.Parse(values[0]),values[2]));
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading provinciedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            // READ GEMEENTEDATA
            stopWatch.Start();
            reader = new StreamReader(WRGemeentenaam);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                foreach (var provincie in provincies.provincies) {
                    foreach (var gemeente in provincie.Gemeentes.gemeentes) {
                        if (gemeente.Id.Equals(int.Parse(values[1])) && gemeente.Taalcode.Equals(values[2])) {
                            gemeente.Naam = values[3];
                            gemeente.NaamId = int.Parse(values[0]);
                            //Console.WriteLine($"Gemeente {gemeente.Naam}({gemeente.Taalcode}) toegevoegd aan {provincie.Naam}.");
                        }
                    }
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading gemeentedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            // READ STRAATDATA
            stopWatch.Start();
            Dictionary<int, int> stratenengemeenten = new Dictionary<int, int>();
            reader = new StreamReader(straatnaamID_gemeenteID);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                stratenengemeenten.Add(int.Parse(values[0]), int.Parse(values[1]));
            }
            foreach (var provincie in provincies.provincies) {
                foreach (var gemeente in provincie.Gemeentes.gemeentes) {
                    var matches = stratenengemeenten.Where(pair => pair.Value == gemeente.Id).Select(pair => pair.Key);
                    foreach (var item in matches) {
                        gemeente.Straten.Add(new Straat(item));
                    }
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading stratenID data took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            stopWatch.Start();
            Dictionary<int, string> straten = new Dictionary<int, string>();
            reader = new StreamReader(WRstraatnamen);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                straten.Add(int.Parse(values[0]), values[1].Trim());
            }
            int teller = 1;
            foreach (var provincie in provincies.provincies) {
                //Console.WriteLine($"Provincie {teller} van {provincies.Count} verwerken...");
                teller++;
                foreach (var gemeente in provincie.Gemeentes.gemeentes) {
                    foreach (var straat in gemeente.Straten.straten) {
                        string name;
                        if (straten.TryGetValue(straat.Id, out name)) {
                            straat.Naam = name; ;
                        }
                    }
                }
            }
            stopWatch.Stop();
            Console.WriteLine($"Reading gemeentedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"Total time elpased: {totalTimeElapsed} millis.");
            stopWatch.Reset();
        }

        public void Read(string targetGemeente) {
            long totalTimeElapsed = 0;
            StreamReader reader;
            Gemeente gevondenGemeente = null;
            Provincie gevondenProvincie = null;
            Straat gevondenStraat = null;
            // READ GEMEENTEDATA
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            reader = new StreamReader(WRGemeentenaam);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (values[2].Equals(this.TaalCode) && values[3].Equals(targetGemeente)) {
                    gevondenGemeente = new Gemeente(line);
                }
            }
            reader.Close();
            stopWatch.Stop();
            Console.WriteLine($"Reading gemeentedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            // READ PROVINCIEDATA
            stopWatch.Start();
            reader = new StreamReader(provincieInfo);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                if (values[2].Equals(this.TaalCode) && gevondenGemeente.Id == int.Parse(values[0])) {
                    gevondenProvincie = new Provincie(line);
                    gevondenProvincie.Gemeentes.Add(gevondenGemeente);
                    this.Regios[0].Provincies.Add(gevondenProvincie);
                }
            }
            reader.Close();
            stopWatch.Stop();
            Console.WriteLine($"Reading provinciedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            // READ STREETID DATA
            stopWatch.Start();
            reader = new StreamReader(straatnaamID_gemeenteID);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }

                if (int.Parse(values[1]).Equals(gevondenGemeente.Id)) {
                    gevondenStraat = new Straat(line);
                    this.Regios[0].Provincies[0].Gemeentes[0].Straten.Add(gevondenStraat);
                }
            }
            Console.WriteLine(this.Naam + ", " + this.Regios[0].Naam + ", " +
                this.Regios[0].Provincies[0].Naam + ", " +
                this.Regios[0].Provincies[0].Gemeentes[0].Naam + " heeft " +
                this.Regios[0].Provincies[0].Gemeentes[0].Straten.Count +
                " straten geregistreerd.");
            reader.Close();
            stopWatch.Stop();
            Console.WriteLine($"Reading streetiddata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            stopWatch.Reset();
            // READ STREETNAMEDATA
            stopWatch.Start();
            reader = new StreamReader(WRstraatnamen);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine().TrimEnd();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }

                if (this.Regios[0].Provincies[0].Gemeentes[0].Straten.Exists(int.Parse(values[0]))) {
                    this.Regios[0].Provincies[0].Gemeentes[0].Straten.KenNaamToe(int.Parse(values[0]), values[1]);
                }
            }
            reader.Close();
            stopWatch.Stop();
            Console.WriteLine($"Reading streetnamedata took {stopWatch.ElapsedMilliseconds} millis.");
            totalTimeElapsed += stopWatch.ElapsedMilliseconds;
            Console.WriteLine($"Total time elpased: {totalTimeElapsed} millis.");
            stopWatch.Reset();
        }

        public void Persist() {
            string root = @".\";

            Console.WriteLine("Vorige versie opkuisen...");
            RecursiveDelete(Path.Combine(root, this.Naam));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("creating dir " + this.Naam);
            var land = Path.Combine(root, this.Naam);
            if (!Directory.Exists(land)) {
                Directory.CreateDirectory(land);
                Console.WriteLine($"De map {this.Naam} werd successvol aangemaakt.");
                root = land;
            }

            Console.WriteLine("path: " + root);

            var regios = this.Regios.regios;
            root = Path.Combine(root, regios[0].Naam);
            foreach (var item in regios) {
                Console.WriteLine("creating dir " + item.Naam);
                if (!Directory.Exists(root)) {
                    Directory.CreateDirectory(root);
                    Console.WriteLine($"De map {item.Naam} werd successvol aangemaakt.");
                }
            }

            Console.WriteLine("path: " + root);

            var provincies = this.Regios[0].Provincies.provincies;
            root = Path.Combine(root, provincies[0].Naam);
            foreach (var item in provincies) {
                Console.WriteLine("creating dir " + item.Naam);
                if (!Directory.Exists(root)) {
                    Directory.CreateDirectory(root);
                    Console.WriteLine($"De map {item.Naam} werd successvol aangemaakt.");
                }
            }

            Console.WriteLine("path: " + root);

            var gemeentes = this.Regios[0].Provincies[0].Gemeentes.gemeentes;
            root = Path.Combine(root, gemeentes[0].Naam);
            foreach (var item in gemeentes) {
                Console.WriteLine("creating dir " + item.Naam);
                if (!Directory.Exists(root)) {
                    Directory.CreateDirectory(root);
                    Console.WriteLine($"De map {item.Naam} werd successvol aangemaakt.");
                }
            }

            Console.WriteLine("path: " + root);

            var straten = this.Regios[0].Provincies[0].Gemeentes[0].Straten.straten;
            string filenaam = this.Regios[0].Provincies[0].Gemeentes[0].Naam + "_Straten.txt";
            FileInfo file = new FileInfo(Path.Combine(root, filenaam));

            using StreamWriter sw = file.AppendText();
            Console.WriteLine("Straten file aangemaakt.");
            foreach (var item in straten) {
                sw.WriteLine(item.Naam);
            }


        }

        public static void RecursiveDelete(string folderpath) {
            DirectoryInfo baseDir = new DirectoryInfo(folderpath);
            if (!baseDir.Exists) return;
            foreach (var dir in baseDir.EnumerateDirectories()) {
                RecursiveDelete(dir.FullName);
            }
            baseDir.Delete(true);
        }
    }
}
