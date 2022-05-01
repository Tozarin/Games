namespace Discra
{
    public class CodingAlgorithm
    {
        private string GetCountBinarySymbolsAfterPoint(double number, int count)
        {
            var rezult = "";
            for(var _ = 0; _ < count; _++)
            {
                rezult += (int)(number *= 2);
                number -= (int)number;
            }

            return rezult;
        }

        public string Shannon(Messenge messenge)
        {
            var code = "";
            var sumOfProbes = 0.0;
            var newAlphabet = new Dictionary<char, string>();

            foreach(var pair in messenge.Alphabet)
            {
                var countOfSymbols = (int)Math.Abs(Math.Log2(pair.Value)) + 1;
                newAlphabet.Add(pair.Key, GetCountBinarySymbolsAfterPoint(sumOfProbes, countOfSymbols));
                sumOfProbes += pair.Value;
            }

            foreach(var c in messenge.Data)
            {
                code += newAlphabet[c];
            }

            return code;
        }

        public string ShannonFano(Messenge messenge)
        {
            var code = "";

            var tree = new ShannonFanoTree(messenge);

            tree.BuildTree(0, messenge.AlphabetSize, 1);
            tree.InitAlphabet();

            foreach(var c in messenge.Data)
            {
                code += tree.newAlphabet[c];
            }

            return code;
        }

        public string Huffman(Messenge messenge)
        {
            var code = "";
            var tree = new HuffmanTree();

            tree.InitTree(messenge.Alphabet);
            tree.GenerateCodes();

            var newAlphabet = tree.Codes;

            foreach(var c in messenge.Data)
            {
                code += newAlphabet[c];
            }

            return code;

        }

        public string AdaptiveHuffman(Messenge messenge)
        {
            var code = "";
            var m = new Messenge("\n"); // \n - eof

            foreach (var c in messenge.Data + "\n")
            {
                m = new Messenge(m.Data + c);
                var tree = new HuffmanTree();

                tree.InitTree(m.Alphabet);
                tree.GenerateCodes();

                code += tree.Codes[c];
            }

            return code;
        }

        public string AdaptiveHuffmanWithEsc(Messenge messenge)
        {
            var m = new Messenge(messenge);
            m.AddEsc(); // ~ - esc
            return AdaptiveHuffman(m);
        }

        private Dictionary<char, (int l, int r, double p)> Recalculation(char c, Dictionary<char, (int l, int r, double p)> alphabet)
        {
            var newAlphabet = new Dictionary<char, (int l, int r, double p)>();
            var ch = alphabet[c];
            var left = ch.l;
            var len = ch.r - ch.l;

            foreach(var pair in alphabet)
            {
                newAlphabet.Add(pair.Key, (left, left += (int)(len * pair.Value.p), pair.Value.p));
            }

            return newAlphabet;
        }

        public int Arithmetic(Messenge messenge)
        {
            var interval = 1000000000;
            var newAlphabet = new Dictionary<char, (int l , int r, double p)>();
            var left = 0;

            foreach (var pair in messenge.Alphabet)
            {
                newAlphabet[pair.Key] = (left, left += (int)(pair.Value * interval), pair.Value);
            }

            foreach(var c in messenge.Data)
            {
                newAlphabet = Recalculation(c, newAlphabet);
            }

            var l = messenge.Data.Last();

            return newAlphabet[l].l;
        }

        public List<(int offset, int len, char next)> Lz77(Messenge messenge)
        {
            var code = new List<(int offest, int len, char next)>();
            var sizeOfBuffer = 5;
            var buffer = "";

            for (var i = 0; i < messenge.SizeOfData; i++)
            {
                var posible = new List<(int offset, int len)>();

                for (var j = 0; j < buffer.Length; j++)
                {
                    if (messenge.Data[i] == buffer[j])
                    {
                        var k = i;
                        var n = j;
                        var offset = buffer.Length - j;
                        var len = 0;
                        while (k < messenge.SizeOfData - 1 && messenge.Data[k] == buffer[n % buffer.Length])
                        {
                            k++;
                            n++;
                            len++;
                        }
                        
                        posible.Add((offset, len));
                    }
                }

                if (posible.Count == 0) code.Add((0, 0, messenge.Data[i]));
                else
                {
                    posible = posible.OrderByDescending(x => x.len).ToList();
                    var len = posible.First().len;
                    var offset = posible.First().offset;
                    i += len;
                    code.Add((offset, len, messenge.Data[i]));
                }

                buffer = "";
                for (var j = i + 1 - sizeOfBuffer; j < i + 1; j++)
                {
                    if (j >= 0)
                    {
                        buffer  += messenge.Data[j];
                    }
                }
            }
            return code;
        }
    }
}
