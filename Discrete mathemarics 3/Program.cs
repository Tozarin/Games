namespace EnterpriseProgramming
{
    class Programm
    {
        static void Main(string[] args)
        {
            List<List<int>> vectors = new List<List<int>>();
            vectors.Add(new List<int> { 0, 7, 4, 0, 0, 0 });
            vectors.Add(new List<int> { 0, 0, 4, 2, 0, 0 });
            vectors.Add(new List<int> { 0, 0, 0, 8, 4, 0 });
            vectors.Add(new List<int> { 0, 0, 0, 0, 4, 5 });
            vectors.Add(new List<int> { 0, 0, 0, 0, 0, 12 });
            vectors.Add(new List<int> { 0, 0, 0, 0, 0, 0 });

            Console.WriteLine(Algoritms.FordFalkerson(vectors));
        }
    
    }
}