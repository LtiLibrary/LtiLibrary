<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tool.aspx.cs" Inherits="SimpleLti12.WebForms.Tool" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            This is a simple Tool Provider using
            <a href="http://www.nuget.org/packages/LtiLibrary" target="_blank">LtiLibrary</a>
            in an ASP.NET Web Forms page.
        </p>
        <asp:Panel ID="OutcomesPanel" runat="server">
            <h2>Outcomes Service</h2>
            <p>The Tool Consumer that sent this launch can provide the LTI 1.x Outcomes Service.</p>
        </asp:Panel>
        <asp:Panel ID="ProfilePanel" runat="server">
            <h2>Tool Consumer Profile</h2>
            <p>The Tool Consumer that sent this launch can provide a Tool Consumer Profile.</p>
        </asp:Panel>
        <asp:Panel ID="RawPanel" runat="server">
            <h2>Raw POST Parameters</h2>
            <asp:Repeater ID="postParameters" runat="server" OnItemDataBound="postParameters_ItemDataBound">
                <ItemTemplate>
                    <div><%# Container.DataItem %>=<asp:Literal ID="Value" runat="server" /></div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </form>
</body>
</html>
