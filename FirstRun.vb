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

        ' Get the native resolution of the main screen
        Dim mainScreen As Screen = Screen.PrimaryScreen
        Dim nativeResolution As Size = mainScreen.Bounds.Size

        ' Save native resolution to settings
        My.Settings.NativeResolution = nativeResolution.Width & "x" & nativeResolution.Height
        My.Settings.Save()

        ' Get monitors in friendly name format
        Dim monitorNames As New List(Of String)()

        Dim query As New ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Monitor'")
        Dim searcher As New ManagementObjectSearcher(query)

        For Each monitor As ManagementObject In searcher.Get()
            Dim monitorName As String = monitor("Caption")?.ToString()

            If Not String.IsNullOrEmpty(monitorName) Then
                ' Check if the monitor name matches the format "Generic Monitor (XXX)"
                If monitorName.StartsWith("Generic Monitor (") AndAlso monitorName.EndsWith(")") Then
                    ' Remove the "Generic Monitor" prefix and brackets
                    monitorName = monitorName.Substring(17, monitorName.Length - 18)
                End If

                monitorNames.Add(monitorName)
            End If
        Next

        ' Save monitor names to My.Settings.Monitors
        My.Settings.Monitors = String.Join(",", monitorNames)

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

        'TEMPORARY SOLUTION PLEASE UPDATE (SETS THE PRIMARY MONITOR)
        My.Settings.MonitorGameRunsOn = monitorNames(0)
        My.Settings.Save()

        ' Save the settings
        My.Settings.Save()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Enabled = True
        My.Settings.FirstRun = "False"
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.Enabled = True
        SettingsForm.Show()
        My.Settings.FirstRun = "False"
        My.Settings.Save()
        Me.Close()
    End Sub
End Class