// todo -> temporary
const htmlTracks: string = `
<div id="player-track">
    <div id="album-name"></div>
    <div id="track-name"></div>
    <div id="track-time">
        <div id="current-time"></div>
        <div id="track-length"></div>
    </div>
    <div id="s-area">
        <div id="ins-time"></div>
        <div id="s-hover"></div>
        <div id="seek-bar"></div>
    </div>
</div>
`;
const htmlContent: string = `
<div id="player-content">
    <a id="buffer-box" href="https://github.com/0xF6/spotify-anthem" target="_blank">Spotify Anthem</a>
    <div id="album-art">
        <img src="https://user-images.githubusercontent.com/13326808/89699856-c5e60600-d932-11ea-9071-270e186b370c.png" class="active" id="albumImage">
    </div>
    <div id="player-controls">
        <div>
            <div class="widget-button" id="play-pause-button">
                <i class="fas fa-play"></i>
            </div>
        </div>
        <div>
            <div class="widget-button" id="open-button">
                <i class="fab fa-spotify"></i>
            </div>
        </div>
    </div>
</div>
`;
export function buildWidget(id: string): void {
    document.getElementById(id).innerHTML = `
    <div style="visibility: hidden;" id="spotify-widget-proxy">${htmlTracks}${htmlContent}</div>
    `;
}