Imports System.Text.RegularExpressions

Namespace My

    Partial Friend Class MyApplication

        ' Process command-line arguments before the form is shown
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            ' Get Command-line Arguments
            Dim args As String() = Environment.GetCommandLineArgs()
            Dim debugoutputargs As New List(Of String)

            ' test
            For i As Integer = 0 To args.Length - 1
                Dim arg = args(i)
                Select Case arg

                    Case "--Dev"
                        DevBuild = True
                        debugoutputargs.Add("--Dev")

                    Case "--AutoDisable"
                        AutoDisable = True
                        debugoutputargs.Add("--AutoDisable")

                    Case "--OverrideNative"
                        ' Regular expression to match a valid resolution such as "1920x1080"
                        Dim resolutionPattern As String = "^\d+x\d+$"
                        Dim regex As New Regex(resolutionPattern)

                        ' Check if the next argument exists and matches a valid resolution format
                        If i + 1 < args.Length Then
                            Dim nextArg = args(i + 1)

                            ' Validate if the next argument matches the required resolution format (e.g., "1920x1080")
                            If regex.IsMatch(nextArg) Then
                                ' Successful validation of "--OverrideNative" argument format
                                OverrideNative = True
                                OverrideNativeRes = nextArg
                                debugoutputargs.Add("--OverrideNative")
                                debugoutputargs.Add("OverrideNative Resolution: " & OverrideNativeRes)
                                i += 1 ' Skip over the resolution part in the next iteration
                            Else
                                ' The next argument is not a valid resolution
                                debugoutputargs.Add("--OverrideNative")
                                debugoutputargs.Add("Invalid or missing resolution after --OverrideNative, continuing...")
                            End If
                        Else
                            ' No next argument exists (no resolution provided)
                            debugoutputargs.Add("--OverrideNative")
                            debugoutputargs.Add("No resolution provided after --OverrideNative, continuing...")
                        End If

                    Case "--AutoStretch"
                        debugoutputargs.Add("--AutoStretch")

                        If i + 1 < args.Length Then
                            Dim nextArg = args(i + 1)

                            If Not (nextArg = "") Then
                                ' Check for valid AutoStretch Game Choice
                                Select Case nextArg
                                    Case "ApexLegends"
                                        AutoStretch = True
                                        My.Settings.SelectedGame = "Apex Legends"
                                        My.Settings.Save()
                                        debugoutputargs.Add("AutoStretchGame Selected: " & nextArg)
                                    Case "Farlight84"
                                        AutoStretch = True
                                        My.Settings.SelectedGame = "Farlight 84"
                                        My.Settings.Save()
                                        debugoutputargs.Add("AutoStretchGame Selected: " & nextArg)
                                    Case "Fortnite"
                                        AutoStretch = True
                                        My.Settings.SelectedGame = "Fortnite"
                                        My.Settings.Save()
                                        debugoutputargs.Add("AutoStretchGame Selected: " & nextArg)
                                    Case "Valorant"
                                        AutoStretch = True
                                        My.Settings.SelectedGame = "Valorant"
                                        My.Settings.Save()
                                        debugoutputargs.Add("AutoStretchGame Selected: " & nextArg)
                                    Case "XDefiant"
                                        AutoStretch = True
                                        My.Settings.SelectedGame = "XDefiant"
                                        My.Settings.Save()
                                        debugoutputargs.Add("AutoStretchGame Selected: " & nextArg)
                                End Select
                            End If

                        End If

                        If Not AutoStretch Then
                            ' Selected AutoStretch Game is Invalid or Missing
                            debugoutputargs.Add("Invalid or missing game after --AutoStretch, continuing...")
                        End If

                End Select
            Next

#If DEBUG Then
            If debugoutputargs IsNot Nothing Then
                Debug.WriteLine("")
                Debug.WriteLine("INFO: Received command-line argument:")
                For Each debugoutputarg As String In debugoutputargs
                    Debug.WriteLine($"{debugoutputarg}")
                Next
                Debug.WriteLine("")
            Else
                Debug.WriteLine("INFO: Didn't receive command-line arguments")
            End If
#End If

            'Start TrueLog
            Dim TrueLogStartArgsList As String = String.Join(" ", debugoutputargs)
            TrueLog("Start", TrueLogStartArgsList)

        End Sub

    End Class

End Namespace