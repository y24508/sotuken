<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="FileUpLoad.aspx.vb" Inherits="FileUpLoad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>
        &nbsp;</p>
    <p>
        <asp:Label ID="Label1" runat="server" 
            style="font-weight: 700; font-size: x-large; color: #00CC99; font-family: HGP明朝B;" 
            Text="画像をアップロードしてください"></asp:Label>
    </p>
    <p>
        <br />
&nbsp;<asp:FileUpload ID="FileUpload1" runat="server" />
    </p>
    <p>
        &nbsp;</p>
    <p>
        <asp:Label ID="UploadStatusLabel" runat="server" style="color: #FF0000"></asp:Label>
    </p>
    <p>
        <asp:Button ID="UploadButton" runat="server" Text="アップロード" Width="97px" />
    </p>
    <p>
        &nbsp;&nbsp;</p>
</asp:Content>

