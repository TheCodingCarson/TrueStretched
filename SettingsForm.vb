Imports System.Reflection
Imports System.Windows.Forms

Public Class SettingsForm
    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Check if the LastLocation is set in My.Settings
        If Not My.Settings.LastLocation.IsEmpty Then
            ' Load the last location from My.Settings
            Me.Location = My.Settings.LastLocation
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        'Load In Settings
        CheckBox1.Checked = My.Settings.AutoClose
        CheckBox2.Checked = My.Settings.AutoMinimize
        CheckBox3.Checked = My.Settings.SetDisplayResolution
        CheckBox4.Checked = My.Settings.RevertDisplayResolution
        CheckBox5.Checked = My.Settings.CheckForUpdateOnStart
        ComboBox1.Text = My.Settings.NativeResolution
        ComboBox2.Text = My.Settings.StretchedResolution
        ComboBox3.Text = My.Settings.MonitorGameRunsOn
        ComboBox4.Text = My.Settings.MainGPU

        If My.Settings.NativeResolution = "" Then
            ComboBox1.Text = "1920x1080"
        Else
            ComboBox1.Text = My.Settings.NativeResolution
        End If

        If My.Settings.StretchedResolution = "" Then
            ComboBox2.Text = "1440x1080"
        Else
            ComboBox2.Text = My.Settings.StretchedResolution
        End If

        ' Ensure Native & Stretched Resolutions Don't match (Fixes "Disable True Stretched" being the only option)
        If My.Settings.NativeResolution = My.Settings.StretchedResolution Then
            ComboBox1.Text = "1920x1080"
            My.Settings.NativeResolution = "1920x1080"
            ComboBox2.Text = "1440x1080"
            My.Settings.StretchedResolution = "1440x1080"
            My.Settings.Save()
        End If

        ' Add monitors to ComboBox3
        ComboBox3.Items.Clear()
        ComboBox3.Items.AddRange(My.Settings.Monitors.Split(","c))

        'Load In Version Info
        Dim currentVersionLong As Version = Assembly.GetExecutingAssembly().GetName().Version
        Dim currentVersionString As String = String.Format("{0}.{1}.{2}", currentVersionLong.Major, currentVersionLong.Minor, currentVersionLong.Build)

        If My.Settings.BetaBuild = True Then
            Label3.Text = "Version: " & currentVersionString & My.Settings.BetaLetter & " Beta"
        Else
            Label3.Text = "Version: " & currentVersionString
        End If

    End Sub

    Private Sub SettingsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' Save the user settigs
        My.Settings.AutoClose = CheckBox1.Checked
        My.Settings.AutoMinimize = CheckBox2.Checked
        My.Settings.SetDisplayResolution = CheckBox3.Checked
        My.Settings.RevertDisplayResolution = CheckBox4.Checked
        My.Settings.CheckForUpdateOnStart = CheckBox5.Checked
        My.Settings.NativeResolution = ComboBox1.Text
        My.Settings.StretchedResolution = ComboBox2.Text
        My.Settings.MonitorGameRunsOn = ComboBox3.Text
        My.Settings.MainGPU = ComboBox4.Text
        My.Settings.Save()

    End Sub

    Private Sub CheckBox1_Click(sender As Object, e As EventArgs) Handles CheckBox1.Click
        CheckBox2.Checked = False
        My.Settings.AutoClose = CheckBox1.Checked
        My.Settings.AutoMinimize = CheckBox2.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckBox2_Click(sender As Object, e As EventArgs) Handles CheckBox2.Click
        CheckBox1.Checked = False
        My.Settings.AutoMinimize = CheckBox2.Checked
        My.Settings.AutoMinimize = CheckBox2.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        My.Settings.SetDisplayResolution = CheckBox3.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        My.Settings.RevertDisplayResolution = CheckBox4.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        My.Settings.CheckForUpdateOnStart = CheckBox5.Checked
        My.Settings.Save()
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Dim url As String = "https://truestretched.com"
        Dim psi As New ProcessStartInfo(url)
        psi.UseShellExecute = True
        Process.Start(psi)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        Dim url As String = "https://www.youtube.com/@CodingCarson"
        Dim psi As New ProcessStartInfo(url)
        psi.UseShellExecute = True
        Process.Start(psi)
    End Sub

    Private Sub ComboBox1_TextChanged(sender As Object, e As EventArgs) Handles ComboBox1.TextChanged
        My.Settings.NativeResolution = ComboBox1.Text
        My.Settings.Save()
    End Sub

    Private Sub ComboBox2_TextChanged(sender As Object, e As EventArgs) Handles ComboBox2.TextChanged
        My.Settings.StretchedResolution = ComboBox2.Text
        My.Settings.Save()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
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
        My.Settings.Save()

        ' Save the settings
        My.Settings.Save()

    End Sub

    Private Sub ComboBox3_TextChanged(sender As Object, e As EventArgs) Handles ComboBox3.TextChanged
        My.Settings.MonitorGameRunsOn = ComboBox3.Text
        My.Settings.Save()
    End Sub

    Private Sub ComboBox4_TextChanged(sender As Object, e As EventArgs) Handles ComboBox4.TextChanged
        My.Settings.MainGPU = ComboBox4.Text
        My.Settings.Save()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        UpdateAvailable.Show()
    End Sub
End Class