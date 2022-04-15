using Markdig.Syntax;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf.Helper
{
    internal class RenderStyles
    {
        private static int mTextSize = 20;

        public static SKPaint GetPaint(HeadingBlock pHeadingBlock)
        {
            var paint = new SKPaint();

            paint.TextSize = (6 - pHeadingBlock.Level) * mTextSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.Black;

            //paint.IsStroke = true;
            //paint.StrokeWidth = 3;
            //paint.TextAlign = SKTextAlign.Center;

            return paint;
        }

        public static SKPaint GetParagraphPaint()
        {
            var paint = new SKPaint();

            paint.TextSize = mTextSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.Black;

            return paint;
        }

    }
}
