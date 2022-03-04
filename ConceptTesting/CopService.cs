using SimpleImageComparisonClassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ConceptTesting
{
    public class CopService
    {
        private static List<Image> _bannedImages = new List<Image>();
        byte _thresholdLevel = 10;

        public CopService()
        {
            LoadBannedImages();
        }

        public bool ShouldBeArrested(Image input)
        {
            foreach (var image in _bannedImages)
            {
                float difference = ImageTool.GetPercentageDifference(image, input, _thresholdLevel);
                Console.WriteLine($"Difference: {difference}");
                if (difference < 0.3)
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadBannedImages()
        {
            var ImagesPathList = Directory.GetFiles("images");

            foreach (var ImagePath in ImagesPathList)
            {
                _bannedImages.Add(Image.FromFile(ImagePath));
            }
        }
    }
}
