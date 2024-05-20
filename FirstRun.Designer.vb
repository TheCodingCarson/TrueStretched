Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FirstRun
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(FirstRun))
        Label1 = New Label()
        Label2 = New Label()
        Button1 = New Button()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        Label1.Location = New Point(69, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(162, 15)
        Label1.TabIndex = 0
        Label1.Text = "Welcome to True Stretched"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(4, 32)
        Label2.Name = "Label2"
        Label2.Size = New Size(290, 60)
        Label2.TabIndex = 1
        Label2.Text = "Recommended: Opening setting and setting your" & vbCrLf & "desired stretched resolution. If you choose to skip this" & vbCrLf & "step you will have to manually set your resolution" & vbCrLf & "before enabling True Stretched"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(4, 95)
        Button1.Name = "Button1"
        Button1.Size = New Size(290, 23)
        Button1.TabIndex = 3
        Button1.Text = "Open Settings"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' FirstRun
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(300, 126)
        ControlBox = False
        Controls.Add(Button1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "FirstRun"
        ShowInTaskbar = False
        Text = "True Stretched - First Run"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Button1 As Button
End Class
