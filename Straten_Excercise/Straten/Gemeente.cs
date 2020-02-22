using System;
using System.Collections.Generic;
using System.Text;

namespace Straten {
    class Gemeente {

        public int NaamId { get; set; }
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Taalcode { get; set; }
        public Provincie Provincie { get; set; }

        public Straten Straten { get; set; }

        public Gemeente(int naamID, int ID, string taalcode, string naam) {
            this.NaamId = naamID;
            this.Id = ID;
            this.Taalcode = taalcode;
            this.Naam = naam;
        }

        public Gemeente(int id, string taalcode, Provincie provincie) {
            this.Id = id;
            this.Taalcode = taalcode;
            this.Provincie = provincie;

            this.Straten = new Straten();
        }

        public Gemeente(string gemeenteCSV) {
            var values = gemeenteCSV.Split(';');
            this.NaamId = int.Parse(values[0]);
            this.Id = int.Parse(values[1]);
            this.Taalcode = values[2];
            this.Naam = values[3];

            this.Straten = new Straten();
        }

    }

    class Gemeentes {
        public Gemeente[] gemeentes = new Gemeente[0];
        public int Count { get; private set; }

        public Gemeente this[int index] {
            get { return gemeentes[index]; }
        }

        public void Add(Gemeente _gemeente) {
            Count = Count + 1;
            Array.Resize(ref gemeentes, Count);
            gemeentes[(Count - 1)] = _gemeente;
        }

        public bool Exists(int gemeenteId, string taalcode) {
            return Array.Exists(gemeentes, element => element.Id.Equals(gemeenteId) && element.Taalcode.Equals(taalcode));
        }

        public void Remove(int _index) {
            for (int index = _index; index < gemeentes.Length - 1; index++) {
                gemeentes[index] = gemeentes[index + 1];
            }
            Count = Count - 1;
            Array.Resize(ref gemeentes, Count);
        }
    }
}
