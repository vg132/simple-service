var simpleServiceProxy = simpleServiceProxy || {
    beginLoading: function () { },
    finishedLoading: function () { },
    error: function (xhr, textStatus, errorThrown) { alert('Service could not be called. \n\nError: ' + $(xhr.responseText).find('i:first').text()); },

    callService: function (serviceUrl, methodName, parameters, callback) {
        simpleServiceProxy.beginLoading();
        parameters._callingUrl = location.href;
        
        return $.ajax(serviceUrl + methodName, {
            type: 'POST',
            data: parameters,
            success: function (data) {
                simpleServiceProxy.finishedLoading();

                if (callback) {
                    callback({ value: data });
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                simpleServiceProxy.finishedLoading();
                simpleServiceProxy.error(xhr, textStatus, errorThrown);

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
    return simpleServiceProxy.callService(this.url, '$ServiceMethodName$', $ParameterObject$, callback);
};
/* end-repeat */