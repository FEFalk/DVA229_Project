
namespace Demo
module Shape =
    open System.Drawing 
    let addShape (shapeList : (RectangleF * Color * bool) list) isRect = (new RectangleF(400.0f, 400.0f, 5.0f, 5.0f), Color.Blue, isRect)::shapeList


