<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CaseMeeting.aspx.cs" Inherits="CaseMeeting" %>--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/DealData.master" AutoEventWireup="true" CodeFile="CaseMeeting.aspx.cs" Inherits="CaseMeeting" %>

<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">


    <table class="Detail-table" width="80%">
        <tr>
            <th class="td-title" style="width: 30%"><span class="ReqItem"></span>開會日期</th>
            <td>
                <asp:TextBox runat="server" ID="txtDate" />
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem"></span>委員姓名</th>
            <td>
                <asp:DropDownList ID="ddlProfUser" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlProfUser_SelectedIndexChanged" />
                <asp:HiddenField ID="hdProfUser" runat="server" />
            </td>
        </tr>
    </table>

    <asp:Button ID="btnPost" runat="server" Text="儲存" class="edit-btn" />
</asp:Content>
