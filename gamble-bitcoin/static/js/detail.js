$(document).ready(function() {
	
	var string = {
		better: "Better Address",
		timestamp_str: "Bet time",
		address_winners: "Bet guess",
		bet_tx: "Bet Transaction",
		result: "Result",
		timestamp: "Timestamp",
		pay_tx: "PayTransaction",
		amount_btc: "Bet amount",
		bet_block: "Bet block",
		betting_addr: "Betting address",
		type: "Bet Type"
	}
	
	var pathname = window.location.pathname;
	var timestamp = pathname.split("/")[2];	
	var bet = $("#bet");
	$.get('/api/bets/get?timestamp='+timestamp, function(data) {
		if (data.success == true) {
			var s = "<div class='row'>";
			for (var property in data.bet) {
			    if (data.bet.hasOwnProperty(property) && string[property]) {
			        s += "<strong><div class='span4'>"+ string[property] + ": </div></strong>" +
			        		"<div class='span4'>" + (data.bet[property]?data.bet[property]:"-") + "</div><br />";
			    }
			}
			s += "</div>"
			bet.html(s);
		}
	}, "json");
});