Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class DevMenu
    Private Sub DevMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Location = New Point(Form1.Left + Form1.Width, Form1.Top)
        WindowLocationTimer.Start()

    End Sub

    Private Sub DevMenu_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        WindowLocationTimer.Stop()
    End Sub

    Private Sub WindowLocationTimer_Tick(sender As Object, e As EventArgs) Handles WindowLocationTimer.Tick
        Me.Location = New Point(Form1.Left + Form1.Width, Form1.Top)
    End Sub

    ' DEV BUTTONS '

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Run FirstRun Button In Dev Menu
        FirstRun.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Set settings to default release build settings (Will NOT change build related settings)
        My.Settings.LastLocation = New Point(0, 0)
        My.Settings.AutoClose = "False"
        My.Settings.AutoMinimize = "True"
        My.Settings.StretchedResolution = "1024x768"
        My.Settings.FirstRun = "True"
        My.Settings.SetDisplayResolution = "True"
        My.Settings.RevertDisplayResolution = "True"
        My.Settings.Monitors = ""
        My.Settings.SelectedGame = "Valorant"
        My.Settings.GameLabelLocation = New Point(75, 11)
        My.Settings.GameMonitor = ""
        My.Settings.MainGPU = ""
        My.Settings.Save()
    End Sub

    Private Sub TestButton1_Click(sender As Object, e As EventArgs) Handles TestButton1.Click

    End Sub

    Private Sub TestButton2_Click(sender As Object, e As EventArgs) Handles TestButton2.Click

    End Sub

    Private Sub TestButton3_Click(sender As Object, e As EventArgs) Handles TestButton3.Click

    End Sub

    Private Sub TestButtonUpdateAvailable_Click(sender As Object, e As EventArgs) Handles TestButtonUpdateAvailable.Click
        UpdateAvailable.Show()
    End Sub

End Class