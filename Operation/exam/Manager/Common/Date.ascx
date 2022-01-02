<%@ control language="C#" autoeventwireup="true" codefile="Date.ascx.cs" inherits="Common_Date" %>
<div style="display: inline-block;">
    <asp:TextBox ID="txtDateTime" runat="server" CssClass="Wdate" Columns="8"></asp:TextBox>
    <asp:DropDownList ID="ddlHour" runat="server" Style="display: none;">
    </asp:DropDownList>
    <asp:DropDownList ID="ddlMin" runat="server" Style="display: none;">
    </asp:DropDownList>

    <input id="btnClean" type="button" value="清除" class="edit-btn" runat="server" />
    <asp:Literal ID="litDate" runat="server" Text=""></asp:Literal>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        ControlToValidate="txtDateTime" ErrorMessage="請選擇日期" SetFocusOnError="True" Display="Dynamic"><span class="ReqItem">*</span></asp:RequiredFieldValidator>

</div>
<asp:Literal ID="litScript" runat="server"></asp:Literal>