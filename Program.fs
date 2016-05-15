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

        let selectedShape = if selectedID > -1 then getShape shapeList selectedID else createShape (new RectangleShape(Rectangle(0, 0, 0, 0), 0, Color.Black, -1))
        
        if selectedShape.id > -1 then GUI.form.Invalidate(new Region(new RectangleF(selectedShape.X-44.0f, selectedShape.Y-44.0f, selectedShape.W*2.0f+80.0f, selectedShape.H*2.0f+80.0f)), false)
                                      GUI.form.Update()
                                      
        //At the start we do the computations that we can do with the inputs we have, just as in a regular application
     
        //Sort the list by z-pos so that we draw the shapes in the right Z-order.
        let sortedShapeList = List.sortBy (fun (elem : ShapeObject) -> elem.Zpos) shapeList

        for r in sortedShapeList do
            r.drawShape

        selectedShape.drawOutline

        printfn "Number of active shapes: %d" (List.length shapeList)
        printfn "Selected Shape-ID: %d" (selectedID)

        
        //Next, since we don't have all inputs (yet) we need to wait for them to become available (Async.Await)
        //let! (bang) enables execution to continue on other computations and threads.
        let! somethingObservable = Async.AwaitObservable(observable) 

        //Now that we have recieved a new input we can perform the rest of our computations
        match somethingObservable with
        | "Add square" | "T"->  let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                                return! loop observable (addShape shapeList (new RectangleShape(new Rectangle(400, 400, 40, 40), 0, selectedColor, (List.length shapeList)))) selectedID
        | "Add circle" | "Y"-> let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                               return! loop observable (addShape shapeList (new CircleShape(new RectangleF(400.0f, 400.0f, 40.0f, 40.0f), 0, selectedColor, (List.length shapeList)))) selectedID
        | "→" | "D" -> return! loop observable (replaceShape (moveX selectedShape true) shapeList) selectedID
        | "←" | "A" -> return! loop observable (replaceShape (moveX selectedShape false) shapeList) selectedID
        | "↑" | "W" -> return! loop observable (replaceShape (moveY selectedShape true) shapeList) selectedID
        | "↓" | "S" -> return! loop observable (replaceShape (moveY selectedShape false) shapeList) selectedID
        | "Set color" | "C" -> let selectedColor = Color.FromName(GUI.comboBoxColor.Text)
                               return! loop observable (replaceShape (changeColor selectedShape selectedColor) shapeList) selectedID
                            //If list is empty we want nothing to be selected. If at end of list we want to start over from the head (index 0).
        | "Select next" | "N" ->  if List.isEmpty shapeList then return! loop observable shapeList -1 
                                  //Get rid of previous "Selected"-highlight
                                  GUI.form.Invalidate(new Region(new RectangleF(selectedShape.X-42.0f, selectedShape.Y-42.0f, selectedShape.W*4.0f, selectedShape.H*4.0f)), false)
                                  if selectedID >= (List.length shapeList - 1) then return! loop observable shapeList 0 
                                                                               else return! loop observable shapeList (selectedID + 1)
        | "Move Forward" | "F" -> return! loop observable (replaceShape (moveZ selectedShape true) shapeList) selectedID
        | "Move Backward" | "G" -> return! loop observable (replaceShape (moveZ selectedShape false) shapeList) selectedID
        | "Resize bigger" | "X" -> return! loop observable (replaceShape (resize selectedShape true) shapeList) selectedID
        | "Resize smaller" | "Z" -> return! loop observable (replaceShape (resize selectedShape false) shapeList) selectedID
        | "Remove" | "R" -> return! loop observable (removeShape shapeList selectedID) -1
        | "Save to file" ->         let rec writeString (l : ShapeObject List) = match l with
                                                                                 | [] -> failwith "Empty"
                                                                                 | x::[] -> objectToString x
                                                                                 | x::xs -> objectToString x + "\n" + writeString xs
                                    let string1 = writeString shapeList

                                    let printStringToFile fileName =
                                       use file = System.IO.File.CreateText(fileName)
                                       fprintfn file "%s" string1
                                    printfn "Saving %d shape-objects to file at location: %s" (List.length shapeList) (Directory.GetCurrentDirectory() + "\\Shapes.txt")
                                    printStringToFile (Directory.GetCurrentDirectory() + "\\Shapes.txt")
                                    
                                    return! loop observable shapeList selectedID

        | "Load from file" ->   use sr = new StreamReader (Directory.GetCurrentDirectory() + "\\Shapes.txt")
                                let rec readFile (s : StreamReader) (l : string list list) = match s with
                                                                                             | _ when s.EndOfStream = true -> l
                                                                                             | _ -> let objectString = sr.ReadLine()
                                                                                                    printfn "%s" objectString
                                                                                                    readFile s (Array.toList(objectString.Split(' ', '\n'))::l)
                                let objectStringList = readFile sr []

                                let newShapeList =  stringListToObjects objectStringList

                                let sortedNewShapeList = List.sortBy (fun (elem : ShapeObject) -> elem.id) newShapeList

                                sr.Close()

                                GUI.form.Refresh()
                                return! loop observable sortedNewShapeList -1

        | "Exit" -> exit 0
        | _ -> return! loop observable shapeList selectedID


        //The last thing we do is a recursive call to ourselves, thus looping
    }

      

    //The GUIInterface is tightly coupled with the main loop which is its only intented user, thus the nested module
    module GUIInterface = 

        //Here we define what we will be observing (clicks)
        let observables = 
             let rec mergeObs (x : #Control list) = match x with
                                                    | c::[] -> Observable.map (fun _-> c.Name) c.Click
                                                    | c::cs -> Observable.merge (Observable.map (fun _-> c.Name) c.Click) (mergeObs cs)
             Observable.merge (Observable.map (fun (e : KeyEventArgs) -> e.KeyCode.ToString()) GUI.form.KeyDown) (mergeObs GUI.buttonList)
             |> Observable.merge (Observable.map (fun _-> GUI.menuItemsList.Head.Name) GUI.menuItemsList.Head.Click) 
             |> Observable.merge (Observable.map (fun _-> (List.nth GUI.menuItemsList 1).Name) (List.nth GUI.menuItemsList 1).Click) 
             |> Observable.merge (Observable.map (fun _-> (List.nth GUI.menuItemsList 2).Name) (List.nth GUI.menuItemsList 2).Click) 





    //The map transforms the observation (click) by the given function. In our case this means
    //that clicking the button AddX will return X. Note the type of observables : IObservable<int>
    let shapes : (ShapeObject) list = []


    //Starts the main loop and opens the GUI
    Async.StartImmediate(loop GUIInterface.observables shapes -1) ; System.Windows.Forms.Application.Run(GUI.form)

