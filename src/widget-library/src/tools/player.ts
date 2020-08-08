type bufferingTime = { current?: number, target?: number };

export class Player {
    private _audio: HTMLAudioElement;
    private _seek: { t: number, loc: number };
    private _track: { name?: string, artwork?: string, url?: string, artistName?: string, spotifyUrl?: string };
    private _bufferingHandler?: NodeJS.Timeout;
    private _isActive: boolean;

    private _bufferingTime: bufferingTime;

    constructor(config: {}) {
        this._seek = { t: 0, loc: 0 };
        this._track = {};
        this._bufferingTime = {};
        this._isActive = false;
        this._bufferingHandler = null;
    }

    public init(album: Anthem): void {
        this._track.artwork = album.imageUrl;
        this._track.name = album.name;
        this._track.url = album.previewUrl;
        this._track.spotifyUrl = album.previewUrl;
        this._track.artistName = album.artistName;
        this.setArtworkImage(this._track.artwork);

        this._audio = new Audio();
        this._audio.loop = true;
        this._audio.volume = 0.4;

        this.selectTrack(0);

        $("#play-pause-button").on('click', this.switchState.bind(this));
        $("#open-button").on('click', this.openSpotifyPage.bind(this));
        $('#s-area').mousemove(this.showHover.bind(this) as any);
        $('#s-area').mouseout(this.hideHover.bind(this));
        $('#s-area').on('click', this.playFromClickedPos.bind(this));
        $(this._audio).on('timeupdate', this.updateCurrTime.bind(this));

        setTimeout(function () {
            $("#player").css("visibility", "visible");
        }, 50);
    }

    public setArtworkImage(url: string): void {
        $("#_1").attr("src", url);
    }


    public switchState(): void {
        setTimeout(this._switchState.bind(this), 300);
    }
    public showHover(event: MouseEvent): void {
        let seekBarPos = $('#s-area').offset();
        this._seek.t = event.clientX - seekBarPos.left;
        this._seek.loc = this.getAudio().duration * (this._seek.t / $('#s-area').outerWidth());

        let { t, loc } = this._seek;

        $('#s-hover').width(t);

        let ctMinutes: number | string = Math.floor(loc / 60);
        let ctSeconds: number | string = Math.floor(loc - ctMinutes * 60);

        if ((ctMinutes < 0) || (ctSeconds < 0))
            return;
        if ((ctMinutes < 0) || (ctSeconds < 0))
            return;
        if (ctMinutes < 10)
            ctMinutes = '0' + ctMinutes;
        if (ctSeconds < 10)
            ctSeconds = '0' + ctSeconds;
        if (isNaN(+ctMinutes) || isNaN(+ctSeconds))
            $('#ins-time').text('--:--');
        else
            $('#ins-time').text(ctMinutes + ':' + ctSeconds);
        $('#ins-time').css({ 'left': t, 'margin-left': '-21px' }).fadeIn(0);
    }

    public hideHover(): void {
        $('#s-hover').width(0);
        $('#ins-time').text('00:00').css({ 'left': '0px', 'margin-left': '0px' }).fadeOut(0);
    }

    public getAudio = (): HTMLAudioElement =>
        this._audio;
    public get isActive(): boolean {
        return this._isActive;
    }
    public set isActive(value: boolean) {
        this._isActive = value;
    }


    public playFromClickedPos(): void {
        this.getAudio().currentTime = this._seek.loc;
        $('#seek-bar').width(this._seek.t);
        this.hideHover();
    }


    private _switchState() {
        if (this.getAudio().paused) {
            $("#player-track").addClass('active');
            $('#album-art').addClass('active');
            this.checkBuffering();
            $("#play-pause-button").find('i').attr('class', 'fas fa-pause');
            this.getAudio().play();
        }
        else {
            $("#player-track").removeClass('active');
            $('#album-art').removeClass('active');
            clearInterval(this._bufferingHandler);
            $("#play-pause-button").find('i').attr('class', 'fas fa-play');
            this.getAudio().pause();
        }
    }

    private updateCurrTime(): void {

        let audio = this.getAudio();

        this._bufferingTime.current = new Date().getTime();

        if (!this.isActive) {
            this.isActive = true;
            $('#track-time').addClass('active');
        }

        let curMinutes: number | string = Math.floor(audio.currentTime / 60);
        let curSeconds: number | string = Math.floor(audio.currentTime - curMinutes * 60);

        let durMinutes: number | string = Math.floor(audio.duration / 60);
        let durSeconds: number | string = Math.floor(audio.duration - durMinutes * 60);

        let playProgress = (audio.currentTime / audio.duration) * 100;

        if (curMinutes < 10)
            curMinutes = '0' + curMinutes;
        if (curSeconds < 10)
            curSeconds = '0' + curSeconds;

        if (durMinutes < 10)
            durMinutes = '0' + durMinutes;
        if (durSeconds < 10)
            durSeconds = '0' + durSeconds;

        if (isNaN(+curMinutes) || isNaN(+curSeconds))
            $('#current-time').text('00:00');
        else
            $('#current-time').text(curMinutes + ':' + curSeconds);

        if (isNaN(+durMinutes) || isNaN(+durSeconds))
            $('#track-length').text('00:00');
        else
            $('#track-length').text(durMinutes + ':' + durSeconds);

        if (isNaN(+curMinutes) || isNaN(+curSeconds) || isNaN(+durMinutes) || isNaN(+durSeconds))
            $('#track-time').removeClass('active');
        else
            $('#track-time').addClass('active');


        $('#seek-bar').width(playProgress + '%');

        if (playProgress == 100) {
            $("#play-pause-button").find('i').attr('class', 'fa fa-play');
            $('#seek-bar').width(0);
            $('#current-time').text('00:00');
            $('#album-art').removeClass('active');
            clearInterval(this._bufferingHandler);
        }
    }

    public selectTrack(flag): void {
        if (flag == 0)
            $("#play-pause-button").find('i').attr('class', 'fa fa-play');
        else {
            console.log("audio::buffering complete.");
            $("#play-pause-button").find('i').attr('class', 'fa fa-pause');
        }

        $('#seek-bar').width(0);
        $('#track-time').removeClass('active');
        $('#current-time').text('00:00');
        $('#track-length').text('00:00');

        this.getAudio().src = this._track.url;

        this._bufferingTime.current = 0;
        this._bufferingTime.target = new Date().getTime();

        if (flag != 0) {
            this.getAudio().play();
            $("#player-track").addClass('active');
            $('#album-art').addClass('active');

            clearInterval(this._bufferingHandler);
            this.checkBuffering();
        }

        $('#album-name').text(this._track.artistName);
        $('#track-name').text(this._track.name);
        $('#album-art').find('img.active').removeClass('active');
        $('#_1').addClass('active');
    }

    private openSpotifyPage(): void {
        window.open(this._track.spotifyUrl, "_blank");
    }

    private checkBuffering(): void {
        clearInterval(this._bufferingHandler);
        this._bufferingHandler = setInterval(this.bufferingAction.bind(this), 100);
    }

    private bufferingAction(): void {
        let { current /* n */, target /* b */ } = this._bufferingTime;
        if ((current == 0) || (target - current) > 1000)
            console.log("audio::start buffering...");
        else
            console.log("audio::buffering...");
        this._bufferingTime.target = new Date().getTime();
    }
}