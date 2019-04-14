// Uses the decorator pattern to wrap the existing $interpolate service, through which all angular binding expressions (either with 'ng-bind', or with '{{ }}') pass.
// This service provides one extra piece of functionality: it logs all Angular binding expressions and their associated values to the console window,
// enabling the dev to view / investigate returned values, as well as see how often the expressions are evaluated, using the dev tools console window on the browser.
(function (module) {
    module.config(function ($provide) {
        // $delegate parameter represents the original service being decorated, in this case $inerpolate)
        // $log parameter allows us to dump output to the console.
        $provide.decorator("$interpolate", function($delegate, $log) {

            // The function which evaluates the binding expression and dumps it out to the console window.
            var bindingWrapper = function (bindingFunction, bindingExpression) {
                return function () {
                    var result = bindingFunction.apply(this, arguments);
                    var trimmedResult = result.trim();
                    var log = trimmedResult ? $log.info : $log.warn; // if the expression has no value, it'll show up as a warning in the console.
                    log.call($log, bindingExpression + " = " + trimmedResult);
                    return result;
                }
            };

            // Fires every time an Angular binding expression is updated.
            var serviceWrapper = function () {
                // $delegate.apply invokes the original interpolate service (which returns a binding function)
                // passing in current context and all the original arguments.
                var bindingFunction = $delegate.apply(this, arguments);

                // arguments[0] is the binding expression.
                if (angular.isFunction(bindingFunction) && arguments[0]) {
                    return bindingWrapper(bindingFunction, arguments[0].trim());
                };
                return bindingFunction;
            };

            // Ensures that our service has all the angular properties and methods that the regular $interpolate service has
            // (i.e. the minimum requirements for the Decorator pattern).
            angular.extend(serviceWrapper, $delegate);
            return serviceWrapper;
        });
    });

}(angular.module("transporterViewer")));