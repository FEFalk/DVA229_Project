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
open System.IO
open Shape


module Main =
    //This main function loops using async and Async.Await. See lecture F13 for alternatives.
    let rec loop observable (shapeList : (ShapeObject) list) selectedID = async{
        GUI.form.Paint.Add(fun draw -> draw.Graphics.Clear(Color.White))

        for i = 1 to 30 do
            GUI.form.Paint.Add(fun draw -> let pen = new Pen(Color.Black, Width=1.0f)
                                           draw.Graphics.DrawLine(pen, Point(i*40, 0), Point(i*40, 800)))
        for i = 1 to 20 do
            GUI.form.Paint.Add(fun draw -> let pen = new Pen(Color.Black, Width=1.0f)
                                           draw.Graphics.DrawLine(pen, Point(0, i*40), Point(1200, i*40)))
        //At the start we do the computations that we can do with the inputs we have, just as in a regular application
     

        for r in shapeList do
            let brush = new SolidBrush(r.Color)
            if r.isRect then GUI.form.Paint.Add(fun draw->
                           draw.Graphics.FillRectangle(brush, Rectangle.Round r.Rect))
            else GUI.form.Paint.Add(fun draw->
                 draw.Graphics.FillEllipse(brush, r.Rect))
        
        printfn "no of rects: %d" (List.length shapeList)
        printfn "selected id: %d" (selectedID)
        
        GUI.form.Refresh()
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
        | "Resize smaller" ->       return! loop observable (replaceRectangle ((getShape shapeList selectedID).resize false) shapeList) selectedID
        | "Resize bigger" ->        return! loop observable (replaceRectangle ((getShape shapeList selectedID).resize true) shapeList) selectedID
        | "Save to file" ->         let objectToString (o : ShapeObject) = (o.Rect.X.ToString()) + " " + (o.Rect.Y.ToString()) + " " + (o.Rect.Width.ToString()) + " " + 
                                                                           (o.Rect.Height.ToString()) + " " + (o.Color.R.ToString()) + " " + (o.Color.G.ToString()) + " " + 
                                                                           (o.Color.B.ToString()) + " " + (o.isRect.ToString()) + " " + (o.id.ToString())

                                    let rec writeString (l : ShapeObject List) = match l with
                                                                                    | [] -> failwith "Empty"
                                                                                    | x::[] -> objectToString x
                                                                                    | x::xs -> objectToString x + "\n" + writeString xs
                                    let string1 = writeString shapeList

                                    let printStringToFile fileName =
                                       use file = System.IO.File.CreateText(fileName)
                                       fprintfn file "%s" string1
                                    printStringToFile "C:\\Users\\Frenning\\Documents\\Test.txt"
                                    
                                    return! loop observable shapeList selectedID
         | "Load from file" ->      use sr = new StreamReader ("C:\\Users\\Frenning\\Documents\\Test.txt")
                                    let readLine = if not sr.EndOfStream then sr.ReadLine() else failwith "Reached EOF. Terminating..."
                                    printf "%s" readLine
                                    printf "\n"
                                    let words =
                                        let splittedList = Seq.toList (readLine.Split (' ', '\n'))
                                        splittedList
                                    
                                        
                                    let stringToFloat = new RectangleF(((List.head words) : float32), ((List.nth words 1) : float32), ((List.nth words 2) : float32), ((List.nth words 3) : float32)) 
//                                    let color = new Color(((List.nth words 4) : byte), ((List.nth words 5) : byte), ((List.nth words 6) : byte))
//                                    let bool = ((List.Item 7 words) : bool)
//                                    let id = ((List.Item 8 words) : int)
                                    sr.Close()
                                    return! loop observable shapeList selectedID
//        | 9 -> return! loop observable (removeShape shapeList selectedID) selectedID
    

        GUI.form.Refresh()
       
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

   