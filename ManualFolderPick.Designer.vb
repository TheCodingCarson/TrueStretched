<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ManualFolderPick
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
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(ManualFolderPick))
        Label1 = New System.Windows.Forms.Label()
        TextBox1 = New System.Windows.Forms.TextBox()
        Button1 = New System.Windows.Forms.Button()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(368, 15)
        Label1.TabIndex = 0
        Label1.Text = "Failed finding {GameName} please select folder containg {EXEName}"
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(12, 37)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(275, 23)
        TextBox1.TabIndex = 1
        TextBox1.Text = "Select Folder Location"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(293, 37)
        Button1.Name = "Button1"
        Button1.Size = New Size(87, 23)
        Button1.TabIndex = 2
        Button1.Text = "Browse"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' ManualFolderPick
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New Size(387, 75)
        Controls.Add(Button1)
        Controls.Add(TextBox1)
        Controls.Add(Label1)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "ManualFolderPick"
        SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Text = "True Stretched - Find Install Location"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
