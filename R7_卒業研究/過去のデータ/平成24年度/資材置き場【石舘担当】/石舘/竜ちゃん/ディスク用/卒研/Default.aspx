<%@ Page Title="ホーム ページ" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>
        &nbsp;</p>
    <p>
        <%--       <asp:Image ID="Image1" runat="server" Height="195px" ImageUrl="getimage.aspx" 
            Width="197px" />--%>
            <asp:Image ID="Image1" runat="server"  ImageUrl="getimage.aspx" />
    
    </p>

    <p>
        <asp:Button ID="Magnify_cut" runat="server" Text="拡大・縮小" />
        　<asp:Button ID="Blur_effect" runat="server" Text="ぼかし" />
        　<asp:Button ID="Sharpening" runat="server" Text="鮮鋭化" />
        　<asp:Button ID="Contrast" runat="server" Text="コントラスト" />
          　<asp:Button ID="Binalize" runat="server" Text="2値化" Width="62px" />
    </p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
   
</asp:Content>