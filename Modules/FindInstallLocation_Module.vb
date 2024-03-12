Imports Microsoft.Win32

Module FindInstallLocation_Module

    ' Default Program Dictionary Map Program Names to Registry Keys
    Private ReadOnly programRegistryPaths As New Dictionary(Of String, String) From {
        {"Apex Legends", "Software\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1172470\InstallLocation"},
        {"Valorant", "Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game Riot_Client.\InstallLocation"}
    }

    ' Secondary Program Dictionary Map Program Names to Registry Keys (For Multiple Launchers)
    Private ReadOnly programRegistryPathsVariations As New Dictionary(Of String, String) From {
        {"Apex Legends", "Software\Respawn\Apex\Install Dir"}
    }

    ' Function to find the installation location of a program
    Public Function FindInstallLocation(ByVal programName As String) As String
        ' Attempt to find the install location using the primary dictionary
        Dim installPath As String = FindInstallPathInDictionary(programName, programRegistryPaths)

        ' If not found in the primary dictionary, attempt to find in the variations dictionary
        If String.IsNullOrEmpty(installPath) AndAlso programRegistryPathsVariations.ContainsKey(programName) Then
            installPath = FindInstallPathInDictionary(programName, programRegistryPathsVariations)
        End If

        ' If the install path is still not found, return a message indicating failure
        If String.IsNullOrEmpty(installPath) Then
            Return $"Installation path for '{programName}' not found."
        Else
            Return installPath
        End If
    End Function

    ' Helper function to attempt to find the installation path using a given dictionary
    Private Function FindInstallPathInDictionary(ByVal programName As String, ByVal pathsDictionary As Dictionary(Of String, String)) As String
        Dim fullPath As String = String.Empty

        If pathsDictionary.TryGetValue(programName, fullPath) Then
            ' Split the path to separate the subkey from the value name
            Dim lastBackslashIndex As Integer = fullPath.LastIndexOf("\")
            Dim subKeyPath As String = fullPath.Substring(0, lastBackslashIndex)
            Dim valueName As String = fullPath.Substring(lastBackslashIndex + 1)

            ' First, try finding the installation path under CurrentUser
            Dim installPath As String = GetRegistryValue(Registry.CurrentUser, subKeyPath, valueName)
            If Not String.IsNullOrEmpty(installPath) Then
                Return StandardizePath(installPath)
            End If

            ' If not found under CurrentUser, try LocalMachine
            installPath = GetRegistryValue(Registry.LocalMachine, subKeyPath, valueName)
            If Not String.IsNullOrEmpty(installPath) Then
                Return StandardizePath(installPath)
            End If
        End If

        Return Nothing
    End Function

    ' Helper function to standardize the output of the final install location return
    Private Function StandardizePath(ByVal path As String) As String
        ' Replace all forward slashes with backslashes
        Dim standardizedPath As String = path.Replace("/", "\")

        ' If the last character is a backslash, remove it
        If standardizedPath.EndsWith("\") Then
            standardizedPath = standardizedPath.Remove(standardizedPath.Length - 1)
        End If

        Return standardizedPath
    End Function

    ' Helper function to retrieve a value from the registry
    Private Function GetRegistryValue(ByVal rootKey As RegistryKey, ByVal subKeyPath As String, ByVal valueName As String) As String
        Using subKey As RegistryKey = rootKey.OpenSubKey(subKeyPath)
            If subKey IsNot Nothing Then
                Return TryCast(subKey.GetValue(valueName), String)
            End If
        End Using

        Return Nothing
    End Function

End Module