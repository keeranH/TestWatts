<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Omu.Awesome.Mvc.Helpers.AjaxDropdownInfo>" %>
<%@ Import Namespace="Omu.Awesome.Mvc" %>
<%
    var o = AwesomeTools.MakeId(Model.Prop, Model.Prefix);
    var p = AwesomeTools.MakeId(Model.ParentId);
%>
<input type="hidden" name="<%=Model.Prop %>" id="<%=o %>" value="<%=Model.Value %>"/>
<select id='<%=o %>dropdown' <%=Model.HtmlAttributes %> ></select>
<script type="text/javascript">   
ae_ajaxDropdown('<%=o %>','<%=p %>','<%=Url.Action("GetItems", Model.Controller) %>');
</script>
