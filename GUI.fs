//Project L4_DVA229
//Created by Björn Dagerman 2015/05 (dagerman@kth.se)
//
//GUI: Creates a simple form with two buttons
namespace Demo
module GUI = 
    open System.Windows.Forms
    /// Double-buffered form
    type CompositedForm () =
        inherit Form()
        override this.CreateParams = 
            let cp = base.CreateParams
            cp.ExStyle <- cp.ExStyle ||| 0x02000000
            cp
    let form = new CompositedForm(Text="Demo", TopMost=true, Width=1200, Height=800)
    
    let btnAddRect = new Button(Text="Add square")
    let btnAddCircle = new Button(Text="Add circle", Top=20)
    let btnMoveX = new Button(Text="←", Top=40)
    let btnMovex = new Button(Text="→", Top=60)
    let btnMovey = new Button(Text="↑", Top=80)
    let btnMoveY = new Button(Text="↓", Top=100)
    let btnSetcolor = new Button(Text="Set color", Top=120)
    let btnSelect = new Button(Text="Select next", Top=140)
    let btnResizesmall = new Button(Text="Resize smaller", Top=160)
    let btnResizebig = new Button(Text="Resize bigger", Top=180)

    
    let items = [|"Blue";"Red";"Green";"Yellow";"Purple"|]
    let comboBoxColor = new ComboBox(Top=20, Left=120, DataSource=items, DropDownStyle = ComboBoxStyle.DropDownList)
    

    let inputDisplay = new Label(Text="0", Left=200, BorderStyle = BorderStyle.Fixed3D)
    let outputDisplay = new Label(Text="", Top=40, Left=200, BorderStyle = BorderStyle.Fixed3D)
    let outputExpressionDisplay = new Label(Text="", Top=80, Left=200, BorderStyle = BorderStyle.Fixed3D)

    let buttonList = [btnAddRect; btnAddCircle; btnMovex; btnMoveX; btnMovey; btnMoveY; btnSetcolor; btnSelect; btnResizesmall; btnResizebig]

    form.Controls.AddRange [| btnAddRect ; btnAddCircle; btnMovex ; btnMoveX; btnMovey ; btnMoveY; btnSetcolor ; btnSelect; btnResizesmall ; btnResizebig; comboBoxColor;
                              outputDisplay; inputDisplay; outputExpressionDisplay|]
    