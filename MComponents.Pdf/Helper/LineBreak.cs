using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf.Helper
{
    public class Linebreak
    {
        internal static string[] BreakToLines(SKPaint paint, float pX, float pBorderX, float maxWidth, string text)
        {
            var spaceWidth = paint.MeasureText(" ");

            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines = lines.SelectMany(l => SplitLine(paint, pX, pBorderX, maxWidth, l, spaceWidth)).ToArray();

            return lines;
        }

        private static string[] SplitLine(SKPaint paint, float pX, float pBorderX, float maxWidth, string text, float spaceWidth)
        {
            var result = new List<string>();

            var words = text.Split(new[] { " " }, StringSplitOptions.None);

            var line = new StringBuilder();
            float width = pX;

            foreach (var word in words)
            {
                var wordWidth = paint.MeasureText(word);
                var wordWithSpaceWidth = wordWidth + spaceWidth;
                var wordWithSpace = word + " ";

                if (width + wordWidth > maxWidth)
                {
                    result.Add(line.ToString());
                    line = new StringBuilder(wordWithSpace);
                    width = pBorderX + wordWithSpaceWidth;
                }
                else
                {
                    line.Append(wordWithSpace);
                    width += wordWithSpaceWidth;
                }
            }

            result.Add(line.ToString());

            return result.ToArray();
        }
    }
}
