<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    var o = ViewData["prop"].ToString();
    var pageable = (bool)ViewData["paging"];
%>
<form id="lsf<%=o %>" action="<%=Url.Action("search") %>" method="post">
<% Html.RenderAction("SearchForm"); %>
</form>
<div id='<%=o %>lsh'>
    <% Html.RenderAction("header"); %>
</div>
<ul id="<%=o%>ls" class="ae-lookup-list ae-lookup-searchlist">
</ul>
<script type="text/javascript">
function lay<%=o %>() {
    var av = $("#lp<%=o %>").height() - $('#lsf<%=o %>').height() - $('#<%=o %>lsh').height();
    $('#<%=o %>ls').css('height', av+'px');     
} 
$("#lp<%=o %>").bind( "dialogresize", lay<%=o %>);
$('#lsf<%=o %> input').keypress(function(e){ if(e.which == 13){ e.preventDefault(); $('#lsf<%=o %>').submit(); } });

$('#lsf<%=o %>').submit(function(e){
    e.preventDefault();    
    var lfm = $('#lsf<%=o %>').serializeArray();

    <% if(pageable){%>
        lfm.push({ name: "page", value: "1" });

        $.post('<%=Url.Action("search") %>', lfm, 
        function(d){
        $('#<%=o%>ls').html(d.rows);        
        $("#<%=o%>ls li").click(lgc<%=o %>);
        lay<%=o %>();        
        if (d.more) {
        lfm.pop();
        var page =1;

        $('<a class="ae-lookup-morebtn">more</a>').click(function() {
        page++;
        lfm.push({ name: "page", value: page });

            $.post('<%=Url.Action("search") %>', lfm, function (d) {
                
                $("#<%=o%>ls .ae-lookup-morebtn").before($(d.rows).css('opacity', 0).animate({ opacity: 1 }, 300, 'easeInCubic'));                
                $("#<%=o%>ls li").click(lgc<%=o %>);

                if (!d.more) $('#<%=o%>ls .ae-lookup-morebtn').fadeOut('slow');      
                lay<%=o %>();
            });
        lfm.pop();
        }).appendTo('#<%=o%>ls');
        }

        });

    <%} else
       {%>   
           $.post('<%=Url.Action("search") %>', lfm, function(d){
           $('#<%=o%>ls').html(d);
           $("#<%=o%>ls li").click(lgc<%=o %>); 
           lay<%=o %>(); });           
       <%
       }%>
});

$('#lsf<%=o %>').submit();
$('#lsf<%=o %> input:first').focus();
</script>
