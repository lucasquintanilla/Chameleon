using SimpleImageComparisonClassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Core.Shared
{
    public class CopService
    {
        private static List<Image> _bannedImages = new List<Image>();
        byte _thresholdLevel = 10;
        string _directory;

        public CopService(string directory)
        {
            _directory = directory;
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
            var imagesPathList = Directory.GetFiles(_directory);

            foreach (var ImagePath in imagesPathList)
            {
                _bannedImages.Add(Image.FromFile(ImagePath));
            }
        }
    }
}
