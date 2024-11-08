Module PrivilegedCheck_Helper
    ' {-Summary-}
    '
    ' Used to check if the program is running with administrator privleages
    '
    ' Usage: IsUserAdministrator()
    ' Return: True/False
    '
    ' {-Summary-}

    Public Function IsUserAdministrator() As Boolean
        Dim identity = System.Security.Principal.WindowsIdentity.GetCurrent()
        Dim principal = New System.Security.Principal.WindowsPrincipal(identity)
        Return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)
    End Function

End Module
