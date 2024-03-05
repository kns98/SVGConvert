using System;
using SkiaSharp;
using Svg.Skia;

namespace SvgProcessingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SvgProcessingApp <input.svg> <output.png>");
                return;
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            try
            {
                var svg = new SKSvg();
                svg.Load(inputFilePath);

                if (svg.Picture != null)
                {
                    using (var image = SKImage.FromPicture(svg.Picture, svg.Picture.CullRect.Size.ToSizeI()))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
                    {
                        using (var stream = System.IO.File.OpenWrite(outputFilePath))
                        {
                            data.SaveTo(stream);
                        }
                    }
                    Console.WriteLine($"SVG processed and saved to {outputFilePath}");
                }
                else
                {
                    Console.WriteLine("Failed to load SVG file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
