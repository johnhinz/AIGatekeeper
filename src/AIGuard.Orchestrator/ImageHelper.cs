using AIGuard.Broker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGuard.Orchestrator
{
    public static class ImageHelper
    {
        public static List<MemoryStream> CropBounds(ILogger<Worker> logger, Image image, IPrediction result, Camera camera)
        {
            List<MemoryStream> streams = new List<MemoryStream>();
            foreach (IDetectedObject detection in result.Detections)
            {

                Item watch = camera.Watches?.FirstOrDefault(w => w.Label == detection.Label);
                if (watch == null || detection.Confidence <= watch.Confidence)
                    continue;

                Rectangle cropRect = new Rectangle(detection.XMin, detection.YMin, detection.XMax - detection.XMin, detection.YMax - detection.YMin);
                Bitmap src = image as Bitmap;
                using (Bitmap target = new Bitmap(cropRect.Width, cropRect.Height))
                {
                    using (Graphics g = Graphics.FromImage(target))
                    {

                        g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                            cropRect,
                                            GraphicsUnit.Pixel);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        target.Save(ms, image.RawFormat);
                        streams.Add(ms);
                    }
                }
            }
            return streams;
        }

        public static MemoryStream DrawBounds(ILogger<Worker> logger, Image image, IPrediction result, Camera camera)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                using (Pen redPen = new Pen(Color.Red, 5))
                using (Font font = new Font("Arial", 30, FontStyle.Italic, GraphicsUnit.Pixel))
                using (SolidBrush brush = new SolidBrush(Color.White))

                    foreach (IDetectedObject detection in result.Detections)
                    {
                        Item watch = camera.Watches?.FirstOrDefault(w => w.Label == detection.Label);
                        if (watch == null || detection.Confidence <= watch.Confidence)
                            continue;

                        if (camera.DrawTarget)
                            g.DrawRectangle(
                                redPen,
                                detection.XMin,
                                detection.YMin,
                                detection.XMax - detection.XMin,
                                detection.YMax - detection.YMin);

                        if (camera.DrawConfidence)
                            g.DrawString($"{detection.Label}:{detection.Confidence}",
                                font,
                                brush,
                                new Point(detection.XMin, detection.YMin - (int)redPen.Width - 1));
                    }
            }
            MemoryStream ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            return ms;
        }
    }
}
