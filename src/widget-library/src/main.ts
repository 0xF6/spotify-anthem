import '@fortawesome/fontawesome-free/css/all.css';
import "./index.css";

import get from "./get";
import { Player } from "./player";
import { buildWidget } from "./widget.builder";

window.harsomtus = {
    init: function(config: { origin?: string, containerId?: string, apiKey?: string }) {
        if (!config)
            config = {};
        if (!config.origin)
            config.origin = 'https://spotify.0xf6.moe';
            //config.origin = 'https://localhost:12904';
        if (!config.apiKey)
            throw new Error('api key is not set.');
        if (!config.containerId)
            throw new Error('container id is not set.');
        buildWidget(config.containerId);
        get<Anthem>(`${config.origin}/api/@me/anthem`, config.apiKey)
        .then(function (response) {
                new Player({}).init(response);
        });
    }
};