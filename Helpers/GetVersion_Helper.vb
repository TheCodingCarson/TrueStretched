Imports System.Reflection

Module GetVersion_Helper
    ' {-Summary-}
    '
    ' Used to return the standardized/formated True Stretched version number (Uses a Custom Class called "VersionInfo")
    '
    ' Usage: CurrentVersion()
    ' Return: CurrentVersion() As "VersionInfo"
    '         • VersionNumber As a String
    '         • BetaBuild As a Boolean
    '         • BetaLetter As a String
    '
    ' Examples:
    '           • --Access properties from the custom class--
    '             Dim versionInfo As VersionInfo = CurrentVersion()
    '             Dim versionNumber As String = VersionInfo.VersionNumber
    '             Dim isBeta As Boolean = VersionInfo.IsBeta
    '             Dim betaLetter As String = VersionInfo.BetaLetter
    '
    '           • --Call function for single property--
    '             Dim versionNumber As String = CurrentVersion().VersionNumber
    '             Dim isBeta As Boolean = CurrentVersion().IsBeta
    '             Dim betaLetter As String = CurrentVersion().BetaLetter
    '
    ' {-Summary-}

    ' Define a custom class to hold version-related data
    Public Class VersionInfo
        Public Property VersionNumber As String
        Public Property IsBeta As Boolean
        Public Property BetaLetter As String
    End Class

    ' Modify the function to return the custom class
    Public Function CurrentVersion() As VersionInfo
        ' Get the version number from the executing assembly
        Dim versionNumber As String = Assembly.GetExecutingAssembly().GetName().Version.ToString()
        versionNumber = versionNumber.Substring(0, versionNumber.Length - 2)

        ' Determine if it's a beta build and get the beta letter if it is
        Dim betaBuild As Boolean = My.Settings.BetaBuild
        Dim betaLetter As String = If(betaBuild, My.Settings.BetaLetter, String.Empty)

        ' Return an instance of the VersionInfo class with the populated values
        Return New VersionInfo With {
            .VersionNumber = versionNumber,
            .IsBeta = betaBuild,
            .BetaLetter = betaLetter
        }
    End Function

End Module
