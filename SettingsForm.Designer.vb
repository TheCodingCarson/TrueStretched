Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SettingsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(SettingsForm))
        GroupBox1 = New GroupBox()
        CheckBox5 = New CheckBox()
        Button1 = New Button()
        CheckBox2 = New CheckBox()
        CheckBox1 = New CheckBox()
        GroupBox2 = New GroupBox()
        TextBox1 = New TextBox()
        Label2 = New Label()
        ComboBox2 = New ComboBox()
        Label1 = New Label()
        CheckBox4 = New CheckBox()
        CheckBox3 = New CheckBox()
        GroupBox3 = New GroupBox()
        Label5 = New Label()
        Label4 = New Label()
        Label3 = New Label()
        GroupBox4 = New GroupBox()
        Button2 = New Button()
        Label7 = New Label()
        Label6 = New Label()
        ComboBox3 = New ComboBox()
        GroupBox5 = New GroupBox()
        ComboBox4 = New ComboBox()
        Label8 = New Label()
        GroupBox1.SuspendLayout()
        GroupBox2.SuspendLayout()
        GroupBox3.SuspendLayout()
        GroupBox4.SuspendLayout()
        GroupBox5.SuspendLayout()
        SuspendLayout()
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(CheckBox5)
        GroupBox1.Controls.Add(Button1)
        GroupBox1.Controls.Add(CheckBox2)
        GroupBox1.Controls.Add(CheckBox1)
        GroupBox1.Location = New Point(12, 12)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(276, 103)
        GroupBox1.TabIndex = 0
        GroupBox1.TabStop = False
        GroupBox1.Text = "Options"
        ' 
        ' CheckBox5
        ' 
        CheckBox5.AutoSize = True
        CheckBox5.Location = New Point(6, 72)
        CheckBox5.Name = "CheckBox5"
        CheckBox5.Size = New Size(185, 19)
        CheckBox5.TabIndex = 3
        CheckBox5.Text = "Check For Updates On Startup"
        CheckBox5.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(190, 69)
        Button1.Name = "Button1"
        Button1.Size = New Size(80, 23)
        Button1.TabIndex = 2
        Button1.Text = "Check Now"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' CheckBox2
        ' 
        CheckBox2.AutoSize = True
        CheckBox2.Location = New Point(6, 47)
        CheckBox2.Name = "CheckBox2"
        CheckBox2.Size = New Size(260, 19)
        CheckBox2.TabIndex = 1
        CheckBox2.Text = "Auto Minimize After Enabling True Stretched"
        CheckBox2.UseVisualStyleBackColor = True
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Location = New Point(6, 22)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(240, 19)
        CheckBox1.TabIndex = 0
        CheckBox1.Text = "Auto Close After Enabling True Stretched"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(TextBox1)
        GroupBox2.Controls.Add(Label2)
        GroupBox2.Controls.Add(ComboBox2)
        GroupBox2.Controls.Add(Label1)
        GroupBox2.Controls.Add(CheckBox4)
        GroupBox2.Controls.Add(CheckBox3)
        GroupBox2.Location = New Point(12, 206)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(276, 166)
        GroupBox2.TabIndex = 1
        GroupBox2.TabStop = False
        GroupBox2.Text = "Resolution Settings"
        ' 
        ' TextBox1
        ' 
        TextBox1.Enabled = False
        TextBox1.Location = New Point(6, 87)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(260, 23)
        TextBox1.TabIndex = 6
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(6, 113)
        Label2.Name = "Label2"
        Label2.Size = New Size(116, 15)
        Label2.TabIndex = 5
        Label2.Text = "Stretched Resolution"
        ' 
        ' ComboBox2
        ' 
        ComboBox2.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox2.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox2.FormattingEnabled = True
        ComboBox2.Items.AddRange(New Object() {"1440x1080", "1280x1024", "1280x960", "1024x768", "800x600"})
        ComboBox2.Location = New Point(6, 131)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New Size(260, 23)
        ComboBox2.TabIndex = 4
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(6, 69)
        Label1.Name = "Label1"
        Label1.Size = New Size(100, 15)
        Label1.TabIndex = 3
        Label1.Text = "Native Resolution"
        ' 
        ' CheckBox4
        ' 
        CheckBox4.AutoSize = True
        CheckBox4.Location = New Point(6, 47)
        CheckBox4.Name = "CheckBox4"
        CheckBox4.Size = New Size(219, 19)
        CheckBox4.TabIndex = 1
        CheckBox4.Text = "Revert Display Resolution On Disable"
        CheckBox4.UseVisualStyleBackColor = True
        ' 
        ' CheckBox3
        ' 
        CheckBox3.AutoSize = True
        CheckBox3.Location = New Point(6, 22)
        CheckBox3.Name = "CheckBox3"
        CheckBox3.Size = New Size(199, 19)
        CheckBox3.TabIndex = 0
        CheckBox3.Text = "Set Display Resolution On Enable"
        CheckBox3.UseVisualStyleBackColor = True
        ' 
        ' GroupBox3
        ' 
        GroupBox3.Controls.Add(Label5)
        GroupBox3.Controls.Add(Label4)
        GroupBox3.Controls.Add(Label3)
        GroupBox3.Location = New Point(12, 121)
        GroupBox3.Name = "GroupBox3"
        GroupBox3.Size = New Size(276, 79)
        GroupBox3.TabIndex = 2
        GroupBox3.TabStop = False
        GroupBox3.Text = "About"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(38, 37)
        Label5.Name = "Label5"
        Label5.Size = New Size(202, 15)
        Label5.TabIndex = 2
        Label5.Text = "Always Free From TrueStretched.com"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(72, 55)
        Label4.Name = "Label4"
        Label4.Size = New Size(132, 15)
        Label4.TabIndex = 1
        Label4.Text = "Made By CodingCarson"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(94, 19)
        Label3.Name = "Label3"
        Label3.Size = New Size(48, 15)
        Label3.TabIndex = 0
        Label3.Text = "Version:"
        ' 
        ' GroupBox4
        ' 
        GroupBox4.Controls.Add(Button2)
        GroupBox4.Controls.Add(Label7)
        GroupBox4.Controls.Add(Label6)
        GroupBox4.Controls.Add(ComboBox3)
        GroupBox4.Location = New Point(12, 432)
        GroupBox4.Name = "GroupBox4"
        GroupBox4.Size = New Size(276, 96)
        GroupBox4.TabIndex = 4
        GroupBox4.TabStop = False
        GroupBox4.Text = "Monitor Settings"
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(201, 19)
        Button2.Name = "Button2"
        Button2.Size = New Size(65, 21)
        Button2.TabIndex = 3
        Button2.Text = "Refresh"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Segoe UI", 7.5F, FontStyle.Regular, GraphicsUnit.Point)
        Label7.Location = New Point(17, 76)
        Label7.Name = "Label7"
        Label7.Size = New Size(243, 12)
        Label7.TabIndex = 2
        Label7.Text = "By default this will be your primary display in Windows"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(6, 25)
        Label6.Name = "Label6"
        Label6.Size = New Size(154, 15)
        Label6.TabIndex = 1
        Label6.Text = "Monitor The Game Runs On"
        ' 
        ' ComboBox3
        ' 
        ComboBox3.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox3.FormattingEnabled = True
        ComboBox3.Location = New Point(6, 43)
        ComboBox3.Name = "ComboBox3"
        ComboBox3.Size = New Size(260, 23)
        ComboBox3.TabIndex = 0
        ' 
        ' GroupBox5
        ' 
        GroupBox5.Controls.Add(ComboBox4)
        GroupBox5.Controls.Add(Label8)
        GroupBox5.Location = New Point(12, 378)
        GroupBox5.Name = "GroupBox5"
        GroupBox5.Size = New Size(276, 48)
        GroupBox5.TabIndex = 5
        GroupBox5.TabStop = False
        GroupBox5.Text = "GPU Settings"
        ' 
        ' ComboBox4
        ' 
        ComboBox4.FormattingEnabled = True
        ComboBox4.Items.AddRange(New Object() {"Intel GPU", "Nvidia GPU", "Amd GPU"})
        ComboBox4.Location = New Point(75, 16)
        ComboBox4.Name = "ComboBox4"
        ComboBox4.Size = New Size(191, 23)
        ComboBox4.TabIndex = 1
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(6, 19)
        Label8.Name = "Label8"
        Label8.Size = New Size(63, 15)
        Label8.TabIndex = 0
        Label8.Text = "Main GPU:"
        ' 
        ' SettingsForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.Control
        ClientSize = New Size(300, 533)
        Controls.Add(GroupBox5)
        Controls.Add(GroupBox4)
        Controls.Add(GroupBox3)
        Controls.Add(GroupBox2)
        Controls.Add(GroupBox1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "SettingsForm"
        Text = "True Stretched | Settings"
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        GroupBox3.ResumeLayout(False)
        GroupBox3.PerformLayout()
        GroupBox4.ResumeLayout(False)
        GroupBox4.PerformLayout()
        GroupBox5.ResumeLayout(False)
        GroupBox5.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents CheckBox4 As CheckBox
    Friend WithEvents CheckBox3 As CheckBox
    Friend WithEvents Button1 As Button
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents ComboBox3 As ComboBox
    Friend WithEvents Button2 As Button
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents ComboBox4 As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents CheckBox5 As CheckBox
    Friend WithEvents TextBox1 As TextBox
End Class
