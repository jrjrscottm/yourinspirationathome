/// <binding BeforeBuild='clean, copy' Clean='clean' />
"use strict";

var fs = require('fs'),
    gulp = require("gulp"),
    path = require("path"),
    merge = require("merge-stream"),
    rename = require("gulp-rename"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");
    

var paths = {
    webroot: "./wwwroot/",
    tenantRoot: "./Tenants/",
    themeRoot: "./Themes/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.img = paths.webroot + "images/*.*";


paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.themeCssDest = paths.webroot + "themes/";
paths.themeJsDest = paths.webroot + "themes/";
paths.themeImagesDest = paths.webroot + "themes/";

function getFolders(dir) {
    return fs.readdirSync(dir)
      .filter(function (file) {
          return fs.statSync(path.join(dir, file)).isDirectory();
      });
}

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:theme:js", function (cb) {
    rimraf(paths.themeJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean:theme:css", function (cb) {
    rimraf(paths.themeCssDest, cb);
});

gulp.task("clean", ["clean:js","clean:theme:js", "clean:css", "clean:theme:css"]);


gulp.task("copy:tenant:js",function() {
    var folders = getFolders(paths.tenantRoot);

    return folders.map(function(folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.tenantRoot, folder, '/**/*.js'))
            .pipe(concat(folder + ".js"))
            .pipe(gulp.dest(paths.themeJsDest + folder + "/scripts/"));
    });
});

gulp.task("copy:theme:js", function () {
    var folders = getFolders(paths.themeRoot);

    return folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.themeRoot, folder, '/**/*.js'))
            .pipe(concat(folder + ".js"))
            .pipe(gulp.dest(paths.themeJsDest + folder + "/scripts/"));
    });
});


gulp.task("copy:tenant:css", function() {
    var folders = getFolders(paths.tenantRoot);

    return folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src([path.join(paths.tenantRoot, folder, '/**/*.css'), '!' + path.join(paths.tenantRoot, folder, '/**/*.min.css')])
            .pipe(concat(folder + ".css"))
            .pipe(gulp.dest(paths.themeCssDest + folder + "/css/"));
    });
});

gulp.task("copy:theme:css", function () {
    var folders = getFolders(paths.themeRoot);

    return folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.themeRoot, folder, '/**/*.css'))
            .pipe(concat(folder + ".css"))
            .pipe(gulp.dest(paths.themeCssDest + folder + "/css/"));
    });
});


gulp.task("copy:tenant:images", function () {
    var folders = getFolders(paths.tenantRoot);

    return folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.tenantRoot, folder, "/**/images/*.*"))
            .pipe(rename({ dirname: '' }))
            .pipe(gulp.dest(paths.themeImagesDest + folder + "/images/"));
    });
});


gulp.task("copy:theme:images", function () {
    var folders = getFolders(paths.themeRoot);

    return folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.themeRoot, folder, "/**/images/*.*"))
            .pipe(rename({ dirname: '' }))
            .pipe(gulp.dest(paths.themeImagesDest + folder + "/images/"));
    });
});




gulp.task("copy", ["copy:theme:js", "copy:theme:css", "copy:theme:images", "copy:tenant:js", "copy:tenant:css", "copy:tenant:images"]);

gulp.task("min:js", function () {
    var folders = getFolders(paths.tenantRoot);

    var tasks = folders.map(function(folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.tenantRoot, folder, '/**/*.js'))
            .pipe(concat(folder + ".js"))
            .pipe(gulp.dest(paths.tenantRoot))
            .pipe(uglify())
            .pipe(rename(folder + ".min.js"))
            .pipe(gulp.dest(paths.themeJsDest + folder + "/scripts/"));
    });

    var root = gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));

    return merge(tasks, root);
});

gulp.task("min:css", function () {
    var folders = getFolders(paths.tenantRoot);

    var tasks = folders.map(function (folder) {
        folder = folder.toLowerCase();

        return gulp.src(path.join(paths.tenantRoot, folder, '/**/*.css'))
            .pipe(concat(folder + ".css"))
            .pipe(cssmin())
            .pipe(rename(folder + ".min.css"))
            .pipe(gulp.dest(paths.themeCssDest + folder + "/css/"));
    });

    var root = gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));

    return merge(tasks, root);

});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("develop", ["clean", "copy"]);

gulp.task("dist", ["clean", "min"]);

gulp.task("default", ["develop"]);