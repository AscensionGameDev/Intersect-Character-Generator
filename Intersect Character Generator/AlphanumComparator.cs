using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Character_Generator
{
    public class AlphanumComparator : IComparer
    {
        private enum ChunkType { Alphanumeric, Numeric }

        private static bool InChunk(char ch, char otherCh)
        {
            var type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            return (type != ChunkType.Alphanumeric || !char.IsDigit(ch)) && (type != ChunkType.Numeric || char.IsDigit(ch));
        }

        public int Compare(object x, object y)
        {
            var s1 = x as string;
            var s2 = y as string;

            if (s1 == null || s2 == null)
            {
                return 0;
            }

            var thisMarker = 0;
            var thatMarker = 0;

            while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
            {
                if (thisMarker >= s1.Length)
                {
                    return -1;
                }

                if (thatMarker >= s2.Length)
                {
                    return 1;
                }

                var thisCh = s1[thisMarker];
                var thatCh = s2[thatMarker];

                var thisChunk = new StringBuilder();
                var thatChunk = new StringBuilder();

                while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while ((thatMarker < s2.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < s2.Length)
                    {
                        thatCh = s2[thatMarker];
                    }
                }

                var result = 0;
                // If both chunks contain numeric characters, sort them numerically
                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    var thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    var thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = -1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = 1;
                    }
                }
                else
                {
                    result = string.Compare(thisChunk.ToString(), thatChunk.ToString(), StringComparison.Ordinal);
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}
