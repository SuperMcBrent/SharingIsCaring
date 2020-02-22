using System;
using System.Collections.Generic;
using System.Text;

namespace Straten {
    [Serializable]
    class Regio {

        public int Id { get; set; }
        public string Naam { get; set; }
        public Land Land { get; set; }

        public Provincies Provincies { get; set; }

        public Regio(int id, string naam, Land land) {
            this.Id = id;
            this.Naam = naam;
            this.Land = land;

            this.Provincies = new Provincies();
        }

    }

    [Serializable]
    class Regios {
        public Regio[] regios = new Regio[0];
        public int Count { get; private set; }

        public Regio this[int index] {
            get { return regios[index]; }
        }

        public void Add(Regio _regio) {
            Count = Count + 1;
            Array.Resize(ref regios, Count);
            regios[(Count - 1)] = _regio;
        }

        public void Remove(int _index) {
            for (int index = _index; index < regios.Length - 1; index++) {
                regios[index] = regios[index + 1];
            }
            Count = Count - 1;
            Array.Resize(ref regios, Count);
        }
    }
}
