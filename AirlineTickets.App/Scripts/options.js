//DELETE SEARCHES
function OnBeginDeleteSearches() {
    $('#searchesLoaderIcon').show();
    $('#message').empty();
}

function OnSuccessDeleteSearches(data) {
    $('#searchesLoaderIcon').hide();
    $('#message').text(data.Message);
    $('#message').css({ 'color': 'Green' });
}

function OnFailureDeleteSearches(data) {
    $('#searchesLoaderIcon').hide();
    $('#message').text(data.responseJSON.Message);
    $('#message').css({ 'color': 'Crimson' });
}

//DELETE AIRPORTS
function OnBeginDeleteAirports() {
    $('#airportsLoaderIcon').show();
    $('#message').empty();
}

function OnSuccessDeleteAirports(data) {
    $('#airportsLoaderIcon').hide();
    $('#message').text(data.Message);
    $('#message').css({ 'color': 'Green' });
}

function OnFailureDeleteAirports(data) {
    $('#airportsLoaderIcon').hide();
    $('#message').text(data.responseJSON.Message);
    $('#message').css({ 'color': 'Crimson' });
}