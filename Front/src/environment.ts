const isLocal = window.location.hostname === 'localhost';

export const environment = {
  production: !isLocal,
  apiUrl: isLocal ? 'https://localhost:7260/api' : 'server url',
};
