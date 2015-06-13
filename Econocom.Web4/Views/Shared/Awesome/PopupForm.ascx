<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Omu.Awesome.Mvc.Helpers.PopupFormInfo>" %>
<%@ Import Namespace="Omu.Awesome.Mvc.Helpers" %>
<%
    var o = "pf" + (Model.Action + Model.Controller).ToLower();
%>

<script type="text/javascript">
        $(function() {ae_popup('<%=o %>',<%=Model.Width %>, <%=Model.Height %>, '<%=Model.Title %>', 'true', <%=Model.Position %>, 'false', {"<%=Model.CancelText %>": function () { $(this).dialog('close'); },"<%=Model.OkText %>": function () { $("#<%=o %> form").submit(); } });});

        var l<%=o %> = null;
        function call<%=o %>(<%=JsTools.MakeParameters(Model.Parameters) %>) { 
            if(l<%=o %> != null) return;
            l<%=o %> = true;
            $.get('<%=Url.Action(Model.Action, Model.Controller) %>',
            <%=JsTools.JsonParam(Model.Parameters) %>
            update<%=o %>
            );
        }

        function OnSuccess<%=o %>(result) {
            if (result == 'ok' || typeof(result) == 'object') {
                $("#<%=o %>").dialog('close');
                <%if(Model.RefreshOnSuccess){%>
                    location.reload(true);
                <%} %>
                <% if(Model.SuccessFunction != null)
                   {%>
                   <%=Model.SuccessFunction %>(result);
                <%}%>
            }
            else update<%=o %>(result);            
        }        

        function update<%=o %>(data) {
            l<%=o %> = null;
            $("#<%=o %>").html(data);
            $("#<%=o %> form").ajaxForm({            
            <% if(Model.ClientSideValidation){%>
                beforeSubmit: function () { return $("#<%=o %> form").validate().valid(); },
            <%} %>
                success: OnSuccess<%=o %>
            }); 
            $("#<%=o %>").dialog('open');
            $("#<%=o %> form input:visible:first").focus();
        }
</script>
<div id="<%=o %>">
</div>
