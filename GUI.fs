//Project L4_DVA229
//Created by Björn Dagerman 2015/05 (dagerman@kth.se)
//
//GUI: Creates a simple form with two buttons
namespace Demo
open System.Drawing  
module GUI = 
    open System.Windows.Forms
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

    let btnAddRect = new Button(Text="Add square", Top=690, Left=10, Size=new Size(100, 60))
    let btnAddCircle = new Button(Text="Add circle", Top=690, Left=120, Size=new Size(100, 60))
    let btnMoveX = new Button(Text="←", Top=40)
    let btnMovex = new Button(Text="→", Top=60)
    let btnMovey = new Button(Text="↑", Top=80)
    let btnMoveY = new Button(Text="↓", Top=100)
    let btnSetcolor = new Button(Text="Set color", Top=690, Left=230, Size=new Size(100, 60))
    let btnSelect = new Button(Text="Select next", Top=690, Left=340, Size=new Size(100, 60))
    let btnResizesmall = new Button(Text="Resize smaller", Top=690, Left=450, Size=new Size(100, 60))
    let btnResizebig = new Button(Text="Resize bigger", Top=690, Left=560, Size=new Size(100, 60))
    let btnRemove = new Button(Text="Remove", Top=690, Left=670, Size=new Size(100, 60))

    
    let items = [|"Blue";"Red";"Green";"Yellow";"Purple"|]
    let comboBoxColor = new ComboBox(Top=20, Left=120, DataSource=items, DropDownStyle = ComboBoxStyle.DropDownList)
    

    let buttonList = [btnAddRect; btnAddCircle; btnMovex; btnMoveX; btnMovey; btnMoveY; btnSetcolor; btnSelect; btnResizesmall; btnResizebig; btnRemove]

    form.Controls.AddRange [| btnAddRect ; btnAddCircle; btnMovex ; btnMoveX; btnMovey ; btnMoveY; btnSetcolor ; btnSelect; btnResizesmall ; btnResizebig; comboBoxColor; btnRemove|]
    