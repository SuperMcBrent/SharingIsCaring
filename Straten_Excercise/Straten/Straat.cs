using System;
using System.Collections.Generic;
using System.Text;

namespace Straten {
    [Serializable]
    class Straat {

        public int Id { get; set; }
        public string Naam { get; set; }
        public Gemeente Gemeente { get; set; }

        public Straat(string straatCSV) {
            var values = straatCSV.Split(';');
            this.Id = int.Parse(values[0]);
        }

        public Straat(int id, Gemeente gemeente) {
            this.Id = id;
            this.Gemeente = gemeente;
        }

    }

    [Serializable]
    class Straten {
        public Straat[] straten = new Straat[0];
        public int Count { get; private set; }

        public Straat this[int index] {
            get { return straten[index]; }
        }

        public void Add(Straat _straat) {
            Count = Count + 1;
            Array.Resize(ref straten, Count);
            straten[(Count - 1)] = _straat;
        }

        public void Remove(int _index) {
            for (int index = _index; index < straten.Length - 1; index++) {
                straten[index] = straten[index + 1];
            }
            Count = Count - 1;
            Array.Resize(ref straten, Count);
        }

        public bool Exists(int straatId) {
            return Array.Exists(straten, element => element.Id.Equals(straatId));
        }

        public void KenNaamToe(int straatID, string straatnaam) {
            foreach (Straat item in straten) {
                if (item.Id.Equals(straatID)) {
                    item.Naam = straatnaam;
                }
            }
        }
    }

}
