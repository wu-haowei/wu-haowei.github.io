<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TimeOut.aspx.cs" Inherits="TimeOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function init() {
            alert('您已逾時或未經系統驗證!');
            location.href = 'Login.aspx';
        }
    </script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
