<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SimpleLti12.WebForms.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            This is a simple Tool Consumer using 
            <a href="http://www.nuget.org/packages/LtiLibrary" target="_blank">LtiLibrary</a>
            in an ASP.NET Web Forms page. Click the link below to launch a tool.
        </p>
        <div>
            <a href="Launch.ashx">Launch</a>
        </div>
    </form>
</body>
</html>
