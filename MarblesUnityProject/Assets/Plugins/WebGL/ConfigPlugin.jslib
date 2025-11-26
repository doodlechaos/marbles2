mergeInto(LibraryManager.library, {
    GetSpacetimeDBHost: function () {
        // Read from window config injected by the web client, with fallback
        var host = (window.SPACETIMEDB_CONFIG && window.SPACETIMEDB_CONFIG.host) 
            || "http://127.0.0.1:3000";
        var bufferSize = lengthBytesUTF8(host) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(host, buffer, bufferSize);
        return buffer;
    },

    GetSpacetimeDBModuleName: function () {
        // Read from window config injected by the web client, with fallback
        var moduleName = (window.SPACETIMEDB_CONFIG && window.SPACETIMEDB_CONFIG.moduleName) 
            || "marbles2";
        var bufferSize = lengthBytesUTF8(moduleName) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(moduleName, buffer, bufferSize);
        return buffer;
    }
});

