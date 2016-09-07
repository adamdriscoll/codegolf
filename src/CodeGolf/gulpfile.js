/// <binding BeforeBuild='copy-assets' />
"use strict";

var _ = require('lodash'),
    gulp = require('gulp');

gulp.task('copy-assets', function () {
    gulp.src(['./node_modules/**/*']).pipe(gulp.dest('./wwwroot/node_modules'));
});