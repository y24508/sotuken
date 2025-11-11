
Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices


Public Class Sample_MyImage
    Private img As Bitmap           '出力用画像
    Private org As Bitmap           '元画像
    Dim NewRect As Rectangle
    Private width As Integer        '幅
    Private height As Integer       '高さ
    Dim myColor As Color

    Public Sub New(ByVal path As String)
        org = Image.FromFile(path)          '画像の呼び出し
        width = org.Width                   '元画像の幅
        height = org.Height                 '元画像の高さ
        'img = New Bitmap(width, height)     'ピクセルデータで定義したイメージを処理する
    End Sub

    Public Sub Normal()
        Dim x As Integer
        Dim y As Integer
        Dim myColor As Color

        img = New Bitmap(width, height)     'ピクセルデータで定義したイメージを処理する
        For x = 0 To width - 1
            For y = 0 To height - 1
                myColor = org.GetPixel(x, y)            'Bitmapの指定したピクセルの色を取得
                img.SetPixel(x, y, myColor)              'Bitmapの指定したピクセルの色を設定
            Next y
        Next x
    End Sub

    Public Sub Rotate90()
        img.RotateFlip(RotateFlipType.Rotate90FlipNone)
        Dim bmp = New Bitmap(img)
    End Sub

    Public Sub Gray()
        Dim x As Integer
        Dim y As Integer
        Dim myColor As Color
        Dim g As Integer
        Dim myGray As Color

        img = New Bitmap(width, height)     'ピクセルデータで定義したイメージを処理する

        For x = 0 To width - 1
            For y = 0 To height - 1
                myColor = org.GetPixel(x, y)            'Bitmapの指定したピクセルの色を取得
                g = 0.298912 * myColor.R + _
                    0.586611 * myColor.G + _
                    0.114478 * myColor.B                'グレースケール化
                myGray = Color.FromArgb(g, g, g)        '8ビットカラー値(赤、緑、青)からColor構造体を作成
                img.SetPixel(x, y, myGray)              'Bitmapの指定したピクセルの色を設定
            Next y
        Next x
    End Sub

    Public Sub Binarize(ByVal threshold As Integer)
        Dim x As Integer
        Dim y As Integer
        Dim myColor As Color
        Dim g As Integer

        For x = 0 To width - 1
            For y = 0 To height - 1
                myColor = org.GetPixel(x, y)        'Bitmapの指定したピクセルの色を取得
                g = Average(myColor)
                If g < threshold Then
                    img.SetPixel(x, y, Color.White)
                Else
                    img.SetPixel(x, y, Color.Black)
                End If
            Next y
        Next x
    End Sub

    Private Function Average(ByRef c As Color) As Integer
        Dim g As Integer
        g = 0.298912 * c.R + 0.586611 * c.G + 0.114478 * c.B     'グレースケール化
        Return g
    End Function

    Public Sub Magnify_cut(ByVal Rate As Double)            '拡大・縮小
        '▼引数のチェック

        If IsNothing(org) Then
            Throw New NullReferenceException("元画像(org)に値が設定されていません。")
        End If

        If CInt(org.Size.Width * Rate) <= 0 OrElse CInt(org.Size.Height * Rate) <= 0 Then
            Throw New ArgumentException("処理後の画像のサイズが0以下になります。Rateには十分大きな値を指定してください。")
        End If

        '▼処理後の大きさの空の画像を作成

        NewRect.Width = CInt(org.Size.Width * Rate / 100.0)
        NewRect.Height = CInt(org.Size.Height * Rate / 100.0)


        '▼拡大・縮小実行
        img = New Bitmap(NewRect.Width, NewRect.Height)

        Dim g As Graphics = Graphics.FromImage(img)
        g.DrawImage(org, NewRect)

    End Sub

    Public Sub Blur_effect(Optional ByVal Value As Integer = 1)  'ぼかし

        Dim Kernel(,) As Integer

        Value = 3 + 2 * (Value - 1) '3, 5, 7 .... となる等差数列

        'Value * Value の正方行列を作成
        Kernel = CType(Array.CreateInstance(GetType(Integer), Value, Value), Integer(,))

        Dim X As Integer
        Dim Y As Integer
        Dim Center As Integer = Value \ 2

        '行列のすべての要素を1にする。
        For Y = 0 To Value - 1
            For X = 0 To Value - 1
                Kernel(X, Y) = 1
            Next
        Next

        '行列の中央の要素を2にする。
        Kernel(Center, Center) = 2

        img = ApplyKernel(org, Kernel, Value * Value + 1)

    End Sub

    Private Function ApplyKernel(ByVal SourceImage As Image, ByVal Kernel(,) As Integer,
                                 ByVal Weight As Integer, Optional ByVal RUpper As Integer = 0,
                                    Optional ByVal GUpper As Integer = 0, Optional ByVal BUpper As Integer = 0) As Bitmap
        Dim i As Integer
        Dim j As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim XS As Integer               '画像の幅の始まり
        Dim YS As Integer               '画像の高さの始まり
        Dim R As Integer
        Dim G As Integer
        Dim B As Integer

        Dim C As Color
        Dim Edge As Integer = (Kernel.GetLength(0) \ 2) * 2 '画像のへりの部分のピクセル数
        Dim MatrixSize As Integer = Kernel.GetLength(0)
        Dim Void As Integer

        org = CType(SourceImage, Bitmap)                    '元画像
        img = New Bitmap(org.Width, org.Height)             '変換後の画像の幅と高さ

        For j = 0 To org.Width - 1
            For i = 0 To org.Height - 1


                R = 0
                G = 0
                B = 0

                'ドット(j, i)の新しい色を算出します。
                Void = 0                                    '画像の無駄の部分の初期値
                For X = 0 To MatrixSize - 1
                    For Y = 0 To MatrixSize - 1
                        XS = X - Edge / 2
                        YS = Y - Edge / 2
                        'XE = X + Edge
                        'YE = Y + Edge

                        If (j + XS >= 0) And (i + YS >= 0) And (j + XS <= org.Width - 1) And (i + YS <= org.Height - 1) Then
                            '有効
                            C = org.GetPixel(j + XS, i + YS)
                            R = (C.R * Kernel(X, Y)) + R
                            G = (C.G * Kernel(X, Y)) + G
                            B = (C.B * Kernel(X, Y)) + B
                        Else
                            '無効
                            Void = Void + 1                 '無効カウントアップ
                        End If
                    Next Y
                Next X

                'R, G, Bの値が～の範囲に収まるように調節します。

                R = RGBRange((R + RUpper) \ (Weight - Void))
                G = RGBRange((G + GUpper) \ (Weight - Void))
                B = RGBRange((B + BUpper) \ (Weight - Void))

                img.SetPixel(j, i, Color.FromArgb(R, G, B))

            Next i
        Next j

        Return img
    End Function

    Private Function RGBRange(ByVal Value As Integer) As Integer
        Select Case Value
            Case Is < 0
                Return 0
            Case Is > 255
                Return 255
            Case Else
                Return Value
        End Select

    End Function

    Public Sub Sharpening(Optional ByVal Clear As Integer = 1)       '先鋭化
        Dim Kernel(,) As Integer

        Clear = 3 + 2 * (Clear - 1) '3, 5, 7 .... となる等差数列

        'Value * Value の正方行列を作成
        Kernel = CType(Array.CreateInstance(GetType(Integer), Clear, Clear), Integer(,))

        Dim X As Integer
        Dim Y As Integer
        Dim Center As Integer = Clear \ 2

        '行列のすべての要素を-1にする。
        For Y = 0 To Clear - 1
            For X = 0 To Clear - 1
                Kernel(X, Y) = -1
            Next
        Next

        '行列の中央の要素をセットする。
        Kernel(Center, Center) = Clear * Clear

        img = Sharp(org, Kernel, 1)


    End Sub

    Private Function Sharp(ByVal SourceImage As Image, ByVal Kernel(,) As Integer,
                                    ByVal Weight As Integer, Optional ByVal RUpper As Integer = 0,
                                        Optional ByVal GUpper As Integer = 0, Optional ByVal BUpper As Integer = 0) As Bitmap
        Dim i As Integer
        Dim j As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim XS As Integer
        Dim YS As Integer
        Dim R As Integer
        Dim G As Integer
        Dim B As Integer

        Dim C As Color
        Dim Edge As Integer = (Kernel.GetLength(0) \ 2) * 2 '画像のへりの部分のピクセル数
        Dim MatrixSize As Integer = Kernel.GetLength(0)
        Dim Void As Integer

        org = CType(SourceImage, Bitmap)
        img = New Bitmap(org.Width, org.Height)

        For j = 0 To org.Width - 1
            For i = 0 To org.Height - 1


                R = 0
                G = 0
                B = 0

                'ドット(j, i)の新しい色を算出します。
                Void = 0
                For X = 0 To MatrixSize - 1
                    For Y = 0 To MatrixSize - 1
                        XS = X - Edge / 2
                        YS = Y - Edge / 2
                        'XE = X + Edge
                        'YE = Y + Edge

                        If (j + XS >= 0) And (i + YS >= 0) And (j + XS <= org.Width - 1) And (i + YS <= org.Height - 1) Then
                            '有効
                            C = org.GetPixel(j + XS, i + YS)
                            R = (C.R * Kernel(X, Y)) + R
                            G = (C.G * Kernel(X, Y)) + G
                            B = (C.B * Kernel(X, Y)) + B
                        Else
                            '無効
                            Void = Void + 1
                        End If
                    Next Y
                Next X

                'R, G, Bの値が～の範囲に収まるように調節します。

                R = RGBRange((R + RUpper) \ (Weight - Void))
                G = RGBRange((G + GUpper) \ (Weight - Void))
                B = RGBRange((B + BUpper) \ (Weight - Void))

                img.SetPixel(j, i, Color.FromArgb(R, G, B))

            Next i
        Next j

        Return img

    End Function

    Private Function RGBColor(ByVal Value As Integer) As Integer
        Select Case Value
            Case Is < 0
                Return 0
            Case Is > 255
                Return 255
            Case Else
                Return Value
        End Select

    End Function

    Public Sub Contrast(ByVal contrast As Double)
        Dim level As Double = 127.5
        Dim bmp As New Bitmap(img)

        For y As Integer = 0 To bmp.Height - 1
            For x As Integer = 0 To bmp.Width - 1
                Dim pixel As Color = bmp.GetPixel(x, y)
                Dim r, g, b As Byte

                r = Math.Min(Math.Max(Byte.MinValue, _
                                      (pixel.R - level) * contrast + level), Byte.MaxValue)
                g = Math.Min(Math.Max(Byte.MinValue, _
                                      (pixel.G - level) * contrast + level), Byte.MaxValue)
                b = Math.Min(Math.Max(Byte.MinValue, _
                                      (pixel.B - level) * contrast + level), Byte.MaxValue)
                bmp.SetPixel(x, y, Color.FromArgb(r, g, b))

            Next
        Next

        img = bmp
    End Sub

    Sub Output(ByRef out As System.Web.HttpResponse, ByRef format As String)

        Dim ExtName As String

        ExtName = IO.Path.GetExtension(format)  '例 「a.jpg」の「.jpg」を取得

        'imageの後に拡張子が任意で出来るように(if文)
        If ExtName = ".jpg" Then
            out.ContentType = "image/jpg"
            out.AppendHeader("content-disposition", "inline; filename=" + format)
            out.Flush()
            img.Save(out.OutputStream, ImageFormat.Jpeg)
        End If

        If ExtName = ".png" Then
            out.ContentType = "image/png"
            out.AppendHeader("content-disposition", "inline; filename=" + format)
            out.Flush()
            img.Save(out.OutputStream, ImageFormat.Png)
        End If

        If ExtName = ".gif" Then
            out.ContentType = "image/gif"
            out.AppendHeader("content-disposition", "inline; filename=" + format)
            out.Flush()
            img.Save(out.OutputStream, ImageFormat.Gif)
        End If

        If ExtName = ".bmp" Then
            out.ContentType = "image/bmp"
            out.AppendHeader("content-disposition", "inline; filename=" + format)
            out.Flush()
            img.Save(out.OutputStream, ImageFormat.Bmp)
        End If

        out.End()
        out.Close()
    End Sub
End Class
