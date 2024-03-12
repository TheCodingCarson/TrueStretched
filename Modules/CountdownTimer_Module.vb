Module CountdownTimer_Module
    Private WithEvents MyTimer As System.Timers.Timer
    Private countdownSeconds As Integer
    Private updateUI As Boolean = False
    Private tcs As TaskCompletionSource(Of Boolean)

    ' {-Summary-}
    '
    ' Without updating the UI, simply call: "CountdownTimer(*Number In Seconds*)"
    ' To include Label3.Text UI updates, call: "CountdownTimer(*Number In Seconds*, True)"
    '
    ' 
    ' - To make sub wait for CountdownTimer to complete before contuining code (Sub must be Async) -
    '
    '                  Await CountdownTimer(*Number In Seconds*, True, True)
    '
    ' - Extra Usage Info -
    '
    '                     Time In Seconds | First True Update Label | Second True Await Completion
    '                            ↓               ↓                       ↓
    '      Await CountdownTimer(10,            True,                   True)
    '
    ' {-Summary-}

    ' Starts the countdown timer. If 'awaitCompletion' is True, the method can be awaited.
    Public Function CountdownTimer(seconds As Integer, Optional updateLabel As Boolean = False, Optional awaitCompletion As Boolean = False) As Task(Of Boolean)
        countdownSeconds = seconds
        updateUI = updateLabel

        If MyTimer Is Nothing Then
            MyTimer = New System.Timers.Timer(1000) ' Tick every second
        Else
            MyTimer.Stop() ' Stop any existing timer
        End If

        tcs = New TaskCompletionSource(Of Boolean)()
        MyTimer.AutoReset = True
        MyTimer.Start()

        AddHandler MyTimer.Elapsed, Sub(sender, e)
                                        countdownSeconds -= 1
                                        If updateUI Then
                                            Try
                                                ' Invoke on the UI thread
                                                Form1.Invoke(Sub()
                                                                 Form1.Label3.Text = $"{countdownSeconds} seconds remaining"
                                                             End Sub)
                                            Catch ex As Exception
                                                ' Handle exceptions
                                            End Try
                                        End If

                                        If countdownSeconds <= 0 Then
                                            MyTimer.Stop()
                                            If updateUI Then
                                                Try
                                                    Form1.Invoke(Sub() Form1.Label3.Text = "Countdown completed.")
                                                Catch ex As Exception
                                                    ' Handle exceptions
                                                End Try
                                            End If
                                            tcs.TrySetResult(True)
                                        End If
                                    End Sub

        If awaitCompletion Then
            Return tcs.Task
        Else
            Return Task.FromResult(False)
        End If

    End Function

End Module