//Project L4_DVA229
//Created by Björn Dagerman 2015/05 (dagerman@kth.se)
//
//GUI: Creates a simple form with two buttons
namespace Demo
open System.Drawing  
module GUI = 
    open System.Windows.Forms
    open System.Data
    /// Double-buffered form
    type MainWindow () as this =
        inherit Form()
        member this.Init() =
            this.Text <- "Simple Graphics Editor"
            this.StartPosition <- FormStartPosition.CenterScreen
            this.FormBorderStyle <- FormBorderStyle.Fixed3D
            this.MaximizeBox <- false
            this.DoubleBuffered <- true
            this.SetStyle(ControlStyles.DoubleBuffer, true)
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true)
            this.SetStyle(ControlStyles.UserPaint, true)
            this.SetStyle(ControlStyles.ResizeRedraw, true)
            this.KeyPreview <- true
            //this.Paint.AddHandler(new System.Windows.Forms.PaintEventHandler(fun s pe -> this.Event_Paint(s, pe)))

        override this.OnPaintBackground(e : PaintEventArgs) = 
            e.Graphics.Clear(Color.White)
            let pen = new Pen(Color.Black, Width=1.0f)
            for i = 1 to 30 do
                e.Graphics.DrawLine(pen, Point(i*40, 0), Point(i*40, 800))
            for i = 1 to 20 do
                e.Graphics.DrawLine(pen, Point(0, i*40), Point(1200, i*40))



    let form = new MainWindow(Width=1220, Height= 800)
    form.Init()
    let graph : Graphics = form.CreateGraphics()      
    
    let brushArray = [new SolidBrush(Color.Blue);new SolidBrush(Color.Red);new SolidBrush(Color.Green);new SolidBrush(Color.Yellow);
                        new SolidBrush(Color.Purple);new SolidBrush(Color.Brown);new SolidBrush(Color.Pink);new SolidBrush(Color.Black)]
    
    let pen = new Pen(Color.Aquamarine, Width=4.0f)
    let rec getBrushWithColor (c : Color) (array : SolidBrush list) = match array with
                                                                      | [] -> failwith "Color was not found in brush list!"
                                                                      | x::xs when (x.Color).ToArgb() = c.ToArgb() -> x
                                                                      | x::xs -> getBrushWithColor c xs

    //TODO: MOVE OBJECT UPWARDS/DOWNWARDS IN Z
    let btnAddRect = new Button(Text="Add square (T)", Top=690, Left=10, Size=new Size(100, 40), Name="Add square")
    let btnAddCircle = new Button(Text="Add circle (Y)", Top=690, Left=120, Size=new Size(100, 40), Name="Add circle")
    let btnSetcolor = new Button(Text="Set color (C)", Top=690, Left=230, Size=new Size(100, 40), Name="Set color")
    let btnSelect = new Button(Text="Select next (N)", Top=690, Left=500, Size=new Size(100, 40), Name="Select next")
    let btnResizesmall = new Button(Text="Resize smaller (Z)", Top=690, Left=620, Size=new Size(100, 40), Name="Resize smaller")
    let btnResizebig = new Button(Text="Resize bigger (X)", Top=690, Left=740, Size=new Size(100, 40), Name="Resize bigger")
    let btnRemove = new Button(Text="Remove (R)", Top=690, Left=860, Size=new Size(100, 40), Name="Remove")
    let btnMoveUp = new Button(Text="Move Forward (F)", Top=675, Left=970, Size=new Size(80, 30), Name="Move Forward")
    let btnMoveDown = new Button(Text="Move Backward (G)", Top=675, Left=1120, Size=new Size(80, 30), Name="Move Backward")
    let btnMoveX = new Button(Text="←", Top=710, Left = 1010, Name="←", Width=40)
    let btnMovex = new Button(Text="→", Top=710, Left = 1120, Name="→", Width=40)
    let btnMovey = new Button(Text="↑", Top=680, Left = 1060, Name="↑", Width=40)
    let btnMoveY = new Button(Text="↓", Top=710, Left = 1060, Name="↓", Width=40)
    let panel = new Panel(Top=660, Size=new Size(1220, 130))
    let btnFile = new MenuItem(Text="File")
    let menuItemLoad = new MenuItem(Text="Load", Name="Load from file")
    let menuItemSave = new MenuItem(Text="Save", Name="Save to file")
    let menuItemExit = new MenuItem(Text="Exit", Name="Exit", Shortcut=Shortcut.CtrlX)
    btnFile.MenuItems.AddRange [|menuItemLoad; menuItemSave; menuItemExit|]
    let menu = new MainMenu([|btnFile|])
    form.Menu <- menu

    
    let colors = [|"Blue";"Red";"Green";"Yellow";"Purple"; "Brown"; "Pink"; "Black"|]
    let comboBoxColor = new ComboBox(Top=700, Left=340, DropDownStyle = ComboBoxStyle.DropDownList, DataSource=colors)
   
    let menuItemsList = [menuItemLoad; menuItemSave; menuItemExit]
    let buttonList = [btnAddRect; btnAddCircle; btnMovex; btnMoveX; btnMovey; btnMoveY; btnSetcolor; btnSelect; btnResizesmall; btnResizebig; btnRemove; btnMoveUp; btnMoveDown]

    form.Controls.AddRange [| btnAddRect ; btnAddCircle; btnMovex ; btnMoveX; btnMovey ; btnMoveY; btnSetcolor ; btnSelect; btnResizesmall ; btnResizebig; comboBoxColor; btnRemove; btnMoveUp; btnMoveDown; panel|]
    