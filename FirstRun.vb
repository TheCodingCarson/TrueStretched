Imports System.Windows.Forms

Public Class FirstRun

    Private Sub FirstRun_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Form1.Enabled = False
        Me.TopMost = True

        ' Check if the LastLocation is set in My.Settings
        If Not My.Settings.LastLocation.IsEmpty Then
            ' Load the last location from My.Settings
            Me.Location = My.Settings.LastLocation
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        ' Log First Run Wizard Started
        TrueLog("Info", "First Run Wizard Started")

        ' Save monitor settings to desired settings values
        SaveMonitorInfo()
        My.Settings.GameMonitor = GetPrimaryMonitor()
        My.Settings.Save()

        ' Save main system gpu to My.Settings.MainGPU
        Dim query2 As New SelectQuery("Win32_VideoController")
        Dim searcher2 As New ManagementObjectSearcher(query2)
        Dim gpuManufacturer As String = ""

        For Each gpu As ManagementObject In searcher2.Get()
            gpuManufacturer = gpu("AdapterCompatibility").ToString()
            Exit For
        Next

        If gpuManufacturer.ToLower().Contains("nvidia") Then
            My.Settings.MainGPU = "Nvidia GPU"
            My.Settings.Save()
        ElseIf gpuManufacturer.ToLower().Contains("amd") Then
            My.Settings.MainGPU = "Amd GPU"
            My.Settings.Save()
        ElseIf gpuManufacturer.ToLower().Contains("intel") Then
            My.Settings.MainGPU = "Intel GPU"
            My.Settings.Save()
        Else
            'GPU Is A Different Brand
            My.Settings.MainGPU = ""
            My.Settings.Save()
        End If

        ' Save the settings
        My.Settings.Save()

        ' Log First Run Wizard Saved Computer Info
        TrueLog("Info", "First run wizard saved system information")

        ' Take Focus of First Run Form
        Me.Activate()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Enabled = True
        SettingsForm.Show()
        My.Settings.FirstRun = "False"
        My.Settings.Save()

        ' Log First Run Wizard Completed
        TrueLog("Info", "First Run Wizard Completed")

        Me.Close()
    End Sub
End Class