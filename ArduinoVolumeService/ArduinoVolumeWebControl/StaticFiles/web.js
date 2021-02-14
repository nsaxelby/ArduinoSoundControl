$(document).ready(function ()
{
	var enocdesArr = [
		$('#encoder1').slider({
			reversed: true
		}),
		$("#encoder2").slider({
			reversed: true
		}),
		$("#encoder3").slider({
			reversed: true
		})
	];


    // Declare a proxy to reference the hub.
    var hub = $.connection.webControlHub;

    // Create a function that the hub can call to broadcast messages.
	hub.client.updateVol = function (encnumber, muted, volume) {
		enocdesArr[encnumber - 1].slider('setValue', volume);
    };

    // Start the connection.
	$.connection.hub.start().done(function () {
		console.log("Connected hub");
	});

	$("#encoder1").on("slide", function (slideEvt) {
		hub.server.changeVol(1, slideEvt.value);
	});

	$("#encoder2").on("slide", function (slideEvt) {
		hub.server.changeVol(2, slideEvt.value);
	});

	$("#encoder3").on("slide", function (slideEvt) {
		hub.server.changeVol(3, slideEvt.value);
	});
});