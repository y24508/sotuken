<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="explanation.aspx.vb" Inherits="explanation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <br />
    <br />
    <asp:Label ID="Label1" runat="server" 
        style="font-size: x-large; font-weight: 700" Text="画像変換サイトについて"></asp:Label>
    <br />
    <br />
    <br />
    <asp:Label ID="Label2" runat="server" 
        style="font-size: large; font-weight: 700" 
        Text="この画像変換サイトを使用する上でいくつかの注意点があります。"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label3" runat="server" 
        Text="1) ぼかし・鮮鋭化については画像サイズが大きくなるにつれて画像の変換処理が遅くなります。" 
        style="font-size: medium; font-weight: 700"></asp:Label>
    <br />
    　<asp:Label ID="Label4" runat="server" 
        style="font-size: medium; font-weight: 700" 
        Text="※高速処理を用いてデバックしたところ, 480×240の画像サイズで, パラメータの値を10にしたところ約50秒ほどかかりました。"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label5" runat="server" 
        Text="2) デジカメで撮った写真など極端に大きい画像は最悪変換できない場合があるので, 大体200KB位まで縮小してやってみて下さ" 
        style="font-size: medium; font-weight: 700"></asp:Label>
    <br />
&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Label ID="Label6" runat="server" 
        style="font-size: medium; font-weight: 700" Text="い。"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label7" runat="server" 
        Text="3) 変換後の画像の保存する際の拡張子は、元画像と同じかBMP形式です。" 
        style="font-size: medium; font-weight: 700"></asp:Label>
    <br />
    　<asp:Label ID="Label8" runat="server" 
        Text="例えば、元画像が「A.jpg」の場合、画像を変換して保存する際には、「A.jpg」か「A.bmp」で保存されます。" 
        style="font-size: medium; font-weight: 700"></asp:Label>
    <br />
    　<asp:Label ID="Label9" runat="server" Text="また、変換後の画像の名前は自分で任意に決めれます。" 
        style="font-size: medium; font-weight: 700"></asp:Label>
    <br />
    <br />
    <br />
</asp:Content>

