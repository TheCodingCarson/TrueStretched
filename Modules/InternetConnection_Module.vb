Imports System.Net.Http

Module InternetConnection_Module
    Private hasChecked As Boolean = False
    Private lastResult As Boolean = False

    Public Function InternetConnection() As Boolean

        If hasChecked Then
            ' Return the stored result if the check has already been performed.
            Return lastResult
        Else
            ' If check hasn't been performed preform check and store value till application shutsdown
            Try
                Using httpClient As New HttpClient()
                    ' Send a GET request to a known website.
                    Dim response = httpClient.GetAsync("http://www.google.com").Result
                    ' Check if the request was successful based on the HTTP status code.
                    If response.IsSuccessStatusCode Then
                        ' Successful HTTP response, internet is likely available.
                        lastResult = True
                    Else
                        ' The HTTP request was unsuccessful.
                        lastResult = False
                    End If
                End Using
            Catch ex As Exception
                ' An error occurred, assume the internet is not available.
                lastResult = False
            End Try
            ' Indicate that a check has been performed and return result.
            hasChecked = True
            Return lastResult
        End If

    End Function

End Module
