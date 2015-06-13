<%@ Page Language="C#" MasterPageFile="Template.Master" Inherits="System.Web.Mvc.ViewPage<Enum>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Data" runat="server">

<%
    var val =
        Enum.GetNames(Model.GetType()).Select(
            o => new SelectListItem {Text = o, Value = o, Selected = o == Model.ToString()});

%>
<%=Html.DropDownList("", val) %>
</asp:Content>