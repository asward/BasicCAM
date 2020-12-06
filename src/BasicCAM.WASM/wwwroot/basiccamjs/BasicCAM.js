import * as THREE from '../lib/three.js/three.module.js';
import { OrbitControls } from '../basiccamjs/OrbitControls.js';

import { TrackballControls } from '../basiccamjs/TrackballControls.js';
//import { CameraControls } from '../basiccamjs/CameraControls.js';



var camera, controls, scene, renderer;


function initialize() {
	_scene();

	_controls();

	_drawGrid();

	_lighting();

	window.addEventListener('resize', onWindowResize, false);
	
	//render(); // remove when using next line for animation loop (requestAnimationFrame)
	animate();
}

function _drawGrid(){


	var lineMaterial = new THREE.LineBasicMaterial({ color: 0x00ff00 });


	//X
	var nx = 20;
	var ny = 20;
	for (var i = -nx; i <= nx; i++) {
		var pointsX = [];
		pointsX.push(new THREE.Vector3(i , -ny, 0));
		pointsX.push(new THREE.Vector3(i, ny, 0));

		var lineGeometryX = new THREE.BufferGeometry().setFromPoints(pointsX);

		var lineX = new THREE.Line(lineGeometryX, lineMaterial);

		scene.add(lineX);
	}

	var xlineMaterial = new THREE.LineBasicMaterial({ color: 0x000ff });
	var xAxis = [];
	xAxis.push(new THREE.Vector3(-nx, 0, 0));
	xAxis.push(new THREE.Vector3(nx, 0, 0));

	var xAxisGeometery = new THREE.BufferGeometry().setFromPoints(xAxis);

	var xAxisLine = new THREE.Line(xAxisGeometery, xlineMaterial);

	scene.add(xAxisLine);


	var ylineMaterial = new THREE.LineBasicMaterial({ color: 0xff0000 });
	var yAxis = [];
	yAxis.push(new THREE.Vector3(0, -ny, 0));
	yAxis.push(new THREE.Vector3(0, ny, 0));

	var yAxisGeometery = new THREE.BufferGeometry().setFromPoints(yAxis);

	var yAxisLine = new THREE.Line(yAxisGeometery, ylineMaterial);

	scene.add(yAxisLine);

	//Y
	for (var i = -ny; i <= ny ; i++) {
		var pointsY = [];
		pointsY.push(new THREE.Vector3(-nx, i , 0));
		pointsY.push(new THREE.Vector3(nx, i , 0));

		var lineGeometryY = new THREE.BufferGeometry().setFromPoints(pointsY);

		var lineY = new THREE.Line(lineGeometryY, lineMaterial);

		scene.add(lineY);
	}
}
function _lighting() {
	// lights

	var light = new THREE.DirectionalLight(0xffffff);
	light.position.set(1, 1, 1);
	scene.add(light);

	var light = new THREE.DirectionalLight(0x002288);
	light.position.set(- 1, - 1, - 1);
	scene.add(light);

	var light = new THREE.AmbientLight(0x222222);
	scene.add(light);
}
function _controls() {
	//renderer.setPixelRatio(window.devicePixelRatio);
	//renderer.setSize(window.innerWidth, window.innerHeight-54);
	//document.body.appendChild(renderer.domElement);

	camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 1, 1000);

	camera.position.set(-10, -10, 10);
	camera.up.set(0,0,1);

	// controls

	//controls = new TrackballControls(camera, renderer.domElement);
	controls = new OrbitControls(camera, renderer.domElement);
	//controls = new CameraControls(camera, renderer.domElement);


	//controls.addEventListener( 'change', render ); // call this only in static scenes (i.e., if there is no animation loop)

	controls.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
	controls.dampingFactor = 0.05;

	controls.screenSpacePanning = false;

	controls.minDistance = 1;
	controls.maxDistance = 500;


	//controls.maxPolarAngle = Math.PI / 2;
}
function _scene() {
	scene = new THREE.Scene();
	scene.background = new THREE.Color(0xcccccc);
	scene.fog = new THREE.FogExp2(0xcccccc, 0.002);

	renderer = new THREE.WebGLRenderer({ antialias: true });

	var container = document.getElementById('renderdiv');
	renderer.setSize($(container).width(), $(container).height());
	container.appendChild(renderer.domElement);
}

function onWindowResize() {

	var container = document.getElementById('renderdiv');
	var cw = $(container).width();
	var ch = $(container).height();

	renderer.setSize(cw, ch);

	camera.aspect = cw / ch;
	camera.updateProjectionMatrix();

	//camera.aspect = window.cw / window.ch;
	//renderer.setSize(window.innerWidth, window.innerHeight);

}

function animate() {

	controls.update(); // only required if controls.enableDamping = true, or if controls.autoRotate = true

	renderer.render(scene, camera);

	requestAnimationFrame(animate);
}

function aceInit(id) {
	ace.config.set('basePath', 'js/ace');
	window.aceEditor = ace.edit(id);
	window.aceEditor.setTheme("ace/theme/monokai");
	window.aceEditor.session.setMode("ace/mode/gcode");
}

