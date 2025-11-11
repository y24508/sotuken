
Partial Class FileUpLoad
    Inherits System.Web.UI.Page

    Protected Sub UploadButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UploadButton.Click

        Dim redirectpath As String
        Dim FileName As String
        ' Before attempting to save the file, verify
        ' that the FileUpload control contains a file.
        If (FileUpload1.HasFile) Then
            ' Call a helper method routine to save the file.
            FileName = SaveFile(FileUpload1.PostedFile)
            If Page.IsValid Then
                redirectpath = "./Default.aspx?gazou=" & FileName         'Response.Redirect("./Default.aspx?gazou=任意で選択した画像")
                Response.Redirect(redirectpath)
            End If
        Else
            ' Notify the user that a file was not uploaded.
            UploadStatusLabel.Text = "※ファイルが選択されていないのでアップロードできません。"
        End If

    End Sub

    Function SaveFile(ByVal file As HttpPostedFile) As String

        ' Specify the path to save the uploaded file to.
        Dim savePath As String = "C:\h23cis12\卒研\画像\"

        ' Get the name of the file to upload.
        Dim fileName As String = FileUpload1.FileName

        ' Create the path and file name to check for duplicates.
        Dim pathToCheck As String = savePath + fileName

        ' Create a temporary file name to use for checking duplicates.
        Dim tempfileName As String

        ' Check to see if a file already exists with the
        ' same name as the file to upload.        
        If (System.IO.File.Exists(pathToCheck)) Then
            Dim counter As Integer = 2
            While (System.IO.File.Exists(pathToCheck))
                ' If a file with this name already exists,
                ' prefix the filename with a number.
                tempfileName = counter.ToString() + fileName
                pathToCheck = savePath + tempfileName
                counter = counter + 1
            End While

            fileName = tempfileName

            ' Notify the user that the file name was changed.
            UploadStatusLabel.Text = "同じ名前のファイルが存在します。" + "<br />" + _
                                     "違う名前で保存します。 " + fileName

        Else

            ' Notify the user that the file was saved successfully.
            UploadStatusLabel.Text = "ファイルのアップロードに成功しました。"

        End If

        ' Append the name of the file to upload to the path.
        savePath = savePath + fileName

        ' Call the SaveAs method to save the uploaded
        ' file to the specified directory.
        FileUpload1.SaveAs(savePath)

        Return fileName

    End Function
End Class
