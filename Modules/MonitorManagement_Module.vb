Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Text.Json
Imports System.Runtime.CompilerServices
Imports WindowsDisplayAPI
Imports Microsoft.Win32
Imports System.Threading
Imports System.Text.RegularExpressions

Public Module MonitorManagement_Module

    ' ------------------------------------ '
    '  Required My.Settings Names & Types  '
    '                                      '
    '  •Monitors                           '
    '    -Type: 'String'                   '
    '                                      '
    '  •GameMonitor                        '
    '    -Type: 'String'                   '
    '                                      '
    ' ------------------------------------ '

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Function EnumDisplayDevices(lpDevice As String, iDevNum As UInteger, ByRef lpDisplayDevice As DISPLAY_DEVICE, dwFlags As UInteger) As Boolean
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Function ChangeDisplaySettingsEx(lpszDeviceName As String, ByRef lpDevMode As DEVMODE, hwnd As IntPtr, dwflags As UInteger, lParam As IntPtr) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Function EnumDisplaySettings(deviceName As String, modeNum As Integer, ByRef devMode As DEVMODE) As Boolean
    End Function

    Private Const ENUM_CURRENT_SETTINGS As Integer = -1
    Private Const CDS_UPDATEREGISTRY As Integer = &H1
    Private Const DISP_CHANGE_SUCCESSFUL As Integer = 0
    Private Const DM_PELSWIDTH As Integer = &H80000
    Private Const DM_PELSHEIGHT As Integer = &H100000

    Private Structure DISPLAY_DEVICE
        Public cb As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public DeviceName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public DeviceString As String
        Public StateFlags As DisplayDeviceStateFlags
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public DeviceID As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public DeviceKey As String

        Public Sub New(ByVal flags As DisplayDeviceStateFlags)
            cb = Marshal.SizeOf(GetType(DISPLAY_DEVICE))
            StateFlags = flags
        End Sub
    End Structure

    <Flags()>
    Enum DisplayDeviceStateFlags As Integer
        AttachedToDesktop = &H1
        MultiDriver = &H2
        PrimaryDevice = &H4
        MirroringDriver = &H8
        VGACompatible = &H10
        Removable = &H20
        ModesPruned = &H8000000
        Remote = &H4000000
        Disconnect = &H2000000
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure DEVMODE
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmPositionX As Integer
        Public dmPositionY As Integer
        Public dmDisplayOrientation As Integer
        Public dmDisplayFixedOutput As Integer
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmFormName As String
        Public dmLogPixels As Short
        Public dmBitsPerPel As Integer
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
    End Structure

    ' MonitorInfo class to hold each monitor's details
    Public Class MonitorInfo
        Public Property DeviceName As String
        Public Property FriendlyName As String
        Public Property HardwareID As String
        Public Property Resolution As String
        Public Property MaxResolution As String
        Public Property Location As String
        Public Property Orientation As String
        Public Property IsPrimary As Boolean
    End Class

    ' ==-- Main functions --==

    ' -Enumerate Monitors-
    Public Function EnumerateMonitors() As List(Of MonitorInfo)
        Dim monitors As New List(Of MonitorInfo)()

        Dim displays = Display.GetDisplays()
        For Each display As Display In displays
            Dim monitorInfo As New MonitorInfo With {
            .DeviceName = display.DisplayFullName,
            .HardwareID = GetHardwareIDFromRegistry(display.DeviceKey),
            .FriendlyName = display.DeviceName,
            .Resolution = ResolutionFormatted(display),
            .MaxResolution = GetMonitorMaxResolutionFromRegistry(display.DeviceKey),
            .Location = GetMonitorLocation(display),
            .Orientation = GetMonitorOrientationAsString(display),
            .IsPrimary = display.IsGDIPrimary
        }
            monitors.Add(monitorInfo)
        Next

        Return monitors
    End Function

    ' Helper Functions for EnumerateMonitors
    Private Function GetHardwareIDFromRegistry(deviceKey As String) As String
        Dim HardwareID As String = String.Empty

        ' Ensure the key starts with a valid registry hive
        If Not deviceKey.StartsWith("\Registry\Machine\") Then
            Return "Invalid device key format."
        End If

        ' Trim the leading part to get the relative path from the HKEY_LOCAL_MACHINE hive
        Dim relativePath = deviceKey.Substring("\Registry\Machine\".Length)

        Try
            ' Open the registry key
            Using key = Registry.LocalMachine.OpenSubKey(relativePath)
                If key IsNot Nothing Then
                    ' Attempt to read the "MatchingDeviceId" value
                    HardwareID = key.GetValue("MatchingDeviceId", String.Empty).ToString()
                End If
            End Using
        Catch ex As Exception
            ' Handle exceptions, such as security issues or missing keys
            Return $"Error accessing registry: {ex.Message}"
        End Try

        Return HardwareID
    End Function

    Private Function ResolutionFormatted(CurrentDisplay As Display) As String
        Dim FormatedResolution As String = String.Empty

        ' Gets All of Displays Settings
        Dim AllCurrentDisplaySettings As String = (CurrentDisplay.CurrentSetting.Resolution).ToString

        Dim pattern As String = "Width=(.*?), Height=(.*?)\}"
        Dim match As Match = Regex.Match(AllCurrentDisplaySettings, pattern)

        If match.Success Then
            Dim widthValue As String = match.Groups(1).Value
            Dim heightValue As String = match.Groups(2).Value
            Return $"{widthValue}x{heightValue}"
        Else
            Return "Width,Height values not found"
        End If

    End Function

    Private Function GetMonitorMaxResolutionFromRegistry(deviceKey As String) As String
        Dim MaxResolution As String = String.Empty

        ' Ensure the key starts with a valid registry hive
        If Not deviceKey.StartsWith("\Registry\Machine\") Then
            Return "Invalid device key format."
        End If

        ' Trim the leading part to get the relative path from the HKEY_LOCAL_MACHINE hive
        Dim relativePath = deviceKey.Substring("\Registry\Machine\".Length)

        Try
            ' Open the registry key
            Using key = Registry.LocalMachine.OpenSubKey(relativePath)
                If key IsNot Nothing Then
                    ' Attempt to read the "MaxResolution" value
                    MaxResolution = key.GetValue("MaxResolution", String.Empty).ToString()
                End If
            End Using
        Catch ex As Exception
            ' Handle exceptions, such as security issues or missing keys
            Return $"Error accessing registry: {ex.Message}"
        End Try

        ' Standardize Resolution Value
        Dim StandardizedResolution As String = MaxResolution.Replace(",", "x")

        Return StandardizedResolution
    End Function

    Private Function GetMonitorLocation(CurrentDisplay As Display)

        ' Gets Resolution from Display Settings
        Dim AllCurrentDisplaySettings As String = (CurrentDisplay.SavedSetting.Position).ToString

        Dim pattern As String = "X=(.*?),Y=(.*?)\}"
        Dim match As Match = Regex.Match(AllCurrentDisplaySettings, pattern)

        If match.Success Then
            Dim xValue As String = match.Groups(1).Value
            Dim yValue As String = match.Groups(2).Value
            Return $"{xValue},{yValue}"
        Else
            Return "X,Y values not found"
        End If

    End Function

    Private Function GetMonitorOrientationAsString(CurrentDisplay As Display)

        ' Gets Current Displays Orientation
        Dim OrientationInt As Integer = CurrentDisplay.SavedSetting.Orientation

        If IsNothing(OrientationInt) Then
            Return "Monitor not found."
        End If

        Select Case OrientationInt
            Case 0 'Landscape
                Return "Landscape"
            Case 1 'Portrait
                Return "Portrait"
            Case 2 'Landscape (Flipped)
                Return "Landscape (Flipped)"
            Case 3 'Portrait (Flipped)
            Case Else
                Return "Monitor Orientation Value Unknown."
        End Select

    End Function
    ' - End of Helper Functions -



    ' -GetPrimaryMonitor Function-
    '
    ' <summary>
    ' Retrieves information about the primary monitor. Can return comprehensive details or a specified attribute.
    ' Usage:
    ' - To get all details: Dim primaryMonitorDetails As String = GetPrimaryMonitor()
    ' - To get a specific attribute: Dim primaryMonitorFriendlyName As String = GetPrimaryMonitor("FriendlyName")
    ' </summary>
    ' <param name="attribute">Optional. The specific attribute of the primary monitor to return (e.g., "FriendlyName", "Resolution"). If not specified, returns all details.</param>
    ' <returns>A string containing the requested information about the primary monitor. If the attribute is not found, returns a message indicating the attribute is unknown.</returns>
    Public Function GetPrimaryMonitor(Optional ByVal attribute As String = "") As String
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim primaryMonitor As MonitorInfo = monitors.FirstOrDefault(Function(m) m.IsPrimary)

        If primaryMonitor Is Nothing Then
            Return "Primary monitor not found."
        End If

        Select Case attribute.ToLower()
            Case "friendlyname"
                Return primaryMonitor.FriendlyName
            Case "hardwareid"
                Return primaryMonitor.HardwareID
            Case "resolution"
                Return primaryMonitor.Resolution
            Case "maxresolution"
                Return primaryMonitor.MaxResolution
            Case ""
                ' If no attribute is specified, return a formatted string with all details
                Return $"DeviceName: {primaryMonitor.DeviceName}; FriendlyName: {primaryMonitor.FriendlyName}; " &
                       $"HardwareID: {primaryMonitor.HardwareID}; Location: {primaryMonitor.Location}; Orientation: {primaryMonitor.Orientation}; " &
                       $"Resolution: {primaryMonitor.Resolution}; MaxResolution: {primaryMonitor.MaxResolution}; IsPrimary: {primaryMonitor.IsPrimary}"
            Case Else
                Return $"Unknown attribute: {attribute}."
        End Select
    End Function

    ' -GetAllMonitors Function-
    '
    ' <summary>
    ' Retrieves information for all connected monitors. Can return comprehensive details for each monitor or a specified attribute.
    ' Usage:
    ' - To get all details for all monitors: Dim allMonitorsDetails As List(Of String) = GetAllMonitors()
    ' - To get a specific attribute for all monitors: Dim allMonitorsFriendlyNames As List(Of String) = GetAllMonitors("FriendlyName")
    ' </summary>
    ' <param name="attribute">Optional. The specific attribute of the monitors to return (e.g., "FriendlyName", "Resolution"). If not specified, returns all details for each monitor.</param>
    ' <returns>A List of strings, each containing the requested information for a connected monitor. If an attribute is not found for a monitor, it returns a message indicating the attribute is unknown for that monitor.</returns>
    Public Function GetAllMonitors(Optional ByVal attribute As String = "") As List(Of String)
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim monitorDetails As New List(Of String)

        For Each monitor In monitors
            Select Case attribute.ToLower()
                Case "friendlyname"
                    monitorDetails.Add(monitor.FriendlyName)
                Case "hardwareid"
                    monitorDetails.Add(monitor.HardwareID)
                Case "resolution"
                    monitorDetails.Add(monitor.Resolution)
                Case ""
                    ' If no attribute is specified, return a formatted string with all details for each monitor
                    monitorDetails.Add($"DeviceName: {monitor.DeviceName}; FriendlyName: {monitor.FriendlyName}; " &
                             $"HardwareID: {monitor.HardwareID}; Location: {monitor.Location}; Orientation: {monitor.Orientation}; " &
                             $"Resolution: {monitor.Resolution}; MaxResolution: {monitor.MaxResolution}; IsPrimary: {monitor.IsPrimary}")
                Case Else
                    monitorDetails.Add($"Unknown attribute: {attribute} for monitor {monitor.DeviceName}.")
            End Select
        Next

        Return monitorDetails
    End Function

    ' -GetMonitorInfo Function-
    '
    ' <summary>
    ' Retrieves information for a specific monitor identified by a unique identifier. Can return comprehensive details or a specified attribute.
    ' Usage:
    ' - To get all details for a specific monitor: Dim monitorDetails As String = GetMonitorInfo("Monitor Identifier")
    ' - To get a specific attribute for a monitor: Dim monitorAttribute As String = GetMonitorInfo("Monitor Identifier", "Attribute Name")
    ' </summary>
    ' <param name="monitorIdentifier">The unique identifier of the monitor (e.g., FriendlyName, HardwareID).</param>
    ' <param name="attribute">Optional. The specific attribute of the monitor to return (e.g., "FriendlyName", "Resolution"). If not specified, returns all details.</param>
    ' <returns>A string containing the requested information for the specified monitor. If the attribute is not found, returns a message indicating the attribute is unknown.</returns>
    Public Function GetMonitorInfo(monitorIdentifier As String, Optional ByVal attribute As String = "") As String
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim targetMonitor As MonitorInfo = monitors.FirstOrDefault(Function(m) m.DeviceName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                                  m.FriendlyName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                                  m.HardwareID.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase))

        If targetMonitor Is Nothing Then
            Return "Specified monitor not found."
        End If

        Select Case attribute.ToLower()
            Case "friendlyname"
                Return targetMonitor.FriendlyName
            Case "hardwareid"
                Return targetMonitor.HardwareID
            Case "resolution"
                Return targetMonitor.Resolution
            Case "location"
                Return targetMonitor.Location
            Case "orientation"
                Return targetMonitor.Orientation
            Case ""
                ' If no attribute is specified, return a formatted string with all details
                Return $"DeviceName: {targetMonitor.DeviceName}; FriendlyName: {targetMonitor.FriendlyName}; " &
                       $"HardwareID: {targetMonitor.HardwareID}; Location: {targetMonitor.Location}; Orientation: {targetMonitor.Orientation}; " &
                       $"Resolution: {targetMonitor.Resolution}; MaxResolution: {targetMonitor.MaxResolution}; IsPrimary: {targetMonitor.IsPrimary}"
            Case Else
                Return $"Unknown attribute: {attribute}."
        End Select
    End Function

    ' -GetCurrentMonitorInfo Function-
    '
    ' <summary>
    ' Retrieves information for the monitor currently displaying the application. Can return comprehensive details or a specified attribute.
    ' Usage:
    ' - To get all details for the current monitor: Dim currentMonitorDetails As String = GetCurrentMonitorInfo()
    ' - To get a specific attribute for the current monitor: Dim currentMonitorAttribute As String = GetCurrentMonitorInfo("Attribute Name")
    ' </summary>
    ' <param name="attribute">Optional. The specific attribute of the current monitor to return (e.g., "FriendlyName", "Resolution"). If not specified, returns all details.</param>
    ' <returns>A string containing the requested information for the current monitor. If the attribute is not found, returns a message indicating the attribute is unknown.</returns>
    Public Function GetCurrentMonitorInfo(Optional ByVal attribute As String = "") As String
        Dim currentScreen As Screen = Screen.FromControl(Form.ActiveForm)
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim currentMonitor As MonitorInfo = monitors.FirstOrDefault(Function(m) currentScreen.DeviceName.Contains(m.DeviceName))

        If currentMonitor Is Nothing Then
            Return "Current monitor not found."
        End If

        Select Case attribute.ToLower()
            Case "friendlyname"
                Return currentMonitor.FriendlyName
            Case "hardwareid"
                Return currentMonitor.HardwareID
            Case "resolution"
                Return currentMonitor.Resolution
            Case ""
                ' If no attribute is specified, return a formatted string with all details
                Return $"DeviceName: {currentMonitor.DeviceName}; FriendlyName: {currentMonitor.FriendlyName}; " &
                       $"HardwareID: {currentMonitor.HardwareID}; Location: {currentMonitor.Location}; Orientation: {currentMonitor.Orientation}; " &
                       $"Resolution: {currentMonitor.Resolution}; MaxResolution: {currentMonitor.MaxResolution}; IsPrimary: {currentMonitor.IsPrimary}"
            Case Else
                Return $"Unknown attribute: {attribute}."
        End Select
    End Function

    ' -GetGameMonitor Function-
    '
    ' <summary>
    ' Retrieves saved information for the game monitor.
    ' Usage: 
    ' - To get the saved game monitor details: Dim gameMonitorDetails As String = GetGameMonitor()
    ' </summary>
    ' <param name="attribute">Optional. Specifies the attribute of the game monitor to return (e.g., "FriendlyName", "Resolution"). If not specified, returns all saved details.</param>
    ' <returns>A string containing the requested information for the game monitor. If the attribute is not found, returns a message indicating the attribute is unknown.</returns>
    Public Function GetGameMonitor(Optional ByVal attribute As String = "") As String
        Dim gameMonitorDetails As String = My.Settings.GameMonitor

        If String.IsNullOrEmpty(gameMonitorDetails) Then
            Return "Game monitor not set."
        End If

        ' Split the details that are saved in a delimited format
        Dim detailsArray As String() = gameMonitorDetails.Split(";"c)

        ' Structure: [0] DeviceName, [1] FriendlyName, [4] Orientation, [5] Resolution, [6] MaxResolution
        Select Case attribute.ToLower()
            Case "devicename"
                If detailsArray.Length > 0 Then Return detailsArray(0).Replace("DeviceName: ", "") Else Return "Attribute not found."
            Case "friendlyname"
                If detailsArray.Length > 1 Then Return detailsArray(1).Replace("FriendlyName: ", "") Else Return "Attribute not found."
            Case "orientation"
                If detailsArray.Length > 4 Then Return (detailsArray(4).Replace("Orientation: ", "")).Trim Else Return "Attribute not found."
            Case "resolution"
                If detailsArray.Length > 5 Then Return (detailsArray(5).Replace("Resolution: ", "")).Trim Else Return "Attribute not found."
            Case "maxresolution"
                If detailsArray.Length > 6 Then Return (detailsArray(6).Replace("MaxResolution: ", "")).Trim Else Return "Attribute not found."
            Case ""
                Return gameMonitorDetails ' Return all details if no specific attribute is requested
            Case Else
                Return "Unknown attribute."
        End Select
    End Function

    ' -GetMonitorResolution Function-
    '
    ' <summary>
    ' Retrieves the current resolution of a specified monitor or the system's primary monitor by default.
    ' Usage:
    ' - To get the resolution as a combined string: Dim resolutionString As String = GetMonitorResolution()
    ' - To get the resolution split into width and height: Dim resolution As String() = GetMonitorResolution(True).Split("x"c)
    ' - To get the resolution of a specific monitor: Dim specificResolution As String = GetMonitorResolution(False, "Monitor Identifier")
    ' </summary>
    ' <param name="split">Optional. Indicates whether to return the resolution split into width and height (True) or as a combined string (False).</param>
    ' <param name="monitorIdentifier">Optional. The unique identifier of the monitor. If not specified, uses the primary monitor.</param>
    ' <returns>A string containing the current resolution of the specified monitor. If split is True, the resolution is returned as "WidthxHeight".</returns>
    Public Function GetMonitorResolution(Optional ByVal split As Boolean = False, Optional ByVal monitorIdentifier As String = "") As String
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim targetMonitor As MonitorInfo

        If String.IsNullOrEmpty(monitorIdentifier) Then
            targetMonitor = monitors.FirstOrDefault(Function(m) m.IsPrimary)
        Else
            targetMonitor = monitors.FirstOrDefault(Function(m) m.DeviceName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                    m.FriendlyName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                    m.HardwareID.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase))
        End If

        If targetMonitor Is Nothing Then
            Return "Monitor not found."
        End If

        Return If(split, $"{targetMonitor.Resolution.Split("x"c)(0)}x{targetMonitor.Resolution.Split("x"c)(1)}", targetMonitor.Resolution)
    End Function

    ' -GetSavedMonitors Function-
    '
    ' <summary>
    ' Retrieves saved monitor information, optionally filtering for specific attributes.
    ' Usage:
    ' - To get all saved monitors with all details: Dim allMonitors = GetSavedMonitors()
    ' - To get specific details for all saved monitors: Dim monitorNames = GetSavedMonitors("FriendlyName")
    ' </summary>
    ' <param name="attribute">Optional. The specific attribute to filter the returned information (e.g., "FriendlyName", "HardwareID", "DeviceName"). If not specified, returns comprehensive details for each saved monitor.</param>
    ' <returns>A List of strings, each representing a saved monitor's information or the specified attribute of all saved monitors.</returns>
    Public Function GetSavedMonitors(Optional ByVal attribute As String = "") As List(Of String)
        Dim savedInfo As String = My.Settings.Monitors ' This is now a semicolon and newline-separated string
        Dim result As New List(Of String)

        If String.IsNullOrEmpty(savedInfo) Then
            Return New List(Of String)({"No saved monitors found."})
        End If

        Dim monitorLines As String() = savedInfo.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)

        If monitorLines.Length = 0 Then
            Return New List(Of String)({"No saved monitors found."})
        End If

        For Each line As String In monitorLines
            Dim properties As String() = line.Split(";")
            Dim monitor As New Dictionary(Of String, String)
            For Each prop As String In properties
                Dim keyValue As String() = prop.Split(":")
                If keyValue.Length = 2 Then
                    monitor(keyValue(0).Trim()) = keyValue(1).Trim()
                End If
            Next

            ' Handling for specific attribute requests
            If Not String.IsNullOrEmpty(attribute) Then
                Select Case attribute.ToLower()
                    Case "friendlyname"
                        Dim friendlyNameValue As String = Nothing
                        Dim friendlyName As String = If(monitor.TryGetValue("FriendlyName", friendlyNameValue), friendlyNameValue, "FriendlyName not found")

                        Dim isPrimaryValue As String = Nothing
                        If monitor.TryGetValue("IsPrimary", isPrimaryValue) AndAlso isPrimaryValue.Trim().Equals("True", StringComparison.OrdinalIgnoreCase) Then
                            friendlyName &= " (Primary)"
                        End If

                        result.Add(friendlyName)

                    Case "hardwareid", "devicename"
                        Dim attributeValue As String = Nothing
                        If monitor.TryGetValue(attribute, attributeValue) Then
                            result.Add(attributeValue)
                        Else
                            result.Add($"{attribute} not found")
                        End If

                    Case Else
                        result.Add($"Unknown attribute: {attribute}")
                End Select
            Else
                ' If no specific attribute is requested, return all details for the monitor
                Dim fullDetails As String = String.Join("; ", monitor.Select(Function(kvp) $"{kvp.Key}: {kvp.Value}"))
                result.Add(fullDetails)
            End If
        Next

        Return result
    End Function

    ' -GetSavedMonitor Function-
    '
    ' <summary>
    ' Retrieves saved monitor information based on identifier attribute, with special handling for primary and gaming monitors.
    ' Usage Examples:
    ' - Get all details for the primary monitor: Dim primaryMonitorDetails = GetSavedMonitor()
    ' - Get all details for the gaming monitor: Dim gameMonitorDetails = GetSavedMonitor("Gaming")
    ' - Get a specific detail of the gaming monitor: Dim gameMonitorDetail = GetSavedMonitor("Gaming", "Resolution")
    ' </summary>
    ' <param name="identifierAttribute">Optional. Uses "Primary" for the primary monitor, "Gaming" for the game monitor, or a monitor identifier. If omitted, defaults to primary monitor.</param>
    ' <param name="detailAttribute">Optional. The specific detail to retrieve. If omitted, returns all details.</param>
    ' <returns>A string containing the requested monitor information.</returns>
    Public Function GetSavedMonitor(Optional ByVal identifierAttribute As String = "Primary", Optional ByVal detailAttribute As String = "") As String
        Dim savedInfo As String = If(identifierAttribute.Equals("Gaming", StringComparison.OrdinalIgnoreCase), My.Settings.GameMonitor, My.Settings.Monitors)
        If String.IsNullOrEmpty(savedInfo) Then
            Return "Monitor information not found."
        End If

        Dim monitors As New List(Of Dictionary(Of String, String))()
        Dim monitorLines As String() = savedInfo.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)

        For Each line As String In monitorLines
            Dim properties As String() = line.Split(";")
            Dim monitor As New Dictionary(Of String, String)
            For Each prop As String In properties
                Dim keyValue As String() = prop.Split(":")
                If keyValue.Length = 2 Then
                    monitor(keyValue(0).Trim()) = keyValue(1).Trim()
                End If
            Next
            monitors.Add(monitor)
        Next

        Dim selectedMonitor As Dictionary(Of String, String) = Nothing
        If identifierAttribute.Equals("Primary", StringComparison.OrdinalIgnoreCase) Then
            selectedMonitor = monitors.FirstOrDefault(Function(m) m.ContainsKey("IsPrimary") AndAlso m("IsPrimary").Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
        ElseIf Not String.IsNullOrEmpty(identifierAttribute) Then
            selectedMonitor = monitors.FirstOrDefault(Function(m) m.ContainsValue(identifierAttribute))
        End If

        If selectedMonitor Is Nothing Then Return "Monitor not found."

        If Not String.IsNullOrEmpty(detailAttribute) Then
            Dim detailValue As String = Nothing
            If selectedMonitor.TryGetValue(detailAttribute, detailValue) Then
                Return detailValue
            Else
                Return $"Detail '{detailAttribute}' not found."
            End If
        Else
            Return String.Join("; ", selectedMonitor.Select(Function(kvp) $"{kvp.Key}: {kvp.Value}"))
        End If
    End Function


    ' -SetMonitorResolution Function-
    '
    ' <summary>
    ' Sets the resolution of a specified monitor.
    ' Usage: SetMonitorResolution("Monitor FriendlyName", 1920, 1080) 'Set by FriendlyName
    ' </summary>
    ' <param name="monitorIdentifier">The unique identifier of the monitor (e.g., FriendlyName, PnpDeviceID, HardwareID).</param>
    ' <param name="width">The desired width in pixels.</param>
    ' <param name="height">The desired height in pixels.</param>
    Public Sub SetMonitorResolution(monitorIdentifier As String, width As Integer, height As Integer)
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim targetMonitor As MonitorInfo = monitors.FirstOrDefault(Function(m) m.DeviceName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                    m.FriendlyName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                    m.HardwareID.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase))

        If targetMonitor Is Nothing Then
            Throw New ArgumentException("Specified monitor not found.")
            Return
        End If

        Dim devMode As DEVMODE = New DEVMODE With {
            .dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))
        }

        ' Adjust the DeviceName to use the correct format for EnumDisplaySettings and ChangeDisplaySettingsEx
        Dim deviceNameAdjusted As String = targetMonitor.DeviceName.Split("\Monitor")(0)

        ' Fetch the current settings before modifying
        If EnumDisplaySettings(deviceNameAdjusted, ENUM_CURRENT_SETTINGS, devMode) = False Then
            MessageBox.Show("Failed to retrieve current display settings.")
            Return
        End If

        devMode.dmPelsWidth = width
        devMode.dmPelsHeight = height
        devMode.dmFields = DM_PELSWIDTH Or DM_PELSHEIGHT

        ' Apply the new settings
        Dim result As Integer = ChangeDisplaySettingsEx(deviceNameAdjusted, devMode, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero)
        If result = DISP_CHANGE_SUCCESSFUL Then

        Else
            MessageBox.Show($"Failed to change resolution for monitor: {monitorIdentifier}. Error code: {result}")
        End If
    End Sub

    ' -HighlightMonitor Function-
    '
    ' <summary>
    ' Highlights the specified monitor by drawing a semi-transparent overlay around it. 
    ' Usage:
    ' - To highlight a specific monitor: HighlightMonitor(True, "Monitor Identifier")
    ' - To clear the highlight: HighlightMonitor(False)
    ' </summary>
    ' <param name="enable">True to enable highlighting, False to disable and clear any existing highlights.</param>
    ' <param name="monitorIdentifier">Optional. The unique identifier of the monitor to highlight. If not specified and enable is True, highlights the primary monitor.</param>
    Public Sub HighlightMonitor(enable As Boolean, Optional ByVal monitorIdentifier As String = "")

        If Not enable Then
            Highlight_Overlay.Close()
            Return
        End If

        ' Get Targeted Monitor Info
        Dim targetScreen As String = GetSavedMonitor(monitorIdentifier, "FriendlyName")
        If targetScreen Is Nothing Then
            MessageBox.Show("Specified monitor not found.")
            Return
        End If

        ' Get Target Monitor Location, Orientation and Size
        Dim monitorLocation As Point = GetMonitorLocationPointFromString(GetSavedMonitor(monitorIdentifier, "Location"))
        Dim monitorOrientation As String = GetSavedMonitor(monitorIdentifier, "Orientation")
        Dim monitorSize As Size = GetMonitorSizeFromString(GetSavedMonitor(monitorIdentifier, "Resolution"))

        ' If Monitor Orentation is set to "Portrait" or "Portait (Flipped)" swap Width and Height
        If monitorOrientation = "Portrait" OrElse monitorOrientation = "Portrait (Flipped)" Then
            monitorSize = New Size(monitorSize.Height, monitorSize.Width)
        End If

        ' Position and size the Highlight_Overlay to match the target monitor
        Highlight_Overlay.SetBounds(monitorLocation.X, monitorLocation.Y, monitorSize.Width, monitorSize.Height)

        ' Display the Highlight_Overlay
        Highlight_Overlay.Show()
        Highlight_Overlay.BringToFront()
    End Sub

    ' Helper Function for HighlightMonitor
    Public Function GetMonitorLocationPointFromString(MonitorlocationAsString As String) As Point
        Dim parts As String() = MonitorlocationAsString.Split(","c)
        If parts.Length = 2 Then
            Dim x As Integer
            Dim y As Integer
            If Integer.TryParse(parts(0), x) AndAlso Integer.TryParse(parts(1), y) Then
                Return New Point(x, y)
            End If
        End If
        Return Point.Empty ' Return an empty point if parsing fails
    End Function

    ' Helper Function for HighlightMonitor
    Public Function GetMonitorSizeFromString(MonitorSizeAsString As String) As Size
        Dim parts As String() = MonitorSizeAsString.Split("x"c)
        If parts.Length = 2 Then
            Dim Width As Integer
            Dim Height As Integer
            If Integer.TryParse(parts(0), Width) AndAlso Integer.TryParse(parts(1), Height) Then
                Return New Size(Width, Height)
            End If
        End If
        Return Size.Empty ' Return an empty size if parsing fails
    End Function

    ' -SaveMonitorInfo Function-
    '
    ' <summary>
    ' Saves detailed information for a specified monitor, or all monitors if no parameters are specified, into application settings.
    ' Usage:
    ' - To save all details of all monitors: SaveMonitorInfo()
    ' - To save all details of the primary monitor: SaveMonitorInfo("Primary")
    ' - To save a specific attribute of a specific monitor: SaveMonitorInfo("MonitorIdentifier", "Resolution")
    ' - To save all details of a specific monitor: SaveMonitorInfo("MonitorIdentifier", "All")
    ' </summary>
    ' <param name="monitorIdentifier">Optional. The unique identifier of the monitor, "Primary" for the primary monitor, or omitted to select all monitors.</param>
    ' <param name="attribute">Optional. The specific attribute to save (e.g., "Resolution"), "All" for all details, or omitted for all details of all monitors.</param>
    Public Sub SaveMonitorInfo(Optional ByVal monitorIdentifier As String = "", Optional ByVal attribute As String = "All")
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim infoToSave As String = ""

        If String.IsNullOrEmpty(monitorIdentifier) Then
            ' Save details for all monitors
            For Each monitor In monitors
                infoToSave &= $"DeviceName: {monitor.DeviceName}; FriendlyName: {monitor.FriendlyName}; " &
                          $"HardwareID: {monitor.HardwareID}; Location: {monitor.Location}; Orientation: {monitor.Orientation}; " &
                          $"Resolution: {monitor.Resolution}; MaxResolution: {monitor.MaxResolution}; IsPrimary: {monitor.IsPrimary}" & Environment.NewLine
            Next
        Else
            ' Find and save details for a specified monitor
            Dim targetMonitor As MonitorInfo = Nothing

            If monitorIdentifier.Equals("Primary", StringComparison.OrdinalIgnoreCase) Then
                targetMonitor = monitors.FirstOrDefault(Function(m) m.IsPrimary)
            Else
                targetMonitor = monitors.FirstOrDefault(Function(m) m.FriendlyName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                        m.HardwareID.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase))
            End If

            If targetMonitor Is Nothing Then
                MessageBox.Show("Specified monitor not found.")
                Return
            End If

            Select Case attribute.ToLower()
                Case "friendlyname"
                    infoToSave = targetMonitor.FriendlyName
                Case "hardwareid"
                    infoToSave = targetMonitor.HardwareID
                Case "resolution"
                    infoToSave = targetMonitor.Resolution
                Case "all"
                    infoToSave = $"DeviceName: {targetMonitor.DeviceName}; FriendlyName: {targetMonitor.FriendlyName}; " &
                             $"HardwareID: {targetMonitor.HardwareID}; Location: {targetMonitor.Location}; Orientation: {targetMonitor.Orientation}; " &
                             $"Resolution: {targetMonitor.Resolution}; MaxResolution: {targetMonitor.MaxResolution}; IsPrimary: {targetMonitor.IsPrimary}"
                Case Else
                    MessageBox.Show($"Unknown attribute: {attribute}.")
                    Return
            End Select
        End If

        ' Assuming My.Settings.Monitors is the setting created to store the monitor information
        My.Settings.Monitors = infoToSave
        My.Settings.Save() ' Persist the change
        MessageBox.Show("Monitor information saved successfully.")
    End Sub

    ' -SaveGameMonitor Function-
    '
    ' <summary>
    ' Saves the designated game monitor's information into application settings.
    ' Usage:
    ' - To save the game monitor by its identifier: SaveGameMonitor("MonitorIdentifier")
    ' </summary>
    ' <param name="monitorIdentifier">The unique identifier of the monitor to be set as the game monitor. Could be based on any attribute like "FriendlyName", or "HardwareID".</param>
    Public Sub SaveGameMonitor(ByVal monitorIdentifier As String)
        Dim monitors As List(Of MonitorInfo) = EnumerateMonitors()
        Dim targetMonitor As MonitorInfo = monitors.FirstOrDefault(Function(m) m.FriendlyName.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase) OrElse
                                                                  m.HardwareID.Equals(monitorIdentifier, StringComparison.OrdinalIgnoreCase))

        If targetMonitor Is Nothing Then
            MessageBox.Show("Specified monitor not found.")
            Return
        End If

        ' Example saves the FriendlyName; adjust according to which attributes are most relevant or required.
        Dim infoToSave As String = targetMonitor.FriendlyName

        ' Assuming My.Settings.GameMonitor is the setting created to store the game monitor information
        My.Settings.GameMonitor = infoToSave
        My.Settings.Save() ' Persist the change
        MessageBox.Show($"Game monitor set to: {infoToSave}")
    End Sub

    ' -ParseResolution Function-
    '
    ' <summary>
    ' Parses a combined resolution into Width and Height
    ' Usage:
    ' - To split a resolution into two parts: ParseResolution("Resolution")
    ' </summary>
    ' <param name="resolution">The combined resolution you want to split. Could be based on in the format of "widthxheight"</param>
    Public Function ParseResolution(resolution As String) As (Width As Integer, Height As Integer)
        Dim parts = resolution.Split("x"c)
        Return (Integer.Parse(parts(0)), Integer.Parse(parts(1)))
    End Function

End Module