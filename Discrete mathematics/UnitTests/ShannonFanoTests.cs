using NUnit.Framework;
using Discra;
using System.Collections.Generic;

namespace UnitTests
{
    public class ShannonFanoTests
    {
        [Test]
        public void BuildTreeTest()
        {
            var messenge = new Messenge("aabbddsscccaa");
            var tree = new ShannonFanoTree(messenge);
            var alph = new Dictionary<char, string>();
            alph.Add('a', "00");
            alph.Add('c', "01");
            alph.Add('b', "100");
            alph.Add('d', "101");
            alph.Add('s', "11");

            tree.BuildTree(0, messenge.AlphabetSize, 1);
            tree.InitAlphabet();

            Assert.AreEqual(alph, tree.newAlphabet);

            Assert.Pass();
        }
    }
}
