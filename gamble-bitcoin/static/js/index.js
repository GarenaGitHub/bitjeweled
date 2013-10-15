$(document).ready(function() {
	$.get("/api/betting_addresses", function(data) {
		alert("Load was performed.");
		alert(data);
	});
});