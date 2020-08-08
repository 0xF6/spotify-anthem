import { Player } from "./tools/player";
import get from "./tools/get";
import "./index.css";
window.harsomtus = {
    init: function(config: { origin?: string }) {
        if (!config)
            config = {};
        if (!config.origin)
            config.origin = 'https://spotify.0xf6.moe';
        //if (!config.apiKey)
        //    throw new Error('api key is not set.');
        //if (!config.containerId)
        //    throw new Error('container id is not set.');
        get<Anthem>(`${config.origin}/api/@me/anthem`)
        .then(function (response) {
                new Player({}).init(response);
        });
    }
};