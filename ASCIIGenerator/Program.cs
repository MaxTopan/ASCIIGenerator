using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

string[] imgPaths = [
    "C:\\Users\\maxto\\Desktop\\test3.jpg",
    "C:\\Users\\maxto\\Pictures\\Camera Roll\\WIN_20241106_15_30_11_Pro.jpg",
    "C:\\Users\\maxto\\Pictures\\Camera Roll\\WIN_20241106_16_55_30_Pro.jpg",
    "C:\\Users\\maxto\\Pictures\\Camera Roll\\WIN_20241106_16_59_53_Pro.jpg"
];

string outPath = "C:\\Users\\maxto\\Desktop\\AsciiOut.txt";
bool outToFile = true;

using Image<Rgba32> image = Image.Load<Rgba32>(imgPaths[3]);

char[] palette = [' ', '.', ',', ';', '*', '#', '@'];

/* RESIZE TO FIT ON A ZOOMED OUT NOTEPAD TXT */
double ratio = (double)image.Height / image.Width;
int targetWidth = 350;
int targetHeight = Convert.ToInt32(targetWidth * ratio);
image.Mutate(x => x.Resize(targetWidth, targetHeight));

StringBuilder result = new StringBuilder();

image.ProcessPixelRows(accessor =>
{
    // loop through each row of pixels in the image
    for (int y = 0; y < accessor.Height; y++)
    {
        Span<Rgba32> pixelRow = accessor.GetRowSpan(y); // SixLabors says this is faster than using the accessor directly throughout ¯\_(ツ)_/¯ 

        for (int x = 0; x < pixelRow.Length; x++)
        {
            ref Rgba32 pixel = ref pixelRow[x];

            // using the NTSC formula: 0.299 ∙ Red + 0.587 ∙ Green + 0.114 ∙ Blue
            double greyscaleVal = pixel.R * 0.299f + pixel.G * 0.114f + pixel.B * 0.114f;
            double normalisedGreyscale = greyscaleVal / 255;
            int convertedVal = Convert.ToInt32(Math.Ceiling(normalisedGreyscale * palette.Length - 1));

            char pixelChar = palette[convertedVal];

            result.Append(pixelChar);
            result.Append(pixelChar);
        }
        result.AppendLine();
    }
    
    if (outToFile)
    {
        File.WriteAllText(outPath, result.ToString());
    }
});