//Project L4_DVA229
//Created by Björn Dagerman 2015/05 (dagerman@kth.se)
//
//Main

namespace Demo
open FSharpx.Control.Observable //Fsharpx
open System.Windows.Forms
open System  
open System.Windows.Forms  
open System.ComponentModel  
open System.Drawing  
open Shape

module Main =
    //This main function loops using async and Async.Await. See lecture F13 for alternatives.
    let rec loop observable shapeList = async{
        //At the start we do the computations that we can do with the inputs we have, just as in a regular application
        for (r, c, isRect) in shapeList do
            let pen = new Pen((c : Color), Width=12.0f)
            if isRect then GUI.form.Paint.Add(fun draw->
                           draw.Graphics.DrawRectangle(pen, Rectangle.Round r))
            else GUI.form.Paint.Add(fun draw->
                 draw.Graphics.DrawEllipse(pen, r))
        
        printfn "no of rects: %d" (List.length shapeList)
        
        GUI.form.Refresh()
        //Next, since we don't have all inputs (yet) we need to wait for them to become available (Async.Await)
        //let! (bang) enables execution to continue on other computations and threads.
        let! somethingObservable = Async.AwaitObservable(observable) 


        //Now that we have recieved a new input we can perform the rest of our computations
        match somethingObservable with
        | 0 -> return! loop observable (addShape shapeList true)
        | 1 -> return! loop observable (addShape shapeList true)
        | 2 -> return! loop observable (addShape shapeList false)
        | 3 -> return! loop observable (addShape shapeList true)
        | 4 -> return! loop observable (addShape shapeList true)
        
        

        //The last thing we do is a recursive call to ourselves, thus looping
    }

    //The GUIInterface is tightly coupled with the main loop which is its only intented user, thus the nested module
    module GUIInterface = 
        //Here we define what we will be observing (clicks)
        let observables = 
             let rec mergeObs (x : Button list) n = match x with
                                                    | c::[] -> Observable.map (fun _-> n) c.Click
                                                    | c::cs -> Observable.merge (Observable.map (fun _-> n) c.Click) (mergeObs cs (n+1))
             mergeObs GUI.buttonList 0

    //The map transforms the observation (click) by the given function. In our case this means
    //that clicking the button AddX will return X. Note the type of observables : IObservable<int>
    let shapes : (RectangleF * Color * bool) list = []
    let temp = new System.Drawing.RectangleF(50.5f, 30.1f, 20.0f, 10.0f)
    let exitbutton=new Button(Top=190,Left=200)
    exitbutton.Text<-"Exit"
    let pen = new Pen(Color.Blue, Width=12.0f)
    GUI.form.Paint.Add(fun draw-> draw.Graphics.DrawRectangle(pen, Rectangle.Round temp))

    GUI.form.Controls.Add(exitbutton)
    exitbutton.Click.Add(fun exit->GUI.form.Close())

    //Starts the main loop and opens the GUI
    Async.StartImmediate(loop GUIInterface.observables shapes) ; System.Windows.Forms.Application.Run(GUI.form)

   