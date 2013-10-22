$(document).ready(function() {
	var pathname = window.location.pathname;
	var timestamp = pathname.split("/")[2];	
	var bet = $("#bet");
	$.get('/api/bets/get?timestamp='+timestamp, function(data) {
		if (data.success == true) {
			bet.html(JSON.stringify(data.bet));
			
		}
	}, "json");
});