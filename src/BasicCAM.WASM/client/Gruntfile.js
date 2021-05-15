    
var destRoot = '../wwwroot/';
var siteSources = [

    //ROOT
    {
        other: ['./index.html', './favicon.ico'],
        dest: ['']
    },
    //BASICCAM
    {
        js: ['./js/AceConfig.js',
            './js/sidebar.js',
            './js/ThreeJSConfig.js',
            './js/OrbitControls.js'],
        css: ['./css/site.css'],
        dest: ['']
    },
]
var libSources = [
    //SAMPLES
    {
        other: ['./samples/*'],
        dest: ['samples/'],
        flatten: true
    },
    //ACE
    {
        js: ['./lib/ace/*',],
        dest: ['lib/ace'],
        flatten: true,
    },

    //LISTJS
    {
        js: ['./node_modules/list.js/dist/list.min.js',],
        dest: ['lib/listjs'],
        flatten: true
    },

    //JQUERY
    {
        js: ['./node_modules/jquery/dist/jquery.min.js',],
        dest: ['lib/jquery'],
        flatten: true
    },

    //THREEJS
    {
        js: ['./node_modules/three/build/three.min.js',],
        dest: ['lib/three.js'],
        flatten: true
    }
];

var mapPaths = function (source, index, sources) {

    var sourcePaths = [];

    if (typeof (source) !== 'object')
        return;

    if (Object.prototype.hasOwnProperty.call(source, 'js')) {
        sourcePaths = sourcePaths.concat(source.js);
    }

    if (Object.prototype.hasOwnProperty.call(source, 'css')) {
        sourcePaths = sourcePaths.concat(source.css);
    }

    if (Object.prototype.hasOwnProperty.call(source, 'other')) {
        sourcePaths = sourcePaths.concat(source.other);
    }
    return {
        expand: true,
        flatten: source.flatten,
        src: sourcePaths,
        dest: destRoot + source.dest,
        filter: 'isFile'
    }
};


module.exports = function (grunt) {
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-less');

    grunt.initConfig({

        clean:
        {
            site: {
                options: {
                    'force': true
                },
                src: [destRoot + '/css/*', destRoot + '/js/*'],
            },
            lib: {
                options: {
                    'force': true
                },
                src: [destRoot + '/lib/*'],
            },
            all: {
                options: {
                    'force': true
                },
                src: [destRoot + '/*'],
            }
        },
        copy: {

            site: {
                files: siteSources.map(mapPaths)
            },
            lib: {
                files: libSources.map(mapPaths)
            },
            all: {
                files: libSources.concat(siteSources).map(mapPaths)
            }
        },
        less: {
            site: {
                options: {
                    paths: ['./less']
                },
                files: {
                    './css/site.css': './less/*.less'
                }
            },
        },
        watch: {
            site: {
                files: ['./css/*', './js/*', './less/*'],
                tasks: ['build-site']
            },
            lib: {
                files: ['./lib/*'],
                tasks: ['build-lib']
            },
            all: {
                files: ['./*'],
                tasks: ['build']
            }
        }
    });


    grunt.registerTask('default', ['build-site']);

    grunt.registerTask('build', ['build-site','build-lib']);
    grunt.registerTask('clean-all', ['clean:all']);

    grunt.registerTask('build-site', ['less:site','copy:site', 'watch:site']);
    grunt.registerTask('clean-site', ['clean:site']);


    grunt.registerTask('build-lib', ['copy:lib', 'watch:lib']);
    grunt.registerTask('clean-lib', ['clean:lib']);
};