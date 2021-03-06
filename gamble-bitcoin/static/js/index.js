$(document).ready(function() {
	
	var result_to_str = {
		0: "Pending",
		1: "Win",
		2: "Loss"
	};
	
	var N = 11;
	var shorten = function(s, last) {
		if (s == null) {
			return null;
		}
	    return last?s.substring(s.length-N, s.length):s.substring(0,N);
	}
	
	var betting_addresses = $("#betting_addresses")
	$.getJSON("/api/betting_addresses", function(data) {
		betting_addresses.html('');
		for (var i in data) {
			var ba = jQuery('<div/>', {
				text : data[i].winners + " " + data[i].addr + " ("
						+ data[i].odds * 100
						+ "%) / payout multiplier: x"
						+ data[i].payout.toFixed(5),
			})

			ba.appendTo(betting_addresses);
		}
	});
	
	
	var bets = $("#bets");
	bets.html("Loading...");
	$.getJSON("/api/bets/list", function(data) {
		if (data.success == true) {
			bets.html("");
			var items = [];
			$.each(data.list, function(key, p) {
				var bet_block_part = p.bet_block?"<td><a href='https://blockchain.info/block/"+p.bet_block+"'>"+shorten(p.bet_block, true)+"</a></td> ":"<td> - </td> "
				var pay_tx_part = p.pay_tx?"<td><a href='https://blockchain.info/tx/"+p.pay_tx+"'>"+shorten(p.pay_tx)+"</a></td> ":"<td> - </td> ";
				
				items.push(
					"<tr> " +
					"<td><a href='https://blockchain.info/address/"+p.betting_addr+"'>"+shorten(p.address_winners)+"</a></td> " +
					"<td><a href='https://blockchain.info/tx/"+p.bet_tx+"'>"+shorten(p.bet_tx)+"</a></td> " +
					"<td><a href='/detail/"+p.timestamp+"'>"+p.timestamp_str+"</a></td> " +
					"<td><a href='https://blockchain.info/address/"+p.better+"'>"+shorten(p.better)+"</a></td> " +
					"<td>"+p.amount_btc+"</td> " +
					"<td><a href='/detail/"+p.timestamp+"'>"+result_to_str[p.result]+"</a></td> " +
					bet_block_part +
					pay_tx_part +
					"</tr>"		
				);
			});
			bets.html(items.join(""));
		}
	});
	
	
});