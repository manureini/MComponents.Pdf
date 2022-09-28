using AngleSharp;
using DocumentFormat.OpenXml.ExtendedProperties;
using Markdig;
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

        protected float mInitialX = 0;
        protected float mInitialY = 0;

        protected float mBorderX = 0;
        protected float mBorderY = 0;

        protected float mX = 0;
        protected float mY = 0;

        protected bool mBoldFont;
        protected bool mItalicFont;

        protected float mLineSpace = 10;

        protected float mContentWidth;
        protected float mContentHeight;

        public bool mPageBreakRequired;

        protected IList<Block> mMarkdownDocument;

        protected int mBlocksWritten;
        protected int mInlinesWritten;
        protected int mLinesWritten;

        protected int mListBlocksWritten;
        protected bool mSkipNextBlockItemPrefix;

        protected SKPaint mPaint;

        public MarkdownRenderHelper(string pMarkdown, float pBorderX, float pBorderY)
        {
            mBorderX = pBorderX;
            mBorderY = pBorderY;

            mMarkdownDocument = Markdig.Markdown.Parse(pMarkdown);
        }

        public void Render(SKCanvas pCanvas, float pX, float pY)
        {
            mPageBreakRequired = false;

            mCanvas = pCanvas;
            mInitialX = pX + mBorderX;
            mInitialY = pY + mBorderY;
            mX = mInitialX;
            mY = mInitialY;

            mContentWidth = mCanvas.DeviceClipBounds.Width - mBorderX;
            mContentHeight = mCanvas.DeviceClipBounds.Height - mBorderY;

            for (int i = mBlocksWritten; i < mMarkdownDocument.Count; i++)
            {
                Render(mMarkdownDocument[i]);
                mBlocksWritten = i;

                if (mPageBreakRequired)
                    return;
            }

            mBlocksWritten = 0;
        }

        private async Task Render(Block block)
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
                    Render(list);
                    break;

                case ThematicBreakBlock thematicBreak:
                    //    Render(thematicBreak);
                    break;

                case HtmlBlock html:
                    await Render(html);
                    break;

                default:
                    //    Debug.WriteLine($"Can't render {block.GetType()} blocks.");
                    break;
            }
        }

        private void Render(HeadingBlock pBlock)
        {
            mPaint = RenderStyles.GetPaint(pBlock);
            mY += GetLineHeight();
            Render(((IEnumerable<Inline>)pBlock.Inline).ToArray());

            mX = mInitialX;
            mY += GetLineHeight();
        }

        private void Render(ParagraphBlock pParagraphBlock)
        {
            mPaint = RenderStyles.GetDefaultPaint();
            mY += GetLineHeight();

            Render(((IEnumerable<Inline>)pParagraphBlock.Inline).ToArray());

            mX = mInitialX;
            mY += GetLineHeight();
        }

        private void Render(ListBlock pListBlock)
        {
            mPaint = RenderStyles.GetDefaultPaint();

            for (int i = mListBlocksWritten; i < pListBlock.Count; i++)
            {
                var block = (ListItemBlock)pListBlock[i];

                var oldInitialX = mInitialX;

                mX = mInitialX;

                mY += GetLineHeight();

                if (!mSkipNextBlockItemPrefix)
                {
                    mCanvas.DrawText("-", mX, mY, mPaint);
                }

                MoveCursorXForText("- ");

                var textBounds = new SKRect();
                var width = mPaint.MeasureText("- ", ref textBounds);

                mInitialX += width;

                mListBlocksWritten = i;
                mSkipNextBlockItemPrefix = false;

                foreach (ParagraphBlock paragraphBlock in block)
                {
                    Render(((IEnumerable<Inline>)paragraphBlock.Inline).ToArray());
                    mX = mInitialX;

                    if (mPageBreakRequired)
                    {
                        mSkipNextBlockItemPrefix = true;
                        return;
                    }
                }

                mInitialX = oldInitialX;
            }

            mListBlocksWritten = 0;
            mX = mInitialX;
            mY += GetLineHeight();
        }

        private void Render(Inline[] pInlines)
        {
            for (int i = mInlinesWritten; i < pInlines.Length; i++)
            {
                Inline inline = pInlines[i];
                Render(inline);

                if (mPageBreakRequired)
                    return;

                mInlinesWritten = i;
            }

            mInlinesWritten = 0;
        }

        private void Render(Inline pInline)
        {
            switch (pInline)
            {
                case LiteralInline literal:

                    var text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length);

                    var lineHeight = GetLineHeight();

                    var lines = Linebreak.BreakToLines(mPaint, mX, mBorderX, mContentWidth, text);

                    for (int i = mLinesWritten; i < lines.Length; i++)
                    {
                        var line = lines[i];

                        mCanvas.DrawText(line, mX, mY, mPaint);

                        if (i < lines.Length - 1)
                        {
                            mY += lineHeight;
                            mX = mInitialX;
                        }

                        mLinesWritten = i + 1;

                        if (mY >= mContentHeight)
                        {
                            mPageBreakRequired = true;
                            return;
                        }
                    }

                    mLinesWritten = 0;

                    MoveCursorXForText(lines[lines.Length - 1]);
                    return;

                case LineBreakInline breakline:
                    mX = mInitialX;
                    mY += GetLineHeight();
                    return;

                case LinkInline link:
                    return;

                case CodeInline code:
                    return;

                case EmphasisInline emphasis:

                    if (emphasis.DelimiterCount == 1)
                    {
                        mItalicFont = true;
                    }
                    if (emphasis.DelimiterCount == 2)
                    {
                        mBoldFont = true;
                    }

                    mPaint.Typeface = RenderStyles.GetTypeface(mBoldFont, mItalicFont);

                    foreach (var inline in (IEnumerable<Inline>)emphasis)
                    {
                        Render(inline);
                    }

                    if (emphasis.DelimiterCount == 1)
                    {
                        mItalicFont = false;
                    }

                    if (emphasis.DelimiterCount == 2)
                    {
                        mBoldFont = false;
                    }

                    mPaint.Typeface = RenderStyles.GetTypeface(mBoldFont, mItalicFont);

                    return;

                default:
                    return;
            }
        }


        private async Task Render(HtmlBlock pInline)
        {
            if(pInline.Type == HtmlBlockType.InterruptingBlock)
            {
                var html = string.Join(Environment.NewLine, pInline.Lines.Lines.Select(l => l.ToString()));

                var config = Configuration.Default;
                using var context = BrowsingContext.New(config);
                using var doc = await context.OpenAsync(req => req.Content(html));

            

            }
        }

        private void MoveCursorXForText(string text)
        {
            var textBounds = new SKRect();
            var width = mPaint.MeasureText(text, ref textBounds);

            // mCursorX += textBounds.Size.Width;
            mX += width; // textBounds.Size.Width;
        }

        private float GetLineHeight()
        {
            var textBounds = new SKRect();
            mPaint.MeasureText("A", ref textBounds);
            return textBounds.Size.Height + mLineSpace;
        }

    }
}
