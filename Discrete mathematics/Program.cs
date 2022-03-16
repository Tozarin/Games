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
        }

    }
}
