
namespace Demo
module Shape =
    open System.Drawing 
    type IShapeObject =
        abstract member Rect : RectangleF
        abstract member Color : Color with get
        abstract member isRect : bool
        abstract member id : int

    type ShapeObject(xPos, yPos, w, h, c, isRect, id) =
        interface IShapeObject with
            member this.Rect = new RectangleF(xPos, yPos, w, h)
            member this.Color = c
            member this.isRect = isRect
            member this.id = id
    
    let addShape (shapeList : (IShapeObject) list) isRect = List.append shapeList [(new ShapeObject(400.0f, 400.0f, 5.0f, 5.0f, Color.Blue, isRect, (List.length shapeList)))]

    //let moveX RectangleF n = 