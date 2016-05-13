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
    
        override this.OnPaint(e : PaintEventArgs) = 
            let pen = new Pen(Color.Black, Width=1.0f)
            for i = 1 to 30 do
                e.Graphics.DrawLine(pen, Point(i*40, 0), Point(i*40, 800))
            for i = 1 to 20 do
                e.Graphics.DrawLine(pen, Point(0, i*40), Point(1200, i*40))



    let form = new MainWindow(Width=1220, Height= 800)
    form.Init()
    let graph : Graphics = form.CreateGraphics()         

    //TODO: MOVE OBJECT UPWARDS/DOWNWARDS IN Z
    let btnAddRect = new Button(Text="Add square (T)", Top=700, Left=10, Size=new Size(100, 40), Name="Add square")
    let btnAddCircle = new Button(Text="Add circle (Y)", Top=700, Left=120, Size=new Size(100, 40), Name="Add circle")
    let btnSetcolor = new Button(Text="Set color (C)", Top=700, Left=230, Size=new Size(100, 40), Name="Set color")
    let btnSelect = new Button(Text="Select next (N)", Top=700, Left=500, Size=new Size(100, 40), Name="Select next")
    let btnResizesmall = new Button(Text="Resize smaller (Z)", Top=700, Left=620, Size=new Size(100, 40), Name="Resize smaller")
    let btnResizebig = new Button(Text="Resize bigger (X)", Top=700, Left=740, Size=new Size(100, 40), Name="Resize bigger")
    let btnRemove = new Button(Text="Remove (R)", Top=700, Left=860, Size=new Size(100, 40), Name="Remove")
    let btnMoveX = new Button(Text="←", Top=720, Left = 1020, Name="←", Width=40)
    let btnMovex = new Button(Text="→", Top=720, Left = 1130, Name="→", Width=40)
    let btnMovey = new Button(Text="↑", Top=690, Left = 1070, Name="↑", Width=40)
    let btnMoveY = new Button(Text="↓", Top=720, Left = 1070, Name="↓", Width=40)
    let panel = new Panel(Text="Hej", Top=670, Size=new Size(1220, 130), Name="Panel1")
    let btnSave = new Button(Text="Save to file", Top=100,Left=10,Size=new Size(100, 40))
    let btnLoad = new Button(Text="Load from file", Top=140,Left=10,Size=new Size(100, 40))

    let colors = [|"Blue";"Red";"Green";"Yellow";"Purple"; "Brown"; "Pink"; "Black"|]
    let comboBoxColor = new ComboBox(Top=710, Left=340, DropDownStyle = ComboBoxStyle.DropDownList, DataSource=colors)
    

    let buttonList = [btnAddRect; btnAddCircle; btnMovex; btnMoveX; btnMovey; btnMoveY; btnSetcolor; btnSelect; btnResizesmall; btnResizebig; btnRemove; btnSave; btnLoad;]

    form.Controls.AddRange [| btnAddRect ; btnAddCircle; btnMovex ; btnMoveX; btnMovey ; btnMoveY; btnSetcolor ; btnSelect; btnResizesmall ; btnResizebig; comboBoxColor; btnRemove; panel; btnSave; btnLoad;|]
    