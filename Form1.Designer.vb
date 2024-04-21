Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Form1))
        Button1 = New Button()
        PictureBox2 = New PictureBox()
        Label1 = New Label()
        PictureBox1 = New PictureBox()
        Label2 = New Label()
        Label3 = New Label()
        PictureBox3 = New PictureBox()
        FortnitePictureBox = New PictureBox()
        ValorantPictureBox = New PictureBox()
        ApexPictureBox = New PictureBox()
        Label4 = New Label()
        AutoCloseTimer = New Timer(components)
        AutoMinimizeTimer = New Timer(components)
        CheckWindowTimer = New Timer(components)
        GroupBox1 = New GroupBox()
        LinkLabel1 = New LinkLabel()
        Label5 = New Label()
        Farlight84PictureBox = New PictureBox()
        GroupBox3 = New GroupBox()
        Label6 = New Label()
        DelayValSwapCheckBox = New CheckBox()
        WidescreenFixCheckBox = New CheckBox()
        Label7 = New Label()
        CType(PictureBox2, ISupportInitialize).BeginInit()
        CType(PictureBox1, ISupportInitialize).BeginInit()
        CType(PictureBox3, ISupportInitialize).BeginInit()
        CType(FortnitePictureBox, ISupportInitialize).BeginInit()
        CType(ValorantPictureBox, ISupportInitialize).BeginInit()
        CType(ApexPictureBox, ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        CType(Farlight84PictureBox, ISupportInitialize).BeginInit()
        GroupBox3.SuspendLayout()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(59, 138)
        Button1.Name = "Button1"
        Button1.Size = New Size(144, 35)
        Button1.TabIndex = 1
        Button1.Text = "Enable True Stretched"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' PictureBox2
        ' 
        PictureBox2.BackColor = Color.FromArgb(CByte(29), CByte(29), CByte(29))
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(257, 495)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(35, 34)
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox2.TabIndex = 3
        PictureBox2.TabStop = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.FromArgb(CByte(29), CByte(29), CByte(29))
        Label1.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold)
        Label1.ForeColor = Color.White
        Label1.Location = New Point(12, 511)
        Label1.Name = "Label1"
        Label1.Size = New Size(151, 17)
        Label1.TabIndex = 4
        Label1.Text = "Made By CodingCarson"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.FromArgb(CByte(29), CByte(29), CByte(29))
        PictureBox1.Location = New Point(-5, 504)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(312, 32)
        PictureBox1.TabIndex = 7
        PictureBox1.TabStop = False
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.BackColor = Color.Transparent
        Label2.ForeColor = Color.White
        Label2.Location = New Point(12, 476)
        Label2.Name = "Label2"
        Label2.Size = New Size(42, 15)
        Label2.TabIndex = 8
        Label2.Text = "Status:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.BackColor = Color.Transparent
        Label3.ForeColor = Color.White
        Label3.Location = New Point(60, 476)
        Label3.Name = "Label3"
        Label3.Size = New Size(26, 15)
        Label3.TabIndex = 9
        Label3.Text = "Idle"
        ' 
        ' PictureBox3
        ' 
        PictureBox3.BackColor = Color.FromArgb(CByte(29), CByte(29), CByte(29))
        PictureBox3.Location = New Point(248, -1)
        PictureBox3.Name = "PictureBox3"
        PictureBox3.Size = New Size(55, 535)
        PictureBox3.TabIndex = 10
        PictureBox3.TabStop = False
        ' 
        ' FortnitePictureBox
        ' 
        FortnitePictureBox.Image = CType(resources.GetObject("FortnitePictureBox.Image"), Image)
        FortnitePictureBox.Location = New Point(253, 109)
        FortnitePictureBox.Name = "FortnitePictureBox"
        FortnitePictureBox.Size = New Size(43, 43)
        FortnitePictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        FortnitePictureBox.TabIndex = 11
        FortnitePictureBox.TabStop = False
        ' 
        ' ValorantPictureBox
        ' 
        ValorantPictureBox.Image = CType(resources.GetObject("ValorantPictureBox.Image"), Image)
        ValorantPictureBox.Location = New Point(253, 158)
        ValorantPictureBox.Name = "ValorantPictureBox"
        ValorantPictureBox.Size = New Size(43, 43)
        ValorantPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        ValorantPictureBox.TabIndex = 12
        ValorantPictureBox.TabStop = False
        ' 
        ' ApexPictureBox
        ' 
        ApexPictureBox.Image = CType(resources.GetObject("ApexPictureBox.Image"), Image)
        ApexPictureBox.Location = New Point(253, 11)
        ApexPictureBox.Name = "ApexPictureBox"
        ApexPictureBox.Size = New Size(43, 43)
        ApexPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        ApexPictureBox.TabIndex = 13
        ApexPictureBox.TabStop = False
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.BackColor = Color.Transparent
        Label4.Font = New Font("Segoe UI", 18F, FontStyle.Bold)
        Label4.ForeColor = Color.White
        Label4.Location = New Point(75, 11)
        Label4.Name = "Label4"
        Label4.Size = New Size(136, 32)
        Label4.TabIndex = 14
        Label4.Text = "Game Title"
        ' 
        ' AutoCloseTimer
        ' 
        AutoCloseTimer.Interval = 1000
        ' 
        ' AutoMinimizeTimer
        ' 
        AutoMinimizeTimer.Interval = 1000
        ' 
        ' CheckWindowTimer
        ' 
        CheckWindowTimer.Interval = 1000
        ' 
        ' GroupBox1
        ' 
        GroupBox1.BackColor = Color.Transparent
        GroupBox1.Controls.Add(LinkLabel1)
        GroupBox1.Controls.Add(Label5)
        GroupBox1.ForeColor = Color.White
        GroupBox1.Location = New Point(14, 371)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(221, 92)
        GroupBox1.TabIndex = 21
        GroupBox1.TabStop = False
        GroupBox1.Text = "Game Guide"
        ' 
        ' LinkLabel1
        ' 
        LinkLabel1.AutoSize = True
        LinkLabel1.LinkColor = Color.White
        LinkLabel1.Location = New Point(6, 65)
        LinkLabel1.Name = "LinkLabel1"
        LinkLabel1.Size = New Size(192, 15)
        LinkLabel1.TabIndex = 1
        LinkLabel1.TabStop = True
        LinkLabel1.Text = "https://TrueStretched.com/*Game*"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(6, 19)
        Label5.Name = "Label5"
        Label5.Size = New Size(203, 30)
        Label5.TabIndex = 0
        Label5.Text = "Before Enabling: Check out the guide" & vbCrLf & "on TrueStretched.com"
        ' 
        ' Farlight84PictureBox
        ' 
        Farlight84PictureBox.Image = My.Resources.Resources.Farlight84_Logo
        Farlight84PictureBox.Location = New Point(253, 60)
        Farlight84PictureBox.Name = "Farlight84PictureBox"
        Farlight84PictureBox.Size = New Size(43, 43)
        Farlight84PictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        Farlight84PictureBox.TabIndex = 23
        Farlight84PictureBox.TabStop = False
        ' 
        ' GroupBox3
        ' 
        GroupBox3.BackColor = Color.Transparent
        GroupBox3.Controls.Add(Label6)
        GroupBox3.Controls.Add(DelayValSwapCheckBox)
        GroupBox3.Controls.Add(WidescreenFixCheckBox)
        GroupBox3.Controls.Add(Label7)
        GroupBox3.ForeColor = Color.White
        GroupBox3.Location = New Point(14, 203)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(221, 162)
        GroupBox3.TabIndex = 24
        GroupBox3.TabStop = False
        GroupBox3.Text = "Valorant Extras"
        GroupBox3.Visible = False
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(6, 105)
        Label6.Name = "Label6"
        Label6.Size = New Size(198, 15)
        Label6.TabIndex = 4
        Label6.Text = "Fixes Stretching Before Game Opens"
        ' 
        ' DelayValSwapCheckBox
        ' 
        DelayValSwapCheckBox.AutoSize = True
        DelayValSwapCheckBox.Location = New Point(6, 83)
        DelayValSwapCheckBox.Name = "DelayValSwapCheckBox"
        DelayValSwapCheckBox.Size = New Size(112, 19)
        DelayValSwapCheckBox.TabIndex = 3
        DelayValSwapCheckBox.Text = "Delay Stretching"
        DelayValSwapCheckBox.UseVisualStyleBackColor = True
        ' 
        ' WidescreenFixCheckBox
        ' 
        WidescreenFixCheckBox.AutoSize = True
        WidescreenFixCheckBox.Location = New Point(6, 22)
        WidescreenFixCheckBox.Name = "WidescreenFixCheckBox"
        WidescreenFixCheckBox.Size = New Size(151, 19)
        WidescreenFixCheckBox.TabIndex = 2
        WidescreenFixCheckBox.Text = "Widescreen Monitor Fix"
        WidescreenFixCheckBox.UseVisualStyleBackColor = True
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(6, 44)
        Label7.Name = "Label7"
        Label7.Size = New Size(197, 15)
        Label7.TabIndex = 1
        Label7.Text = "Fixes widescreen monitor black bars"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(41), CByte(55), CByte(66))
        BackgroundImageLayout = ImageLayout.Zoom
        ClientSize = New Size(300, 533)
        Controls.Add(GroupBox3)
        Controls.Add(Farlight84PictureBox)
        Controls.Add(GroupBox1)
        Controls.Add(Label4)
        Controls.Add(ApexPictureBox)
        Controls.Add(ValorantPictureBox)
        Controls.Add(FortnitePictureBox)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(PictureBox2)
        Controls.Add(Label1)
        Controls.Add(Button1)
        Controls.Add(PictureBox1)
        Controls.Add(PictureBox3)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form1"
        Text = "True Stretched"
        CType(PictureBox2, ISupportInitialize).EndInit()
        CType(PictureBox1, ISupportInitialize).EndInit()
        CType(PictureBox3, ISupportInitialize).EndInit()
        CType(FortnitePictureBox, ISupportInitialize).EndInit()
        CType(ValorantPictureBox, ISupportInitialize).EndInit()
        CType(ApexPictureBox, ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        CType(Farlight84PictureBox, ISupportInitialize).EndInit()
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents Button1 As Button
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents FortnitePictureBox As PictureBox
    Friend WithEvents ValorantPictureBox As PictureBox
    Friend WithEvents ApexPictureBox As PictureBox
    Friend WithEvents Label4 As Label
    Friend WithEvents AutoCloseTimer As Timer
    Friend WithEvents AutoMinimizeTimer As Timer
    Friend WithEvents CheckWindowTimer As Timer
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label5 As Label
    Friend WithEvents LinkLabel1 As LinkLabel
    Friend WithEvents Farlight84PictureBox As PictureBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label7 As Label
    Friend WithEvents WidescreenFixCheckBox As CheckBox
    Friend WithEvents DelayValSwapCheckBox As CheckBox
    Friend WithEvents Label6 As Label
End Class
