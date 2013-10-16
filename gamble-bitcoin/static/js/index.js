$(document).ready(
		function() {
			var betting_addresses = $("#betting_addresses")
			$.getJSON("/api/betting_addresses", function(data) {
				betting_addresses.html('');
				for ( var i in data) {
					var ba = jQuery('<div/>', {
						text : data[i].winners + " " + data[i].addr + " ("
								+ data[i].odds * 100
								+ "%) / payout multiplier: x"
								+ data[i].payout.toFixed(5)
					})

					ba.appendTo(betting_addresses);
				}
			});
		});