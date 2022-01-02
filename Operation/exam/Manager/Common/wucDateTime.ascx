<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucDateTime.ascx.cs" Inherits="Common_wucDateTime" %>
<asp:TextBox ID="txtDateTime" runat="server" CssClass="Wdate" Width="130px"></asp:TextBox>

<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="Red" Display="Dynamic"    
    ControlToValidate="txtDateTime" ErrorMessage="請選擇日期" SetFocusOnError="True">請選擇日期</asp:RequiredFieldValidator>
<%--<input id="btnDateTime" type="button" value="月曆選單" runat="server" />--%>
<asp:Literal ID="litScript" runat="server"></asp:Literal>