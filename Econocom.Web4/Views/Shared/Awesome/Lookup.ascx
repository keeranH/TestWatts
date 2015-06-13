<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Omu.Awesome.Mvc.Helpers.LookupInfo>" %>
<%@ Import Namespace="Omu.Awesome.Mvc" %>
<%
    var o = AwesomeTools.MakeId(Model.Prop, Model.Prefix);
    var sel = Settings.Lookup.SelectedRowCssClass;
%>
<script type="text/javascript">
    
    function lgc<%=o %>() {        
        $('li',$(this).parent()).removeClass('<%=sel %>').unbind('click').click(lgc<%=o %>);        
        $(this).addClass('<%=sel %>').click(function(){ae_lookupChoose('<%=o %>', '<%=Url.Action("Get", Model.Controller) %>', '<%=sel %>');});
    }
    
    $(function () {
        var o = '<%=o %>';
        $("#ld"+o).addClass("ae-lookup-textbox");
        ae_loadLookupDisplay(o, '<%=Url.Action("Get", Model.Controller) %>');
        $("."+o+"ie8").remove();
        $("#lp"+o).addClass(o+"ie8");

        ae_popup('lp'+o, <%=Model.Width %>, <%=Model.Height %>, '<%=AwesomeTools.JsEncode(Model.Title) %>', true, 'center', true, {'<%=AwesomeTools.JsEncode(Model.ChooseText) %>': function () {ae_lookupChoose('<%=o %>', '<%=Url.Action("Get", Model.Controller) %>', '<%=sel %>');},'<%=AwesomeTools.JsEncode(Model.CancelText) %>': function () { $(this).dialog('close'); }}, <%=Model.Fullscreen.ToString().ToLower() %>);

        var lck<%=o%> = null;
        $("#lpo"+o).click(function () {           
            if(lck<%=o%> != null) return;
            lck<%=o%> = true;
            $.get('<%=Url.Action("Index", Model.Controller) %>',
                { prop: o <%= Model.Paging ? ", paging: 'true'": "" %>},
                function (d) { $("#lp"+o).html(d).dialog('open'); lck<%=o%> = null; });
        });
        
        <%if(Model.ClearButton){%>
        ae_lookupClear(o);        
        <%} %>
    });  

    
</script>
<div id='lp<%=o%>'>
</div>
<input type="hidden" id="<%=o %>" name="<%=Model.Prop %>" value="<%=Model.Value %>" />
<input type="text" id="ld<%=o%>" disabled="disabled" <%=Model.HtmlAttributes %> />
<a class="ae-lookup-openbtn" id="lpo<%=o%>" href="#"></a>
<%if (Model.ClearButton)
  {%>
<a class="ae-lookup-clearbtn" id="lc<%=o%>" href="#"></a>
<%} %>