module.exports = function (api) {
  api.cache(true);
  return {
    plugins: [["relay", { artifactDirectory: "./__generated__" }]],
    presets: ["babel-preset-expo"],
  };
};
