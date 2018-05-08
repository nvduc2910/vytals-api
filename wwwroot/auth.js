(function () {
    $(function () {
        var basicAuthUI =
            '<div class="input"><input placeholder="access_token" id="input_username" name="username" type="text" size="20"></div><div class="input"><input placeholder="language" id="input_lang" name="language" type="text" size="20"></div>'
        $(basicAuthUI).insertBefore('#api_selector div.input:last-child');
        $("#input_apiKey").hide();
        $('#input_username').change(addAuthorization);
        $('#input_lang').change(addAuthorization);
    });

    function addAuthorization() {
        var username = $('#input_username').val();
        var language = $('#input_lang').val();
        if (username.trim() != "") {

            var apiKeyAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + username, "header");
            var apiKeyAuth1 = new SwaggerClient.ApiKeyAuthorization("Accept-Language", language, "header");
            window.swaggerUi.api.clientAuthorizations.add("bearer", apiKeyAuth);

        }
    }
})();