using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf.Helper.Markdown
{
    internal class MarkdownRenderHelper
    {
        protected SKCanvas mCanvas;

        internal float mX = 100;
        internal float mY = 100;

        public MarkdownRenderHelper(SKCanvas pCanvas, float pX, float pY)
        {
            mCanvas = pCanvas;
            mX = pX;
            mY = pY;
        }

        public void Render(string pMarkdown)
        {
            var parsed = Markdig.Markdown.Parse(pMarkdown);
            Render((IList<Block>)parsed);
        }


        private void Render(IList<Block> blocks)
        {
            foreach (var block in blocks)
            {
                Render(block);
            }
        }

        private void Render(Block block)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    Render(heading);
                    break;

                case ParagraphBlock paragraph:
                    Render(paragraph);
                    break;

                case QuoteBlock quote:
                    //     Render(quote);
                    break;

                case CodeBlock code:
                    //    Render(code);
                    break;

                case ListBlock list:
                    //     Render(list);
                    break;

                case ThematicBreakBlock thematicBreak:
                    //    Render(thematicBreak);
                    break;

                case HtmlBlock html:
                    //    Render(html);
                    break;

                default:
                    //    Debug.WriteLine($"Can't render {block.GetType()} blocks.");
                    break;
            }

            /*
            if (queuedViews.Any())
            {
                foreach (var view in queuedViews)
                {
                    this.stack.Children.Add(view);
                }
                queuedViews.Clear();
            }
            */
        }

        private void Render(HeadingBlock block)
        {
            var paint = RenderStyles.GetPaint(block);
            Render((IEnumerable<Inline>)block.Inline, paint);
        }

        private void Render(ParagraphBlock pParagraphBlock)
        {
            var paint = RenderStyles.GetParagraphPaint();
            Render((IEnumerable<Inline>)pParagraphBlock.Inline, paint);
        }

        private void Render(IEnumerable<Inline> pInlines, SKPaint pPaint)
        {
            foreach (var inline in pInlines)
            {
                Render(inline, pPaint);
            }
        }

        private void Render(Inline pInline, SKPaint pPaint)
        {
            switch (pInline)
            {
                case LiteralInline literal:

                    var text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length);

                    var rect = new SKRect(mX, mY, mX + 100, mY + 100);

                    mCanvas.DrawText(text, mX, mY, pPaint);

                    var textBounds = new SKRect();
                    pPaint.MeasureText(text, ref textBounds);

                    // mCursorX += textBounds.Size.Width;
                    mY += textBounds.Size.Height;

                    return;

                case LineBreakInline breakline:
                    mX = 0;
                    mY += 100;
                    return;

                case LinkInline link:
                    return;

                case CodeInline code:
                    return;
            }
        }

    }
}
