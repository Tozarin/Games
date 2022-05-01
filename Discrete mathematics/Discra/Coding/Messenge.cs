namespace Discra
{
    public class Messenge
    {
        public string Data { get; private set; }
        public int SizeOfData { get; }
        public int AlphabetSize { get; }
        public Dictionary<char, double> Alphabet { get; private set; }

        public Messenge(string messenge)
        {
            Data = messenge;
            SizeOfData = messenge.Length;
            Alphabet = new Dictionary<char, double>();

            foreach(var c in messenge)
            {
                if (Alphabet.ContainsKey(c)) Alphabet[c]++;
                else Alphabet.Add(c, 1);
            }

            Alphabet = Alphabet.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach(var c in Alphabet.Keys)
            {
                Alphabet[c] /= messenge.Length;
            }

            AlphabetSize = Alphabet.Count;
        }

        public Messenge(Messenge messenge)
        {
            Data = new string(messenge.Data);
            SizeOfData = messenge.SizeOfData;
            AlphabetSize = messenge.AlphabetSize;
            Alphabet = new Dictionary<char, double>(messenge.Alphabet);
        }

        public void AddEsc()
        {
            var newData = "";
            var used = new Dictionary<char, bool>();

            foreach(var pair in Alphabet)
            {
                used.Add(pair.Key, false);
            }

            foreach(var c in Data)
            {
                if (!used[c])
                {
                    newData += "~" + c; // ~ - esc
                    used[c] = true;
                }
                else newData += c;
            }

            Data = newData;
        }
    }
}
