using System;
using System.IO;
using SkiaSharp;
using Svg.Skia;

namespace SvgProcessingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SvgProcessingApp <directoryPath>");
                return;
            }

            string directoryPath = args[0];
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Directory does not exist: {directoryPath}");
                return;
            }

            var svgFiles = Directory.GetFiles(directoryPath, "*.svg");
            int fileCounter = 1;

            foreach (var svgFilePath in svgFiles)
            {
                string outputFilePath = Path.Combine(directoryPath, $"output_{fileCounter}.png");

                try
                {
                    var svg = new SKSvg();
                    svg.Load(svgFilePath);

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
                        fileCounter++;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to load SVG file: {svgFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred processing {svgFilePath}: {ex.Message}");
                }
            }

            if (fileCounter == 1) // No files were processed
            {
                Console.WriteLine("No SVG files found to process.");
            }
        }
    }
}
