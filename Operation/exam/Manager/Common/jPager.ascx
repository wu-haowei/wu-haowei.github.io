<%@ Control Language="C#" AutoEventWireup="true" CodeFile="jPager.ascx.cs" Inherits="Common_jPager" %>
<asp:HiddenField runat="server" ID="hidPage" Value ="0" />
<asp:HiddenField runat="server" ID="hidPageSize" Value ="7" />
<asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" style="display:none;" />
<asp:Button ID="Button2" runat="server" Text="Button" OnClick="Button2_Click" style="display:none;" />
<script>
    $(function () {
        $('.page > li ').each(function () {
            $(this).click(function () {
                console.log($(this).data('index'));
                $('#<%=hidPage.ClientID%>').val($(this).data('index'));
                $('#<%=Button1.ClientID%>').click();
            })
            $('.btn').click(function () {
                console.log($(this).data('index'));
                $('#<%=hidPageSize.ClientID%>').val($('#<%=ddlPageIndex.ClientID%>').val());
                $('#<%=Button2.ClientID%>').click();
            })
            
        })
        $('#<%=oGridView.ClientID %>').closest('div').addClass('tableWrap');
    })
</script>
<div class="area-customize pagination" runat="server" id="PagerUI" data-type="0" data-child="1">
    <div class="in">
        <div class="ct">
            <div class="in">
                <ul class="page" data-index="3" data-child="8">
                    <asp:Literal runat="server" ID="litContent"></asp:Literal>
                </ul>
                <ul data-index="3" data-child="3" class="single C_Pager" data-path="/tamplate10/News.aspx?n=14502&amp;sms=19768">
                    <li data-index="1">
                        <span class="select">
                            <label>每頁筆數</label>
                            <asp:DropDownList runat="server" ID="ddlPageIndex">
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                <asp:ListItem Text="200" Value="200"></asp:ListItem>
                            </asp:DropDownList>
                            <a class="btn" onkeypress="this.click();"><span>go</span></a>
                        </span>
                    </li>
                    <li data-index="3">
                        <span class="count"><i>/</i><%=TotalCount %></span>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</div>
