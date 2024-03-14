<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DevMenu
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
        components = New Container()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(DevMenu))
        WindowLocationTimer = New System.Windows.Forms.Timer(components)
        Button1 = New System.Windows.Forms.Button()
        Button2 = New System.Windows.Forms.Button()
        TestButton1 = New System.Windows.Forms.Button()
        TestButton2 = New System.Windows.Forms.Button()
        TestButtonUpdateAvailable = New System.Windows.Forms.Button()
        TestButton3 = New System.Windows.Forms.Button()
        SuspendLayout()
        ' 
        ' WindowLocationTimer
        ' 
        WindowLocationTimer.Interval = 1
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(12, 12)
        Button1.Name = "Button1"
        Button1.Size = New Size(136, 23)
        Button1.TabIndex = 0
        Button1.Text = "Show FirstRun"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(12, 41)
        Button2.Name = "Button2"
        Button2.Size = New Size(136, 23)
        Button2.TabIndex = 1
        Button2.Text = "Set Settings To Default"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' TestButton1
        ' 
        TestButton1.Location = New Point(12, 392)
        TestButton1.Name = "TestButton1"
        TestButton1.Size = New Size(136, 23)
        TestButton1.TabIndex = 2
        TestButton1.Text = "Test Button 1"
        TestButton1.UseVisualStyleBackColor = True
        ' 
        ' TestButton2
        ' 
        TestButton2.Location = New Point(12, 421)
        TestButton2.Name = "TestButton2"
        TestButton2.Size = New Size(136, 23)
        TestButton2.TabIndex = 3
        TestButton2.Text = "Test Button 2"
        TestButton2.UseVisualStyleBackColor = True
        ' 
        ' TestButtonUpdateAvailable
        ' 
        TestButtonUpdateAvailable.Location = New Point(12, 479)
        TestButtonUpdateAvailable.Name = "TestButtonUpdateAvailable"
        TestButtonUpdateAvailable.Size = New Size(136, 42)
        TestButtonUpdateAvailable.TabIndex = 4
        TestButtonUpdateAvailable.Text = "Show UpdateAvailable Dialog"
        TestButtonUpdateAvailable.UseVisualStyleBackColor = True
        ' 
        ' TestButton3
        ' 
        TestButton3.Location = New Point(12, 450)
        TestButton3.Name = "TestButton3"
        TestButton3.Size = New Size(136, 23)
        TestButton3.TabIndex = 5
        TestButton3.Text = "Test Button 3"
        TestButton3.UseVisualStyleBackColor = True
        ' 
        ' DevMenu
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(29), CByte(29), CByte(29))
        ClientSize = New Size(160, 533)
        Controls.Add(TestButton3)
        Controls.Add(TestButtonUpdateAvailable)
        Controls.Add(TestButton2)
        Controls.Add(TestButton1)
        Controls.Add(Button2)
        Controls.Add(Button1)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "DevMenu"
        Text = "Developer Menu"
        ResumeLayout(False)
    End Sub

    Friend WithEvents WindowLocationTimer As System.Windows.Forms.Timer
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents TestButton1 As System.Windows.Forms.Button
    Friend WithEvents TestButton2 As System.Windows.Forms.Button
    Friend WithEvents TestButtonUpdateAvailable As System.Windows.Forms.Button
    Friend WithEvents TestButton3 As System.Windows.Forms.Button
End Class
