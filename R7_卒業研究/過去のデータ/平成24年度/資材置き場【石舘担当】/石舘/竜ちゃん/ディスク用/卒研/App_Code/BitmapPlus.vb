Imports System.Drawing
Imports System.Drawing.Imaging

Public Class Maximum
    ''' <summary>
    ''' オリジナルのBitmapオブジェクト
    ''' </summary>
    Private _bmp As Bitmap = Nothing

    ''' <summary>
    ''' Bitmapに直接アクセスするためのオブジェクト
    ''' </summary>
    Private _img As BitmapData = Nothing

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="original"></param>
    Public Sub New(ByVal original As Bitmap)
        ' オリジナルのBitmapオブジェクトを保存
        _bmp = original
    End Sub

    ''' <summary>
    ''' Bitmap処理の高速化開始
    ''' </summary>
    Public Sub BeginAccess()
        ' Bitmapに直接アクセスするためのオブジェクト取得(LockBits)
        _img = _bmp.LockBits(New Rectangle(0, 0, _bmp.Width, _bmp.Height), _
            System.Drawing.Imaging.ImageLockMode.ReadWrite, _
            System.Drawing.Imaging.PixelFormat.Format24bppRgb)
    End Sub

    ''' <summary>
    ''' Bitmap処理の高速化終了
    ''' </summary>
    Public Sub EndAccess()
        If _img Is Nothing = False Then
            ' Bitmapに直接アクセスするためのオブジェクト開放(UnlockBits)
            _bmp.UnlockBits(_img)
            _img = Nothing
        End If
    End Sub

    ''' <summary>
    ''' BitmapのGetPixel同等
    ''' </summary>
    ''' <param name="x">Ｘ座標</param>
    ''' <param name="y">Ｙ座標</param>
    ''' <returns>Colorオブジェクト</returns>
    Public Function GetPixel(ByVal x As Integer, ByVal y As Integer) As Color
        If _img Is Nothing = True Then
            ' Bitmap処理の高速化を開始していない場合はBitmap標準のGetPixel
            Return _bmp.GetPixel(x, y)
        End If
        Dim adr As IntPtr = _img.Scan0
        Dim pos As Integer = x * 3 + _img.Stride * y
        Dim b As Byte = System.Runtime.InteropServices.Marshal.ReadByte(adr, pos + 0)
        Dim g As Byte = System.Runtime.InteropServices.Marshal.ReadByte(adr, pos + 1)
        Dim r As Byte = System.Runtime.InteropServices.Marshal.ReadByte(adr, pos + 2)
        Return Color.FromArgb(r, g, b)
    End Function


    '''// <summary>
    ''' BitmapのSetPixel同等
    ''' </summary>
    ''' <param name="x">Ｘ座標</param>
    ''' <param name="y">Ｙ座標</param>
    ''' <param name="col">Colorオブジェクト</param>
    Public Sub SetPixel(ByVal x As Integer, ByVal y As Integer, ByVal col As Color)
        If _img Is Nothing = True Then
            ' Bitmap処理の高速化を開始していない場合はBitmap標準のSetPixel
            _bmp.SetPixel(x, y, col)
            Return
        End If

        ' Bitmap処理の高速化を開始している場合はBitmapメモリへの直接アクセス
        Dim adr As IntPtr = _img.Scan0
        Dim pos As Integer = x * 3 + _img.Stride * y
        System.Runtime.InteropServices.Marshal.WriteByte(adr, pos + 0, col.B)
        System.Runtime.InteropServices.Marshal.WriteByte(adr, pos + 1, col.G)
        System.Runtime.InteropServices.Marshal.WriteByte(adr, pos + 2, col.R)
    End Sub


End Class
