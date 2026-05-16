//! Licensed to the .NET Foundation under one or more agreements.
//! The .NET Foundation licenses this file to you under the MIT license.

var e=!1;const t=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,4,1,96,0,0,3,2,1,0,10,8,1,6,0,6,64,25,11,11])),o=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,15,1,13,0,65,1,253,15,65,2,253,15,253,128,2,11])),n=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,10,1,8,0,65,0,253,15,253,98,11])),r=Symbol.for("wasm promise_control");function i(e,t){let o=null;const n=new Promise((function(n,r){o={isDone:!1,promise:null,resolve:t=>{o.isDone||(o.isDone=!0,n(t),e&&e())},reject:e=>{o.isDone||(o.isDone=!0,r(e),t&&t())}}}));o.promise=n;const i=n;return i[r]=o,{promise:i,promise_control:o}}function s(e){return e[r]}function a(e){e&&function(e){return void 0!==e[r]}(e)||Be(!1,"Promise is not controllable")}const l="__mono_message__",c=["debug","log","trace","warn","info","error"],d="MONO_WASM: ";let u,f,m,g,p,h;function w(e){g=e}function b(e){if(Pe.diagnosticTracing){const t="function"==typeof e?e():e;console.debug(d+t)}}function y(e,...t){console.info(d+e,...t)}function v(e,...t){console.info(e,...t)}function E(e,...t){console.warn(d+e,...t)}function _(e,...t){if(t&&t.length>0&&t[0]&&"object"==typeof t[0]){if(t[0].silent)return;if(t[0].toString)return void console.error(d+e,t[0].toString())}console.error(d+e,...t)}function x(e,t,o){return function(...n){try{let r=n[0];if(void 0===r)r="undefined";else if(null===r)r="null";else if("function"==typeof r)r=r.toString();else if("string"!=typeof r)try{r=JSON.stringify(r)}catch(e){r=r.toString()}t(o?JSON.stringify({method:e,payload:r,arguments:n.slice(1)}):[e+r,...n.slice(1)])}catch(e){m.error(`proxyConsole failed: ${e}`)}}}function j(e,t,o){f=t,g=e,m={...t};const n=`${o}/console`.replace("https://","wss://").replace("http://","ws://");u=new WebSocket(n),u.addEventListener("error",A),u.addEventListener("close",S),function(){for(const e of c)f[e]=x(`console.${e}`,T,!0)}()}function R(e){let t=30;const o=()=>{u?0==u.bufferedAmount||0==t?(e&&v(e),function(){for(const e of c)f[e]=x(`console.${e}`,m.log,!1)}(),u.removeEventListener("error",A),u.removeEventListener("close",S),u.close(1e3,e),u=void 0):(t--,globalThis.setTimeout(o,100)):e&&m&&m.log(e)};o()}function T(e){u&&u.readyState===WebSocket.OPEN?u.send(e):m.log(e)}function A(e){m.error(`[${g}] proxy console websocket error: ${e}`,e)}function S(e){m.debug(`[${g}] proxy console websocket closed: ${e}`,e)}function D(){Pe.preferredIcuAsset=O(Pe.config);let e="invariant"==Pe.config.globalizationMode;if(!e)if(Pe.preferredIcuAsset)Pe.diagnosticTracing&&b("ICU data archive(s) available, disabling invariant mode");else{if("custom"===Pe.config.globalizationMode||"all"===Pe.config.globalizationMode||"sharded"===Pe.config.globalizationMode){const e="invariant globalization mode is inactive and no ICU data archives are available";throw _(`ERROR: ${e}`),new Error(e)}Pe.diagnosticTracing&&b("ICU data archive(s) not available, using invariant globalization mode"),e=!0,Pe.preferredIcuAsset=null}const t="DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",o=Pe.config.environmentVariables;if(void 0===o[t]&&e&&(o[t]="1"),void 0===o.TZ)try{const e=Intl.DateTimeFormat().resolvedOptions().timeZone||null;e&&(o.TZ=e)}catch(e){y("failed to detect timezone, will fallback to UTC")}}function O(e){var t;if((null===(t=e.resources)||void 0===t?void 0:t.icu)&&"invariant"!=e.globalizationMode){const t=e.applicationCulture||(ke?globalThis.navigator&&globalThis.navigator.languages&&globalThis.navigator.languages[0]:Intl.DateTimeFormat().resolvedOptions().locale),o=e.resources.icu;let n=null;if("custom"===e.globalizationMode){if(o.length>=1)return o[0].name}else t&&"all"!==e.globalizationMode?"sharded"===e.globalizationMode&&(n=function(e){const t=e.split("-")[0];return"en"===t||["fr","fr-FR","it","it-IT","de","de-DE","es","es-ES"].includes(e)?"icudt_EFIGS.dat":["zh","ko","ja"].includes(t)?"icudt_CJK.dat":"icudt_no_CJK.dat"}(t)):n="icudt.dat";if(n)for(let e=0;e<o.length;e++){const t=o[e];if(t.virtualPath===n)return t.name}}return e.globalizationMode="invariant",null}(new Date).valueOf();const C=class{constructor(e){this.url=e}toString(){return this.url}};async function k(e,t){try{const o="function"==typeof globalThis.fetch;if(Se){const n=e.startsWith("file://");if(!n&&o)return globalThis.fetch(e,t||{credentials:"same-origin"});p||(h=Ne.require("url"),p=Ne.require("fs")),n&&(e=h.fileURLToPath(e));const r=await p.promises.readFile(e);return{ok:!0,headers:{length:0,get:()=>null},url:e,arrayBuffer:()=>r,json:()=>JSON.parse(r),text:()=>{throw new Error("NotImplementedException")}}}if(o)return globalThis.fetch(e,t||{credentials:"same-origin"});if("function"==typeof read)return{ok:!0,url:e,headers:{length:0,get:()=>null},arrayBuffer:()=>new Uint8Array(read(e,"binary")),json:()=>JSON.parse(read(e,"utf8")),text:()=>read(e,"utf8")}}catch(t){return{ok:!1,url:e,status:500,headers:{length:0,get:()=>null},statusText:"ERR28: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t},text:()=>{throw t}}}throw new Error("No fetch implementation available")}function I(e){return"string"!=typeof e&&Be(!1,"url must be a string"),!M(e)&&0!==e.indexOf("./")&&0!==e.indexOf("../")&&globalThis.URL&&globalThis.document&&globalThis.document.baseURI&&(e=new URL(e,globalThis.document.baseURI).toString()),e}const U=/^[a-zA-Z][a-zA-Z\d+\-.]*?:\/\//,P=/[a-zA-Z]:[\\/]/;function M(e){return Se||Ie?e.startsWith("/")||e.startsWith("\\")||-1!==e.indexOf("///")||P.test(e):U.test(e)}let L,N=0;const $=[],z=[],W=new Map,F={"js-module-threads":!0,"js-module-runtime":!0,"js-module-dotnet":!0,"js-module-native":!0,"js-module-diagnostics":!0},B={...F,"js-module-library-initializer":!0},V={...F,dotnetwasm:!0,heap:!0,manifest:!0},q={...B,manifest:!0},H={...B,dotnetwasm:!0},J={dotnetwasm:!0,symbols:!0},Z={...B,dotnetwasm:!0,symbols:!0},Q={symbols:!0};function G(e){return!("icu"==e.behavior&&e.name!=Pe.preferredIcuAsset)}function K(e,t,o){null!=t||(t=[]),Be(1==t.length,`Expect to have one ${o} asset in resources`);const n=t[0];return n.behavior=o,X(n),e.push(n),n}function X(e){V[e.behavior]&&W.set(e.behavior,e)}function Y(e){Be(V[e],`Unknown single asset behavior ${e}`);const t=W.get(e);if(t&&!t.resolvedUrl)if(t.resolvedUrl=Pe.locateFile(t.name),F[t.behavior]){const e=ge(t);e?("string"!=typeof e&&Be(!1,"loadBootResource response for 'dotnetjs' type should be a URL string"),t.resolvedUrl=e):t.resolvedUrl=ce(t.resolvedUrl,t.behavior)}else if("dotnetwasm"!==t.behavior)throw new Error(`Unknown single asset behavior ${e}`);return t}function ee(e){const t=Y(e);return Be(t,`Single asset for ${e} not found`),t}let te=!1;async function oe(){if(!te){te=!0,Pe.diagnosticTracing&&b("mono_download_assets");try{const e=[],t=[],o=(e,t)=>{!Z[e.behavior]&&G(e)&&Pe.expected_instantiated_assets_count++,!H[e.behavior]&&G(e)&&(Pe.expected_downloaded_assets_count++,t.push(se(e)))};for(const t of $)o(t,e);for(const e of z)o(e,t);Pe.allDownloadsQueued.promise_control.resolve(),Promise.all([...e,...t]).then((()=>{Pe.allDownloadsFinished.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),await Pe.runtimeModuleLoaded.promise;const n=async e=>{const t=await e;if(t.buffer){if(!Z[t.behavior]){t.buffer&&"object"==typeof t.buffer||Be(!1,"asset buffer must be array-like or buffer-like or promise of these"),"string"!=typeof t.resolvedUrl&&Be(!1,"resolvedUrl must be string");const e=t.resolvedUrl,o=await t.buffer,n=new Uint8Array(o);pe(t),await Ue.beforeOnRuntimeInitialized.promise,Ue.instantiate_asset(t,e,n)}}else J[t.behavior]?("symbols"===t.behavior&&(await Ue.instantiate_symbols_asset(t),pe(t)),J[t.behavior]&&++Pe.actual_downloaded_assets_count):(t.isOptional||Be(!1,"Expected asset to have the downloaded buffer"),!H[t.behavior]&&G(t)&&Pe.expected_downloaded_assets_count--,!Z[t.behavior]&&G(t)&&Pe.expected_instantiated_assets_count--)},r=[],i=[];for(const t of e)r.push(n(t));for(const e of t)i.push(n(e));Promise.all(r).then((()=>{Ce||Ue.coreAssetsInMemory.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),Promise.all(i).then((async()=>{Ce||(await Ue.coreAssetsInMemory.promise,Ue.allAssetsInMemory.promise_control.resolve())})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e}))}catch(e){throw Pe.err("Error in mono_download_assets: "+e),e}}}let ne=!1;function re(){if(ne)return;ne=!0;const e=Pe.config,t=[];if(e.assets)for(const t of e.assets)"object"!=typeof t&&Be(!1,`asset must be object, it was ${typeof t} : ${t}`),"string"!=typeof t.behavior&&Be(!1,"asset behavior must be known string"),"string"!=typeof t.name&&Be(!1,"asset name must be string"),t.resolvedUrl&&"string"!=typeof t.resolvedUrl&&Be(!1,"asset resolvedUrl could be string"),t.hash&&"string"!=typeof t.hash&&Be(!1,"asset resolvedUrl could be string"),t.pendingDownload&&"object"!=typeof t.pendingDownload&&Be(!1,"asset pendingDownload could be object"),t.isCore?$.push(t):z.push(t),X(t);else if(e.resources){const o=e.resources;o.wasmNative||Be(!1,"resources.wasmNative must be defined"),o.jsModuleNative||Be(!1,"resources.jsModuleNative must be defined"),o.jsModuleRuntime||Be(!1,"resources.jsModuleRuntime must be defined"),K(z,o.wasmNative,"dotnetwasm"),K(t,o.jsModuleNative,"js-module-native"),K(t,o.jsModuleRuntime,"js-module-runtime"),o.jsModuleDiagnostics&&K(t,o.jsModuleDiagnostics,"js-module-diagnostics");const n=(e,t,o)=>{const n=e;n.behavior=t,o?(n.isCore=!0,$.push(n)):z.push(n)};if(o.coreAssembly)for(let e=0;e<o.coreAssembly.length;e++)n(o.coreAssembly[e],"assembly",!0);if(o.assembly)for(let e=0;e<o.assembly.length;e++)n(o.assembly[e],"assembly",!o.coreAssembly);if(0!=e.debugLevel&&Pe.isDebuggingSupported()){if(o.corePdb)for(let e=0;e<o.corePdb.length;e++)n(o.corePdb[e],"pdb",!0);if(o.pdb)for(let e=0;e<o.pdb.length;e++)n(o.pdb[e],"pdb",!o.corePdb)}if(e.loadAllSatelliteResources&&o.satelliteResources)for(const e in o.satelliteResources)for(let t=0;t<o.satelliteResources[e].length;t++){const r=o.satelliteResources[e][t];r.culture=e,n(r,"resource",!o.coreAssembly)}if(o.coreVfs)for(let e=0;e<o.coreVfs.length;e++)n(o.coreVfs[e],"vfs",!0);if(o.vfs)for(let e=0;e<o.vfs.length;e++)n(o.vfs[e],"vfs",!o.coreVfs);const r=O(e);if(r&&o.icu)for(let e=0;e<o.icu.length;e++){const t=o.icu[e];t.name===r&&n(t,"icu",!1)}if(o.wasmSymbols)for(let e=0;e<o.wasmSymbols.length;e++)n(o.wasmSymbols[e],"symbols",!1)}if(e.appsettings)for(let t=0;t<e.appsettings.length;t++){const o=e.appsettings[t],n=he(o);"appsettings.json"!==n&&n!==`appsettings.${e.applicationEnvironment}.json`||z.push({name:o,behavior:"vfs",cache:"no-cache",useCredentials:!0})}e.assets=[...$,...z,...t]}async function ie(e){const t=await se(e);return await t.pendingDownloadInternal.response,t.buffer}async function se(e){try{return await ae(e)}catch(t){if(!Pe.enableDownloadRetry)throw t;if(Ie||Se)throw t;if(e.pendingDownload&&e.pendingDownloadInternal==e.pendingDownload)throw t;if(e.resolvedUrl&&-1!=e.resolvedUrl.indexOf("file://"))throw t;if(t&&404==t.status)throw t;e.pendingDownloadInternal=void 0,await Pe.allDownloadsQueued.promise;try{return Pe.diagnosticTracing&&b(`Retrying download '${e.name}'`),await ae(e)}catch(t){return e.pendingDownloadInternal=void 0,await new Promise((e=>globalThis.setTimeout(e,100))),Pe.diagnosticTracing&&b(`Retrying download (2) '${e.name}' after delay`),await ae(e)}}}async function ae(e){for(;L;)await L.promise;try{++N,N==Pe.maxParallelDownloads&&(Pe.diagnosticTracing&&b("Throttling further parallel downloads"),L=i());const t=await async function(e){if(e.pendingDownload&&(e.pendingDownloadInternal=e.pendingDownload),e.pendingDownloadInternal&&e.pendingDownloadInternal.response)return e.pendingDownloadInternal.response;if(e.buffer){const t=await e.buffer;return e.resolvedUrl||(e.resolvedUrl="undefined://"+e.name),e.pendingDownloadInternal={url:e.resolvedUrl,name:e.name,response:Promise.resolve({ok:!0,arrayBuffer:()=>t,json:()=>JSON.parse(new TextDecoder("utf-8").decode(t)),text:()=>{throw new Error("NotImplementedException")},headers:{get:()=>{}}})},e.pendingDownloadInternal.response}const t=e.loadRemote&&Pe.config.remoteSources?Pe.config.remoteSources:[""];let o;for(let n of t){n=n.trim(),"./"===n&&(n="");const t=le(e,n);e.name===t?Pe.diagnosticTracing&&b(`Attempting to download '${t}'`):Pe.diagnosticTracing&&b(`Attempting to download '${t}' for ${e.name}`);try{e.resolvedUrl=t;const n=fe(e);if(e.pendingDownloadInternal=n,o=await n.response,!o||!o.ok)continue;return o}catch(e){o||(o={ok:!1,url:t,status:0,statusText:""+e});continue}}const n=e.isOptional||e.name.match(/\.pdb$/)&&Pe.config.ignorePdbLoadErrors;if(o||Be(!1,`Response undefined ${e.name}`),!n){const t=new Error(`download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`);throw t.status=o.status,t}y(`optional download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`)}(e);return t?(J[e.behavior]||(e.buffer=await t.arrayBuffer(),++Pe.actual_downloaded_assets_count),e):e}finally{if(--N,L&&N==Pe.maxParallelDownloads-1){Pe.diagnosticTracing&&b("Resuming more parallel downloads");const e=L;L=void 0,e.promise_control.resolve()}}}function le(e,t){let o;return null==t&&Be(!1,`sourcePrefix must be provided for ${e.name}`),e.resolvedUrl?o=e.resolvedUrl:(o=""===t?"assembly"===e.behavior||"pdb"===e.behavior?e.name:"resource"===e.behavior&&e.culture&&""!==e.culture?`${e.culture}/${e.name}`:e.name:t+e.name,o=ce(Pe.locateFile(o),e.behavior)),o&&"string"==typeof o||Be(!1,"attemptUrl need to be path or url string"),o}function ce(e,t){return Pe.modulesUniqueQuery&&q[t]&&(e+=Pe.modulesUniqueQuery),e}let de=0;const ue=new Set;function fe(e){try{e.resolvedUrl||Be(!1,"Request's resolvedUrl must be set");const t=function(e){let t=e.resolvedUrl;if(Pe.loadBootResource){const o=ge(e);if(o instanceof Promise)return o;"string"==typeof o&&(t=o)}const o={};return e.cache?o.cache=e.cache:Pe.config.disableNoCacheFetch||(o.cache="no-cache"),e.useCredentials?o.credentials="include":!Pe.config.disableIntegrityCheck&&e.hash&&(o.integrity=e.hash),Pe.fetch_like(t,o)}(e),o={name:e.name,url:e.resolvedUrl,response:t};return ue.add(e.name),o.response.then((()=>{"assembly"==e.behavior&&Pe.loadedAssemblies.push(e.name),de++,Pe.onDownloadResourceProgress&&Pe.onDownloadResourceProgress(de,ue.size)})),o}catch(t){const o={ok:!1,url:e.resolvedUrl,status:500,statusText:"ERR29: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t}};return{name:e.name,url:e.resolvedUrl,response:Promise.resolve(o)}}}const me={resource:"assembly",assembly:"assembly",pdb:"pdb",icu:"globalization",vfs:"configuration",manifest:"manifest",dotnetwasm:"dotnetwasm","js-module-dotnet":"dotnetjs","js-module-native":"dotnetjs","js-module-runtime":"dotnetjs","js-module-threads":"dotnetjs"};function ge(e){var t;if(Pe.loadBootResource){const o=null!==(t=e.hash)&&void 0!==t?t:"",n=e.resolvedUrl,r=me[e.behavior];if(r){const t=Pe.loadBootResource(r,e.name,n,o,e.behavior);return"string"==typeof t?I(t):t}}}function pe(e){e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null}function he(e){let t=e.lastIndexOf("/");return t>=0&&t++,e.substring(t)}async function we(e){e&&await Promise.all((null!=e?e:[]).map((e=>async function(e){try{const t=e.name;if(!e.moduleExports){const o=ce(Pe.locateFile(t),"js-module-library-initializer");Pe.diagnosticTracing&&b(`Attempting to import '${o}' for ${e}`),e.moduleExports=await import(/*! webpackIgnore: true */o)}Pe.libraryInitializers.push({scriptName:t,exports:e.moduleExports})}catch(t){E(`Failed to import library initializer '${e}': ${t}`)}}(e))))}async function be(e,t){if(!Pe.libraryInitializers)return;const o=[];for(let n=0;n<Pe.libraryInitializers.length;n++){const r=Pe.libraryInitializers[n];r.exports[e]&&o.push(ye(r.scriptName,e,(()=>r.exports[e](...t))))}await Promise.all(o)}async function ye(e,t,o){try{await o()}catch(o){throw E(`Failed to invoke '${t}' on library initializer '${e}': ${o}`),Xe(1,o),o}}function ve(e,t){if(e===t)return e;const o={...t};return void 0!==o.assets&&o.assets!==e.assets&&(o.assets=[...e.assets||[],...o.assets||[]]),void 0!==o.resources&&(o.resources=_e(e.resources||{assembly:[],jsModuleNative:[],jsModuleRuntime:[],wasmNative:[]},o.resources)),void 0!==o.environmentVariables&&(o.environmentVariables={...e.environmentVariables||{},...o.environmentVariables||{}}),void 0!==o.runtimeOptions&&o.runtimeOptions!==e.runtimeOptions&&(o.runtimeOptions=[...e.runtimeOptions||[],...o.runtimeOptions||[]]),Object.assign(e,o)}function Ee(e,t){if(e===t)return e;const o={...t};return o.config&&(e.config||(e.config={}),o.config=ve(e.config,o.config)),Object.assign(e,o)}function _e(e,t){if(e===t)return e;const o={...t};return void 0!==o.coreAssembly&&(o.coreAssembly=[...e.coreAssembly||[],...o.coreAssembly||[]]),void 0!==o.assembly&&(o.assembly=[...e.assembly||[],...o.assembly||[]]),void 0!==o.lazyAssembly&&(o.lazyAssembly=[...e.lazyAssembly||[],...o.lazyAssembly||[]]),void 0!==o.corePdb&&(o.corePdb=[...e.corePdb||[],...o.corePdb||[]]),void 0!==o.pdb&&(o.pdb=[...e.pdb||[],...o.pdb||[]]),void 0!==o.jsModuleWorker&&(o.jsModuleWorker=[...e.jsModuleWorker||[],...o.jsModuleWorker||[]]),void 0!==o.jsModuleNative&&(o.jsModuleNative=[...e.jsModuleNative||[],...o.jsModuleNative||[]]),void 0!==o.jsModuleDiagnostics&&(o.jsModuleDiagnostics=[...e.jsModuleDiagnostics||[],...o.jsModuleDiagnostics||[]]),void 0!==o.jsModuleRuntime&&(o.jsModuleRuntime=[...e.jsModuleRuntime||[],...o.jsModuleRuntime||[]]),void 0!==o.wasmSymbols&&(o.wasmSymbols=[...e.wasmSymbols||[],...o.wasmSymbols||[]]),void 0!==o.wasmNative&&(o.wasmNative=[...e.wasmNative||[],...o.wasmNative||[]]),void 0!==o.icu&&(o.icu=[...e.icu||[],...o.icu||[]]),void 0!==o.satelliteResources&&(o.satelliteResources=function(e,t){if(e===t)return e;for(const o in t)e[o]=[...e[o]||[],...t[o]||[]];return e}(e.satelliteResources||{},o.satelliteResources||{})),void 0!==o.modulesAfterConfigLoaded&&(o.modulesAfterConfigLoaded=[...e.modulesAfterConfigLoaded||[],...o.modulesAfterConfigLoaded||[]]),void 0!==o.modulesAfterRuntimeReady&&(o.modulesAfterRuntimeReady=[...e.modulesAfterRuntimeReady||[],...o.modulesAfterRuntimeReady||[]]),void 0!==o.extensions&&(o.extensions={...e.extensions||{},...o.extensions||{}}),void 0!==o.vfs&&(o.vfs=[...e.vfs||[],...o.vfs||[]]),Object.assign(e,o)}function xe(){const e=Pe.config;if(e.environmentVariables=e.environmentVariables||{},e.runtimeOptions=e.runtimeOptions||[],e.resources=e.resources||{assembly:[],jsModuleNative:[],jsModuleWorker:[],jsModuleRuntime:[],wasmNative:[],vfs:[],satelliteResources:{}},e.assets){Pe.diagnosticTracing&&b("config.assets is deprecated, use config.resources instead");for(const t of e.assets){const o={};switch(t.behavior){case"assembly":o.assembly=[t];break;case"pdb":o.pdb=[t];break;case"resource":o.satelliteResources={},o.satelliteResources[t.culture]=[t];break;case"icu":o.icu=[t];break;case"symbols":o.wasmSymbols=[t];break;case"vfs":o.vfs=[t];break;case"dotnetwasm":o.wasmNative=[t];break;case"js-module-threads":o.jsModuleWorker=[t];break;case"js-module-runtime":o.jsModuleRuntime=[t];break;case"js-module-native":o.jsModuleNative=[t];break;case"js-module-diagnostics":o.jsModuleDiagnostics=[t];break;case"js-module-dotnet":break;default:throw new Error(`Unexpected behavior ${t.behavior} of asset ${t.name}`)}_e(e.resources,o)}}e.debugLevel,e.applicationEnvironment||(e.applicationEnvironment="Production"),e.applicationCulture&&(e.environmentVariables.LANG=`${e.applicationCulture}.UTF-8`),Ue.diagnosticTracing=Pe.diagnosticTracing=!!e.diagnosticTracing,Ue.waitForDebugger=e.waitForDebugger,Pe.maxParallelDownloads=e.maxParallelDownloads||Pe.maxParallelDownloads,Pe.enableDownloadRetry=void 0!==e.enableDownloadRetry?e.enableDownloadRetry:Pe.enableDownloadRetry}let je=!1;async function Re(e){var t;if(je)return void await Pe.afterConfigLoaded.promise;let o;try{if(e.configSrc||Pe.config&&0!==Object.keys(Pe.config).length&&(Pe.config.assets||Pe.config.resources)||(e.configSrc="dotnet.boot.js"),o=e.configSrc,je=!0,o&&(Pe.diagnosticTracing&&b("mono_wasm_load_config"),await async function(e){const t=e.configSrc,o=Pe.locateFile(t);let n=null;void 0!==Pe.loadBootResource&&(n=Pe.loadBootResource("manifest",t,o,"","manifest"));let r,i=null;if(n)if("string"==typeof n)n.includes(".json")?(i=await s(I(n)),r=await Ae(i)):r=(await import(I(n))).config;else{const e=await n;"function"==typeof e.json?(i=e,r=await Ae(i)):r=e.config}else o.includes(".json")?(i=await s(ce(o,"manifest")),r=await Ae(i)):r=(await import(ce(o,"manifest"))).config;function s(e){return Pe.fetch_like(e,{method:"GET",credentials:"include",cache:"no-cache"})}Pe.config.applicationEnvironment&&(r.applicationEnvironment=Pe.config.applicationEnvironment),ve(Pe.config,r)}(e)),xe(),await we(null===(t=Pe.config.resources)||void 0===t?void 0:t.modulesAfterConfigLoaded),await be("onRuntimeConfigLoaded",[Pe.config]),e.onConfigLoaded)try{await e.onConfigLoaded(Pe.config,Le),xe()}catch(e){throw _("onConfigLoaded() failed",e),e}xe(),Pe.afterConfigLoaded.promise_control.resolve(Pe.config)}catch(t){const n=`Failed to load config file ${o} ${t} ${null==t?void 0:t.stack}`;throw Pe.config=e.config=Object.assign(Pe.config,{message:n,error:t,isError:!0}),Xe(1,new Error(n)),t}}function Te(){return!!globalThis.navigator&&(Pe.isChromium||Pe.isFirefox)}async function Ae(e){const t=Pe.config,o=await e.json();t.applicationEnvironment||o.applicationEnvironment||(o.applicationEnvironment=e.headers.get("Blazor-Environment")||e.headers.get("DotNet-Environment")||void 0),o.environmentVariables||(o.environmentVariables={});const n=e.headers.get("DOTNET-MODIFIABLE-ASSEMBLIES");n&&(o.environmentVariables.DOTNET_MODIFIABLE_ASSEMBLIES=n);const r=e.headers.get("ASPNETCORE-BROWSER-TOOLS");return r&&(o.environmentVariables.__ASPNETCORE_BROWSER_TOOLS=r),o}"function"!=typeof importScripts||globalThis.onmessage||(globalThis.dotnetSidecar=!0);const Se="object"==typeof process&&"object"==typeof process.versions&&"string"==typeof process.versions.node,De="function"==typeof importScripts,Oe=De&&"undefined"!=typeof dotnetSidecar,Ce=De&&!Oe,ke="object"==typeof window||De&&!Se,Ie=!ke&&!Se;let Ue={},Pe={},Me={},Le={},Ne={},$e=!1;const ze={},We={config:ze},Fe={mono:{},binding:{},internal:Ne,module:We,loaderHelpers:Pe,runtimeHelpers:Ue,diagnosticHelpers:Me,api:Le};function Be(e,t){if(e)return;const o="Assert failed: "+("function"==typeof t?t():t),n=new Error(o);_(o,n),Ue.nativeAbort(n)}function Ve(){return void 0!==Pe.exitCode}function qe(){return Ue.runtimeReady&&!Ve()}function He(){Ve()&&Be(!1,`.NET runtime already exited with ${Pe.exitCode} ${Pe.exitReason}. You can use runtime.runMain() which doesn't exit the runtime.`),Ue.runtimeReady||Be(!1,".NET runtime didn't start yet. Please call dotnet.create() first.")}function Je(){ke&&(globalThis.addEventListener("unhandledrejection",et),globalThis.addEventListener("error",tt))}let Ze,Qe;function Ge(e){Qe&&Qe(e),Xe(e,Pe.exitReason)}function Ke(e){Ze&&Ze(e||Pe.exitReason),Xe(1,e||Pe.exitReason)}function Xe(t,o){var n,r;const i=o&&"object"==typeof o;t=i&&"number"==typeof o.status?o.status:void 0===t?-1:t;const s=i&&"string"==typeof o.message?o.message:""+o;(o=i?o:Ue.ExitStatus?function(e,t){const o=new Ue.ExitStatus(e);return o.message=t,o.toString=()=>t,o}(t,s):new Error("Exit with code "+t+" "+s)).status=t,o.message||(o.message=s);const a=""+(o.stack||(new Error).stack);try{Object.defineProperty(o,"stack",{get:()=>a})}catch(e){}const l=!!o.silent;if(o.silent=!0,Ve())Pe.diagnosticTracing&&b("mono_exit called after exit");else{try{We.onAbort==Ke&&(We.onAbort=Ze),We.onExit==Ge&&(We.onExit=Qe),ke&&(globalThis.removeEventListener("unhandledrejection",et),globalThis.removeEventListener("error",tt)),Ue.runtimeReady?(Ue.jiterpreter_dump_stats&&Ue.jiterpreter_dump_stats(!1),0===t&&(null===(n=Pe.config)||void 0===n?void 0:n.interopCleanupOnExit)&&Ue.forceDisposeProxies(!0,!0),e&&0!==t&&(null===(r=Pe.config)||void 0===r||r.dumpThreadsOnNonZeroExit)):(Pe.diagnosticTracing&&b(`abort_startup, reason: ${o}`),function(e){Pe.allDownloadsQueued.promise_control.reject(e),Pe.allDownloadsFinished.promise_control.reject(e),Pe.afterConfigLoaded.promise_control.reject(e),Pe.wasmCompilePromise.promise_control.reject(e),Pe.runtimeModuleLoaded.promise_control.reject(e),Ue.dotnetReady&&(Ue.dotnetReady.promise_control.reject(e),Ue.afterInstantiateWasm.promise_control.reject(e),Ue.beforePreInit.promise_control.reject(e),Ue.afterPreInit.promise_control.reject(e),Ue.afterPreRun.promise_control.reject(e),Ue.beforeOnRuntimeInitialized.promise_control.reject(e),Ue.afterOnRuntimeInitialized.promise_control.reject(e),Ue.afterPostRun.promise_control.reject(e))}(o))}catch(e){E("mono_exit A failed",e)}try{l||(function(e,t){if(0!==e&&t){const e=Ue.ExitStatus&&t instanceof Ue.ExitStatus?b:_;"string"==typeof t?e(t):(void 0===t.stack&&(t.stack=(new Error).stack+""),t.message?e(Ue.stringify_as_error_with_stack?Ue.stringify_as_error_with_stack(t.message+"\n"+t.stack):t.message+"\n"+t.stack):e(JSON.stringify(t)))}!Ce&&Pe.config&&(Pe.config.logExitCode?Pe.config.forwardConsoleLogsToWS?R("WASM EXIT "+e):v("WASM EXIT "+e):Pe.config.forwardConsoleLogsToWS&&R())}(t,o),function(e){if(ke&&!Ce&&Pe.config&&Pe.config.appendElementOnExit&&document){const t=document.createElement("label");t.id="tests_done",0!==e&&(t.style.background="red"),t.innerHTML=""+e,document.body.appendChild(t)}}(t))}catch(e){E("mono_exit B failed",e)}Pe.exitCode=t,Pe.exitReason||(Pe.exitReason=o),!Ce&&Ue.runtimeReady&&We.runtimeKeepalivePop()}if(Pe.config&&Pe.config.asyncFlushOnExit&&0===t)throw(async()=>{try{await async function(){try{const e=await import(/*! webpackIgnore: true */"process"),t=e=>new Promise(((t,o)=>{e.on("error",o),e.end("","utf8",t)})),o=t(e.stderr),n=t(e.stdout);let r;const i=new Promise((e=>{r=setTimeout((()=>e("timeout")),1e3)}));await Promise.race([Promise.all([n,o]),i]),clearTimeout(r)}catch(e){_(`flushing std* streams failed: ${e}`)}}()}finally{Ye(t,o)}})(),o;Ye(t,o)}function Ye(e,t){if(Ue.runtimeReady&&Ue.nativeExit)try{Ue.nativeExit(e)}catch(e){!Ue.ExitStatus||e instanceof Ue.ExitStatus||E("set_exit_code_and_quit_now failed: "+e.toString())}if(0!==e||!ke)throw Se&&Ne.process?Ne.process.exit(e):Ue.quit&&Ue.quit(e,t),t}function et(e){ot(e,e.reason,"rejection")}function tt(e){ot(e,e.error,"error")}function ot(e,t,o){e.preventDefault();try{t||(t=new Error("Unhandled "+o)),void 0===t.stack&&(t.stack=(new Error).stack),t.stack=t.stack+"",t.silent||(_("Unhandled error:",t),Xe(1,t))}catch(e){}}!function(e){if($e)throw new Error("Loader module already loaded");$e=!0,Ue=e.runtimeHelpers,Pe=e.loaderHelpers,Me=e.diagnosticHelpers,Le=e.api,Ne=e.internal,Object.assign(Le,{INTERNAL:Ne,invokeLibraryInitializers:be}),Object.assign(e.module,{config:ve(ze,{environmentVariables:{}})});const r={mono_wasm_bindings_is_ready:!1,config:e.module.config,diagnosticTracing:!1,nativeAbort:e=>{throw e||new Error("abort")},nativeExit:e=>{throw new Error("exit:"+e)}},l={gitHash:"94ea82652cdd4e0f8046b5bd5becbd11461482ca",config:e.module.config,diagnosticTracing:!1,maxParallelDownloads:16,enableDownloadRetry:!0,_loaded_files:[],loadedFiles:[],loadedAssemblies:[],libraryInitializers:[],workerNextNumber:1,actual_downloaded_assets_count:0,actual_instantiated_assets_count:0,expected_downloaded_assets_count:0,expected_instantiated_assets_count:0,afterConfigLoaded:i(),allDownloadsQueued:i(),allDownloadsFinished:i(),wasmCompilePromise:i(),runtimeModuleLoaded:i(),loadingWorkers:i(),is_exited:Ve,is_runtime_running:qe,assert_runtime_running:He,mono_exit:Xe,createPromiseController:i,getPromiseController:s,assertIsControllablePromise:a,mono_download_assets:oe,resolve_single_asset_path:ee,setup_proxy_console:j,set_thread_prefix:w,installUnhandledErrorHandler:Je,retrieve_asset_download:ie,invokeLibraryInitializers:be,isDebuggingSupported:Te,exceptions:t,simd:n,relaxedSimd:o};Object.assign(Ue,r),Object.assign(Pe,l)}(Fe);let nt,rt,it,st=!1,at=!1;async function lt(e){if(!at){if(at=!0,ke&&Pe.config.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&j("main",globalThis.console,globalThis.location.origin),We||Be(!1,"Null moduleConfig"),Pe.config||Be(!1,"Null moduleConfig.config"),"function"==typeof e){const t=e(Fe.api);if(t.ready)throw new Error("Module.ready couldn't be redefined.");Object.assign(We,t),Ee(We,t)}else{if("object"!=typeof e)throw new Error("Can't use moduleFactory callback of createDotnetRuntime function.");Ee(We,e)}await async function(e){if(Se){const e=await import(/*! webpackIgnore: true */"process"),t=14;if(e.versions.node.split(".")[0]<t)throw new Error(`NodeJS at '${e.execPath}' has too low version '${e.versions.node}', please use at least ${t}. See also https://aka.ms/dotnet-wasm-features`)}const t=/*! webpackIgnore: true */import.meta.url,o=t.indexOf("?");var n;if(o>0&&(Pe.modulesUniqueQuery=t.substring(o)),Pe.scriptUrl=t.replace(/\\/g,"/").replace(/[?#].*/,""),Pe.scriptDirectory=(n=Pe.scriptUrl).slice(0,n.lastIndexOf("/"))+"/",Pe.locateFile=e=>"URL"in globalThis&&globalThis.URL!==C?new URL(e,Pe.scriptDirectory).toString():M(e)?e:Pe.scriptDirectory+e,Pe.fetch_like=k,Pe.out=console.log,Pe.err=console.error,Pe.onDownloadResourceProgress=e.onDownloadResourceProgress,ke&&globalThis.navigator){const e=globalThis.navigator,t=e.userAgentData&&e.userAgentData.brands;t&&t.length>0?Pe.isChromium=t.some((e=>"Google Chrome"===e.brand||"Microsoft Edge"===e.brand||"Chromium"===e.brand)):e.userAgent&&(Pe.isChromium=e.userAgent.includes("Chrome"),Pe.isFirefox=e.userAgent.includes("Firefox"))}Ne.require=Se?await import(/*! webpackIgnore: true */"module").then((e=>e.createRequire(/*! webpackIgnore: true */import.meta.url))):Promise.resolve((()=>{throw new Error("require not supported")})),void 0===globalThis.URL&&(globalThis.URL=C)}(We)}}async function ct(e){return await lt(e),Ze=We.onAbort,Qe=We.onExit,We.onAbort=Ke,We.onExit=Ge,We.ENVIRONMENT_IS_PTHREAD?async function(){(function(){const e=new MessageChannel,t=e.port1,o=e.port2;t.addEventListener("message",(e=>{var n,r;n=JSON.parse(e.data.config),r=JSON.parse(e.data.monoThreadInfo),st?Pe.diagnosticTracing&&b("mono config already received"):(ve(Pe.config,n),Ue.monoThreadInfo=r,xe(),Pe.diagnosticTracing&&b("mono config received"),st=!0,Pe.afterConfigLoaded.promise_control.resolve(Pe.config),ke&&n.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&Pe.setup_proxy_console("worker-idle",console,globalThis.location.origin)),t.close(),o.close()}),{once:!0}),t.start(),self.postMessage({[l]:{monoCmd:"preload",port:o}},[o])})(),await Pe.afterConfigLoaded.promise,function(){const e=Pe.config;e.assets||Be(!1,"config.assets must be defined");for(const t of e.assets)X(t),Q[t.behavior]&&z.push(t)}(),setTimeout((async()=>{try{await oe()}catch(e){Xe(1,e)}}),0);const e=dt(),t=await Promise.all(e);return await ut(t),We}():async function(){var e;await Re(We),re();const t=dt();(async function(){try{const e=ee("dotnetwasm");await se(e),e&&e.pendingDownloadInternal&&e.pendingDownloadInternal.response||Be(!1,"Can't load dotnet.native.wasm");const t=await e.pendingDownloadInternal.response,o=t.headers&&t.headers.get?t.headers.get("Content-Type"):void 0;let n;if("function"==typeof WebAssembly.compileStreaming&&"application/wasm"===o)n=await WebAssembly.compileStreaming(t);else{ke&&"application/wasm"!==o&&E('WebAssembly resource does not have the expected content type "application/wasm", so falling back to slower ArrayBuffer instantiation.');const e=await t.arrayBuffer();Pe.diagnosticTracing&&b("instantiate_wasm_module buffered"),n=Ie?await Promise.resolve(new WebAssembly.Module(e)):await WebAssembly.compile(e)}e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null,Pe.wasmCompilePromise.promise_control.resolve(n)}catch(e){Pe.wasmCompilePromise.promise_control.reject(e)}})(),setTimeout((async()=>{try{D(),await oe()}catch(e){Xe(1,e)}}),0);const o=await Promise.all(t);return await ut(o),await Ue.dotnetReady.promise,await we(null===(e=Pe.config.resources)||void 0===e?void 0:e.modulesAfterRuntimeReady),await be("onRuntimeReady",[Fe.api]),Le}()}function dt(){const e=ee("js-module-runtime"),t=ee("js-module-native");if(nt&&rt)return[nt,rt,it];"object"==typeof e.moduleExports?nt=e.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${e.resolvedUrl}' for ${e.name}`),nt=import(/*! webpackIgnore: true */e.resolvedUrl)),"object"==typeof t.moduleExports?rt=t.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${t.resolvedUrl}' for ${t.name}`),rt=import(/*! webpackIgnore: true */t.resolvedUrl));const o=Y("js-module-diagnostics");return o&&("object"==typeof o.moduleExports?it=o.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${o.resolvedUrl}' for ${o.name}`),it=import(/*! webpackIgnore: true */o.resolvedUrl))),[nt,rt,it]}async function ut(e){const{initializeExports:t,initializeReplacements:o,configureRuntimeStartup:n,configureEmscriptenStartup:r,configureWorkerStartup:i,setRuntimeGlobals:s,passEmscriptenInternals:a}=e[0],{default:l}=e[1],c=e[2];s(Fe),t(Fe),c&&c.setRuntimeGlobals(Fe),await n(We),Pe.runtimeModuleLoaded.promise_control.resolve(),l((e=>(Object.assign(We,{ready:e.ready,__dotnet_runtime:{initializeReplacements:o,configureEmscriptenStartup:r,configureWorkerStartup:i,passEmscriptenInternals:a}}),We))).catch((e=>{if(e.message&&e.message.toLowerCase().includes("out of memory"))throw new Error(".NET runtime has failed to start, because too much memory was requested. Please decrease the memory by adjusting EmccMaximumHeapSize. See also https://aka.ms/dotnet-wasm-features");throw e}))}const ft=new class{withModuleConfig(e){try{return Ee(We,e),this}catch(e){throw Xe(1,e),e}}withOnConfigLoaded(e){try{return Ee(We,{onConfigLoaded:e}),this}catch(e){throw Xe(1,e),e}}withConsoleForwarding(){try{return ve(ze,{forwardConsoleLogsToWS:!0}),this}catch(e){throw Xe(1,e),e}}withExitOnUnhandledError(){try{return ve(ze,{exitOnUnhandledError:!0}),Je(),this}catch(e){throw Xe(1,e),e}}withAsyncFlushOnExit(){try{return ve(ze,{asyncFlushOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withExitCodeLogging(){try{return ve(ze,{logExitCode:!0}),this}catch(e){throw Xe(1,e),e}}withElementOnExit(){try{return ve(ze,{appendElementOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withInteropCleanupOnExit(){try{return ve(ze,{interopCleanupOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withDumpThreadsOnNonZeroExit(){try{return ve(ze,{dumpThreadsOnNonZeroExit:!0}),this}catch(e){throw Xe(1,e),e}}withWaitingForDebugger(e){try{return ve(ze,{waitForDebugger:e}),this}catch(e){throw Xe(1,e),e}}withInterpreterPgo(e,t){try{return ve(ze,{interpreterPgo:e,interpreterPgoSaveDelay:t}),ze.runtimeOptions?ze.runtimeOptions.push("--interp-pgo-recording"):ze.runtimeOptions=["--interp-pgo-recording"],this}catch(e){throw Xe(1,e),e}}withConfig(e){try{return ve(ze,e),this}catch(e){throw Xe(1,e),e}}withConfigSrc(e){try{return e&&"string"==typeof e||Be(!1,"must be file path or URL"),Ee(We,{configSrc:e}),this}catch(e){throw Xe(1,e),e}}withVirtualWorkingDirectory(e){try{return e&&"string"==typeof e||Be(!1,"must be directory path"),ve(ze,{virtualWorkingDirectory:e}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariable(e,t){try{const o={};return o[e]=t,ve(ze,{environmentVariables:o}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariables(e){try{return e&&"object"==typeof e||Be(!1,"must be dictionary object"),ve(ze,{environmentVariables:e}),this}catch(e){throw Xe(1,e),e}}withDiagnosticTracing(e){try{return"boolean"!=typeof e&&Be(!1,"must be boolean"),ve(ze,{diagnosticTracing:e}),this}catch(e){throw Xe(1,e),e}}withDebugging(e){try{return null!=e&&"number"==typeof e||Be(!1,"must be number"),ve(ze,{debugLevel:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArguments(...e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ve(ze,{applicationArguments:e}),this}catch(e){throw Xe(1,e),e}}withRuntimeOptions(e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ze.runtimeOptions?ze.runtimeOptions.push(...e):ze.runtimeOptions=e,this}catch(e){throw Xe(1,e),e}}withMainAssembly(e){try{return ve(ze,{mainAssemblyName:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArgumentsFromQuery(){try{if(!globalThis.window)throw new Error("Missing window to the query parameters from");if(void 0===globalThis.URLSearchParams)throw new Error("URLSearchParams is supported");const e=new URLSearchParams(globalThis.window.location.search).getAll("arg");return this.withApplicationArguments(...e)}catch(e){throw Xe(1,e),e}}withApplicationEnvironment(e){try{return ve(ze,{applicationEnvironment:e}),this}catch(e){throw Xe(1,e),e}}withApplicationCulture(e){try{return ve(ze,{applicationCulture:e}),this}catch(e){throw Xe(1,e),e}}withResourceLoader(e){try{return Pe.loadBootResource=e,this}catch(e){throw Xe(1,e),e}}async download(){try{await async function(){lt(We),await Re(We),re(),D(),oe(),await Pe.allDownloadsFinished.promise}()}catch(e){throw Xe(1,e),e}}async create(){try{return this.instance||(this.instance=await async function(){return await ct(We),Fe.api}()),this.instance}catch(e){throw Xe(1,e),e}}async run(){try{return We.config||Be(!1,"Null moduleConfig.config"),this.instance||await this.create(),this.instance.runMainAndExit()}catch(e){throw Xe(1,e),e}}},mt=Xe,gt=ct;Ie||"function"==typeof globalThis.URL||Be(!1,"This browser/engine doesn't support URL API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),"function"!=typeof globalThis.BigInt64Array&&Be(!1,"This browser/engine doesn't support BigInt64Array API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),ft.withConfig(/*json-start*/{
  "mainAssemblyName": "ExpressiveSharp.Docs.Playground.Wasm",
  "resources": {
    "hash": "sha256-DpZfRslrb9NSXqDQJulVj5p8dN7qkTaiuyyjrFNTZ1M=",
    "jsModuleNative": [
      {
        "name": "dotnet.native.js"
      }
    ],
    "jsModuleRuntime": [
      {
        "name": "dotnet.runtime.js"
      }
    ],
    "wasmNative": [
      {
        "name": "dotnet.native.wasm",
        "hash": "sha256-i4MH1ttKidpkFY/9i4kRe+7ux5JQMZds+qXuSkJqeog="
      }
    ],
    "icu": [
      {
        "virtualPath": "icudt_CJK.dat",
        "name": "icudt_CJK.dat",
        "hash": "sha256-SZLtQnRc0JkwqHab0VUVP7T3uBPSeYzxzDnpxPpUnHk="
      },
      {
        "virtualPath": "icudt_EFIGS.dat",
        "name": "icudt_EFIGS.dat",
        "hash": "sha256-8fItetYY8kQ0ww6oxwTLiT3oXlBwHKumbeP2pRF4yTc="
      },
      {
        "virtualPath": "icudt_no_CJK.dat",
        "name": "icudt_no_CJK.dat",
        "hash": "sha256-L7sV7NEYP37/Qr2FPCePo5cJqRgTXRwGHuwF5Q+0Nfs="
      }
    ],
    "coreAssembly": [
      {
        "virtualPath": "System.Private.CoreLib.dll",
        "name": "System.Private.CoreLib.dll",
        "hash": "sha256-X6p6b8KIiX+OhR/EhhzuD80IXaVV7Btpg8hlYEGeEuU="
      },
      {
        "virtualPath": "System.Runtime.InteropServices.JavaScript.dll",
        "name": "System.Runtime.InteropServices.JavaScript.dll",
        "hash": "sha256-MPJTn/vD9SgXsf2LnB4F2U5+MBt4k6SbXvC5bGjWz7U="
      }
    ],
    "assembly": [
      {
        "virtualPath": "Basic.Reference.Assemblies.Net100.dll",
        "name": "Basic.Reference.Assemblies.Net100.dll",
        "hash": "sha256-eQCX70U4CYh302iVUHHMLaTyy/bYq0ydTQtdK6zpym8="
      },
      {
        "virtualPath": "ExpressiveSharp.Abstractions.dll",
        "name": "ExpressiveSharp.Abstractions.dll",
        "hash": "sha256-Gxxnu+rXKUWF/8L2TtoFmrzg+D9WhyneTMW+cQdZc+I="
      },
      {
        "virtualPath": "ExpressiveSharp.Docs.Playground.Core.dll",
        "name": "ExpressiveSharp.Docs.Playground.Core.dll",
        "hash": "sha256-Ktc3M2ZJIO9gjgeg8OfLVMsTKcmkIQFeqaLZRDWvht8="
      },
      {
        "virtualPath": "ExpressiveSharp.Docs.Playground.Wasm.dll",
        "name": "ExpressiveSharp.Docs.Playground.Wasm.dll",
        "hash": "sha256-zPiDOhPp1Ql1OiYQ9x3lYHggHdG4PP2SflbEEZA63iY="
      },
      {
        "virtualPath": "ExpressiveSharp.Docs.PlaygroundModel.dll",
        "name": "ExpressiveSharp.Docs.PlaygroundModel.dll",
        "hash": "sha256-9+bVji4/C+nGXDast3T8YEMg/rsyyXjiw7QkcCITnZQ="
      },
      {
        "virtualPath": "ExpressiveSharp.EntityFrameworkCore.dll",
        "name": "ExpressiveSharp.EntityFrameworkCore.dll",
        "hash": "sha256-T1TALoa30quVqcssICsxbb/Uz3+PZ8iKiIB4lJ22pb0="
      },
      {
        "virtualPath": "ExpressiveSharp.Generator.dll",
        "name": "ExpressiveSharp.Generator.dll",
        "hash": "sha256-ZjHJ5L332OAZdauMxIaQkQS4BjCdYM34/fUutyO+QfE="
      },
      {
        "virtualPath": "ExpressiveSharp.dll",
        "name": "ExpressiveSharp.dll",
        "hash": "sha256-TFXQ0rqjaUCm/KKuNlfKJ+ghZ196NyPHlLNpsGRJyHY="
      },
      {
        "virtualPath": "Humanizer.dll",
        "name": "Humanizer.dll",
        "hash": "sha256-1oUwdVR9fjA+/WA1TZEaL/GO26WCzaL6WdkaLl3Pnpg="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Authorization.dll",
        "name": "Microsoft.AspNetCore.Authorization.dll",
        "hash": "sha256-Q2XIN3CVipS4GddMRj2awuYtg7d5A1KzGB4OE0DFn0w="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.CustomElements.dll",
        "name": "Microsoft.AspNetCore.Components.CustomElements.dll",
        "hash": "sha256-s4h8btpQGaULPq03WjiFUipC+CkwdZnoS7LowyXK9Go="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.Forms.dll",
        "name": "Microsoft.AspNetCore.Components.Forms.dll",
        "hash": "sha256-stkBnmx+vIsI8Me3RZpzBcBCC3Rz3yJ8BPq8pq5tebw="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.Web.dll",
        "name": "Microsoft.AspNetCore.Components.Web.dll",
        "hash": "sha256-gw5JlgLgW2EfbZFYBcQsgcNGTQ9wvZt3HLZIOELJoxs="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.WebAssembly.dll",
        "name": "Microsoft.AspNetCore.Components.WebAssembly.dll",
        "hash": "sha256-Zh05kJQRAhAi1cItcOTdRaaBBfDIRqjiI7HHfovHYps="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.dll",
        "name": "Microsoft.AspNetCore.Components.dll",
        "hash": "sha256-FegMgFRt0V92h1K6WPYmiEZHohl/LhJM+xVyOR83raM="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Metadata.dll",
        "name": "Microsoft.AspNetCore.Metadata.dll",
        "hash": "sha256-Gbv+GTSFYPh8fHBx1EtL+hs0ppdtjcxqA4VZ3wNfU0o="
      },
      {
        "virtualPath": "Microsoft.CSharp.dll",
        "name": "Microsoft.CSharp.dll",
        "hash": "sha256-mR+mjVhOBR5RzuEIK/sP7FVo2RcIc9QjC6af0/JZn8k="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.dll",
        "name": "Microsoft.CodeAnalysis.CSharp.Features.dll",
        "hash": "sha256-s0DEXk+Iby/SVXodxongUQ9KNILWQ6s8pBBM6QarzPU="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.dll",
        "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.dll",
        "hash": "sha256-vzD7mq/NmcSnS51nM3dKvL+CxKg2xalPrm2RxFUj8IU="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.CSharp.dll",
        "name": "Microsoft.CodeAnalysis.CSharp.dll",
        "hash": "sha256-NvWsjaMKYQf1OiymUJth4oTAjrWaobIpVtTiAORRSqU="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Elfie.dll",
        "name": "Microsoft.CodeAnalysis.Elfie.dll",
        "hash": "sha256-jsDhisVwFXs4Y/cANbZEkl2JmWrZAfMe+RjSsPdCiUE="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Features.dll",
        "name": "Microsoft.CodeAnalysis.Features.dll",
        "hash": "sha256-YLidM43TlVYOG1FISqrZY2qvTTsxSR73pQqWDRrXNHA="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Scripting.dll",
        "name": "Microsoft.CodeAnalysis.Scripting.dll",
        "hash": "sha256-q+by3PLR0uC5CU3aODQq6i0zB9AyBUs1ZF0RIRfEB2Y="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Workspaces.UnitTests.dll",
        "name": "Microsoft.CodeAnalysis.Workspaces.UnitTests.dll",
        "hash": "sha256-pQTix8BdUE/6dd5LiwNUYJYRb/xMPvoI4xBUauPEgeo="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.Workspaces.dll",
        "name": "Microsoft.CodeAnalysis.Workspaces.dll",
        "hash": "sha256-eUmAVWNVclkACaR4Gm4+4eLnDjCFsuo84WGCD4CbCys="
      },
      {
        "virtualPath": "Microsoft.CodeAnalysis.dll",
        "name": "Microsoft.CodeAnalysis.dll",
        "hash": "sha256-EPSJ22e4rHSJ5Y05IWbJKDArpWmFBt1lIxHaXYnwoPg="
      },
      {
        "virtualPath": "Microsoft.Data.Sqlite.dll",
        "name": "Microsoft.Data.Sqlite.dll",
        "hash": "sha256-7KvsKSK1rowqLgN9bzEnXRjYffMwd1UQCPmtfDA3mxw="
      },
      {
        "virtualPath": "Microsoft.DiaSymReader.dll",
        "name": "Microsoft.DiaSymReader.dll",
        "hash": "sha256-fz51QzIf9MJnW5TlakoyyTXBHIL8xg1PbLYRywqNqtw="
      },
      {
        "virtualPath": "Microsoft.EntityFrameworkCore.Abstractions.dll",
        "name": "Microsoft.EntityFrameworkCore.Abstractions.dll",
        "hash": "sha256-zlfPUXa8twFkTsSmb920y3SIWjMHZjoA1X0+gm9cUGo="
      },
      {
        "virtualPath": "Microsoft.EntityFrameworkCore.Relational.dll",
        "name": "Microsoft.EntityFrameworkCore.Relational.dll",
        "hash": "sha256-Zy9yGqGgRrqndujlWNv6X+tpNvGFaHYRAHEpyLhafWo="
      },
      {
        "virtualPath": "Microsoft.EntityFrameworkCore.Sqlite.dll",
        "name": "Microsoft.EntityFrameworkCore.Sqlite.dll",
        "hash": "sha256-9+CnxALTBnsRE7qQ+4F4V9I+I07TDphphbRLQ8KRpBs="
      },
      {
        "virtualPath": "Microsoft.EntityFrameworkCore.dll",
        "name": "Microsoft.EntityFrameworkCore.dll",
        "hash": "sha256-2eKCRIX9BxiDWbLDJHCHxyC4i1s93lOoFN6c3EPYduQ="
      },
      {
        "virtualPath": "Microsoft.Extensions.Caching.Abstractions.dll",
        "name": "Microsoft.Extensions.Caching.Abstractions.dll",
        "hash": "sha256-INJOtKKgAsqgYPZFsOUWMTAaO3gwakMt0WJhk9lyLic="
      },
      {
        "virtualPath": "Microsoft.Extensions.Caching.Memory.dll",
        "name": "Microsoft.Extensions.Caching.Memory.dll",
        "hash": "sha256-P+iJw82GX/IsrxEeJaC6nag2MVes9+gndTepoU11PuA="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Abstractions.dll",
        "name": "Microsoft.Extensions.Configuration.Abstractions.dll",
        "hash": "sha256-5yuHVBKmE9AI+SRqddXrcXbcfvdaovIoygUiHUVZZm4="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Binder.dll",
        "name": "Microsoft.Extensions.Configuration.Binder.dll",
        "hash": "sha256-mdo8B0xm4WvMN5ifOgrzBolbErpZT4qaC6PbSKCCd9Q="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.FileExtensions.dll",
        "name": "Microsoft.Extensions.Configuration.FileExtensions.dll",
        "hash": "sha256-bBbuF4fMcKlXyOs3KjUhfLGfOmlRARN6icdvUvadvYc="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Json.dll",
        "name": "Microsoft.Extensions.Configuration.Json.dll",
        "hash": "sha256-sz7MhDs2/319OWCYTlMgpb1cjqPxyjJ3Iz8xRYJuojc="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.dll",
        "name": "Microsoft.Extensions.Configuration.dll",
        "hash": "sha256-4sgiDeI3yRPgvEpAxaqAA4l1d0vclAypqqhvvMGxNgM="
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
        "name": "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
        "hash": "sha256-hC/BmAKnYeScZbDpCyxovGhouYkUXaWsbKMLdm4mWPI="
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.dll",
        "name": "Microsoft.Extensions.DependencyInjection.dll",
        "hash": "sha256-w7lv+tEC8b/Hm5zxw623pinWwfN1X5YGBQ7CrVWiLwY="
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyModel.dll",
        "name": "Microsoft.Extensions.DependencyModel.dll",
        "hash": "sha256-ypXxqkmtbdR3FNvfv9DTTsCYOxRxoMQNSFtwFwWBO6c="
      },
      {
        "virtualPath": "Microsoft.Extensions.Diagnostics.Abstractions.dll",
        "name": "Microsoft.Extensions.Diagnostics.Abstractions.dll",
        "hash": "sha256-RAY+T0vpA6FSqm4yc+hrHmRTYckt4qKtVZvicM8MKMM="
      },
      {
        "virtualPath": "Microsoft.Extensions.Diagnostics.dll",
        "name": "Microsoft.Extensions.Diagnostics.dll",
        "hash": "sha256-RWrTIRSILAHwvOgi59KfMDYpoZb0ZSabEYOWSB1wNjQ="
      },
      {
        "virtualPath": "Microsoft.Extensions.FileProviders.Abstractions.dll",
        "name": "Microsoft.Extensions.FileProviders.Abstractions.dll",
        "hash": "sha256-4bYO7fObvHVZxgmIFMNTrwjX6TzOF192c8YZU1JGd+I="
      },
      {
        "virtualPath": "Microsoft.Extensions.FileProviders.Physical.dll",
        "name": "Microsoft.Extensions.FileProviders.Physical.dll",
        "hash": "sha256-cHWFfT2Sm3JDkX9jyTuZIDPfZudhdvtZddyAvJhXAak="
      },
      {
        "virtualPath": "Microsoft.Extensions.FileSystemGlobbing.dll",
        "name": "Microsoft.Extensions.FileSystemGlobbing.dll",
        "hash": "sha256-0mBuTsjYZvUZZbl6UvN3bl4TpfeJVjw6ht1h0jOYRxU="
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Abstractions.dll",
        "name": "Microsoft.Extensions.Logging.Abstractions.dll",
        "hash": "sha256-A+kWucX5Fxe2fHkDJdTl3S3r6B2OQDkogGuh1DDqcUc="
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.dll",
        "name": "Microsoft.Extensions.Logging.dll",
        "hash": "sha256-YH6R9XUCaE410cdm6Ekm8uT501z7QXPAdk/m64qKaxE="
      },
      {
        "virtualPath": "Microsoft.Extensions.Options.ConfigurationExtensions.dll",
        "name": "Microsoft.Extensions.Options.ConfigurationExtensions.dll",
        "hash": "sha256-rLaU/5LGawAewBnuBaGoiTnv2PBF6EK4rg6LhJDVQD4="
      },
      {
        "virtualPath": "Microsoft.Extensions.Options.dll",
        "name": "Microsoft.Extensions.Options.dll",
        "hash": "sha256-b3pLwr5rNaHyjNYMJQ5UcarDHLKZXqfsc2XqXm+AZA4="
      },
      {
        "virtualPath": "Microsoft.Extensions.Primitives.dll",
        "name": "Microsoft.Extensions.Primitives.dll",
        "hash": "sha256-5DZz/wvHYh+JPbHYAhKOQkLMxssMu+H6e114CQYoUU8="
      },
      {
        "virtualPath": "Microsoft.Extensions.Validation.dll",
        "name": "Microsoft.Extensions.Validation.dll",
        "hash": "sha256-yPgm1SypfnsO5o7q9YUkTlDISGJbUUGupMIKJ31Ukf0="
      },
      {
        "virtualPath": "Microsoft.JSInterop.WebAssembly.dll",
        "name": "Microsoft.JSInterop.WebAssembly.dll",
        "hash": "sha256-BZ2IY9ugTIj3Y7NXF4klgUgeOIITRspLgGViHE0vxFY="
      },
      {
        "virtualPath": "Microsoft.JSInterop.dll",
        "name": "Microsoft.JSInterop.dll",
        "hash": "sha256-okiz21Yf0KTsZtFc506e5Fy9HHJp+NFk2VZRgbZtXfA="
      },
      {
        "virtualPath": "Microsoft.VisualBasic.Core.dll",
        "name": "Microsoft.VisualBasic.Core.dll",
        "hash": "sha256-9v8AgXyHQ6iuyT1c9LY7dNB+B+plaSuxBhcL9pbZ1a0="
      },
      {
        "virtualPath": "Microsoft.VisualBasic.dll",
        "name": "Microsoft.VisualBasic.dll",
        "hash": "sha256-jqPKW0cNT3E+yrGTLRWRzuaJF40Bl6WUk2ToZCMPsnc="
      },
      {
        "virtualPath": "Microsoft.Win32.Primitives.dll",
        "name": "Microsoft.Win32.Primitives.dll",
        "hash": "sha256-05PLSq1EkzYub8zHbpJhyR25RLOo4pyS5mpYzDhY9ls="
      },
      {
        "virtualPath": "Microsoft.Win32.Registry.dll",
        "name": "Microsoft.Win32.Registry.dll",
        "hash": "sha256-ZGi0A05pW74w2+mKhdWTxPHlChT7qyDu4Aa3HRA6EiY="
      },
      {
        "virtualPath": "SQLitePCLRaw.core.dll",
        "name": "SQLitePCLRaw.core.dll",
        "hash": "sha256-PS7Y4YbxJPmI69tF0DVBhbQkNXviQzu6ADOrnsMb0ls="
      },
      {
        "virtualPath": "System.AppContext.dll",
        "name": "System.AppContext.dll",
        "hash": "sha256-trMwiBtmzpbJ9TifYSomlAC/9jgO4BnK3qsKESSGluw="
      },
      {
        "virtualPath": "System.Buffers.dll",
        "name": "System.Buffers.dll",
        "hash": "sha256-hLR0hRYvUeZ8rK0E8v73H95wcbEvLpeVtiv/VhGCuW0="
      },
      {
        "virtualPath": "System.Collections.Concurrent.dll",
        "name": "System.Collections.Concurrent.dll",
        "hash": "sha256-iWWm5qRHuTIqXPse9FY4udD5ZgruWhJqMVpmANPGQ94="
      },
      {
        "virtualPath": "System.Collections.Immutable.dll",
        "name": "System.Collections.Immutable.dll",
        "hash": "sha256-0iguf+hhUCDwA7kJi0XOSHvQM38N7rsDMIpi9LuOmtM="
      },
      {
        "virtualPath": "System.Collections.NonGeneric.dll",
        "name": "System.Collections.NonGeneric.dll",
        "hash": "sha256-BDSdhNCQyxHifR3SSb1T+bzdjzfrIaaXXBiu8Zgao+c="
      },
      {
        "virtualPath": "System.Collections.Specialized.dll",
        "name": "System.Collections.Specialized.dll",
        "hash": "sha256-7fMfhguLcugekQfVxgUTn3tFZFu76s36pipYqyf7u9k="
      },
      {
        "virtualPath": "System.Collections.dll",
        "name": "System.Collections.dll",
        "hash": "sha256-RAcDmrh/Lt6VaZFHzynBlrC8jdGKZfoiH2Kq6d8WLbA="
      },
      {
        "virtualPath": "System.ComponentModel.Annotations.dll",
        "name": "System.ComponentModel.Annotations.dll",
        "hash": "sha256-9ePIuuEhGbmun7+mDOEfXICXET0ecSBXRDtK2W3baAM="
      },
      {
        "virtualPath": "System.ComponentModel.DataAnnotations.dll",
        "name": "System.ComponentModel.DataAnnotations.dll",
        "hash": "sha256-U70F7kmsp6gZXObLb5YlBFHublEsyBXHRt5Xm0eUcGE="
      },
      {
        "virtualPath": "System.ComponentModel.EventBasedAsync.dll",
        "name": "System.ComponentModel.EventBasedAsync.dll",
        "hash": "sha256-iWHcPxlbwVd7qSIpXbsbGGoXXH9V3va8LSveY1fGqIw="
      },
      {
        "virtualPath": "System.ComponentModel.Primitives.dll",
        "name": "System.ComponentModel.Primitives.dll",
        "hash": "sha256-s0UUEzOitbPTRpkKGYWAh286O9RNhchkSeMSI21RQj8="
      },
      {
        "virtualPath": "System.ComponentModel.TypeConverter.dll",
        "name": "System.ComponentModel.TypeConverter.dll",
        "hash": "sha256-T1rHpuvxlsBpM8DyOUUw+ZI1xgG0tsIASxBhdMzhgEU="
      },
      {
        "virtualPath": "System.ComponentModel.dll",
        "name": "System.ComponentModel.dll",
        "hash": "sha256-foBtTxFP3ztQX4ipWeUtCPs8y7iWBSUbRNzMRVERiD8="
      },
      {
        "virtualPath": "System.Composition.AttributedModel.dll",
        "name": "System.Composition.AttributedModel.dll",
        "hash": "sha256-1PHshedCt3zaSPcJIQ2q802LSn4eVA81Pv90HcwFgSg="
      },
      {
        "virtualPath": "System.Composition.Convention.dll",
        "name": "System.Composition.Convention.dll",
        "hash": "sha256-vpX0gESasyNjFb6zIc5ZimE4LWRHnd9QDnQ0Y3NF9HQ="
      },
      {
        "virtualPath": "System.Composition.Hosting.dll",
        "name": "System.Composition.Hosting.dll",
        "hash": "sha256-LI8Qkjw7YP0bFtifJmFxKpvhEuaXmQvtvW+rnbqrsbY="
      },
      {
        "virtualPath": "System.Composition.Runtime.dll",
        "name": "System.Composition.Runtime.dll",
        "hash": "sha256-FIAS233a/HbSi76qC66j4jUee8GV6QMdV/EHmJwuDqo="
      },
      {
        "virtualPath": "System.Composition.TypedParts.dll",
        "name": "System.Composition.TypedParts.dll",
        "hash": "sha256-68lCw1jZl+QWNcmjZdtUKj/SoF8wH2AtXWqLDV3UIHE="
      },
      {
        "virtualPath": "System.Configuration.ConfigurationManager.dll",
        "name": "System.Configuration.ConfigurationManager.dll",
        "hash": "sha256-33lQJfIHtJQOyaSN7drtqzMzsDGxUwP9pPFIsJxFbW0="
      },
      {
        "virtualPath": "System.Configuration.dll",
        "name": "System.Configuration.dll",
        "hash": "sha256-RH4dEU9XEwAF9yZnzR2c/SORuN9aSRtXjIGvc+pZg8s="
      },
      {
        "virtualPath": "System.Console.dll",
        "name": "System.Console.dll",
        "hash": "sha256-FS2TQhN9W3hblb3PHRxX787tRRHaXoyATrp56ogYlr8="
      },
      {
        "virtualPath": "System.Core.dll",
        "name": "System.Core.dll",
        "hash": "sha256-1DvUSkitAeNl/o6v2y/G72xzBzPSRLgcXKqNK0ciVdA="
      },
      {
        "virtualPath": "System.Data.Common.dll",
        "name": "System.Data.Common.dll",
        "hash": "sha256-fdVRb3iQ7Tnq/O8ofbD75a27L4Lqp8aDmTUwrz1xamk="
      },
      {
        "virtualPath": "System.Data.DataSetExtensions.dll",
        "name": "System.Data.DataSetExtensions.dll",
        "hash": "sha256-Lepu6lhXOWy0sSO07LRs3BUyl3GKz4K6ucOwQ2XgeNI="
      },
      {
        "virtualPath": "System.Data.dll",
        "name": "System.Data.dll",
        "hash": "sha256-OCkePAkU6y/1gN1aau9Gj+1HG+ecq8zwqxNhicsEjU4="
      },
      {
        "virtualPath": "System.Diagnostics.Contracts.dll",
        "name": "System.Diagnostics.Contracts.dll",
        "hash": "sha256-6mRuSvSIdEYVEc8HlAVHbLlxkzCbckWV3MFlNnxkN7o="
      },
      {
        "virtualPath": "System.Diagnostics.Debug.dll",
        "name": "System.Diagnostics.Debug.dll",
        "hash": "sha256-UAmojnpzbzc4dWuY8DyfZQ6hyy241faDiD+Adau9kZM="
      },
      {
        "virtualPath": "System.Diagnostics.DiagnosticSource.dll",
        "name": "System.Diagnostics.DiagnosticSource.dll",
        "hash": "sha256-38qDNkuynYO0EpeaIM0hzjCQiCDJOa5aqVP5fXL4YQA="
      },
      {
        "virtualPath": "System.Diagnostics.EventLog.dll",
        "name": "System.Diagnostics.EventLog.dll",
        "hash": "sha256-gDukoL1BtjtkJI/ujJbM/OzecqFtxTjQD+H9HbdRcpU="
      },
      {
        "virtualPath": "System.Diagnostics.FileVersionInfo.dll",
        "name": "System.Diagnostics.FileVersionInfo.dll",
        "hash": "sha256-ZQ7PbuSIi9CieAJ4Cb6S0Hp7GCvItaevtDogvzzQFPs="
      },
      {
        "virtualPath": "System.Diagnostics.Process.dll",
        "name": "System.Diagnostics.Process.dll",
        "hash": "sha256-0BGFpFZjqeBNuEMhJiUPwuHnnGtTvgCtkFCnE6W0z6A="
      },
      {
        "virtualPath": "System.Diagnostics.StackTrace.dll",
        "name": "System.Diagnostics.StackTrace.dll",
        "hash": "sha256-azwR79hxbBpKZoXvH+JQla/hU9hGAcjUihIoloCRJO4="
      },
      {
        "virtualPath": "System.Diagnostics.TextWriterTraceListener.dll",
        "name": "System.Diagnostics.TextWriterTraceListener.dll",
        "hash": "sha256-C/7RDbbznxTjZymEpx2qsFG3rEW/8xdIRuDPL82LduY="
      },
      {
        "virtualPath": "System.Diagnostics.Tools.dll",
        "name": "System.Diagnostics.Tools.dll",
        "hash": "sha256-zuujrrTXxHpvCpvTQLY2APGqLOXDFBU54d1UMf7d1NY="
      },
      {
        "virtualPath": "System.Diagnostics.TraceSource.dll",
        "name": "System.Diagnostics.TraceSource.dll",
        "hash": "sha256-ejYXZHs+mHPQUls/xAlZ65JPR5XWIs0LR2+eDR8YbuM="
      },
      {
        "virtualPath": "System.Diagnostics.Tracing.dll",
        "name": "System.Diagnostics.Tracing.dll",
        "hash": "sha256-qGV+dwIvS8WFjp6NQf/bOFcLUUDHwCK0bWB2BvK28YQ="
      },
      {
        "virtualPath": "System.Drawing.Primitives.dll",
        "name": "System.Drawing.Primitives.dll",
        "hash": "sha256-iYTsRfk0AkFkSgGHvKmb6rnwYrPC3GcleJ/KLab/sgw="
      },
      {
        "virtualPath": "System.Drawing.dll",
        "name": "System.Drawing.dll",
        "hash": "sha256-AMRNi9Ah0D3ey3Oll2NcYgSyUiPF3uQmE0zIDkjHeek="
      },
      {
        "virtualPath": "System.Dynamic.Runtime.dll",
        "name": "System.Dynamic.Runtime.dll",
        "hash": "sha256-sg9g9e8268ql78Hd7T1EhG0FFwbQEw2oJnLkN30olyo="
      },
      {
        "virtualPath": "System.Formats.Asn1.dll",
        "name": "System.Formats.Asn1.dll",
        "hash": "sha256-/zlalLT5cpKQtp1I8YtQxK8BQLMxomZEFd6uE8/JAsQ="
      },
      {
        "virtualPath": "System.Formats.Tar.dll",
        "name": "System.Formats.Tar.dll",
        "hash": "sha256-7MXNAezYwuz5TsqKxu7eJ8s4pAfTQuCldhZpjUJPYTk="
      },
      {
        "virtualPath": "System.Globalization.Calendars.dll",
        "name": "System.Globalization.Calendars.dll",
        "hash": "sha256-3tRS/pkkal2y5Y3gr/TOxjkcxnUjAA3ijDorlnydhXI="
      },
      {
        "virtualPath": "System.Globalization.Extensions.dll",
        "name": "System.Globalization.Extensions.dll",
        "hash": "sha256-ipOejJH//LZQCC0SVVRqrnxMT1tXUML94iXn+99fcOw="
      },
      {
        "virtualPath": "System.Globalization.dll",
        "name": "System.Globalization.dll",
        "hash": "sha256-qzZya7RrDA8ttKSU8wtc9SvgmFVW3NuIGKE+59vLFJ8="
      },
      {
        "virtualPath": "System.IO.Compression.Brotli.dll",
        "name": "System.IO.Compression.Brotli.dll",
        "hash": "sha256-RYDHhdX3ggEb9smSzaK3n6LH7DBEUkFi/dW5bIEoUak="
      },
      {
        "virtualPath": "System.IO.Compression.FileSystem.dll",
        "name": "System.IO.Compression.FileSystem.dll",
        "hash": "sha256-SJ10YtaInVtx7V4wExDLyQgGA+3KYH2JFYGx/AVyISw="
      },
      {
        "virtualPath": "System.IO.Compression.ZipFile.dll",
        "name": "System.IO.Compression.ZipFile.dll",
        "hash": "sha256-Jyu9XN5TBvH1ACufGK3J1kSvJe2RabazjJ07QVwaAk0="
      },
      {
        "virtualPath": "System.IO.Compression.dll",
        "name": "System.IO.Compression.dll",
        "hash": "sha256-97jun9i/PSwnmBKXVT9DPAZeRmk8uJ9A3JlayfQICSg="
      },
      {
        "virtualPath": "System.IO.FileSystem.AccessControl.dll",
        "name": "System.IO.FileSystem.AccessControl.dll",
        "hash": "sha256-NF5K/M3bvo5X8PATiAiN+HzPmckcRhOUquCx33n40gE="
      },
      {
        "virtualPath": "System.IO.FileSystem.DriveInfo.dll",
        "name": "System.IO.FileSystem.DriveInfo.dll",
        "hash": "sha256-JY/gtrlIfthX1X7mG2sLjUNzzQLDRtV+zq0QD4hqP1c="
      },
      {
        "virtualPath": "System.IO.FileSystem.Primitives.dll",
        "name": "System.IO.FileSystem.Primitives.dll",
        "hash": "sha256-X5yDcQmDQRx2A+ZinV84Rj+ir+ofXwlEVMnqnuqXv4g="
      },
      {
        "virtualPath": "System.IO.FileSystem.Watcher.dll",
        "name": "System.IO.FileSystem.Watcher.dll",
        "hash": "sha256-HlOjN5SFM574G+ng1mKkILmEdP/ezYq4EBF0/TYtAPY="
      },
      {
        "virtualPath": "System.IO.FileSystem.dll",
        "name": "System.IO.FileSystem.dll",
        "hash": "sha256-Oyt/6K9Cuu69KYALthvgEM4ESin0drs+e8cPFITu4pw="
      },
      {
        "virtualPath": "System.IO.IsolatedStorage.dll",
        "name": "System.IO.IsolatedStorage.dll",
        "hash": "sha256-N1mJAywf/RcBKv7axDvCqxrRZ8sKPR0R6SxRhYMzcl0="
      },
      {
        "virtualPath": "System.IO.MemoryMappedFiles.dll",
        "name": "System.IO.MemoryMappedFiles.dll",
        "hash": "sha256-Uf+ZGG6SUmn0kwMWoCLt2bxNPKb9rPcp7Ca3UMlwBS8="
      },
      {
        "virtualPath": "System.IO.Pipelines.dll",
        "name": "System.IO.Pipelines.dll",
        "hash": "sha256-+NN8QbxcslmWTqq5Q6OR/mExc0blS2+MM9t0jowYnjY="
      },
      {
        "virtualPath": "System.IO.Pipes.AccessControl.dll",
        "name": "System.IO.Pipes.AccessControl.dll",
        "hash": "sha256-gKtePfGz2n6HRYNqRxjaKR0Dqsu7R7Gc1XslUD1DnP0="
      },
      {
        "virtualPath": "System.IO.Pipes.dll",
        "name": "System.IO.Pipes.dll",
        "hash": "sha256-6TEMTzFOcewvGsHnC0Zy1DdnRDzPo22ohUdbDnKTxn8="
      },
      {
        "virtualPath": "System.IO.UnmanagedMemoryStream.dll",
        "name": "System.IO.UnmanagedMemoryStream.dll",
        "hash": "sha256-jT9qXj16F4BSPUkSiPAWNO5MO1XpjPfX5UWU1KK6y/c="
      },
      {
        "virtualPath": "System.IO.dll",
        "name": "System.IO.dll",
        "hash": "sha256-usQynjKB82p7ctPK5Qf9rPZK8SUNicBxt1QWSl9VuTw="
      },
      {
        "virtualPath": "System.Linq.AsyncEnumerable.dll",
        "name": "System.Linq.AsyncEnumerable.dll",
        "hash": "sha256-jr9jzDHUcC6Ak+9p5Cz3KYjN4RZddZ0QepwfNzS9Q7A="
      },
      {
        "virtualPath": "System.Linq.Expressions.dll",
        "name": "System.Linq.Expressions.dll",
        "hash": "sha256-Y+9d2rgrDxF7gdfQ0VCZ4SbzpBZbwQ2JaF6F1JIh+1E="
      },
      {
        "virtualPath": "System.Linq.Parallel.dll",
        "name": "System.Linq.Parallel.dll",
        "hash": "sha256-/wkk/mxpFZSleAYras39KpHjd54ZYHUGktpyjMJVwZQ="
      },
      {
        "virtualPath": "System.Linq.Queryable.dll",
        "name": "System.Linq.Queryable.dll",
        "hash": "sha256-bYIpbXGtyNEnxzHL/fLEti/RS/DER0u/RmkUDphOO/s="
      },
      {
        "virtualPath": "System.Linq.dll",
        "name": "System.Linq.dll",
        "hash": "sha256-/IDrAAx6+ldhEtvIq9zVpzC65A+5gcqAp4v3LjiIRcU="
      },
      {
        "virtualPath": "System.Memory.dll",
        "name": "System.Memory.dll",
        "hash": "sha256-1wxpq5Z1ec7crmqJjbxxUFdToXF09inRpMz6LProLbk="
      },
      {
        "virtualPath": "System.Net.Http.Json.dll",
        "name": "System.Net.Http.Json.dll",
        "hash": "sha256-ixyULS8gsFM1vZ1tEucJhfRbTjBXLHnsnYiQGSRM3Gs="
      },
      {
        "virtualPath": "System.Net.Http.dll",
        "name": "System.Net.Http.dll",
        "hash": "sha256-f7fYZPSRMMQGQJBv0b38cHlXF7EbUdWRMMEa8kSmIC4="
      },
      {
        "virtualPath": "System.Net.HttpListener.dll",
        "name": "System.Net.HttpListener.dll",
        "hash": "sha256-Xwq0bgZ49MYXD84ieP0psTyfW3IbB+O7xvazMfM6wjQ="
      },
      {
        "virtualPath": "System.Net.Mail.dll",
        "name": "System.Net.Mail.dll",
        "hash": "sha256-NJXZaQBZW1bp7jbnpUedm1iuqDpL/tUSdbTm3CUAcsM="
      },
      {
        "virtualPath": "System.Net.NameResolution.dll",
        "name": "System.Net.NameResolution.dll",
        "hash": "sha256-XCSpn5buJqYt6Odv7lBbZ9Aa9A+pLoNg8XlKeOInl2Y="
      },
      {
        "virtualPath": "System.Net.NetworkInformation.dll",
        "name": "System.Net.NetworkInformation.dll",
        "hash": "sha256-nOS0FxQk5Bm1iRF4pwgqfY/ARUFsSjslbrdrNTicAe8="
      },
      {
        "virtualPath": "System.Net.Ping.dll",
        "name": "System.Net.Ping.dll",
        "hash": "sha256-kcLD+8D+1tP7GoHClBrq9jQNRAvHMnbpfZGixswCmmg="
      },
      {
        "virtualPath": "System.Net.Primitives.dll",
        "name": "System.Net.Primitives.dll",
        "hash": "sha256-eanq96aw6U3PYAElOeNPnQU5j+y8+A05tTHeqQPW4Io="
      },
      {
        "virtualPath": "System.Net.Quic.dll",
        "name": "System.Net.Quic.dll",
        "hash": "sha256-gHAqCLvr9tmcidF6iPvBkT5KhmL/FluFKNKlUlJ0fDI="
      },
      {
        "virtualPath": "System.Net.Requests.dll",
        "name": "System.Net.Requests.dll",
        "hash": "sha256-R5Rn8jzCH0E9+gIndFnqM0nn6F5C/mqm4e1ZXCnT1hs="
      },
      {
        "virtualPath": "System.Net.Security.dll",
        "name": "System.Net.Security.dll",
        "hash": "sha256-0+7h+gT0bsZX65RjI+Mkw3Fs3Pq7zuJEWHHmjVvJXXM="
      },
      {
        "virtualPath": "System.Net.ServerSentEvents.dll",
        "name": "System.Net.ServerSentEvents.dll",
        "hash": "sha256-30yz0l7Eq5H12804piZnCjD+fMjNKQ6MEpp3/aMxLRs="
      },
      {
        "virtualPath": "System.Net.ServicePoint.dll",
        "name": "System.Net.ServicePoint.dll",
        "hash": "sha256-R4poJW485GQtP3Bc6PmLCiz6zMCaqPI12pAQUWzKT/o="
      },
      {
        "virtualPath": "System.Net.Sockets.dll",
        "name": "System.Net.Sockets.dll",
        "hash": "sha256-winJh664GLEkqircaI5ThZHuchsbPu89aeu8ESxQQb0="
      },
      {
        "virtualPath": "System.Net.WebClient.dll",
        "name": "System.Net.WebClient.dll",
        "hash": "sha256-n+fUhJcJgVNc5BRLR4CXLW6OcTtA/nld7XimYs/aiK4="
      },
      {
        "virtualPath": "System.Net.WebHeaderCollection.dll",
        "name": "System.Net.WebHeaderCollection.dll",
        "hash": "sha256-eqga+VnNnvGmSIjCjCI/ygsg5PmzDaRTUFWxzbuBgko="
      },
      {
        "virtualPath": "System.Net.WebProxy.dll",
        "name": "System.Net.WebProxy.dll",
        "hash": "sha256-Mzy65ose3s2aOxMJzGavZkH5iYbkPRGJIZESJjj61OI="
      },
      {
        "virtualPath": "System.Net.WebSockets.Client.dll",
        "name": "System.Net.WebSockets.Client.dll",
        "hash": "sha256-R/pBqXl9grx7U01VLm6jaJaf0Ntag+ko7NW4oFg4ab4="
      },
      {
        "virtualPath": "System.Net.WebSockets.dll",
        "name": "System.Net.WebSockets.dll",
        "hash": "sha256-XD7Euh/Hy+Xjegy5hIbC8YuK0oUTuNUNb3DesPO8LmU="
      },
      {
        "virtualPath": "System.Net.dll",
        "name": "System.Net.dll",
        "hash": "sha256-Y7Rntc0PkeOxBeF/XkLUREx4nINBoP8mjmBJAfdyVzs="
      },
      {
        "virtualPath": "System.Numerics.Vectors.dll",
        "name": "System.Numerics.Vectors.dll",
        "hash": "sha256-1zkItzYr13OdRQqesjaEQUbORkVrcEyVJsv+30IsQDs="
      },
      {
        "virtualPath": "System.Numerics.dll",
        "name": "System.Numerics.dll",
        "hash": "sha256-rVLYVXfh7dl9U1SB9ft6R6gw4toFx/EtQx2LLdXkKlM="
      },
      {
        "virtualPath": "System.ObjectModel.dll",
        "name": "System.ObjectModel.dll",
        "hash": "sha256-pY/qKyaD438oUj1HJjpvfVHw2UlenkGLO7UmAXiqHZU="
      },
      {
        "virtualPath": "System.Private.DataContractSerialization.dll",
        "name": "System.Private.DataContractSerialization.dll",
        "hash": "sha256-Zn4D32KDOB8QJypWjnUKhjUEgiO7rTmIuPi4akZZYh8="
      },
      {
        "virtualPath": "System.Private.Uri.dll",
        "name": "System.Private.Uri.dll",
        "hash": "sha256-858VKlIa2NYyBhYNDnaEu6l8FgkNeiPyYnmhW5zU5Po="
      },
      {
        "virtualPath": "System.Private.Xml.Linq.dll",
        "name": "System.Private.Xml.Linq.dll",
        "hash": "sha256-bi+jNsz0C4Z0YS4FXB2VlnRC8vIL/Fknk0331Qv4rZQ="
      },
      {
        "virtualPath": "System.Private.Xml.dll",
        "name": "System.Private.Xml.dll",
        "hash": "sha256-+tYyoig205XDeUtQLF3EaxzHxX28k48CkX1eoQVOGG4="
      },
      {
        "virtualPath": "System.Reflection.DispatchProxy.dll",
        "name": "System.Reflection.DispatchProxy.dll",
        "hash": "sha256-wunMr90XO5KPCIll26wFN9ExGKN0zAg8nFwXFugzcxU="
      },
      {
        "virtualPath": "System.Reflection.Emit.ILGeneration.dll",
        "name": "System.Reflection.Emit.ILGeneration.dll",
        "hash": "sha256-i7sJ7rGETJRaA8EmRcOhccA8i3/2EwCX11ufkskuLQM="
      },
      {
        "virtualPath": "System.Reflection.Emit.Lightweight.dll",
        "name": "System.Reflection.Emit.Lightweight.dll",
        "hash": "sha256-r1XOwM19T/QzTzZVOLg2IuG8AET4P/AX5pgIBx0OHxY="
      },
      {
        "virtualPath": "System.Reflection.Emit.dll",
        "name": "System.Reflection.Emit.dll",
        "hash": "sha256-xgzv2VtR7Qn4gSgyHrTOR75kOD7P4WpfhALtso9Cxv8="
      },
      {
        "virtualPath": "System.Reflection.Extensions.dll",
        "name": "System.Reflection.Extensions.dll",
        "hash": "sha256-Ml88eKkY92R3kfjqvHGcy0wKEJpMjC1eoEoAHSE4hAk="
      },
      {
        "virtualPath": "System.Reflection.Metadata.dll",
        "name": "System.Reflection.Metadata.dll",
        "hash": "sha256-V6yhvaZrwESktCvin94C6ygO2mJ9vGtNuh7eOq9qI3o="
      },
      {
        "virtualPath": "System.Reflection.Primitives.dll",
        "name": "System.Reflection.Primitives.dll",
        "hash": "sha256-G16/3No9Zcq0B7VtbxI3b+vfLtK4zfmOqsIMR0HM8mU="
      },
      {
        "virtualPath": "System.Reflection.TypeExtensions.dll",
        "name": "System.Reflection.TypeExtensions.dll",
        "hash": "sha256-MkMnHj434z4yn4T5Q+R3S97c/MIDNjX0hPKQj0siM38="
      },
      {
        "virtualPath": "System.Reflection.dll",
        "name": "System.Reflection.dll",
        "hash": "sha256-zJ9qeTK5OsP4hUJFeB7kMBYVOFeumAXOFKcImJGMkmc="
      },
      {
        "virtualPath": "System.Resources.Reader.dll",
        "name": "System.Resources.Reader.dll",
        "hash": "sha256-Pt88goXHe47Zs2uN8En7BGYpONENfISVXml4Ah9vMWg="
      },
      {
        "virtualPath": "System.Resources.ResourceManager.dll",
        "name": "System.Resources.ResourceManager.dll",
        "hash": "sha256-lB7o6iGPIcm3gL7ScSQer+wlGD8h00CX8FVWJVYmPzQ="
      },
      {
        "virtualPath": "System.Resources.Writer.dll",
        "name": "System.Resources.Writer.dll",
        "hash": "sha256-BWWfnXpNAdjPeBVVTK4IPqWPa1FmyCrKErPQYa5mdoM="
      },
      {
        "virtualPath": "System.Runtime.CompilerServices.Unsafe.dll",
        "name": "System.Runtime.CompilerServices.Unsafe.dll",
        "hash": "sha256-5SkSSEYM6pvIXxhVV2DpmKX43u9WxUlUWOLFpRdTrmA="
      },
      {
        "virtualPath": "System.Runtime.CompilerServices.VisualC.dll",
        "name": "System.Runtime.CompilerServices.VisualC.dll",
        "hash": "sha256-o/8M+/1rH7q0k4PdxSoBZ9777Rza9pY4xdijZxsFE3k="
      },
      {
        "virtualPath": "System.Runtime.Extensions.dll",
        "name": "System.Runtime.Extensions.dll",
        "hash": "sha256-m5su/e9/cA9Vl7VpNI88myZfJNckRdSis6fooD+g6u8="
      },
      {
        "virtualPath": "System.Runtime.Handles.dll",
        "name": "System.Runtime.Handles.dll",
        "hash": "sha256-yf97y5eGvARdC+yhKc1WHWyVZQ/gVRdowLNC+zyXakg="
      },
      {
        "virtualPath": "System.Runtime.InteropServices.RuntimeInformation.dll",
        "name": "System.Runtime.InteropServices.RuntimeInformation.dll",
        "hash": "sha256-5xB6j95iTii9d31v7l5pNu7iXNj3KKEh9Pr+jRRMDOg="
      },
      {
        "virtualPath": "System.Runtime.InteropServices.dll",
        "name": "System.Runtime.InteropServices.dll",
        "hash": "sha256-spX/9APDmJgaXJv0azTwTGjI15oaPh3CeSt1Uu1oUD0="
      },
      {
        "virtualPath": "System.Runtime.Intrinsics.dll",
        "name": "System.Runtime.Intrinsics.dll",
        "hash": "sha256-4JhjCZWbZRO3yIKyq3kcPUZZd8iZB6gPw26/Ylht0TI="
      },
      {
        "virtualPath": "System.Runtime.Loader.dll",
        "name": "System.Runtime.Loader.dll",
        "hash": "sha256-3fyy85BenjX1N43hRmly+SpumJYrdsMClFCB29j4JgU="
      },
      {
        "virtualPath": "System.Runtime.Numerics.dll",
        "name": "System.Runtime.Numerics.dll",
        "hash": "sha256-Zj5ciOUCt+ChUs+Sqq+hkou4t7L8Pv8BONfsfz4QXr0="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Formatters.dll",
        "name": "System.Runtime.Serialization.Formatters.dll",
        "hash": "sha256-wJtGteWvSQBU9Xo2Aamb8Uyo1di0ui3TuS4n/wT7Vp8="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Json.dll",
        "name": "System.Runtime.Serialization.Json.dll",
        "hash": "sha256-0K4esfoR5Pos6lc47rqVag3RNgF8i9608tV+Yv3ng6o="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Primitives.dll",
        "name": "System.Runtime.Serialization.Primitives.dll",
        "hash": "sha256-nNltr9vNyvtQlUVXeBL4RBoWjiloEnR2icUjRFfk9BI="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Xml.dll",
        "name": "System.Runtime.Serialization.Xml.dll",
        "hash": "sha256-BPzwVr/oHqBfQwgolfUnwqTFEGE86oEDrpkIytJeJCc="
      },
      {
        "virtualPath": "System.Runtime.Serialization.dll",
        "name": "System.Runtime.Serialization.dll",
        "hash": "sha256-JKaa1Nl3bhpFHvyW4+qlSBIXG4dmnUBn79WGSpCgeeQ="
      },
      {
        "virtualPath": "System.Runtime.dll",
        "name": "System.Runtime.dll",
        "hash": "sha256-pKGJ7zKnKWRo45RnW/hqgQy5/yRWTvTACIHLl91TZqg="
      },
      {
        "virtualPath": "System.Security.AccessControl.dll",
        "name": "System.Security.AccessControl.dll",
        "hash": "sha256-/2X8xwvXtGL7bbdr0wHVtGhmVCXbTqJtKD1oOW5Yir8="
      },
      {
        "virtualPath": "System.Security.Claims.dll",
        "name": "System.Security.Claims.dll",
        "hash": "sha256-NmngBTIQAR8Hfj5emfX2AWeSXeMpK7Sz0VZs7Bu43Nk="
      },
      {
        "virtualPath": "System.Security.Cryptography.Algorithms.dll",
        "name": "System.Security.Cryptography.Algorithms.dll",
        "hash": "sha256-Gw4pjV1mP5KlCj4p8EJSHlWAq6rQ6OiJRzboyXng2lA="
      },
      {
        "virtualPath": "System.Security.Cryptography.Cng.dll",
        "name": "System.Security.Cryptography.Cng.dll",
        "hash": "sha256-TPDbudMxOOlr/b8dV9tgW4zAL6WObZNosb3aC3p9jTE="
      },
      {
        "virtualPath": "System.Security.Cryptography.Csp.dll",
        "name": "System.Security.Cryptography.Csp.dll",
        "hash": "sha256-WNKJjKyhHf/DXhwEupuFNZp8DGNeOJFThagqjnn6lCA="
      },
      {
        "virtualPath": "System.Security.Cryptography.Encoding.dll",
        "name": "System.Security.Cryptography.Encoding.dll",
        "hash": "sha256-EMk4DYkJvhj8uAL/efHLuOePNk4awYJHn9m3/w2lKLc="
      },
      {
        "virtualPath": "System.Security.Cryptography.OpenSsl.dll",
        "name": "System.Security.Cryptography.OpenSsl.dll",
        "hash": "sha256-bmJnRnHS2+ldp+SSdsGExfA/75aSA/SkIdCNSb4GjSA="
      },
      {
        "virtualPath": "System.Security.Cryptography.Primitives.dll",
        "name": "System.Security.Cryptography.Primitives.dll",
        "hash": "sha256-PADS5NbY0hs/OsrzdHYu4wt8zVRe5LUOcQDbVK5bScM="
      },
      {
        "virtualPath": "System.Security.Cryptography.ProtectedData.dll",
        "name": "System.Security.Cryptography.ProtectedData.dll",
        "hash": "sha256-t2HJmIE1Yrqe3rSsSS2em9eaLr2IB4aEGBaUpiYnAV8="
      },
      {
        "virtualPath": "System.Security.Cryptography.X509Certificates.dll",
        "name": "System.Security.Cryptography.X509Certificates.dll",
        "hash": "sha256-rS5EfNUaqztj1M5HhTaktiJKogzvuvJbqUAVXBRiANo="
      },
      {
        "virtualPath": "System.Security.Cryptography.dll",
        "name": "System.Security.Cryptography.dll",
        "hash": "sha256-xbumrqyEE1rFXm92M3E71Ki/+0g1heka+lCE5UxpUVU="
      },
      {
        "virtualPath": "System.Security.Principal.Windows.dll",
        "name": "System.Security.Principal.Windows.dll",
        "hash": "sha256-iLtdJL4kys0OYIG9YyOeHNXEzLnRsAJ+vcLc10zleQQ="
      },
      {
        "virtualPath": "System.Security.Principal.dll",
        "name": "System.Security.Principal.dll",
        "hash": "sha256-hregpiKO5qvzxPzQS735Uq2c4iIAOtSDrrqZLPQ5lHQ="
      },
      {
        "virtualPath": "System.Security.SecureString.dll",
        "name": "System.Security.SecureString.dll",
        "hash": "sha256-lF5I+v6qRD9rBzrYwxsjawaItNim8oMaX2a9G/J3Peo="
      },
      {
        "virtualPath": "System.Security.dll",
        "name": "System.Security.dll",
        "hash": "sha256-DQZN+DBVstaLoT5y+FFwM3wwYhh7rZmC39deY+dcSr0="
      },
      {
        "virtualPath": "System.ServiceModel.Web.dll",
        "name": "System.ServiceModel.Web.dll",
        "hash": "sha256-JbYKP2ECoixnAfT1l+cqudb70NiLr6klkFif988agYQ="
      },
      {
        "virtualPath": "System.ServiceProcess.dll",
        "name": "System.ServiceProcess.dll",
        "hash": "sha256-i2u43PeIwYo8U/X6dheHSP1Kwh5YaYnzRJxdKU+DpTM="
      },
      {
        "virtualPath": "System.Text.Encoding.CodePages.dll",
        "name": "System.Text.Encoding.CodePages.dll",
        "hash": "sha256-Ff+WP45akeDGxNa0VZ4cexceCbJtr64o+ovhgGC9G3o="
      },
      {
        "virtualPath": "System.Text.Encoding.Extensions.dll",
        "name": "System.Text.Encoding.Extensions.dll",
        "hash": "sha256-PIqpcRrEUYIcdR4YcQZlp8K9bc3jwo9xzQnvKnQL93s="
      },
      {
        "virtualPath": "System.Text.Encoding.dll",
        "name": "System.Text.Encoding.dll",
        "hash": "sha256-z+VOYdh2CMN0gVoha+bN4LCZySIvMypwA2gaNh7o0I4="
      },
      {
        "virtualPath": "System.Text.Encodings.Web.dll",
        "name": "System.Text.Encodings.Web.dll",
        "hash": "sha256-ahH1MMSdxG0z1m+wg8rrJ6fwkOP35jPvnNPxyrsFHP0="
      },
      {
        "virtualPath": "System.Text.Json.dll",
        "name": "System.Text.Json.dll",
        "hash": "sha256-lz3Y5f5KpGgZlEH761a7uJQRGIKxABaO4RdlgJ2dRew="
      },
      {
        "virtualPath": "System.Text.RegularExpressions.dll",
        "name": "System.Text.RegularExpressions.dll",
        "hash": "sha256-HTyeD+cxcptypA401ctM2uejl3VivQ3s5M8o8iKljSk="
      },
      {
        "virtualPath": "System.Threading.AccessControl.dll",
        "name": "System.Threading.AccessControl.dll",
        "hash": "sha256-5+43Bo85aWh6c6yqhMt5Ca6s2QNA6qIuN0ktgq0MPuE="
      },
      {
        "virtualPath": "System.Threading.Channels.dll",
        "name": "System.Threading.Channels.dll",
        "hash": "sha256-b4KJyRtBa8z8hAbpYGa8w2ALjQJ2bZYYcoTRrJoO8sY="
      },
      {
        "virtualPath": "System.Threading.Overlapped.dll",
        "name": "System.Threading.Overlapped.dll",
        "hash": "sha256-sjK4/AorueEXUBEQ0aDd1FExNW6pzy76lCthLdrSBc8="
      },
      {
        "virtualPath": "System.Threading.Tasks.Dataflow.dll",
        "name": "System.Threading.Tasks.Dataflow.dll",
        "hash": "sha256-EdfvIdN3TfJ3HBAO3dpzvJ+r+ixTCtdf0otliAUcyaM="
      },
      {
        "virtualPath": "System.Threading.Tasks.Extensions.dll",
        "name": "System.Threading.Tasks.Extensions.dll",
        "hash": "sha256-9qZTZPXXzJO85faRIk/Z5NbrxDsWiJD6zejTTLLAsa4="
      },
      {
        "virtualPath": "System.Threading.Tasks.Parallel.dll",
        "name": "System.Threading.Tasks.Parallel.dll",
        "hash": "sha256-HqJ8KuIuolifXZiWzLnjuFVnflNNSt909bo3HLywCSE="
      },
      {
        "virtualPath": "System.Threading.Tasks.dll",
        "name": "System.Threading.Tasks.dll",
        "hash": "sha256-yCca4Mr91utP3LYorHsHiy1q1rTjkWZkeswB5H2/I3g="
      },
      {
        "virtualPath": "System.Threading.Thread.dll",
        "name": "System.Threading.Thread.dll",
        "hash": "sha256-/O5zUHZ1M+GpbOvW2gN4nwP2wd/AqHSnmzV0UZQu01c="
      },
      {
        "virtualPath": "System.Threading.ThreadPool.dll",
        "name": "System.Threading.ThreadPool.dll",
        "hash": "sha256-0fZi2i4zGGIofVMEm9lg2r+fmFQEhdsURogdzRmrGEc="
      },
      {
        "virtualPath": "System.Threading.Timer.dll",
        "name": "System.Threading.Timer.dll",
        "hash": "sha256-6ob5By5yRbp7yolDt7OS1qigeeSe5UaFuZzd5ZVfs/8="
      },
      {
        "virtualPath": "System.Threading.dll",
        "name": "System.Threading.dll",
        "hash": "sha256-pRBa3tGLKGWZBmQO5T7f1uESvXuWKzPSmNvFaszf1SI="
      },
      {
        "virtualPath": "System.Transactions.Local.dll",
        "name": "System.Transactions.Local.dll",
        "hash": "sha256-dBmNgC9XBvkuNaMTos+8Ldq7OmiZKU3a7bvDZPgK2Tw="
      },
      {
        "virtualPath": "System.Transactions.dll",
        "name": "System.Transactions.dll",
        "hash": "sha256-myNOdmEzaE4pVXCVlAg1WUDCA0tVqLPfE7LqZjKDAzM="
      },
      {
        "virtualPath": "System.ValueTuple.dll",
        "name": "System.ValueTuple.dll",
        "hash": "sha256-yv1ZY6kL2f53GXD1E+8H8wZ2LVXrdXcRkeXC6do8eqk="
      },
      {
        "virtualPath": "System.Web.HttpUtility.dll",
        "name": "System.Web.HttpUtility.dll",
        "hash": "sha256-IPnIw2QvPwEATzQlhG9p7YpjBVhCfV7DTxbD7RRwutU="
      },
      {
        "virtualPath": "System.Web.dll",
        "name": "System.Web.dll",
        "hash": "sha256-weKwzDqP31g537PLUr8M925hHv1Te6X1mC7GOLeKFmE="
      },
      {
        "virtualPath": "System.Windows.dll",
        "name": "System.Windows.dll",
        "hash": "sha256-Yu1jzbtcG4wABEGzFF/x31m2MqISQpDakonIn6ZKq60="
      },
      {
        "virtualPath": "System.Xml.Linq.dll",
        "name": "System.Xml.Linq.dll",
        "hash": "sha256-2gTSFCmO6yW0Tobp9KrthAL7HgS6cVe2MUMyhOHC/Fo="
      },
      {
        "virtualPath": "System.Xml.ReaderWriter.dll",
        "name": "System.Xml.ReaderWriter.dll",
        "hash": "sha256-8DrXzXUCcOh89UmlKQMSW46kNdXXfi6XK1tt8q2KMMc="
      },
      {
        "virtualPath": "System.Xml.Serialization.dll",
        "name": "System.Xml.Serialization.dll",
        "hash": "sha256-bUPi0HKMPUTkScF3nDjX0M7eNHLFDERwJoOHcLzsn28="
      },
      {
        "virtualPath": "System.Xml.XDocument.dll",
        "name": "System.Xml.XDocument.dll",
        "hash": "sha256-DUYzGGnl9unuc7jtNWZFNA6Q1aeqM18E+q1YzLECkdI="
      },
      {
        "virtualPath": "System.Xml.XPath.XDocument.dll",
        "name": "System.Xml.XPath.XDocument.dll",
        "hash": "sha256-f1fgSwp2nl6rGUljbUz8FxTDvKgnEWmSl0SMhc1w6LU="
      },
      {
        "virtualPath": "System.Xml.XPath.dll",
        "name": "System.Xml.XPath.dll",
        "hash": "sha256-13KQMHx0OMH7tPu7uiOlCJPcVlNFgQ7kNrWObhAR4Kw="
      },
      {
        "virtualPath": "System.Xml.XmlDocument.dll",
        "name": "System.Xml.XmlDocument.dll",
        "hash": "sha256-am28xONXrS0UgRDAzPasNIrY8u7svHYUBQLSVevo1tM="
      },
      {
        "virtualPath": "System.Xml.XmlSerializer.dll",
        "name": "System.Xml.XmlSerializer.dll",
        "hash": "sha256-1FPq93aKAvbRaRdfghu65d7pr9oa88zMaQdpfHH0ZJs="
      },
      {
        "virtualPath": "System.Xml.dll",
        "name": "System.Xml.dll",
        "hash": "sha256-msp6FNAH6vH1QsnftdBk4HqIQ7SIjxMxxHeSck2CRlU="
      },
      {
        "virtualPath": "System.dll",
        "name": "System.dll",
        "hash": "sha256-Z1jJ1XwSfaTpypnIO/ak2Mv3/7jsBN4halxU55tIJrQ="
      },
      {
        "virtualPath": "WindowsBase.dll",
        "name": "WindowsBase.dll",
        "hash": "sha256-1VI5RHLbHTqhGO9oxKq6JpzCRk8DN2uQaJMoYitwMn4="
      },
      {
        "virtualPath": "mscorlib.dll",
        "name": "mscorlib.dll",
        "hash": "sha256-QgkbQ9iojIDWoSvhgrLyPCh0Bik3ceavY/OMSiAilL0="
      },
      {
        "virtualPath": "netstandard.dll",
        "name": "netstandard.dll",
        "hash": "sha256-4YnTTYf7Ofa1xx5V2ncTyFvHO8a31Qm+mZvLeCjwbxc="
      }
    ],
    "satelliteResources": {
      "cs": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-wMT+92xu5wDSHbwtZnARSZHd/U5gPCyFLl/+9qmQYJo="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-viYVhuX5DH69yLXhLUHy9JO7GPa1p2/h+rShSj2AMU8="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-G3qVM04EV+0zRkbLYuEoe4jpCOUKXM3OYIkWE//3KVw="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-R+ID9h3YauLzcLNz7vQcJCXjfJu7ojBjrlJ1YeL9WZU="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-11RJizusuX4aU+m4bdr65crncrI9uXxJIqnBR1aVsMk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-xeJC/JONs+/d6P9EV+scA/UKvGPfs+bt1gGIoBUYSdg="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-tvl3DHyDFgsn2W5BSG2pntQ963G7+40VvXDyXcRSwas="
        }
      ],
      "de": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-U05vUt7s7Z4+SmNhDrUfrG70lo9ZjWZuUfwWPguScuI="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-brTCCRQ6i5d8YdnzCh65LTmntqdBECwiFmPw5g+8RCc="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-dO4tiIl74hHomtTNEaFFqXO9SvDnSHe1pk+NIn4LpAk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-B5dlEO/75jsfTyC1ySRTD3y92V9KISm1sxzHYAIhokc="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-Cwcmqw91R6Tgnb5AOXcdREY5bIqVllc4H1D7phHK8VM="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-ltY5pmxsihgtxTK0f3q9cha80dOtLDYYd37YiQv8f+s="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-zWKhv+mYEclxvsEyilSNvF8k47yc0n+XFTc8HlKr8y8="
        }
      ],
      "es": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-lcPKfdHI9HJgqp7EfyI88g9T2YJDSrSQFn8Hs1l9A50="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-qNsnxeq3gl5da5Z9jiFPATbBLDyKkdNEzv0rNIWrPq0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-cZvA95nhJwRJhkkmfkcxZvkQFbYp9BFkdAkfjuklPk0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-2MH2D7JBAfvYOQtKViegIaxuLovx/BEY2R3nTq18rSA="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-9bsQ4rtzhPuyN0inTFfF6iupiWX1nwOUz2bJLhxiop4="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-xchj+oQsXcRGLc1U6aa4wlMa4cwfGl6AFvZBJvd8t2Y="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-THQbR/or7PLxKD6W7G922Vjx3vwoaNk8LAEcr8vJ+fQ="
        }
      ],
      "fr": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-SOVyROgd7vzE44aDhhy9P6W0HI7iLXuhkxOMn57cozc="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-pwYJ+RrVwdIj8vnb/25K5yIaA6fKWfmuO9iU8QFcU0Q="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-Rj4iug/f7gAl1vsAMUoKfPikfWtmidiOBCloYCWo9Tw="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-kfHnTHy0qdWK0hlgVVieKB/2zBxlpnna4mT+Svq9/WM="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-yWjx9BTbaKcVCmUKRkBljXanR3tp2W2QjyiPZhYDmWo="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-/x7hOV8V2PE+n9MRPD+s2h4yHGZdMHZYkANO5bu7wJM="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-gPWgeDiwRjiP2niCIyecHGY47M+WEP6ApZi7F/OyDyI="
        }
      ],
      "it": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-UtxKrcT81WgEJ6fAG4duLkBmNMiw01RpE5TVPTIn8wU="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-KXkcuivUre4n/JRdvXvCbWottF04zaaOW/w5aTpiTUM="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-qFPz5DPJiSFQxHrVu+3UbTrLdVu8IO1YeWuePnWqklA="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-YCFGiSy7QKvqnDheggeG6x7rFGq1jP1LwOu6CEGxCUk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-k4yE0048sHxuIHr2JXjTSZTjncyxVe5770QHZdaHDhg="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-bEdZgHrrVHQ3rcPnek4khSdT0Hh28AVvZce249P2UG0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-Mhok9axgUxQ4BYX0mR2w2ZvKxZ7Jdr6Jalif6EL+96A="
        }
      ],
      "ja": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-zRlpnrTfftsUaXFsaHbR/2xqxkmL9dYkz5SlCbdpqw0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-6rc6lju0YDJy7RfS/+z3AaZIduLoa56TlgyzBJW2djY="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-TSuq2IjX2i8g1SjSf2tWKGlhpsqilKPTkn+DSjRZ9x0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-tP13JDAwTolRTAXKAdgWJ3j8MXvPcYru5mZlj7a29FE="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-iT7YmXb0JGisQ0ewFatEGIsMjriH8FsIWLTJjLZz0Ho="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-AhpKM52J0G4rMASAQWWXph2Xzxx4f9PHAkyEkiA4fkM="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-9mQypReoNMoWkxfBcNMqYQCUw+vAwrH1nQCHsz4tNq0="
        }
      ],
      "ko": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-1ZZiV3htIohE8OVg0UQNRkIHf7ZJ9fAczPxqcrIVecw="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-CIFCCCZAZBBavMyWlXweKuBz1Ofdl+10WZjwS3dyc8s="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-ai5HfCqGBBV/LJ+ON1apirB5lcDm/P4i8ue/ZnnzA4k="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-7zYO/1s6hkJZPUaBMPQH+fmng995pPIoOFi4tRj4CsQ="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-xbdgJpIXoBsGhYRnPnH1RLNQpORm79XVD39nuCTMV4I="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-byjhzUx85TEd37TeO0KWalVe5CqkFgMzL4gVT2qbIcQ="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-EDPT1dEFV/cHBDC+VkPciWINhMvqwhEs+4xKHRWAhF8="
        }
      ],
      "pl": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-5VW1/GhR7n/QTY6RSgtuPTHVWfoIJy2L/B4+9Od/4XA="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-SSkr85fkeGlPaNfLGGrvqOZWX6p2aOcNSGxTyttKvJ4="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-ObXhRxgXeQox7EA4Y5GI97if1nE+oZyIRyvc9PpW95U="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-uwDXrbTwoipQm92jGzZpXkLdfg3E60ixX3oWgHrzxu0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-zURsuCHHhmegfyAjkcJ5/mOylrYjfe054o1l8A/+0zs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-4u6E90tI27cwgJNBtSeeUh2/1oRMATQAd34OHQIWDA8="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-+HeeRbH2mf5yKJixhlN3vtr1pDPBqrYpivWz7fBn6I8="
        }
      ],
      "pt-BR": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-M7J3tAn+m+tjBPN6x/ZE+R0OtHJ9ktZMzXQe5b34jqU="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-umtAedVgrzF25I6PRqp/MwW0NxewUfx80nqRO6k3MKk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-AYgkb6I56trSWIUJPyHCDfSndfeU/DRGhDBymXSowew="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-Ctt4Zy5QV4acRmDl2EnGpzyV1KWTFUZV01jwiUaYMi4="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-2nPrDtJOAQ9Uo640gLAitf/DJfB6Zzmdx7hqZ9rnsfE="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-U/oznQFRIVz97Bh7LqpzIV+Am5XKhqjIgrDegvbUhVA="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-j/2w7/fC3mNaOPAJPaXpX1LKkF7CCAyrTvrEJYXdAUM="
        }
      ],
      "ru": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-JXbtcPu1p0sxSVjloKFpPfgdTQwmJpNkJ6/L/nPxEWI="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-OGLpmQ6n2S7sNAZuhzDlyE1FS/Ohq/01YjrnXQXOvx0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-0Le1e7kKSu1knxDDD82uXJ77goBEcoenaj1k6w2xPkE="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-X8cwOzMlc48wMxn9MsmDSQlIiQJesmScq85wnBCRILs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-rnhnT4Pslyvz9e7216rnxjfSLZq5c5TVVLiI7KJSln8="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-wkiGfoc8YpoiFO7YWk/ZvUJrIUBHI0aJgdqrd77vPC8="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-UVPRg2giThINJBUmLEkwIQC7du96/4qmhjANQiV2bGc="
        }
      ],
      "tr": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-TyO+XIe2zaOBL4emYNx2BEPjJSxuPR3uIwEGSN6YOqE="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-9NAF0YFIInoXGlaJwSLs3hHCJEJkTmmMXbpoENJv59w="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-Cf9FbUfXm28afuVRJmKkkK/aoFxg64GQp9zXauBQ3A4="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-mwUQV9+R6TbHrIydknZ++/0JPbgV+Z+XMFEXEEmA1Tk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-wII24jPsJNC4OMi5qwFTuRV573huNTe1NpFMV1qyFgo="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-uQI8sj/nsZYLAcXGi7NNLyWNxPdGPOwIaKAITG59+nY="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-X96YRRvoudEe+pqgeDxk3PumxVL0BdA7gqV1zv9jECc="
        }
      ],
      "zh-Hans": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-I98OoDG71aktI9HGTmFWya7Cf/ECVaiwNWvic5aiX0Y="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-8JHjbNgpRFrlVySO+O98p2ZCj+D2VL3K8Vm+NU6ryPk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-/lJqfN+5oG4sZ5pnvAfx9PTxr8tKwtF2846i/EsRv74="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-007vihlEeZJn4Sx8jQNZgxTgUT2CcYM6SoJYwyFRsFY="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-dyZtPYbA/61e5pIH232zZOS21JTGU8djcbiMXnLItEs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-YdK4k4hmooNfyuQUvMIJz8fvw2dlYwYtgN64ZTheQAs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-K4Rg5i2GyRzOIYLDG1ncPO+c+Kx/rOGa4FklFsMieJc="
        }
      ],
      "zh-Hant": [
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Features.resources.dll",
          "hash": "sha256-r0bCUWaxP1s4Q/3SBuqv9zvludSts+jq2Ln0JC/Eimw="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll",
          "hash": "sha256-XYIiQEIbmrtfhT7rFavnRQSNt3I+nf8RbZaJUNC+v4A="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "name": "Microsoft.CodeAnalysis.CSharp.resources.dll",
          "hash": "sha256-UV533evWM8f3nfrjHI1bND7SeIMhGTV73DIoxybD5I0="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Features.resources.dll",
          "name": "Microsoft.CodeAnalysis.Features.resources.dll",
          "hash": "sha256-tmN+Tf84X9Bh6+AAjPCS8PbqeDB3FK1Q+1wCsq9Vmqs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "name": "Microsoft.CodeAnalysis.Scripting.resources.dll",
          "hash": "sha256-WbFudx6xwLel76C8GsxDIg33LR5YCqNCCEryUQBz1Fs="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "name": "Microsoft.CodeAnalysis.Workspaces.resources.dll",
          "hash": "sha256-L5nQvC670fa4OO9FC290S5VbqZZ195gLXhFjEnSjZsk="
        },
        {
          "virtualPath": "Microsoft.CodeAnalysis.resources.dll",
          "name": "Microsoft.CodeAnalysis.resources.dll",
          "hash": "sha256-4biCpiT8L++Pxjot0KYeGVQ/bCl0sK5fGo5YDApJCu0="
        }
      ]
    },
    "lazyAssembly": [
      {
        "virtualPath": "Npgsql.EntityFrameworkCore.PostgreSQL.dll",
        "name": "Npgsql.EntityFrameworkCore.PostgreSQL.dll",
        "hash": "sha256-kCv5AnkGzFMRxPnq2G0Zb0HiI6uRSQlqb+qTWIX0RLQ="
      },
      {
        "virtualPath": "Npgsql.dll",
        "name": "Npgsql.dll",
        "hash": "sha256-bHTwIhHTwjI9tzx8PIi/YQ6e6V4/hN1updYw7vmJXBs="
      }
    ],
    "libraryInitializers": [
      {
        "name": "_content/Microsoft.AspNetCore.Components.CustomElements/Microsoft.AspNetCore.Components.CustomElements.lib.module.js"
      }
    ],
    "modulesAfterRuntimeReady": [
      {
        "name": "../_content/Microsoft.AspNetCore.Components.CustomElements/Microsoft.AspNetCore.Components.CustomElements.lib.module.js"
      }
    ]
  },
  "debugLevel": 0,
  "globalizationMode": "sharded",
  "extensions": {
    "blazor": {}
  },
  "runtimeConfig": {
    "runtimeOptions": {
      "configProperties": {
        "Microsoft.AspNetCore.Components.Routing.RegexConstraintSupport": false,
        "System.Diagnostics.Debugger.IsSupported": false,
        "System.Diagnostics.Metrics.Meter.IsSupported": false,
        "System.Diagnostics.Tracing.EventSource.IsSupported": false,
        "System.GC.Server": true,
        "System.Globalization.Invariant": false,
        "System.TimeZoneInfo.Invariant": false,
        "System.Linq.Enumerable.IsSizeOptimized": true,
        "System.Net.Http.EnableActivityPropagation": false,
        "System.Net.Http.WasmEnableStreamingResponse": true,
        "System.Net.SocketsHttpHandler.Http3Support": false,
        "System.Reflection.Metadata.MetadataUpdater.IsSupported": false,
        "System.Reflection.NullabilityInfoContext.IsSupported": true,
        "System.Resources.UseSystemResourceKeys": true,
        "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false,
        "System.Text.Encoding.EnableUnsafeUTF7Encoding": false,
        "System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault": true
      }
    }
  }
}/*json-end*/);export{gt as default,ft as dotnet,mt as exit};
