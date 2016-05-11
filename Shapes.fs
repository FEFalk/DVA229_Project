
namespace Demo
module Shape =
    open System.Drawing 

    type ShapeObject(xPos : float32, yPos : float32, w : float32, h : float32, c, isRect : bool, id : int) =
        member val Rect = new RectangleF(xPos, yPos, w, h)
        member val Color = c
        member val isRect = isRect
        member val id = id
    
    let rec remove i l =
            match i, l with
            | 0, x::xs -> xs
            | i, x::xs -> x::remove (i - 1) xs
            | i, [] -> failwith "index out of range"
    let addShape (shapeList : (ShapeObject) list) isRect = List.append shapeList [(new ShapeObject(400.0f, 400.0f, 5.0f, 5.0f, Color.Blue, isRect, (List.length shapeList)))]
    let addShape2 (shapeList : (ShapeObject) list) isRect = List.append shapeList [(new ShapeObject(300.0f, 300.0f, 5.0f, 5.0f, Color.Blue, not isRect, (List.length shapeList)))]
    let removeShape (shapeList : (ShapeObject) list) = remove 0 shapeList 
    //let moveX RectangleF n = 