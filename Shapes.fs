
namespace Demo
module Shape =
    open System.Drawing 

    type ShapeObject(xPos : float32, yPos : float32, w : float32, h : float32, c, isRect : bool, id : int) =
        member val Rect = new RectangleF(xPos, yPos, w, h)
        member val Color = c
        member val isRect = isRect
        member val id = id
    
    let addShape (shapeList : (ShapeObject) list) isRect = List.append shapeList [(new ShapeObject(400.0f, 400.0f, 5.0f, 5.0f, Color.Blue, isRect, (List.length shapeList)))]

    //let moveX RectangleF n = 