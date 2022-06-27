using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Pattern
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image (.jpg)|*.jpg";
            saveFileDialog.InitialDirectory = @"c:\temp\";
            Rect bounds = VisualTreeHelper.GetDescendantBounds(Canvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int) bounds.Width, (int) bounds.Height, dpi, dpi,
                System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(Canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));
            if (saveFileDialog.ShowDialog() == true)
            {
                Stream stream = File.Create(saveFileDialog.FileName);
                png.Save(stream);
                stream.Close();
            }
        }
        private void ButtonBase_OnClick_Draw(object sender, RoutedEventArgs e)
        {
           if ( IsNumber(FirstKey.Text) )
           {
               int firstKeySideLeft = Convert.ToInt32(FirstKey.Text);
               int secondKeySideTop = GenerateSecondKey(firstKeySideLeft);
               const int MaxAreaSize = 69, MinAreaSize = 3;
               if (firstKeySideLeft < MinAreaSize || MaxAreaSize < firstKeySideLeft)
               {
                   MessageBox.Show("Число не должно быть меньше трех или больше 70");
                   FirstKey.Text = "";
               }
               else
               {
                   firstKeySideLeft *= 5;
                   secondKeySideTop *= 5;
                   DrawPattern(firstKeySideLeft, secondKeySideTop);
               }
           }
           else
           {
               FirstKey.Text = "";
           }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH", MessageId = "type: FreezableContextPair[]; size: 9449MB")]
        private void DrawPattern(int height, int width)
        {
            Canvas.Children.Clear();
            DrawRectAngel(width, height);
            
            int HorizontalIterator = 5, 
                VerticalIterator = 5, 
                horizontalStartPositionToDraw = 0, 
                verticalStartPositionToDraw = 0;
            bool isDraw = false;
            
            DrawLine(verticalStartPositionToDraw, VerticalIterator, horizontalStartPositionToDraw, HorizontalIterator);
           
            horizontalStartPositionToDraw += HorizontalIterator;
            verticalStartPositionToDraw += VerticalIterator; 
            
            do
            {
                horizontalStartPositionToDraw += HorizontalIterator;
                verticalStartPositionToDraw += VerticalIterator;

                if ( horizontalStartPositionToDraw <= 0 || width <= horizontalStartPositionToDraw )
                {
                    HorizontalIterator *= -1;
                }

                if ( verticalStartPositionToDraw <= 0 ||  height < verticalStartPositionToDraw  )
                {
                    VerticalIterator *= -1;
                }
                isDraw = !isDraw;
                if (isDraw)
                {
                    DrawLine(verticalStartPositionToDraw, VerticalIterator, horizontalStartPositionToDraw, HorizontalIterator);
                }
                
            } while (NotInCorner(horizontalStartPositionToDraw, verticalStartPositionToDraw, width, height));
        }
        
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: FreezableContextPair[]")]
        private void DrawLine(int vertical, int stepVertical,int horizontal, int stepHorizontal )
        {
            Line  newLine = new Line()
            {
                X1 = horizontal,
                X2 = horizontal + stepHorizontal,
                Y1 = vertical,
                Y2 = vertical + stepVertical,
                StrokeThickness = 2,
                Stroke = ColorPicker.Brush
            };
            Canvas.Children.Add(newLine);
        }

        private void DrawRectAngel(int Width, int Height)
        {
            Rectangle border = new Rectangle()
            {
                Stroke = Brushes.Black,
                Width = Width,
                Height = Height
            };
            
            Canvas.Children.Add(border);
        }
        
        private bool NotInCorner(int Horizontal, int Vetical, int Width, int Height)
        {
            return !((Horizontal == 0 && Vetical == 0) || 
                   (Horizontal == Width && Vetical == 0) || 
                   (Horizontal == Width && Vetical == Height) ||
                   (Horizontal == 0 && Vetical == Height));
        }

        private bool IsNumber(string String)
        { 
            try 
            { 
                Convert.ToInt32(String);
            }
            catch (FormatException)
            {
                MessageBox.Show("Вы ввели ввели строку!");
                return false;
            }
            return true;
        }

        private int GenerateSecondKey(int firstKey)
        {
            int secondKey = firstKey * 16 / 10;
            while(IsDigit(secondKey))
            {
                secondKey++;
            }
            return secondKey;
        }
        
        private bool IsDigit(int secondKey)
        {
            for (int i=2; i < secondKey; i++)
            {
                if (secondKey % i == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}