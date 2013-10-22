$(document).ready(function() {
	var pathname = window.location.pathname;
	var timestamp = pathname.split("/")[2];	
	var bet = $("#bet");
	$.get('/api/bets/get?timestamp='+timestamp, function(data) {
		if (data.success == true) {
			var s = "";
			for (var property in data.bet) {
			    if (data.bet.hasOwnProperty(property)) {
			        s += property + ": " + data.bet[property] + "<br />";
			    }
			}
			bet.html(s);
		}
	}, "json");
});