Imports System.IO
Imports System.Reflection

Module TrueLogging_Module
    ' {-Summary-}
    '
    ' Used to create and update the "TrueStretched-Log.log" file used for users to provide to help debug and issues they are having
    '
    ' If log file size hits 350mb during Initilization it will be deleted and re-created
    '
    ' Initilazation: TrueLog("Start", StartArgs)
    ' Usage: TrueLog("Info/Warn/Error", "Log Message")
    ' Closing: TrueLog("Close", "--True Stretched Closing--")
    '
    ' DateTime Format: MM'-'dd'-'yyyy HH':'mm':'ss'Z'
    '
    ' {-Summary-}

    Private ReadOnly appDataTrueStretchedPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "True Stretched")
    Private LogFilePath As String = ""
    Private Const MaxLogSize As Long = 350 * 1024 * 1024 ' 350 MB in bytes
    Private StartArgs As String = ""

    ' Public function for logging
    Public Sub TrueLog(logType As String, message As String)

        ' Watch For Start Message
        If (logType = "Start") Then
            StartArgs = message

            #If DEBUG Then
            Debug.WriteLine("TrueLog Initialized!")
            #End If

            InitializeTrueLog()
        Else
            ' Log Message to "TrueStretched-Log.log" File
            Dim curentDateTime As String = DateTime.UtcNow.ToString("MM'-'dd'-'yyyy HH':'mm':'ss")
            Dim logEntry As String = $"{curentDateTime} - {logType}: {message}"

            ' Appends the log entry to the log file on a new line, ensuring there are no unnecessary spaces before or after the entry.
            File.AppendAllText(LogFilePath, logEntry & Environment.NewLine)
        End If

    End Sub

    ' Initialize and manage the log file
    Private Sub InitializeTrueLog()

        ' Check for or Create log Directory in "AppData\Local"
        If Not Directory.Exists(appDataTrueStretchedPath) Then
            Directory.CreateDirectory(appDataTrueStretchedPath)
        End If

        ' Set the Log File Full Path
        LogFilePath = Path.Combine(appDataTrueStretchedPath, "TrueStretched-Log.log")

        ' Check For or Create log File
        If Not File.Exists(LogFilePath) Then
            File.Create(LogFilePath).Dispose()
            LogStartMessages(True)
        ElseIf New FileInfo(LogFilePath).Length > MaxLogSize Then
            File.Delete(LogFilePath)
            File.Create(LogFilePath).Dispose()
            LogStartMessages(True)
        Else
            LogStartMessages(False)
        End If
    End Sub

    ' Handles start-up messages including application settings
    Private Sub LogStartMessages(isNewFile As Boolean)
        Dim versionNumber As String = Assembly.GetExecutingAssembly().GetName().Version.ToString()
        versionNumber = versionNumber.Substring(0, versionNumber.Length - 2)
        Dim startTime As String = DateTime.UtcNow.ToString("MM'-'dd'-'yyyy HH':'mm':'ss")
        Dim betaBuild As Boolean = My.Settings.BetaBuild
        Dim betaLetter As String = If(betaBuild, My.Settings.BetaLetter, String.Empty)
        Dim firstRun As Boolean = My.Settings.FirstRun
        Dim mainGPU As String = My.Settings.MainGPU
        Dim stretchedResolution As String = My.Settings.StretchedResolution
        Dim gameMonitor As String = My.Settings.GameMonitor

        Dim startupInfo As New Text.StringBuilder()
        startupInfo.AppendLine("========True Stretched v" & versionNumber & " Started========")
        startupInfo.AppendLine("Start Time: " & startTime)
        startupInfo.AppendLine("Administrator: " & IsUserAdministrator())
        startupInfo.AppendLine("Command Arguments: " & StartArgs.ToString())
        startupInfo.AppendLine("FirstRun: " & firstRun.ToString())
        startupInfo.AppendLine("Version: " & versionNumber)
        startupInfo.AppendLine("Beta: " & betaBuild.ToString())
        If betaBuild Then
            startupInfo.AppendLine("Beta Build: " & betaLetter)
        End If
        startupInfo.AppendLine("Main GPU: " & mainGPU)
        If Not firstRun Then
            startupInfo.AppendLine("Stretched Resolution: " & stretchedResolution)
            startupInfo.AppendLine("Game Monitor: " & gameMonitor)
        End If
        startupInfo.AppendLine() ' Blank line for spacing

        ' Add two blank lines if not a new file
        If Not isNewFile Then
            File.AppendAllText(LogFilePath, Environment.NewLine & Environment.NewLine)
        End If

        File.AppendAllText(LogFilePath, startupInfo.ToString())
    End Sub

    ' Example helper method to check if the current user is an administrator
    Private Function IsUserAdministrator() As Boolean
        Dim identity = System.Security.Principal.WindowsIdentity.GetCurrent()
        Dim principal = New System.Security.Principal.WindowsPrincipal(identity)
        Return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)
    End Function

End Module