const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
    ],
    target: "http://localhost:5036",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
