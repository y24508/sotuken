Imports System.Drawing

Partial Class Magnify_cut
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myUrl As String
        Dim picture As String
        Dim value As Double
        picture = " "

        If Not Page.IsPostBack Then
            Me.txt1.Text = 100
        End If

        value = Double.Parse(Me.txt1.Text)

        If Request.QueryString("gazou") IsNot Nothing Then
            picture = Request.QueryString("gazou")
        End If

        myUrl = "getimage1.aspx?gazou=" & picture & "&scale=" & value

        Me.Image1.ImageUrl = myUrl
    End Sub
End Class
