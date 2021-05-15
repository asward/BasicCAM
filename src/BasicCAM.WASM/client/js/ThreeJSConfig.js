import * as THREE from '../lib/three.js/three.module.js';
import { OrbitControls } from './OrbitControls.js';

//import { TrackballControls } from '../basiccamjs/TrackballControls.js';
//import { CameraControls } from '../basiccamjs/CameraControls.js';

var camera, controls, scene, renderer, CAMObjectUUIDS, CADObjectUUIDS, baseCadMat, toolOffMat, toolOnMat, tanFOV, windowHeight;
var majorGridMinMax = 10;
var minorGridDivisons = 5;
var frustumSize = 5; 

function initialize() {
	console.log("Initializing three.js");

	CAMObjectUUIDS = [];
	CADObjectUUIDS = [];
	_scene();
	_camera();
	_controls();

	_drawGrid();
	_drawAxes();

	_lighting();

	baseCadMat = new THREE.LineBasicMaterial({ color: 0x0000ff });
	toolOffMat = new THREE.LineBasicMaterial({ color: 0x00ff00 });
	toolOnMat = new THREE.LineBasicMaterial({ color: 0xff0000 });

	var container = document.getElementById('renderdiv');


	window.addEventListener('resize', onResize, false);
	
	//render(); // remove when using next line for animation loop (requestAnimationFrame)

	onResize();
	animate();
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

function _camera() {
	//camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 1, 1000);
	camera = new THREE.OrthographicCamera(-10, 10,
		10, -10,
		0.1, 2000);

	camera.zoom = 1;
	camera.aspect = 1;
	camera.position.set(10, 10, 10);
	camera.up.set(0, 0, 1);
	camera.fov = 60;

	var { cw, ch } = widthAndHeight();
	// remember these initial values
	tanFOV = Math.tan(((Math.PI / 180) * camera.fov / 2));
	windowHeight = ch;
}
function widthAndHeight() {
	var container = document.getElementById('renderdiv');
	var cw = $(container).width();
	var ch = $(container).height();
	return { cw, ch };
}
function _controls() {
	//document.body.appendChild(renderer.domElement);

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

	controls.maxAzimuthAngle = Math.PI / 2;

	//controls.maxPolarAngle = Math.PI / 2;
}

function _drawAxes() {
	var origin = new THREE.Vector3(0, 0, 0);

	var xAxisMaterial = new THREE.LineBasicMaterial({ color: 0xff0000 });
	var xAxisPoints = [];
	xAxisPoints.push(origin);
	xAxisPoints.push(new THREE.Vector3(majorGridMinMax, 0, 0));
	var xAxisGeometery = new THREE.BufferGeometry().setFromPoints(xAxisPoints);
	var xAxis = new THREE.Line(xAxisGeometery, xAxisMaterial);
	scene.add(xAxis);

	var yAxisMaterial = new THREE.LineBasicMaterial({ color: 0x00ff00 });
	var yAxisPoints = [];
	yAxisPoints.push(origin);
	yAxisPoints.push(new THREE.Vector3(0, majorGridMinMax, 0));
	var yAxisGeometery = new THREE.BufferGeometry().setFromPoints(yAxisPoints);

	var yAxisLine = new THREE.Line(yAxisGeometery, yAxisMaterial);

	scene.add(yAxisLine);


	var zAxisMaterial = new THREE.LineBasicMaterial({ color: 0x0000ff });
	var zAxisPoints = [];
	zAxisPoints.push(origin);
	zAxisPoints.push(new THREE.Vector3(0, 0, majorGridMinMax));

	var zAxisGeometery = new THREE.BufferGeometry().setFromPoints(zAxisPoints);

	var zAxisLine = new THREE.Line(zAxisGeometery, zAxisMaterial);

	scene.add(zAxisLine);
}

function _drawGrid() {
	var majorGridMaterial = new THREE.LineBasicMaterial({ color: 0x808080 });
	var minorGridMaterial = new THREE.LineBasicMaterial({ color: 0xbbbbbb });

	//X
	for (var majorIndex = -majorGridMinMax; majorIndex <= majorGridMinMax; majorIndex++) {
		var pointsX = [];
		pointsX.push(new THREE.Vector3(majorIndex, -majorGridMinMax, 0));
		pointsX.push(new THREE.Vector3(majorIndex, majorGridMinMax, 0));
		var majorGridXGeometery = new THREE.BufferGeometry().setFromPoints(pointsX);
		var majorGridX = new THREE.Line(majorGridXGeometery, majorGridMaterial);

		scene.add(majorGridX);
		if (majorIndex > majorGridMinMax - 1)
			continue;

		for (var minorIndexX = 1; minorIndexX < minorGridDivisons; minorIndexX++)
		{
			var minorPointsX = [];
			 
			minorPointsX.push(new THREE.Vector3(majorIndex + minorIndexX / (minorGridDivisons), -majorGridMinMax, 0));
			minorPointsX.push(new THREE.Vector3(majorIndex + minorIndexX / (minorGridDivisons), majorGridMinMax, 0));
			var minorGridXGeometery = new THREE.BufferGeometry().setFromPoints(minorPointsX);
			var minorGridX = new THREE.Line(minorGridXGeometery, minorGridMaterial);
			scene.add(minorGridX);
		} 
	}

	//Y

	for (var majorIndex = -majorGridMinMax; majorIndex <= majorGridMinMax; majorIndex++) {
		var pointsY = [];
		pointsY.push(new THREE.Vector3(-majorGridMinMax, majorIndex, 0));
		pointsY.push(new THREE.Vector3(majorGridMinMax, majorIndex, 0));
		var majorGridYGeometery = new THREE.BufferGeometry().setFromPoints(pointsY);
		var majorGridY = new THREE.Line(majorGridYGeometery, majorGridMaterial);

		scene.add(majorGridY);
		if (majorIndex > majorGridMinMax - 1)
			continue;

		for (var minorIndexY = 1; minorIndexY < minorGridDivisons; minorIndexY++) {
			var minorPointsY = [];

			minorPointsY.push(new THREE.Vector3(-majorGridMinMax, majorIndex + minorIndexY / (minorGridDivisons), 0));
			minorPointsY.push(new THREE.Vector3(majorGridMinMax, majorIndex + minorIndexY / (minorGridDivisons),  0));
			var minorGridYGeometery = new THREE.BufferGeometry().setFromPoints(minorPointsY);
			var minorGridY = new THREE.Line(minorGridYGeometery, minorGridMaterial);
			scene.add(minorGridY);
		}
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

function onResize() {
	var { cw, ch } = widthAndHeight();

	var aspect = cw / ch;

	camera.left = frustumSize * aspect / - 2;
	camera.right = frustumSize * aspect / 2;
	camera.top = frustumSize / 2;
	camera.bottom = - frustumSize / 2;

	camera.updateProjectionMatrix();
	renderer.setSize(cw, ch);

		
	//var container = document.getElementById('renderdiv');
	//var cw = $(container).width();
	//var ch = $(container).height();


	//camera.aspect = ch/cw;
	//camera.updateProjectionMatrix();

	//renderer.setSize(cw, ch);





	//renderer.setSize(cw, ch);

	//camera.aspect = cw / ch;
	//camera.updateProjectionMatrix();

	////camera.aspect = window.cw / window.ch;
	////renderer.setSize(window.innerWidth, window.innerHeight);

	////renderer.setPixelRatio(window.devicePixelRatio);
	//renderer.setSize(cw, ch);
}

function animate() {

	controls.update(); // only required if controls.enableDamping = true, or if controls.autoRotate = true

	renderer.render(scene, camera);

	requestAnimationFrame(animate);
}

function clearSolution() {
	clearPlotByUUIDs(CAMObjectUUIDS);
}

function clearPlotByUUIDs(uuids) {
	for (const uuid of uuids) {
		const object = scene.getObjectByProperty('uuid', uuid);

		if (typeof object === "undefined")
			continue;

		object.geometry.dispose();
		//object.material.dispose();
		scene.remove(object);
	}
	uuids.length = 0;

}
function plotArcSegment(segment, material) {
	var sa = 0;
	var ea = 0;
	if (segment.CW) {
		sa = segment.StartAngle + Math.PI / 2;
		ea = segment.EndAngle + Math.PI / 2;
	} else {
		sa = segment.StartAngle - Math.PI / 2;
		ea = segment.EndAngle - Math.PI / 2;
	}

	var curve = new THREE.EllipseCurve(
		segment.Center.X, segment.Center.Y,            // ax, aY
		segment.Radius, segment.Radius,           // xRadius, yRadius
		sa, ea,
		segment.CW,            // aClockwise
	);

	var arcPoints = curve.getSpacedPoints(20);
	var path = new THREE.Path();
	var geometry = path.createGeometry(arcPoints);

	//geometry.Translate(0, 0, segment.Center.Z);

	var arcLine = new THREE.Line(geometry, material);
	arcLine.position.set(0, 0, segment.Center.Z);

	scene.add(arcLine);

	return arcLine.uuid;
 }
function plotCircleSegment(segment, material) {
	var circleCurve = new THREE.EllipseCurve(
		segment.Center.X, segment.Center.Y,            // ax, aY
		segment.Radius, segment.Radius,           // xRadius, yRadius
		0, 0, //Arc start/stop at same point
		segment.CW,            // aClockwise
	);

	var circlePoints = circleCurve.getSpacedPoints(64);
	var circlePath = new THREE.Path();
	var circleGeometry = circlePath.createGeometry(circlePoints);

	var circleLines = new THREE.Line(circleGeometry, material);

	circleLines.position.set(0, 0, segment.Center.Z);

	scene.add(circleLines);

	return circleLines.uuid;
}
function plotLineSegment(segment, material) {
	var points = [];

	points.push(new THREE.Vector3(segment.Start.X, segment.Start.Y, segment.Start.Z));

	points.push(new THREE.Vector3(segment.End.X, segment.End.Y, segment.End.Z));

	var lineGeometry = new THREE.BufferGeometry().setFromPoints(points);

	var line = new THREE.Line(lineGeometry, material);

	scene.add(line);

	return line.uuid;
}

function plotCAM(segments, appendExisting = false) {
	
	if (!appendExisting) {
		clearPlotByUUIDs(CAMObjectUUIDS);
	}
	var jsonSegments = JSON.parse(segments);

	var material;

	for (const segment of jsonSegments) {
		var geometry = segment.Segment;
		var settings = segment.Settings;

		if (settings.ToolOn) {
			material = toolOnMat;
		} else {
			material = toolOffMat;
		}

		var uuid = plotGeometry(geometry, material)

		CAMObjectUUIDS.push(uuid );
	}
}
function plotCAD(segments, appendExisting = false) {
	if (!appendExisting) {
		clearPlotByUUIDs(CADObjectUUIDS);
	}
	var material = baseCadMat;

	var jsonSegments= JSON.parse(segments);

	for (const geometry of jsonSegments) {
		CADObjectUUIDS.push(plotGeometry(geometry, material));
	}
}

function plotGeometry(geometry, material) {
	if (geometry.Type === "Arc") {
		return plotArcSegment(geometry, material);
	} else if (geometry.Type === "Circle") {
		return plotCircleSegment(geometry, material);
	} else if (geometry.Type === "Line") {
		return plotLineSegment(geometry, material);
	}
	return null;
}


window.ThreeJSConfig = {
	load: () => { loadSolutionView(); },
	initialize: () => { initialize(); }, 
	
	plotCAD: (segments, appendExisting) => { plotCAD(segments, appendExisting); },
	plotCAM: (segments, appendExisting) => { plotCAM(segments, appendExisting); },
	clearSolution: () => { clearSolution(); },
};
