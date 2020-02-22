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

        public Provincie(string provincieCSV, Regio regio) {
            var values = provincieCSV.Split(';');
            this.Id = int.Parse(values[1]);
            this.Naam = values[3];
            this.Regio = regio;

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

        public bool Exists(int provincieId) {
            return Array.Exists(provincies, element => element.Id.Equals(provincieId));
        }

        public void Remove(int _index) {
            for (int index = _index; index < provincies.Length - 1; index++) {
                provincies[index] = provincies[index + 1];
            }
            Count = Count - 1;
            Array.Resize(ref provincies, Count);
        }
        
        public Provincie Get(int Id) {
            Provincie provincie = null;
            foreach (var item in provincies) {
                if (item.Id.Equals(Id)) {
                    return item;
                }
            }
            return provincie;
        }
    }
}
