function applyAwesomeStyles() {
    $(".ae-lookup-openbtn").empty().prepend('<span class="ui-icon ui-icon-newwin"></span>');
    $(".ae-lookup-clearbtn").empty().prepend('<span class="ui-icon ui-icon-gear"></span>');
    mybutton(".ae-lookup-openbtn");
    mybutton(".ae-lookup-clearbtn");
}

$(function () {
    $(".ae-pagination-current").addClass('ui-state-highlight');
    mybutton(".ae-pagination a");
    applyAwesomeStyles();
    $("body").ajaxComplete(applyAwesomeStyles);
});

function mybutton(sel) {
    $(sel).unbind('mousedown mouseup mouseleave')
        .hover(function () { $(this).addClass("ui-state-hover"); },
	            function () { $(this).removeClass("ui-state-hover"); })
        .bind({ 'mousedown mouseup': function () { $(this).toggleClass('ui-state-active'); } })
        .addClass("ui-state-default").addClass("ui-corner-all")
        .bind('mouseleave', function () { $(this).removeClass('ui-state-active') });
}

function ae_fullscreen(o) {
    $(window).bind("resize", function (e) { $(o).dialog("option", { height: $(window).height() - 50, width: $(window).width() - 50 }).trigger('dialogresize'); });
}
function ae_ajaxDropdown(o, p, url) {
    ae_loadAjaxDropdown(o, p, url);
    $("#" + o + "dropdown").change(function () { $('#' + o).val($('#' + o + 'dropdown').val()).trigger('change'); });
    if (p) $('#' + p).change(function () { ae_loadAjaxDropdown(o, p, url, true); });

}

function ae_loadAjaxDropdown(o, p, url, c) {
    if (c) $('#' + o).val(null);
    var data = new Array();
    data.push({ name: "key", value: $('#' + o).val() });
    if (p) data.push({ name: "parent", value: $('#' + p).val() });
    $.post(url, data,
        function (d) {
            $("#" + o + "dropdown").empty();
            $.each(d, function (i, j) {
                var sel = "";
                if (j.Selected == true) sel = "selected = 'selected'";
                $("#" + o + "dropdown").append("<option " + sel + " value=\"" + j.Value + "\">" + j.Text + "</option>");
            });
            if (c) $("#" + o + "dropdown").trigger('change');
        });
    }

function ae_autocomplete(o, k, p, u, mr, delay, minLen) {
    $('#' + o).autocomplete({
        delay: delay,
        minLength: minLen,
        source: function (request, response) {
            var data = new Array();
            data.push({ name: 'searchText', value: request.term });
            data.push({ name: 'maxResults', value: mr });
            if (p) data.push({ name: 'parent', value: $('#' + p).val() });

            $.ajax({
                url: u, type: "POST", dataType: "json",
                data: data,
                success: function (d) { response($.map(d, function (o) { return { label: o.Text, value: o.Text, id: o.Id} })); }
            });
        }
    });

    $('#' + o).bind("autocompleteselect", function (e, ui) {
        $('#' + k).val(ui.item ? ui.item.id : null).trigger('change');
        $('#' + o).trigger('change');
    });

    $('#' + o).keyup(function (e) { if (e.keyCode != '13') $("#" + k).val(null).trigger('change'); });
}

function ae_popup(o, w, h, title, modal, pos, res, btns, fulls) {
    $("#" + o).dialog({
        show: "fade",
        width: fulls ? $(window).width() - 50 : w,
        height: fulls ? $(window).height() - 50 : h,
        title: title,
        modal: modal,
        position: pos,
        resizable: res,
        buttons: btns,
        autoOpen: false,
        close: function (e, ui) { $("#" + o).find('*').remove(); }
    });
    ae_fullscreen("#" + o);
}

function ae_loadLookupDisplay(o, url) {
    $('#ld' + o).val('');
    var id = $('#' + o).val();
    if (id) $.get(url, { id: id }, function (d) { $("#ld" + o).val(d); });
}

function ae_loadMultiLookupDisplay(o, url) {
    var ids = $("#" + o + " input").map(function () { return $(this).attr("value"); }).get();
    $("#ld" + o).html('');
    if (ids.length != 0) $.post(url, $.param({ selected: ids }, true),
        function (d) {
            $.each(d, function () { $("#ld" + o).append('<li>' + this.Text + '</li>') });
        });
}

function ae_lookupChoose(o, url, sel) {
    $('#' + o).val('');
    $('#' + o).val($('#' + o + 'ls .' + sel).attr("data-value"));
    ae_loadLookupDisplay(o, url);
    $("#lp" + o).dialog('close');
}

function ae_multiLookupChoose(o, loadUrl) {
    $("#" + o).empty();
    $.each($("#" + o + "se li").map(function () { return $(this).attr("data-value"); }).get(), function () {
        $("#" + o).append($("<input type='hidden' name='" + o + "' \>").attr("value", this));
    });
    ae_loadMultiLookupDisplay(o, loadUrl);
    $("#lp" + o).dialog('close');
}

function ae_lookupClear(o) {
    $("#lc" + o).click(function () {
        $("#" + o).val("");
        $("#ld" + o).val("");
    });
}

function ae_multiLookupClear(o) {
    $("#lc" + o).click(function () {
        $("#" + o + ",#ld" + o).empty();
    });
}

function ae_confirm(o, f, h, w, yes, no) {
    $("#dialog-confirm-" + o).dialog({
        show: "drop",
        hide: "fade",
        resizable: false,
        height: h,
        width: w,
        modal: true,
        autoOpen: false,
        buttons: {
            yes: function () {
                $(this).dialog('close');
                f.submit();
            },
            no: function () {
                $(this).dialog('close');
            }
        }
    });

    $("." + o).live('click', function () {
        f = $(this).closest('form');
        $("#dialog-confirm-" + o).dialog('open');
        return false;
    });
}

