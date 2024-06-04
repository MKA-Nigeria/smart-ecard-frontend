/*using QRCoder;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using static QRCoder.QRCodeGenerator;

namespace Infrastructure.Common
{
    public class QrCodeService
    {
        public byte[] GenerateQRCode(string text)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCode = qrCodeGenerator.CreateQrCode(text, ECCLevel.Q);

            var info = new SKImageInfo(200, 200);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                float scale = info.Width / (float)qrCode.ModuleMatrix.Count;
                var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    Style = SKPaintStyle.Fill
                };

                for (int y = 0; y < qrCode.ModuleMatrix.Count; y++)
                {
                    for (int x = 0; x < qrCode.ModuleMatrix[y].Count; x++)
                    {
                        if (qrCode.ModuleMatrix[y][x])
                        {
                            canvas.DrawRect(x * scale, y * scale, scale, scale, paint);
                        }
                    }
                }

                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return data.ToArray();
                }
            }
        }
    }
}
*/