

Partial Class getimage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Create image.
        Dim newImage As System.Drawing.Image = System.Drawing.Image.FromFile("P:sample.jpg")

        newImage.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone)

        Response.ContentType = "image/png"

        Response.Flush()
        newImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg)
        'Response.WriteFile("P:\sample.jpg")
        Response.End()
    End Sub
End Class
