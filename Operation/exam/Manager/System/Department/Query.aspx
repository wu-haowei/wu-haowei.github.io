<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="System_Department_Query" %>

<%@ Register Assembly="Hamastar.GridViewHead" Namespace="GridViewHead" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="fieldset" style="width: 100%">

        <table class="search-box" style="width: 100%">
            <tr>
                <td>
                    <asp:Literal ID="litNavigation" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>院所名稱：
                    <asp:TextBox ID="tbxKeyWordName" runat="server" Width="204px"></asp:TextBox>
                </td>
                <td>院所編號：
                    <asp:TextBox ID="tbxKeyWordSN" runat="server" Width="100px"></asp:TextBox>
                </td>
                <td>鄉鎮巿區：
                    <asp:DropDownList ID="ddlAreaSN" runat="server" ClientIDMode="Static" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true" />
                    <asp:HiddenField ID="hdAreaID" runat="server" />
                </td>
                <td style="text-align: right;">
                    <span class="submit">
                        <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" CssClass="btn" />
                    </span>
                    <asp:Button ID="btnClear" runat="server" Text="清除" OnClick="btnClear_Click" CssClass="btn" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right;" colspan="8">
                    <asp:Button ID="btnAdd" runat="server" Text="＋ 新增院所" CssClass="btn" />
                    <asp:Button ID="btnRecord" runat="server" Text="操作紀錄" CssClass="btn" OnClick="btnRecord_Click" Visible="false" />
                </td>
            </tr>
        </table>

    </div>
    <asp:GridViewHeadText ID="gvIndex" runat="server" CssClass="case-list-table" AutoGenerateColumns="False" DataKeyNames="SN"
        EmptyShowHeader="True" Width="100%" OnRowDataBound="gvIndex_RowDataBound" OnRowCommand="gvIndex_RowCommand"
        GridLines="None" AllowPaging="True" AllowSorting="True" OnDataBound="gvIndex_DataBound"
        DataSourceID="odsIndex" odsSortReverse="False">
        <Columns>
            <asp:TemplateField HeaderText="功能">
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="修改" CssClass="img-btn edit" title="修改" CommandName="btnEdit" CommandArgument='<%# Eval("SN") %>' />
                    <asp:Button ID="btnRead" runat="server" Text="檢視" CssClass="img-btn ques" title="檢視" CommandName="btnRead" CommandArgument='<%# Eval("SN") %>' />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="DeptName" HeaderText="院所名稱" SortExpression="DeptName" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="SN" HeaderText="院所代碼" SortExpression="SN" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Name" HeaderText="鄉鎮巿區" SortExpression="Name" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Tel" HeaderText="院所電話" SortExpression="Tel" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Addr" HeaderText="院所地址" SortExpression="Addr" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>

        </Columns>

    </asp:GridViewHeadText>


    <div class="list_gotopage" style="text-align: center; padding: 10px 0 0 0;">
        <asp:DataPager ID="DataPager1" runat="server" PagedControlID="gvIndex">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        共
                                            <%# Container.TotalRowCount %>
                                            筆 |
                                            <%# Container.TotalRowCount > 0 ? Math.Ceiling(((double)(Container.StartRowIndex + Container.MaximumRows) / Container.MaximumRows)) : 0 %>
                                            /
                                            <%# Math.Ceiling((double)Container.TotalRowCount / Container.MaximumRows)%>
                                            頁 |
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowLastPageButton="True"
                    FirstPageText="第一頁 | " LastPageText="最後一頁" NextPageText="下一頁 | " PreviousPageText="上一頁 | " />
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        到第
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtCurrentPage"
                                                ErrorMessage="頁數指定錯誤!" ValidationExpression="\d{1,6}" ValidationGroup="gv">*</asp:RegularExpressionValidator>
                        <asp:TextBox ID="txtCurrentPage" runat="server" MaxLength="6" Width="20"></asp:TextBox>
                        頁 : 每頁
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtPageSize"
                                                ErrorMessage="頁數大小指定錯誤!" ValidationExpression="\d{1,6}" ValidationGroup="gv">*</asp:RegularExpressionValidator>
                        <asp:TextBox ID="txtPageSize" runat="server" MaxLength="6" Width="20" Text='<%# Container.MaximumRows  %>'></asp:TextBox>
                        筆
                    </PagerTemplate>
                </asp:TemplatePagerField>
            </Fields>
        </asp:DataPager>
        <asp:Button ID="btnGo" runat="server" Text=" Go " OnClick="btnGo_Click" ValidationGroup="gv" CssClass="btn" />
    </div>


    <asp:ObjectDataSource ID="odsIndex" runat="server" SelectMethod="GetListData" SelectCountMethod="GetListCount"
        TypeName="Hamastar.BusinessObject.vw_Department" EnablePaging="True" StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows" SortParameterName="sortExpression" OnLoad="odsIndex_Load"></asp:ObjectDataSource>
</asp:Content>

