using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Tests
{
    public class Coder
    {
        public void Haffman(string messenge, char[] alphabet)
        {
            int len = alphabet.Length;
            List<double> p = new double[len].Select(x => 1.0).ToList();
            double count = len;
            foreach (char c in messenge)
            {
                List<Letters> letters = new List<Letters>();

                for (int i = 0; i < len; i++)
                {
                    letters.Add(new Letters(p[i] / count, i, alphabet[i]));
                }

                for (int _ = 0; _ < len - 1; _++)
                {
                    double mn = 1;
                    int index = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (letters[i].Prob <= mn)
                        {
                            index = i;
                            mn = letters[i].Prob;
                        }
                    }

                    double mn2 = 1;
                    int index2 = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (letters[i].Prob <= mn2 && !(letters[index].ConnectedElements.Contains(i)))
                        {
                            index2 = i;
                            mn2 = letters[i].Prob;
                        }
                    }

                    double mn3 = mn + mn2;
                    foreach (int i in letters[index].ConnectedElements)
                    {
                        letters[i].Prob = mn3;
                        letters[i].Code += "1";
                    }

                    foreach (int i in letters[index2].ConnectedElements)
                    {
                        letters[i].Prob = mn3;
                        letters[i].Code += "0";
                    }

                    letters[index].ConnectedElements = letters[index].ConnectedElements.Concat(letters[index2].ConnectedElements).ToList();
                    letters[index2].ConnectedElements = letters[index].ConnectedElements;
                }

                for (int i = 0; i < len; i++)
                {
                    if (letters[i].Letter == c)
                    {
                        p[i]++;
                        Write(new string(letters[i].Code.Reverse().ToArray()));
                        count++;
                        break;
                    }
                }
            }
        }

        public void HaffmanWithEsc(string messenge)
        {
            List<char> alphabet = new List<char> { '~', '@' };
            int len = 2;
            List<double> p = new List<double> { 1.0, 1.0 };
            double count = len;

            string escCode = "1";

            foreach(char c in messenge)
            {
                if (!alphabet.Contains(c))
                {
                    WriteLine(new string(escCode.Reverse().ToArray()));
                    p.Add(1.0);
                    alphabet.Add(c);
                    len++;
                    count++;
                }

                List<Letters> letters = new List<Letters>();

                for (int i = 0; i < len; i++)
                {
                    letters.Add(new Letters(p[i] / count, i, alphabet[i]));
                }

                for (int _ = 0; _ < len - 1; _++)
                {
                    double mn = 1;
                    int index = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (letters[i].Prob <= mn)
                        {
                            index = i;
                            mn = letters[i].Prob;
                        }
                    }

                    double mn2 = 1;
                    int index2 = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (letters[i].Prob <= mn2 && !(letters[index].ConnectedElements.Contains(i)))
                        {
                            index2 = i;
                            mn2 = letters[i].Prob;
                        }
                    }

                    double mn3 = mn + mn2;
                    foreach (int i in letters[index].ConnectedElements)
                    {
                        letters[i].Prob = mn3;
                        letters[i].Code += "1";
                    }

                    foreach (int i in letters[index2].ConnectedElements)
                    {
                        letters[i].Prob = mn3;
                        letters[i].Code += "0";
                    }

                    letters[index].ConnectedElements = letters[index].ConnectedElements.Concat(letters[index2].ConnectedElements).ToList();
                    letters[index2].ConnectedElements = letters[index].ConnectedElements;
                }

                escCode = letters.First().Code;

                for (int i = 0; i < len; i++)
                {
                    if (letters[i].Letter == c)
                    {
                        p[i]++;
                        WriteLine(new string(letters[i].Code.Reverse().ToArray()));
                        count++;
                        break;
                    }
                }
            }
        }

        public void Arifmetic(string messenge, char[] alphabet, double[] prob)
        {
            List<ArifmeticLetters> letters = new List<ArifmeticLetters>();
            double sum = 0;
            for(int i = 0; i < alphabet.Length; i++)
            {
                letters.Add(new ArifmeticLetters(alphabet[i], sum, sum + prob[i], prob[i]));
                sum += prob[i];
            }

            double len = 1;
            double leftSide = 0;

            foreach (char c in messenge)
            {
                foreach(ArifmeticLetters let in letters)
                {
                    if(let.Letter == c)
                    {
                        double newLen = let.GetLength();
                        leftSide = let.Left;
                        double summa = 0;
                        foreach(ArifmeticLetters letter in letters)
                        {
                            double oldLen = letter.GetLength();
                            letter.Left = leftSide + summa;
                            summa += ((oldLen / len) * newLen);
                            letter.Right = leftSide + summa;
                        } // Харисс Архитектура, 
                        len = newLen;
                        break;
                    }
                }
            }

            WriteLine(letters.First().Left);
        }

        public void LZ77(string messenge)
        {
            string buffer = "";
            for(int i = 0; i < messenge.Length; i++)
            {
                List<List<int>> posible = new List<List<int>>();

                for (int j = 0; j < buffer.Length; j++)
                {
                    if (messenge[i] == buffer[j])
                    {
                        int k = i;
                        int n = j;
                        int offset = buffer.Length - j;
                        int len = 0;
                        while (k < messenge.Length - 1 && messenge[k] == buffer[n % buffer.Length])
                        {
                            k++;
                            n++;
                            len++;
                        }
                        posible.Add(new int[] { len, offset }.ToList());
                    }
                }

                if (posible.Count == 0)
                {
                    WriteLine("0, 0, " + messenge[i]);
                    
                }
                else
                {
                    posible = posible.OrderBy(x => x.First()).Reverse().ToList();
                    i += posible.First().First();
                    WriteLine(posible.First().Last() + ", " + posible.First().First() + ", " +  messenge[i]);
                }

                buffer = "";
                for (int j = i - 9; j <= i; j++)
                {
                    if (j >= 0)
                    {
                        buffer += messenge[j];
                    }
                }
            }
        }
    }
}
