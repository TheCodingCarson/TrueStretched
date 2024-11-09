﻿Module GlobalVariables

    ' Module Used to Declare Global Veriables

    Public DevBuild As Boolean = False         ' Used For Enabling DevMode
    Public StretchedEnabled As Boolean = False ' Used for checking if Stretched is currently enabled
    Public AutoDisable As Boolean = False      ' Used To Automatically Disable True Stretched On Launch
    Public OverrideNative As Boolean = False   ' Used To Override Native Display Resolution
    Public OverrideNativeRes As String = ""    ' ^ Resolution used by Override

End Module
