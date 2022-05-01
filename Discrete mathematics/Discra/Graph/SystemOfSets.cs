namespace Discra
{
    public class SystemOfSets
    {
        private List<int>[] sets;

        public SystemOfSets(int size)
        {
            sets = new List<int>[size].Select(x => new List<int>()).ToArray();

            for (var i = 0; i < size; i++)
            {
                sets[i].Add(i); 
            }
        }

        public int FindSet(int v)
        {
            for (var i = 0; i < sets.Length; i++)
            {
                for (var j = 0; j < sets[i].Count; j++)
                {
                    if (sets[i][j] == v) return i;
                }
            }

            return 0;
        }

        public void Union(int firstSet, int secondSet)
        {
            sets[firstSet] = sets[firstSet].Concat(sets[secondSet]).ToList();
            sets[secondSet].Clear();
        }
    }
}
