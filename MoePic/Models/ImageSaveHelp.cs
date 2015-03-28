using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;

namespace MoePic.Models
{
    public static class ImageSaveHelp
    {
        public async static Task SaveImage(string name, System.Windows.Media.Imaging.BitmapImage image)
        {
            try
            {
                MediaLibrary ml = new MediaLibrary();
                System.Windows.Media.ImageSourceConverter con = new System.Windows.Media.ImageSourceConverter();
                using (System.IO.MemoryStream sm = new System.IO.MemoryStream())
                {
                    System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(image as System.Windows.Media.Imaging.BitmapSource);
                    if (System.IO.Path.GetExtension(name) == "png")
                    {
                        Telerik.Windows.Controls.PngEncoder.PNGWriter.WritePNG(wb, sm);
                    }
                    else
                    {
                        System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, sm, wb.PixelWidth, wb.PixelHeight, 0, 100);
                    }
                    sm.Seek(0, System.IO.SeekOrigin.Begin);
                    ml.SavePicture(name, sm);
                    sm.Close();
                }
            }
            catch (Exception)
            {
                ToastService.Show("图片保存失败,请重试");
            }
            

            return;
        }

        public static byte[] GetByte(System.Windows.Media.Imaging.BitmapImage image)
        {
            byte[] bytes;
            using (System.IO.MemoryStream sm = new System.IO.MemoryStream())
            {
                System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(image as System.Windows.Media.Imaging.BitmapSource);
                int w = 0;
                int h = 0;
                if(wb.PixelHeight > wb.PixelWidth)
                {
                    if(wb.PixelHeight > 800)
                    {
                        h = 800;
                        w = (int)(1.0 * 800 / wb.PixelHeight * wb.PixelWidth);
                    }
                }
                else
                {
                    if (wb.PixelWidth > 800)
                    {
                        w = 800;
                        h = (int)(1.0 * 800 / wb.PixelWidth * wb.PixelHeight);
                    }
                }
                    System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, sm, w, h, 0, 100);
                
                sm.Seek(0, System.IO.SeekOrigin.Begin);
                bytes = new byte[sm.Length];
                sm.Read(bytes, 0, (int)sm.Length);
                sm.Close();
            }
            return bytes;
        }

        public static System.IO.Stream GetStream(System.Windows.Media.Imaging.BitmapImage image)
        {
            System.IO.MemoryStream sm = new System.IO.MemoryStream();
            
                System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(image as System.Windows.Media.Imaging.BitmapSource);
                int w = 0;
                int h = 0;
                if (wb.PixelHeight > wb.PixelWidth)
                {
                    if (wb.PixelHeight > 800)
                    {
                        h = 800;
                        w = (int)(1.0 * 800 / wb.PixelHeight * wb.PixelWidth);
                    }
                }
                else
                {
                    if (wb.PixelWidth > 800)
                    {
                        w = 800;
                        h = (int)(1.0 * 800 / wb.PixelWidth * wb.PixelHeight);
                    }
                }
                System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, sm, w, h, 0, 100);
                sm.Seek(0, System.IO.SeekOrigin.Begin);
                return sm;
        }

        public async static Task<String> SaveImageToIso(string format, System.Windows.Media.Imaging.BitmapImage image)
        {
            try
            {
                System.IO.IsolatedStorage.IsolatedStorageFile file = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                var fileStream = file.OpenFile("shared\\media\\share." + format, System.IO.FileMode.Create);
                String path = fileStream.Name;
                System.Windows.Media.ImageSourceConverter con = new System.Windows.Media.ImageSourceConverter();
                using (System.IO.MemoryStream sm = new System.IO.MemoryStream())
                {
                    System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(image as System.Windows.Media.Imaging.BitmapSource);
                    System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, sm, wb.PixelWidth, wb.PixelHeight, 0, 100);
                    sm.Seek(0, System.IO.SeekOrigin.Begin);
                    await sm.CopyToAsync(fileStream);
                    sm.Close();
                    fileStream.Close();
                }

                return path;

            }
            catch (Exception)
            {
                ToastService.Show("图片保存失败,请重试");
                return null;
            }
            
        }

        public static byte[] GetThumbData(System.Windows.Media.Imaging.BitmapImage image)
        {
            System.IO.MemoryStream sm = new System.IO.MemoryStream();

            System.Windows.Media.Imaging.WriteableBitmap wb = new System.Windows.Media.Imaging.WriteableBitmap(image as System.Windows.Media.Imaging.BitmapSource);
            int w = 0;
            int h = 0;
            if (wb.PixelHeight > wb.PixelWidth)
            {
                if (wb.PixelHeight > 150)
                {
                    h = 150;
                    w = (int)(1.0 * 150 / wb.PixelHeight * wb.PixelWidth);
                }
            }
            else
            {
                if (wb.PixelWidth > 150)
                {
                    w = 150;
                    h = (int)(1.0 * 150 / wb.PixelWidth * wb.PixelHeight);
                }
            }
            System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, sm, w, h, 0, 100);
            sm.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] buff = new byte[sm.Length];
            sm.Read(buff, 0, buff.Length);
            sm.Close();
            return buff;
        }

        public async static Task SaveImage(string name, System.IO.Stream stream)
        {
            try
            {
                MediaLibrary ml = new MediaLibrary();
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                var image = new System.Windows.Media.Imaging.BitmapImage();
                image.SetSource(stream);
                await SaveImage(name, image);
            }
            catch (Exception)
            {
                ToastService.Show("图片保存失败,请重试");
            }
            
            return;
        }
    }
}
