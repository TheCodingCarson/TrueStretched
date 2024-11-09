Imports System.IO
Imports System.Windows.Forms

Module Valorant_Games
    ' {-Summary-}
    '
    ' Used to enable true stretched resolution in Valorant after Patch 9.09 (https://playvalorant.com/en-us/news/game-updates/valorant-patch-notes-9-09)
    '
    ' Usage:
    '        • EnableValorantStretched()  - Enables Valorant Stretched
    '        • DisableValorantStretched() - Disables Valorant Stretched
    '        • FixValorantUltraWide()     - Fixes Side/Top Black Bars on Ultra Wide Monitors in Valorant
    '
    ' Return:
    '         • EnableValorantStretched() | DisableValorantStretched() -- Returns Boolean True/False if succesfully stretched
    '         • FixValorantUltraWide()                                 -- Returns Boolean True/False if widescreen fix was successful
    '
    ' Notes: Game needs to be closed before running!
    '
    ' {-Summary-}
    Public ValorantProcess As Process = Nothing

    Public Async Function EnableValorantStretched() As Task(Of Boolean)
        ' Get Stretched in format needed
        Dim StretchedResolution = ParseResolution(My.Settings.StretchedResolution)

        ' Get Native Resoultion in format needed (Allow Global Variable Override)
        Dim NativeResolution As (Width As Integer, Height As Integer)
        If (OverrideNative) Then
            NativeResolution = ParseResolution(OverrideNativeRes)
        Else
            NativeResolution = ParseResolution(GetGameMonitor("MaxResolution"))
        End If

        ' Log Valorant Start of Enabling Stretched to File
        TrueLog("Info", "Enabling Valorant Stretched...")

        If Not CheckForWindow("VALORANT") Then
            If My.Settings.SetDisplayResolution = True Then
                ' Change the resolution of the screen
                Form1.Label3.Text = "Changing Screen Resolution"
                ' Log to File
                TrueLog("Info", "Changing Monitor Resolution...")

                SetMonitorResolution(GetGameMonitor("DeviceName"), StretchedResolution.Width, StretchedResolution.Height)
                StretchedEnabled = True

                TrueLog("Info", "Changing Monitor Completed")
            End If

            ' Switch Config Files For Stretched
            TrueLog("Info", "Valorant isn't running proceeding")
            Form1.Label3.Text = "Switching Valorant Config"
            ValorantConfigFileSwitch()

            ' Valorant Launch Parameters
            Dim valstartInfo As New ProcessStartInfo()
            Dim valinstallLocation As String = FindInstallLocation("Valorant")
            Dim valexeLocation As String = valinstallLocation + "\RiotClientServices.exe"
            valstartInfo.FileName = valexeLocation
            valstartInfo.WorkingDirectory = valinstallLocation
            valstartInfo.Arguments = "--launch-product=valorant --launch-patchline=live"
            valstartInfo.WindowStyle = ProcessWindowStyle.Normal
            valstartInfo.UseShellExecute = True
            Form1.Label3.Text = "Starting Valorant"

            ' Launch Valorant
            ValorantProcess = Process.Start(valstartInfo)

            ' Check if Valorant started successfully
            Dim WaitTime As Integer = 60
            Dim ValStarted As Boolean = False
            Dim WaitTimeExpired As Boolean = False
            Do
                ' Update Status Label
                Form1.Label3.Text = "Waiting for Valorant: " + WaitTime.ToString() + "s"

                If WaitTime = 0 Then
                    ' Valorant Failed to Open in Time
                    WaitTimeExpired = True
                Else
                    WaitTime -= 1

                    ' Check for Valorant Process
                    If ValorantProcess IsNot Nothing AndAlso Not ValorantProcess.HasExited Then
                        ' Valorant Is Running
                        ValStarted = True
                        ' End While Loop
                        WaitTimeExpired = True
                    End If
                End If

                Await Task.Delay(1000) ' Wait 1 Second Before looping
            Loop Until WaitTimeExpired

            If ValStarted Then
                ' Log Valorant Successful Start to File
                TrueLog("Info", "Valorant Started Successfully")

                ' Return True for Succesfully Enabled Stretch
                Return True

            Else ' Valorant Failed to Open in Time

                ' Log Failed to Start Valorant to File
                TrueLog("Error", "Valorant Failed to Start after 60 seconds")

                ' Reutrn to Native Resolution
                SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)

                ' Update Status Label
                Form1.Label3.ForeColor = Color.Red
                Form1.Label3.Text = "Valorant failed to start!"

                ' Return False for Failed to Stretch
                Return False

            End If

        Else
            ' Ask User to close Valorant
            MessageBox.Show("Please close Valorant before running True Stretched!")

            ' Log Error to File
            TrueLog("Error", "Valorant already running, canceling.")

            ' Return False for Failed to Stretch
            Return False
        End If

    End Function

    Public Function DisableValorantStretched() As Boolean
        ' Get Stretched in format needed
        Dim StretchedResolution = ParseResolution(My.Settings.StretchedResolution)

        ' Get Native Resoultion in format needed (Allow Global Variable Override)
        Dim NativeResolution As (Width As Integer, Height As Integer)
        If (OverrideNative) Then
            NativeResolution = ParseResolution(OverrideNativeRes)
        Else
            NativeResolution = ParseResolution(GetGameMonitor("MaxResolution"))
        End If

        ' Log Disabling Valorant Stretched to File
        TrueLog("Info", "Disabling Valorant True Stretched Res...")

        If My.Settings.RevertDisplayResolution = True Then
            SetMonitorResolution(GetGameMonitor("DeviceName"), NativeResolution.Width, NativeResolution.Height)

            ' Log Reverting Monitor Resolution to File
            TrueLog("Info", "Reverting Monitor Resolution Completed")
        End If

        ' Log Updating Config File to File
        TrueLog("Info", "Updating Valorant Config File...")
        ValorantConfigFileSwitch()
        TrueLog("Info", "Completed Updating Valorant Config File!")
        StretchedEnabled = False

        ' Stop Check For Window Timer if Running
        If Form1.CheckWindowTimer.Enabled Then
            Form1.CheckWindowTimer.Stop()
        End If

        ' Reset Valorant Process to 'Nothing'
        ValorantProcess = Nothing

        ' Log Completed Disabling Valorant Stretched to File
        TrueLog("Info", "Completed Disabling Valorant True Stretched Res!")

        ' Return True for Succesfully Disabled Stretch
        Return True

    End Function

    ' Helper Function - Sets the Last Known User's configuration file
    Private Sub ValorantConfigFileSwitch()

        ' Get Stretched in format needed
        Dim StretchedResolution = ParseResolution(My.Settings.StretchedResolution)

        ' Valorant Get Config Location (Handle Errors)
        Dim filePath As String = FindConfigLocation("Valorant")
        If filePath = "'Valorant' not found in configuration paths." OrElse
        filePath = "'Valorant Last Riot User ID' not found in 'RiotLocalMachine.ini'" OrElse
        filePath = "'Valorant Last Riot User ID' configuration folder not found." Then
            ' Valorant Last User Riot ID Config File ERROR
            Form1.Label3.ForeColor = Color.Red
            Form1.Label3.Text = filePath
        Else
            ' Valorant Last User Riot ID Config File Found
            ' Set the desired screen resolution
            Dim width As String = StretchedResolution.Width
            Dim height As String = StretchedResolution.Height
            Dim UseLetterbox As String = "False"
            Dim FullscreenMode As String = "2"
            Dim PreferredFullScreenMode As String = "0"
            Dim UseDynamicResolution As String = "False"
            Dim LastRecommendedScreenWidthHeight As String = "-1.000000"
            Dim DesiredScreenWidth As String = "1280"
            Dim DesiredScreenHeight As String = "720"
            Dim UseDesiredScreenHeight As String = "False"
            Dim WindowPos As String = "0"

            If File.Exists(filePath) Then
                ' Create a FileInfo object
                Dim fileInfo As New FileInfo(filePath)

                Dim lines As List(Of String) = File.ReadAllLines(filePath).ToList()

                ' Track if fullscreen line exists
                Dim fullscreenModeExists As Boolean = False

                For i As Integer = 0 To lines.Count - 1

                    If lines(i).StartsWith("FullscreenMode=") Then
                        lines(i) = "FullscreenMode=" & FullscreenMode
                        fullscreenModeExists = True
                    ElseIf lines(i).StartsWith("LastConfirmedFullscreenMode=") Then
                        lines(i) = "LastConfirmedFullscreenMode=" & FullscreenMode
                    ElseIf lines(i).StartsWith("PreferredFullscreenMode=") Then
                        lines(i) = "PreferredFullscreenMode=" & PreferredFullScreenMode
                    ElseIf lines(i).StartsWith("bShouldLetterbox=") Then
                        lines(i) = "bShouldLetterbox=" & UseLetterbox
                    ElseIf lines(i).StartsWith("bLastConfirmedShouldLetterbox=") Then
                        lines(i) = "bLastConfirmedShouldLetterbox=" & UseLetterbox
                    ElseIf lines(i).StartsWith("bUseDynamicResolution=") Then
                        lines(i) = "bUseDynamicResolution=" & UseDynamicResolution
                    ElseIf lines(i).StartsWith("ResolutionSizeX=") Then
                        lines(i) = "ResolutionSizeX=" & width
                    ElseIf lines(i).StartsWith("ResolutionSizeY=") Then
                        lines(i) = "ResolutionSizeY=" & height
                    ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeX=") Then
                        lines(i) = "LastUserConfirmedResolutionSizeX=" & width
                    ElseIf lines(i).StartsWith("LastUserConfirmedResolutionSizeY=") Then
                        lines(i) = "LastUserConfirmedResolutionSizeY=" & height
                    ElseIf lines(i).StartsWith("DesiredScreenWidth=") Then
                        lines(i) = "DesiredScreenWidth=" & DesiredScreenWidth
                    ElseIf lines(i).StartsWith("DesiredScreenHeight=") Then
                        lines(i) = "DesiredScreenHeight=" & DesiredScreenHeight
                    ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenWidth=") Then
                        lines(i) = "LastUserConfirmedDesiredScreenWidth=" & DesiredScreenWidth
                    ElseIf lines(i).StartsWith("LastUserConfirmedDesiredScreenHeight=") Then
                        lines(i) = "LastUserConfirmedDesiredScreenHeight=" & DesiredScreenHeight
                    ElseIf lines(i).StartsWith("LastRecommendedScreenWidth=") Then
                        lines(i) = "LastRecommendedScreenWidth=" & LastRecommendedScreenWidthHeight
                    ElseIf lines(i).StartsWith("LastRecommendedScreenHeight=") Then
                        lines(i) = "LastRecommendedScreenHeight=" & LastRecommendedScreenWidthHeight
                    ElseIf lines(i).StartsWith("WindowPosX=") Then
                        lines(i) = "WindowPosX=" & WindowPos
                    ElseIf lines(i).StartsWith("WindowPosY=") Then
                        lines(i) = "WindowPosX=" & WindowPos
                    ElseIf lines(i).StartsWith("bUseDesiredScreenHeight=") Then
                        lines(i) = "bUseDesiredScreenHeight=" & UseDesiredScreenHeight
                    End If
                Next

                ' If "FullscreenMode=" line does not exist, insert it at the 6th position
                If Not fullscreenModeExists Then
                    lines.Insert(5, "FullscreenMode=" & FullscreenMode)
                End If

                File.WriteAllLines(filePath, lines.ToArray())
                ' Log Valorant Config Update Success to File
                TrueLog("Info", "Valorant Config Update File Successfully!")
            End If
        End If

    End Sub

End Module