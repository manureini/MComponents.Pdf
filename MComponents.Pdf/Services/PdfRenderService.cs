using DocumentFormat.OpenXml.Drawing;
using Markdig.Syntax;
using MComponents.MForm;
using MComponents.Pdf.Helper;
using MComponents.Pdf.Helper.Markdown;
using MComponents.Pdf.Helper.PropertyField;
using Microsoft.Extensions.Localization;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf.Services
{
    public class PdfRenderService
    {
        protected IStringLocalizer L;

        public PdfRenderService(IStringLocalizer pL)
        {
            L = pL;
        }

        public void RenderFields(IEnumerable<IMField> pFields, object pModel)
        {
            var metadata = new SKDocumentPdfMetadata
            {
                Creation = DateTime.Now,
                Modified = DateTime.Now,
            };

            using var document = SKDocument.CreatePdf("test.pdf", metadata);
            using var paint = new SKPaint();

            /*
            paint.TextSize = 64.0f;
            paint.IsAntialias = true;
            paint.Color = (SKColor)0xFF9CAFB7;
            paint.IsStroke = true;
            paint.StrokeWidth = 3;
            paint.TextAlign = SKTextAlign.Center;
            */

            var pageWidth = 840;
            var pageHeight = 1188;

            using (var pdfCanvas = document.BeginPage(pageWidth, pageHeight))
            {
                float offset = 100;

                foreach (var field in pFields)
                {
                    /*
                    if (field is Markdown.MarkdownMFieldGenerator mfg)
                    {
                        var render = new MarkdownRenderHelper(pdfCanvas, 100, offset, 100, 100);
                        render.Render(mfg.Text);

                       // offset = render.mY;
                    }
                    */

                    if (field is IMPropertyField propField)
                    {
                        var render = new PropertyFieldRenderHelper(pdfCanvas, 100, offset, L);
                        render.Render(propField, pModel);

                        //  offset = render.mY;
                    }
                }

                document.EndPage();
            }

            document.Close();
        }

        public Stream RenderMarkdown(string pMarkdown)
        {
            var metadata = new SKDocumentPdfMetadata
            {
                Creation = DateTime.Now,
                Modified = DateTime.Now,
            };

            using var document = SKDocument.CreatePdf("test.pdf", metadata);
            using var paint = new SKPaint();

            /*
            paint.TextSize = 64.0f;
            paint.IsAntialias = true;
            paint.Color = (SKColor)0xFF9CAFB7;
            paint.IsStroke = true;
            paint.StrokeWidth = 3;
            paint.TextAlign = SKTextAlign.Center;
            */

            var pageWidth = 840;
            var pageHeight = 1188;

            var render = new MarkdownRenderHelper(pMarkdown, 100, 100);

            while (true)
            {
                using var pdfCanvas = document.BeginPage(pageWidth, pageHeight);
                render.Render(pdfCanvas, 0, 0);

                document.EndPage();

                if (!render.mPageBreakRequired)
                    break;
            }

            document.Close();

            return null;
        }


    }
}
