namespace Discra
{
    public class ShannonFanoTree
    {
        (char c, double p, string code)[] codes;
        public Dictionary<char, string> newAlphabet;

        public ShannonFanoTree(Messenge messenge)
        {
            codes = new (char c, double p, string code)[messenge.AlphabetSize];
            newAlphabet = new Dictionary<char, string>();

            var counter = 0;
            foreach (var pair in messenge.Alphabet)
            {
                codes[counter++] = (pair.Key, pair.Value, "");
            }
        }

        public void BuildTree(int left, int right, double sum)
        {
            if (left >= right - 1)
            {
                return;
            }

            var s = 0.0;
            var mid = right;

            for (var i = left; i < right; i++)
            {
                if (s >= sum / 2)
                {
                    mid = i;
                    break;
                }

                s += codes[i].p;
                codes[i].code += "0";

            }

            for(var i = mid; i < right ; i++)
            {
                codes[i].code += "1";
            }

            BuildTree(left, mid, s);
            BuildTree(mid, right, sum - s);
        }

        public void InitAlphabet()
        {
            foreach(var t in codes)
            {
                newAlphabet.Add(t.c, t.code);
            }
        }
    }
}
