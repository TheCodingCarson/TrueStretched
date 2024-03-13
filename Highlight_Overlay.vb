Public Class Highlight_Overlay

    ' Ensure Overlay Panel doesn't set to active when it's shown
    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Private Sub Highlight_Overlay_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Initial adjustments of Overlay Panel
        AdjustPanelSizeAndLocation()

    End Sub

    Private Sub Highlight_Overlay_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        AdjustPanelSizeAndLocation()
    End Sub

    Private Sub AdjustPanelSizeAndLocation()
        ' Ensuring Panel1 is always 5px smaller around the border compared to OverlayForm
        With Panel1
            .Location = New Point(5, 5) ' 5px offset from the top-left corner
            .Size = New Size(Me.ClientSize.Width - 10, Me.ClientSize.Height - 10) ' 5px offset from each side
        End With
    End Sub
End Class