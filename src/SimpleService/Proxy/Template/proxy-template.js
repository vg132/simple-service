var simpleServiceProxy = simpleServiceProxy || {
    beginLoading: function () { },
    finishedLoading: function () { },
    error: function (jqXHR, textStatus, errorThrown) { alert('Service could not be called. \nError: ' + textStatus + ' ' + errorThrown); },

    callService: function (serviceUrl, methodName, parameters, callback) {
        simpleServiceProxy.beginLoading();

        $.ajax(serviceUrl + methodName, {
            type: 'POST',
            data: parameters,
            success: function (data) {
                simpleServiceProxy.finishedLoading();

                if (callback) {
                    callback({ value: data });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                simpleServiceProxy.finishedLoading();
                simpleServiceProxy.error(jqXHR, textStatus, errorThrown);

                if (callback) {
                    callback({ error: textStatus + ' ' + errorThrown });
                }
            }
        });
    }
};

var $ServiceName$ = function () { };
$ServiceName$.prototype.url = '$ServiceUrl$';
/* start-repeat:Methods */
$ServiceName$.prototype.$MethodName$ = function ($ParameterString$) {
    simpleServiceProxy.callService(this.url, '$ServiceMethodName$', $ParameterObject$, callback);
};
/* end-repeat */