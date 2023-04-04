using MComponents.Pdf.Services;
using System.IO;
using System.Threading.Tasks;

namespace MComponents.Pdf
{
    public class MainTest
    {
        public static async Task Main(string[] args)
        {
            var testmd = File.ReadAllText("Test.md");

            var mdRender = new PdfRenderService(null);

            await mdRender.RenderMarkdown(testmd);
        }
    }
}
