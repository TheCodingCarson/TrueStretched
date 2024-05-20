Imports System.IO
Imports System.Net.Http
Imports System.Reflection
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq

Public Class UpdateAvailable
    Private Sub UpdateAvailable_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Form1.Enabled = False
        SettingsForm.Close()
        Me.TopMost = True

        ' Check if the LastLocation is set in My.Settings and the form is fully visible on any screen
        Dim fixedFormSize As New Size(316, 230) ' Fixed size of the form
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

    End Sub

    Private Async Sub UpdateAvailable_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Check for updates (After form has completely loaded to prevent UI freezing)
        Await CheckForUpdates()
    End Sub

    Private Sub UpdateAvailable_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Form1.Enabled = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DownloadUpdate()
    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim httpClient As New HttpClient()
        Dim json As String = Await httpClient.GetStringAsync("https://download.truestretched.com/latestversion.json")
        Dim jsonObject As JObject = JObject.Parse(json)

        Dim version As String = jsonObject("Version").ToString()
        If jsonObject("Beta").ToObject(Of Boolean)() AndAlso My.Settings.BetaBuild Then
            version &= jsonObject("BetaLetter").ToString()
        End If

        My.Settings.SkippedVersion = version
        My.Settings.Save()

        Me.Close()
    End Sub

    Private Async Function CheckForUpdates() As Task
        Dim currentVersionLong As Version = Assembly.GetExecutingAssembly().GetName().Version
        Dim currentVersion As String = String.Format("{0}.{1}.{2}", currentVersionLong.Major, currentVersionLong.Minor, currentVersionLong.Build)
        Dim betaSetting As Boolean = My.Settings.BetaBuild
        Dim currentBetaLetter As Char
        Dim json As String = String.Empty

        If betaSetting Then
            currentBetaLetter = My.Settings.BetaLetter
        End If

        Try
            ' Await the async call properly
            json = Await GetLatestVersionJson()

            ' Validate JSON before parsing
            If String.IsNullOrWhiteSpace(json) Then
                Throw New Exception("Received empty JSON response.")
            End If

            ' Parse JSON and update UI elements in a thread-safe manner
            Dim jsonObject As JObject = JObject.Parse(json)
            Dim newVersion As String = jsonObject("Version").ToString()
            Dim isBeta As Boolean = Boolean.Parse(jsonObject("Beta").ToString())
            Dim newBetaLetter As Char

            If isBeta Then
                newBetaLetter = jsonObject("BetaLetter").ToString().ToCharArray()(0)
            End If

            ' Safely update Label2.Text on the UI thread
            Me.Invoke(Sub()
                          Label2.Text = "v" & newVersion
                          If betaSetting Then
                              Label2.Text &= newBetaLetter
                          End If
                      End Sub)

            ' Extract the release notes, replace each "*" with a newline and a dot
            Dim releaseNotes As String = jsonObject("ReleaseNotes")("Updates").ToString()
            releaseNotes = releaseNotes.Replace("*", vbCrLf & "•")

            ' If the first line is empty, remove it
            Dim lines As List(Of String) = releaseNotes.Split(New String() {vbCrLf}, StringSplitOptions.None).ToList()
            If lines.Count > 0 AndAlso String.IsNullOrWhiteSpace(lines(0)) Then
                lines.RemoveAt(0)
            End If

            ' Safely update RichTextBox1.Text on the UI thread
            Me.Invoke(Sub() RichTextBox1.Text = String.Join(vbCrLf, lines))

            ' Check if an update is available and safely update Button1.Enabled on the UI thread
            Dim updateAvailable As Boolean = (newVersion > currentVersion) AndAlso (isBeta = False OrElse (isBeta = True AndAlso betaSetting = True)) OrElse (newVersion = currentVersion AndAlso isBeta = True AndAlso betaSetting = True AndAlso newBetaLetter > currentBetaLetter)

            If Not updateAvailable Then
                Me.Invoke(Sub() Text = "True Stretched - No Updates Available")
                Me.Invoke(Sub() Label1.Text = "Current Version Up to Date:")
                Me.Invoke(Sub() Label2.Location = New Point(179, 9))

                Me.Invoke(Sub() Button1.Enabled = False)
                Me.Invoke(Sub() Button2.Enabled = False)
            End If



        Catch ex As Exception
            ' Handle exceptions
            Me.Invoke(Sub() MessageBox.Show("An error occurred while checking for updates: " & ex.Message))
        End Try
    End Function

    Shared Async Function GetLatestVersionJson() As Task(Of String)
        Dim httpClient As New HttpClient()
        Return Await httpClient.GetStringAsync("https://download.truestretched.com/latestversion.json")
    End Function

    Private downloadPath As String

    ' Ensure method is marked as Async and returns a Task
    Async Sub DownloadUpdate()
        Try
            Dim jsonUrl As String = "https://download.truestretched.com/latestversion.json"
            Dim httpClient As New HttpClient()

            ' Use HttpClient to get the JSON string
            Dim json As String = Await httpClient.GetStringAsync(jsonUrl)

            Dim jsonObject As JObject = JObject.Parse(json)
            Dim downloadLink As String = jsonObject("Download")("DownloadLink").ToString()
            Dim fileName As String = Path.GetFileName(downloadLink)
            Dim downloadPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName)

            ' Progress reporting
            Dim progressIndicator As New Progress(Of Integer)(Sub(percentage) Me.Invoke(Sub() ProgressBar1.Value = percentage))

            ' Download the file with progress reporting
            Using response As HttpResponseMessage = Await httpClient.GetAsync(downloadLink, HttpCompletionOption.ResponseHeadersRead)
                response.EnsureSuccessStatusCode()

                Dim totalBytes As Long = response.Content.Headers.ContentLength.GetValueOrDefault(-1L)
                Using contentStream As Stream = Await response.Content.ReadAsStreamAsync(), fileStream As New FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, True)
                    Dim totalBytesRead As Long = 0L
                    Dim bytesRead As Integer
                    Dim buffer As Byte() = New Byte(8191) {}
                    While (InlineAssignHelper(bytesRead, Await contentStream.ReadAsync(buffer))) > 0
                        Await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead))
                        totalBytesRead += bytesRead
                        If totalBytes <> -1L Then
                            Dim progressPercentage As Integer = Convert.ToInt32((totalBytesRead / totalBytes) * 100)
                            DirectCast(progressIndicator, IProgress(Of Integer)).Report(progressPercentage)
                        End If
                    End While
                End Using
            End Using

            ' Mimic the DownloadFileCompleted event
            Dim args As New AsyncCompletedEventArgs(Nothing, False, Nothing)
            Me.Invoke(New Action(Sub() WebClient_DownloadFileCompleted(Me, args)))
        Catch ex As Exception
            Dim errorArgs As New AsyncCompletedEventArgs(ex, False, Nothing)
            Me.Invoke(New Action(Sub() WebClient_DownloadFileCompleted(Me, errorArgs)))
        End Try
    End Sub

    ' Helper method for inline assignments within While loops
    Private Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

    Private Sub WebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            Me.Invoke(New System.Windows.Forms.MethodInvoker(Sub() ProgressBar1.Value = 100))
            Process.Start(downloadPath)
            Application.Exit()
        End If
    End Sub
End Class