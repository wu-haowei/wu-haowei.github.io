<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MP.master" AutoEventWireup="true" CodeFile="Query.aspx.cs" Inherits="System_Examiner_Query" %>

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
                <td>
                    <table>
                        <tr>
                            <td>委員姓名：
                                <asp:TextBox ID="txtKeyWord" runat="server" Width="204px"></asp:TextBox>
                            </td>
                            <td>狀態：
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlStatus" runat="server">
                                    <asp:ListItem Value="">全部</asp:ListItem>
                                    <asp:ListItem Value="1">啟用</asp:ListItem>
                                    <asp:ListItem Value="2">停用</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <span class="submit">
                                    <asp:Button ID="btnSearch" runat="server" Text="查詢" OnClick="btnSearch_Click" CssClass="btn" />
                                </span>

                            </td>
                            <td>
                                <span class="submit">
                                    <asp:Button ID="btnClick" runat="server" Text="清除" OnClick="btnClear_Click" CssClass="btn" />
                                </span>
                            </td>
                        </tr>
                    </table>

                </td>

                <td style="text-align: right; width: 30%">

                    <asp:Button ID="btnAdd" runat="server" Text="新增" CssClass="btn" />
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

            <asp:TemplateField HeaderText="動作">

                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="修改" CssClass="img-btn edit" title="修改" CommandName="btnEdit" />
                    <asp:Button ID="btnRead" runat="server" Text="查看" CssClass="img-btn read" title="查看" CommandName="btnRead" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>

            </asp:TemplateField>

            <asp:BoundField DataField="SN" HeaderText="委員編號" SortExpression="SN" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>

            <asp:BoundField DataField="Name" HeaderText="委員姓名" SortExpression="Name" HeaderStyle-HorizontalAlign="Center">
                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundField>

            <asp:TemplateField HeaderText="狀態">

                <ItemTemplate>
                    <asp:Label ID="lblStatus" runat="server" Text='<%# GetStatusName(Convert.ToInt32( Eval("Status" ).ToString())) %>'></asp:Label>

                    <%--                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status" ).ToString()=="0"?"鎖定":Eval("Status" ).ToString()=="1"?"啟用":"停用"%>' 
                        ForeColor='<%# Eval("Status" ).ToString()=="2"?System.Drawing.Color.Red:System.Drawing.Color.Black %>'></asp:Label>--%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>

            </asp:TemplateField>




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
        TypeName="Hamastar.BusinessObject.Comm_Examiner" EnablePaging="True" StartRowIndexParameterName="startRowIndex"
        MaximumRowsParameterName="maximumRows" SortParameterName="sortExpression" OnLoad="odsIndex_Load"></asp:ObjectDataSource>
</asp:Content>

