//// this datepicker also work chcnge event in angular js

//ngApp.directive('datepicker', function ($filter) {
//    return {
//        restrict: 'A',
//        require: 'ngModel',
//        compile: function () {
//            return {
//                pre: function (scope, element, attrs, ngModelCtrl) {
//                    var format, dateObj;
//                    format = (!attrs.dpFormat) ? 'dd-M-yyyy' : attrs.dpFormat;
//                    if (!attrs.initDate && !attrs.dpFormat) {
//                        // If there is no initDate attribute than we will get todays date as the default
//                        dateObj = new Date();

//                        //scope[attrs.ngModel] = dateObj.getDate() + '/' + (dateObj.getMonth() + 1) + '/' + dateObj.getFullYear();
//                        scope[attrs.ngModel] = $filter("date")(dateObj, 'dd-MMM-yyyy')

//                    } else if (!attrs.initDate) {
//                        // Otherwise set as the init date
//                        scope[attrs.ngModel] = attrs.initDate;
//                    } else {
//                        // I could put some complex logic that changes the order of the date string I
//                        // create from the dateObj based on the format, but I'll leave that for now
//                        // Or I could switch case and limit the types of formats...
//                    }
//                    // Initialize the date-picker
//                    $(element).datepicker({
//                        format: format,
//                    }).on('changeDate', function (ev) {
//                        // To me this looks cleaner than adding $apply(); after everything.
//                        scope.$apply(function () {
//                            ngModelCtrl.$setViewValue(ev.format(format));
//                        });
//                    });
//                }
//            }
//        }
//    }
//});
//// this datepicker also work chcnge event in angular js
//app.directive('datepicker', function ($filter) {
//    return {
//        restrict: 'A',
//        require: 'ngModel',
//        compile: function () {
//            return {
//                pre: function (scope, element, attrs, ngModelCtrl) {
//                    var format, dateObj;
//                    format = (!attrs.dpFormat) ? 'dd-M-yyyy' : attrs.dpFormat;
//                    if (!attrs.initDate && !attrs.dpFormat) {
//                        // If there is no initDate attribute than we will get todays date as the default
//                        dateObj = new Date();

//                        //scope[attrs.ngModel] = dateObj.getDate() + '/' + (dateObj.getMonth() + 1) + '/' + dateObj.getFullYear();
//                        scope[attrs.ngModel] = $filter("date")(dateObj, 'dd-MMM-yyyy')

//                    } else if (!attrs.initDate) {
//                        // Otherwise set as the init date
//                        scope[attrs.ngModel] = attrs.initDate;
//                    } else {
//                        // I could put some complex logic that changes the order of the date string I
//                        // create from the dateObj based on the format, but I'll leave that for now
//                        // Or I could switch case and limit the types of formats...
//                    }
//                    // Initialize the date-picker
//                    $(element).datepicker({
//                        format: format,
//                    }).on('changeDate', function (ev) {
//                        // To me this looks cleaner than adding $apply(); after everything.
//                        scope.$apply(function () {
//                            ngModelCtrl.$setViewValue(ev.format(format));
//                        });
//                    });
//                }
//            }
//        }
//    }
//});

ngApp.directive("datepicker", function () {
        return {
            restrict: "A",
            link: function (scope, el, attr) {
                el.datepicker({
                    format: 'dd/mm/yyyy',
                    todayBtn: 'linked',
                    todayHighlight: true,
                    autoclose: true,
                });

            }
        };
    })
ngApp.directive("datepicker1", function () {
        return {
            restrict: "A",
            link: function (scope, el, attr) {
                el.datepicker({
                    format: 'dd/mm/yyyy',
                    todayBtn: 'linked',
                    todayHighlight: true,
                    autoclose: true,
                });
            }
        };
    })
    
