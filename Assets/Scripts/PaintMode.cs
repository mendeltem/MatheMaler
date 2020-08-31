using System;

namespace Picasso
{
    //die Zustände sind Modis
    [Serializable]
    public enum PaintMode
    {
        
        DrawingLines, 
        Draw,
        DrawLongLine,
        DrawPoints,
        DrawCircles,
    }
}