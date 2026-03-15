ngApp.directive('disableButton', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            const timeoutDuration = 1000; // Timeout duration in milliseconds

            element.bind('click', function () {
                element.prop('disabled', true);

                let countdown = timeoutDuration / 1000; // Convert to seconds
                scope.remainingTime = countdown;

                const countdownInterval = setInterval(function () {
                    countdown--;
                    scope.$apply(function () {
                        scope.remainingTime = countdown;
                    });
                    if (countdown <= 0) {
                        clearInterval(countdownInterval);
                        element.prop('disabled', false);
                        scope.remainingTime = null; // Reset remaining time when countdown ends
                    }
                }, 1000);
            });
        }
    };
});


ngApp.directive('select2Init', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).select2();
        }
    };
});

//ngApp.directive('select2Init', function () {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//            $(element).select2({
//                allowClear: true
//            });
//        }
//    };
//});

ngApp.directive('decimalPlaces', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            ngModelCtrl.$parsers.push(function (value) {
                var transformedInput = value.replace(/[^0-9.]/g, ''); // Remove non-numeric characters except decimal point
                var decimalSplit = transformedInput.split('.');
                if (decimalSplit.length > 2) {
                    transformedInput = decimalSplit[0] + '.' + decimalSplit[1]; // Keep only the first decimal point
                }
                ngModelCtrl.$setViewValue(transformedInput);
                ngModelCtrl.$render();
                return transformedInput;
            });
        }
    };
});
