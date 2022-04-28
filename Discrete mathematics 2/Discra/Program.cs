namespace Discra
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<int>> vectors = new List<List<int>>();
            vectors.Add(new List<int> { 0, 10, 30, 50, 10 });
            vectors.Add(new List<int> { 0, 0, 0, 0, 0 });
            vectors.Add(new List<int> { 0, 0, 0, 0, 10, });
            vectors.Add(new List<int> { 0, 40, 20, 0, 0 });
            vectors.Add(new List<int> { 10, 0, 10, 30, 0 });

            vectors = new Alhoritms().Prim(vectors, 4);

            foreach (List<int> str in vectors)
            {
                foreach(int v in str)
                {
                    Console.Write(v + " ");
                }
                Console.WriteLine();
            }

            List<int> answer = new Alhoritms().Decsra(vectors, 0);
        }
    }

}
