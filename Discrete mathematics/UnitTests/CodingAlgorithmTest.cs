using NUnit.Framework;
using Discra;
using System.Collections.Generic;

namespace UnitTests
{
    public class CodingAlgorithmTest
    {
        Messenge messenge = new Messenge("abacabacabadaca");

        [Test]
        public void ShannonTest()
        {
            var code = new CodingAlgorithm().Shannon(messenge);

            Assert.AreEqual(code, "010001010100010101000111001010");

            Assert.Pass();
        }

        [Test]
        public void ShannonFanoTest()
        {
            var code = new CodingAlgorithm().ShannonFano(messenge);

            Assert.AreEqual(code, "0100010101000101010001101010");

            Assert.Pass();
        }

        [Test]
        public void HuffmanTest()
        {
            var code = new CodingAlgorithm().Huffman(messenge);

            Assert.AreEqual(code, "01101000110100011010101000");

            Assert.Pass();
        }

        [Test]
        public void AdaptiveHuffmanTest()
        {
            var code = new CodingAlgorithm().AdaptiveHuffman(messenge);

            Assert.AreEqual(code, "101111101100010110001100010010");

            Assert.Pass();
        }

        [Test]
        public void AdaptiveHuffmanWithEscTest()
        {
            var code = new CodingAlgorithm().AdaptiveHuffmanWithEsc(messenge);

            Assert.AreEqual(code, "101111011011011100010001100010101101010010");

            Assert.Pass();
        }

        [Test]
        public void ArithmeticTest()
        {
            var code = new CodingAlgorithm().Arithmetic(messenge);

            Assert.AreEqual(code, 329917851);

            Assert.Pass();
        }

        [Test]
        public void Lz77Test()
        {
            var code = new CodingAlgorithm().Lz77(messenge);
            var c = new List<(int offset, int len, char next)>
            {
                (0, 0, 'a'),
                (0, 0, 'b'),
                (2, 1, 'c'),
                (4, 7, 'd'),
                (4, 1, 'c'),
                (4, 0, 'a')
            };

            Assert.AreEqual(code, c);

            Assert.Pass();
        }
    }
}
