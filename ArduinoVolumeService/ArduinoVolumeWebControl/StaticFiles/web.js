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
		var muteButonName = "#mutebutton".concat(encnumber);
		var encoderNumber = "#encoder".concat(encnumber);

		if (muted == true) {
			$(encoderNumber).addClass("muted-bar");
			$(muteButonName).prop('value', 'Unmute');
		}
		else
		{
			$(encoderNumber).removeClass("muted-bar");
			$(muteButonName).prop('value', 'Mute');
		}
		enocdesArr[encnumber - 1].slider('setValue', volume);
	};

	hub.client.updateDevices = function (json) {
		for (var i = 0; i < json.DeviceItems.length; i++) {
			var entry = json.DeviceItems[i];
			if (entry.EncoderNumber !== null) {
				$('#encoderTitle'.concat(entry.EncoderNumber)).html(entry.Name);
			}
			for (var x = 0; x < entry.SoundSessions.length; x++) {
				var ssentry = entry.SoundSessions[x];
				if (ssentry.EncoderNumber !== null) {
					$('#encoderTitle'.concat(ssentry.EncoderNumber)).html(ssentry.Name);
				}
			}
		}
    }

    // Start the connection.
	$.connection.hub.start().done(function () {
		hub.server.requestBoundDevices();
		console.log("Connected hub");
	});

	$("#encoder1").on("slideStop", function (slideEvt) {
		hub.server.changeVol(1, slideEvt.value);
	});

	$("#encoder2").on("slideStop", function (slideEvt) {
		hub.server.changeVol(2, slideEvt.value);
	});

	$("#encoder3").on("slideStop", function (slideEvt) {
		hub.server.changeVol(3, slideEvt.value);
	});

	$("#mutebutton1").on("click", function () {
		if ($("#mutebutton1").attr("value") == "Mute") {
			hub.server.muteEncoder(1, true);
		}
		else {
			hub.server.muteEncoder(1, false);
		}
	});

	$("#mutebutton2").on("click", function () {
		if ($("#mutebutton2").attr("value") == "Mute") {
			hub.server.muteEncoder(2, true);
		}
		else {
			hub.server.muteEncoder(2, false);
		}
	});

	$("#mutebutton3").on("click", function () {
		if ($("#mutebutton3").attr("value") == "Mute") {
			hub.server.muteEncoder(3, true);
		}
		else {
			hub.server.muteEncoder(3, false);
		}
	});
});