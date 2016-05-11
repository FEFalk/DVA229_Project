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
    let form = new CompositedForm(Text="Demo", TopMost=true, Width=500, Height=600)
    
    let btnAdd0 = new Button(Text="0")
    let btnAdd1 = new Button(Text="1", Top=20)
    let btnAdd2 = new Button(Text="2", Top=40)
    let btnAdd3 = new Button(Text="3", Top=60)
    let btnAdd4 = new Button(Text="4", Top=80)
    let btnAdd5 = new Button(Text="5", Top=100)
    let btnAdd6 = new Button(Text="6", Top=120)
    let btnAdd7 = new Button(Text="7", Top=140)
    let btnAdd8 = new Button(Text="8", Top=160)
    let btnAdd9 = new Button(Text="9", Top=180)

    let items = [|"Blue";"Red";"Green";"Yellow";"Purple"|]
    let comboBoxColor = new ComboBox(Top=20, Left=120, DataSource=items, DropDownStyle = ComboBoxStyle.DropDownList)
    

    let inputDisplay = new Label(Text="0", Left=200, BorderStyle = BorderStyle.Fixed3D)
    let outputDisplay = new Label(Text="", Top=40, Left=200, BorderStyle = BorderStyle.Fixed3D)
    let outputExpressionDisplay = new Label(Text="", Top=80, Left=200, BorderStyle = BorderStyle.Fixed3D)

    let buttonList = [btnAdd0; btnAdd1; btnAdd2; btnAdd3; btnAdd4; btnAdd5; btnAdd6; btnAdd7; btnAdd8; btnAdd9]

    form.Controls.AddRange [| btnAdd0 ; btnAdd1; btnAdd2 ; btnAdd3; btnAdd4 ; btnAdd5; btnAdd6 ; btnAdd7; btnAdd8 ; btnAdd9; comboBoxColor; grid;
                              outputDisplay; inputDisplay; outputExpressionDisplay|]
    