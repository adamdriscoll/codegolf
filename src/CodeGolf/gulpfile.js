/// <binding BeforeBuild='copy-assets' />
"use strict";

var _ = require('lodash'),
    gulp = require('gulp');

gulp.task('copy-assets', function () {
    gulp.src(['./node_modules/monaco-editor/min/vs/**/*']).pipe(gulp.dest('./wwwroot/lib/monaco-editor'));
    gulp.src(['./node_modules/react/dist/react.min.js']).pipe(gulp.dest('./wwwroot/lib/react'));
    gulp.src(['./node_modules/react-dom/dist/react-dom.min.js']).pipe(gulp.dest('./wwwroot/lib/react-dom'));
    gulp.src(['./node_modules/highlight.js/styles/default.css']).pipe(gulp.dest('./wwwroot/lib/highlight.js/styles/'));
    gulp.src(['./node_modules/highlight.js/lib/**/*']).pipe(gulp.dest('./wwwroot/lib/highlight.js/lib/'));
    gulp.src(['./node_modules/moment/min/moment.min.js']).pipe(gulp.dest('./wwwroot/lib/moment/'));
});