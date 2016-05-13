//Project L4_DVA229
//Created by Björn Dagerman 2015/05 (dagerman@kth.se)
//
//Main

namespace Demo
open FSharpx.Control.Observable //Fsharpx
open System  
open System.Windows.Forms

open System.ComponentModel  
open System.Drawing  
open Shape

module Main =
    let brushArray = [new SolidBrush(Color.Blue);new SolidBrush(Color.Red);new SolidBrush(Color.Green);new SolidBrush(Color.Yellow);
                        new SolidBrush(Color.Purple);new SolidBrush(Color.Brown);new SolidBrush(Color.Pink);new SolidBrush(Color.Black)]
    let pen = new Pen(Color.Aquamarine, Width=2.0f)
    let rec getBrushWithColor c (array : SolidBrush list) = match array with
                                                            | [] -> failwith "Color was not found in brush list!"
                                                            | x::xs when x.Color = c -> x
                                                            | x::xs -> getBrushWithColor c xs

           
    //This main function loops using async and Async.Await. See lecture F13 for alternatives.
    let rec loop observable (shapeList : (ShapeObject) list) selectedID = async{
        let selectedShape = getShape shapeList selectedID
        GUI.form.Invalidate(new Region(new RectangleF(selectedShape.Rect.X-42.0f, selectedShape.Rect.Y-42.0f, selectedShape.Rect.Width*2.0f+80.0f, selectedShape.Rect.Height*2.0f+80.0f)), false)
        GUI.form.Update()
        
        //At the start we do the computations that we can do with the inputs we have, just as in a regular application
        for r in shapeList do
            if r.isRect then GUI.graph.FillRectangle(getBrushWithColor r.Color brushArray, Rectangle.Round r.Rect)
            else GUI.graph.FillEllipse(getBrushWithColor r.Color brushArray, r.Rect)
        if selectedID > -1 then if selectedShape.isRect then GUI.graph.DrawRectangle(pen, Rectangle.Round selectedShape.Rect)
                                                        else GUI.graph.DrawEllipse(pen, selectedShape.Rect)
        

        printfn "no of rects: %d" (List.length shapeList)
        printfn "selected id: %d" (selectedID)

        
        //Next, since we don't have all inputs (yet) we need to wait for them to become available (Async.Await)
        //let! (bang) enables execution to continue on other computations and threads.
        let! somethingObservable = Async.AwaitObservable(observable) 

        //TODO: MOVE OBJECT UPWARDS/DOWNWARDS IN Z
        //Now that we have recieved a new input we can perform the rest of our computations
        match somethingObservable with
        | "Add square" | "T"->  let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                                return! loop observable (addShape shapeList (new ShapeObject(400.0f, 400.0f, 40.0f, 40.0f, selectedColor, true, -1))) selectedID
        | "Add circle" | "Y"-> let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                               return! loop observable (addShape shapeList (new ShapeObject(400.0f, 400.0f, 40.0f, 40.0f, selectedColor, false, -1))) selectedID
        | "→" | "D" -> return! loop observable (replaceRectangle (selectedShape.moveX true) shapeList) selectedID
        | "←" | "A" -> return! loop observable (replaceRectangle (selectedShape.moveX false) shapeList) selectedID
        | "↑" | "W" -> return! loop observable (replaceRectangle (selectedShape.moveY true) shapeList) selectedID
        | "↓" | "S" -> return! loop observable (replaceRectangle (selectedShape.moveY false) shapeList) selectedID
        | "Set color" | "C" -> let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                               return! loop observable (replaceRectangle (selectedShape.changeColor selectedColor) shapeList) selectedID
                            //If list is empty we want nothing to be selected. If at end of list we want to start over from the head (index 0).
        | "Select next" | "N" ->  if List.isEmpty shapeList then return! loop observable shapeList -1 
                                  //Get rid of previous "Selected"-highlight
                                  GUI.form.Invalidate(new Region(new RectangleF(selectedShape.Rect.X-42.0f, selectedShape.Rect.Y-42.0f, selectedShape.Rect.Width*4.0f, selectedShape.Rect.Height*4.0f)), false)
                                  if selectedID >= (List.length shapeList - 1) then return! loop observable shapeList 0 
                                                                               else return! loop observable shapeList (selectedID + 1)
        | "Resize smaller" | "Z" -> return! loop observable (replaceRectangle (selectedShape.resize false) shapeList) selectedID
        | "Resize bigger" | "X" -> return! loop observable (replaceRectangle (selectedShape.resize true) shapeList) selectedID
        | "Remove" | "R" -> return! loop observable (removeShape shapeList selectedID) -1
        | _ -> return! loop observable shapeList selectedID

        //The last thing we do is a recursive call to ourselves, thus looping
    }

    //The GUIInterface is tightly coupled with the main loop which is its only intented user, thus the nested module
    module GUIInterface = 

        //Here we define what we will be observing (clicks)
        let observables = 
             let rec mergeObs (x : Button list) = match x with
                                                    | c::[] -> Observable.map (fun _-> c.Name) c.Click
                                                    | c::cs -> Observable.merge (Observable.map (fun _-> c.Name) c.Click) (mergeObs cs)
             Observable.merge (Observable.map (fun (e : KeyEventArgs) -> e.KeyCode.ToString()) GUI.form.KeyDown) (mergeObs GUI.buttonList)





    //The map transforms the observation (click) by the given function. In our case this means
    //that clicking the button AddX will return X. Note the type of observables : IObservable<int>
    let shapes : (ShapeObject) list = []

    let stringToObject (words : string list) = let x = float32 (List.head words) 
                                               let y = float32 (List.nth words 1)
                                               let w = float32 (List.nth words 2)
                                               let h = float32 (List.nth words 3)
                                               let cr = int (List.nth words 4)
                                               let cg = int (List.nth words 5)
                                               let cb = int (List.nth words 6)
                                               let isRect = Boolean.Parse (List.nth words 7)
                                               let id = int (List.nth words 8)
                                               let color = Color.FromArgb(255, cr, cg, cb)
                                               new ShapeObject(x, y, w, h, color, isRect, id)

    //Starts the main loop and opens the GUI
    Async.StartImmediate(loop GUIInterface.observables shapes -1) ; System.Windows.Forms.Application.Run(GUI.form)

