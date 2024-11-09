Imports System.Runtime.InteropServices

Module CheckForWindow_Helper
    ' {-Summary-}
    '
    ' Used to check if the program is running with administrator privleages
    '
    ' Usage:
    '        • CheckForWindow("WindowNameAsString") -- Checks to see if a window is currently running with the same name
    '        • FindWindow(Nothing, "WindowNameAsString") -- Checks for processid of window running with the same name
    '
    ' Return:
    '         • CheckForWindow() -- True/False if Window was found with name
    '         • FindWindow()     -- Returns Window Handle as IntPtr
    '
    ' {-Summary-}

    Public Function CheckForWindow(windowName As String)
        Dim hwnd As IntPtr = FindWindow(Nothing, windowName)
        If hwnd = IntPtr.Zero Then
            Return False 'Window Not Found
        Else
            Return True ' Window was Found
        End If
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Public Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    End Function
End Module
