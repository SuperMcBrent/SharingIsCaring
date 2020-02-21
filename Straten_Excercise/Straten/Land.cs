using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly string provincieIDsVlaanderen = Path.Combine(repo, "ProvincieIDsVlaanderen.csv");
        private readonly string provincieInfo = Path.Combine(repo, "ProvincieInfo.csv");
        private readonly string straatnaamID_gemeenteID = Path.Combine(repo, "StraatnaamID_gemeenteID.csv");
        private readonly string WRGemeentenaam = Path.Combine(repo, "WRGemeentenaam.csv");
        private readonly string WRstraatnamen = Path.Combine(repo, "WRstraatnamen.csv");

        public void Read(string targetGemeente) {
            StreamReader reader;
            Gemeente gevondenGemeente = null;
            Provincie gevondenProvincie = null;
            Straat gevondenStraat = null;
            // READ GEMEENTEDATA
            reader = new StreamReader(WRGemeentenaam);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (values[2].Equals(this.TaalCode) && values[3].Equals(targetGemeente)) {
                    //Console.WriteLine("FOUND: " + line);
                    gevondenGemeente = new Gemeente(line);
                    //Console.WriteLine(gevondenGemeente.Naam);
                }
            }
            reader.Close();
            // READ PROVINCIEDATA
            reader = new StreamReader(provincieInfo);
            //Console.WriteLine($"Ik zoek naar: " + gevondenGemeente.Id);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    //Console.WriteLine("Skipping...");
                    continue;
                }
                //Console.WriteLine(values[0]);
                if (values[2].Equals(this.TaalCode) && gevondenGemeente.Id == int.Parse(values[0])) {
                    //Console.WriteLine("FOUND: " + line);
                    gevondenProvincie = new Provincie(line);
                    //Console.WriteLine(gevondenProvincie.Naam);

                    gevondenProvincie.Gemeentes.Add(gevondenGemeente);
                    this.Regios[0].Provincies.Add(gevondenProvincie);
                }
            }
            reader.Close();
            // READ STREETID DATA
            reader = new StreamReader(straatnaamID_gemeenteID);
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(';');

                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    //Console.WriteLine("Skipping...");
                    continue;
                }

                if (int.Parse(values[1]).Equals(gevondenGemeente.Id)) {
                    //Console.WriteLine("FOUND: " + line);
                    gevondenStraat = new Straat(line);
                    //Console.WriteLine(gevondenStraat.Id);

                    this.Regios[0].Provincies[0].Gemeentes[0].Straten.Add(gevondenStraat);
                }
            }
            Console.WriteLine(this.Naam + ", " + this.Regios[0].Naam + ", " +
                this.Regios[0].Provincies[0].Naam + ", " +
                this.Regios[0].Provincies[0].Gemeentes[0].Naam + " heeft " +
                this.Regios[0].Provincies[0].Gemeentes[0].Straten.Count +
                " straten geregistreerd.");
            reader.Close();

            // READ STREET NAMES
            reader = new StreamReader(WRstraatnamen);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine().TrimEnd();
                var values = line.Split(';');
                //Console.WriteLine(values[1].TrimEnd().Length);

                if (!String.IsNullOrEmpty(line) && Char.IsLetter(line[0])) {
                    //Console.WriteLine("Skipping...");
                    continue;
                }

                if (this.Regios[0].Provincies[0].Gemeentes[0].Straten.Exists(int.Parse(values[0]))) {
                    //Console.WriteLine(values[1] + " toekennen aan staatID: " + (int.Parse(values[0])));
                    this.Regios[0].Provincies[0].Gemeentes[0].Straten.KenNaamToe(int.Parse(values[0]), values[1]);
                }
            }
            reader.Close();

            // PRINT STREET NAMES FULL INFO
            //int teller = 0;
            //foreach (Straat item in this.Regios[0].Provincies[0].Gemeentes[0].Straten.straten) {
            //    //Console.WriteLine("teller: " + teller);
            //    //teller++;
            //    Console.WriteLine(" " + item.Naam + " " + this.Regios[0].Provincies[0].Gemeentes[0].Naam);
            //    //Console.WriteLine(item.Naam);
            //    //Console.WriteLine(this.Regios[0].Provincies[0].Gemeentes[0].Naam);
            //}
        }

        public void Persist() {
            // folderstructuur genereren voor opslag.
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
