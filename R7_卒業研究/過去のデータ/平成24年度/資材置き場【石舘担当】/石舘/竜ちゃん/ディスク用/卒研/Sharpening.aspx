<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" 
    CodeFile="Sharpening.aspx.vb" Inherits="Sharpening" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>
        &nbsp;</p>
    <p>
        <asp:Image ID="Image1" runat="server" />
            <%--<asp:Image ID="Image2" runat="server" Height="195px" ImageUrl="getimage3.aspx" 
            Width="197px" />--%>
    </p>
    <p>
        &nbsp;</p>
    <p>
        <asp:TextBox ID="txt1" runat="server" Width="66px"></asp:TextBox>
    </p>
    <p>
        <asp:TextBox ID="TextBox" runat="server" AutoPostBack="true" Value="0"></asp:TextBox>
            <asp:SliderExtender ID="TextBox_Slider" runat="server" 
                Enabled="true" TargetControlID="Textbox" Length="150" Minimum="1"
                    Maximum="10" BoundControlID="txt1">
        </asp:SliderExtender>
    </p>
    <p>
        &nbsp;</p>
</asp:Content>

