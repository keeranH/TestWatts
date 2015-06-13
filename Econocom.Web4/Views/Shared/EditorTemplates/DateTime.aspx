<%@ Page Language="C#" MasterPageFile="Template.Master" Inherits="System.Web.Mvc.ViewPage<DateTime?>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Data" runat="server">
  <% if (null!=Model)
     { %>
    <%= Html.TextBox("", ((DateTime)Model).ToShortDateString())%>
<% }
     else
     { %>
     <%= Html.TextBox("", "") %>
<% } %>
    <button onclick="resetDate()" type="reset" >Clear</button>
        <script type="text/javascript">
            $(function () {
                $("#<%=ViewData.TemplateInfo.GetFullHtmlFieldId(string.Empty)%>").datepicker();
            });

            function resetDate() {
                $("#<%=ViewData.TemplateInfo.GetFullHtmlFieldId(string.Empty)%>").val('');
            }
        </script>

</asp:Content>