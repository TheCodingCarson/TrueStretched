Imports System.IO
Imports System.Text.RegularExpressions

Module FindConfigLocation_Module

    ' {-Summary-}
    '
    ' To Find Config File of Game Listed In Dictionary, simply call: "*var* = FindConfigLocation(*Game Name*)"
    ' Will return full path of config location
    '
    ' {-Summary-}

    ' Program Dictionary Map Program Names to Config File Location (Either Full Location or marked "Individual Method Required")
    Private ReadOnly programConfigPaths As New Dictionary(Of String, String) From {
        {"Apex Legends", (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "\Saved Games\Respawn\Apex\local\videoconfig.txt")},
        {"Valorant", "Individual Method Required"}
    }

    ' Function to find the config file location of a program
    Public Function FindConfigLocation(ByVal programName As String) As String
        Dim configPath As String = ""
        Dim success As Boolean = programConfigPaths.TryGetValue(programName, configPath)

        If success Then
            If configPath = "Individual Method Required" Then
                ' Select Individual Method based on programName
                Select Case programName
                    Case "Valorant"
                        configPath = ValorantConfigLocationMethod()
                        ' Add additional cases for other programs as needed
                End Select
            Else
                ' The value from the dictionary is already set to configPath
            End If
        Else
            configPath = $"'{programName}' not found in configuration paths."
        End If

        ' Return Config Path Value or Error
        Return configPath

    End Function

    ' Helper function to standardize the output of the final config location
    Private Function StandardizePath(ByVal path As String) As String
        ' Replace all forward slashes with backslashes
        Dim standardizedPath As String = path.Replace("/", "\")

        Return standardizedPath
    End Function

    ' -------- Methods For Individual Games -------- '

    ' -Valorant-
    Private Function ValorantConfigLocationMethod() As String

        ' Get last Valorant User ID
        Dim useridfilepath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VALORANT/Saved/Config/Windows/RiotLocalMachine.ini")
        Dim LastKnownValorantUser As String = ""
        Dim ValorantConfigReturn As String = ""

        If File.Exists(useridfilepath) Then
            ' Read all lines of the file
            Dim lines As String() = File.ReadAllLines(useridfilepath)

            ' Iterate through each line
            For Each line As String In lines
                ' Check if the line contains the key
                If line.StartsWith("LastKnownUser" & "=", StringComparison.OrdinalIgnoreCase) Then
                    ' Extract and return the value after the '='
                    LastKnownValorantUser = line.Substring(line.IndexOf("="c) + 1).Trim()
                End If
            Next
        Else
            ValorantConfigReturn = "'Valorant Last Riot User ID' not found in 'RiotLocalMachine.ini'"
        End If

        If Not (String.IsNullOrEmpty(LastKnownValorantUser)) Then
            Dim valorantmainfolderPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/VALORANT/Saved/Config"

            ' Get all subdirectories in the folder
            Dim valorantAllConfigFolders As String() = Directory.GetDirectories(valorantmainfolderPath)

            ' Define the pattern to match the folder name (riotid + "-" + region regex)
            Dim regionPattern As String = "^" & Regex.Escape(LastKnownValorantUser) & "\-[a-z]{2,3}$"

            ' Find the first matching folder or Nothing if no match is found
            Dim valorantUserConfigFolderPath = Path.GetFileName(valorantAllConfigFolders.FirstOrDefault(Function(folder) Regex.IsMatch(Path.GetFileName(folder), regionPattern)))

            ' Check if a matching file was found
            If valorantUserConfigFolderPath IsNot Nothing Then
                ' Add the rest of the file path to the folder for the last riot id + region code
                Dim fullvalorantUserConfigFolderPath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $"/VALORANT/Saved/Config/{valorantUserConfigFolderPath}/Windows/GameUserSettings.ini"
                ' Return full last riot user config path with region code added (and standardized)
                ValorantConfigReturn = StandardizePath(fullvalorantUserConfigFolderPath)
            Else
                ValorantConfigReturn = "'Valorant Last Riot User ID' configuration folder not found."
            End If
        Else
            ' The error value from the If File.Exists
        End If

        ' Return Config File Path or Error
        Return ValorantConfigReturn

    End Function

End Module
