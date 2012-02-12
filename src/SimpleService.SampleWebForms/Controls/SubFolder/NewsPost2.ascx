<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsPost2.ascx.cs" Inherits="SimpleService.SampleWebForms.Controls.SubFolder.NewsPost2" %>
<h1>Here's a list</h1>

<b><asp:Literal Text="text" runat="server"  ID="literalName" /></b>

<ul>
<asp:Repeater ID="MyList" runat="server">
    <ItemTemplate>
        <li><%# Container.DataItem %></li>
    </ItemTemplate>
</asp:Repeater>
</ul>