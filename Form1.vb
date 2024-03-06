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

Public Class Form1
    Private Declare Function EnumWindows Lib "user32.dll" (ByVal lpEnumFunc As EnumWindowCallback, ByVal lParam As IntPtr) As Boolean
    Private Declare Function GetWindowText Lib "user32.dll" Alias "GetWindowTextA" (ByVal hWnd As IntPtr, ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
    Private Declare Function GetWindowTextLength Lib "user32.dll" Alias "GetWindowTextLengthA" (ByVal hWnd As IntPtr) As Integer
    Private Declare Function GetWindowThreadProcessId Lib "user32.dll" (ByVal hWnd As IntPtr, ByRef processId As Integer) As Integer
    Private Declare Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Integer
    Private Declare Function SetForegroundWindow Lib "user32.dll" (ByVal hWnd As IntPtr) As Integer
    Private Declare Function GetWindowLong Lib "user32.dll" Alias "GetWindowLongA" (ByVal hWnd As IntPtr, ByVal nIndex As Integer) As Integer
    Private Declare Function SetWindowLong Lib "user32.dll" Alias "SetWindowLongA" (ByVal hWnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    Private Declare Function SetWindowPos Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
    Private Declare Function GetClassName Lib "user32.dll" Alias "GetClassNameA" (ByVal hWnd As IntPtr, ByVal lpClassName As StringBuilder, ByVal nMaxCount As Integer) As Integer

    Private Delegate Function EnumWindowCallback(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000
    Private Const SWP_FRAMECHANGED As UInteger = &H20
    Private Const SWP_SHOWWINDOW As UInteger = &H40
    Private Const SWP_NOZORDER As UInteger = &H4
    Private Const SWP_NOSIZE As UInteger = &H1
    Private Const SW_MAXIMIZE As Integer = 3

    Dim Game As String = My.Settings.SelectedGame
    Dim GPU As String = My.Settings.MainGPU
    Dim StretchedEnabled As Boolean = False
    Dim IsWindowFound As Boolean = False
    Dim ApexMenuMatchFound As Boolean = False
    Dim AutoCloseCounter As Integer = 5
    Dim AutoMinimizeCounter As Integer = 5

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Check if the LastLocation is set in My.Settings
        If Not My.Settings.LastLocation.IsEmpty Then
            ' Load the last location from My.Settings
            Me.Location = My.Settings.LastLocation
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        ' Check For First Run
        If My.Settings.FirstRun = True Then
            FirstRun.Show()
        End If

        'Check If It's A Dev Build
        If My.Settings.DevBuild = True Then
            DevMenu.Show()
            Me.Text = "True Stretched (Dev)"
        ElseIf My.Settings.BetaBuild = True Then
            Me.Text = "True Stretched (Beta)"
        End If

        ' Ensure Native & Stretched Resolutions Don't match (Fixes "Disable True Stretched" being the only option)
        If My.Settings.NativeResolution = My.Settings.StretchedResolution Then
            My.Settings.NativeResolution = "1920x1080"
            My.Settings.StretchedResolution = "1440x1080"
            My.Settings.Save()
        End If

        'Check to see if opening long already enabled
        Dim MainScreen As Screen = Screen.PrimaryScreen
        Dim CurrentResolution As Size = MainScreen.Bounds.Size
        Dim CurrentResolutionConverted As String = CurrentResolution.Width & "x" & CurrentResolution.Height
        If CurrentResolutionConverted = My.Settings.StretchedResolution Then
            StretchedEnabled = True
            Button1.Text = "Disable True Stretched"
        End If

        'Load GPU Guide
        If GPU = "Nvidia GPU" Then
            GroupBox2.Text = "Nvidia Guide"
        ElseIf GPU = "Amd GPU" Then
            GroupBox2.Text = "Amd Guide"
        ElseIf GPU = "Intel GPU" Then
            GroupBox2.Text = "Intel Guide"
        Else
            GPU = "Nvidia GPU"
            GroupBox2.Text = "Nvidia Guide"
        End If

        'Load Last Selected Game & Guide
        Label4.Location = My.Settings.GameLabelLocation
        Label4.Text = Game
        If Game = "Apex Legends" Then 'Settings For Apex
            Me.BackgroundImage = My.Resources.Apex_Background
            GroupBox1.Text = "Apex Guide"
            LinkLabel1.Location = New Point(0, 65)
            LinkLabel1.Text = "https://TrueStretched.com/ApexLegends"
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        ElseIf Game = "Farlight 84" Then 'Settings For Farlight 84
            Me.BackgroundImage = My.Resources.Farlight84_Background
            GroupBox1.Text = "Farlight 84 Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Farlight84"
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        ElseIf Game = "Fortnite" Then 'Settings For Fortnite
            Me.BackgroundImage = My.Resources.Fortnight_Background
            GroupBox1.Text = "Fortnite Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Fortnite"
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

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
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        End If

        ' Set the groupbox backgrounds color to black with translucency
        Dim translucentBlack As Color = Color.FromArgb(128, 29, 29, 29)
        GroupBox1.BackColor = translucentBlack
        Label5.BackColor = Color.Transparent
        LinkLabel1.BackColor = Color.Transparent
        GroupBox2.BackColor = translucentBlack
        Label6.BackColor = Color.Transparent

        'Tooltips for Game Icons
        Dim toolTip1 As New Windows.Forms.ToolTip()
        toolTip1.AutoPopDelay = 5000
        toolTip1.InitialDelay = 1000
        toolTip1.ReshowDelay = 500
        toolTip1.ShowAlways = True

        toolTip1.SetToolTip(Me.ApexPictureBox, "Apex Legends")
        toolTip1.SetToolTip(Me.Farlight84PictureBox, "Farlight 84")
        toolTip1.SetToolTip(Me.FortnitePictureBox, "Fortnite")
        toolTip1.SetToolTip(Me.ValorantPictureBox, "Valorant")

        'Check for Updates on Startup
        If My.Settings.CheckForUpdateOnStart = True Then
            If My.Settings.DevBuild = False Then
                CheckForUpdates()
            End If
        End If

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' Save the form's location in My.Settings
        My.Settings.LastLocation = Me.Location
        My.Settings.Save()

    End Sub

    Private Async Sub CheckForUpdates()
        Dim httpClient As HttpClient = New HttpClient()
        Dim json As String = Await httpClient.GetStringAsync("https://download.truestretched.com/latestversion.json")
        Dim jsonObject As JObject = JObject.Parse(json)

        Dim serverVersionString As String = jsonObject("Version").ToString()
        Dim serverVersion As Version = New Version(serverVersionString)

        Dim currentVersion As Version = New Version(Application.ProductVersion)
        Dim currentVersionString As String = Application.ProductVersion
        If My.Settings.BetaBuild Then
            currentVersionString = currentVersionString & My.Settings.BetaLetter
        End If

        Dim serverBeta As Boolean = jsonObject("Beta").ToObject(Of Boolean)()
        Dim serverBetaLetter As String = jsonObject("BetaLetter").ToString()

        Dim skippedVersion As String = My.Settings.SkippedVersion

        If serverBeta Then
            If My.Settings.BetaBuild Then ' If user has opted in for beta updates
                serverVersionString = serverVersionString & serverBetaLetter ' Append beta letter to the server version
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

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click

        '---Apex Legends Mode Code Starts---
        If Game = "Apex Legends" Then
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                CheckForWindow("Apex Legends")
                If IsWindowFound = False Then
                    Dim url As String = "steam://rungameid/1172470"
                    Dim psi As New ProcessStartInfo(url)
                    psi.UseShellExecute = True

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        ChangeScreenResolutionStretched()
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
                    ChangeScreenResolutionNative()
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
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                CheckForWindow("Farlight 84")
                If IsWindowFound = False Then
                    Dim url As String = "steam://rungameid/1928420"
                    Dim psi As New ProcessStartInfo(url)
                    psi.UseShellExecute = True

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        ChangeScreenResolutionStretched()
                    End If
                    StretchedEnabled = True
                    EnableFarlight84Stretched()
                    Process.Start(psi)
                    If Label3.Text = "Game is not running!" Then
                        ChangeScreenResolutionNative()
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
                    ChangeScreenResolutionNative()
                End If
                DisableFarlight84Stretched()
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If
            '---Farlight 84 Mode Code Ends---

            '---Fortnite Mode Code Starts---
        ElseIf Game = "Fortnite" Then
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched

                CheckForWindow("Fortnite")
                If IsWindowFound = False Then
                    Dim url As String = "com.epicgames.launcher://apps/fn%3A4fe75bbc5a674f4f9b356b5c90567da5%3AFortnite?action=launch&silent=true"
                    Dim psi As New ProcessStartInfo(url)
                    psi.UseShellExecute = True

                    If My.Settings.SetDisplayResolution = True Then
                        ' Change the resolution of the screen
                        Label3.Text = "Changing Screen Resolution"
                        ChangeScreenResolutionStretched()
                    End If
                    StretchedEnabled = True
                    EnableFortniteStretched()
                    Process.Start(psi)
                    If Label3.Text = "Game is not running!" Then
                        ChangeScreenResolutionNative()
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
                    ChangeScreenResolutionNative()
                End If
                DisableFortniteStretched()
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If
            '---Fortnite Mode Code Ends---

            '---Valorant Mode Code Starts---
        ElseIf Game = "Valorant" Then
            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

            If Button1.Text = "Enable True Stretched" Then 'Enable True Stretched
                If My.Settings.SetDisplayResolution = True Then
                    ' Change the resolution of the screen (Unless using Wide Screen Fix then set to native)
                    '-Widescreen Fix-
                    If WidescreenFixCheckBox.Checked = True Then

                    Else
                        '-End of Widescreen Fix-
                        Label3.Text = "Changing Screen Resolution"
                        ChangeScreenResolutionStretched()
                        StretchedEnabled = True
                    End If
                End If
                RemoveBorderAndMaximizeValorant()
                If Label3.Text = "Game is not running!" Then
                    ChangeScreenResolutionNative()
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

            Else 'Disable True Stretched
                Button1.Text = "Enable True Stretched"
                If My.Settings.RevertDisplayResolution = True Then
                    ChangeScreenResolutionNative()
                End If
                Label3.ForeColor = Color.Green
                Label3.Text = "Successfully disabled True Stretched Res"
                StretchedEnabled = False
            End If
        End If
        '---Valorant Mode Code Ends---

    End Sub

    Public Sub CheckForWindow(windowName As String)
        Dim hwnd As IntPtr = FindWindow(Nothing, windowName)
        If hwnd = IntPtr.Zero Then
            IsWindowFound = False
        Else
            IsWindowFound = True
        End If
    End Sub

    Public Sub ApexMenuMatchWait()
        Task.Run(Sub()

                     While Not ApexMenuMatchFound
                         ' Capture the screen
                         Dim screenSize As Size = New Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
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
            Dim fileInfo As New FileInfo(Farlight84ConfigFilePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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

    Public Sub DisableFarlight84Stretched()
        Dim Farlight84ConfigFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Solarland\Saved\Config\WindowsClient\GameUserSettings.ini"

        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.NativeResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(Farlight84ConfigFilePath) Then
            ' Create a FileInfo object
            Dim fileInfo As New FileInfo(Farlight84ConfigFilePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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
            Dim fileInfo As New FileInfo(filePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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
        Dim resolution As String = My.Settings.NativeResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(filePath) Then
            ' Create a FileInfo object
            Dim fileInfo As New FileInfo(filePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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
            GetWindowText(hWnd, windowText, windowTextLength)

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
            GetWindowThreadProcessId(targetHWnd, processId)

            Dim process As Process = Process.GetProcessById(processId)

            If Not process.HasExited Then
                Dim windowStyle As Integer = GetWindowLong(targetHWnd, GWL_STYLE)
                windowStyle = windowStyle And Not WS_BORDER
                SetWindowLong(targetHWnd, GWL_STYLE, windowStyle)
                SetWindowPos(targetHWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_FRAMECHANGED Or SWP_SHOWWINDOW)

                ShowWindow(targetHWnd, SW_MAXIMIZE)

                Dim taskbarHWnd As IntPtr = GetTaskbarHWnd()
                SetWindowPos(targetHWnd, taskbarHWnd, -8, -8, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_FRAMECHANGED Or SWP_SHOWWINDOW)

                SetForegroundWindow(targetHWnd)
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
                GetClassName(hWnd, className, classNameLength)

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
            Dim fileInfo As New FileInfo(ApexConfigFilePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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

    Public Sub DisableApexStretched()
        Dim ApexConfigFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\Saved Games\Respawn\Apex\local\videoconfig.txt"

        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.NativeResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        If File.Exists(ApexConfigFilePath) Then
            ' Create a FileInfo object
            Dim fileInfo As New FileInfo(ApexConfigFilePath)

            ' Turn off read-only
            fileInfo.IsReadOnly = False

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

    <DllImport("user32.dll")>
    Private Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function

    Public Sub FullToWinToFullScreenApex()
        Dim hWnd As IntPtr = FindWindow(Nothing, "Apex Legends")
        If hWnd <> IntPtr.Zero Then
            SetForegroundWindow(hWnd)
            Threading.Thread.Sleep(3000) 'Wait 3 seconds
            SendKeys.SendWait("%{ENTER}") 'Alt+Enter to toggle fullscreen
            Threading.Thread.Sleep(100) 'Wait 100ms
            SendKeys.SendWait("^") 'Release Alt
            SetForegroundWindow(hWnd)
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

    Private Sub ChangeScreenResolutionStretched()
        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.StretchedResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        Dim devMode As New DEVMODE()
        devMode.dmSize = CShort(Marshal.SizeOf(devMode))
        devMode.dmPelsWidth = width
        devMode.dmPelsHeight = height
        devMode.dmFields = DM_PELSWIDTH Or DM_PELSHEIGHT

        ' Change the screen resolution
        Me.BeginInvoke(
        Sub()
            Dim result As Integer = ChangeDisplaySettings(devMode, 0)

            ' Check if the resolution change was successful
            If result = 0 Then
                ' Show a message box to indicate that the resolution has been changed
                Label3.ForeColor = Color.Green
                Label3.Text = "Screen resolution changed successfully!"
            Else
                ' Display an error message if the resolution change failed
                MessageBox.Show("Failed to change screen resolution!")
            End If
        End Sub
    )

    End Sub

    Private Sub ChangeScreenResolutionNative()
        ' Set the desired screen resolution
        Dim resolution As String = My.Settings.NativeResolution
        Dim dimensions() As String = resolution.Split("x"c)

        Dim width As Integer = Integer.Parse(dimensions(0))
        Dim height As Integer = Integer.Parse(dimensions(1))

        Dim devMode As New DEVMODE()
        devMode.dmSize = CShort(Marshal.SizeOf(devMode))
        devMode.dmPelsWidth = width
        devMode.dmPelsHeight = height
        devMode.dmFields = DM_PELSWIDTH Or DM_PELSHEIGHT

        ' Change the screen resolution
        Me.Invoke(
        Sub()
            Dim result As Integer = ChangeDisplaySettings(devMode, 0)

            ' Check if the resolution change was successful
            If result = 0 Then
                ' Successful if the resolution change failed
            Else
                ' Error message if the resolution change failed
            End If
        End Sub
    )
    End Sub

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
                checkWindowTimer.Start()
                AutoMinimizeCounter = 5
                Label3.Text = "Minimizing Now"
                Me.WindowState = FormWindowState.Minimized

            End If

        Else
            AutoMinimizeCounter -= 1
            Label3.Text = "Minimize In: " & AutoMinimizeCounter.ToString()

            If AutoMinimizeCounter < 0 Then
                AutoMinimizeTimer.Stop()
                checkWindowTimer.Start()
                AutoMinimizeCounter = 5
                Label3.Text = "Minimizing Now"
                Me.WindowState = FormWindowState.Minimized
            End If
        End If

    End Sub

    Private Sub checkWindowTimer_Tick(sender As Object, e As EventArgs) Handles checkWindowTimer.Tick

        '--Apex Section---
        If Game = "Apex Legends" Then
            Dim targetWindowTitle As String = "Apex Legends"
            Dim targetHWnd As IntPtr = IntPtr.Zero

            EnumWindows(
                Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                    Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
                    Dim windowText As New StringBuilder(windowTextLength)
                    GetWindowText(hWnd, windowText, windowTextLength)

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
                GetWindowThreadProcessId(targetHWnd, processId)

                Dim process As Process = Process.GetProcessById(processId)

                If process.HasExited Then
                    ' "Apex Legends" window is closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Apex Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If My.Settings.DevBuild = True Then
                        DevMenu.Show()
                    End If
                    checkWindowTimer.Stop()
                End If
            Else
                ' "Apex Legends" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Apex Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If My.Settings.DevBuild = True Then
                    DevMenu.Show()
                End If
                checkWindowTimer.Stop()
            End If

            '--Fortnite Section---
        ElseIf Game = "Fortnite" Then
            Dim targetWindowTitle As String = "Fortnite"
            Dim targetHWnd As IntPtr = IntPtr.Zero

            EnumWindows(
                Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                    Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
                    Dim windowText As New StringBuilder(windowTextLength)
                    GetWindowText(hWnd, windowText, windowTextLength)

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
                GetWindowThreadProcessId(targetHWnd, processId)

                Dim process As Process = Process.GetProcessById(processId)

                If process.HasExited Then
                    ' "Fortnite" window is closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Fortnite Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If My.Settings.DevBuild = True Then
                        DevMenu.Show()
                    End If
                    checkWindowTimer.Stop()
                End If
            Else
                ' "Fortnite" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Fortnite Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If My.Settings.DevBuild = True Then
                    DevMenu.Show()
                End If
                checkWindowTimer.Stop()
            End If

            '--Valorant Section---
        ElseIf Game = "Valorant" Then
            Dim targetWindowTitle As String = "VALORANT"
            Dim targetHWnd As IntPtr = IntPtr.Zero

            EnumWindows(
                Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                    Dim windowTextLength As Integer = GetWindowTextLength(hWnd) + 1
                    Dim windowText As New StringBuilder(windowTextLength)
                    GetWindowText(hWnd, windowText, windowTextLength)

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
                GetWindowThreadProcessId(targetHWnd, processId)

                Dim process As Process = Process.GetProcessById(processId)

                If process.HasExited Then
                    ' "VALORANT" window is closed, Unminimize Form1 And Change Label3
                    Label3.Text = "Valorant Closed, Unminimized"
                    Me.WindowState = FormWindowState.Normal
                    If My.Settings.DevBuild = True Then
                        DevMenu.Show()
                    End If
                    checkWindowTimer.Stop()
                End If
            Else
                ' "VALORANT" window is not found, Unminimize Form1 And Change Label3
                Label3.Text = "Valorant Closed, Unminimized"
                Me.WindowState = FormWindowState.Normal
                If My.Settings.DevBuild = True Then
                    DevMenu.Show()
                End If
                checkWindowTimer.Stop()
            End If
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        If Game = "Apex Legends" Then
            Dim url As String = "https://truestretched.com/ApexLegends"
            Dim psi As New ProcessStartInfo(url)
            psi.UseShellExecute = True
            Process.Start(psi)
        ElseIf Game = "Farlight 84" Then
            Dim url As String = "https://truestretched.com/Farlight84"
            Dim psi As New ProcessStartInfo(url)
            psi.UseShellExecute = True
            Process.Start(psi)
        ElseIf Game = "Fortnite" Then
            Dim url As String = "https://truestretched.com/Fortnite"
            Dim psi As New ProcessStartInfo(url)
            psi.UseShellExecute = True
            Process.Start(psi)
        ElseIf Game = "Valorant" Then
            Dim url As String = "https://truestretched.com/Valorant"
            Dim psi As New ProcessStartInfo(url)
            psi.UseShellExecute = True
            Process.Start(psi)
        End If

    End Sub

    Private Sub ApexPictureBox_Click(sender As Object, e As EventArgs) Handles ApexPictureBox.Click

        GroupBox3.Visible = False

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

            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        Else
        End If

    End Sub

    Private Sub Farlight84PictureBox_Click(sender As Object, e As EventArgs) Handles Farlight84PictureBox.Click

        GroupBox3.Visible = False

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

            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        Else
        End If

    End Sub

    Private Sub FortnitePictureBox_Click(sender As Object, e As EventArgs) Handles FortnitePictureBox.Click

        GroupBox3.Visible = False

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

            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        Else
        End If

    End Sub

    Private Sub ValorantPictureBox_Click(sender As Object, e As EventArgs) Handles ValorantPictureBox.Click

        If StretchedEnabled = False Then
            Game = "Valorant"
            Button1.Text = "Enable True Stretched"
            Label4.Location = New Point(75, 11)
            Label4.Text = Game
            GroupBox1.Text = "Valorant Guide"
            LinkLabel1.Location = New Point(6, 65)
            LinkLabel1.Text = "https://TrueStretched.com/Valorant"
            Me.BackgroundImage = My.Resources.Valorant_Background
            My.Settings.SelectedGame = Game
            My.Settings.GameLabelLocation = Label4.Location
            My.Settings.Save()

            If GPU = "Amd GPU" Then
                GroupBox2.Visible = False
            ElseIf GPU = "Nvidia GPU" Then
                GroupBox2.Visible = True
                Label6.Text = "One Time Only: Open the Nvidia" & vbCrLf & "Control Panel and set the scaling mode" & vbCrLf & "to ""Full-screen"" on the monitor you are" & vbCrLf & "using for the game"
            End If

        Else
        End If

        '-Valorant Widescreen Fix
        GroupBox3.Visible = True

        If My.Settings.ValorantWidescreenFix = True Then
            WidescreenFixCheckBox.Checked = True
            Button1.Text = "Enable Widescreen Fix"
        End If
        '-End of Valorant Widescreen Fix

    End Sub

    Private Sub WidescreenFixCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles WidescreenFixCheckBox.CheckedChanged

        '-Valorant Widescreen Fix Checkbox
        My.Settings.ValorantWidescreenFix = WidescreenFixCheckBox.Checked
        My.Settings.Save()

    End Sub
End Class