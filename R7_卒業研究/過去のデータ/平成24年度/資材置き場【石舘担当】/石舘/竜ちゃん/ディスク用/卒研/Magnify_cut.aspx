<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" 
    CodeFile="Magnify_cut.aspx.vb" Inherits="Magnify_cut" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>
        &nbsp;</p>
    <p>
        <asp:Image ID="Image1" runat="server"  />
       <%-- <asp:Image ID="Image1" runat="server" ImageUrl="getimage1.aspx?threshold=100" />--%>

    </p>
    <p>
        <asp:TextBox ID="txt1" runat="server" Width="66px"></asp:TextBox>
    </p>
    <p>
        <asp:TextBox ID="TextBox" runat="server" AutoPostBack="true" Value="100"></asp:TextBox>
            <asp:SliderExtender ID="TextBox_Slider" runat="server" 
                Enabled= "true" TargetControlID="Textbox" Length="200" Minimum="1" 
                    Maximum="200" BoundControlID="txt1">
            </asp:SliderExtender>
    </p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
</asp:Content>

