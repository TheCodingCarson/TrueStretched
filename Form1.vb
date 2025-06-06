﻿Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Threading
Imports Newtonsoft.Json.Linq
Imports System.Net.Http
Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports Emgu.CV.Structure
Imports System.Reflection

Public Class Form1
    Private Declare Function EnumWindows Lib "user32.dll" (lpEnumFunc As EnumWindowCallback, lParam As IntPtr) As Boolean
    <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="GetWindowTextW")>
    Private Shared Function GetWindowText(hWnd As IntPtr, lpString As StringBuilder, nMaxCount As Integer) As Integer
    End Function
    Private Declare Function GetWindowTextLength Lib "user32.dll" Alias "GetWindowTextLengthA" (hWnd As IntPtr) As Integer
    Private Declare Function GetWindowThreadProcessId Lib "user32.dll" (hWnd As IntPtr, ByRef processId As Integer) As Integer
    Private Declare Function ShowWindow Lib "user32.dll" (hWnd As IntPtr, nCmdShow As Integer) As Integer
    Private Declare Function SetForegroundWindow Lib "user32.dll" (hWnd As IntPtr) As Integer
    Private Declare Function GetWindowLong Lib "user32.dll" Alias "GetWindowLongA" (hWnd As IntPtr, nIndex As Integer) As Integer
    Private Declare Function SetWindowLong Lib "user32.dll" Alias "SetWindowLongA" (hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
    Private Declare Function SetWindowPos Lib "user32.dll" (hWnd As IntPtr, hWndInsertAfter As IntPtr, X As Integer, Y As Integer, cx As Integer, cy As Integer, uFlags As UInteger) As Boolean
    <DllImport("user32.dll", CharSet:=CharSet.Unicode, EntryPoint:="GetClassNameW")>
    Private Shared Function GetClassName(hWnd As IntPtr, lpClassName As StringBuilder, nMaxCount As Integer) As Integer
    End Function

    Private Delegate Function EnumWindowCallback(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000
    Private Const SWP_FRAMECHANGED As UInteger = &H20
    Private Const SWP_SHOWWINDOW As UInteger = &H40
    Private Const SWP_NOZORDER As UInteger = &H4
    Private Const SWP_NOSIZE As UInteger = &H1
    Private Const SW_MAXIMIZE As Integer = 3
    Private Const SW_MINIMIZE As Integer = 6
    Private Const SW_SHOWMINNOACTIVE As Integer = 7

    Dim Game As String = My.Settings.SelectedGame
    Dim GPU As String = My.Settings.MainGPU
    Dim StretchedEnabled As Boolean = False
    Dim ApexMenuMatchFound As Boolean = False
    Dim AutoCloseCounter As Integer = 5
    Dim AutoMinimizeCounter As Integer = 5
    Dim RightClickedGame As String = ""

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Check if the LastLocation is set in My.Settings and the form is fully visible on any screen
        Dim fixedFormSize As New Size(316, 572) ' Fixed size of the form
        Dim formRectangle As New Rectangle(My.Settings.LastLocation, fixedFormSize)
        Dim isFormFullyVisible As Boolean = False

        If Not My.Settings.LastLocation.IsEmpty Then
            ' Iterate through all screens to check if the form is fully visible on any screen
            For Each scr In Screen.AllScreens
                ' Check if the formRectangle is within the screen's bounds considering the fixed size
                If scr.Bounds.IntersectsWith(formRectangle) Then
                    ' Further check if the form's entire size is within the screen's working area
                    If scr.WorkingArea.Contains(formRectangle) Then
                        isFormFullyVisible = True
                        Exit For
                    End If
                End If
            Next

            ' If the form is fully visible on a screen, load its last location; otherwise, use WindowsDefaultLocation
            If isFormFullyVisible Then
                Me.Location = My.Settings.LastLocation
            Else
                Me.StartPosition = FormStartPosition.WindowsDefaultLocation
            End If
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        'Check If It's A Dev Build
        If DevBuild = True Then
            DevMenu.Show()
            Me.Text = "True Stretched (Dev)"
        ElseIf My.Settings.BetaBuild = True Then
            Me.Text = "True Stretched (Beta " + My.Settings.BetaLetter.ToUpper() + ")"
        End If

        'Only Check for Saved Monitor Information if program has completed first run
        If My.Settings.FirstRun = False Then

            ' Ensure Native & Stretched Resolutions Don't match (Fixes "Disable True Stretched" being the only option)
            If GetGameMonitor("Resolution") = My.Settings.StretchedResolution Then
                My.Settings.StretchedResolution = "1440x1080"
                My.Settings.Save()
            Else
            End If

            'Check to see if opening long already enabled
            If GetMonitorResolution(, GetGameMonitor("DeviceName")) = My.Settings.StretchedResolution Then
                StretchedEnabled = True
                Button1.Text = "Disable True Stretched"
            End If

        End If

        'Load Last Selected Game & Guide
        Label4.Location = My.Settings.GameLabelLocation
        Label4.Text = Game
        If Game = "Apex Legends" Then 'Settings For Apex
            Me.BackgroundImage = My.Resources.Apex_Background
            GroupBox1.Text = "Apex Guide"
            LinkLabel1.Location = New Point(0, 65)
            LinkLabel1.Text = "https://TrueStretched.com/ApexLegends"

        ElseIf Game = "Farlight 84" Then 'Settings For Farlight 84
            Me.BackgroundImage = My.Resources.Farlight84_Background
            GroupBox1.Text = "Farlight 84 Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Farlight84"

        ElseIf Game = "Fortnite" Then 'Settings For Fortnite
            Me.BackgroundImage = My.Resources.Fortnight_Background
            GroupBox1.Text = "Fortnite Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Fortnite"

        ElseIf Game = "Valorant" Then 'Settings For Valorant
            Me.BackgroundImage = My.Resources.Valorant_Background
            GroupBox1.Text = "Valorant Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Valorant"
            GroupBox3.Visible = True
            '-Valorant Widescreen Fix
            If My.Settings.ValorantWidescreenFix = True Then
                WidescreenFixCheckBox.Checked = True
                Button1.Text = "Enable Widescreen Fix"
            End If
            '-End of Valorant Widescreen Fix

        ElseIf Game = "XDefiant" Then 'Settings For XDefiant
            Me.BackgroundImage = My.Resources.XDefiant_Background
            GroupBox1.Text = "XDefiant Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/XDefiant"

        End If

        ' Set the groupbox backgrounds color to black with translucency
        Dim translucentBlack As Color = Color.FromArgb(128, 29, 29, 29)
        GroupBox1.BackColor = translucentBlack
        GroupBox3.BackColor = translucentBlack
        Label5.BackColor = Color.Transparent
        LinkLabel1.BackColor = Color.Transparent
        WidescreenFixCheckBox.BackColor = Color.Transparent

        'Tooltips Settings
        Dim toolTip1 As New System.Windows.Forms.ToolTip With {
            .AutoPopDelay = 10000,
            .InitialDelay = 500,
            .ReshowDelay = 500,
            .ShowAlways = True
        }

        'Tooltips for Game Icons
        toolTip1.SetToolTip(Me.ApexPictureBox, "Apex Legends")
        toolTip1.SetToolTip(Me.Farlight84PictureBox, "Farlight 84")
        toolTip1.SetToolTip(Me.FortnitePictureBox, "Fortnite")
        toolTip1.SetToolTip(Me.ValorantPictureBox, "Valorant")
        toolTip1.SetToolTip(Me.XDefiantPictureBox, "XDefiant")

        'General Tooltips
        toolTip1.SetToolTip(Me.PictureBox2, "Settings")
        toolTip1.SetToolTip(Me.WidescreenFixCheckBox, "Enabling This Options **DISABLES** Stretching The Resolution!!!")

        'Check for Updates on Startup (Make Sure Application Has Network Access)
        If My.Settings.CheckForUpdateOnStart = True Then
            If DevBuild = False Then
                If InternetConnection() = True Then
                    CheckForUpdates()
                End If
            End If
        End If

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown ' Code to run after Form1 has loaded fully

        ' Check For First Run
        If My.Settings.FirstRun = True Then

            ' Save Form1 Location Before Showing First Run
            My.Settings.LastLocation = Me.Location
            My.Settings.Save()

            ' Show First Run Dialog
            FirstRun.Show()
        End If

        ' Auto Disable True Stretched if command line argument exists
        If AutoDisable = True AndAlso StretchedEnabled = True Then
            Button1.PerformClick()
        End If

        ' Auto Launch Stretched Game is command line argument exists
        If AutoStretch = True Then
            If StretchedEnabled Then
                ' If Stretched is already enabled check for AutoDisable Argument
                If StretchedEnabled AndAlso AutoDisable Then
                    ' Do nothing
                End If
            Else
                ' Auto Enable Stretched from specified game in argument
                Button1.PerformClick()
            End If
        End If

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        ' Check if the form is being minimized
        If Me.WindowState = FormWindowState.Minimized Then
            ' Use ShowWindow to minimize without focus
            ShowWindow(Me.Handle, SW_SHOWMINNOACTIVE)
        End If

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' Check if the Form1 Location is fully visible on any screen
        Dim fixedFormSize As New Size(316, 572) ' Fixed size of the form
        Dim formRectangle As New Rectangle(Me.Location, fixedFormSize)
        Dim isFormFullyVisible As Boolean = False

        ' Iterate through all screens to check if the form is fully visible on any screen
        For Each scr In Screen.AllScreens
            ' Check if the formRectangle is within the screen's bounds considering the fixed size
            If scr.Bounds.IntersectsWith(formRectangle) Then
                ' Further check if the form's entire size is within the screen's working area
                If scr.WorkingArea.Contains(formRectangle) Then
                    isFormFullyVisible = True
                    Exit For
                End If
            End If
        Next

        ' If the form is fully visible on a screen, save its location; otherwise, don't save
        If isFormFullyVisible Then
            My.Settings.LastLocation = Me.Location
            My.Settings.Save()
        End If

        ' Send Closing Log Message
        TrueLog("Close", "--True Stretched Closing--")

    End Sub

    Private Async Sub CheckForUpdates()
        Dim httpClient As New HttpClient()
        Dim json As String = Await httpClient.GetStringAsync("https://download.truestretched.com/latestversion.json")
        Dim jsonObject As JObject = JObject.Parse(json)

        Dim serverVersionString As String = jsonObject("Version").ToString()
        Dim serverVersion As New Version(serverVersionString)

        Dim currentVersionLong As Version = Assembly.GetExecutingAssembly().GetName().Version
        Dim currentVersion As New Version(String.Format("{0}.{1}.{2}", currentVersionLong.Major, currentVersionLong.Minor, currentVersionLong.Build))
        Dim currentVersionString As String = String.Format("{0}.{1}.{2}", currentVersionLong.Major, currentVersionLong.Minor, currentVersionLong.Build)
        If My.Settings.BetaBuild Then
            currentVersionString &= My.Settings.BetaLetter
        End If

        Dim serverBeta As Boolean = jsonObject("Beta").ToObject(Of Boolean)()
        Dim serverBetaLetter As String = jsonObject("BetaLetter").ToString()

        Dim skippedVersion As String = My.Settings.SkippedVersion

        If serverBeta Then
            If My.Settings.BetaBuild Then ' If user has opted in for beta updates
                serverVersionString &= serverBetaLetter ' Append beta letter to the server version
            Else ' If user has not opted in for beta updates
                Return ' Do not prompt for update
            End If
        End If

        ' Check if the server version (with or without beta letter) is the same as the skipped version
        If serverVersionString.Equals(skippedVersion) Then
            Return ' Do not prompt for update
        End If

        ' If server version is greater than the current version, prompt for update
        If serverVersion > currentVersion Then
            UpdateAvailable.Show()
        ElseIf serverVersion = currentVersion AndAlso serverBeta AndAlso serverVersionString > currentVersionString Then
            UpdateAvailable.Show()
        End If
    End Sub

    Private Async Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click

        ' Get Stretched in format needed
        Dim StretchedResolution = ParseResolution(My.Settings.StretchedResolution)

        ' Get Native Resoultion in format needed (Allow Global Variable Override)
        Dim NativeResolution As (Width As Integer, Height As Integer)
        If (OverrideNative) Then
            NativeResolution = ParseResolution(OverrideNativeRes)
        Else
            NativeResolution = ParseResolution(GetGameMonitor("MaxResolution"))
        End If

        '---Apex Legends Mode Code Starts---
        If Game = "Apex Legends" Then

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                If Not CheckForWindow("Apex Legends") Then
                    Dim url As String = "steam://rungameid/1172470"
                    Dim psi As New ProcessStartInfo(url) With {
                        .UseShellExecute = True
                    }

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        SetMonitorResolution(GetGameMonitor("DeviceName"), StretchedResolution.Width, StretchedResolution.Height)
                    End If
                    StretchedEnabled = True
                    EnableApexStretched()
                    Process.Start(psi)

                    Label3.Text = "Waiting For Continue Menu"
                    ApexMenuMatchFound = False
                    ApexMenuMatchWait()
                Else
                    MessageBox.Show("Please Close Apex Legends Before Enabling", "True Stretched - Error")
                End If

            Else 'Disable True Stretched
                Button1.Text = "Enable True Stretched"
                If My.Settings.RevertDisplayResolution = True Then
                    SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                End If
                DisableApexStretched()
                ApexMenuMatchFound = True
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If

            '---Apex Legends Mode Code Ends---

            '---Farlight 84 Mode Code Starts---
        ElseIf Game = "Farlight 84" Then

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                If Not CheckForWindow("Farlight 84") Then
                    Dim url As String = "steam://rungameid/1928420"
                    Dim psi As New ProcessStartInfo(url) With {
                        .UseShellExecute = True
                    }

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        SetMonitorResolution(GetGameMonitor("DeviceName"), StretchedResolution.Width, StretchedResolution.Height)
                    End If
                    StretchedEnabled = True
                    EnableFarlight84Stretched()
                    Process.Start(psi)
                    If Label3.Text = "Game is not running!" Then
                        SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                        DisableFarlight84Stretched()
                        Label3.ForeColor = Color.Red
                        Label3.Text = "Game is not running!"
                    Else
                        Label3.ForeColor = Color.Green
                        Label3.Text = "Successfully enabled True Stretched Res"

                        If My.Settings.AutoClose = True Then
                            AutoCloseTimer.Start()
                        ElseIf My.Settings.AutoMinimize = True Then
                            AutoMinimizeTimer.Start()
                        End If

                    End If
                Else
                    MessageBox.Show("Please Close Farlight 84 Before Enabling", "True Stretched - Error")
                End If

            Else 'Disable True Stretched
                Button1.Text = "Enable True Stretched"
                If My.Settings.RevertDisplayResolution = True Then
                    SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                End If
                DisableFarlight84Stretched()
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If
            '---Farlight 84 Mode Code Ends---

            '---Fortnite Mode Code Starts---
        ElseIf Game = "Fortnite" Then

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                If Not CheckForWindow("Fortnite") Then
                    Dim url As String = "com.epicgames.launcher://apps/fn%3A4fe75bbc5a674f4f9b356b5c90567da5%3AFortnite?action=launch&silent=true"
                    Dim psi As New ProcessStartInfo(url) With {
                        .UseShellExecute = True
                    }

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        SetMonitorResolution(GetGameMonitor("DeviceName"), StretchedResolution.Width, StretchedResolution.Height)
                    End If
                    StretchedEnabled = True
                    EnableFortniteStretched()
                    Process.Start(psi)
                    If Label3.Text = "Game is not running!" Then
                        SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                        DisableFortniteStretched()
                        Label3.ForeColor = Color.Red
                        Label3.Text = "Game is not running!"
                    Else
                        Label3.ForeColor = Color.Green
                        Label3.Text = "Successfully enabled True Stretched Res"

                        If My.Settings.AutoClose = True Then
                            AutoCloseTimer.Start()
                        ElseIf My.Settings.AutoMinimize = True Then
                            AutoMinimizeTimer.Start()
                        End If
                    End If
                Else
                    MessageBox.Show("Please Close Fortnite Before Enabling", "True Stretched - Error")
                End If

            Else 'Disable True Stretched
                Button1.Text = "Enable True Stretched"
                If My.Settings.RevertDisplayResolution = True Then
                    SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                End If
                DisableFortniteStretched()
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If
            '---Fortnite Mode Code Ends---

            '---Valorant Mode Code Starts---
        ElseIf Game = "Valorant" Then

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                ' Disable Enable Button Long Starting Valorant
                Button1.Text = "Enabling True Stretched"
                Button1.Enabled = False

                If Await EnableValorantStretched() Then
                    ' Update Status Label
                    Label3.ForeColor = Color.Green
                    Label3.Text = "Valorant True Stretched Enabled!"

                    ' Update Button1
                    Button1.Text = "Disable True Stretched"

                    ' Auto Minimize or Auto Close is User has the option enabled
                    If My.Settings.AutoClose = True Then
                        AutoCloseTimer.Start()
                    ElseIf My.Settings.AutoMinimize = True Then
                        AutoMinimizeTimer.Start()
                    End If
                Else
                    ' Update Status Label
                    Label3.ForeColor = Color.Red
                    Label3.Text = "Error Stretching Valorant!"
                End If
            Else 'Disable True Stretched
                ' Disable Enable Button Long Starting Valorant
                Button1.Text = "Enable True Stretched"
                Button1.Enabled = False

                If (DisableValorantStretched()) Then
                    ' Update Status Label
                    Label3.ForeColor = Color.Green
                    Label3.Text = "Valorant True Stretched Disabled!"
                End If
            End If
            '---Valorant Mode Code Ends---

            '---XDefiant Mode Code Starts---
        ElseIf Game = "XDefiant" Then

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                ' Log XDefiant Start of Enabling Stretched to File
                TrueLog("Info", "Enabling XDefiant Stretched...")

                If Not CheckForWindow("XDefiant") Then
                    Dim url As String = "uplay://launch/15657/0"
                    Dim psi As New ProcessStartInfo(url) With {
                        .UseShellExecute = True
                    }

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"

                        ' Log Screen Res Changing
                        TrueLog("Info", "Changing Monitor Resolution...")

                        ' Change Screen res
                        SetMonitorResolution(GetGameMonitor("DeviceName"), StretchedResolution.Width, StretchedResolution.Height)
                    End If
                    StretchedEnabled = True
                    Process.Start(psi)
                    Await CountdownTimer(30, True, True)

                    If Label3.Text = "Game is not running!" Then
                        SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)
                        Label3.ForeColor = Color.Red
                        Label3.Text = "Game is not running!"

                        ' Log Error
                        TrueLog("Error", "Stretching XDefiant Failed - Game is not running!")
                    Else
                        Label3.ForeColor = Color.Green
                        Label3.Text = "Successfully enabled True Stretched Res"

                        ' Log Stretch Success
                        TrueLog("Info", "Successfully enabled True Stretched Res - Stretching Complete!")

                        If My.Settings.AutoClose = True Then
                            AutoCloseTimer.Start()
                        ElseIf My.Settings.AutoMinimize = True Then
                            AutoMinimizeTimer.Start()
                        End If
                    End If
                Else
                    MessageBox.Show("Please Close XDefiant Before Enabling", "True Stretched - Error")
                End If

            Else 'Disable True Stretched

                ' Log Disabling XDefiant Stretched to File
                TrueLog("Info", "Disabling XDefiant True Stretched Res...")

                Button1.Text = "Enable True Stretched"
                If My.Settings.RevertDisplayResolution = True Then
                    SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)

                    ' Log Reverting Monitor Resolution to File
                    TrueLog("Info", "Reverting Monitor Resolution Completed")
                End If
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False

                ' Log Disable Success
                TrueLog("Info", "Successfully Disabled XDefiant True Stretched Res!")
            End If
            '---XDefiant Mode Code Ends---

        End If

        ' Renable Main Button
        Button1.Enabled = True

    End Sub

    Public Sub ApexMenuMatchWait()
        Task.Run(Sub()

                     While Not ApexMenuMatchFound
                         ' Capture the screen
                         Dim screenSize As New Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
                         Dim screenImage As New Bitmap(screenSize.Width, screenSize.Height)
                         Using g As Graphics = Graphics.FromImage(screenImage)
                             g.CopyFromScreen(New Point(0, 0), New Point(0, 0), screenSize)
                         End Using

                         ' Convert the Bitmap to a BitmapData object
                         Dim bmpData As System.Drawing.Imaging.BitmapData = screenImage.LockBits(New Rectangle(0, 0, screenImage.Width, screenImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, screenImage.PixelFormat)

                         ' Create an Emgu.CV.Image from the BitmapData
                         Dim sourceImage As New Image(Of Bgr, Byte)(screenImage.Width, screenImage.Height, bmpData.Stride, bmpData.Scan0)

                         ' Unlock the bits of the original Bitmap
                         screenImage.UnlockBits(bmpData)

                         ' Convert the template image to a BitmapData object
                         If Screen.PrimaryScreen.Bounds.Width = "1440" AndAlso Screen.PrimaryScreen.Bounds.Height = "1080" Then ' For 1440x1080
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template_1440x1080

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()

                         ElseIf Screen.PrimaryScreen.Bounds.Width = "1280" AndAlso Screen.PrimaryScreen.Bounds.Height = "1024" Then ' For 1280x1024
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template_1280x1024

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()

                         ElseIf Screen.PrimaryScreen.Bounds.Width = "1280" AndAlso Screen.PrimaryScreen.Bounds.Height = "960" Then ' For 1280x960
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template_1280x960

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()

                         ElseIf Screen.PrimaryScreen.Bounds.Width = "1024" AndAlso Screen.PrimaryScreen.Bounds.Height = "768" Then ' For 1024x768
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template_1024x768

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()

                         ElseIf Screen.PrimaryScreen.Bounds.Width = "800" AndAlso Screen.PrimaryScreen.Bounds.Height = "600" Then ' For 800x600
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template_800x600

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()

                         Else 'Backup to 1920x1080
                             Dim templateBitmap As Bitmap = My.Resources.Apex_Continue_Screen_Button_Template

                             Dim templateData As System.Drawing.Imaging.BitmapData = templateBitmap.LockBits(New Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, templateBitmap.PixelFormat)

                             ' Create an Emgu.CV.Image from the BitmapData
                             Dim templateImage As New Image(Of Bgr, Byte)(templateBitmap.Width, templateBitmap.Height, templateData.Stride, templateData.Scan0)

                             ' Unlock the bits of the template Bitmap
                             templateBitmap.UnlockBits(templateData)

                             ' Convert the images to grayscale
                             Dim sourceGray As Image(Of Gray, Byte) = sourceImage.Convert(Of Gray, Byte)()
                             Dim templateGray As Image(Of Gray, Byte) = templateImage.Convert(Of Gray, Byte)()

                             ' Dispose of the Bitmap objects
                             screenImage.Dispose()
                             templateBitmap.Dispose()

                             ' Create the result matrix
                             Dim result As New Mat()

                             ' Perform template matching
                             CvInvoke.MatchTemplate(sourceGray, templateGray, result, TemplateMatchingType.CcoeffNormed)

                             ' Find the minimum and maximum values and their locations
                             Dim minVal As Double, maxVal As Double
                             Dim minLoc As Point, maxLoc As Point
                             CvInvoke.MinMaxLoc(result, minVal, maxVal, minLoc, maxLoc)

                             ' If Match is found
                             If maxVal > 0.45 Then

                                 ' Set ApexMenuMatchFound to True to exit the loop
                                 ApexMenuMatchFound = True

                                 Label3.Invoke(Sub() Label3.Text = "On Continue Menu - Fixing Bars")
                                 FullToWinToFullScreenApex()

                                 If My.Settings.AutoClose = True Then
                                     Me.Invoke(Sub() AutoCloseTimer.Start())
                                 ElseIf My.Settings.AutoMinimize = True Then
                                     Me.Invoke(Sub() AutoMinimizeTimer.Start())
                                 End If
                             End If

                             ' Add a delay to prevent high CPU usage
                             Thread.Sleep(100)  ' Sleep for 100 milliseconds

                             ' Dispose of the images and the result matrix to free up memory
                             sourceImage.Dispose()
                             templateImage.Dispose()
                             sourceGray.Dispose()
                             templateGray.Dispose()
                             result.Dispose()
                         End If

                     End While
                 End Sub)
    End Sub

    Public Sub EnableFarlight84Stretched()
        Dim Farlight84ConfigFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Solarland\Saved\Config\WindowsClient\GameUserSettings.ini"

        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.StretchedResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(Farlight84ConfigFilePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(Farlight84ConfigFilePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(Farlight84ConfigFilePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("FullscreenMode=") Then
                    lines(i) = "FullscreenMode=0"
                ElseIf lines(i).StartsWith("LastConfirmedFullscreenMode=") Then
                    lines(i) = "LastConfirmedFullscreenMode=0"
                ElseIf lines(i).StartsWith("PreferredFullscreenMode=") Then
                    lines(i) = "PreferredFullscreenMode=1"
                ElseIf lines(i).StartsWith("ResolutionSizeX=") Then
                    lines(i) = "ResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("ResolutionSizeY=") Then
                    lines(i) = "ResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeX=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeY=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("DesiredScreenWidth=") Then
                    lines(i) = "DesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("DesiredScreenHeight=") Then
                    lines(i) = "DesiredScreenHeight=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenWidth=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenHeight=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenHeight=" & height
                End If
            Next

            File.WriteAllLines(Farlight84ConfigFilePath, lines.ToArray())

            ' Set the file back to read-only
            fileInfo.IsReadOnly = True
        End If

        Button1.Text = "Disable True Stretched"
    End Sub

    Public Shared Sub DisableFarlight84Stretched()
        Dim Farlight84ConfigFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Solarland\Saved\Config\WindowsClient\GameUserSettings.ini"

        ' Set the desired screen resolution
        Dim resolution As String = GetGameMonitor("Resolution")
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(Farlight84ConfigFilePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(Farlight84ConfigFilePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(Farlight84ConfigFilePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("FullscreenMode=") Then
                    lines(i) = "FullscreenMode=0"
                ElseIf lines(i).StartsWith("LastConfirmedFullscreenMode=") Then
                    lines(i) = "LastConfirmedFullscreenMode=0"
                ElseIf lines(i).StartsWith("PreferredFullscreenMode=") Then
                    lines(i) = "PreferredFullscreenMode=1"
                ElseIf lines(i).StartsWith("ResolutionSizeX=") Then
                    lines(i) = "ResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("ResolutionSizeY=") Then
                    lines(i) = "ResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeX=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeY=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("DesiredScreenWidth=") Then
                    lines(i) = "DesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("DesiredScreenHeight=") Then
                    lines(i) = "DesiredScreenHeight=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenWidth=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenHeight=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenHeight=" & height
                End If
            Next

            File.WriteAllLines(Farlight84ConfigFilePath, lines.ToArray())
        End If
    End Sub

    Private Sub EnableFortniteStretched()
        Dim filePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortniteGame/Saved/Config/WindowsClient/GameUserSettings.ini")

        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.StretchedResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(filePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(filePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(filePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("LastConfirmedFullscreenMode=") Then
                    lines(i) = "LastConfirmedFullscreenMode=0"
                ElseIf lines(i).StartsWith("PreferredFullscreenMode=") Then
                    lines(i) = "PreferredFullscreenMode=0"
                ElseIf lines(i).StartsWith("ResolutionSizeX=") Then
                    lines(i) = "ResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("ResolutionSizeY=") Then
                    lines(i) = "ResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeX=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeY=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("DesiredScreenWidth=") Then
                    lines(i) = "DesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("DesiredScreenHeight=") Then
                    lines(i) = "DesiredScreenHeight=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenWidth=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenHeight=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenHeight=" & height
                End If
            Next

            File.WriteAllLines(filePath, lines.ToArray())

            ' Set the file back to read-only
            fileInfo.IsReadOnly = True
        End If

        Button1.Text = "Disable True Stretched"
    End Sub

    Private Sub DisableFortniteStretched()
        Dim filePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortniteGame/Saved/Config/WindowsClient/GameUserSettings.ini")

        ' Set the desired screen resolution
        Dim resolution As String = GetGameMonitor("Resolution")
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(filePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(filePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(filePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith("ResolutionSizeX=") Then
                    lines(i) = "ResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("ResolutionSizeY=") Then
                    lines(i) = "ResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeX=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeX=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeY=") Then
                    lines(i) = "LastUserConfirmedResolutionSizeY=" & height
                ElseIf lines(i).StartsWith("DesiredScreenWidth=") Then
                    lines(i) = "DesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("DesiredScreenHeight=") Then
                    lines(i) = "DesiredScreenHeight=" & height
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenWidth=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenWidth=" & width
                ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenHeight=") Then
                    lines(i) = "LastUserConfirmedDesiredScreenHeight=" & height
                End If
            Next

            File.WriteAllLines(filePath, lines.ToArray())
        End If
    End Sub

    Private Sub RemoveBorderAndMaximizeValorant()

        Dim targetWindowTitle As String = "VALORANT"
        Dim targetHWnd As IntPtr = IntPtr.Zero

        EnumWindows(
        Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
            Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
            Dim windowText As New StringBuilder(windowTextLength)
            Dim unused1 = GetWindowText(hWnd, windowText, windowTextLength)

            If windowText.ToString().Contains(targetWindowTitle) Then
                targetHWnd = hWnd
                Return False ' Stop enumerating
            End If

            Return True ' Continue enumerating
        End Function,
        IntPtr.Zero
    )

        If targetHWnd <> IntPtr.Zero Then
            Dim processId As Integer = 0
            Dim unused2 = GetWindowThreadProcessId(targetHWnd, processId)

            Dim process As Process = Process.GetProcessById(processId)

            If Not process.HasExited Then
                Dim windowStyle As Integer = GetWindowLong(targetHWnd, GWL_STYLE)
                windowStyle = windowStyle And Not WS_BORDER
                Dim unused4 = SetWindowLong(targetHWnd, GWL_STYLE, windowStyle)
                SetWindowPos(targetHWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_FRAMECHANGED Or SWP_SHOWWINDOW)

                Dim unused = ShowWindow(targetHWnd, SW_MAXIMIZE)

                Dim taskbarHWnd As IntPtr = GetTaskbarHWnd()
                SetWindowPos(targetHWnd, taskbarHWnd, -8, -8, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_FRAMECHANGED Or SWP_SHOWWINDOW)

                Dim unused3 = SetForegroundWindow(targetHWnd)
            End If

            '-Widescreen Fix-
            If WidescreenFixCheckBox.Checked = True Then

            Else
                '-End of Widescreen Fix-
                Button1.Text = "Disable True Stretched"
            End If

        Else
            Label3.ForeColor = Color.Red
            Label3.Text = "Game is not running!"
        End If
    End Sub

    Private Function GetTaskbarHWnd() As IntPtr
        Dim taskbarHWnd As IntPtr = IntPtr.Zero
        EnumWindows(
            Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                Dim classNameLength As Integer = 256
                Dim className As New StringBuilder(classNameLength)
                Dim unused = GetClassName(hWnd, className, classNameLength)

                If className.ToString() = "Shell_TrayWnd" Then
                    taskbarHWnd = hWnd
                    Return False ' Stop enumerating
                End If

                Return True ' Continue enumerating
            End Function,
            IntPtr.Zero
        )
        Return taskbarHWnd
    End Function

    Public Sub EnableApexStretched()
        Dim ApexConfigFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\Saved Games\Respawn\Apex\local\videoconfig.txt"

        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.StretchedResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(ApexConfigFilePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(ApexConfigFilePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(ApexConfigFilePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith(vbTab & """setting.fullscreen""") Then
                    lines(i) = vbTab & """setting.fullscreen""" & vbTab & vbTab & """1"""
                ElseIf lines(i).StartsWith(vbTab & """setting.nowindowborder""") Then
                    lines(i) = vbTab & """setting.nowindowborder""" & vbTab & vbTab & """0"""
                ElseIf lines(i).StartsWith(vbTab & """setting.defaultres""") Then
                    lines(i) = vbTab & """setting.defaultres""" & vbTab & vbTab & """" & width & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.defaultresheight""") Then
                    lines(i) = vbTab & """setting.defaultresheight""" & vbTab & vbTab & """" & height & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.last_display_width""") Then
                    lines(i) = vbTab & """setting.last_display_width""" & vbTab & vbTab & """" & width & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.last_display_height""") Then
                    lines(i) = vbTab & """setting.last_display_height""" & vbTab & vbTab & """" & height & """"
                End If
            Next

            File.WriteAllLines(ApexConfigFilePath, lines.ToArray())

            ' Set the file back to read-only
            fileInfo.IsReadOnly = True
        End If

        Button1.Text = "Disable True Stretched"
    End Sub

    Public Shared Sub DisableApexStretched()
        Dim ApexConfigFilePath As String = FindConfigLocation("Apex Legends")

        ' Set the desired screen resolution
        Dim resolution As String = GetGameMonitor("Resolution")
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(ApexConfigFilePath) Then
            ' Create a FileInfo object
            ' Turn off read-only
            Dim fileInfo As New FileInfo(ApexConfigFilePath) With {
                .IsReadOnly = False
            }

            Dim lines As List(Of String) = File.ReadAllLines(ApexConfigFilePath).ToList()

            For i As Integer = 0 To lines.Count - 1
                If lines(i).StartsWith(vbTab & """setting.fullscreen""") Then
                    lines(i) = vbTab & """setting.fullscreen""" & vbTab & vbTab & """1"""
                ElseIf lines(i).StartsWith(vbTab & """setting.nowindowborder""") Then
                    lines(i) = vbTab & """setting.nowindowborder""" & vbTab & vbTab & """0"""
                ElseIf lines(i).StartsWith(vbTab & """setting.defaultres""") Then
                    lines(i) = vbTab & """setting.defaultres""" & vbTab & vbTab & """" & width & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.defaultresheight""") Then
                    lines(i) = vbTab & """setting.defaultresheight""" & vbTab & vbTab & """" & height & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.last_display_width""") Then
                    lines(i) = vbTab & """setting.last_display_width""" & vbTab & vbTab & """" & width & """"
                ElseIf lines(i).StartsWith(vbTab & """setting.last_display_height""") Then
                    lines(i) = vbTab & """setting.last_display_height""" & vbTab & vbTab & """" & height & """"
                End If
            Next

            File.WriteAllLines(ApexConfigFilePath, lines.ToArray())
        End If
    End Sub

    Public Shared Sub FullToWinToFullScreenApex()
        Dim hWnd As IntPtr = FindWindow(Nothing, "Apex Legends")
        If hWnd <> IntPtr.Zero Then
            Dim unused = SetForegroundWindow(hWnd)
            Threading.Thread.Sleep(3000) 'Wait 3 seconds
            SendKeys.SendWait("%{ENTER}") 'Alt+Enter to toggle fullscreen
            Threading.Thread.Sleep(100) 'Wait 100ms
            SendKeys.SendWait("^") 'Release Alt
            Dim unused1 = SetForegroundWindow(hWnd)
            Threading.Thread.Sleep(3000) 'Wait 3 seconds
            SendKeys.SendWait("%{ENTER}") 'Alt+Enter to toggle fullscreen again
            Threading.Thread.Sleep(100) 'Wait 100ms
            SendKeys.SendWait("^") 'Release Alt
        End If
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

        ' Save the form's location in My.Settings
        My.Settings.LastLocation = Me.Location
        My.Settings.Save()

        SettingsForm.Show()

    End Sub

    Private Sub PictureBox2_MouseEnter(sender As Object, e As EventArgs) Handles PictureBox2.MouseEnter
        PictureBox2.BackColor = Color.FromArgb(45, 45, 45)
    End Sub

    Private Sub PictureBox2_MouseLeave(sender As Object, e As EventArgs) Handles PictureBox2.MouseLeave
        ' Reset the background color when the mouse leaves the PictureBox
        PictureBox2.BackColor = Color.FromArgb(29, 29, 29)
    End Sub

    <DllImport("user32.dll")>
    Private Shared Function ChangeDisplaySettings(ByRef lpDevMode As DEVMODE, ByVal dwFlags As Integer) As Integer
    End Function

    Private Const DM_PELSWIDTH As Integer = &H80000
    Private Const DM_PELSHEIGHT As Integer = &H100000

    Private Structure DEVMODE
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmPositionX As Integer
        Public dmPositionY As Integer
        Public dmDisplayOrientation As Integer
        Public dmDisplayFixedOutput As Integer
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmFormName As String
        Public dmLogPixels As Short
        Public dmBitsPerPel As Integer
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
    End Structure

    Private Sub AutoCloseTimer_Tick(sender As Object, e As EventArgs) Handles AutoCloseTimer.Tick

        AutoCloseCounter -= 1
        Label3.Text = "Closing In: " & AutoCloseCounter.ToString()

        If AutoCloseCounter < 0 Then
            AutoCloseTimer.Stop()
            AutoCloseCounter = 5
            Label3.Text = "Closing Now"
            Me.Close()
        End If
    End Sub

    Private Sub AutoMinimizeTimer_Tick(sender As Object, e As EventArgs) Handles AutoMinimizeTimer.Tick

        If Game = "Fortnite" Then 'Fortnite needs a delay to allow user to open Fortnite (Future figure out better way)
            AutoMinimizeCounter -= 1
            Label3.Text = "Minimize In: " & AutoMinimizeCounter.ToString()
            Thread.Sleep(5000)

            If AutoMinimizeCounter < 0 Then
                AutoMinimizeTimer.Stop()
                CheckWindowTimer.Start()
                AutoMinimizeCounter = 5
                Label3.Text = "Minimizing Now"
                Me.WindowState = FormWindowState.Minimized

            End If

        Else
            AutoMinimizeCounter -= 1
            Label3.Text = "Minimize In: " & AutoMinimizeCounter.ToString()

            If AutoMinimizeCounter < 0 Then
                AutoMinimizeTimer.Stop()
                CheckWindowTimer.Start()
                AutoMinimizeCounter = 5
                Label3.Text = "Minimizing Now"
                Me.WindowState = FormWindowState.Minimized
            End If
        End If

    End Sub

    Private Sub CheckWindowTimer_Tick(sender As Object, e As EventArgs) Handles CheckWindowTimer.Tick

        '--Apex Section---
        If Game = "Apex Legends" Then
            Dim targetWindowTitle As String = "Apex Legends"
            Dim targetHWnd As IntPtr = IntPtr.Zero

            EnumWindows(
                Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                    Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
                    Dim windowText As New StringBuilder(windowTextLength)
                    Dim unused = GetWindowText(hWnd, windowText, windowTextLength)

                    If windowText.ToString().Contains(targetWindowTitle) Then
                        targetHWnd = hWnd
                        Return False ' Stop enumerating
                    End If

                    Return True ' Continue enumerating
                End Function,
                IntPtr.Zero
            )

            If targetHWnd <> IntPtr.Zero Then
                Dim processId As Integer = 0
                Dim unused3 = GetWindowThreadProcessId(targetHWnd, processId)

                Dim process As Process = Process.GetProcessById(processId)

                If process.HasExited Then
                    ' "Apex Legends" window is closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Apex Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If DevBuild = True Then
                        DevMenu.Show()
                    End If
                    CheckWindowTimer.Stop()
                End If
            Else
                ' "Apex Legends" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Apex Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If DevBuild = True Then
                    DevMenu.Show()
                End If
                CheckWindowTimer.Stop()
            End If

            '--Fortnite Section---
        ElseIf Game = "Fortnite" Then
            Dim targetWindowTitle As String = "Fortnite"
            Dim targetHWnd As IntPtr = IntPtr.Zero

            EnumWindows(
                Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                    Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
                    Dim windowText As New StringBuilder(windowTextLength)
                    Dim unused1 = GetWindowText(hWnd, windowText, windowTextLength)

                    If windowText.ToString().Contains(targetWindowTitle) Then
                        targetHWnd = hWnd
                        Return False ' Stop enumerating
                    End If

                    Return True ' Continue enumerating
                End Function,
                IntPtr.Zero
            )

            If targetHWnd <> IntPtr.Zero Then
                Dim processId As Integer = 0
                Dim unused4 = GetWindowThreadProcessId(targetHWnd, processId)

                Dim process As Process = Process.GetProcessById(processId)

                If process.HasExited Then
                    ' "Fortnite" window is closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Fortnite Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If DevBuild = True Then
                        DevMenu.Show()
                    End If
                    CheckWindowTimer.Stop()
                End If
            Else
                ' "Fortnite" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Fortnite Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If DevBuild = True Then
                    DevMenu.Show()
                End If
                CheckWindowTimer.Stop()
            End If

            '--Valorant Section---
        ElseIf Game = "Valorant" Then
            If ValorantProcess IsNot Nothing Then

                If ValorantProcess.HasExited Then
                    ' "VALORANT" process has exited closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Valorant Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If DevBuild = True Then
                        DevMenu.Show()
                    End If
                    ValorantProcess = Nothing ' Set Process to 'Nothing'
                    CheckWindowTimer.Stop()
                End If
            Else
                ' "VALORANT" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Valorant Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If DevBuild = True Then
                    DevMenu.Show()
                End If
                ValorantProcess = Nothing ' Set Process to 'Nothing'
                CheckWindowTimer.Stop()

                ' "VALORANT" - Disable True Stretched Automatically
                If Button1.Text = "Disable True Stretched" Then
                    Button1.PerformClick()
                End If
            End If
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        If Game = "Apex Legends" Then
            Dim url As String = "https://truestretched.com/ApexLegends"
            Dim psi As New ProcessStartInfo(url) With {
                .UseShellExecute = True
            }
            Process.Start(psi)
        ElseIf Game = "Farlight 84" Then
            Dim url As String = "https://truestretched.com/Farlight84"
            Dim psi As New ProcessStartInfo(url) With {
                .UseShellExecute = True
            }
            Process.Start(psi)
        ElseIf Game = "Fortnite" Then
            Dim url As String = "https://truestretched.com/Fortnite"
            Dim psi As New ProcessStartInfo(url) With {
                .UseShellExecute = True
            }
            Process.Start(psi)
        ElseIf Game = "Valorant" Then
            Dim url As String = "https://truestretched.com/Valorant"
            Dim psi As New ProcessStartInfo(url) With {
                .UseShellExecute = True
            }
            Process.Start(psi)
        ElseIf Game = "XDefiant" Then
            Dim url As String = "https://truestretched.com/XDefiant"
            Dim psi As New ProcessStartInfo(url) With {
                .UseShellExecute = True
            }
            Process.Start(psi)
        End If

    End Sub

    Private Sub ApexPictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles ApexPictureBox.MouseClick

        If e.Button = MouseButtons.Left Then '===Left Click===

            GroupBox3.Visible = False ' Valorant Extra Options Groupbox

            If StretchedEnabled = False Then
                Game = "Apex Legends"
                Button1.Text = "Enable True Stretched"
                Label4.Location = New Point(45, 11)
                Label4.Text = Game
                GroupBox1.Text = "Apex Guide"
                LinkLabel1.Location = New Point(0, 65)
                LinkLabel1.Text = "https://TrueStretched.com/ApexLegends"
                Me.BackgroundImage = My.Resources.Apex_Background
                My.Settings.SelectedGame = Game
                My.Settings.GameLabelLocation = Label4.Location
                My.Settings.Save()

            Else
            End If

        ElseIf e.Button = MouseButtons.Right Then '===Right Click===
            RightClickedGame = "Apex Legends"
            GameBoxContextMenuStrip.Show(MousePosition)
        End If

    End Sub

    Private Sub Farlight84PictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles Farlight84PictureBox.MouseClick

        If e.Button = MouseButtons.Left Then '===Left Click===

            GroupBox3.Visible = False ' Valorant Extra Options Groupbox

            If StretchedEnabled = False Then
                Game = "Farlight 84"
                Button1.Text = "Enable True Stretched"
                Label4.Location = New Point(75, 11)
                Label4.Text = Game
                GroupBox1.Text = "Farlight 84 Guide"
                LinkLabel1.Location = New Point(6, 65)
                LinkLabel1.Text = "https://TrueStretched.com/Farlight84"
                Me.BackgroundImage = My.Resources.Farlight84_Background
                My.Settings.SelectedGame = Game
                My.Settings.GameLabelLocation = Label4.Location
                My.Settings.Save()

            Else
            End If

        ElseIf e.Button = MouseButtons.Right Then '===Right Click===
            RightClickedGame = "Farlight 84"
            GameBoxContextMenuStrip.Show(MousePosition)
        End If

    End Sub

    Private Sub FortnitePictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles FortnitePictureBox.MouseClick

        If e.Button = MouseButtons.Left Then '===Left Click===

            GroupBox3.Visible = False ' Valorant Extra Options Groupbox

            If StretchedEnabled = False Then
                Game = "Fortnite"
                Button1.Text = "Enable True Stretched"
                Label4.Location = New Point(75, 11)
                Label4.Text = Game
                GroupBox1.Text = "Fortnite Guide"
                LinkLabel1.Location = New Point(6, 65)
                LinkLabel1.Text = "https://TrueStretched.com/Fortnite"
                Me.BackgroundImage = My.Resources.Fortnight_Background
                My.Settings.SelectedGame = Game
                My.Settings.GameLabelLocation = Label4.Location
                My.Settings.Save()

            Else
            End If

        ElseIf e.Button = MouseButtons.Right Then '===Right Click===
            RightClickedGame = "Fortnite"
            GameBoxContextMenuStrip.Show(MousePosition)
        End If



    End Sub

    Private Sub ValorantPictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles ValorantPictureBox.MouseClick

        If e.Button = MouseButtons.Left Then '===Left Click===

            If StretchedEnabled = False Then
                Game = "Valorant"
                Button1.Text = "Enable True Stretched"
                Label4.Location = New Point(75, 11)
                Label4.Text = Game
                GroupBox1.Text = "Valorant Guide"
                LinkLabel1.Location = New Point(6, 65)
                LinkLabel1.Text = "https://TrueStretched.com/Valorant"
                BackgroundImage = My.Resources.Valorant_Background
                My.Settings.SelectedGame = Game
                My.Settings.GameLabelLocation = Label4.Location
                My.Settings.Save()

            Else
            End If

            '-Valorant Widescreen Fix
            GroupBox3.Visible = True

            '-Valorant Widescreen Fix checkbox
            If My.Settings.ValorantWidescreenFix = True Then
                WidescreenFixCheckBox.Checked = True
                Button1.Text = "Enable Widescreen Fix"
            End If
            '-End of Valorant Widescreen Fix

        ElseIf e.Button = MouseButtons.Right Then '===Right Click===
            RightClickedGame = "Valorant"
            GameBoxContextMenuStrip.Show(MousePosition)
        End If

    End Sub

    Private Sub WidescreenFixCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles WidescreenFixCheckBox.CheckedChanged

        '-Valorant Widescreen Fix Checkbox
        My.Settings.ValorantWidescreenFix = WidescreenFixCheckBox.Checked
        My.Settings.Save()

        If My.Settings.ValorantWidescreenFix = True Then
            WidescreenFixCheckBox.Checked = True
            Button1.Text = "Enable Widescreen Fix"
        Else
            WidescreenFixCheckBox.Checked = False
            Button1.Text = "Enable True Stretched"
        End If

    End Sub

    Private Sub XDefiantPictureBox_MouseClick(sender As Object, e As MouseEventArgs) Handles XDefiantPictureBox.MouseClick

        If e.Button = MouseButtons.Left Then '===Left Click===

            GroupBox3.Visible = False ' Valorant Extra Options Groupbox

            If StretchedEnabled = False Then
                Game = "XDefiant"
                Button1.Text = "Enable True Stretched"
                Label4.Location = New Point(75, 11)
                Label4.Text = Game
                GroupBox1.Text = "XDefiant Guide"
                LinkLabel1.Location = New Point(6, 65)
                LinkLabel1.Text = "https://TrueStretched.com/XDefiant"
                Me.BackgroundImage = My.Resources.XDefiant_Background
                My.Settings.SelectedGame = Game
                My.Settings.GameLabelLocation = Label4.Location
                My.Settings.Save()

            Else
            End If

        ElseIf e.Button = MouseButtons.Right Then '===Right Click===
            RightClickedGame = "XDefiant"
            GameBoxContextMenuStrip.Show(MousePosition)
        End If

    End Sub

    Private Sub CreateDesktopShortcutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateDesktopShortcutToolStripMenuItem.Click

        ' Create Desktop Shortcut
        CreateDesktopShortcut(RightClickedGame)

        ' Status Label Update
        Label3.ForeColor = Color.White
        Label3.Text = RightClickedGame + " Shortcut Created"

        ' Clear RightClickedGame Variable
        RightClickedGame = ""

    End Sub
End Class