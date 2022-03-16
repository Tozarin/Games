// Startsev Matvey's homework
// Coder class contains four methods
// that coding string
// Letters and AlgoritmicLetters are used
// as structures

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Coder coder = new Coder();
            string messenge = "abcbbbbbacabbacddacdbbaccbbadadaddd abcccccbacabbacbbaddbdaccbbddadadcc bcabbcdabacbbacbbddcbbaccbbdbdadaac@";
            char[] alphabet = { 'a', 'b', 'c', 'd', ' ', '@' };
            double[] pr = { 0.23, 0.31, 0.23, 0.19, 0.02, 0.2 };
            
            coder.LZ77(messenge);

            coder.Haffman(messenge, alphabet);

            coder.HaffmanWithEsc(messenge);

            // For large lines returns NaN
            // so I coded it with Python for
            // task's messenge
            coder.Arifmetic(messenge, alphabet, pr);
        }
    }
}
