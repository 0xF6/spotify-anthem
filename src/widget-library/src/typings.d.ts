declare class Anthem {
    artistName: string;
    externalLink: string;
    imageUrl: string;
    name: string;
    previewUrl: string;
    uri: string;
}

interface Window {
    harsomtus: { init(config: { origin?: string }) } ;
}