Imports System.Windows.Forms
Imports IWshRuntimeLibrary
Imports System.IO

Module CreateDesktopShortcut_Module
    ' {-Summary-}
    '
    ' Creates a Desktop shorcut used with "AutoStretch" to automaticlly launch a game in stretched without interacticing with the TrueStretched UI
    '
    ' Usage: 
    '
    ' {-Summary-}


    Public Sub CreateDesktopShortcut(ByVal gameName As String)
        Dim ShortcutName As String = gameName + " (Stretched).lnk"
        Dim iconPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "icons\" + gameName + ".ico")

        ' Get desktop path
        Dim desktopPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

        ' Define shortcut path
        Dim shortcutPath As String = Path.Combine(desktopPath, ShortcutName)

        ' Get path to the running executable
        Dim exePath As String = Application.ExecutablePath

        ' Arguments to pass to the program
        Dim arguments As String = ShortcutArgs(gameName)

        ' Create WshShell object
        Dim wsh As New WshShell()

        ' Create the shortcut
        Dim shortcut As IWshShortcut = CType(wsh.CreateShortcut(shortcutPath), IWshShortcut)
        shortcut.TargetPath = exePath
        shortcut.Arguments = arguments
        shortcut.WorkingDirectory = Path.GetDirectoryName(exePath)
        shortcut.IconLocation = iconPath
        shortcut.Description = gameName + " (True Stretched)"
        shortcut.Save()

    End Sub

    Private Function ShortcutArgs(ByVal gameName As String) As String
        Dim ShortcutArgsGenenerated As String = ""

        Select Case gameName
            Case "Apex Legends"
                ShortcutArgsGenenerated = "--AutoDisable --AutoStretch ApexLegends"
            Case "Farlight 84"
                ShortcutArgsGenenerated = "--AutoDisable --AutoStretch Farlight84"
            Case "Fortnite"
                ShortcutArgsGenenerated = "--AutoDisable --AutoStretch Fortnite"
            Case "Valorant"
                ShortcutArgsGenenerated = "--AutoDisable --AutoStretch Valorant"
            Case "XDefiant"
                ShortcutArgsGenenerated = "--AutoDisable --AutoStretch XDefiant"
        End Select

        Return ShortcutArgsGenenerated

    End Function

End Module
