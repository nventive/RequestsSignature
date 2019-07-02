const sdk = require('postman-collection');

// Parameters
const signatureClientId = pm.variables.get("signatureClientId") || pm.environment.get("signatureClientId");
if (!signatureClientId) {
  throw new Error("Missing signatureClientId variable to configure the client id for request signature.");
}
const signatureClientSecret = pm.variables.get("signatureClientSecret") || pm.environment.get("signatureClientSecret");
if (!signatureClientSecret) {
  throw new Error("Missing signatureClientSecret variable to configure the secret for request signature.");
}

const signatureHeaderName = pm.variables.get("signatureHeaderName") || pm.environment.get("signatureHeaderName") || "X-RequestSignature";
let signatureBodySourceComponents = pm.variables.get("signatureBodySourceComponents") || pm.environment.get("signatureBodySourceComponents") || ["Nonce", "Timestamp", "Method", "Scheme", "Host", "LocalPath", "QueryString", "Body"];
if (typeof signatureBodySourceComponents === "string") {
  signatureBodySourceComponents = JSON.parse(signatureBodySourceComponents);
}
const signaturePattern = pm.variables.get("signaturePattern") || pm.environment.get("signaturePattern") || "{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}";

// Variables
const timestamp = Math.round(new Date() / 1000);
const nonce = require('uuid').v4();
const resolvedRequest = new sdk.Request(pm.request.toJSON()).toObjectResolved(null, [pm.variables.toObject()], { ignoreOwnVariables: true });
const url = new sdk.Url(resolvedRequest.url);

// Utilities
const utf8Bytes = (input) => input.split("").map(c => c.charCodeAt(0));
const byteArrayToWordArray = (byteArray) => {
  const wordArray = [];
  for (let i = 0; i < byteArray.length; i++) {
    wordArray[(i / 4) | 0] |= byteArray[i] << (24 - 8 * i);
  }

  return CryptoJS.lib.WordArray.create(wordArray, byteArray.length);
};

// Signature body computation
let signatureBodySource = [];

signatureBodySourceComponents.forEach(component => {
  switch (component) {
    case "Method":
      signatureBodySource.push(...utf8Bytes(resolvedRequest.method.toUpperCase()));
      break;
    case "Scheme":
      let scheme = url.getHost().split("://")[0];
      signatureBodySource.push(...utf8Bytes(scheme));
      break;
    case "Host":
      let host = url.getHost().split("://")[1].split(":")[0];
      signatureBodySource.push(...utf8Bytes(host));
      break;
    case "Port":
      let port = url.getHost().split("://")[1].split(":")[1];
      if (port) {
        signatureBodySource.push(...utf8Bytes(port));
      }
      break;
    case "LocalPath":
      let localPath = url.getPath();
      if (localPath) {
        signatureBodySource.push(...utf8Bytes(localPath));
      }
      break;
    case "QueryString":
      let queryString = url.getQueryString();
      if (queryString) {
        signatureBodySource.push(...utf8Bytes(`?${queryString}`));
      }
      break;
    case "Body":
      if (resolvedRequest.body) {
        const requestBody = new sdk.RequestBody(resolvedRequest.body);
        if (requestBody.toString()) {
          signatureBodySource.push(...utf8Bytes(requestBody.toString()));
        }
      }
      break;
    case "Timestamp":
      signatureBodySource.push(...utf8Bytes(`${timestamp}`));
      break;
    case "Nonce":
      signatureBodySource.push(...utf8Bytes(`${nonce}`));
      break;
    default:
      if (!component.startsWith("Header")) {
        throw new Error(`Unrecognized signature body source component ${component}.`);
      }
      const headerName = component.substr("Header".length);
      const header = resolvedRequest.header && resolvedRequest.header.find(x => x.key === headerName);
      if (header && header.value) {
        signatureBodySource.push(...utf8Bytes(header.value));
      }
  }
});

const signatureBody = CryptoJS.enc.Base64.stringify(CryptoJS.HmacSHA256(byteArrayToWordArray(signatureBodySource), signatureClientSecret));

const signature = signaturePattern
  .replace("{ClientId}", signatureClientId)
  .replace("{Nonce}", `${nonce}`)
  .replace("{Timestamp}", `${timestamp}`)
  .replace("{SignatureBody}", signatureBody);

pm.request.headers.add({
  key: signatureHeaderName,
  value: signature
});