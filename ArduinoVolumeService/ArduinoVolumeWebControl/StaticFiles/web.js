$(document).ready(function () {
	$('#encoder1').slider({
		formatter: function (value) {
			return 'Current value: ' + value;
		},
		reversed: true
	});
	$("#encoder2").slider({
		reversed: true
	});
	$("#encoder3").slider({
		reversed: true
	});
});