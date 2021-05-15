

function aceInit(id) {
	ace.config.set('basePath', 'js/ace');
	window.aceEditor = ace.edit(id);
	window.aceEditor.setTheme("ace/theme/monokai");
	window.aceEditor.session.setMode("ace/mode/gcode");
}

function aceUpdateText(id, array) {
	console.log(array);

	//Store KEYs in array for lookup with change events
	//Writer VALUEs to editor as array
	const keys = array.map(i => i.Key);
	const lines = array.map(i => i.Value);
	var newSession = ace.createEditSession(lines);

	window.aceEditor.setSession(newSession);
	window.aceEditor.setTheme("ace/theme/monokai");
	window.aceEditor.session.setMode("ace/mode/gcode");

	window.aceEditor.on("input", function () {
		console.log(window.aceEditor[1].selection.cursor.row); //this line always print zero (initial position)
	});

}

window.AceConfig = {
	aceInitialize: (id) => { aceInit(id); },
	aceUpdateText: (id, array) => { aceUpdateText(id, array); },
};
