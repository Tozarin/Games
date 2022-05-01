using NUnit.Framework;
using Discra;
using System.Collections.Generic;

namespace UnitTests
{
    public class HuffmanTreeTests
    {
        [Test]
        public void GenerateCodesTest()
        {
            var messenge = new Messenge("aabbddsscccaa");
            var tree = new HuffmanTree();
            var alph = new Dictionary<char, string>();
            alph.Add('b', "11");
            alph.Add('c', "10");
            alph.Add('a', "01");
            alph.Add('s', "001");
            alph.Add('d', "000");

            tree.InitTree(messenge.Alphabet);
            tree.GenerateCodes();

            Assert.AreEqual(alph, tree.Codes);

            Assert.Pass();
        }
    }
}
