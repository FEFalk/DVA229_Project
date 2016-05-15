
namespace Demo
module Shape =
    open System.Drawing 
    open System.Windows.Forms 

    //TODO : INHERITENCE MED CIRKEL/REKTANGEL 
    //  "The interested student may extend the project in a number of ways, 
    //  including adding more shapes, introducing connectors between shapes, adding text boxes, etc."
    [<AbstractClass>]
    type ShapeObject(typeName : string, xPos : float32, yPos : float32, w : float32, h : float32, z : int, c, id : int) =
        abstract member drawShape: unit -> unit with get
        abstract member drawOutline: unit -> unit with get
        member val typeName = typeName
        member val X = xPos
        member val Y = yPos
        member val W = w
        member val H = h
        member val Zpos = z
        member val Color = c
        member val id = id

//    [<AbstractClass>]
//    type MoveableShapeObject(typeName : string, xPos : float32, yPos : float32, w : float32, h : float32, z : int, c, id : int) =
//        inherit ShapeObject(typeName, xPos, yPos, w, h, z, c, id)
//        override this.moveX movingRight = if movingRight then (createShapeByTypeName this.typeName this.X+40.0f this.Y this.W this.H this.Zpos this.Color this.id)
//                                                            else (new ShapeObject(this.X-40.0f, this.Y, this.W, this.H, this.Zpos, this.Color, this.id))
    type RectangleShape(r : Rectangle, z : int, c : Color, id : int) =
        inherit ShapeObject("Square", (float32 r.X), (float32 r.Y), (float32 r.Width), (float32 r.Height), z, c, id)
        member val RectangleShape : Rectangle = new Rectangle(r.X, r.Y, r.Width, r.Height)
        override this.drawShape = GUI.graph.FillRectangle(GUI.getBrushWithColor this.Color GUI.brushArray, this.RectangleShape)
        override this.drawOutline = GUI.graph.DrawRectangle(GUI.pen, this.RectangleShape)

    type CircleShape(r : RectangleF, z : int, c : Color, id : int) =
        inherit ShapeObject("Circle", r.X, r.Y, r.Width, r.Height, z, c, id)
        member val RectangleFShape : RectangleF = new RectangleF(r.X, r.Y, r.Width, r.Height)
        override this.drawShape = GUI.graph.FillEllipse(GUI.getBrushWithColor this.Color GUI.brushArray, this.RectangleFShape)
        override this.drawOutline = GUI.graph.DrawEllipse(GUI.pen, this.RectangleFShape)

    let createShape (s: ShapeObject) = s

    let createShapeByTypeName name (x : float32) (y : float32) (w : float32) (h : float32) z c id = 
                                                            match name with
                                                            | "Square" -> createShape (new RectangleShape(new Rectangle((int x), (int y), (int w), (int h)), z, c, id))
                                                            | "Circle" -> createShape (new CircleShape(new RectangleF(x, y, w, h), z, c, id))
                                                            | _ -> failwith "No valid name was loaded with the shape! in (createShapeByTypeName)"

    let moveX (s : ShapeObject) movingRight = if movingRight then (createShapeByTypeName s.typeName (s.X+40.0f) s.Y s.W s.H s.Zpos s.Color s.id)
                                              else (createShapeByTypeName s.typeName (s.X-40.0f) s.Y s.W s.H s.Zpos s.Color s.id)
    let moveY (s : ShapeObject) movingUp = if movingUp then (createShapeByTypeName s.typeName s.X (s.Y-40.0f) s.W s.H s.Zpos s.Color s.id)
                                           else (createShapeByTypeName s.typeName s.X (s.Y+40.0f) s.W s.H s.Zpos s.Color s.id)
    let resize (s : ShapeObject) enlarging = if enlarging then (createShapeByTypeName s.typeName s.X s.Y (s.W+10.0f) (s.H+10.0f) s.Zpos s.Color s.id)
                                             else if s.W >= 30.0f then (createShapeByTypeName s.typeName s.X s.Y (s.W-10.0f) (s.H-10.0f) s.Zpos s.Color s.id)
                                             else s
    let moveZ (s : ShapeObject) movingForward = if movingForward then (createShapeByTypeName s.typeName s.X s.Y s.W s.H (s.Zpos+1) s.Color s.id)
                                                else (createShapeByTypeName s.typeName s.X s.Y s.W s.H (s.Zpos-1) s.Color s.id)
    let changeColor (s : ShapeObject) (c : Color) = (createShapeByTypeName s.typeName s.X s.Y s.W s.H s.Zpos c s.id)

    let addShape (shapeList : (ShapeObject) list) (shape : ShapeObject) = List.append shapeList [shape]
        
    let objectToString (o : ShapeObject) = o.typeName + " " + (o.X.ToString()) + " " + (o.Y.ToString()) + " " + (o.W.ToString()) + " " + (o.H.ToString()) + " " + (o.Zpos.ToString()) + " " +
                                           (o.Color.R.ToString()) + " " + (o.Color.G.ToString()) + " " + (o.Color.B.ToString()) + " " + (o.id.ToString())

    let stringToObject (words : string list) = 
            let x = float32 (List.nth words 1) 
            let y = float32 (List.nth words 2)
            let w = float32 (List.nth words 3)
            let h = float32 (List.nth words 4)
            let z = int (List.nth words 5)
            let cr = int (List.nth words 6)
            let cg = int (List.nth words 7)
            let cb = int (List.nth words 8)
            let id = int (List.nth words 9)
            let color = Color.FromArgb(cr, cg, cb)
            createShapeByTypeName (List.head words) x y w h z color id

    let rec stringListToObjects (objectStrings : string list list) = match objectStrings with
                                                                     | x::[] -> [stringToObject x]
                                                                     | x::xs -> (stringToObject x)::stringListToObjects xs

                                                                                  //If trying to access rect without having one selected
    let rec replaceShape (r : ShapeObject) (shapeList : (ShapeObject) list) = if r.id = -1 then shapeList
                                                                                    else match shapeList with
                                                                                            | [] -> shapeList
                                                                                            | x::xs when x.id = r.id -> r::replaceShape r xs
                                                                                            | x::xs -> x::replaceShape r xs

                                                           //If trying to access rect without having one selected
    let rec getShape (shapeList : (ShapeObject) list) id = match shapeList with
                                                            | [] -> failwith "List is empty or reached end of list in getShape!"
                                                            | x::xs when x.id = id -> x
                                                            | x::xs -> getShape xs id
                                           
                                            
    let rec remove i l =
            match i, l with
            | 0, x::xs -> xs
            | i, x::xs -> x::remove (i - 1) xs
            | i, [] -> failwith "index out of range"

    let rec HupdateID (l : ShapeObject list) n = match l with
                                                 | x::[] -> [createShapeByTypeName x.typeName x.X x.Y x.W x.H x.Zpos x.Color n]
                                                 | x::xs -> (createShapeByTypeName x.typeName x.X x.Y x.W x.H x.Zpos x.Color n)::HupdateID xs (n+1)
    let updateID l = HupdateID l 0

    let removeShape (shapeList : (ShapeObject) list) id = let selectedShape = getShape shapeList id
                                                          GUI.form.Invalidate(new Region(new RectangleF(selectedShape.X-40.0f, selectedShape.Y-40.0f, selectedShape.W*4.0f, selectedShape.H*4.0f)), false)
                                                          if id = -1 then shapeList elif id = 0 && List.length shapeList = 1 then remove id shapeList else updateID (remove id shapeList)
