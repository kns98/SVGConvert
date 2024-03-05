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
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SvgProcessingApp <directoryPath> <width>");
                return;
            }

            string directoryPath = args[0];
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Directory does not exist: {directoryPath}");
                return;
            }

            if (!int.TryParse(args[1], out int desiredWidth))
            {
                Console.WriteLine("Invalid width provided.");
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
                        // Calculate new height to maintain aspect ratio
                        var originalWidth = svg.Picture.CullRect.Width;
                        var originalHeight = svg.Picture.CullRect.Height;
                        var aspectRatio = originalHeight / originalWidth;
                        var desiredHeight = (int)(desiredWidth * aspectRatio);

                        using (var bitmap = new SKBitmap(desiredWidth, desiredHeight))
                        using (var canvas = new SKCanvas(bitmap))
                        {
                            canvas.Clear(SKColors.White);
                            canvas.DrawPicture(svg.Picture, desiredWidth, desiredHeight);
                            using (var image = SKImage.FromBitmap(bitmap))
                            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                            {
                                using (var stream = File.OpenWrite(outputFilePath))
                                {
                                    data.SaveTo(stream);
                                }
                            }
                            Console.WriteLine($"SVG processed and saved to {outputFilePath}");
                            fileCounter++;
                        }
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
