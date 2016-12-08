using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VMAX.Helpers
{
    public class VMAXHelper
    {
        public enum VMAX_COLOR
        {
            GRADIENT_BLUE,
            GRADIENT_GREEN
        }

        private static LinearGradientBrush getBlueGradientBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 1);
            brush.EndPoint = new Point(0.5, 0);

            //brush.StartPoint = new Point(0, 0);
            //brush.EndPoint = new Point(0, 1);

            GradientStop gs1 = new GradientStop();
            gs1.Offset = 0;
            gs1.Color = (Color)ColorConverter.ConvertFromString("#FF0F3368");
            GradientStop gs2 = new GradientStop();
            gs2.Offset = 0.86;
            gs2.Color = (Color)ColorConverter.ConvertFromString("#FF6DA9FF");
            GradientStop gs3 = new GradientStop();
            gs3.Offset = 0.468;
            gs3.Color = (Color)ColorConverter.ConvertFromString("#FF85ADD6");
            GradientStop gs4 = new GradientStop();
            gs4.Offset = 1;
            gs4.Color = (Color)ColorConverter.ConvertFromString("#FF325DA8");

            brush.GradientStops.Add(gs1);
            brush.GradientStops.Add(gs2);
           // brush.GradientStops.Add(gs3);
           // brush.GradientStops.Add(gs4);

            /*
             *  <LinearGradientBrush EndPoint="0.5,1" StartPoint="0.5,0">
                    <GradientStop Color="#FF0F3368" Offset="0"/>
                    <GradientStop Color="#FF6DA9FF" Offset="1"/>
                </LinearGradientBrush>*/

            return brush;
        }

        private static LinearGradientBrush getGreenGradientBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 1);
            brush.EndPoint = new Point(0.5, 0);

            GradientStop gs1 = new GradientStop();
            gs1.Offset = 0;
            gs1.Color = (Color)ColorConverter.ConvertFromString("#FF009B15");
            GradientStop gs2 = new GradientStop();
            gs2.Offset = 0.86;
            gs2.Color = (Color)ColorConverter.ConvertFromString("#FF309521");
            GradientStop gs3 = new GradientStop();
            gs3.Offset = 0.468;
            gs3.Color = (Color)ColorConverter.ConvertFromString("#FF90D685");
            GradientStop gs4 = new GradientStop();
            gs4.Offset = 1;
            gs4.Color = (Color)ColorConverter.ConvertFromString("#FF42A832");

            brush.GradientStops.Add(gs1);
            brush.GradientStops.Add(gs2);
            brush.GradientStops.Add(gs3);
            brush.GradientStops.Add(gs4);
            return brush;
        }

        public static LinearGradientBrush getColorBrush(VMAX_COLOR color)
        {
            LinearGradientBrush brush = new LinearGradientBrush();

            switch (color)
            {
                case VMAX_COLOR.GRADIENT_BLUE:
                    brush = getBlueGradientBrush();
                    break;
                case VMAX_COLOR.GRADIENT_GREEN:
                    brush = getGreenGradientBrush();
                    break;
                default:
                    ;
                    break;              
            }

            return brush;
           
        }
    }
}
