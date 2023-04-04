using Markdig.Syntax;
using SkiaSharp;

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

        public static SKPaint GetDefaultPaint()
        {
            var paint = new SKPaint();

            paint.TextSize = mTextSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.Black;
            paint.Typeface = GetTypeface(false, false);

            return paint;
        }

        public static SKTypeface GetTypeface(bool pBold, bool pItalic)
        {
            return SKTypeface.FromFamilyName("Arial", pBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, pItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
        }
    }
}
