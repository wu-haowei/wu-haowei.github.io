<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_RejectDesc_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>
    <table class="Detail-table" width="100%">

        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>審核類型</th>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="Catregory" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Selected="True" Value="1">行政審核</asp:ListItem>
                            <asp:ListItem Value="2">專業審核</asp:ListItem>
                            <asp:ListItem Value="3">核銷審核</asp:ListItem>
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>退件事由說明</th>
            <td>
                <asp:TextBox ID="txtName" runat="server" MaxLength="100" Width="500px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName" ErrorMessage="必填" ForeColor="#FF3300"></asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red">*</span>狀態</th>
            <td>
                <asp:RadioButtonList ID="rdbEnabel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1" Selected="True">啟用</asp:ListItem>
                    <asp:ListItem Value="2">停用</asp:ListItem>
                    <%--<asp:ListItem Value="0">鎖定</asp:ListItem>--%>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <th class="td-title">停用說明</th>
            <td>
                <asp:TextBox ID="Deactivateillustrate" runat="server" Width="500px" MaxLength="255"></asp:TextBox>
            </td>
        </tr>

    </table>
    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