function aceUpdateText(id, array) {
	var newSession = ace.createEditSession(array);

	window.aceEditor.setSession(newSession);
	window.aceEditor.setTheme("ace/theme/monokai");
	window.aceEditor.session.setMode("ace/mode/gcode");
}


function plot(segments, appendExisting = false) {
	var material = new THREE.LineBasicMaterial({ color: 0x0000ff });
	var points = [];
	var segmentsNew = JSON.parse(segments);
	segments = segmentsNew;
	//points.push(new THREE.Vector3(segments[0].Start.X, segments[0].Start.Y, segments[0].Start.Z));

	for (const segment of segments) {
		var points = [];

		if (segment.Type == "Arc") {
			//const curve = new THREE.EllipseCurve(
			//	segment.Center.X, segment.Center.Y,            // ax, aY
			//	segment.Radius, segment.Radius,           // xRadius, yRadius
			//	segment.StartAngle, segment.EndAngle,  // aStartAngle, aEndAngle
			//	segment.CW,            // aClockwise
			//	0                 // aRotation
			//);
			//var curvePoints = curve.getPoints(50);
			//for (const curvePoint of curvePoints) {
			//	points.push(new THREE.Vector3(curvePoint.x, curvePoint.y, segment.End.Z));
			//}
			console.log(segment);
			var sa = 0;
			var ea = 0;
			if (segment.CW) {
				sa = segment.StartAngle + Math.PI / 2;
				ea = segment.EndAngle + Math.PI / 2;
			} else {
				sa = segment.StartAngle - Math.PI / 2;
				ea = segment.EndAngle - Math.PI / 2;
            }
			//var sa = ((segment.StartAngle < 0 ? segment.StartAngle + Math.PI * 2 : segment.StartAngle) + Math.PI * 2) % (2 * Math.PI);
			//var ea = ((segment.EndAngle < 0 ? segment.EndAngle + Math.PI * 2 : segment.EndAngle) + Math.PI * 2) % (2 * Math.PI);
			//console.log(sa);
			//console.log(ea);
				
			var curve = new THREE.EllipseCurve(
				segment.Center.X, segment.Center.Y,            // ax, aY
				//0,0,            // ax, aY
				segment.Radius, segment.Radius,           // xRadius, yRadius
				//segment.StartAngle, segment.EndAngle,  // aStartAngle, aEndAngle
				sa, ea,
				segment.CW,            // aClockwise
			);



			var arcPoints = curve.getSpacedPoints(20);

			var path = new THREE.Path();
			var geometry = path.createGeometry(arcPoints);

			//var material = new THREE.LineBasicMaterial({ color: 0xff0000 });

			var line = new THREE.Line(geometry, material);

			scene.add(line);

			renderer.render(scene, camera);



			////var points = curve.getSpacedPoints(20);

			//points.push(curve.getSpacedPoints(20));

			////var path = new THREE.Path();
			//var arcGeometry = new THREE.BufferGeometry().setFromPoints(points);

			//var arcLines = new THREE.Line(arcGeometry, material);

			//scene.add(arcLines);





		} else {
			points.push(new THREE.Vector3(segment.Start.X, segment.Start.Y, segment.Start.Z));

			points.push(new THREE.Vector3(segment.End.X, segment.End.Y, segment.End.Z));

			var lineGeometry = new THREE.BufferGeometry().setFromPoints(points);

			var line = new THREE.Line(lineGeometry, material);

			scene.add(line);
        }
	}
	renderer.render(scene, camera);

	//var curve = new THREE.EllipseCurve(
	//	5, 5,             // ax, aY
	//	1, 1,            // xRadius, yRadius
	//	0, .1, // aStartAngle, aEndAngle
	//	false             // aClockwise
	//);

	//var points = curve.getSpacedPoints(20);

	//var path = new THREE.Path();
	//var geometry = path.createGeometry(points);

	//var material = new THREE.LineBasicMaterial({ color: 0xff0000 });

	//var line = new THREE.Line(geometry, material);

	//scene.add(line);

	//renderer.render(scene, camera);

}

function split() {
	//Split(['#segment-tree', '#segment-properties'], {
	//	sizes: [25, 75],
	//	direction: 'vertical',
	//	cursor: 'row-resize',
	//});

	var mySplit = Split(['#segment-tree', '#segment-properties'], {
		sizes: [50, 50],
		direction: 'vertical',
		cursor: 'row-resize',
	});
	console.log(mySplit);
}

window.BasicCAMJS = {

    //load: () => { loadSolutionView(); }
	load: () => { loadSolutionView(); },
	initialize: () => { initialize(); },

	//initializeSidebar: () => { initializeSidebar(); },

	plot: (segments, appendExisting) => { plot(segments, appendExisting); },

	aceInitialize: (id) => { aceInit(id); },
	aceUpdateText: (id, array) => { aceUpdateText(id, array); },

	split: () => { split(); },
};
