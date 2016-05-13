
namespace Demo
module Shape =
    open System.Drawing 
    open System.Windows.Forms 

    let brushArray = [new SolidBrush(Color.Blue);new SolidBrush(Color.Red);new SolidBrush(Color.Green);new SolidBrush(Color.Yellow);new SolidBrush(Color.Purple)]
    
    let rec getBrushWithColor c (array : SolidBrush list) = match array with
                                                            | [] -> failwith "Color was not found in brush list!"
                                                            | x::xs when x.Color = c -> x
                                                            | x::xs -> getBrushWithColor c xs

    //TODO : INHERITENCE MED CIRKEL/REKTANGEL 
    //  "The interested student may extend the project in a number of ways, 
    //  including adding more shapes, introducing connectors between shapes, adding text boxes, etc."
    type ShapeObject(xPos : float32, yPos : float32, w : float32, h : float32, z : int, c, isRect : bool, id : int) =
        abstract member moveX: bool -> ShapeObject 
        abstract member moveY: bool -> ShapeObject 
        abstract member moveZ: bool -> ShapeObject 
        abstract member resize: bool -> ShapeObject 
        abstract member changeColor: Color -> ShapeObject 
        member val Rect = new RectangleF(xPos, yPos, w, h)
        member val Zpos = z
        member val Color = c
        member val isRect = isRect
        member val id = id
        default this.moveX movingRight = if movingRight then (new ShapeObject(this.Rect.X+40.0f, this.Rect.Y, this.Rect.Width, this.Rect.Height, this.Zpos, this.Color, this.isRect, this.id))
                                            else (new ShapeObject(this.Rect.X-40.0f, this.Rect.Y, this.Rect.Width, this.Rect.Height, this.Zpos, this.Color, this.isRect, this.id))
        default this.moveY movingUp = if movingUp then (new ShapeObject(this.Rect.X, this.Rect.Y-40.0f, this.Rect.Width, this.Rect.Height, this.Zpos, this.Color, this.isRect, this.id))
                                             else (new ShapeObject(this.Rect.X, this.Rect.Y+40.0f, this.Rect.Width, this.Rect.Height, this.Zpos, this.Color, this.isRect, this.id))
        default this.resize enlarging = if enlarging then (new ShapeObject(this.Rect.X, this.Rect.Y, this.Rect.Width+10.0f, this.Rect.Height+10.0f, this.Zpos, this.Color, this.isRect, this.id))
                                            else if this.Rect.Width >= 30.0f then (new ShapeObject(this.Rect.X, this.Rect.Y, this.Rect.Width-10.0f, this.Rect.Height-10.0f, this.Zpos, this.Color, this.isRect, this.id))
                                                 else this
        default this.moveZ movingForward = if movingForward then (new ShapeObject(this.Rect.X, this.Rect.Y, this.Rect.Width, this.Rect.Height, this.Zpos+1, this.Color, this.isRect, this.id))
                                            else (new ShapeObject(this.Rect.X, this.Rect.Y, this.Rect.Width-10.0f, this.Rect.Height-10.0f, this.Zpos-1, this.Color, this.isRect, this.id))
        default this.changeColor (c : Color) = (new ShapeObject(this.Rect.X, this.Rect.Y, this.Rect.Width, this.Rect.Height, this.Zpos, c, this.isRect, this.id))




    let addShape (shapeList : (ShapeObject) list) (shape : ShapeObject) = List.append shapeList [(new ShapeObject(400.0f, 400.0f, 40.0f, 40.0f, shape.Zpos, shape.Color, shape.isRect, (List.length shapeList)))]
        
    let objectToString (o : ShapeObject) = (o.Rect.X.ToString()) + (o.Rect.Y.ToString()) + (o.Rect.Width.ToString()) + (o.Rect.Height.ToString()) +
                                           (o.Color.R.ToString()) + (o.Color.G.ToString()) + (o.Color.B.ToString()) + (o.isRect.ToString()) + (o.id.ToString())

                                                                                  //If trying to access rect without having one selected
    let rec replaceRectangle (r : ShapeObject) (shapeList : (ShapeObject) list) = if r.id = -1 then shapeList
                                                                                    else match shapeList with
                                                                                            | [] -> shapeList
                                                                                            | x::xs when x.id = r.id -> r::replaceRectangle r xs
                                                                                            | x::xs -> x::replaceRectangle r xs

                                                           //If trying to access rect without having one selected
    let rec getShape (shapeList : (ShapeObject) list) id = if id = -1 then new ShapeObject(400.0f, 400.0f, 5.0f, 5.0f, 0, Color.Blue, true, -1) 
                                                            else match shapeList with
                                                                  | [] -> failwith "List is empty or reached end of list in getShape!"
                                                                  | x::xs when x.id = id -> x
                                                                  | x::xs -> getShape xs id
                                           
                                            
    let rec remove i l =
            match i, l with
            | 0, x::xs -> xs
            | i, x::xs -> x::remove (i - 1) xs
            | i, [] -> failwith "index out of range"

    let rec HupdateID (l : ShapeObject list) n = match l with
                                                | x::[] -> [new ShapeObject(x.Rect.X, x.Rect.Y, x.Rect.Width, x.Rect.Height, x.Zpos, x.Color, x.isRect, n)]
                                                | x::xs -> new ShapeObject(x.Rect.X, x.Rect.Y, x.Rect.Width, x.Rect.Height, x.Zpos, x.Color, x.isRect, n)::HupdateID xs (n+1)
    let updateID l = HupdateID l 0

    let removeShape (shapeList : (ShapeObject) list) id = let selectedShape = getShape shapeList id
                                                          GUI.form.Invalidate(new Region(new RectangleF(selectedShape.Rect.X-40.0f, selectedShape.Rect.Y-40.0f, selectedShape.Rect.Width*4.0f, selectedShape.Rect.Height*4.0f)), false)
                                                          if id = -1 then shapeList elif id = 0 && List.length shapeList = 1 then remove id shapeList else updateID (remove id shapeList)

    let intersectionRectangle (s1 : ShapeObject) (s2 : ShapeObject) = 
        let s3 = RectangleF.Intersect(s1.Rect, s2.Rect)
        if s1.Zpos > s2.Zpos then new ShapeObject(s3.X, s3.Y, s3.Width, s3.Height, 0, s1.Color, s1.isRect, 0) 
                             else new ShapeObject(s3.X, s3.Y, s3.Width, s3.Height, 0, s2.Color, s2.isRect, 0)