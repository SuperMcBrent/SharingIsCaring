using System;
using System.Collections.Generic;
using System.Text;

namespace Straten {
    class Provincie {
        public int Id { get; set; }
        public string Naam { get; set; }
        public Regio Regio { get; set; }

        public Gemeentes Gemeentes { get; set; }

        public Provincie(string provincieCSV) {
            var values = provincieCSV.Split(';');
            this.Id = int.Parse(values[1]);
            this.Naam = values[3];

            this.Gemeentes = new Gemeentes();
        }
    }

    class Provincies {
        public Provincie[] provincies = new Provincie[0];
        public int Count { get; private set; }

        public Provincie this[int index] {
            get { return provincies[index]; }
        }

        public void Add(Provincie _provincie) {
            Count = Count + 1;
            Array.Resize(ref provincies, Count);
            provincies[(Count - 1)] = _provincie;
        }

        public void Remove(int _index) {
            for (int index = _index; index < provincies.Length - 1; index++) {
                provincies[index] = provincies[index + 1];
            }
            Count = Count - 1;
            Array.Resize(ref provincies, Count);
        }
    }
}
