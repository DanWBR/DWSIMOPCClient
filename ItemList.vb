Imports Opc.Ua

Public Class ItemList

    Public Property MonItem As ReferenceDescription
    Public Property Session As Opc.Ua.Client.Session

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        MonItem = BrowseCTRL.NodesTV.SelectedNode.Tag

        Close()

    End Sub

End Class