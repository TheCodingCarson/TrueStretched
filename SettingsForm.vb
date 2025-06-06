﻿Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Header

Public Class SettingsForm
    Dim FormLoading As Boolean = True
    'Initilize Variable For ComboBox3
    Private lastSelectedIndex As Integer = -999

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Check if the LastLocation is set in My.Settings
        If Not My.Settings.LastLocation.IsEmpty Then
            ' Load the last location from My.Settings
            Me.Location = My.Settings.LastLocation
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        ' Log Gettings All Settings
        TrueLog("Info", "Loading all settings for Settings Page...")

        ' Disable Switching Game Monitor Selection till Combobox3 vales are fully loaded
        ComboBox3.Enabled = False

        'Load In Settings
        CheckBox1.Checked = My.Settings.AutoClose
        CheckBox2.Checked = My.Settings.AutoMinimize
        CheckBox3.Checked = My.Settings.SetDisplayResolution
        CheckBox4.Checked = My.Settings.RevertDisplayResolution
        CheckBox5.Checked = My.Settings.CheckForUpdateOnStart
        ComboBox2.Text = My.Settings.StretchedResolution
        ComboBox4.Text = My.Settings.MainGPU

        If My.Settings.StretchedResolution = "" Then
            ComboBox2.Text = "1440x1080"
            My.Settings.StretchedResolution = "1440x1080"
            My.Settings.Save()
        Else
            ComboBox2.Text = My.Settings.StretchedResolution
        End If

        ' Ensure Native & Stretched Resolutions Don't match (Fixes "Disable True Stretched" being the only option)
        If My.Settings.StretchedResolution = GetGameMonitor("MaxResolution") Then
            ComboBox2.Text = "1440x1080"
            My.Settings.StretchedResolution = "1440x1080"
            My.Settings.Save()
        End If

        ' Add monitors to ComboBox3
        InitializeMonitorsComboBox()

        'Load In Version Info
        Dim currentVersionLong As Version = Assembly.GetExecutingAssembly().GetName().Version
        Dim currentVersionString As String = String.Format("{0}.{1}.{2}", currentVersionLong.Major, currentVersionLong.Minor, currentVersionLong.Build)

        If My.Settings.BetaBuild = True Then
            Label3.Text = "Version: " & currentVersionString & My.Settings.BetaLetter & " Beta"
            Label3.Location = New Point(85, 19)
        Else
            Label3.Text = "Version: " & currentVersionString
        End If

        ' Reset Textbox1 Colors since it's disabled
        TextBox1.BackColor = Color.White
        TextBox1.ForeColor = Color.Black

        'Tooltips for options
        Dim toolTip1 As New System.Windows.Forms.ToolTip With {
            .AutoPopDelay = 10000,
            .InitialDelay = 500,
            .ReshowDelay = 500,
            .ShowAlways = True
        }

        ' Valorant Specific Tooltips
        If My.Settings.SelectedGame = "Valorant" Then
            toolTip1.SetToolTip(Me.CheckBox1, "Will Always Be Applied If Widescreen Fix Is Enabled")
            toolTip1.SetToolTip(Me.CheckBox2, "Will Not Be Applied If Widescreen Fix Is Enabled")
        Else
        End If

        ' General Tooltips
        toolTip1.SetToolTip(Me.Button2, "Resets All Saved Monitor Settings & Rechecks Monitors")
        toolTip1.SetToolTip(Me.Label1, "Native Resolution Can Be Overrode Using '--OverrideNative WIDTHxHEIGHT' As a Start Argument")
        toolTip1.SetToolTip(Me.TextBox1, "Native Resolution Can Be Overrode Using '--OverrideNative WIDTHxHEIGHT' As a Start Argument")
        toolTip1.SetToolTip(Me.ComboBox2, "Any Resolution Can Be Entered As WIDTHxHEIGHT")

        ' Log Completed Gettings All Settings
        TrueLog("Info", "Completed Loading all settings for Settings Page")

        ' Add event handlers
        AddHandler ComboBox2.Leave, AddressOf ComboBox2_Leave
        AddHandler ComboBox3.DropDown, AddressOf ComboBox3_DropDown
        AddHandler ComboBox3.DropDownClosed, AddressOf ComboBox3_DropDownClosed

    End Sub

    Private Sub SettingsForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' Log Completed Saving All Settings - Closing Settings Page
        TrueLog("Info", "Saving all settings...")

        ' Save the user settigs
        My.Settings.AutoClose = CheckBox1.Checked
        My.Settings.AutoMinimize = CheckBox2.Checked
        My.Settings.SetDisplayResolution = CheckBox3.Checked
        My.Settings.RevertDisplayResolution = CheckBox4.Checked
        My.Settings.CheckForUpdateOnStart = CheckBox5.Checked
        My.Settings.StretchedResolution = ComboBox2.Text
        My.Settings.GameMonitor = GetSavedMonitor(ComboBox3.SelectedValue)
        My.Settings.MainGPU = ComboBox4.Text
        My.Settings.Save()

        ' Log Completed Saving All Settings - Closing Settings Page
        TrueLog("Info", "Completed Saving all settings, Closing Settings Page")

        ' Refocus Form1 on close
        Form1.Activate()

    End Sub

    Private Sub InitializeMonitorsComboBox()

        ' Clear ComboBox3 before repopulating
        ComboBox3.Items.Clear()
        ComboBox3.DisplayMember = "Key"
        ComboBox3.ValueMember = "Value"

        ' Repopulate ComboBox3 with the friendly names of saved monitors, including "(Primary)" where applicable
        Dim monitorsDict As New Dictionary(Of String, String)()

        Dim monitorsList As List(Of String) = GetSavedMonitors()
        For Each monitorInfo As String In monitorsList
            Dim properties As String() = monitorInfo.Split(";")
            Dim friendlyName As String = ""
            Dim deviceName As String = ""
            Dim isPrimary As Boolean = False
            For Each prop As String In properties
                Dim keyValue As String() = prop.Split(":")
                If keyValue.Length = 2 Then
                    Select Case keyValue(0).Trim().ToLower()
                        Case "friendlyname"
                            friendlyName = keyValue(1).Trim()
                        Case "devicename"
                            deviceName = keyValue(1).Trim()
                        Case "isprimary"
                            isPrimary = keyValue(1).Trim().Equals("True", StringComparison.OrdinalIgnoreCase)
                    End Select
                End If
            Next
            Dim displayName As String = friendlyName & If(isPrimary, " (Primary)", "")
            ' Avoid duplicates: append device name in parentheses for internal use if the friendly name is not unique
            If monitorsDict.ContainsKey(displayName) Then
                displayName &= " (" & deviceName & ")"
            End If
            monitorsDict(displayName) = deviceName
        Next

        ComboBox3.DataSource = New BindingSource(monitorsDict, Nothing)
        ComboBox3.DisplayMember = "Key"  ' The friendly name for display
        ComboBox3.ValueMember = "Value"  ' The device name for internal use

        ' Refresh ComboBox3 to display the updated list
        ComboBox3.Refresh()

        ' Set the selected monitor based on the DeviceName stored in My.Settings.GameMonitor
        Dim SavedGameMonitor As String = GetGameMonitor("DeviceName")
        If Not (IsNothing(SavedGameMonitor)) Then
            ComboBox3.SelectedValue = SavedGameMonitor
        Else
            Dim primarySelection As String = monitorsDict.FirstOrDefault(Function(kvp) kvp.Key.EndsWith(" (Primary)")).Value
            ComboBox3.SelectedValue = primarySelection
        End If

        ' Update TextBox1 Native Resolution Display (Allow Global Variable Override)
        If (OverrideNative) Then
            TextBox1.Text = OverrideNativeRes
        Else
            TextBox1.Text = GetGameMonitor("MaxResolution")
        End If

        ' Set FormLoading to False To Allow Gaming Monitor to be set by user
        FormLoading = False
        ComboBox3.Enabled = True

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

        'Disable Saving Fully Loaded (Avoids Saving Unnecessary)
        If FormLoading = False Then
            My.Settings.Save()
        End If

    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged

        My.Settings.RevertDisplayResolution = CheckBox4.Checked

        'Disable Saving Fully Loaded (Avoids Saving Unnecessary)
        If FormLoading = False Then
            My.Settings.Save()
        End If

    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged

        My.Settings.CheckForUpdateOnStart = CheckBox5.Checked

        'Disable Saving Fully Loaded (Avoids Saving Unnecessary)
        If FormLoading = False Then
            My.Settings.Save()
        End If

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Dim url As String = "https://truestretched.com"
        Dim psi As New ProcessStartInfo(url) With {
            .UseShellExecute = True
        }
        Process.Start(psi)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        Dim url As String = "https://www.youtube.com/@CodingCarson"
        Dim psi As New ProcessStartInfo(url) With {
            .UseShellExecute = True
        }
        Process.Start(psi)
    End Sub

    Private Sub ComboBox2_TextChanged(sender As Object, e As EventArgs) Handles ComboBox2.TextChanged

        ' Block Entering same value for stretched resolution as native resolution
        If ComboBox2.Text.Equals(TextBox1.Text) Then
            ComboBox2.Text = My.Settings.StretchedResolution
            ComboBox2.Focus()
        End If

    End Sub

    Private Sub ComboBox2_Leave(sender As Object, e As EventArgs)

        'Disable Saving Fully Loaded (Avoids Saving Unnecessary)
        If FormLoading = False Then

            ' Block Entering same value for stretched resolution as native resolution
            If ComboBox2.Text.Equals(TextBox1.Text) Then
                ComboBox2.Text = My.Settings.StretchedResolution
                ComboBox2.Focus()
            Else
                My.Settings.StretchedResolution = ComboBox2.Text
                My.Settings.Save()
            End If

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Clear any previously saved monitor information
        Dim primaryMonitorFriendlyName As String = ""
        My.Settings.Monitors = String.Empty
        My.Settings.GameMonitor = String.Empty
        My.Settings.Save()

        ' Save current connected monitor(s) information
        SaveMonitorInfo()

        ' Clear ComboBox3 before repopulating
        ComboBox3.DataSource = Nothing
        ComboBox3.Items.Clear()
        ComboBox3.DisplayMember = "Key"
        ComboBox3.ValueMember = "Value"

        ' Repopulate ComboBox3 with the friendly names of saved monitors, including "(Primary)" where applicable
        Dim monitorsDict As New Dictionary(Of String, String)()

        Dim monitorsList As List(Of String) = GetSavedMonitors()
        For Each monitorInfo As String In monitorsList
            Dim properties As String() = monitorInfo.Split(";")
            Dim friendlyName As String = ""
            Dim deviceName As String = ""
            Dim isPrimary As Boolean = False
            For Each prop As String In properties
                Dim keyValue As String() = prop.Split(":")
                If keyValue.Length = 2 Then
                    Select Case keyValue(0).Trim().ToLower()
                        Case "friendlyname"
                            friendlyName = keyValue(1).Trim()
                        Case "devicename"
                            deviceName = keyValue(1).Trim()
                        Case "isprimary"
                            isPrimary = keyValue(1).Trim().Equals("True", StringComparison.OrdinalIgnoreCase)
                    End Select
                End If
            Next
            Dim displayName As String = friendlyName & If(isPrimary, " (Primary)", "")
            ' Avoid duplicates: append device name in parentheses for internal use if the friendly name is not unique
            If monitorsDict.ContainsKey(displayName) Then
                displayName &= " (" & deviceName & ")"
            End If
            monitorsDict(displayName) = deviceName
        Next

        ComboBox3.DataSource = New BindingSource(monitorsDict, Nothing)
        ComboBox3.DisplayMember = "Key"  ' The friendly name for display
        ComboBox3.ValueMember = "Value"  ' The device name for internal use

        ' Refresh ComboBox3 to display the updated list
        ComboBox3.Refresh()

        ' Find and automatically select the primary monitor in ComboBox3
        Dim primarySelection As String = monitorsDict.FirstOrDefault(Function(kvp) kvp.Key.EndsWith(" (Primary)")).Value
        ComboBox3.SelectedValue = primarySelection

        ' Save Primary monitor to Game Monitor
        My.Settings.GameMonitor = GetPrimaryMonitor()

        ' GetPrimaryMonitor("Resolution") to update primary monitor's resolution correctly. (Allowing for Global Override Variable)
        If (OverrideNative) Then
            TextBox1.Text = OverrideNativeRes
        Else
            TextBox1.Text = GetPrimaryMonitor("MaxResolution")
        End If

        ' Save settings
        My.Settings.Save()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged

        'Disable Changing/Saving Gaming Monitor Till Fully Loaded (Avoids Resetting My.Settings.GameMonitor During Load)
        If FormLoading = False Then

            If ComboBox3.SelectedIndex = -1 Then Return

            Dim selectedDeviceName As String = CType(ComboBox3.SelectedValue, String)
            ' Retrieve full monitor information based on the selected device name
            Dim monitorFullInfo As String = GetSavedMonitor(selectedDeviceName)

            ' Update GameMonitor setting with the selected monitor's full information
            My.Settings.GameMonitor = monitorFullInfo
            My.Settings.Save()

            ' Update Native Display Resolution Textbox (Allow Global Variable Override)
            If (OverrideNative) Then
                TextBox1.Text = OverrideNativeRes
            Else
                TextBox1.Text = GetGameMonitor("MaxResolution")
            End If

        End If

    End Sub

    ' - Helpers For Ensuring Proper Function of HighLightMonitor Function -

    Private Sub ComboBox3_DropDown(sender As Object, e As EventArgs) Handles ComboBox3.DropDown
        ' Highlight Selected Monitor
        If ComboBox3.SelectedValue IsNot Nothing Then
            Dim monitorIdentifier As String = ComboBox3.SelectedValue
            lastSelectedIndex = ComboBox3.SelectedIndex
            HighlightMonitor(True, monitorIdentifier)

            ' Start HighlightCheckTimer to update highlighed monitor on mouse hover
            HighlightCheckTimer.Start()
        End If
    End Sub

    Private Sub HighlightCheckTimer_Tick(sender As Object, e As EventArgs) Handles HighlightCheckTimer.Tick

        If ComboBox3.DroppedDown Then

            ' Check if the index is valid and different from the currently hovered index
            If Not ComboBox3.SelectedIndex = lastSelectedIndex Then

                Dim monitorIdentifier As String = ComboBox3.SelectedValue
                HighlightMonitor(True, monitorIdentifier)

                lastSelectedIndex = ComboBox3.SelectedIndex
            End If
        Else
            ' Ensure Timer Stops & Highlight is Disabled if DropDown closes
            HighlightMonitor(False)
            lastSelectedIndex = -999
            HighlightCheckTimer.Stop()
        End If

    End Sub

    Private Sub ComboBox3_DropDownClosed(sender As Object, e As EventArgs) Handles ComboBox3.DropDownClosed
        ' Clear any existing highlights when the dropdown is closed
        HighlightMonitor(False)
        HighlightCheckTimer.Stop()
        lastSelectedIndex = -999
    End Sub
    ' - End of Helpers For Ensuring Proper Function of HighLightMonitor Function -

    Private Sub ComboBox4_TextChanged(sender As Object, e As EventArgs) Handles ComboBox4.TextChanged

        My.Settings.MainGPU = ComboBox4.Text

        'Disable Saving Fully Loaded (Avoids Saving Unnecessary)
        If FormLoading = False Then
            My.Settings.Save()
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        UpdateAvailable.Show()
    End Sub

    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click
        Dim appDataTrueStretchedPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "True Stretched")

        ' Open Folder In Appdata\Local containing log file
        Try
            ' Check if the directory exists
            If System.IO.Directory.Exists(appDataTrueStretchedPath) Then
                Process.Start("explorer.exe", appDataTrueStretchedPath)
            Else
                ' Log Foldor doesn't exist
                Console.WriteLine($"Log Folder Directory Doesn't exist: {appDataTrueStretchedPath}")
                TrueLog("Error", $"Log Folder Directory Doesn't exist: {appDataTrueStretchedPath}")
            End If
        Catch ex As Exception
            ' Log Error opening Appdata Log Folder Directory
            Console.WriteLine($"Error Opening Log Folder: {ex.Message}")
            TrueLog("Error", $"Error Opening Log Folder: {ex.Message}")
        End Try

    End Sub

End Class