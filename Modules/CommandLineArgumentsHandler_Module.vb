Namespace My

    Partial Friend Class MyApplication

        ' Process command-line arguments before the form is shown
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            ' Get Command-line Arguments
            Dim args As String() = Environment.GetCommandLineArgs()
            Dim debugoutputargs As New List(Of String)

            ' test
            For Each arg As String In args
                Select Case arg
                    Case "--Dev"
                        DevBuild = True
                        debugoutputargs.Add("--Dev")
                    Case "--AutoDisable"
                        AutoDisable = True
                        debugoutputargs.Add("--AutoDisable")
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

        End Sub

    End Class

End Namespace