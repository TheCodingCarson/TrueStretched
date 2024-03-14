<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdateAvailable
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(UpdateAvailable))
        Label1 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Button1 = New System.Windows.Forms.Button()
        Button2 = New System.Windows.Forms.Button()
        ProgressBar1 = New System.Windows.Forms.ProgressBar()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(133, 15)
        Label1.TabIndex = 0
        Label1.Text = "New Version Available:"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(151, 9)
        Label2.Name = "Label2"
        Label2.Size = New Size(123, 15)
        Label2.TabIndex = 1
        Label2.Text = "*NewVersionNumber*"
        ' 
        ' RichTextBox1
        ' 
        RichTextBox1.Location = New Point(12, 61)
        RichTextBox1.Name = "RichTextBox1"
        RichTextBox1.ReadOnly = True
        RichTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical
        RichTextBox1.Size = New Size(276, 96)
        RichTextBox1.TabIndex = 2
        RichTextBox1.Text = ""
        ' 
        ' Button1
        ' 
        Button1.Enabled = False
        Button1.Location = New Point(12, 163)
        Button1.Name = "Button1"
        Button1.Size = New Size(111, 23)
        Button1.TabIndex = 3
        Button1.Text = "Update Now"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(177, 163)
        Button2.Name = "Button2"
        Button2.Size = New Size(111, 23)
        Button2.TabIndex = 4
        Button2.Text = "Skip This Version"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(12, 37)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(276, 18)
        ProgressBar1.TabIndex = 5
        ' 
        ' UpdateAvailable
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New Size(300, 191)
        Controls.Add(ProgressBar1)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(RichTextBox1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "UpdateAvailable"
        Text = "True Stretched Update Available"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
End Class
