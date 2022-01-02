<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_Doctor_Detail" %>

<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>
<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="ButtonStyle_H1" CausesValidation="False" CssClass="btn" />
    </div>
    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title" style="width: 15%">醫師編號</th>
            <td>
                <asp:Label runat="server" ID="lbSN"></asp:Label></td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>服務院所</th>
            <td>
                <asp:DropDownList ID="ddlDeptSN" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlDeptSN_SelectedIndexChanged" />
                <%--<asp:HiddenField ID="hdAreaID" runat="server" />--%>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>醫師姓名</th>
            <td>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem">*</span>使用狀態</th>
            <td>
                <asp:RadioButtonList ID="rdbStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1" Selected="True">啟用</asp:ListItem>
                    <asp:ListItem Value="2">停用</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem"></span>備註</th>
            <td>
                <asp:TextBox ID="txtMemo" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>停用說明</th>
            <td>
                <asp:TextBox ID="txtStatusDesc" runat="server"></asp:TextBox>
            </td>
        </tr>

    </table>
    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

