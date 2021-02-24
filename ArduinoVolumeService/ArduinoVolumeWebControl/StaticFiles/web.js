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
	hub.client.updateVol = function (encnumbers, muted, volume) {
		for (var i = 0; i < encnumbers.length; i++) {
			var encnumber = encnumbers[i];
			var muteButonName = "#mutebutton".concat(encnumber);
			var encoderNumber = "#encoder".concat(encnumber);

			if (muted == true) {
				$(encoderNumber).addClass("muted-bar");
				$(muteButonName).prop('value', 'Unmute');
			}
			else {
				$(encoderNumber).removeClass("muted-bar");
				$(muteButonName).prop('value', 'Mute');
			}
			enocdesArr[encnumber - 1].slider('setValue', volume);
        }
	};

	hub.client.updateDevices = function (json) {
		for (var i = 1; i <= 3; i++)
		{
			$('#deviceMenu'.concat(i)).empty();
			for (var d = 0; d < json.DeviceItems.length; d++) {
				var entry = json.DeviceItems[d];
				for (var g = 0; g < entry.EncoderNumbers.length; g++) {
					if (entry.EncoderNumbers[g] === i) {
						$('#encoderSelectButton'.concat(i)).html(entry.Name.concat(' <span class="caret"></span>'));
                    }
                }

				$('#deviceMenu'.concat(i)).append('<li><a href="#" onclick="rebindEncoderDevice(' + i + ",'" + entry.DeviceID + "');" + '"\>' + entry.Name + "</a></li>");

				for (var x = 0; x < entry.SoundSessions.length; x++) {
					var ssentry = entry.SoundSessions[x];
					for (var k = 0; k < ssentry.EncoderNumbers.length; k++) {
						if (ssentry.EncoderNumbers[k] === i) {
							$('#encoderSelectButton'.concat(i)).html(ssentry.Name.concat(' <span class="caret"></span>'));
                        }
                    }
					$('#deviceMenu'.concat(i)).append('<li><a href="#" onclick="rebindEncoderSession(' + i + ",'" + entry.DeviceID + "'," + ssentry.SoundSessionProcessID + ");" + '"\>' + entry.Name + " - " + ssentry.Name + "</a></li>");
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

function rebindEncoderDevice(encoderNumber, deviceID) {
	// Declare a proxy to reference the hub.
	var hub = $.connection.webControlHub;
	hub.server.rebindEncoderToDevice(encoderNumber, deviceID);
}

function rebindEncoderSession(encoderNumber, deviceID, sessionID) {
	// Declare a proxy to reference the hub.
	var hub = $.connection.webControlHub;
	hub.server.rebindEncoderToSession(encoderNumber, deviceID, sessionID);
}