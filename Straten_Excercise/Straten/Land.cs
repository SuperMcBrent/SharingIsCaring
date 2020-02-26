using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Straten {
    [Serializable]
    class Land {

        public int Id { get; set; }
        public string Naam { get; set; }
        public string TaalCode { get; set; }

        readonly string path = @".\";

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
                var line = reader.ReadLine();
                var values = line.Split(';');
                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    continue;
                }
                if (!provincies.Exists(int.Parse(values[1]), values[2])) {
                    provincies.Add(new Provincie(line, this.Regios[0]));
                }
                Provincie huidigeProvincie = provincies.Get(int.Parse(values[1]));
                if (huidigeProvincie.Gemeentes.Exists(int.Parse(values[0]), values[2])) {
                    continue;
                }
                huidigeProvincie.Gemeentes.Add(new Gemeente(int.Parse(values[0]), values[2], huidigeProvincie));
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
                        gemeente.Straten.Add(new Straat(item, gemeente));
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

        /// <summary>
        /// not 100% functional, for better results use upgraded non-selectve ReadAll()
        /// </summary>
        /// <param name="targetGemeente"></param>
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
            Console.WriteLine("Vorige versie opkuisen...");
            RecursiveDelete(Path.Combine(path, this.Naam));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            DirectoryInfo rootFolder = DirectoryTools.CreateDir(path, this.Naam);
            foreach (var regio in this.Regios.regios) {
                _ = DirectoryTools.CreateDir(rootFolder.FullName, regio.Naam);
                foreach (var provincie in regio.Provincies.provincies) {
                    string temp = Path.Combine(rootFolder.FullName, provincie.Regio.Naam);
                    _ = DirectoryTools.CreateDir(temp, provincie.Naam);
                    foreach (var gemeente in provincie.Gemeentes.gemeentes) {
                        if (String.IsNullOrEmpty(gemeente.Naam)) {
                            continue;
                        }
                        temp = Path.Combine(rootFolder.FullName, gemeente.Provincie.Regio.Naam, gemeente.Provincie.Naam);
                        _ = DirectoryTools.CreateDir(temp, gemeente.Naam);

                        PrintToekenningen(temp, gemeente.Naam);

                        string filenaam = Path.Combine(temp,gemeente.Naam, gemeente.Naam + "_Straten.txt");
                        FileInfo file = new FileInfo(Path.Combine(filenaam));
                        using StreamWriter sw = file.AppendText();
                        foreach (var straat in gemeente.Straten.straten) {
                            sw.WriteLine(straat.Naam);
                        }
                    }
                }
            }
            Console.WriteLine("Folders OK");
        }

        [Conditional("DEBUG")]
        protected void PrintToekenningen(string path, string gemeente) {
            Console.WriteLine(path + " <-- " + gemeente);
        }
        // bij deze methode moet alles apart geencapsuleerd worden
        // #if DEBUG
        // #endif
        // Dit werkt ook
        // #if DEBUG
        //         public const String value = "iets voor te debuggen";
        // #else
        //         public const String value = "iets anders voor de release versie";
        // #endif
        public void MakeBLOB() {
            string filename = "Straten.bin";
            RecursiveDelete(Path.Combine(path, filename));
            IFormatter f = new BinaryFormatter();
            Stream s = new FileStream(Path.Combine(path,filename), FileMode.Create, FileAccess.Write);
            f.Serialize(s, this);
            Console.WriteLine("Write OK");
            s.Close();
        }

        public void LoadBLOB() {
            string filename = "Straten.bin";
            IFormatter f = new BinaryFormatter();
            Stream s = new FileStream(Path.Combine(path, filename), FileMode.Open, FileAccess.Read);

            Land loadedLand = (Land)f.Deserialize(s);
            this.Regios = loadedLand.Regios;
            Console.WriteLine("Read OK");
            s.Close();
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
