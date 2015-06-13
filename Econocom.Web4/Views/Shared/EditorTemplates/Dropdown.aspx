<%@ Page Language="C#" MasterPageFile="Template.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Data" runat="server">
    <%= Html.DropDownList("", ViewData.TemplateInfo.FormattedModelValue as IEnumerable<SelectListItem>) %>
</asp:Content>