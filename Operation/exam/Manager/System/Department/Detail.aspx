<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_Department_Detail" %>

<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>
    <div class="Detail-table-title"><span>院所基本資料</span></div>
    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title" style="width: 15%"><span class="ReqItem"></span>院所編號</th>
            <td style="width: 35%">
                <asp:Label ID="lbSN" runat="server" ClientIDMode="Static"></asp:Label>
            </td>
            <th class="td-title" style="width: 15%"><span class="ReqItem" style="color: red;">*</span>院所名稱</th>
            <td style="width: 35%">
                <asp:TextBox ID="tbxDeptName" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>院所電話</th>
            <td>
                <asp:TextBox ID="tbxTel" runat="server" Width="95%"></asp:TextBox>
            </td>
             <th class="td-title"><span class="ReqItem"></span>院所傳真</th>
            <td>
                <asp:TextBox ID="txtFax" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <th class="td-title"><span class="ReqItem"></span>鄉巿鎮</th>
            <td>
                <asp:DropDownList ID="ddlAreaSN" runat="server" ClientIDMode="Static" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true" />
                <asp:HiddenField ID="hdAreaID" runat="server" />
            </td>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>郵遞區號</th>
            <td>
                <asp:Label ID="lbPostId" runat="server" ClientIDMode="Static"></asp:Label>
            </td>
            
        </tr>
        <tr>
             <th class="td-title"><span class="ReqItem" style="color: red;">*</span>地址<br />
                <br />
                <span class="ReqItem" style="color: red;">(請寫村里路名)</span></th>
            <td colspan="3">
                <asp:TextBox ID="tbxAddr" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem"></span>院所統編</th>
            <td>
                <asp:TextBox ID="tbxDID" runat="server" Width="95%"></asp:TextBox>
            </td>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>院所狀態</th>
            <td>
                <asp:RadioButtonList ID="rdbStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1" Selected="True">啟用</asp:ListItem>
                    <asp:ListItem Value="2">停用</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>

        <tr>
            <th class="td-title"><span class="ReqItem"></span>停用說明</th>
            <td colspan="3">
                <asp:TextBox ID="tbxStatusDesc" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
    </table>

    <br />
    <br />
    <div class="Detail-table-title"><span>匯款入帳資料</span></div>
    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title" style="width: 15%"><span class="ReqItem"></span>入帳銀行</th>
            <td style="width: 35%">
                <asp:TextBox ID="tbxBankName" runat="server" Width="95%"></asp:TextBox>
            </td>
            <th class="td-title" style="width: 15%"><span class="ReqItem"></span>入帳戶名</th>
            <td style="width: 35%">
                <asp:TextBox ID="tbxBankAccID" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem"></span>存款帳號</th>
            <td colspan="3">
                <asp:TextBox ID="tbxBankID" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
    </table>

    <br />
    <br />
    <div class="Detail-table-title"><span>院所IP位址</span></div>
    <br />
    <asp:Panel runat="server" ID="panelIP">
        <table class="Detail-table" width="100%">
            <tr>
                <th class="td-title" style="width: 15%"><span class="ReqItem"></span>IP位址</th>
                <td style="width: 75%">
                    <asp:TextBox ID="tbxIP" runat="server" Width="95%"></asp:TextBox>
                </td>
                <td style="width: 5%">
                    <asp:Button ID="btnAdd" runat="server" Text="➕" class="edit-btn" OnClick="BtnAdd_Click" />
                </td>
            </tr>
        </table>
        <asp:GridView ID="gvIndex" runat="server" CssClass="case-list-table" AutoGenerateColumns="False"
            EmptyShowHeader="True" Width="100%" OnRowDataBound="gvIndex_RowDataBound" OnRowCommand="gvIndex_RowCommand"
            GridLines="None" AllowPaging="false" AllowSorting="True" OnDataBound="gvIndex_DataBound"
            odsSortReverse="False">
            <Columns>
                <asp:TemplateField HeaderText="功能">
                    <ItemTemplate>
                        <asp:Button ID="btnDel" runat="server" Text="刪除" CssClass="img-btn delete" title="刪除" CommandName="btnDel" CommandArgument='<%# Eval("IP") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" HeaderStyle-HorizontalAlign="Center">
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <HeaderStyle Width="10%"></HeaderStyle>
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </asp:Panel>

    <asp:Literal runat="server" ID="li" Visible="false" />

    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

