<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="System_Report_Query" %>

<%@ Register Src="~/Common/Date.ascx" TagPrefix="uc1" TagName="Date" %>
<%@ Register Src="~/Common/wucDateTime.ascx" TagPrefix="uc1" TagName="wucDateTime" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title"><span class="ReqItem">*</span>報表類型</th>
            <td colspan="3">
                <asp:DropDownList ID="ddlReportType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged">
                    <asp:ListItem Value="">請選擇</asp:ListItem>
                    <asp:ListItem Value="審查案件一覽表">審查案件一覽表</asp:ListItem>
                    <asp:ListItem Value="裝置完成統計表">裝置完成統計表(名冊)</asp:ListItem>
                    <asp:ListItem Value="核銷統計表(名冊)">核銷統計表(名冊)</asp:ListItem>
                    <asp:ListItem Value="合約院所執行表">合約院所執行表</asp:ListItem>
                    <asp:ListItem Value="滿意度調查表">滿意度調查表</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="trReport1_1" runat="server" visible="false">
            <th class="td-title">收件日期區間</th>
            <td colspan="3">自<uc1:wucDateTime runat="server" ID="txtSDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
                至<uc1:wucDateTime runat="server" ID="txtEDate" IsShowTime="false" IsShowDefaultDate="false" DateTimeValidatorEnableFlag="false" />
            </td>
        </tr>
        <tr id="trReport1_2" runat="server" visible="false">
            <th class="td-title">會議類型</th>
            <td>
                <asp:DropDownList ID="ddlMeetingType" runat="server">
                    <asp:ListItem Value="">全部</asp:ListItem>
                    <asp:ListItem Value="1">專業審查</asp:ListItem>
                    <asp:ListItem Value="2">核銷審查</asp:ListItem>
                </asp:DropDownList>
            </td>
            <th class="td-title">資料類型</th>
            <td>
                <asp:DropDownList ID="ddlDataType" runat="server">
                    <asp:ListItem Value="">全部</asp:ListItem>
                    <asp:ListItem Value="1">案件數量</asp:ListItem>
                    <asp:ListItem Value="2">案件明細</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

    </table>
    <div style="float: right">
        <asp:Button ID="btnExport" runat="server" Text="匯出" OnClientClick="CCMS_DisableLockUI()" OnClick="btnExport_Click" CssClass="btn" />
    </div>
</asp:Content>

