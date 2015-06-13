<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AutocompleteInfo>" %>
<%@ Import Namespace="Omu.Awesome.Mvc" %>
<%
    var o = AwesomeTools.MakeId(Model.Prop, Model.Prefix);
    var k = AwesomeTools.MakeId(Model.PropId, Model.Prefix);
    var p = AwesomeTools.MakeId(Model.ParentId);
%>
<input type="text" id="<%=o %>" name="<%=Model.Prop %>" value="<%=Model.Value %>"  <%=Model.HtmlAttributes %> />
<%if (Model.GeneratePropId)
  { %><input type="hidden" id="<%=k %>" name="<%=Model.PropId %>"
    value="<%=Model.PropIdValue %>" /><%} %>
<script type="text/javascript">
    ae_autocomplete('<%=o %>','<%=k %>','<%=p %>','<%=Url.Action("Search", Model.Controller) %>',<%=Model.MaxResults %>,<%=Model.Delay %>,<%=Model.MinLength %>);
</script>
