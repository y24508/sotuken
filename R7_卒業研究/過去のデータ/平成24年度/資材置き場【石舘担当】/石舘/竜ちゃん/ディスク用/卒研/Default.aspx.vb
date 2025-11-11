
Partial Class _Default
    Inherits System.Web.UI.Page

    Dim myUrl As String
    Dim picture As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
       
        picture = " "
        If Request.QueryString("gazou") IsNot Nothing Then
            picture = Request.QueryString("gazou")
        End If

        myUrl = "getimage.aspx?gazou=" & picture

        Me.Image1.ImageUrl = myUrl

    End Sub

    Protected Sub Magnify_cut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Magnify_cut.Click

        Dim redirectpath As String

        If Page.IsValid Then
            redirectpath = "./Magnify_cut.aspx?gazou=" & picture
            Response.Redirect(redirectpath)
        End If
    End Sub

    Protected Sub Blur_effect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Blur_effect.Click

        Dim redirectpath As String

        If Page.IsValid Then
            redirectpath = "./Blur_effect.aspx?gazou=" & picture
            Response.Redirect(redirectpath)
        End If
    End Sub

    Protected Sub Sharpening_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Sharpening.Click

        Dim redirectpath As String

        If Page.IsValid Then
            redirectpath = "./Sharpening.aspx?gazou=" & picture
            Response.Redirect(redirectpath)
        End If
    End Sub

    Protected Sub Contrast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Contrast.Click
        Dim redirectpath As String

        If Page.IsValid Then
            redirectpath = "./Contrast.aspx?gazou=" & picture
            Response.Redirect(redirectpath)
        End If
    End Sub

    Protected Sub Binalize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Binalize.Click
        Dim redirectpath As String

        If Page.IsValid Then
            redirectpath = "./Binalize.aspx?gazou=" & picture
            Response.Redirect(redirectpath)
        End If
    End Sub
End Class
