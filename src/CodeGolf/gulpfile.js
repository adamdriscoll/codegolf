/// <binding BeforeBuild='copy-assets' />
"use strict";

var _ = require('lodash'),
    gulp = require('gulp');

gulp.task('copy-assets', function () {
    gulp.src(['./node_modules/monaco-editor/min/vs/**/*']).pipe(gulp.dest('./wwwroot/lib/monaco-editor'));
    gulp.src(['./node_modules/react/dist/react-with-addons.min.js']).pipe(gulp.dest('./wwwroot/lib/react'));

});