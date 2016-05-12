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
    let brushArray = [new SolidBrush(Color.Blue);new SolidBrush(Color.Red);new SolidBrush(Color.Green);new SolidBrush(Color.Yellow);new SolidBrush(Color.Purple)]
    let pen = new Pen(Color.Black, Width=1.0f)
    let rec getBrushWithColor c (array : SolidBrush list) = match array with
                                                            | [] -> failwith "Color was not found in brush list!"
                                                            | x::xs when x.Color = c -> x
                                                            | x::xs -> getBrushWithColor c xs

           
    //This main function loops using async and Async.Await. See lecture F13 for alternatives.
    let rec loop observable (shapeList : (ShapeObject) list) selectedID = async{
        let selectedShape = getShape shapeList selectedID
        GUI.form.Invalidate(new Region(new RectangleF(selectedShape.Rect.X-40.0f, selectedShape.Rect.Y-40.0f, selectedShape.Rect.Width*4.0f, selectedShape.Rect.Height*4.0f)), false)
        GUI.form.Update()
        
        //At the start we do the computations that we can do with the inputs we have, just as in a regular application
        for r in shapeList do
            if r.isRect then GUI.graph.FillRectangle(getBrushWithColor r.Color brushArray, Rectangle.Round r.Rect)
            else GUI.graph.FillEllipse(getBrushWithColor r.Color brushArray, r.Rect)

        printfn "no of rects: %d" (List.length shapeList)
        printfn "selected id: %d" (selectedID)


        //Next, since we don't have all inputs (yet) we need to wait for them to become available (Async.Await)
        //let! (bang) enables execution to continue on other computations and threads.
        let! somethingObservable = Async.AwaitObservable(observable) 

        //Now that we have recieved a new input we can perform the rest of our computations
        match somethingObservable with
        | "Add square" -> return! loop observable (addShape shapeList true) selectedID
        | "Add circle" -> return! loop observable (addShape shapeList false) selectedID
        | "→" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).moveX true) shapeList) selectedID
        | "←" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).moveX false) shapeList) selectedID
        | "↑" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).moveY true) shapeList) selectedID
        | "↓" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).moveY false) shapeList) selectedID
        | "Set color" -> let selectedColorString = GUI.comboBoxColor.Text in let selectedColor = match selectedColorString with
                                                                                                 | "Blue" -> Color.Blue
                                                                                                 | "Red" -> Color.Red
                                                                                                 | "Green" -> Color.Green
                                                                                                 | "Yellow" -> Color.Yellow
                                                                                                 | "Purple" -> Color.Purple
                          return! loop observable (replaceRectangle ((getShape shapeList selectedID).changeColor selectedColor) shapeList) selectedID
                            //If list is empty we want nothing to be selected. If at end of list we want to start over from the head (index 0).
        | "Select next" ->  if List.isEmpty shapeList then return! loop observable shapeList -1 
                                   elif selectedID >= (List.length shapeList - 1) then return! loop observable shapeList 0 
                                   else return! loop observable shapeList (selectedID + 1)
        | "Resize smaller" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).resize false) shapeList) selectedID
        | "Resize bigger" -> return! loop observable (replaceRectangle ((getShape shapeList selectedID).resize true) shapeList) selectedID
        | "Remove" -> return! loop observable (removeShape shapeList selectedID) -1
        

        //The last thing we do is a recursive call to ourselves, thus looping
    }

    //The GUIInterface is tightly coupled with the main loop which is its only intented user, thus the nested module
    module GUIInterface = 
        //Here we define what we will be observing (clicks)
        let observables = 
             let rec mergeObs (x : Button list) = match x with
                                                    | c::[] -> Observable.map (fun _-> c.Text) c.Click
                                                    | c::cs -> Observable.merge (Observable.map (fun _-> c.Text) c.Click) (mergeObs cs)
             mergeObs GUI.buttonList

    //The map transforms the observation (click) by the given function. In our case this means
    //that clicking the button AddX will return X. Note the type of observables : IObservable<int>
    let shapes : (ShapeObject) list = []


    //Starts the main loop and opens the GUI
    Async.StartImmediate(loop GUIInterface.observables shapes -1) ; System.Windows.Forms.Application.Run(GUI.form)

   