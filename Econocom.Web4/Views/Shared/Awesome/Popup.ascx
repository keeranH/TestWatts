<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Omu.Awesome.Mvc.Helpers.PopupInfo>" %>
<%@ Import Namespace="Omu.Awesome.Mvc.Helpers" %>
<%
    var o = "p" + Model.Prefix + (Model.Action + Model.Controller).ToLower();
%>

<script type="text/javascript">
        $(function () {ae_popup('<%=o %>',<%=Model.Width %>, <%=Model.Height %>, '<%=Model.Title %>', <%=Model.Modal.ToString().ToLower() %>, <%=Model.Position %>, <%=Model.Resizable.ToString().ToLower() %>, {<%var i = 0;foreach (var button in Model.Buttons){i++;%>  "<%=button.Key %>" : <%=button.Value %><%=i == Model.Buttons.Count ? "": "," %><%} %>});});
        var l<%=o %> = null;
        function call<%=o %>(<%=JsTools.MakeParameters(Model.Parameters) %>) { 
            if(l<%=o %> != null) return;
            l<%=o %> = true;
            <%if(Model.Content == null)
              {%>
            $.get('<%=Url.Action(Model.Action, Model.Controller) %>',            
            <%=JsTools.JsonParam(Model.Parameters) %>            
            function(d){
            l<%=o %> = null;
            $("#<%=o %>").html(d).dialog('open');
            });
            <%
              }else
              {%>
            $("#<%=o %>").dialog('open');  
            l<%=o %> = null;
              <%}%>            
        }  
</script>
<div id="<%=o %>" style="max-width: 650px!important; background-color: yellow;">
<%=Model.Content %>
</div>
