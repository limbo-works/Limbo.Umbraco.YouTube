// Both names are kept for backwards compatibility: "limbo-youtube-alt" is the icon of the property editor, and
// the two SVGs were identical in previous versions of this package.
export default [
    {
        name: "limbo-youtube",
        path: () => import("./Icons/YouTube.js")
    },
    {
        name: "limbo-youtube-alt",
        path: () => import("./Icons/YouTube.js")
    }
];
