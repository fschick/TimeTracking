export const environment = {
  production: true,
  timeTrackingApiBasePath: `${document.baseURI.replace(/\/$/, '')}`,
  reportingApiBasePath: `${location.protocol}//${location.hostname}:5010`
};
