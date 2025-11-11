
Partial Class getimage2
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Dim value As Integer
        Dim picture As String

        picture = " "

        If Request.QueryString("scale") IsNot Nothing Then
            value = Integer.Parse(Request.QueryString("scale"))
        End If

        If Request.QueryString("gazou") IsNot Nothing Then
            picture = Request.QueryString("gazou")
        End If

        Dim gazou As New MyImage("C:\h23cis12\卒研\画像\" _
                                  & picture)

        gazou.Normal()
        gazou.Blur_effect(value)
        gazou.Output(Me.Response, picture)
    End Sub

End Class
