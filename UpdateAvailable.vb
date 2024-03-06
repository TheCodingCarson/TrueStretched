Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq

Public Class UpdateAvailable
    Private Sub UpdateAvailable_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Form1.Enabled = False
        Me.TopMost = True

        ' Check if the LastLocation is set in My.Settings
        If Not My.Settings.LastLocation.IsEmpty Then
            ' Load the last location from My.Settings
            Me.Location = My.Settings.LastLocation
        Else
            ' Set a default location for the form
            Me.StartPosition = FormStartPosition.WindowsDefaultLocation
        End If

        'Check for updates
        CheckForUpdates()

    End Sub

    Private Sub UpdateAvailable_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Form1.Enabled = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DownloadUpdate("https://download.truestretched.com/latestversion.json")
    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim httpClient As HttpClient = New HttpClient()
        Dim json As String = Await httpClient.GetStringAsync("https://download.truestretched.com/latestversion.json")
        Dim jsonObject As JObject = JObject.Parse(json)

        Dim version As String = jsonObject("Version").ToString()
        If jsonObject("Beta").ToObject(Of Boolean)() AndAlso My.Settings.BetaBuild Then
            version = version & jsonObject("BetaLetter").ToString()
        End If

        My.Settings.SkippedVersion = version
        My.Settings.Save()

        Me.Close()
    End Sub

    Sub CheckForUpdates()
        Dim currentVersion As String = Application.ProductVersion
        Dim currentBetaLetter As Char = My.Settings.BetaLetter
        Dim betaSetting As Boolean = My.Settings.BetaBuild
        Dim json As String = String.Empty

        Try
            Dim request As WebRequest = WebRequest.Create("https://download.truestretched.com/latestversion.json")
            Dim response As WebResponse = request.GetResponse()
            Using dataStream As Stream = response.GetResponseStream()
                Dim reader As New StreamReader(dataStream)
                json = reader.ReadToEnd()
            End Using
            response.Close()

            Dim jsonObject As JObject = JObject.Parse(json)
            Dim newVersion As String = jsonObject("Version").ToString()
            Dim isBeta As Boolean = Boolean.Parse(jsonObject("Beta").ToString())
            Dim newBetaLetter As Char = jsonObject("BetaLetter").ToString().ToCharArray()(0)

            ' Display the new version number and, if it's a beta, the beta letter
            Label2.Text = newVersion
            If betaSetting = True Then
                Label2.Text &= newBetaLetter
            End If

            ' Extract the release notes, replace each "*" with a newline and a dot, and set the RichTextBox text
            Dim releaseNotes As String = jsonObject("ReleaseNotes")("Updates").ToString()
            releaseNotes = releaseNotes.Replace("*", vbCrLf & "•")

            ' If the first line is empty, remove it
            Dim lines As List(Of String) = releaseNotes.Split(New String() {vbCrLf}, StringSplitOptions.None).ToList()
            If lines.Count > 0 AndAlso String.IsNullOrWhiteSpace(lines(0)) Then
                lines.RemoveAt(0)
            End If
            RichTextBox1.Text = String.Join(vbCrLf, lines)

            If (newVersion > currentVersion) AndAlso (isBeta = False Or (isBeta = True And betaSetting = True)) Then
                ' Update available
                Button1.Enabled = True
            ElseIf (newVersion = currentVersion And isBeta = True And betaSetting = True And newBetaLetter > currentBetaLetter) Then
                ' Beta update available
                Button1.Enabled = True
            End If
        Catch ex As Exception
            ' Handle exceptions if any
        End Try
    End Sub

    Private downloadPath As String

    Sub DownloadUpdate(jsonUrl As String)
        Try
            Dim json As String = String.Empty
            Dim request As WebRequest = WebRequest.Create(jsonUrl)
            Dim response As WebResponse = request.GetResponse()
            Using dataStream As Stream = response.GetResponseStream()
                Dim reader As New StreamReader(dataStream)
                json = reader.ReadToEnd()
            End Using
            response.Close()

            Dim jsonObject As JObject = JObject.Parse(json)
            Dim downloadLink As String = jsonObject("Download")("DownloadLink").ToString()
            Dim fileName As String = Path.GetFileName(downloadLink)
            downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName)

            Using client As New WebClient()
                AddHandler client.DownloadFileCompleted, AddressOf WebClient_DownloadFileCompleted
                client.DownloadFileAsync(New Uri(downloadLink), downloadPath)
            End Using
        Catch ex As Exception
            ' Handle exceptions if any
        End Try
    End Sub

    Private Sub WebClient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            Me.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = 100))
            Process.Start(downloadPath)
            Application.Exit()
        End If
    End Sub
End Class