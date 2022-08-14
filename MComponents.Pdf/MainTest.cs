using MComponents.Pdf.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MComponents.Pdf
{
    public class MainTest
    {
        public static void Main(string[] args)
        {

            var testmd = File.ReadAllText("Test.md");


            var mdRender = new PdfRenderService(null);

            mdRender.RenderMarkdown(testmd);



        }
    }
}
