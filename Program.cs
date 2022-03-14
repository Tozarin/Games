using System;
using static System.Console;
using System.Linq;
using System.Collections.Generic;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            string messenge = "abcbbbbbacabbacddacdbbaccbbadadaddd abcccccbacabbacbbaddbdaccbbddadadcc bcabbcdabacbbacbbddcbbaccbbdbdadaac";

            double[] p = { 0.23, 0.31, 0.23, 0.19, 0.02, 0.02 };

            List<Letters> letters = new List<Letters>();

            for(int i = 0; i < 6; i++)
            {
                letters.Add(new Letters(p[i], i));
            }

            for(int _ = 0; _ < 5; _++)
            {
                double mn = 1;
                int index = 0;
                for(int i = 0; i < 6; i++)
                {
                    if (letters[i].P <= mn)
                    {
                        index = i;
                        mn = letters[i].P;
                    }
                }

                double mn2 = 1;
                int index2 = 0;
                for(int i = 0; i < 6; i++)
                {
                    if(letters[i].P <= mn2 && !(letters[index].OtherElements.Contains(i)))
                    {
                        index2 = i;
                        mn2 = letters[i].P;
                    }
                }

                foreach(int i in letters[index].OtherElements)
                {
                    letters[i].P = mn + mn2;
                    letters[i].Code += "1";
                }

                foreach (int i in letters[index2].OtherElements)
                {
                    letters[i].P = mn + mn2;
                    letters[i].Code += "0";
                }

                letters[index].OtherElements = letters[index].OtherElements.Concat(letters[index2].OtherElements).ToList();
                letters[index2].OtherElements = letters[index].OtherElements;
            }

            foreach (Letters l in letters)
            {
                string str = new string(l.Code.Reverse().ToArray());
                WriteLine(str);
            }

        }

    }
}
