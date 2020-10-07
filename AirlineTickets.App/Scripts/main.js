$('.date-picker').datepicker({
    dateFormat: 'dd.mm.yy',
    language: 'hr',
});

window.onpopstate = function (event) {
    var currentState = history.state;
    document.body.innerHTML = currentState;
    location.reload(true);
};

//SEARCH OFFERS
function OnSearchBegin() {
    $('.searchInputs').prop('disabled', true);
    $('#listFlights').addClass('blur');
    $('#flightsLoaderIcon').show();
    $('#flightErrorMessage').empty();
}

function OnSearchSuccess() {
    url = "?";
    $.each(this, function (key, value) {
            if (value.value !== "" && value.type !== "submit" && value.value !== "0") {
                if (value.type === "checkbox") {
                    if (value.checked) {
                        url += value.id + "=" + value.value + "&";
                    }
                }
                else
                    url += value.id + "=" + value.value + "&";
            }
    });
    url = url.slice(0, url.length - 1);
    if (url === '') {
        url = "?";
    }
    history.pushState(document.body.innerHTML, "SearchParam", url);

    $('.searchInputs').prop('disabled', false);
    $('#listFlights').removeClass('blur');
    $('#flightsLoaderIcon').hide();
    
}

function OnSearchFail(data) {
    $('.searchInputs').prop('disabled', false);
    $('#flightsLoaderIcon').hide();

    $('#flightErrorMessage').text(data.responseJSON.Message);
    $('#flightErrorMessage').css({ 'color': 'Crimson' });
}
//PAGINATION
function OnSortSuccess() {
    $('.searchInputs').prop('disabled', false);
    $('#listFlights').removeClass('blur');
    $('#flightsLoaderIcon').hide();
    history.pushState(document.body.innerHTML, "sort", this);
}


const toggleRow = (element) => {
    element.getElementsByClassName('expanded-row-content')[0].classList.toggle('hide-row');
    console.log(event);
}

$(document).ready(function () {

    $.ajax({
        url: "/Home/ScapreAirports",
        type: "POST",
        dataType: "json",
        beforeSend: function () {
            $('#flightsLoaderIcon').show();
            $('#flightErrorMessage').text("Dodavanje aeordroma u bazu podataka, molim pričekajte.");
            $('#flightErrorMessage').css({ 'color': 'Blue' });
            $('.searchInputs').prop('disabled', true);
        },
        success: function (data) {
            $('#flightsLoaderIcon').hide();
            $('.searchInputs').prop('disabled', false);
            $('#flightErrorMessage').text(data.Message);
            $('#flightErrorMessage').css({ 'color': 'Green' });
        },
        error: function (data) {
            $('#searchesLoaderIcon').hide();
            $('#flightErrorMessage').text(data.responseJSON.Message);
            $('#flightErrorMessage').css({ 'color': 'Crimson' });
        }
    });

    $("#originalLocation").autocomplete({
        source: function(request, response) {
            $.ajax({
                url: "/Home/SearchAirports",
                type: "POST",
                dataType: "json",
                data: {
                    search: request.term,
                    otherSearch: $("#destinationLocation").val() ? $("#destinationLocation").val() : ""
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Location, value: item.IATA };
                    }))

                }
            })
        },
        focus: function (event, ui) {
            $("#originalLocation").val(ui.item.label + " (" + ui.item.value + ")");
            return false;
        },
        select: function (event, ui) {
            $("#originalLocation").val(ui.item.label + " (" + ui.item.value + ")");
            $('#originalLocationCode').val(ui.item.value);

            return false;
        },
        change: function (event, ui) {
            if (ui.item == null || ui.item == undefined) {
                $("#originalLocation").val("");
            } 
        },
        open: function (e, ui) {
            var
                acData = $(this).data('ui-autocomplete');

            acData
                .menu
                .element
                .find('div')
                .each(function () {
                    var me = $(this);
                    var keywords = acData.term.split(' ').join('|');
                    me.html(me.text().replace(new RegExp("(" + keywords + ")", "gi"), '<b>$1</b>'));
                });
        },
    })
    .data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<div>" + item.label + " (" + item.value + ")</div>")
            .appendTo(ul);
        };

    $("#destinationLocation").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/SearchAirports",
                type: "POST",
                dataType: "json",
                data: {
                    search: request.term,
                    otherSearch: $("#originalLocation").val() ? $("#originalLocation").val() : ""
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Location, value: item.IATA };
                    }))

                }
            })
        },
        focus: function (event, ui) {
            $("#destinationLocation").val(ui.item.label + " (" + ui.item.value + ")");
            return false;
        },
        select: function (event, ui) {
            $("#destinationLocation").val(ui.item.label + " (" + ui.item.value + ")");
            $('#destinationLocationCode').val(ui.item.value);
            return false;
        },
        change: function (event, ui) {
            if (ui.item == null || ui.item == undefined) {
                $("#destinationLocation").val("");
            }
        },
        open: function (e, ui) {
            var
                acData = $(this).data('ui-autocomplete');

            acData
                .menu
                .element
                .find('div')
                .each(function () {
                    var me = $(this);
                    var keywords = acData.term.split(' ').join('|');
                    me.html(me.text().replace(new RegExp("(" + keywords + ")", "gi"), '<b>$1</b>'));
                });
        },
    })
        .data("ui-autocomplete")._renderItem = function (ul, item) {
            return $("<li>")
                .append("<div>" + item.label + " (" + item.value + ")</div>")
                .appendTo(ul);
        };
})  