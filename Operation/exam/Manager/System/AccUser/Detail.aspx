<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="System_AccUser_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div align="right">
        <asp:Button ID="btnGoBack" runat="server" Text="回上一頁" class="btn" CausesValidation="False" />
    </div>

    <table class="Detail-table" width="100%">
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>姓名</th>
            <td>
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="請輸入姓名" ControlToValidate="txtName" ForeColor="Red"></asp:RequiredFieldValidator>

            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>帳號</th>
            <td>
                <asp:TextBox ID="txtAccount" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="請輸入帳號" ControlToValidate="txtName" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>密碼</th>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="新密碼必須包含4個字以上的英數字混合" ForeColor="Red" ValidationExpression="^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{4,}$"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="請輸入密碼" ControlToValidate="txtPassword" ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th class="td-title"><span class="ReqItem" style="color: red;">*</span>啟用狀態</th>
            <td>
                <asp:RadioButtonList ID="rdbEnabel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1" Selected="True">啟用</asp:ListItem>
                    <asp:ListItem Value="2">停用</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
         <tr>
            <th class="td-title"><span class="ReqItem"></span>停用說明</th>
            <td>
                 <asp:TextBox ID="tbxStatusDesc" runat="server" Width="80%"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div style="text-align: center; padding-top: 10px">
        <asp:Button ID="btnSave" runat="server" Text="確定" class="edit-btn" OnClick="btnSave_Click" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="取消" class="edit-btn" CausesValidation="False" />
    </div>
</asp:Content>

