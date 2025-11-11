
Partial Class getimage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim picture As String

        picture = " "

        If Request.QueryString("gazou") IsNot Nothing Then
            picture = Request.QueryString("gazou")
        End If

        Dim gazou As New MyImage("C:\h23cis12\卒研\画像\" _
                                  & picture)

        gazou.Normal()
        gazou.Output(Me.Response, picture)
    End Sub
End Class
