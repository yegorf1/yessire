var socket = io.connect('http://' + document.domain + ':' + location.port);
var userID = getParameterByName('username');
var receiveSendTimeout = 100;

function getParameterByName(name) {
	// get params from URL/Address-line
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regexS = "[\\?&]" + name + "=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.search);
	if (results == null)
		return "";
	else
		return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function startGame() {
	socket.emit('startGame');
}

function sendMessage() {
	message = document.getElementById('messageEdit').value;
	socket.emit('sendMessage', {
		'msg': message
	});
	addNewLineToTextarea('Вы: ' + message);

	document.getElementById('messageEdit').value = '';
}

function getMessages() {
	socket.emit('getMessages');
}

function getMessageHistory() {
	socket.emit('getMessageHistory');
}

function getMessagesWithTimeout() {
	setTimeout(getMessages, receiveSendTimeout);
}

function addNewLineToTextarea(text) {
	textarea = document.getElementById('history');
	textarea.value = text + '\n' + textarea.value;
}

socket.on('connect', function () {
	console.log('Connected to SocketIO server');
	getMessageHistory();
});

socket.on('startGame', function () {
	document.getElementById('history').value = '';
	getMessagesWithTimeout();
});

socket.on('sendMessage', function () {
	getMessagesWithTimeout();
});

socket.on('getMessages', function (data) {
	console.log(data);
	messages = JSON.parse(data);
	if (!messages) return;

	for (var i = 0; i < messages.length; i++) {
		var from = messages[i].Message.From;
		if (from == '{{current_user.username}}')
			from = 'Вы';

		addNewLineToTextarea(from + ': ' + messages[i].Message.Text);
	}
});

document.getElementById('messageEdit').onkeydown = function (ev) {
	// 13 -- enter
	if (ev.keyCode == 13) {
		sendMessage();
	}
}