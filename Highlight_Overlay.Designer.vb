<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Highlight_Overlay
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(Highlight_Overlay))
        Panel1 = New Windows.Forms.Panel()
        SuspendLayout()
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.Black
        Panel1.Location = New Point(9, 9)
        Panel1.Margin = New System.Windows.Forms.Padding(0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(332, 182)
        Panel1.TabIndex = 0
        ' 
        ' Highlight_Overlay
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = Windows.Forms.AutoScaleMode.Font
        AutoValidate = Windows.Forms.AutoValidate.Disable
        BackColor = Color.Cyan
        ClientSize = New Size(350, 200)
        ControlBox = False
        Controls.Add(Panel1)
        Enabled = False
        FormBorderStyle = Windows.Forms.FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "Highlight_Overlay"
        ShowInTaskbar = False
        SizeGripStyle = Windows.Forms.SizeGripStyle.Hide
        StartPosition = Windows.Forms.FormStartPosition.Manual
        Text = "True Stretched - Monitor Highlight"
        TransparencyKey = Color.Black
        ResumeLayout(False)
    End Sub

    Friend WithEvents Panel1 As Windows.Forms.Panel
End Class
