<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/DealData.master" AutoEventWireup="true" CodeFile="CaseLog.aspx.cs" Inherits="CaseLog" %>

<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">

    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title" style="width: 20%;">操作者姓名</th>
            <th class="td-title" style="width: 20%">動作</th>
            <th class="td-title" style="width: 40%">備註</th>
            <th class="td-title" style="width: 20%">時間</th>
        </tr>
        <asp:Literal runat="server" ID="litLog"></asp:Literal>
    </table>
</asp:Content>
