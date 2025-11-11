<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Binalize.aspx.vb" Inherits="Threshold" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>
        &nbsp;</p>
    <p>
        <asp:Image ID="Image1" runat="server"  />
       <%-- <asp:Image ID="Image1" runat="server" ImageUrl="getimage5.aspx" />--%>

    </p>
    <p>
        <asp:TextBox ID="txt1" runat="server" Width="66px"></asp:TextBox>
    </p>
    <p>
        <asp:TextBox ID="TextBox" runat="server" AutoPostBack="true" Value="128"></asp:TextBox>
            <asp:SliderExtender ID="TextBox_Slider" runat="server" 
                Enabled= "true" TargetControlID="Textbox" Length="255" Minimum="0" 
                    Maximum="255" BoundControlID="txt1">
                
</asp:SliderExtender>
    </p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <p>
    </p>
</asp:Content>

