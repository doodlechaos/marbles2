var __create = Object.create;
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __name = (target, value) => __defProp(target, "name", { value, configurable: true });
var __esm = (fn, res) => function __init() {
  return fn && (res = (0, fn[__getOwnPropNames(fn)[0]])(fn = 0)), res;
};
var __commonJS = (cb, mod) => function __require() {
  return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
};
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key2 of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key2) && key2 !== except)
        __defProp(to, key2, { get: () => from[key2], enumerable: !(desc = __getOwnPropDesc(from, key2)) || desc.enumerable });
  }
  return to;
};
var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(
  // If the importer is in node compatibility mode or this is not an ESM
  // file that has been converted to a CommonJS file using a Babel-
  // compatible transform (i.e. "__esModule" has not been set), then set
  // "default" to the CommonJS "module.exports" for node compatibility.
  isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target,
  mod
));

// node_modules/unenv/dist/runtime/_internal/utils.mjs
// @__NO_SIDE_EFFECTS__
function createNotImplementedError(name) {
  return new Error(`[unenv] ${name} is not implemented yet!`);
}
// @__NO_SIDE_EFFECTS__
function notImplemented(name) {
  const fn = /* @__PURE__ */ __name(() => {
    throw /* @__PURE__ */ createNotImplementedError(name);
  }, "fn");
  return Object.assign(fn, { __unenv__: true });
}
// @__NO_SIDE_EFFECTS__
function notImplementedClass(name) {
  return class {
    __unenv__ = true;
    constructor() {
      throw new Error(`[unenv] ${name} is not implemented yet!`);
    }
  };
}
var init_utils = __esm({
  "node_modules/unenv/dist/runtime/_internal/utils.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    __name(createNotImplementedError, "createNotImplementedError");
    __name(notImplemented, "notImplemented");
    __name(notImplementedClass, "notImplementedClass");
  }
});

// node_modules/unenv/dist/runtime/node/internal/perf_hooks/performance.mjs
var _timeOrigin, _performanceNow, nodeTiming, PerformanceEntry, PerformanceMark, PerformanceMeasure, PerformanceResourceTiming, PerformanceObserverEntryList, Performance, PerformanceObserver, performance;
var init_performance = __esm({
  "node_modules/unenv/dist/runtime/node/internal/perf_hooks/performance.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_utils();
    _timeOrigin = globalThis.performance?.timeOrigin ?? Date.now();
    _performanceNow = globalThis.performance?.now ? globalThis.performance.now.bind(globalThis.performance) : () => Date.now() - _timeOrigin;
    nodeTiming = {
      name: "node",
      entryType: "node",
      startTime: 0,
      duration: 0,
      nodeStart: 0,
      v8Start: 0,
      bootstrapComplete: 0,
      environment: 0,
      loopStart: 0,
      loopExit: 0,
      idleTime: 0,
      uvMetricsInfo: {
        loopCount: 0,
        events: 0,
        eventsWaiting: 0
      },
      detail: void 0,
      toJSON() {
        return this;
      }
    };
    PerformanceEntry = class {
      static {
        __name(this, "PerformanceEntry");
      }
      __unenv__ = true;
      detail;
      entryType = "event";
      name;
      startTime;
      constructor(name, options2) {
        this.name = name;
        this.startTime = options2?.startTime || _performanceNow();
        this.detail = options2?.detail;
      }
      get duration() {
        return _performanceNow() - this.startTime;
      }
      toJSON() {
        return {
          name: this.name,
          entryType: this.entryType,
          startTime: this.startTime,
          duration: this.duration,
          detail: this.detail
        };
      }
    };
    PerformanceMark = class PerformanceMark2 extends PerformanceEntry {
      static {
        __name(this, "PerformanceMark");
      }
      entryType = "mark";
      constructor() {
        super(...arguments);
      }
      get duration() {
        return 0;
      }
    };
    PerformanceMeasure = class extends PerformanceEntry {
      static {
        __name(this, "PerformanceMeasure");
      }
      entryType = "measure";
    };
    PerformanceResourceTiming = class extends PerformanceEntry {
      static {
        __name(this, "PerformanceResourceTiming");
      }
      entryType = "resource";
      serverTiming = [];
      connectEnd = 0;
      connectStart = 0;
      decodedBodySize = 0;
      domainLookupEnd = 0;
      domainLookupStart = 0;
      encodedBodySize = 0;
      fetchStart = 0;
      initiatorType = "";
      name = "";
      nextHopProtocol = "";
      redirectEnd = 0;
      redirectStart = 0;
      requestStart = 0;
      responseEnd = 0;
      responseStart = 0;
      secureConnectionStart = 0;
      startTime = 0;
      transferSize = 0;
      workerStart = 0;
      responseStatus = 0;
    };
    PerformanceObserverEntryList = class {
      static {
        __name(this, "PerformanceObserverEntryList");
      }
      __unenv__ = true;
      getEntries() {
        return [];
      }
      getEntriesByName(_name, _type) {
        return [];
      }
      getEntriesByType(type) {
        return [];
      }
    };
    Performance = class {
      static {
        __name(this, "Performance");
      }
      __unenv__ = true;
      timeOrigin = _timeOrigin;
      eventCounts = /* @__PURE__ */ new Map();
      _entries = [];
      _resourceTimingBufferSize = 0;
      navigation = void 0;
      timing = void 0;
      timerify(_fn, _options) {
        throw createNotImplementedError("Performance.timerify");
      }
      get nodeTiming() {
        return nodeTiming;
      }
      eventLoopUtilization() {
        return {};
      }
      markResourceTiming() {
        return new PerformanceResourceTiming("");
      }
      onresourcetimingbufferfull = null;
      now() {
        if (this.timeOrigin === _timeOrigin) {
          return _performanceNow();
        }
        return Date.now() - this.timeOrigin;
      }
      clearMarks(markName) {
        this._entries = markName ? this._entries.filter((e3) => e3.name !== markName) : this._entries.filter((e3) => e3.entryType !== "mark");
      }
      clearMeasures(measureName) {
        this._entries = measureName ? this._entries.filter((e3) => e3.name !== measureName) : this._entries.filter((e3) => e3.entryType !== "measure");
      }
      clearResourceTimings() {
        this._entries = this._entries.filter((e3) => e3.entryType !== "resource" || e3.entryType !== "navigation");
      }
      getEntries() {
        return this._entries;
      }
      getEntriesByName(name, type) {
        return this._entries.filter((e3) => e3.name === name && (!type || e3.entryType === type));
      }
      getEntriesByType(type) {
        return this._entries.filter((e3) => e3.entryType === type);
      }
      mark(name, options2) {
        const entry = new PerformanceMark(name, options2);
        this._entries.push(entry);
        return entry;
      }
      measure(measureName, startOrMeasureOptions, endMark) {
        let start;
        let end;
        if (typeof startOrMeasureOptions === "string") {
          start = this.getEntriesByName(startOrMeasureOptions, "mark")[0]?.startTime;
          end = this.getEntriesByName(endMark, "mark")[0]?.startTime;
        } else {
          start = Number.parseFloat(startOrMeasureOptions?.start) || this.now();
          end = Number.parseFloat(startOrMeasureOptions?.end) || this.now();
        }
        const entry = new PerformanceMeasure(measureName, {
          startTime: start,
          detail: {
            start,
            end
          }
        });
        this._entries.push(entry);
        return entry;
      }
      setResourceTimingBufferSize(maxSize) {
        this._resourceTimingBufferSize = maxSize;
      }
      addEventListener(type, listener, options2) {
        throw createNotImplementedError("Performance.addEventListener");
      }
      removeEventListener(type, listener, options2) {
        throw createNotImplementedError("Performance.removeEventListener");
      }
      dispatchEvent(event) {
        throw createNotImplementedError("Performance.dispatchEvent");
      }
      toJSON() {
        return this;
      }
    };
    PerformanceObserver = class {
      static {
        __name(this, "PerformanceObserver");
      }
      __unenv__ = true;
      static supportedEntryTypes = [];
      _callback = null;
      constructor(callback) {
        this._callback = callback;
      }
      takeRecords() {
        return [];
      }
      disconnect() {
        throw createNotImplementedError("PerformanceObserver.disconnect");
      }
      observe(options2) {
        throw createNotImplementedError("PerformanceObserver.observe");
      }
      bind(fn) {
        return fn;
      }
      runInAsyncScope(fn, thisArg, ...args) {
        return fn.call(thisArg, ...args);
      }
      asyncId() {
        return 0;
      }
      triggerAsyncId() {
        return 0;
      }
      emitDestroy() {
        return this;
      }
    };
    performance = globalThis.performance && "addEventListener" in globalThis.performance ? globalThis.performance : new Performance();
  }
});

// node_modules/unenv/dist/runtime/node/perf_hooks.mjs
var init_perf_hooks = __esm({
  "node_modules/unenv/dist/runtime/node/perf_hooks.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_performance();
  }
});

// node_modules/@cloudflare/unenv-preset/dist/runtime/polyfill/performance.mjs
var init_performance2 = __esm({
  "node_modules/@cloudflare/unenv-preset/dist/runtime/polyfill/performance.mjs"() {
    init_perf_hooks();
    globalThis.performance = performance;
    globalThis.Performance = Performance;
    globalThis.PerformanceEntry = PerformanceEntry;
    globalThis.PerformanceMark = PerformanceMark;
    globalThis.PerformanceMeasure = PerformanceMeasure;
    globalThis.PerformanceObserver = PerformanceObserver;
    globalThis.PerformanceObserverEntryList = PerformanceObserverEntryList;
    globalThis.PerformanceResourceTiming = PerformanceResourceTiming;
  }
});

// node_modules/unenv/dist/runtime/mock/noop.mjs
var noop_default;
var init_noop = __esm({
  "node_modules/unenv/dist/runtime/mock/noop.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    noop_default = Object.assign(() => {
    }, { __unenv__: true });
  }
});

// node_modules/unenv/dist/runtime/node/console.mjs
import { Writable } from "node:stream";
var _console, _ignoreErrors, _stderr, _stdout, log, info, trace, debug, table, error, warn, createTask, clear, count, countReset, dir, dirxml, group, groupEnd, groupCollapsed, profile, profileEnd, time, timeEnd, timeLog, timeStamp, Console, _times, _stdoutErrorHandler, _stderrErrorHandler;
var init_console = __esm({
  "node_modules/unenv/dist/runtime/node/console.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_noop();
    init_utils();
    _console = globalThis.console;
    _ignoreErrors = true;
    _stderr = new Writable();
    _stdout = new Writable();
    log = _console?.log ?? noop_default;
    info = _console?.info ?? log;
    trace = _console?.trace ?? info;
    debug = _console?.debug ?? log;
    table = _console?.table ?? log;
    error = _console?.error ?? log;
    warn = _console?.warn ?? error;
    createTask = _console?.createTask ?? /* @__PURE__ */ notImplemented("console.createTask");
    clear = _console?.clear ?? noop_default;
    count = _console?.count ?? noop_default;
    countReset = _console?.countReset ?? noop_default;
    dir = _console?.dir ?? noop_default;
    dirxml = _console?.dirxml ?? noop_default;
    group = _console?.group ?? noop_default;
    groupEnd = _console?.groupEnd ?? noop_default;
    groupCollapsed = _console?.groupCollapsed ?? noop_default;
    profile = _console?.profile ?? noop_default;
    profileEnd = _console?.profileEnd ?? noop_default;
    time = _console?.time ?? noop_default;
    timeEnd = _console?.timeEnd ?? noop_default;
    timeLog = _console?.timeLog ?? noop_default;
    timeStamp = _console?.timeStamp ?? noop_default;
    Console = _console?.Console ?? /* @__PURE__ */ notImplementedClass("console.Console");
    _times = /* @__PURE__ */ new Map();
    _stdoutErrorHandler = noop_default;
    _stderrErrorHandler = noop_default;
  }
});

// node_modules/@cloudflare/unenv-preset/dist/runtime/node/console.mjs
var workerdConsole, assert, clear2, context, count2, countReset2, createTask2, debug2, dir2, dirxml2, error2, group2, groupCollapsed2, groupEnd2, info2, log2, profile2, profileEnd2, table2, time2, timeEnd2, timeLog2, timeStamp2, trace2, warn2, console_default;
var init_console2 = __esm({
  "node_modules/@cloudflare/unenv-preset/dist/runtime/node/console.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_console();
    workerdConsole = globalThis["console"];
    ({
      assert,
      clear: clear2,
      context: (
        // @ts-expect-error undocumented public API
        context
      ),
      count: count2,
      countReset: countReset2,
      createTask: (
        // @ts-expect-error undocumented public API
        createTask2
      ),
      debug: debug2,
      dir: dir2,
      dirxml: dirxml2,
      error: error2,
      group: group2,
      groupCollapsed: groupCollapsed2,
      groupEnd: groupEnd2,
      info: info2,
      log: log2,
      profile: profile2,
      profileEnd: profileEnd2,
      table: table2,
      time: time2,
      timeEnd: timeEnd2,
      timeLog: timeLog2,
      timeStamp: timeStamp2,
      trace: trace2,
      warn: warn2
    } = workerdConsole);
    Object.assign(workerdConsole, {
      Console,
      _ignoreErrors,
      _stderr,
      _stderrErrorHandler,
      _stdout,
      _stdoutErrorHandler,
      _times
    });
    console_default = workerdConsole;
  }
});

// node_modules/wrangler/_virtual_unenv_global_polyfill-@cloudflare-unenv-preset-node-console
var init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console = __esm({
  "node_modules/wrangler/_virtual_unenv_global_polyfill-@cloudflare-unenv-preset-node-console"() {
    init_console2();
    globalThis.console = console_default;
  }
});

// node_modules/unenv/dist/runtime/node/internal/process/hrtime.mjs
var hrtime;
var init_hrtime = __esm({
  "node_modules/unenv/dist/runtime/node/internal/process/hrtime.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    hrtime = /* @__PURE__ */ Object.assign(/* @__PURE__ */ __name(function hrtime2(startTime) {
      const now = Date.now();
      const seconds = Math.trunc(now / 1e3);
      const nanos = now % 1e3 * 1e6;
      if (startTime) {
        let diffSeconds = seconds - startTime[0];
        let diffNanos = nanos - startTime[0];
        if (diffNanos < 0) {
          diffSeconds = diffSeconds - 1;
          diffNanos = 1e9 + diffNanos;
        }
        return [diffSeconds, diffNanos];
      }
      return [seconds, nanos];
    }, "hrtime"), { bigint: /* @__PURE__ */ __name(function bigint() {
      return BigInt(Date.now() * 1e6);
    }, "bigint") });
  }
});

// node_modules/unenv/dist/runtime/node/internal/tty/read-stream.mjs
var ReadStream;
var init_read_stream = __esm({
  "node_modules/unenv/dist/runtime/node/internal/tty/read-stream.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    ReadStream = class {
      static {
        __name(this, "ReadStream");
      }
      fd;
      isRaw = false;
      isTTY = false;
      constructor(fd) {
        this.fd = fd;
      }
      setRawMode(mode) {
        this.isRaw = mode;
        return this;
      }
    };
  }
});

// node_modules/unenv/dist/runtime/node/internal/tty/write-stream.mjs
var WriteStream;
var init_write_stream = __esm({
  "node_modules/unenv/dist/runtime/node/internal/tty/write-stream.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    WriteStream = class {
      static {
        __name(this, "WriteStream");
      }
      fd;
      columns = 80;
      rows = 24;
      isTTY = false;
      constructor(fd) {
        this.fd = fd;
      }
      clearLine(dir3, callback) {
        callback && callback();
        return false;
      }
      clearScreenDown(callback) {
        callback && callback();
        return false;
      }
      cursorTo(x, y, callback) {
        callback && typeof callback === "function" && callback();
        return false;
      }
      moveCursor(dx, dy, callback) {
        callback && callback();
        return false;
      }
      getColorDepth(env3) {
        return 1;
      }
      hasColors(count3, env3) {
        return false;
      }
      getWindowSize() {
        return [this.columns, this.rows];
      }
      write(str, encoding, cb) {
        if (str instanceof Uint8Array) {
          str = new TextDecoder().decode(str);
        }
        try {
          console.log(str);
        } catch {
        }
        cb && typeof cb === "function" && cb();
        return false;
      }
    };
  }
});

// node_modules/unenv/dist/runtime/node/tty.mjs
var init_tty = __esm({
  "node_modules/unenv/dist/runtime/node/tty.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_read_stream();
    init_write_stream();
  }
});

// node_modules/unenv/dist/runtime/node/internal/process/node-version.mjs
var NODE_VERSION;
var init_node_version = __esm({
  "node_modules/unenv/dist/runtime/node/internal/process/node-version.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    NODE_VERSION = "22.14.0";
  }
});

// node_modules/unenv/dist/runtime/node/internal/process/process.mjs
import { EventEmitter } from "node:events";
var Process;
var init_process = __esm({
  "node_modules/unenv/dist/runtime/node/internal/process/process.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_tty();
    init_utils();
    init_node_version();
    Process = class _Process extends EventEmitter {
      static {
        __name(this, "Process");
      }
      env;
      hrtime;
      nextTick;
      constructor(impl) {
        super();
        this.env = impl.env;
        this.hrtime = impl.hrtime;
        this.nextTick = impl.nextTick;
        for (const prop of [...Object.getOwnPropertyNames(_Process.prototype), ...Object.getOwnPropertyNames(EventEmitter.prototype)]) {
          const value = this[prop];
          if (typeof value === "function") {
            this[prop] = value.bind(this);
          }
        }
      }
      // --- event emitter ---
      emitWarning(warning, type, code) {
        console.warn(`${code ? `[${code}] ` : ""}${type ? `${type}: ` : ""}${warning}`);
      }
      emit(...args) {
        return super.emit(...args);
      }
      listeners(eventName) {
        return super.listeners(eventName);
      }
      // --- stdio (lazy initializers) ---
      #stdin;
      #stdout;
      #stderr;
      get stdin() {
        return this.#stdin ??= new ReadStream(0);
      }
      get stdout() {
        return this.#stdout ??= new WriteStream(1);
      }
      get stderr() {
        return this.#stderr ??= new WriteStream(2);
      }
      // --- cwd ---
      #cwd = "/";
      chdir(cwd2) {
        this.#cwd = cwd2;
      }
      cwd() {
        return this.#cwd;
      }
      // --- dummy props and getters ---
      arch = "";
      platform = "";
      argv = [];
      argv0 = "";
      execArgv = [];
      execPath = "";
      title = "";
      pid = 200;
      ppid = 100;
      get version() {
        return `v${NODE_VERSION}`;
      }
      get versions() {
        return { node: NODE_VERSION };
      }
      get allowedNodeEnvironmentFlags() {
        return /* @__PURE__ */ new Set();
      }
      get sourceMapsEnabled() {
        return false;
      }
      get debugPort() {
        return 0;
      }
      get throwDeprecation() {
        return false;
      }
      get traceDeprecation() {
        return false;
      }
      get features() {
        return {};
      }
      get release() {
        return {};
      }
      get connected() {
        return false;
      }
      get config() {
        return {};
      }
      get moduleLoadList() {
        return [];
      }
      constrainedMemory() {
        return 0;
      }
      availableMemory() {
        return 0;
      }
      uptime() {
        return 0;
      }
      resourceUsage() {
        return {};
      }
      // --- noop methods ---
      ref() {
      }
      unref() {
      }
      // --- unimplemented methods ---
      umask() {
        throw createNotImplementedError("process.umask");
      }
      getBuiltinModule() {
        return void 0;
      }
      getActiveResourcesInfo() {
        throw createNotImplementedError("process.getActiveResourcesInfo");
      }
      exit() {
        throw createNotImplementedError("process.exit");
      }
      reallyExit() {
        throw createNotImplementedError("process.reallyExit");
      }
      kill() {
        throw createNotImplementedError("process.kill");
      }
      abort() {
        throw createNotImplementedError("process.abort");
      }
      dlopen() {
        throw createNotImplementedError("process.dlopen");
      }
      setSourceMapsEnabled() {
        throw createNotImplementedError("process.setSourceMapsEnabled");
      }
      loadEnvFile() {
        throw createNotImplementedError("process.loadEnvFile");
      }
      disconnect() {
        throw createNotImplementedError("process.disconnect");
      }
      cpuUsage() {
        throw createNotImplementedError("process.cpuUsage");
      }
      setUncaughtExceptionCaptureCallback() {
        throw createNotImplementedError("process.setUncaughtExceptionCaptureCallback");
      }
      hasUncaughtExceptionCaptureCallback() {
        throw createNotImplementedError("process.hasUncaughtExceptionCaptureCallback");
      }
      initgroups() {
        throw createNotImplementedError("process.initgroups");
      }
      openStdin() {
        throw createNotImplementedError("process.openStdin");
      }
      assert() {
        throw createNotImplementedError("process.assert");
      }
      binding() {
        throw createNotImplementedError("process.binding");
      }
      // --- attached interfaces ---
      permission = { has: /* @__PURE__ */ notImplemented("process.permission.has") };
      report = {
        directory: "",
        filename: "",
        signal: "SIGUSR2",
        compact: false,
        reportOnFatalError: false,
        reportOnSignal: false,
        reportOnUncaughtException: false,
        getReport: /* @__PURE__ */ notImplemented("process.report.getReport"),
        writeReport: /* @__PURE__ */ notImplemented("process.report.writeReport")
      };
      finalization = {
        register: /* @__PURE__ */ notImplemented("process.finalization.register"),
        unregister: /* @__PURE__ */ notImplemented("process.finalization.unregister"),
        registerBeforeExit: /* @__PURE__ */ notImplemented("process.finalization.registerBeforeExit")
      };
      memoryUsage = Object.assign(() => ({
        arrayBuffers: 0,
        rss: 0,
        external: 0,
        heapTotal: 0,
        heapUsed: 0
      }), { rss: /* @__PURE__ */ __name(() => 0, "rss") });
      // --- undefined props ---
      mainModule = void 0;
      domain = void 0;
      // optional
      send = void 0;
      exitCode = void 0;
      channel = void 0;
      getegid = void 0;
      geteuid = void 0;
      getgid = void 0;
      getgroups = void 0;
      getuid = void 0;
      setegid = void 0;
      seteuid = void 0;
      setgid = void 0;
      setgroups = void 0;
      setuid = void 0;
      // internals
      _events = void 0;
      _eventsCount = void 0;
      _exiting = void 0;
      _maxListeners = void 0;
      _debugEnd = void 0;
      _debugProcess = void 0;
      _fatalException = void 0;
      _getActiveHandles = void 0;
      _getActiveRequests = void 0;
      _kill = void 0;
      _preload_modules = void 0;
      _rawDebug = void 0;
      _startProfilerIdleNotifier = void 0;
      _stopProfilerIdleNotifier = void 0;
      _tickCallback = void 0;
      _disconnect = void 0;
      _handleQueue = void 0;
      _pendingMessage = void 0;
      _channel = void 0;
      _send = void 0;
      _linkedBinding = void 0;
    };
  }
});

// node_modules/@cloudflare/unenv-preset/dist/runtime/node/process.mjs
var globalProcess, getBuiltinModule, workerdProcess, isWorkerdProcessV2, unenvProcess, exit, features, platform, env, hrtime3, nextTick, _channel, _disconnect, _events, _eventsCount, _handleQueue, _maxListeners, _pendingMessage, _send, assert2, disconnect, mainModule, _debugEnd, _debugProcess, _exiting, _fatalException, _getActiveHandles, _getActiveRequests, _kill, _linkedBinding, _preload_modules, _rawDebug, _startProfilerIdleNotifier, _stopProfilerIdleNotifier, _tickCallback, abort, addListener, allowedNodeEnvironmentFlags, arch, argv, argv0, availableMemory, binding, channel, chdir, config, connected, constrainedMemory, cpuUsage, cwd, debugPort, dlopen, domain, emit, emitWarning, eventNames, execArgv, execPath, exitCode, finalization, getActiveResourcesInfo, getegid, geteuid, getgid, getgroups, getMaxListeners, getuid, hasUncaughtExceptionCaptureCallback, initgroups, kill, listenerCount, listeners, loadEnvFile, memoryUsage, moduleLoadList, off, on, once, openStdin, permission, pid, ppid, prependListener, prependOnceListener, rawListeners, reallyExit, ref, release, removeAllListeners, removeListener, report, resourceUsage, send, setegid, seteuid, setgid, setgroups, setMaxListeners, setSourceMapsEnabled, setuid, setUncaughtExceptionCaptureCallback, sourceMapsEnabled, stderr, stdin, stdout, throwDeprecation, title, traceDeprecation, umask, unref, uptime, version, versions, _process, process_default;
var init_process2 = __esm({
  "node_modules/@cloudflare/unenv-preset/dist/runtime/node/process.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_hrtime();
    init_process();
    globalProcess = globalThis["process"];
    getBuiltinModule = globalProcess.getBuiltinModule;
    workerdProcess = getBuiltinModule("node:process");
    isWorkerdProcessV2 = globalThis.Cloudflare.compatibilityFlags.enable_nodejs_process_v2;
    unenvProcess = new Process({
      env: globalProcess.env,
      // `hrtime` is only available from workerd process v2
      hrtime: isWorkerdProcessV2 ? workerdProcess.hrtime : hrtime,
      // `nextTick` is available from workerd process v1
      nextTick: workerdProcess.nextTick
    });
    ({ exit, features, platform } = workerdProcess);
    ({
      env: (
        // Always implemented by workerd
        env
      ),
      hrtime: (
        // Only implemented in workerd v2
        hrtime3
      ),
      nextTick: (
        // Always implemented by workerd
        nextTick
      )
    } = unenvProcess);
    ({
      _channel,
      _disconnect,
      _events,
      _eventsCount,
      _handleQueue,
      _maxListeners,
      _pendingMessage,
      _send,
      assert: assert2,
      disconnect,
      mainModule
    } = unenvProcess);
    ({
      _debugEnd: (
        // @ts-expect-error `_debugEnd` is missing typings
        _debugEnd
      ),
      _debugProcess: (
        // @ts-expect-error `_debugProcess` is missing typings
        _debugProcess
      ),
      _exiting: (
        // @ts-expect-error `_exiting` is missing typings
        _exiting
      ),
      _fatalException: (
        // @ts-expect-error `_fatalException` is missing typings
        _fatalException
      ),
      _getActiveHandles: (
        // @ts-expect-error `_getActiveHandles` is missing typings
        _getActiveHandles
      ),
      _getActiveRequests: (
        // @ts-expect-error `_getActiveRequests` is missing typings
        _getActiveRequests
      ),
      _kill: (
        // @ts-expect-error `_kill` is missing typings
        _kill
      ),
      _linkedBinding: (
        // @ts-expect-error `_linkedBinding` is missing typings
        _linkedBinding
      ),
      _preload_modules: (
        // @ts-expect-error `_preload_modules` is missing typings
        _preload_modules
      ),
      _rawDebug: (
        // @ts-expect-error `_rawDebug` is missing typings
        _rawDebug
      ),
      _startProfilerIdleNotifier: (
        // @ts-expect-error `_startProfilerIdleNotifier` is missing typings
        _startProfilerIdleNotifier
      ),
      _stopProfilerIdleNotifier: (
        // @ts-expect-error `_stopProfilerIdleNotifier` is missing typings
        _stopProfilerIdleNotifier
      ),
      _tickCallback: (
        // @ts-expect-error `_tickCallback` is missing typings
        _tickCallback
      ),
      abort,
      addListener,
      allowedNodeEnvironmentFlags,
      arch,
      argv,
      argv0,
      availableMemory,
      binding: (
        // @ts-expect-error `binding` is missing typings
        binding
      ),
      channel,
      chdir,
      config,
      connected,
      constrainedMemory,
      cpuUsage,
      cwd,
      debugPort,
      dlopen,
      domain: (
        // @ts-expect-error `domain` is missing typings
        domain
      ),
      emit,
      emitWarning,
      eventNames,
      execArgv,
      execPath,
      exitCode,
      finalization,
      getActiveResourcesInfo,
      getegid,
      geteuid,
      getgid,
      getgroups,
      getMaxListeners,
      getuid,
      hasUncaughtExceptionCaptureCallback,
      initgroups: (
        // @ts-expect-error `initgroups` is missing typings
        initgroups
      ),
      kill,
      listenerCount,
      listeners,
      loadEnvFile,
      memoryUsage,
      moduleLoadList: (
        // @ts-expect-error `moduleLoadList` is missing typings
        moduleLoadList
      ),
      off,
      on,
      once,
      openStdin: (
        // @ts-expect-error `openStdin` is missing typings
        openStdin
      ),
      permission,
      pid,
      ppid,
      prependListener,
      prependOnceListener,
      rawListeners,
      reallyExit: (
        // @ts-expect-error `reallyExit` is missing typings
        reallyExit
      ),
      ref,
      release,
      removeAllListeners,
      removeListener,
      report,
      resourceUsage,
      send,
      setegid,
      seteuid,
      setgid,
      setgroups,
      setMaxListeners,
      setSourceMapsEnabled,
      setuid,
      setUncaughtExceptionCaptureCallback,
      sourceMapsEnabled,
      stderr,
      stdin,
      stdout,
      throwDeprecation,
      title,
      traceDeprecation,
      umask,
      unref,
      uptime,
      version,
      versions
    } = isWorkerdProcessV2 ? workerdProcess : unenvProcess);
    _process = {
      abort,
      addListener,
      allowedNodeEnvironmentFlags,
      hasUncaughtExceptionCaptureCallback,
      setUncaughtExceptionCaptureCallback,
      loadEnvFile,
      sourceMapsEnabled,
      arch,
      argv,
      argv0,
      chdir,
      config,
      connected,
      constrainedMemory,
      availableMemory,
      cpuUsage,
      cwd,
      debugPort,
      dlopen,
      disconnect,
      emit,
      emitWarning,
      env,
      eventNames,
      execArgv,
      execPath,
      exit,
      finalization,
      features,
      getBuiltinModule,
      getActiveResourcesInfo,
      getMaxListeners,
      hrtime: hrtime3,
      kill,
      listeners,
      listenerCount,
      memoryUsage,
      nextTick,
      on,
      off,
      once,
      pid,
      platform,
      ppid,
      prependListener,
      prependOnceListener,
      rawListeners,
      release,
      removeAllListeners,
      removeListener,
      report,
      resourceUsage,
      setMaxListeners,
      setSourceMapsEnabled,
      stderr,
      stdin,
      stdout,
      title,
      throwDeprecation,
      traceDeprecation,
      umask,
      uptime,
      version,
      versions,
      // @ts-expect-error old API
      domain,
      initgroups,
      moduleLoadList,
      reallyExit,
      openStdin,
      assert: assert2,
      binding,
      send,
      exitCode,
      channel,
      getegid,
      geteuid,
      getgid,
      getgroups,
      getuid,
      setegid,
      seteuid,
      setgid,
      setgroups,
      setuid,
      permission,
      mainModule,
      _events,
      _eventsCount,
      _exiting,
      _maxListeners,
      _debugEnd,
      _debugProcess,
      _fatalException,
      _getActiveHandles,
      _getActiveRequests,
      _kill,
      _preload_modules,
      _rawDebug,
      _startProfilerIdleNotifier,
      _stopProfilerIdleNotifier,
      _tickCallback,
      _disconnect,
      _handleQueue,
      _pendingMessage,
      _channel,
      _send,
      _linkedBinding
    };
    process_default = _process;
  }
});

// node_modules/wrangler/_virtual_unenv_global_polyfill-@cloudflare-unenv-preset-node-process
var init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process = __esm({
  "node_modules/wrangler/_virtual_unenv_global_polyfill-@cloudflare-unenv-preset-node-process"() {
    init_process2();
    globalThis.process = process_default;
  }
});

// node_modules/@sveltejs/kit/src/exports/internal/remote-functions.js
var init_remote_functions = __esm({
  "node_modules/@sveltejs/kit/src/exports/internal/remote-functions.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
  }
});

// node_modules/@sveltejs/kit/src/exports/internal/index.js
var HttpError, Redirect, SvelteKitError, ActionFailure;
var init_internal = __esm({
  "node_modules/@sveltejs/kit/src/exports/internal/index.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_remote_functions();
    HttpError = class {
      static {
        __name(this, "HttpError");
      }
      /**
       * @param {number} status
       * @param {{message: string} extends App.Error ? (App.Error | string | undefined) : App.Error} body
       */
      constructor(status, body2) {
        this.status = status;
        if (typeof body2 === "string") {
          this.body = { message: body2 };
        } else if (body2) {
          this.body = body2;
        } else {
          this.body = { message: `Error: ${status}` };
        }
      }
      toString() {
        return JSON.stringify(this.body);
      }
    };
    Redirect = class {
      static {
        __name(this, "Redirect");
      }
      /**
       * @param {300 | 301 | 302 | 303 | 304 | 305 | 306 | 307 | 308} status
       * @param {string} location
       */
      constructor(status, location) {
        this.status = status;
        this.location = location;
      }
    };
    SvelteKitError = class extends Error {
      static {
        __name(this, "SvelteKitError");
      }
      /**
       * @param {number} status
       * @param {string} text
       * @param {string} message
       */
      constructor(status, text2, message) {
        super(message);
        this.status = status;
        this.text = text2;
      }
    };
    ActionFailure = class {
      static {
        __name(this, "ActionFailure");
      }
      /**
       * @param {number} status
       * @param {T} data
       */
      constructor(status, data) {
        this.status = status;
        this.data = data;
      }
    };
  }
});

// node_modules/esm-env/true.js
var true_default;
var init_true = __esm({
  "node_modules/esm-env/true.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    true_default = true;
  }
});

// node_modules/esm-env/dev-fallback.js
var node_env, dev_fallback_default;
var init_dev_fallback = __esm({
  "node_modules/esm-env/dev-fallback.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    node_env = "development";
    dev_fallback_default = node_env && !node_env.toLowerCase().startsWith("prod");
  }
});

// node_modules/esm-env/false.js
var init_false = __esm({
  "node_modules/esm-env/false.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
  }
});

// node_modules/esm-env/index.js
var init_esm_env = __esm({
  "node_modules/esm-env/index.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_true();
    init_dev_fallback();
    init_false();
  }
});

// node_modules/@sveltejs/kit/src/runtime/pathname.js
var init_pathname = __esm({
  "node_modules/@sveltejs/kit/src/runtime/pathname.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
  }
});

// node_modules/@sveltejs/kit/src/runtime/utils.js
var text_encoder, text_decoder;
var init_utils2 = __esm({
  "node_modules/@sveltejs/kit/src/runtime/utils.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_esm_env();
    text_encoder = new TextEncoder();
    text_decoder = new TextDecoder();
  }
});

// node_modules/@sveltejs/kit/src/version.js
var init_version = __esm({
  "node_modules/@sveltejs/kit/src/version.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
  }
});

// node_modules/@sveltejs/kit/src/exports/index.js
function error3(status, body2) {
  if ((!true_default || dev_fallback_default) && (isNaN(status) || status < 400 || status > 599)) {
    throw new Error(`HTTP error status codes must be between 400 and 599 \u2014 ${status} is invalid`);
  }
  throw new HttpError(status, body2);
}
function json(data, init2) {
  const body2 = JSON.stringify(data);
  const headers2 = new Headers(init2?.headers);
  if (!headers2.has("content-length")) {
    headers2.set("content-length", text_encoder.encode(body2).byteLength.toString());
  }
  if (!headers2.has("content-type")) {
    headers2.set("content-type", "application/json");
  }
  return new Response(body2, {
    ...init2,
    headers: headers2
  });
}
function text(body2, init2) {
  const headers2 = new Headers(init2?.headers);
  if (!headers2.has("content-length")) {
    const encoded = text_encoder.encode(body2);
    headers2.set("content-length", encoded.byteLength.toString());
    return new Response(encoded, {
      ...init2,
      headers: headers2
    });
  }
  return new Response(body2, {
    ...init2,
    headers: headers2
  });
}
var init_exports = __esm({
  "node_modules/@sveltejs/kit/src/exports/index.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_internal();
    init_esm_env();
    init_pathname();
    init_utils2();
    init_version();
    __name(error3, "error");
    __name(json, "json");
    __name(text, "text");
  }
});

// node_modules/@sveltejs/kit/src/runtime/server/constants.js
var IN_WEBCONTAINER;
var init_constants = __esm({
  "node_modules/@sveltejs/kit/src/runtime/server/constants.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    IN_WEBCONTAINER = !!globalThis.process?.versions?.webcontainer;
  }
});

// node_modules/@sveltejs/kit/src/exports/internal/event.js
function with_request_store(store, fn) {
  try {
    sync_store = store;
    return als ? als.run(store, fn) : fn();
  } finally {
    if (!IN_WEBCONTAINER) {
      sync_store = null;
    }
  }
}
var sync_store, als;
var init_event = __esm({
  "node_modules/@sveltejs/kit/src/exports/internal/event.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_constants();
    sync_store = null;
    import("node:async_hooks").then((hooks) => als = new hooks.AsyncLocalStorage()).catch(() => {
    });
    __name(with_request_store, "with_request_store");
  }
});

// node_modules/@sveltejs/kit/src/exports/internal/server.js
function merge_tracing(event_like, current2) {
  return {
    ...event_like,
    tracing: {
      ...event_like.tracing,
      current: current2
    }
  };
}
var init_server = __esm({
  "node_modules/@sveltejs/kit/src/exports/internal/server.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_event();
    __name(merge_tracing, "merge_tracing");
  }
});

// .svelte-kit/output/server/chunks/equality.js
function run_all(arr) {
  for (var i = 0; i < arr.length; i++) {
    arr[i]();
  }
}
function deferred() {
  var resolve2;
  var reject;
  var promise = new Promise((res, rej) => {
    resolve2 = res;
    reject = rej;
  });
  return { promise, resolve: resolve2, reject };
}
function equals(value) {
  return value === this.v;
}
function safe_not_equal(a, b) {
  return a != a ? b == b : a !== b || a !== null && typeof a === "object" || typeof a === "function";
}
function safe_equals(value) {
  return !safe_not_equal(value, this.v);
}
var is_array, index_of, array_from, define_property, get_descriptor, object_prototype, array_prototype, get_prototype_of, is_extensible, noop;
var init_equality = __esm({
  ".svelte-kit/output/server/chunks/equality.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    is_array = Array.isArray;
    index_of = Array.prototype.indexOf;
    array_from = Array.from;
    define_property = Object.defineProperty;
    get_descriptor = Object.getOwnPropertyDescriptor;
    object_prototype = Object.prototype;
    array_prototype = Array.prototype;
    get_prototype_of = Object.getPrototypeOf;
    is_extensible = Object.isExtensible;
    noop = /* @__PURE__ */ __name(() => {
    }, "noop");
    __name(run_all, "run_all");
    __name(deferred, "deferred");
    __name(equals, "equals");
    __name(safe_not_equal, "safe_not_equal");
    __name(safe_equals, "safe_equals");
  }
});

// node_modules/clsx/dist/clsx.mjs
function r(e3) {
  var t2, f, n2 = "";
  if ("string" == typeof e3 || "number" == typeof e3) n2 += e3;
  else if ("object" == typeof e3) if (Array.isArray(e3)) {
    var o2 = e3.length;
    for (t2 = 0; t2 < o2; t2++) e3[t2] && (f = r(e3[t2])) && (n2 && (n2 += " "), n2 += f);
  } else for (f in e3) e3[f] && (n2 && (n2 += " "), n2 += f);
  return n2;
}
function clsx() {
  for (var e3, t2, f = 0, n2 = "", o2 = arguments.length; f < o2; f++) (e3 = arguments[f]) && (t2 = r(e3)) && (n2 && (n2 += " "), n2 += t2);
  return n2;
}
var init_clsx = __esm({
  "node_modules/clsx/dist/clsx.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    __name(r, "r");
    __name(clsx, "clsx");
  }
});

// .svelte-kit/output/server/chunks/exports.js
function resolve(base2, path) {
  if (path[0] === "/" && path[1] === "/") return path;
  let url = new URL(base2, internal);
  url = new URL(path, url);
  return url.protocol === internal.protocol ? url.pathname + url.search + url.hash : url.href;
}
function normalize_path(path, trailing_slash) {
  if (path === "/" || trailing_slash === "ignore") return path;
  if (trailing_slash === "never") {
    return path.endsWith("/") ? path.slice(0, -1) : path;
  } else if (trailing_slash === "always" && !path.endsWith("/")) {
    return path + "/";
  }
  return path;
}
function decode_pathname(pathname) {
  return pathname.split("%25").map(decodeURI).join("%25");
}
function decode_params(params) {
  for (const key2 in params) {
    params[key2] = decodeURIComponent(params[key2]);
  }
  return params;
}
function make_trackable(url, callback, search_params_callback, allow_hash = false) {
  const tracked = new URL(url);
  Object.defineProperty(tracked, "searchParams", {
    value: new Proxy(tracked.searchParams, {
      get(obj, key2) {
        if (key2 === "get" || key2 === "getAll" || key2 === "has") {
          return (param) => {
            search_params_callback(param);
            return obj[key2](param);
          };
        }
        callback();
        const value = Reflect.get(obj, key2);
        return typeof value === "function" ? value.bind(obj) : value;
      }
    }),
    enumerable: true,
    configurable: true
  });
  const tracked_url_properties = ["href", "pathname", "search", "toString", "toJSON"];
  if (allow_hash) tracked_url_properties.push("hash");
  for (const property of tracked_url_properties) {
    Object.defineProperty(tracked, property, {
      get() {
        callback();
        return url[property];
      },
      enumerable: true,
      configurable: true
    });
  }
  {
    tracked[Symbol.for("nodejs.util.inspect.custom")] = (depth, opts, inspect) => {
      return inspect(url, opts);
    };
    tracked.searchParams[Symbol.for("nodejs.util.inspect.custom")] = (depth, opts, inspect) => {
      return inspect(url.searchParams, opts);
    };
  }
  if (!allow_hash) {
    disable_hash(tracked);
  }
  return tracked;
}
function disable_hash(url) {
  allow_nodejs_console_log(url);
  Object.defineProperty(url, "hash", {
    get() {
      throw new Error(
        "Cannot access event.url.hash. Consider using `page.url.hash` inside a component instead"
      );
    }
  });
}
function disable_search(url) {
  allow_nodejs_console_log(url);
  for (const property of ["search", "searchParams"]) {
    Object.defineProperty(url, property, {
      get() {
        throw new Error(`Cannot access url.${property} on a page with prerendering enabled`);
      }
    });
  }
}
function allow_nodejs_console_log(url) {
  {
    url[Symbol.for("nodejs.util.inspect.custom")] = (depth, opts, inspect) => {
      return inspect(new URL(url), opts);
    };
  }
}
function readable(value, start) {
  return {
    subscribe: writable(value, start).subscribe
  };
}
function writable(value, start = noop) {
  let stop = null;
  const subscribers = /* @__PURE__ */ new Set();
  function set2(new_value) {
    if (safe_not_equal(value, new_value)) {
      value = new_value;
      if (stop) {
        const run_queue = !subscriber_queue.length;
        for (const subscriber of subscribers) {
          subscriber[1]();
          subscriber_queue.push(subscriber, value);
        }
        if (run_queue) {
          for (let i = 0; i < subscriber_queue.length; i += 2) {
            subscriber_queue[i][0](subscriber_queue[i + 1]);
          }
          subscriber_queue.length = 0;
        }
      }
    }
  }
  __name(set2, "set");
  function update(fn) {
    set2(fn(
      /** @type {T} */
      value
    ));
  }
  __name(update, "update");
  function subscribe(run, invalidate = noop) {
    const subscriber = [run, invalidate];
    subscribers.add(subscriber);
    if (subscribers.size === 1) {
      stop = start(set2, update) || noop;
    }
    run(
      /** @type {T} */
      value
    );
    return () => {
      subscribers.delete(subscriber);
      if (subscribers.size === 0 && stop) {
        stop();
        stop = null;
      }
    };
  }
  __name(subscribe, "subscribe");
  return { set: set2, update, subscribe };
}
function validator(expected) {
  function validate(module, file) {
    if (!module) return;
    for (const key2 in module) {
      if (key2[0] === "_" || expected.has(key2)) continue;
      const values = [...expected.values()];
      const hint = hint_for_supported_files(key2, file?.slice(file.lastIndexOf("."))) ?? `valid exports are ${values.join(", ")}, or anything with a '_' prefix`;
      throw new Error(`Invalid export '${key2}'${file ? ` in ${file}` : ""} (${hint})`);
    }
  }
  __name(validate, "validate");
  return validate;
}
function hint_for_supported_files(key2, ext = ".js") {
  const supported_files = [];
  if (valid_layout_exports.has(key2)) {
    supported_files.push(`+layout${ext}`);
  }
  if (valid_page_exports.has(key2)) {
    supported_files.push(`+page${ext}`);
  }
  if (valid_layout_server_exports.has(key2)) {
    supported_files.push(`+layout.server${ext}`);
  }
  if (valid_page_server_exports.has(key2)) {
    supported_files.push(`+page.server${ext}`);
  }
  if (valid_server_exports.has(key2)) {
    supported_files.push(`+server${ext}`);
  }
  if (supported_files.length > 0) {
    return `'${key2}' is a valid export in ${supported_files.slice(0, -1).join(", ")}${supported_files.length > 1 ? " or " : ""}${supported_files.at(-1)}`;
  }
}
var internal, subscriber_queue, valid_layout_exports, valid_page_exports, valid_layout_server_exports, valid_page_server_exports, valid_server_exports, validate_layout_exports, validate_page_exports, validate_layout_server_exports, validate_page_server_exports, validate_server_exports;
var init_exports2 = __esm({
  ".svelte-kit/output/server/chunks/exports.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_equality();
    init_clsx();
    internal = new URL("sveltekit-internal://");
    __name(resolve, "resolve");
    __name(normalize_path, "normalize_path");
    __name(decode_pathname, "decode_pathname");
    __name(decode_params, "decode_params");
    __name(make_trackable, "make_trackable");
    __name(disable_hash, "disable_hash");
    __name(disable_search, "disable_search");
    __name(allow_nodejs_console_log, "allow_nodejs_console_log");
    subscriber_queue = [];
    __name(readable, "readable");
    __name(writable, "writable");
    __name(validator, "validator");
    __name(hint_for_supported_files, "hint_for_supported_files");
    valid_layout_exports = /* @__PURE__ */ new Set([
      "load",
      "prerender",
      "csr",
      "ssr",
      "trailingSlash",
      "config"
    ]);
    valid_page_exports = /* @__PURE__ */ new Set([...valid_layout_exports, "entries"]);
    valid_layout_server_exports = /* @__PURE__ */ new Set([...valid_layout_exports]);
    valid_page_server_exports = /* @__PURE__ */ new Set([...valid_layout_server_exports, "actions", "entries"]);
    valid_server_exports = /* @__PURE__ */ new Set([
      "GET",
      "POST",
      "PATCH",
      "PUT",
      "DELETE",
      "OPTIONS",
      "HEAD",
      "fallback",
      "prerender",
      "trailingSlash",
      "config",
      "entries"
    ]);
    validate_layout_exports = validator(valid_layout_exports);
    validate_page_exports = validator(valid_page_exports);
    validate_layout_server_exports = validator(valid_layout_server_exports);
    validate_page_server_exports = validator(valid_page_server_exports);
    validate_server_exports = validator(valid_server_exports);
  }
});

// .svelte-kit/output/server/chunks/utils.js
function get_relative_path(from, to) {
  const from_parts = from.split(/[/\\]/);
  const to_parts = to.split(/[/\\]/);
  from_parts.pop();
  while (from_parts[0] === to_parts[0]) {
    from_parts.shift();
    to_parts.shift();
  }
  let i = from_parts.length;
  while (i--) from_parts[i] = "..";
  return from_parts.concat(to_parts).join("/");
}
function base64_encode(bytes) {
  if (globalThis.Buffer) {
    return globalThis.Buffer.from(bytes).toString("base64");
  }
  let binary = "";
  for (let i = 0; i < bytes.length; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return btoa(binary);
}
function base64_decode(encoded) {
  if (globalThis.Buffer) {
    const buffer = globalThis.Buffer.from(encoded, "base64");
    return new Uint8Array(buffer);
  }
  const binary = atob(encoded);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i);
  }
  return bytes;
}
var text_encoder2, text_decoder2;
var init_utils3 = __esm({
  ".svelte-kit/output/server/chunks/utils.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    text_encoder2 = new TextEncoder();
    text_decoder2 = new TextDecoder();
    __name(get_relative_path, "get_relative_path");
    __name(base64_encode, "base64_encode");
    __name(base64_decode, "base64_decode");
  }
});

// .svelte-kit/output/server/chunks/escaping.js
function escape_html(value, is_attr) {
  const str = String(value ?? "");
  const pattern2 = is_attr ? ATTR_REGEX : CONTENT_REGEX;
  pattern2.lastIndex = 0;
  let escaped2 = "";
  let last = 0;
  while (pattern2.test(str)) {
    const i = pattern2.lastIndex - 1;
    const ch = str[i];
    escaped2 += str.substring(last, i) + (ch === "&" ? "&amp;" : ch === '"' ? "&quot;" : "&lt;");
    last = i + 1;
  }
  return escaped2 + str.substring(last);
}
var ATTR_REGEX, CONTENT_REGEX;
var init_escaping = __esm({
  ".svelte-kit/output/server/chunks/escaping.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    ATTR_REGEX = /[&"<]/g;
    CONTENT_REGEX = /[&<]/g;
    __name(escape_html, "escape_html");
  }
});

// .svelte-kit/output/server/chunks/context.js
function lifecycle_outside_component(name) {
  {
    throw new Error(`https://svelte.dev/e/lifecycle_outside_component`);
  }
}
function set_ssr_context(v) {
  ssr_context = v;
}
function getContext(key2) {
  const context_map = get_or_init_context_map();
  const result = (
    /** @type {T} */
    context_map.get(key2)
  );
  return result;
}
function setContext(key2, context3) {
  get_or_init_context_map().set(key2, context3);
  return context3;
}
function get_or_init_context_map(name) {
  if (ssr_context === null) {
    lifecycle_outside_component();
  }
  return ssr_context.c ??= new Map(get_parent_context(ssr_context) || void 0);
}
function push(fn) {
  ssr_context = { p: ssr_context, c: null, r: null };
}
function pop() {
  ssr_context = /** @type {SSRContext} */
  ssr_context.p;
}
function get_parent_context(ssr_context2) {
  let parent = ssr_context2.p;
  while (parent !== null) {
    const context_map = parent.c;
    if (context_map !== null) {
      return context_map;
    }
    parent = parent.p;
  }
  return null;
}
var ssr_context;
var init_context = __esm({
  ".svelte-kit/output/server/chunks/context.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    __name(lifecycle_outside_component, "lifecycle_outside_component");
    ssr_context = null;
    __name(set_ssr_context, "set_ssr_context");
    __name(getContext, "getContext");
    __name(setContext, "setContext");
    __name(get_or_init_context_map, "get_or_init_context_map");
    __name(push, "push");
    __name(pop, "pop");
    __name(get_parent_context, "get_parent_context");
  }
});

// .svelte-kit/output/server/chunks/index.js
function is_boolean_attribute(name) {
  return DOM_BOOLEAN_ATTRIBUTES.includes(name);
}
function is_passive_event(name) {
  return PASSIVE_EVENTS.includes(name);
}
function attr(name, value, is_boolean = false) {
  if (name === "hidden" && value !== "until-found") {
    is_boolean = true;
  }
  if (value == null || !value && is_boolean) return "";
  const normalized = name in replacements && replacements[name].get(value) || value;
  const assignment = is_boolean ? "" : `="${escape_html(normalized, true)}"`;
  return ` ${name}${assignment}`;
}
function clsx2(value) {
  if (typeof value === "object") {
    return clsx(value);
  } else {
    return value ?? "";
  }
}
function to_class(value, hash2, directives) {
  var classname = value == null ? "" : "" + value;
  if (hash2) {
    classname = classname ? classname + " " + hash2 : hash2;
  }
  if (directives) {
    for (var key2 in directives) {
      if (directives[key2]) {
        classname = classname ? classname + " " + key2 : key2;
      } else if (classname.length) {
        var len = key2.length;
        var a = 0;
        while ((a = classname.indexOf(key2, a)) >= 0) {
          var b = a + len;
          if ((a === 0 || whitespace.includes(classname[a - 1])) && (b === classname.length || whitespace.includes(classname[b]))) {
            classname = (a === 0 ? "" : classname.substring(0, a)) + classname.substring(b + 1);
          } else {
            a = b;
          }
        }
      }
    }
  }
  return classname === "" ? null : classname;
}
function append_styles(styles, important = false) {
  var separator = important ? " !important;" : ";";
  var css = "";
  for (var key2 in styles) {
    var value = styles[key2];
    if (value != null && value !== "") {
      css += " " + key2 + ": " + value + separator;
    }
  }
  return css;
}
function to_css_name(name) {
  if (name[0] !== "-" || name[1] !== "-") {
    return name.toLowerCase();
  }
  return name;
}
function to_style(value, styles) {
  if (styles) {
    var new_style = "";
    var normal_styles;
    var important_styles;
    if (Array.isArray(styles)) {
      normal_styles = styles[0];
      important_styles = styles[1];
    } else {
      normal_styles = styles;
    }
    if (value) {
      value = String(value).replaceAll(/\s*\/\*.*?\*\/\s*/g, "").trim();
      var in_str = false;
      var in_apo = 0;
      var in_comment = false;
      var reserved_names = [];
      if (normal_styles) {
        reserved_names.push(...Object.keys(normal_styles).map(to_css_name));
      }
      if (important_styles) {
        reserved_names.push(...Object.keys(important_styles).map(to_css_name));
      }
      var start_index = 0;
      var name_index = -1;
      const len = value.length;
      for (var i = 0; i < len; i++) {
        var c2 = value[i];
        if (in_comment) {
          if (c2 === "/" && value[i - 1] === "*") {
            in_comment = false;
          }
        } else if (in_str) {
          if (in_str === c2) {
            in_str = false;
          }
        } else if (c2 === "/" && value[i + 1] === "*") {
          in_comment = true;
        } else if (c2 === '"' || c2 === "'") {
          in_str = c2;
        } else if (c2 === "(") {
          in_apo++;
        } else if (c2 === ")") {
          in_apo--;
        }
        if (!in_comment && in_str === false && in_apo === 0) {
          if (c2 === ":" && name_index === -1) {
            name_index = i;
          } else if (c2 === ";" || i === len - 1) {
            if (name_index !== -1) {
              var name = to_css_name(value.substring(start_index, name_index).trim());
              if (!reserved_names.includes(name)) {
                if (c2 !== ";") {
                  i++;
                }
                var property = value.substring(start_index, i).trim();
                new_style += " " + property + ";";
              }
            }
            start_index = i + 1;
            name_index = -1;
          }
        }
      }
    }
    if (normal_styles) {
      new_style += append_styles(normal_styles);
    }
    if (important_styles) {
      new_style += append_styles(important_styles, true);
    }
    new_style = new_style.trim();
    return new_style === "" ? null : new_style;
  }
  return value == null ? null : String(value);
}
function abort2() {
  controller?.abort(STALE_REACTION);
  controller = null;
}
function await_invalid() {
  const error4 = new Error(`await_invalid
Encountered asynchronous work while rendering synchronously.
https://svelte.dev/e/await_invalid`);
  error4.name = "Svelte error";
  throw error4;
}
function render(component5, options2 = {}) {
  return Renderer.render(
    /** @type {Component<Props>} */
    component5,
    options2
  );
}
function head(hash2, renderer, fn) {
  renderer.head((renderer2) => {
    renderer2.push(`<!--${hash2}-->`);
    renderer2.child(fn);
    renderer2.push(EMPTY_COMMENT);
  });
}
function attributes(attrs, css_hash, classes, styles, flags2 = 0) {
  if (styles) {
    attrs.style = to_style(attrs.style, styles);
  }
  if (attrs.class) {
    attrs.class = clsx2(attrs.class);
  }
  if (css_hash || classes) {
    attrs.class = to_class(attrs.class, css_hash, classes);
  }
  let attr_str = "";
  let name;
  const is_html = (flags2 & ELEMENT_IS_NAMESPACED) === 0;
  const lowercase = (flags2 & ELEMENT_PRESERVE_ATTRIBUTE_CASE) === 0;
  const is_input = (flags2 & ELEMENT_IS_INPUT) !== 0;
  for (name in attrs) {
    if (typeof attrs[name] === "function") continue;
    if (name[0] === "$" && name[1] === "$") continue;
    if (INVALID_ATTR_NAME_CHAR_REGEX.test(name)) continue;
    var value = attrs[name];
    if (lowercase) {
      name = name.toLowerCase();
    }
    if (is_input) {
      if (name === "defaultvalue" || name === "defaultchecked") {
        name = name === "defaultvalue" ? "value" : "checked";
        if (attrs[name]) continue;
      }
    }
    attr_str += attr(name, value, is_html && is_boolean_attribute(name));
  }
  return attr_str;
}
function slot(renderer, $$props, name, slot_props, fallback_fn) {
  var slot_fn = $$props.$$slots?.[name];
  if (slot_fn === true) {
    slot_fn = $$props["children"];
  }
  if (slot_fn !== void 0) {
    slot_fn(renderer, slot_props);
  }
}
var DERIVED, EFFECT, RENDER_EFFECT, BLOCK_EFFECT, BRANCH_EFFECT, ROOT_EFFECT, BOUNDARY_EFFECT, CONNECTED, CLEAN, DIRTY, MAYBE_DIRTY, INERT, DESTROYED, EFFECT_RAN, EFFECT_TRANSPARENT, EAGER_EFFECT, HEAD_EFFECT, EFFECT_PRESERVED, USER_EFFECT, WAS_MARKED, REACTION_IS_UPDATING, ASYNC, ERROR_VALUE, STATE_SYMBOL, LEGACY_PROPS, STALE_REACTION, COMMENT_NODE, HYDRATION_START, HYDRATION_START_ELSE, HYDRATION_END, HYDRATION_ERROR, ELEMENT_IS_NAMESPACED, ELEMENT_PRESERVE_ATTRIBUTE_CASE, ELEMENT_IS_INPUT, UNINITIALIZED, DOM_BOOLEAN_ATTRIBUTES, PASSIVE_EVENTS, replacements, whitespace, BLOCK_OPEN, BLOCK_CLOSE, EMPTY_COMMENT, controller, Renderer, SSRState, INVALID_ATTR_NAME_CHAR_REGEX;
var init_chunks = __esm({
  ".svelte-kit/output/server/chunks/index.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_escaping();
    init_clsx();
    init_context();
    DERIVED = 1 << 1;
    EFFECT = 1 << 2;
    RENDER_EFFECT = 1 << 3;
    BLOCK_EFFECT = 1 << 4;
    BRANCH_EFFECT = 1 << 5;
    ROOT_EFFECT = 1 << 6;
    BOUNDARY_EFFECT = 1 << 7;
    CONNECTED = 1 << 9;
    CLEAN = 1 << 10;
    DIRTY = 1 << 11;
    MAYBE_DIRTY = 1 << 12;
    INERT = 1 << 13;
    DESTROYED = 1 << 14;
    EFFECT_RAN = 1 << 15;
    EFFECT_TRANSPARENT = 1 << 16;
    EAGER_EFFECT = 1 << 17;
    HEAD_EFFECT = 1 << 18;
    EFFECT_PRESERVED = 1 << 19;
    USER_EFFECT = 1 << 20;
    WAS_MARKED = 1 << 15;
    REACTION_IS_UPDATING = 1 << 21;
    ASYNC = 1 << 22;
    ERROR_VALUE = 1 << 23;
    STATE_SYMBOL = Symbol("$state");
    LEGACY_PROPS = Symbol("legacy props");
    STALE_REACTION = new class StaleReactionError extends Error {
      static {
        __name(this, "StaleReactionError");
      }
      name = "StaleReactionError";
      message = "The reaction that called `getAbortSignal()` was re-run or destroyed";
    }();
    COMMENT_NODE = 8;
    HYDRATION_START = "[";
    HYDRATION_START_ELSE = "[!";
    HYDRATION_END = "]";
    HYDRATION_ERROR = {};
    ELEMENT_IS_NAMESPACED = 1;
    ELEMENT_PRESERVE_ATTRIBUTE_CASE = 1 << 1;
    ELEMENT_IS_INPUT = 1 << 2;
    UNINITIALIZED = Symbol();
    DOM_BOOLEAN_ATTRIBUTES = [
      "allowfullscreen",
      "async",
      "autofocus",
      "autoplay",
      "checked",
      "controls",
      "default",
      "disabled",
      "formnovalidate",
      "indeterminate",
      "inert",
      "ismap",
      "loop",
      "multiple",
      "muted",
      "nomodule",
      "novalidate",
      "open",
      "playsinline",
      "readonly",
      "required",
      "reversed",
      "seamless",
      "selected",
      "webkitdirectory",
      "defer",
      "disablepictureinpicture",
      "disableremoteplayback"
    ];
    __name(is_boolean_attribute, "is_boolean_attribute");
    PASSIVE_EVENTS = ["touchstart", "touchmove"];
    __name(is_passive_event, "is_passive_event");
    replacements = {
      translate: /* @__PURE__ */ new Map([
        [true, "yes"],
        [false, "no"]
      ])
    };
    __name(attr, "attr");
    __name(clsx2, "clsx");
    whitespace = [..." 	\n\r\f\xA0\v\uFEFF"];
    __name(to_class, "to_class");
    __name(append_styles, "append_styles");
    __name(to_css_name, "to_css_name");
    __name(to_style, "to_style");
    BLOCK_OPEN = `<!--${HYDRATION_START}-->`;
    BLOCK_CLOSE = `<!--${HYDRATION_END}-->`;
    EMPTY_COMMENT = `<!---->`;
    controller = null;
    __name(abort2, "abort");
    __name(await_invalid, "await_invalid");
    Renderer = class _Renderer {
      static {
        __name(this, "Renderer");
      }
      /**
       * The contents of the renderer.
       * @type {RendererItem[]}
       */
      #out = [];
      /**
       * Any `onDestroy` callbacks registered during execution of this renderer.
       * @type {(() => void)[] | undefined}
       */
      #on_destroy = void 0;
      /**
       * Whether this renderer is a component body.
       * @type {boolean}
       */
      #is_component_body = false;
      /**
       * The type of string content that this renderer is accumulating.
       * @type {RendererType}
       */
      type;
      /** @type {Renderer | undefined} */
      #parent;
      /**
       * Asynchronous work associated with this renderer
       * @type {Promise<void> | undefined}
       */
      promise = void 0;
      /**
       * State which is associated with the content tree as a whole.
       * It will be re-exposed, uncopied, on all children.
       * @type {SSRState}
       * @readonly
       */
      global;
      /**
       * State that is local to the branch it is declared in.
       * It will be shallow-copied to all children.
       *
       * @type {{ select_value: string | undefined }}
       */
      local;
      /**
       * @param {SSRState} global
       * @param {Renderer | undefined} [parent]
       */
      constructor(global, parent) {
        this.#parent = parent;
        this.global = global;
        this.local = parent ? { ...parent.local } : { select_value: void 0 };
        this.type = parent ? parent.type : "body";
      }
      /**
       * @param {(renderer: Renderer) => void} fn
       */
      head(fn) {
        const head2 = new _Renderer(this.global, this);
        head2.type = "head";
        this.#out.push(head2);
        head2.child(fn);
      }
      /**
       * @param {Array<Promise<void>>} blockers
       * @param {(renderer: Renderer) => void} fn
       */
      async_block(blockers, fn) {
        this.#out.push(BLOCK_OPEN);
        this.async(blockers, fn);
        this.#out.push(BLOCK_CLOSE);
      }
      /**
       * @param {Array<Promise<void>>} blockers
       * @param {(renderer: Renderer) => void} fn
       */
      async(blockers, fn) {
        let callback = fn;
        if (blockers.length > 0) {
          const context3 = ssr_context;
          callback = /* @__PURE__ */ __name((renderer) => {
            return Promise.all(blockers).then(() => {
              const previous_context = ssr_context;
              try {
                set_ssr_context(context3);
                return fn(renderer);
              } finally {
                set_ssr_context(previous_context);
              }
            });
          }, "callback");
        }
        this.child(callback);
      }
      /**
       * @param {Array<() => void>} thunks
       */
      run(thunks) {
        const context3 = ssr_context;
        let promise = Promise.resolve(thunks[0]());
        const promises = [promise];
        for (const fn of thunks.slice(1)) {
          promise = promise.then(() => {
            const previous_context = ssr_context;
            set_ssr_context(context3);
            try {
              return fn();
            } finally {
              set_ssr_context(previous_context);
            }
          });
          promises.push(promise);
        }
        return promises;
      }
      /**
       * Create a child renderer. The child renderer inherits the state from the parent,
       * but has its own content.
       * @param {(renderer: Renderer) => MaybePromise<void>} fn
       */
      child(fn) {
        const child = new _Renderer(this.global, this);
        this.#out.push(child);
        const parent = ssr_context;
        set_ssr_context({
          ...ssr_context,
          p: parent,
          c: null,
          r: child
        });
        const result = fn(child);
        set_ssr_context(parent);
        if (result instanceof Promise) {
          if (child.global.mode === "sync") {
            await_invalid();
          }
          result.catch(() => {
          });
          child.promise = result;
        }
        return child;
      }
      /**
       * Create a component renderer. The component renderer inherits the state from the parent,
       * but has its own content. It is treated as an ordering boundary for ondestroy callbacks.
       * @param {(renderer: Renderer) => MaybePromise<void>} fn
       * @param {Function} [component_fn]
       * @returns {void}
       */
      component(fn, component_fn) {
        push();
        const child = this.child(fn);
        child.#is_component_body = true;
        pop();
      }
      /**
       * @param {Record<string, any>} attrs
       * @param {(renderer: Renderer) => void} fn
       * @param {string | undefined} [css_hash]
       * @param {Record<string, boolean> | undefined} [classes]
       * @param {Record<string, string> | undefined} [styles]
       * @param {number | undefined} [flags]
       * @returns {void}
       */
      select(attrs, fn, css_hash, classes, styles, flags2) {
        const { value, ...select_attrs } = attrs;
        this.push(`<select${attributes(select_attrs, css_hash, classes, styles, flags2)}>`);
        this.child((renderer) => {
          renderer.local.select_value = value;
          fn(renderer);
        });
        this.push("</select>");
      }
      /**
       * @param {Record<string, any>} attrs
       * @param {string | number | boolean | ((renderer: Renderer) => void)} body
       * @param {string | undefined} [css_hash]
       * @param {Record<string, boolean> | undefined} [classes]
       * @param {Record<string, string> | undefined} [styles]
       * @param {number | undefined} [flags]
       */
      option(attrs, body2, css_hash, classes, styles, flags2) {
        this.#out.push(`<option${attributes(attrs, css_hash, classes, styles, flags2)}`);
        const close = /* @__PURE__ */ __name((renderer, value, { head: head2, body: body22 }) => {
          if ("value" in attrs) {
            value = attrs.value;
          }
          if (value === this.local.select_value) {
            renderer.#out.push(" selected");
          }
          renderer.#out.push(`>${body22}</option>`);
          if (head2) {
            renderer.head((child) => child.push(head2));
          }
        }, "close");
        if (typeof body2 === "function") {
          this.child((renderer) => {
            const r3 = new _Renderer(this.global, this);
            body2(r3);
            if (this.global.mode === "async") {
              return r3.#collect_content_async().then((content) => {
                close(renderer, content.body.replaceAll("<!---->", ""), content);
              });
            } else {
              const content = r3.#collect_content();
              close(renderer, content.body.replaceAll("<!---->", ""), content);
            }
          });
        } else {
          close(this, body2, { body: body2 });
        }
      }
      /**
       * @param {(renderer: Renderer) => void} fn
       */
      title(fn) {
        const path = this.get_path();
        const close = /* @__PURE__ */ __name((head2) => {
          this.global.set_title(head2, path);
        }, "close");
        this.child((renderer) => {
          const r3 = new _Renderer(renderer.global, renderer);
          fn(r3);
          if (renderer.global.mode === "async") {
            return r3.#collect_content_async().then((content) => {
              close(content.head);
            });
          } else {
            const content = r3.#collect_content();
            close(content.head);
          }
        });
      }
      /**
       * @param {string | (() => Promise<string>)} content
       */
      push(content) {
        if (typeof content === "function") {
          this.child(async (renderer) => renderer.push(await content()));
        } else {
          this.#out.push(content);
        }
      }
      /**
       * @param {() => void} fn
       */
      on_destroy(fn) {
        (this.#on_destroy ??= []).push(fn);
      }
      /**
       * @returns {number[]}
       */
      get_path() {
        return this.#parent ? [...this.#parent.get_path(), this.#parent.#out.indexOf(this)] : [];
      }
      /**
       * @deprecated this is needed for legacy component bindings
       */
      copy() {
        const copy = new _Renderer(this.global, this.#parent);
        copy.#out = this.#out.map((item) => item instanceof _Renderer ? item.copy() : item);
        copy.promise = this.promise;
        return copy;
      }
      /**
       * @param {Renderer} other
       * @deprecated this is needed for legacy component bindings
       */
      subsume(other) {
        if (this.global.mode !== other.global.mode) {
          throw new Error(
            "invariant: A renderer cannot switch modes. If you're seeing this, there's a compiler bug. File an issue!"
          );
        }
        this.local = other.local;
        this.#out = other.#out.map((item) => {
          if (item instanceof _Renderer) {
            item.subsume(item);
          }
          return item;
        });
        this.promise = other.promise;
        this.type = other.type;
      }
      get length() {
        return this.#out.length;
      }
      /**
       * Only available on the server and when compiling with the `server` option.
       * Takes a component and returns an object with `body` and `head` properties on it, which you can use to populate the HTML when server-rendering your app.
       * @template {Record<string, any>} Props
       * @param {Component<Props>} component
       * @param {{ props?: Omit<Props, '$$slots' | '$$events'>; context?: Map<any, any>; idPrefix?: string }} [options]
       * @returns {RenderOutput}
       */
      static render(component5, options2 = {}) {
        let sync;
        const result = (
          /** @type {RenderOutput} */
          {}
        );
        Object.defineProperties(result, {
          html: {
            get: /* @__PURE__ */ __name(() => {
              return (sync ??= _Renderer.#render(component5, options2)).body;
            }, "get")
          },
          head: {
            get: /* @__PURE__ */ __name(() => {
              return (sync ??= _Renderer.#render(component5, options2)).head;
            }, "get")
          },
          body: {
            get: /* @__PURE__ */ __name(() => {
              return (sync ??= _Renderer.#render(component5, options2)).body;
            }, "get")
          },
          then: {
            value: (
              /**
               * this is not type-safe, but honestly it's the best I can do right now, and it's a straightforward function.
               *
               * @template TResult1
               * @template [TResult2=never]
               * @param { (value: SyncRenderOutput) => TResult1 } onfulfilled
               * @param { (reason: unknown) => TResult2 } onrejected
               */
              /* @__PURE__ */ __name((onfulfilled, onrejected) => {
                {
                  const result2 = sync ??= _Renderer.#render(component5, options2);
                  const user_result = onfulfilled({
                    head: result2.head,
                    body: result2.body,
                    html: result2.body
                  });
                  return Promise.resolve(user_result);
                }
              }, "value")
            )
          }
        });
        return result;
      }
      /**
       * Collect all of the `onDestroy` callbacks registered during rendering. In an async context, this is only safe to call
       * after awaiting `collect_async`.
       *
       * Child renderers are "porous" and don't affect execution order, but component body renderers
       * create ordering boundaries. Within a renderer, callbacks run in order until hitting a component boundary.
       * @returns {Iterable<() => void>}
       */
      *#collect_on_destroy() {
        for (const component5 of this.#traverse_components()) {
          yield* component5.#collect_ondestroy();
        }
      }
      /**
       * Performs a depth-first search of renderers, yielding the deepest components first, then additional components as we backtrack up the tree.
       * @returns {Iterable<Renderer>}
       */
      *#traverse_components() {
        for (const child of this.#out) {
          if (typeof child !== "string") {
            yield* child.#traverse_components();
          }
        }
        if (this.#is_component_body) {
          yield this;
        }
      }
      /**
       * @returns {Iterable<() => void>}
       */
      *#collect_ondestroy() {
        if (this.#on_destroy) {
          for (const fn of this.#on_destroy) {
            yield fn;
          }
        }
        for (const child of this.#out) {
          if (child instanceof _Renderer && !child.#is_component_body) {
            yield* child.#collect_ondestroy();
          }
        }
      }
      /**
       * Render a component. Throws if any of the children are performing asynchronous work.
       *
       * @template {Record<string, any>} Props
       * @param {Component<Props>} component
       * @param {{ props?: Omit<Props, '$$slots' | '$$events'>; context?: Map<any, any>; idPrefix?: string }} options
       * @returns {AccumulatedContent}
       */
      static #render(component5, options2) {
        var previous_context = ssr_context;
        try {
          const renderer = _Renderer.#open_render("sync", component5, options2);
          const content = renderer.#collect_content();
          return _Renderer.#close_render(content, renderer);
        } finally {
          abort2();
          set_ssr_context(previous_context);
        }
      }
      /**
       * Render a component.
       *
       * @template {Record<string, any>} Props
       * @param {Component<Props>} component
       * @param {{ props?: Omit<Props, '$$slots' | '$$events'>; context?: Map<any, any>; idPrefix?: string }} options
       * @returns {Promise<AccumulatedContent>}
       */
      static async #render_async(component5, options2) {
        var previous_context = ssr_context;
        try {
          const renderer = _Renderer.#open_render("async", component5, options2);
          const content = await renderer.#collect_content_async();
          return _Renderer.#close_render(content, renderer);
        } finally {
          abort2();
          set_ssr_context(previous_context);
        }
      }
      /**
       * Collect all of the code from the `out` array and return it as a string, or a promise resolving to a string.
       * @param {AccumulatedContent} content
       * @returns {AccumulatedContent}
       */
      #collect_content(content = { head: "", body: "" }) {
        for (const item of this.#out) {
          if (typeof item === "string") {
            content[this.type] += item;
          } else if (item instanceof _Renderer) {
            item.#collect_content(content);
          }
        }
        return content;
      }
      /**
       * Collect all of the code from the `out` array and return it as a string.
       * @param {AccumulatedContent} content
       * @returns {Promise<AccumulatedContent>}
       */
      async #collect_content_async(content = { head: "", body: "" }) {
        await this.promise;
        for (const item of this.#out) {
          if (typeof item === "string") {
            content[this.type] += item;
          } else if (item instanceof _Renderer) {
            await item.#collect_content_async(content);
          }
        }
        return content;
      }
      /**
       * @template {Record<string, any>} Props
       * @param {'sync' | 'async'} mode
       * @param {import('svelte').Component<Props>} component
       * @param {{ props?: Omit<Props, '$$slots' | '$$events'>; context?: Map<any, any>; idPrefix?: string }} options
       * @returns {Renderer}
       */
      static #open_render(mode, component5, options2) {
        const renderer = new _Renderer(
          new SSRState(mode, options2.idPrefix ? options2.idPrefix + "-" : "")
        );
        renderer.push(BLOCK_OPEN);
        if (options2.context) {
          push();
          ssr_context.c = options2.context;
          ssr_context.r = renderer;
        }
        component5(renderer, options2.props ?? {});
        if (options2.context) {
          pop();
        }
        renderer.push(BLOCK_CLOSE);
        return renderer;
      }
      /**
       * @param {AccumulatedContent} content
       * @param {Renderer} renderer
       */
      static #close_render(content, renderer) {
        for (const cleanup of renderer.#collect_on_destroy()) {
          cleanup();
        }
        let head2 = content.head + renderer.global.get_title();
        let body2 = content.body;
        for (const { hash: hash2, code } of renderer.global.css) {
          head2 += `<style id="${hash2}">${code}</style>`;
        }
        return {
          head: head2,
          body: body2
        };
      }
    };
    SSRState = class {
      static {
        __name(this, "SSRState");
      }
      /** @readonly @type {'sync' | 'async'} */
      mode;
      /** @readonly @type {() => string} */
      uid;
      /** @readonly @type {Set<{ hash: string; code: string }>} */
      css = /* @__PURE__ */ new Set();
      /** @type {{ path: number[], value: string }} */
      #title = { path: [], value: "" };
      /**
       * @param {'sync' | 'async'} mode
       * @param {string} [id_prefix]
       */
      constructor(mode, id_prefix = "") {
        this.mode = mode;
        let uid = 1;
        this.uid = () => `${id_prefix}s${uid++}`;
      }
      get_title() {
        return this.#title.value;
      }
      /**
       * Performs a depth-first (lexicographic) comparison using the path. Rejects sets
       * from earlier than or equal to the current value.
       * @param {string} value
       * @param {number[]} path
       */
      set_title(value, path) {
        const current2 = this.#title.path;
        let i = 0;
        let l = Math.min(path.length, current2.length);
        while (i < l && path[i] === current2[i]) i += 1;
        if (path[i] === void 0) return;
        if (current2[i] === void 0 || path[i] > current2[i]) {
          this.#title.path = path;
          this.#title.value = value;
        }
      }
    };
    INVALID_ATTR_NAME_CHAR_REGEX = /[\s'">/=\u{FDD0}-\u{FDEF}\u{FFFE}\u{FFFF}\u{1FFFE}\u{1FFFF}\u{2FFFE}\u{2FFFF}\u{3FFFE}\u{3FFFF}\u{4FFFE}\u{4FFFF}\u{5FFFE}\u{5FFFF}\u{6FFFE}\u{6FFFF}\u{7FFFE}\u{7FFFF}\u{8FFFE}\u{8FFFF}\u{9FFFE}\u{9FFFF}\u{AFFFE}\u{AFFFF}\u{BFFFE}\u{BFFFF}\u{CFFFE}\u{CFFFF}\u{DFFFE}\u{DFFFF}\u{EFFFE}\u{EFFFF}\u{FFFFE}\u{FFFFF}\u{10FFFE}\u{10FFFF}]/u;
    __name(render, "render");
    __name(head, "head");
    __name(attributes, "attributes");
    __name(slot, "slot");
  }
});

// node_modules/cookie/index.js
var require_cookie = __commonJS({
  "node_modules/cookie/index.js"(exports) {
    "use strict";
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    exports.parse = parse3;
    exports.serialize = serialize2;
    var __toString = Object.prototype.toString;
    var fieldContentRegExp = /^[\u0009\u0020-\u007e\u0080-\u00ff]+$/;
    function parse3(str, options2) {
      if (typeof str !== "string") {
        throw new TypeError("argument str must be a string");
      }
      var obj = {};
      var opt = options2 || {};
      var dec = opt.decode || decode;
      var index5 = 0;
      while (index5 < str.length) {
        var eqIdx = str.indexOf("=", index5);
        if (eqIdx === -1) {
          break;
        }
        var endIdx = str.indexOf(";", index5);
        if (endIdx === -1) {
          endIdx = str.length;
        } else if (endIdx < eqIdx) {
          index5 = str.lastIndexOf(";", eqIdx - 1) + 1;
          continue;
        }
        var key2 = str.slice(index5, eqIdx).trim();
        if (void 0 === obj[key2]) {
          var val = str.slice(eqIdx + 1, endIdx).trim();
          if (val.charCodeAt(0) === 34) {
            val = val.slice(1, -1);
          }
          obj[key2] = tryDecode(val, dec);
        }
        index5 = endIdx + 1;
      }
      return obj;
    }
    __name(parse3, "parse");
    function serialize2(name, val, options2) {
      var opt = options2 || {};
      var enc = opt.encode || encode2;
      if (typeof enc !== "function") {
        throw new TypeError("option encode is invalid");
      }
      if (!fieldContentRegExp.test(name)) {
        throw new TypeError("argument name is invalid");
      }
      var value = enc(val);
      if (value && !fieldContentRegExp.test(value)) {
        throw new TypeError("argument val is invalid");
      }
      var str = name + "=" + value;
      if (null != opt.maxAge) {
        var maxAge = opt.maxAge - 0;
        if (isNaN(maxAge) || !isFinite(maxAge)) {
          throw new TypeError("option maxAge is invalid");
        }
        str += "; Max-Age=" + Math.floor(maxAge);
      }
      if (opt.domain) {
        if (!fieldContentRegExp.test(opt.domain)) {
          throw new TypeError("option domain is invalid");
        }
        str += "; Domain=" + opt.domain;
      }
      if (opt.path) {
        if (!fieldContentRegExp.test(opt.path)) {
          throw new TypeError("option path is invalid");
        }
        str += "; Path=" + opt.path;
      }
      if (opt.expires) {
        var expires = opt.expires;
        if (!isDate(expires) || isNaN(expires.valueOf())) {
          throw new TypeError("option expires is invalid");
        }
        str += "; Expires=" + expires.toUTCString();
      }
      if (opt.httpOnly) {
        str += "; HttpOnly";
      }
      if (opt.secure) {
        str += "; Secure";
      }
      if (opt.partitioned) {
        str += "; Partitioned";
      }
      if (opt.priority) {
        var priority = typeof opt.priority === "string" ? opt.priority.toLowerCase() : opt.priority;
        switch (priority) {
          case "low":
            str += "; Priority=Low";
            break;
          case "medium":
            str += "; Priority=Medium";
            break;
          case "high":
            str += "; Priority=High";
            break;
          default:
            throw new TypeError("option priority is invalid");
        }
      }
      if (opt.sameSite) {
        var sameSite = typeof opt.sameSite === "string" ? opt.sameSite.toLowerCase() : opt.sameSite;
        switch (sameSite) {
          case true:
            str += "; SameSite=Strict";
            break;
          case "lax":
            str += "; SameSite=Lax";
            break;
          case "strict":
            str += "; SameSite=Strict";
            break;
          case "none":
            str += "; SameSite=None";
            break;
          default:
            throw new TypeError("option sameSite is invalid");
        }
      }
      return str;
    }
    __name(serialize2, "serialize");
    function decode(str) {
      return str.indexOf("%") !== -1 ? decodeURIComponent(str) : str;
    }
    __name(decode, "decode");
    function encode2(val) {
      return encodeURIComponent(val);
    }
    __name(encode2, "encode");
    function isDate(val) {
      return __toString.call(val) === "[object Date]" || val instanceof Date;
    }
    __name(isDate, "isDate");
    function tryDecode(str, decode2) {
      try {
        return decode2(str);
      } catch (e3) {
        return str;
      }
    }
    __name(tryDecode, "tryDecode");
  }
});

// node_modules/set-cookie-parser/lib/set-cookie.js
var require_set_cookie = __commonJS({
  "node_modules/set-cookie-parser/lib/set-cookie.js"(exports, module) {
    "use strict";
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    var defaultParseOptions = {
      decodeValues: true,
      map: false,
      silent: false
    };
    function isForbiddenKey(key2) {
      return typeof key2 !== "string" || key2 in {};
    }
    __name(isForbiddenKey, "isForbiddenKey");
    function createNullObj() {
      return /* @__PURE__ */ Object.create(null);
    }
    __name(createNullObj, "createNullObj");
    function isNonEmptyString(str) {
      return typeof str === "string" && !!str.trim();
    }
    __name(isNonEmptyString, "isNonEmptyString");
    function parseString2(setCookieValue, options2) {
      var parts = setCookieValue.split(";").filter(isNonEmptyString);
      var nameValuePairStr = parts.shift();
      var parsed = parseNameValuePair(nameValuePairStr);
      var name = parsed.name;
      var value = parsed.value;
      options2 = options2 ? Object.assign({}, defaultParseOptions, options2) : defaultParseOptions;
      if (isForbiddenKey(name)) {
        return null;
      }
      try {
        value = options2.decodeValues ? decodeURIComponent(value) : value;
      } catch (e3) {
        console.error(
          "set-cookie-parser: failed to decode cookie value. Set options.decodeValues=false to disable decoding.",
          e3
        );
      }
      var cookie = createNullObj();
      cookie.name = name;
      cookie.value = value;
      parts.forEach(function(part) {
        var sides = part.split("=");
        var key2 = sides.shift().trimLeft().toLowerCase();
        if (isForbiddenKey(key2)) {
          return;
        }
        var value2 = sides.join("=");
        if (key2 === "expires") {
          cookie.expires = new Date(value2);
        } else if (key2 === "max-age") {
          var n2 = parseInt(value2, 10);
          if (!Number.isNaN(n2)) cookie.maxAge = n2;
        } else if (key2 === "secure") {
          cookie.secure = true;
        } else if (key2 === "httponly") {
          cookie.httpOnly = true;
        } else if (key2 === "samesite") {
          cookie.sameSite = value2;
        } else if (key2 === "partitioned") {
          cookie.partitioned = true;
        } else if (key2) {
          cookie[key2] = value2;
        }
      });
      return cookie;
    }
    __name(parseString2, "parseString");
    function parseNameValuePair(nameValuePairStr) {
      var name = "";
      var value = "";
      var nameValueArr = nameValuePairStr.split("=");
      if (nameValueArr.length > 1) {
        name = nameValueArr.shift();
        value = nameValueArr.join("=");
      } else {
        value = nameValuePairStr;
      }
      return { name, value };
    }
    __name(parseNameValuePair, "parseNameValuePair");
    function parse3(input, options2) {
      options2 = options2 ? Object.assign({}, defaultParseOptions, options2) : defaultParseOptions;
      if (!input) {
        if (!options2.map) {
          return [];
        } else {
          return createNullObj();
        }
      }
      if (input.headers) {
        if (typeof input.headers.getSetCookie === "function") {
          input = input.headers.getSetCookie();
        } else if (input.headers["set-cookie"]) {
          input = input.headers["set-cookie"];
        } else {
          var sch = input.headers[Object.keys(input.headers).find(function(key2) {
            return key2.toLowerCase() === "set-cookie";
          })];
          if (!sch && input.headers.cookie && !options2.silent) {
            console.warn(
              "Warning: set-cookie-parser appears to have been called on a request object. It is designed to parse Set-Cookie headers from responses, not Cookie headers from requests. Set the option {silent: true} to suppress this warning."
            );
          }
          input = sch;
        }
      }
      if (!Array.isArray(input)) {
        input = [input];
      }
      if (!options2.map) {
        return input.filter(isNonEmptyString).map(function(str) {
          return parseString2(str, options2);
        }).filter(Boolean);
      } else {
        var cookies = createNullObj();
        return input.filter(isNonEmptyString).reduce(function(cookies2, str) {
          var cookie = parseString2(str, options2);
          if (cookie && !isForbiddenKey(cookie.name)) {
            cookies2[cookie.name] = cookie;
          }
          return cookies2;
        }, cookies);
      }
    }
    __name(parse3, "parse");
    function splitCookiesString2(cookiesString) {
      if (Array.isArray(cookiesString)) {
        return cookiesString;
      }
      if (typeof cookiesString !== "string") {
        return [];
      }
      var cookiesStrings = [];
      var pos = 0;
      var start;
      var ch;
      var lastComma;
      var nextStart;
      var cookiesSeparatorFound;
      function skipWhitespace() {
        while (pos < cookiesString.length && /\s/.test(cookiesString.charAt(pos))) {
          pos += 1;
        }
        return pos < cookiesString.length;
      }
      __name(skipWhitespace, "skipWhitespace");
      function notSpecialChar() {
        ch = cookiesString.charAt(pos);
        return ch !== "=" && ch !== ";" && ch !== ",";
      }
      __name(notSpecialChar, "notSpecialChar");
      while (pos < cookiesString.length) {
        start = pos;
        cookiesSeparatorFound = false;
        while (skipWhitespace()) {
          ch = cookiesString.charAt(pos);
          if (ch === ",") {
            lastComma = pos;
            pos += 1;
            skipWhitespace();
            nextStart = pos;
            while (pos < cookiesString.length && notSpecialChar()) {
              pos += 1;
            }
            if (pos < cookiesString.length && cookiesString.charAt(pos) === "=") {
              cookiesSeparatorFound = true;
              pos = nextStart;
              cookiesStrings.push(cookiesString.substring(start, lastComma));
              start = pos;
            } else {
              pos = lastComma + 1;
            }
          } else {
            pos += 1;
          }
        }
        if (!cookiesSeparatorFound || pos >= cookiesString.length) {
          cookiesStrings.push(cookiesString.substring(start, cookiesString.length));
        }
      }
      return cookiesStrings;
    }
    __name(splitCookiesString2, "splitCookiesString");
    module.exports = parse3;
    module.exports.parse = parse3;
    module.exports.parseString = parseString2;
    module.exports.splitCookiesString = splitCookiesString2;
  }
});

// .svelte-kit/output/server/entries/pages/_layout.svelte.js
var layout_svelte_exports = {};
__export(layout_svelte_exports, {
  default: () => _layout
});
function _layout($$renderer, $$props) {
  $$renderer.push(`<!--[-->`);
  slot($$renderer, $$props, "default", {});
  $$renderer.push(`<!--]-->`);
}
var init_layout_svelte = __esm({
  ".svelte-kit/output/server/entries/pages/_layout.svelte.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_chunks();
    __name(_layout, "_layout");
  }
});

// .svelte-kit/output/server/nodes/0.js
var __exports = {};
__export(__exports, {
  component: () => component,
  fonts: () => fonts,
  imports: () => imports,
  index: () => index,
  stylesheets: () => stylesheets
});
var index, component_cache, component, imports, stylesheets, fonts;
var init__ = __esm({
  ".svelte-kit/output/server/nodes/0.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    index = 0;
    component = /* @__PURE__ */ __name(async () => component_cache ??= (await Promise.resolve().then(() => (init_layout_svelte(), layout_svelte_exports))).default, "component");
    imports = ["_app/immutable/nodes/0.Ds_eWx-U.js", "_app/immutable/chunks/DFqY95ai.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/Di_XG6Ig.js"];
    stylesheets = ["_app/immutable/assets/0.DNMtJkfe.css"];
    fonts = [];
  }
});

// .svelte-kit/output/server/entries/fallbacks/error.svelte.js
var error_svelte_exports = {};
__export(error_svelte_exports, {
  default: () => Error$1
});
function create_updated_store() {
  const { set: set2, subscribe } = writable(false);
  {
    return {
      subscribe,
      // eslint-disable-next-line @typescript-eslint/require-await
      check: /* @__PURE__ */ __name(async () => false, "check")
    };
  }
}
function context2() {
  return getContext("__request__");
}
function Error$1($$renderer, $$props) {
  $$renderer.component(($$renderer2) => {
    $$renderer2.push(`<h1>${escape_html(page.status)}</h1> <p>${escape_html(page.error?.message)}</p>`);
  });
}
var is_legacy, stores, page$1, page;
var init_error_svelte = __esm({
  ".svelte-kit/output/server/entries/fallbacks/error.svelte.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_escaping();
    init_clsx();
    init_equality();
    init_server();
    init_internal();
    init_exports2();
    init_utils3();
    init_context();
    __name(create_updated_store, "create_updated_store");
    is_legacy = noop.toString().includes("$$") || /function \w+\(\) \{\}/.test(noop.toString());
    if (is_legacy) {
      ({
        data: {},
        form: null,
        error: null,
        params: {},
        route: { id: null },
        state: {},
        status: -1,
        url: new URL("https://example.com")
      });
    }
    stores = {
      updated: /* @__PURE__ */ create_updated_store()
    };
    ({
      check: stores.updated.check
    });
    __name(context2, "context");
    page$1 = {
      get error() {
        return context2().page.error;
      },
      get status() {
        return context2().page.status;
      }
    };
    page = page$1;
    __name(Error$1, "Error$1");
  }
});

// .svelte-kit/output/server/nodes/1.js
var __exports2 = {};
__export(__exports2, {
  component: () => component2,
  fonts: () => fonts2,
  imports: () => imports2,
  index: () => index2,
  stylesheets: () => stylesheets2
});
var index2, component_cache2, component2, imports2, stylesheets2, fonts2;
var init__2 = __esm({
  ".svelte-kit/output/server/nodes/1.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    index2 = 1;
    component2 = /* @__PURE__ */ __name(async () => component_cache2 ??= (await Promise.resolve().then(() => (init_error_svelte(), error_svelte_exports))).default, "component");
    imports2 = ["_app/immutable/nodes/1.DDlo7ndi.js", "_app/immutable/chunks/DFqY95ai.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/Di_XG6Ig.js", "_app/immutable/chunks/DWDlLD0t.js", "_app/immutable/chunks/CligCAuS.js", "_app/immutable/chunks/C_Sh5Hij.js"];
    stylesheets2 = [];
    fonts2 = [];
  }
});

// .svelte-kit/output/server/entries/pages/_page.svelte.js
var page_svelte_exports = {};
__export(page_svelte_exports, {
  default: () => _page
});
function Counter($$renderer) {
  let count3 = 0;
  $$renderer.push(`<button>count is ${escape_html(count3)}</button>`);
}
function _page($$renderer) {
  $$renderer.push(`<main><div class="card">`);
  Counter($$renderer);
  $$renderer.push(`<!----></div> <p>Check out <a href="https://github.com/sveltejs/kit#readme" target="_blank" rel="noreferrer">SvelteKit</a>, the official Svelte app framework powered by Vite!</p> <p class="read-the-docs svelte-1uha8ag">Click on the Vite and Svelte logos to learn more</p> <p><a href="/game">Play the Game!</a></p></main>`);
}
var init_page_svelte = __esm({
  ".svelte-kit/output/server/entries/pages/_page.svelte.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_clsx();
    init_escaping();
    __name(Counter, "Counter");
    __name(_page, "_page");
  }
});

// .svelte-kit/output/server/nodes/2.js
var __exports3 = {};
__export(__exports3, {
  component: () => component3,
  fonts: () => fonts3,
  imports: () => imports3,
  index: () => index3,
  stylesheets: () => stylesheets3
});
var index3, component_cache3, component3, imports3, stylesheets3, fonts3;
var init__3 = __esm({
  ".svelte-kit/output/server/nodes/2.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    index3 = 2;
    component3 = /* @__PURE__ */ __name(async () => component_cache3 ??= (await Promise.resolve().then(() => (init_page_svelte(), page_svelte_exports))).default, "component");
    imports3 = ["_app/immutable/nodes/2.B2EiJEZV.js", "_app/immutable/chunks/DFqY95ai.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/Di_XG6Ig.js", "_app/immutable/chunks/DWDlLD0t.js"];
    stylesheets3 = ["_app/immutable/assets/2.9XHcVk5V.css"];
    fonts3 = [];
  }
});

// .svelte-kit/output/server/entries/pages/game/_page.server.ts.js
var page_server_ts_exports = {};
__export(page_server_ts_exports, {
  load: () => load
});
var load;
var init_page_server_ts = __esm({
  ".svelte-kit/output/server/entries/pages/game/_page.server.ts.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    load = /* @__PURE__ */ __name(async ({ platform: platform2 }) => {
      const spacetimedbHost = platform2?.env?.SPACETIMEDB_HOST || "ws://localhost:3000";
      const spacetimedbDbName = platform2?.env?.SPACETIMEDB_DB_NAME || "marbles2";
      return {
        spacetimedbConfig: {
          host: spacetimedbHost,
          moduleName: spacetimedbDbName
        }
      };
    }, "load");
  }
});

// .svelte-kit/output/server/entries/pages/game/_page.svelte.js
var page_svelte_exports2 = {};
__export(page_svelte_exports2, {
  default: () => _page2
});
function _page2($$renderer, $$props) {
  $$renderer.component(($$renderer2) => {
    let { data } = $$props;
    head("4p1id7", $$renderer2, ($$renderer3) => {
      $$renderer3.title(($$renderer4) => {
        $$renderer4.push(`<title>Unity Web Player | MarblesUnityClient</title>`);
      });
      $$renderer3.push(`<meta name="viewport" content="width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes"/>`);
    });
    $$renderer2.push(`<div class="unity-container svelte-4p1id7"><canvas id="unity-canvas" tabindex="-1" class="svelte-4p1id7"></canvas></div>`);
  });
}
var init_page_svelte2 = __esm({
  ".svelte-kit/output/server/entries/pages/game/_page.svelte.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_chunks();
    __name(_page2, "_page");
  }
});

// .svelte-kit/output/server/nodes/3.js
var __exports4 = {};
__export(__exports4, {
  component: () => component4,
  fonts: () => fonts4,
  imports: () => imports4,
  index: () => index4,
  server: () => page_server_ts_exports,
  server_id: () => server_id,
  stylesheets: () => stylesheets4
});
var index4, component_cache4, component4, server_id, imports4, stylesheets4, fonts4;
var init__4 = __esm({
  ".svelte-kit/output/server/nodes/3.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_page_server_ts();
    index4 = 3;
    component4 = /* @__PURE__ */ __name(async () => component_cache4 ??= (await Promise.resolve().then(() => (init_page_svelte2(), page_svelte_exports2))).default, "component");
    server_id = "src/routes/game/+page.server.ts";
    imports4 = ["_app/immutable/nodes/3.D6V_G3qs.js", "_app/immutable/chunks/DFqY95ai.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/C_Sh5Hij.js", "_app/immutable/chunks/yg97jW04.js"];
    stylesheets4 = ["_app/immutable/assets/3.CtrtHYcH.css"];
    fonts4 = [];
  }
});

// node_modules/base64-js/index.js
var require_base64_js = __commonJS({
  "node_modules/base64-js/index.js"(exports) {
    "use strict";
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    exports.byteLength = byteLength;
    exports.toByteArray = toByteArray;
    exports.fromByteArray = fromByteArray2;
    var lookup = [];
    var revLookup = [];
    var Arr = typeof Uint8Array !== "undefined" ? Uint8Array : Array;
    var code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    for (i = 0, len = code.length; i < len; ++i) {
      lookup[i] = code[i];
      revLookup[code.charCodeAt(i)] = i;
    }
    var i;
    var len;
    revLookup["-".charCodeAt(0)] = 62;
    revLookup["_".charCodeAt(0)] = 63;
    function getLens(b64) {
      var len2 = b64.length;
      if (len2 % 4 > 0) {
        throw new Error("Invalid string. Length must be a multiple of 4");
      }
      var validLen = b64.indexOf("=");
      if (validLen === -1) validLen = len2;
      var placeHoldersLen = validLen === len2 ? 0 : 4 - validLen % 4;
      return [validLen, placeHoldersLen];
    }
    __name(getLens, "getLens");
    function byteLength(b64) {
      var lens = getLens(b64);
      var validLen = lens[0];
      var placeHoldersLen = lens[1];
      return (validLen + placeHoldersLen) * 3 / 4 - placeHoldersLen;
    }
    __name(byteLength, "byteLength");
    function _byteLength(b64, validLen, placeHoldersLen) {
      return (validLen + placeHoldersLen) * 3 / 4 - placeHoldersLen;
    }
    __name(_byteLength, "_byteLength");
    function toByteArray(b64) {
      var tmp;
      var lens = getLens(b64);
      var validLen = lens[0];
      var placeHoldersLen = lens[1];
      var arr = new Arr(_byteLength(b64, validLen, placeHoldersLen));
      var curByte = 0;
      var len2 = placeHoldersLen > 0 ? validLen - 4 : validLen;
      var i2;
      for (i2 = 0; i2 < len2; i2 += 4) {
        tmp = revLookup[b64.charCodeAt(i2)] << 18 | revLookup[b64.charCodeAt(i2 + 1)] << 12 | revLookup[b64.charCodeAt(i2 + 2)] << 6 | revLookup[b64.charCodeAt(i2 + 3)];
        arr[curByte++] = tmp >> 16 & 255;
        arr[curByte++] = tmp >> 8 & 255;
        arr[curByte++] = tmp & 255;
      }
      if (placeHoldersLen === 2) {
        tmp = revLookup[b64.charCodeAt(i2)] << 2 | revLookup[b64.charCodeAt(i2 + 1)] >> 4;
        arr[curByte++] = tmp & 255;
      }
      if (placeHoldersLen === 1) {
        tmp = revLookup[b64.charCodeAt(i2)] << 10 | revLookup[b64.charCodeAt(i2 + 1)] << 4 | revLookup[b64.charCodeAt(i2 + 2)] >> 2;
        arr[curByte++] = tmp >> 8 & 255;
        arr[curByte++] = tmp & 255;
      }
      return arr;
    }
    __name(toByteArray, "toByteArray");
    function tripletToBase64(num) {
      return lookup[num >> 18 & 63] + lookup[num >> 12 & 63] + lookup[num >> 6 & 63] + lookup[num & 63];
    }
    __name(tripletToBase64, "tripletToBase64");
    function encodeChunk(uint8, start, end) {
      var tmp;
      var output = [];
      for (var i2 = start; i2 < end; i2 += 3) {
        tmp = (uint8[i2] << 16 & 16711680) + (uint8[i2 + 1] << 8 & 65280) + (uint8[i2 + 2] & 255);
        output.push(tripletToBase64(tmp));
      }
      return output.join("");
    }
    __name(encodeChunk, "encodeChunk");
    function fromByteArray2(uint8) {
      var tmp;
      var len2 = uint8.length;
      var extraBytes = len2 % 3;
      var parts = [];
      var maxChunkLength = 16383;
      for (var i2 = 0, len22 = len2 - extraBytes; i2 < len22; i2 += maxChunkLength) {
        parts.push(encodeChunk(uint8, i2, i2 + maxChunkLength > len22 ? len22 : i2 + maxChunkLength));
      }
      if (extraBytes === 1) {
        tmp = uint8[len2 - 1];
        parts.push(
          lookup[tmp >> 2] + lookup[tmp << 4 & 63] + "=="
        );
      } else if (extraBytes === 2) {
        tmp = (uint8[len2 - 2] << 8) + uint8[len2 - 1];
        parts.push(
          lookup[tmp >> 10] + lookup[tmp >> 4 & 63] + lookup[tmp << 2 & 63] + "="
        );
      }
      return parts.join("");
    }
    __name(fromByteArray2, "fromByteArray");
  }
});

// node_modules/spacetimedb/dist/index.browser.mjs
function deepEqual(obj1, obj2) {
  if (obj1 === obj2) return true;
  if (typeof obj1 !== "object" || obj1 === null || typeof obj2 !== "object" || obj2 === null) {
    return false;
  }
  const keys1 = Object.keys(obj1);
  const keys2 = Object.keys(obj2);
  if (keys1.length !== keys2.length) return false;
  for (const key2 of keys1) {
    if (!keys2.includes(key2) || !deepEqual(obj1[key2], obj2[key2])) {
      return false;
    }
  }
  return true;
}
function uint8ArrayToHexString(array2) {
  return Array.prototype.map.call(array2.reverse(), (x) => ("00" + x.toString(16)).slice(-2)).join("");
}
function uint8ArrayToU128(array2) {
  if (array2.length != 16) {
    throw new Error(`Uint8Array is not 16 bytes long: ${array2}`);
  }
  return new BinaryReader(array2).readU128();
}
function uint8ArrayToU256(array2) {
  if (array2.length != 32) {
    throw new Error(`Uint8Array is not 32 bytes long: [${array2}]`);
  }
  return new BinaryReader(array2).readU256();
}
function hexStringToUint8Array(str) {
  if (str.startsWith("0x")) {
    str = str.slice(2);
  }
  const matches = str.match(/.{1,2}/g) || [];
  const data = Uint8Array.from(
    matches.map((byte) => parseInt(byte, 16))
  );
  return data.reverse();
}
function hexStringToU128(str) {
  return uint8ArrayToU128(hexStringToUint8Array(str));
}
function hexStringToU256(str) {
  return uint8ArrayToU256(hexStringToUint8Array(str));
}
function u128ToUint8Array(data) {
  const writer = new BinaryWriter(16);
  writer.writeU128(data);
  return writer.getBuffer();
}
function u128ToHexString(data) {
  return uint8ArrayToHexString(u128ToUint8Array(data));
}
function u256ToUint8Array(data) {
  const writer = new BinaryWriter(32);
  writer.writeU256(data);
  return writer.getBuffer();
}
function u256ToHexString(data) {
  return uint8ArrayToHexString(u256ToUint8Array(data));
}
function parseValue(ty, src) {
  const reader = new BinaryReader(src);
  return ty.deserialize(reader);
}
function comparePreReleases(a, b) {
  const len = Math.min(a.length, b.length);
  for (let i = 0; i < len; i++) {
    const aPart = a[i];
    const bPart = b[i];
    if (aPart === bPart) continue;
    if (typeof aPart === "number" && typeof bPart === "number") {
      return aPart - bPart;
    }
    if (typeof aPart === "string" && typeof bPart === "string") {
      return aPart.localeCompare(bPart);
    }
    return typeof aPart === "string" ? 1 : -1;
  }
  return a.length - b.length;
}
function ensureMinimumVersionOrThrow(versionString) {
  if (versionString === void 0) {
    throw new Error(versionErrorMessage(versionString));
  }
  const version2 = SemanticVersion.parseVersionString(versionString);
  if (version2.compare(_MINIMUM_CLI_VERSION) < 0) {
    throw new Error(versionErrorMessage(versionString));
  }
}
function versionErrorMessage(incompatibleVersion) {
  return `Module code was generated with an incompatible version of the spacetimedb cli (${incompatibleVersion}). Update the cli version to at least ${_MINIMUM_CLI_VERSION.toString()} and regenerate the bindings. You can upgrade to the latest cli version by running: spacetime version upgrade`;
}
async function decompress(buffer, type, chunkSize = 128 * 1024) {
  let offset = 0;
  const readableStream = new ReadableStream({
    pull(controller2) {
      if (offset < buffer.length) {
        const chunk = buffer.subarray(
          offset,
          Math.min(offset + chunkSize, buffer.length)
        );
        controller2.enqueue(chunk);
        offset += chunkSize;
      } else {
        controller2.close();
      }
    }
  });
  const decompressionStream = new DecompressionStream(type);
  const decompressedStream = readableStream.pipeThrough(decompressionStream);
  const reader = decompressedStream.getReader();
  const chunks = [];
  let totalLength = 0;
  let result;
  while (!(result = await reader.read()).done) {
    chunks.push(result.value);
    totalLength += result.value.length;
  }
  const decompressedArray = new Uint8Array(totalLength);
  let chunkOffset = 0;
  for (const chunk of chunks) {
    decompressedArray.set(chunk, chunkOffset);
    chunkOffset += chunk.length;
  }
  return decompressedArray;
}
async function resolveWS() {
  if (typeof globalThis.WebSocket !== "undefined") {
    return globalThis.WebSocket;
  }
  const dynamicImport = new Function("m", "return import(m)");
  try {
    const { WebSocket: UndiciWS } = await dynamicImport("undici");
    return UndiciWS;
  } catch (err) {
    console.warn(
      "[spacetimedb-sdk] No global WebSocket found. On Node 18\u201321, please install `undici` (npm install undici) to enable WebSocket support."
    );
    throw err;
  }
}
function callReducerFlagsToNumber(flags2) {
  switch (flags2) {
    case "FullUpdate":
      return 0;
    case "NoSuccessNotify":
      return 1;
  }
}
var import_base64_js, TimeDuration, Timestamp, BinaryWriter, BinaryReader, Identity, Option, _cached_SumTypeVariant_type_value, SumTypeVariant, _cached_SumType_type_value, SumType, _cached_ProductTypeElement_type_value, ProductTypeElement, _cached_ProductType_type_value, ProductType, _cached_AlgebraicType_type_value, AlgebraicType2, ScheduleAt, Interval, Time, schedule_at_default, AlgebraicType, ProductType2, SumType2, ConnectionId, _cached_RowSizeHint_type_value, RowSizeHint, _cached_BsatnRowList_type_value, BsatnRowList, _cached_CallReducer_type_value, CallReducer, _cached_Subscribe_type_value, Subscribe, _cached_OneOffQuery_type_value, OneOffQuery, _cached_QueryId_type_value, QueryId, _cached_SubscribeSingle_type_value, SubscribeSingle, _cached_SubscribeMulti_type_value, SubscribeMulti, _cached_Unsubscribe_type_value, Unsubscribe, _cached_UnsubscribeMulti_type_value, UnsubscribeMulti, _cached_ClientMessage_type_value, ClientMessage, _cached_QueryUpdate_type_value, QueryUpdate, _cached_CompressableQueryUpdate_type_value, CompressableQueryUpdate, _cached_TableUpdate_type_value, TableUpdate, _cached_DatabaseUpdate_type_value, DatabaseUpdate, _cached_InitialSubscription_type_value, InitialSubscription, _cached_UpdateStatus_type_value, UpdateStatus, _cached_ReducerCallInfo_type_value, ReducerCallInfo, _cached_EnergyQuanta_type_value, EnergyQuanta, _cached_TransactionUpdate_type_value, TransactionUpdate, _cached_TransactionUpdateLight_type_value, TransactionUpdateLight, _cached_IdentityToken_type_value, IdentityToken, _cached_OneOffTable_type_value, OneOffTable, _cached_OneOffQueryResponse_type_value, OneOffQueryResponse, _cached_SubscribeRows_type_value, SubscribeRows, _cached_SubscribeApplied_type_value, SubscribeApplied, _cached_UnsubscribeApplied_type_value, UnsubscribeApplied, _cached_SubscriptionError_type_value, SubscriptionError, _cached_SubscribeMultiApplied_type_value, SubscribeMultiApplied, _cached_UnsubscribeMultiApplied_type_value, UnsubscribeMultiApplied, _cached_ServerMessage_type_value, ServerMessage, EventEmitter2, LogLevelIdentifierIcon, LogStyle, LogTextStyle, stdbLogger, TableCache, ClientCache, SemanticVersion, _MINIMUM_CLI_VERSION, WebsocketDecompressAdapter, DbConnectionBuilder, SubscriptionBuilderImpl, SubscriptionManager, SubscriptionHandleImpl, DbConnectionImpl;
var init_index_browser = __esm({
  "node_modules/spacetimedb/dist/index.browser.mjs"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    import_base64_js = __toESM(require_base64_js(), 1);
    TimeDuration = class _TimeDuration {
      static {
        __name(this, "_TimeDuration");
      }
      __time_duration_micros__;
      static MICROS_PER_MILLIS = 1000n;
      /**
       * Get the algebraic type representation of the {@link TimeDuration} type.
       * @returns The algebraic type representation of the type.
       */
      static getAlgebraicType() {
        return AlgebraicType.Product({
          elements: [
            {
              name: "__time_duration_micros__",
              algebraicType: AlgebraicType.I64
            }
          ]
        });
      }
      get micros() {
        return this.__time_duration_micros__;
      }
      get millis() {
        return Number(this.micros / _TimeDuration.MICROS_PER_MILLIS);
      }
      constructor(micros) {
        this.__time_duration_micros__ = micros;
      }
      static fromMillis(millis) {
        return new _TimeDuration(BigInt(millis) * _TimeDuration.MICROS_PER_MILLIS);
      }
      /** This outputs the same string format that we use in the host and in Rust modules */
      toString() {
        const micros = this.micros;
        const sign = micros < 0 ? "-" : "+";
        const pos = micros < 0 ? -micros : micros;
        const secs = pos / 1000000n;
        const micros_remaining = pos % 1000000n;
        return `${sign}${secs}.${String(micros_remaining).padStart(6, "0")}`;
      }
    };
    Timestamp = class _Timestamp {
      static {
        __name(this, "_Timestamp");
      }
      __timestamp_micros_since_unix_epoch__;
      static MICROS_PER_MILLIS = 1000n;
      get microsSinceUnixEpoch() {
        return this.__timestamp_micros_since_unix_epoch__;
      }
      constructor(micros) {
        this.__timestamp_micros_since_unix_epoch__ = micros;
      }
      /**
       * Get the algebraic type representation of the {@link Timestamp} type.
       * @returns The algebraic type representation of the type.
       */
      static getAlgebraicType() {
        return AlgebraicType.Product({
          elements: [
            {
              name: "__timestamp_micros_since_unix_epoch__",
              algebraicType: AlgebraicType.I64
            }
          ]
        });
      }
      /**
       * The Unix epoch, the midnight at the beginning of January 1, 1970, UTC.
       */
      static UNIX_EPOCH = new _Timestamp(0n);
      /**
       * Get a `Timestamp` representing the execution environment's belief of the current moment in time.
       */
      static now() {
        return _Timestamp.fromDate(/* @__PURE__ */ new Date());
      }
      /**
       * Get a `Timestamp` representing the same point in time as `date`.
       */
      static fromDate(date) {
        const millis = date.getTime();
        const micros = BigInt(millis) * _Timestamp.MICROS_PER_MILLIS;
        return new _Timestamp(micros);
      }
      /**
       * Get a `Date` representing approximately the same point in time as `this`.
       *
       * This method truncates to millisecond precision,
       * and throws `RangeError` if the `Timestamp` is outside the range representable as a `Date`.
       */
      toDate() {
        const micros = this.__timestamp_micros_since_unix_epoch__;
        const millis = micros / _Timestamp.MICROS_PER_MILLIS;
        if (millis > BigInt(Number.MAX_SAFE_INTEGER) || millis < BigInt(Number.MIN_SAFE_INTEGER)) {
          throw new RangeError(
            "Timestamp is outside of the representable range of JS's Date"
          );
        }
        return new Date(Number(millis));
      }
      since(other) {
        return new TimeDuration(
          this.__timestamp_micros_since_unix_epoch__ - other.__timestamp_micros_since_unix_epoch__
        );
      }
    };
    BinaryWriter = class {
      static {
        __name(this, "BinaryWriter");
      }
      #buffer;
      #view;
      #offset = 0;
      constructor(size) {
        this.#buffer = new Uint8Array(size);
        this.#view = new DataView(this.#buffer.buffer);
      }
      #expandBuffer(additionalCapacity) {
        const minCapacity = this.#offset + additionalCapacity + 1;
        if (minCapacity <= this.#buffer.length) return;
        let newCapacity = this.#buffer.length * 2;
        if (newCapacity < minCapacity) newCapacity = minCapacity;
        const newBuffer = new Uint8Array(newCapacity);
        newBuffer.set(this.#buffer);
        this.#buffer = newBuffer;
        this.#view = new DataView(this.#buffer.buffer);
      }
      toBase64() {
        return (0, import_base64_js.fromByteArray)(this.#buffer.subarray(0, this.#offset));
      }
      getBuffer() {
        return this.#buffer.slice(0, this.#offset);
      }
      get offset() {
        return this.#offset;
      }
      writeUInt8Array(value) {
        const length = value.length;
        this.#expandBuffer(4 + length);
        this.writeU32(length);
        this.#buffer.set(value, this.#offset);
        this.#offset += value.length;
      }
      writeBool(value) {
        this.#expandBuffer(1);
        this.#view.setUint8(this.#offset, value ? 1 : 0);
        this.#offset += 1;
      }
      writeByte(value) {
        this.#expandBuffer(1);
        this.#view.setUint8(this.#offset, value);
        this.#offset += 1;
      }
      writeI8(value) {
        this.#expandBuffer(1);
        this.#view.setInt8(this.#offset, value);
        this.#offset += 1;
      }
      writeU8(value) {
        this.#expandBuffer(1);
        this.#view.setUint8(this.#offset, value);
        this.#offset += 1;
      }
      writeI16(value) {
        this.#expandBuffer(2);
        this.#view.setInt16(this.#offset, value, true);
        this.#offset += 2;
      }
      writeU16(value) {
        this.#expandBuffer(2);
        this.#view.setUint16(this.#offset, value, true);
        this.#offset += 2;
      }
      writeI32(value) {
        this.#expandBuffer(4);
        this.#view.setInt32(this.#offset, value, true);
        this.#offset += 4;
      }
      writeU32(value) {
        this.#expandBuffer(4);
        this.#view.setUint32(this.#offset, value, true);
        this.#offset += 4;
      }
      writeI64(value) {
        this.#expandBuffer(8);
        this.#view.setBigInt64(this.#offset, value, true);
        this.#offset += 8;
      }
      writeU64(value) {
        this.#expandBuffer(8);
        this.#view.setBigUint64(this.#offset, value, true);
        this.#offset += 8;
      }
      writeU128(value) {
        this.#expandBuffer(16);
        const lowerPart = value & BigInt("0xFFFFFFFFFFFFFFFF");
        const upperPart = value >> BigInt(64);
        this.#view.setBigUint64(this.#offset, lowerPart, true);
        this.#view.setBigUint64(this.#offset + 8, upperPart, true);
        this.#offset += 16;
      }
      writeI128(value) {
        this.#expandBuffer(16);
        const lowerPart = value & BigInt("0xFFFFFFFFFFFFFFFF");
        const upperPart = value >> BigInt(64);
        this.#view.setBigInt64(this.#offset, lowerPart, true);
        this.#view.setBigInt64(this.#offset + 8, upperPart, true);
        this.#offset += 16;
      }
      writeU256(value) {
        this.#expandBuffer(32);
        const low_64_mask = BigInt("0xFFFFFFFFFFFFFFFF");
        const p0 = value & low_64_mask;
        const p1 = value >> BigInt(64 * 1) & low_64_mask;
        const p2 = value >> BigInt(64 * 2) & low_64_mask;
        const p3 = value >> BigInt(64 * 3);
        this.#view.setBigUint64(this.#offset + 8 * 0, p0, true);
        this.#view.setBigUint64(this.#offset + 8 * 1, p1, true);
        this.#view.setBigUint64(this.#offset + 8 * 2, p2, true);
        this.#view.setBigUint64(this.#offset + 8 * 3, p3, true);
        this.#offset += 32;
      }
      writeI256(value) {
        this.#expandBuffer(32);
        const low_64_mask = BigInt("0xFFFFFFFFFFFFFFFF");
        const p0 = value & low_64_mask;
        const p1 = value >> BigInt(64 * 1) & low_64_mask;
        const p2 = value >> BigInt(64 * 2) & low_64_mask;
        const p3 = value >> BigInt(64 * 3);
        this.#view.setBigUint64(this.#offset + 8 * 0, p0, true);
        this.#view.setBigUint64(this.#offset + 8 * 1, p1, true);
        this.#view.setBigUint64(this.#offset + 8 * 2, p2, true);
        this.#view.setBigInt64(this.#offset + 8 * 3, p3, true);
        this.#offset += 32;
      }
      writeF32(value) {
        this.#expandBuffer(4);
        this.#view.setFloat32(this.#offset, value, true);
        this.#offset += 4;
      }
      writeF64(value) {
        this.#expandBuffer(8);
        this.#view.setFloat64(this.#offset, value, true);
        this.#offset += 8;
      }
      writeString(value) {
        const encoder = new TextEncoder();
        const encodedString = encoder.encode(value);
        this.writeU32(encodedString.length);
        this.#expandBuffer(encodedString.length);
        this.#buffer.set(encodedString, this.#offset);
        this.#offset += encodedString.length;
      }
    };
    BinaryReader = class {
      static {
        __name(this, "BinaryReader");
      }
      /**
       * The DataView used to read values from the binary data.
       *
       * Note: The DataView's `byteOffset` is relative to the beginning of the
       * underlying ArrayBuffer, not the start of the provided Uint8Array input.
       * This `BinaryReader`'s `#offset` field is used to track the current read position
       * relative to the start of the provided Uint8Array input.
       */
      #view;
      /**
       * Represents the offset (in bytes) relative to the start of the DataView
       * and provided Uint8Array input.
       *
       * Note: This is *not* the absolute byte offset within the underlying ArrayBuffer.
       */
      #offset = 0;
      constructor(input) {
        this.#view = new DataView(input.buffer, input.byteOffset, input.byteLength);
        this.#offset = 0;
      }
      get offset() {
        return this.#offset;
      }
      get remaining() {
        return this.#view.byteLength - this.#offset;
      }
      /** Ensure we have at least `n` bytes left to read */
      #ensure(n2) {
        if (this.#offset + n2 > this.#view.byteLength) {
          throw new RangeError(
            `Tried to read ${n2} byte(s) at relative offset ${this.#offset}, but only ${this.remaining} byte(s) remain`
          );
        }
      }
      readUInt8Array() {
        const length = this.readU32();
        this.#ensure(length);
        return this.readBytes(length);
      }
      readBool() {
        const value = this.#view.getUint8(this.#offset);
        this.#offset += 1;
        return value !== 0;
      }
      readByte() {
        const value = this.#view.getUint8(this.#offset);
        this.#offset += 1;
        return value;
      }
      readBytes(length) {
        const array2 = new Uint8Array(
          this.#view.buffer,
          this.#view.byteOffset + this.#offset,
          length
        );
        this.#offset += length;
        return array2;
      }
      readI8() {
        const value = this.#view.getInt8(this.#offset);
        this.#offset += 1;
        return value;
      }
      readU8() {
        return this.readByte();
      }
      readI16() {
        const value = this.#view.getInt16(this.#offset, true);
        this.#offset += 2;
        return value;
      }
      readU16() {
        const value = this.#view.getUint16(this.#offset, true);
        this.#offset += 2;
        return value;
      }
      readI32() {
        const value = this.#view.getInt32(this.#offset, true);
        this.#offset += 4;
        return value;
      }
      readU32() {
        const value = this.#view.getUint32(this.#offset, true);
        this.#offset += 4;
        return value;
      }
      readI64() {
        const value = this.#view.getBigInt64(this.#offset, true);
        this.#offset += 8;
        return value;
      }
      readU64() {
        const value = this.#view.getBigUint64(this.#offset, true);
        this.#offset += 8;
        return value;
      }
      readU128() {
        const lowerPart = this.#view.getBigUint64(this.#offset, true);
        const upperPart = this.#view.getBigUint64(this.#offset + 8, true);
        this.#offset += 16;
        return (upperPart << BigInt(64)) + lowerPart;
      }
      readI128() {
        const lowerPart = this.#view.getBigUint64(this.#offset, true);
        const upperPart = this.#view.getBigInt64(this.#offset + 8, true);
        this.#offset += 16;
        return (upperPart << BigInt(64)) + lowerPart;
      }
      readU256() {
        const p0 = this.#view.getBigUint64(this.#offset, true);
        const p1 = this.#view.getBigUint64(this.#offset + 8, true);
        const p2 = this.#view.getBigUint64(this.#offset + 16, true);
        const p3 = this.#view.getBigUint64(this.#offset + 24, true);
        this.#offset += 32;
        return (p3 << BigInt(3 * 64)) + (p2 << BigInt(2 * 64)) + (p1 << BigInt(1 * 64)) + p0;
      }
      readI256() {
        const p0 = this.#view.getBigUint64(this.#offset, true);
        const p1 = this.#view.getBigUint64(this.#offset + 8, true);
        const p2 = this.#view.getBigUint64(this.#offset + 16, true);
        const p3 = this.#view.getBigInt64(this.#offset + 24, true);
        this.#offset += 32;
        return (p3 << BigInt(3 * 64)) + (p2 << BigInt(2 * 64)) + (p1 << BigInt(1 * 64)) + p0;
      }
      readF32() {
        const value = this.#view.getFloat32(this.#offset, true);
        this.#offset += 4;
        return value;
      }
      readF64() {
        const value = this.#view.getFloat64(this.#offset, true);
        this.#offset += 8;
        return value;
      }
      readString() {
        const uint8Array = this.readUInt8Array();
        return new TextDecoder("utf-8").decode(uint8Array);
      }
    };
    __name(deepEqual, "deepEqual");
    __name(uint8ArrayToHexString, "uint8ArrayToHexString");
    __name(uint8ArrayToU128, "uint8ArrayToU128");
    __name(uint8ArrayToU256, "uint8ArrayToU256");
    __name(hexStringToUint8Array, "hexStringToUint8Array");
    __name(hexStringToU128, "hexStringToU128");
    __name(hexStringToU256, "hexStringToU256");
    __name(u128ToUint8Array, "u128ToUint8Array");
    __name(u128ToHexString, "u128ToHexString");
    __name(u256ToUint8Array, "u256ToUint8Array");
    __name(u256ToHexString, "u256ToHexString");
    Identity = class _Identity {
      static {
        __name(this, "_Identity");
      }
      __identity__;
      /**
       * Creates a new `Identity`.
       *
       * `data` can be a hexadecimal string or a `bigint`.
       */
      constructor(data) {
        this.__identity__ = typeof data === "string" ? hexStringToU256(data) : data;
      }
      /**
       * Get the algebraic type representation of the {@link Identity} type.
       * @returns The algebraic type representation of the type.
       */
      static getAlgebraicType() {
        return AlgebraicType.Product({
          elements: [{ name: "__identity__", algebraicType: AlgebraicType.U256 }]
        });
      }
      /**
       * Compare two identities for equality.
       */
      isEqual(other) {
        return this.toHexString() === other.toHexString();
      }
      /**
       * Print the identity as a hexadecimal string.
       */
      toHexString() {
        return u256ToHexString(this.__identity__);
      }
      /**
       * Convert the address to a Uint8Array.
       */
      toUint8Array() {
        return u256ToUint8Array(this.__identity__);
      }
      /**
       * Parse an Identity from a hexadecimal string.
       */
      static fromString(str) {
        return new _Identity(str);
      }
      /**
       * Zero identity (0x0000000000000000000000000000000000000000000000000000000000000000)
       */
      static zero() {
        return new _Identity(0n);
      }
      toString() {
        return this.toHexString();
      }
    };
    Option = {
      getAlgebraicType(innerType) {
        return AlgebraicType.Sum({
          variants: [
            { name: "some", algebraicType: innerType },
            {
              name: "none",
              algebraicType: AlgebraicType.Product({ elements: [] })
            }
          ]
        });
      }
    };
    _cached_SumTypeVariant_type_value = null;
    SumTypeVariant = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SumTypeVariant_type_value)
          return _cached_SumTypeVariant_type_value;
        _cached_SumTypeVariant_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SumTypeVariant_type_value.value.elements.push(
          {
            name: "name",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.String
            )
          },
          {
            name: "algebraicType",
            algebraicType: AlgebraicType2.getTypeScriptAlgebraicType()
          }
        );
        return _cached_SumTypeVariant_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SumTypeVariant.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SumTypeVariant.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SumType_type_value = null;
    SumType = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SumType_type_value) return _cached_SumType_type_value;
        _cached_SumType_type_value = AlgebraicType.Product({ elements: [] });
        _cached_SumType_type_value.value.elements.push({
          name: "variants",
          algebraicType: AlgebraicType.Array(
            SumTypeVariant.getTypeScriptAlgebraicType()
          )
        });
        return _cached_SumType_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SumType.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SumType.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_ProductTypeElement_type_value = null;
    ProductTypeElement = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_ProductTypeElement_type_value)
          return _cached_ProductTypeElement_type_value;
        _cached_ProductTypeElement_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_ProductTypeElement_type_value.value.elements.push(
          {
            name: "name",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.String
            )
          },
          {
            name: "algebraicType",
            algebraicType: AlgebraicType2.getTypeScriptAlgebraicType()
          }
        );
        return _cached_ProductTypeElement_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          ProductTypeElement.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          ProductTypeElement.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_ProductType_type_value = null;
    ProductType = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_ProductType_type_value) return _cached_ProductType_type_value;
        _cached_ProductType_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_ProductType_type_value.value.elements.push({
          name: "elements",
          algebraicType: AlgebraicType.Array(
            ProductTypeElement.getTypeScriptAlgebraicType()
          )
        });
        return _cached_ProductType_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          ProductType.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          ProductType.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_AlgebraicType_type_value = null;
    AlgebraicType2 = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      Ref: /* @__PURE__ */ __name((value) => ({ tag: "Ref", value }), "Ref"),
      Sum: /* @__PURE__ */ __name((value) => ({ tag: "Sum", value }), "Sum"),
      Product: /* @__PURE__ */ __name((value) => ({
        tag: "Product",
        value
      }), "Product"),
      Array: /* @__PURE__ */ __name((value) => ({
        tag: "Array",
        value
      }), "Array"),
      String: { tag: "String" },
      Bool: { tag: "Bool" },
      I8: { tag: "I8" },
      U8: { tag: "U8" },
      I16: { tag: "I16" },
      U16: { tag: "U16" },
      I32: { tag: "I32" },
      U32: { tag: "U32" },
      I64: { tag: "I64" },
      U64: { tag: "U64" },
      I128: { tag: "I128" },
      U128: { tag: "U128" },
      I256: { tag: "I256" },
      U256: { tag: "U256" },
      F32: { tag: "F32" },
      F64: { tag: "F64" },
      getTypeScriptAlgebraicType() {
        if (_cached_AlgebraicType_type_value)
          return _cached_AlgebraicType_type_value;
        _cached_AlgebraicType_type_value = AlgebraicType.Sum({
          variants: []
        });
        _cached_AlgebraicType_type_value.value.variants.push(
          { name: "Ref", algebraicType: AlgebraicType.U32 },
          { name: "Sum", algebraicType: SumType.getTypeScriptAlgebraicType() },
          {
            name: "Product",
            algebraicType: ProductType.getTypeScriptAlgebraicType()
          },
          {
            name: "Array",
            algebraicType: AlgebraicType2.getTypeScriptAlgebraicType()
          },
          {
            name: "String",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "Bool",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I8",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U8",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I16",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U16",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I32",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U32",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I64",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U64",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I128",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U128",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "I256",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "U256",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "F32",
            algebraicType: AlgebraicType.Product({ elements: [] })
          },
          {
            name: "F64",
            algebraicType: AlgebraicType.Product({ elements: [] })
          }
        );
        return _cached_AlgebraicType_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          AlgebraicType2.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          AlgebraicType2.getTypeScriptAlgebraicType()
        );
      }
    };
    ScheduleAt = {
      interval(value) {
        return Interval(value);
      },
      time(value) {
        return Time(value);
      },
      getAlgebraicType() {
        return AlgebraicType.Sum({
          variants: [
            {
              name: "Interval",
              algebraicType: TimeDuration.getAlgebraicType()
            },
            { name: "Time", algebraicType: Timestamp.getAlgebraicType() }
          ]
        });
      }
    };
    Interval = /* @__PURE__ */ __name((micros) => ({
      tag: "Interval",
      value: new TimeDuration(micros)
    }), "Interval");
    Time = /* @__PURE__ */ __name((microsSinceUnixEpoch) => ({
      tag: "Time",
      value: new Timestamp(microsSinceUnixEpoch)
    }), "Time");
    schedule_at_default = ScheduleAt;
    AlgebraicType = {
      ...AlgebraicType2,
      Sum: /* @__PURE__ */ __name((value) => ({
        tag: "Sum",
        value
      }), "Sum"),
      Product: /* @__PURE__ */ __name((value) => ({
        tag: "Product",
        value
      }), "Product"),
      Array: /* @__PURE__ */ __name((value) => ({
        tag: "Array",
        value
      }), "Array"),
      createOptionType: /* @__PURE__ */ __name(function(innerType) {
        return Option.getAlgebraicType(innerType);
      }, "createOptionType"),
      createIdentityType: /* @__PURE__ */ __name(function() {
        return Identity.getAlgebraicType();
      }, "createIdentityType"),
      createConnectionIdType: /* @__PURE__ */ __name(function() {
        return ConnectionId.getAlgebraicType();
      }, "createConnectionIdType"),
      createScheduleAtType: /* @__PURE__ */ __name(function() {
        return schedule_at_default.getAlgebraicType();
      }, "createScheduleAtType"),
      createTimestampType: /* @__PURE__ */ __name(function() {
        return Timestamp.getAlgebraicType();
      }, "createTimestampType"),
      createTimeDurationType: /* @__PURE__ */ __name(function() {
        return TimeDuration.getAlgebraicType();
      }, "createTimeDurationType"),
      serializeValue: /* @__PURE__ */ __name(function(writer, ty, value, typespace) {
        if (ty.tag === "Ref") {
          if (!typespace)
            throw new Error("cannot serialize refs without a typespace");
          while (ty.tag === "Ref") ty = typespace.types[ty.value];
        }
        switch (ty.tag) {
          case "Product":
            ProductType2.serializeValue(writer, ty.value, value, typespace);
            break;
          case "Sum":
            SumType2.serializeValue(writer, ty.value, value, typespace);
            break;
          case "Array":
            if (ty.value.tag === "U8") {
              writer.writeUInt8Array(value);
            } else {
              const elemType = ty.value;
              writer.writeU32(value.length);
              for (const elem of value) {
                AlgebraicType.serializeValue(writer, elemType, elem, typespace);
              }
            }
            break;
          case "Bool":
            writer.writeBool(value);
            break;
          case "I8":
            writer.writeI8(value);
            break;
          case "U8":
            writer.writeU8(value);
            break;
          case "I16":
            writer.writeI16(value);
            break;
          case "U16":
            writer.writeU16(value);
            break;
          case "I32":
            writer.writeI32(value);
            break;
          case "U32":
            writer.writeU32(value);
            break;
          case "I64":
            writer.writeI64(value);
            break;
          case "U64":
            writer.writeU64(value);
            break;
          case "I128":
            writer.writeI128(value);
            break;
          case "U128":
            writer.writeU128(value);
            break;
          case "I256":
            writer.writeI256(value);
            break;
          case "U256":
            writer.writeU256(value);
            break;
          case "F32":
            writer.writeF32(value);
            break;
          case "F64":
            writer.writeF64(value);
            break;
          case "String":
            writer.writeString(value);
            break;
        }
      }, "serializeValue"),
      deserializeValue: /* @__PURE__ */ __name(function(reader, ty, typespace) {
        if (ty.tag === "Ref") {
          if (!typespace)
            throw new Error("cannot deserialize refs without a typespace");
          while (ty.tag === "Ref") ty = typespace.types[ty.value];
        }
        switch (ty.tag) {
          case "Product":
            return ProductType2.deserializeValue(reader, ty.value, typespace);
          case "Sum":
            return SumType2.deserializeValue(reader, ty.value, typespace);
          case "Array":
            if (ty.value.tag === "U8") {
              return reader.readUInt8Array();
            } else {
              const elemType = ty.value;
              const length = reader.readU32();
              const result = [];
              for (let i = 0; i < length; i++) {
                result.push(
                  AlgebraicType.deserializeValue(reader, elemType, typespace)
                );
              }
              return result;
            }
          case "Bool":
            return reader.readBool();
          case "I8":
            return reader.readI8();
          case "U8":
            return reader.readU8();
          case "I16":
            return reader.readI16();
          case "U16":
            return reader.readU16();
          case "I32":
            return reader.readI32();
          case "U32":
            return reader.readU32();
          case "I64":
            return reader.readI64();
          case "U64":
            return reader.readU64();
          case "I128":
            return reader.readI128();
          case "U128":
            return reader.readU128();
          case "I256":
            return reader.readI256();
          case "U256":
            return reader.readU256();
          case "F32":
            return reader.readF32();
          case "F64":
            return reader.readF64();
          case "String":
            return reader.readString();
        }
      }, "deserializeValue"),
      /**
       * Convert a value of the algebraic type into something that can be used as a key in a map.
       * There are no guarantees about being able to order it.
       * This is only guaranteed to be comparable to other values of the same type.
       * @param value A value of the algebraic type
       * @returns Something that can be used as a key in a map.
       */
      intoMapKey: /* @__PURE__ */ __name(function(ty, value) {
        switch (ty.tag) {
          case "U8":
          case "U16":
          case "U32":
          case "U64":
          case "U128":
          case "U256":
          case "I8":
          case "I16":
          case "I32":
          case "I64":
          case "I128":
          case "I256":
          case "F32":
          case "F64":
          case "String":
          case "Bool":
            return value;
          case "Product":
            return ProductType2.intoMapKey(ty.value, value);
          default: {
            const writer = new BinaryWriter(10);
            AlgebraicType.serializeValue(writer, ty, value);
            return writer.toBase64();
          }
        }
      }, "intoMapKey")
    };
    ProductType2 = {
      ...ProductType,
      serializeValue(writer, ty, value, typespace) {
        for (const element of ty.elements) {
          AlgebraicType.serializeValue(
            writer,
            element.algebraicType,
            value[element.name],
            typespace
          );
        }
      },
      deserializeValue(reader, ty, typespace) {
        const result = {};
        if (ty.elements.length === 1) {
          if (ty.elements[0].name === "__time_duration_micros__") {
            return new TimeDuration(reader.readI64());
          }
          if (ty.elements[0].name === "__timestamp_micros_since_unix_epoch__") {
            return new Timestamp(reader.readI64());
          }
          if (ty.elements[0].name === "__identity__") {
            return new Identity(reader.readU256());
          }
          if (ty.elements[0].name === "__connection_id__") {
            return new ConnectionId(reader.readU128());
          }
        }
        for (const element of ty.elements) {
          result[element.name] = AlgebraicType.deserializeValue(
            reader,
            element.algebraicType,
            typespace
          );
        }
        return result;
      },
      intoMapKey(ty, value) {
        if (ty.elements.length === 1) {
          if (ty.elements[0].name === "__time_duration_micros__") {
            return value.__time_duration_micros__;
          }
          if (ty.elements[0].name === "__timestamp_micros_since_unix_epoch__") {
            return value.__timestamp_micros_since_unix_epoch__;
          }
          if (ty.elements[0].name === "__identity__") {
            return value.__identity__;
          }
          if (ty.elements[0].name === "__connection_id__") {
            return value.__connection_id__;
          }
        }
        const writer = new BinaryWriter(10);
        AlgebraicType.serializeValue(writer, AlgebraicType.Product(ty), value);
        return writer.toBase64();
      }
    };
    SumType2 = {
      ...SumType,
      serializeValue: /* @__PURE__ */ __name(function(writer, ty, value, typespace) {
        if (ty.variants.length == 2 && ty.variants[0].name === "some" && ty.variants[1].name === "none") {
          if (value !== null && value !== void 0) {
            writer.writeByte(0);
            AlgebraicType.serializeValue(
              writer,
              ty.variants[0].algebraicType,
              value,
              typespace
            );
          } else {
            writer.writeByte(1);
          }
        } else {
          const variant = value["tag"];
          const index5 = ty.variants.findIndex((v) => v.name === variant);
          if (index5 < 0) {
            throw `Can't serialize a sum type, couldn't find ${value.tag} tag`;
          }
          writer.writeU8(index5);
          AlgebraicType.serializeValue(
            writer,
            ty.variants[index5].algebraicType,
            value["value"],
            typespace
          );
        }
      }, "serializeValue"),
      deserializeValue: /* @__PURE__ */ __name(function(reader, ty, typespace) {
        const tag = reader.readU8();
        if (ty.variants.length == 2 && ty.variants[0].name === "some" && ty.variants[1].name === "none") {
          if (tag === 0) {
            return AlgebraicType.deserializeValue(
              reader,
              ty.variants[0].algebraicType,
              typespace
            );
          } else if (tag === 1) {
            return void 0;
          } else {
            throw `Can't deserialize an option type, couldn't find ${tag} tag`;
          }
        } else {
          const variant = ty.variants[tag];
          const value = AlgebraicType.deserializeValue(
            reader,
            variant.algebraicType,
            typespace
          );
          return { tag: variant.name, value };
        }
      }, "deserializeValue")
    };
    ConnectionId = class _ConnectionId {
      static {
        __name(this, "_ConnectionId");
      }
      __connection_id__;
      /**
       * Creates a new `ConnectionId`.
       */
      constructor(data) {
        this.__connection_id__ = data;
      }
      /**
       * Get the algebraic type representation of the {@link ConnectionId} type.
       * @returns The algebraic type representation of the type.
       */
      static getAlgebraicType() {
        return AlgebraicType.Product({
          elements: [
            { name: "__connection_id__", algebraicType: AlgebraicType.U128 }
          ]
        });
      }
      isZero() {
        return this.__connection_id__ === BigInt(0);
      }
      static nullIfZero(addr) {
        if (addr.isZero()) {
          return null;
        } else {
          return addr;
        }
      }
      static random() {
        function randomU8() {
          return Math.floor(Math.random() * 255);
        }
        __name(randomU8, "randomU8");
        let result = BigInt(0);
        for (let i = 0; i < 16; i++) {
          result = result << BigInt(8) | BigInt(randomU8());
        }
        return new _ConnectionId(result);
      }
      /**
       * Compare two connection IDs for equality.
       */
      isEqual(other) {
        return this.__connection_id__ == other.__connection_id__;
      }
      /**
       * Print the connection ID as a hexadecimal string.
       */
      toHexString() {
        return u128ToHexString(this.__connection_id__);
      }
      /**
       * Convert the connection ID to a Uint8Array.
       */
      toUint8Array() {
        return u128ToUint8Array(this.__connection_id__);
      }
      /**
       * Parse a connection ID from a hexadecimal string.
       */
      static fromString(str) {
        return new _ConnectionId(hexStringToU128(str));
      }
      static fromStringOrNull(str) {
        const addr = _ConnectionId.fromString(str);
        if (addr.isZero()) {
          return null;
        } else {
          return addr;
        }
      }
    };
    __name(parseValue, "parseValue");
    _cached_RowSizeHint_type_value = null;
    RowSizeHint = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      FixedSize: /* @__PURE__ */ __name((value) => ({
        tag: "FixedSize",
        value
      }), "FixedSize"),
      RowOffsets: /* @__PURE__ */ __name((value) => ({
        tag: "RowOffsets",
        value
      }), "RowOffsets"),
      getTypeScriptAlgebraicType() {
        if (_cached_RowSizeHint_type_value) return _cached_RowSizeHint_type_value;
        _cached_RowSizeHint_type_value = AlgebraicType.Sum({ variants: [] });
        _cached_RowSizeHint_type_value.value.variants.push(
          { name: "FixedSize", algebraicType: AlgebraicType.U16 },
          {
            name: "RowOffsets",
            algebraicType: AlgebraicType.Array(AlgebraicType.U64)
          }
        );
        return _cached_RowSizeHint_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          RowSizeHint.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          RowSizeHint.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_BsatnRowList_type_value = null;
    BsatnRowList = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_BsatnRowList_type_value) return _cached_BsatnRowList_type_value;
        _cached_BsatnRowList_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_BsatnRowList_type_value.value.elements.push(
          {
            name: "sizeHint",
            algebraicType: RowSizeHint.getTypeScriptAlgebraicType()
          },
          {
            name: "rowsData",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          }
        );
        return _cached_BsatnRowList_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          BsatnRowList.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          BsatnRowList.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_CallReducer_type_value = null;
    CallReducer = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_CallReducer_type_value) return _cached_CallReducer_type_value;
        _cached_CallReducer_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_CallReducer_type_value.value.elements.push(
          { name: "reducer", algebraicType: AlgebraicType.String },
          {
            name: "args",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          },
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          { name: "flags", algebraicType: AlgebraicType.U8 }
        );
        return _cached_CallReducer_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          CallReducer.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          CallReducer.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_Subscribe_type_value = null;
    Subscribe = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_Subscribe_type_value) return _cached_Subscribe_type_value;
        _cached_Subscribe_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_Subscribe_type_value.value.elements.push(
          {
            name: "queryStrings",
            algebraicType: AlgebraicType.Array(AlgebraicType.String)
          },
          { name: "requestId", algebraicType: AlgebraicType.U32 }
        );
        return _cached_Subscribe_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          Subscribe.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          Subscribe.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_OneOffQuery_type_value = null;
    OneOffQuery = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_OneOffQuery_type_value) return _cached_OneOffQuery_type_value;
        _cached_OneOffQuery_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_OneOffQuery_type_value.value.elements.push(
          {
            name: "messageId",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          },
          { name: "queryString", algebraicType: AlgebraicType.String }
        );
        return _cached_OneOffQuery_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          OneOffQuery.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          OneOffQuery.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_QueryId_type_value = null;
    QueryId = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_QueryId_type_value) return _cached_QueryId_type_value;
        _cached_QueryId_type_value = AlgebraicType.Product({ elements: [] });
        _cached_QueryId_type_value.value.elements.push({
          name: "id",
          algebraicType: AlgebraicType.U32
        });
        return _cached_QueryId_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          QueryId.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          QueryId.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscribeSingle_type_value = null;
    SubscribeSingle = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscribeSingle_type_value)
          return _cached_SubscribeSingle_type_value;
        _cached_SubscribeSingle_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscribeSingle_type_value.value.elements.push(
          { name: "query", algebraicType: AlgebraicType.String },
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() }
        );
        return _cached_SubscribeSingle_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscribeSingle.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscribeSingle.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscribeMulti_type_value = null;
    SubscribeMulti = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscribeMulti_type_value)
          return _cached_SubscribeMulti_type_value;
        _cached_SubscribeMulti_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscribeMulti_type_value.value.elements.push(
          {
            name: "queryStrings",
            algebraicType: AlgebraicType.Array(AlgebraicType.String)
          },
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() }
        );
        return _cached_SubscribeMulti_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscribeMulti.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscribeMulti.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_Unsubscribe_type_value = null;
    Unsubscribe = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_Unsubscribe_type_value) return _cached_Unsubscribe_type_value;
        _cached_Unsubscribe_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_Unsubscribe_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() }
        );
        return _cached_Unsubscribe_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          Unsubscribe.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          Unsubscribe.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_UnsubscribeMulti_type_value = null;
    UnsubscribeMulti = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_UnsubscribeMulti_type_value)
          return _cached_UnsubscribeMulti_type_value;
        _cached_UnsubscribeMulti_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_UnsubscribeMulti_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() }
        );
        return _cached_UnsubscribeMulti_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          UnsubscribeMulti.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          UnsubscribeMulti.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_ClientMessage_type_value = null;
    ClientMessage = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      CallReducer: /* @__PURE__ */ __name((value) => ({
        tag: "CallReducer",
        value
      }), "CallReducer"),
      Subscribe: /* @__PURE__ */ __name((value) => ({
        tag: "Subscribe",
        value
      }), "Subscribe"),
      OneOffQuery: /* @__PURE__ */ __name((value) => ({
        tag: "OneOffQuery",
        value
      }), "OneOffQuery"),
      SubscribeSingle: /* @__PURE__ */ __name((value) => ({
        tag: "SubscribeSingle",
        value
      }), "SubscribeSingle"),
      SubscribeMulti: /* @__PURE__ */ __name((value) => ({ tag: "SubscribeMulti", value }), "SubscribeMulti"),
      Unsubscribe: /* @__PURE__ */ __name((value) => ({
        tag: "Unsubscribe",
        value
      }), "Unsubscribe"),
      UnsubscribeMulti: /* @__PURE__ */ __name((value) => ({
        tag: "UnsubscribeMulti",
        value
      }), "UnsubscribeMulti"),
      getTypeScriptAlgebraicType() {
        if (_cached_ClientMessage_type_value)
          return _cached_ClientMessage_type_value;
        _cached_ClientMessage_type_value = AlgebraicType.Sum({
          variants: []
        });
        _cached_ClientMessage_type_value.value.variants.push(
          {
            name: "CallReducer",
            algebraicType: CallReducer.getTypeScriptAlgebraicType()
          },
          {
            name: "Subscribe",
            algebraicType: Subscribe.getTypeScriptAlgebraicType()
          },
          {
            name: "OneOffQuery",
            algebraicType: OneOffQuery.getTypeScriptAlgebraicType()
          },
          {
            name: "SubscribeSingle",
            algebraicType: SubscribeSingle.getTypeScriptAlgebraicType()
          },
          {
            name: "SubscribeMulti",
            algebraicType: SubscribeMulti.getTypeScriptAlgebraicType()
          },
          {
            name: "Unsubscribe",
            algebraicType: Unsubscribe.getTypeScriptAlgebraicType()
          },
          {
            name: "UnsubscribeMulti",
            algebraicType: UnsubscribeMulti.getTypeScriptAlgebraicType()
          }
        );
        return _cached_ClientMessage_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          ClientMessage.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          ClientMessage.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_QueryUpdate_type_value = null;
    QueryUpdate = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_QueryUpdate_type_value) return _cached_QueryUpdate_type_value;
        _cached_QueryUpdate_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_QueryUpdate_type_value.value.elements.push(
          {
            name: "deletes",
            algebraicType: BsatnRowList.getTypeScriptAlgebraicType()
          },
          {
            name: "inserts",
            algebraicType: BsatnRowList.getTypeScriptAlgebraicType()
          }
        );
        return _cached_QueryUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          QueryUpdate.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          QueryUpdate.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_CompressableQueryUpdate_type_value = null;
    CompressableQueryUpdate = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      Uncompressed: /* @__PURE__ */ __name((value) => ({
        tag: "Uncompressed",
        value
      }), "Uncompressed"),
      Brotli: /* @__PURE__ */ __name((value) => ({
        tag: "Brotli",
        value
      }), "Brotli"),
      Gzip: /* @__PURE__ */ __name((value) => ({
        tag: "Gzip",
        value
      }), "Gzip"),
      getTypeScriptAlgebraicType() {
        if (_cached_CompressableQueryUpdate_type_value)
          return _cached_CompressableQueryUpdate_type_value;
        _cached_CompressableQueryUpdate_type_value = AlgebraicType.Sum({
          variants: []
        });
        _cached_CompressableQueryUpdate_type_value.value.variants.push(
          {
            name: "Uncompressed",
            algebraicType: QueryUpdate.getTypeScriptAlgebraicType()
          },
          {
            name: "Brotli",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          },
          {
            name: "Gzip",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          }
        );
        return _cached_CompressableQueryUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          CompressableQueryUpdate.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          CompressableQueryUpdate.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_TableUpdate_type_value = null;
    TableUpdate = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_TableUpdate_type_value) return _cached_TableUpdate_type_value;
        _cached_TableUpdate_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_TableUpdate_type_value.value.elements.push(
          { name: "tableId", algebraicType: AlgebraicType.U32 },
          { name: "tableName", algebraicType: AlgebraicType.String },
          { name: "numRows", algebraicType: AlgebraicType.U64 },
          {
            name: "updates",
            algebraicType: AlgebraicType.Array(
              CompressableQueryUpdate.getTypeScriptAlgebraicType()
            )
          }
        );
        return _cached_TableUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          TableUpdate.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          TableUpdate.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_DatabaseUpdate_type_value = null;
    DatabaseUpdate = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_DatabaseUpdate_type_value)
          return _cached_DatabaseUpdate_type_value;
        _cached_DatabaseUpdate_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_DatabaseUpdate_type_value.value.elements.push({
          name: "tables",
          algebraicType: AlgebraicType.Array(
            TableUpdate.getTypeScriptAlgebraicType()
          )
        });
        return _cached_DatabaseUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          DatabaseUpdate.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          DatabaseUpdate.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_InitialSubscription_type_value = null;
    InitialSubscription = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_InitialSubscription_type_value)
          return _cached_InitialSubscription_type_value;
        _cached_InitialSubscription_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_InitialSubscription_type_value.value.elements.push(
          {
            name: "databaseUpdate",
            algebraicType: DatabaseUpdate.getTypeScriptAlgebraicType()
          },
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "totalHostExecutionDuration",
            algebraicType: AlgebraicType.createTimeDurationType()
          }
        );
        return _cached_InitialSubscription_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          InitialSubscription.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          InitialSubscription.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_UpdateStatus_type_value = null;
    UpdateStatus = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      Committed: /* @__PURE__ */ __name((value) => ({
        tag: "Committed",
        value
      }), "Committed"),
      Failed: /* @__PURE__ */ __name((value) => ({
        tag: "Failed",
        value
      }), "Failed"),
      OutOfEnergy: { tag: "OutOfEnergy" },
      getTypeScriptAlgebraicType() {
        if (_cached_UpdateStatus_type_value) return _cached_UpdateStatus_type_value;
        _cached_UpdateStatus_type_value = AlgebraicType.Sum({
          variants: []
        });
        _cached_UpdateStatus_type_value.value.variants.push(
          {
            name: "Committed",
            algebraicType: DatabaseUpdate.getTypeScriptAlgebraicType()
          },
          { name: "Failed", algebraicType: AlgebraicType.String },
          {
            name: "OutOfEnergy",
            algebraicType: AlgebraicType.Product({ elements: [] })
          }
        );
        return _cached_UpdateStatus_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          UpdateStatus.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          UpdateStatus.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_ReducerCallInfo_type_value = null;
    ReducerCallInfo = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_ReducerCallInfo_type_value)
          return _cached_ReducerCallInfo_type_value;
        _cached_ReducerCallInfo_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_ReducerCallInfo_type_value.value.elements.push(
          { name: "reducerName", algebraicType: AlgebraicType.String },
          { name: "reducerId", algebraicType: AlgebraicType.U32 },
          {
            name: "args",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          },
          { name: "requestId", algebraicType: AlgebraicType.U32 }
        );
        return _cached_ReducerCallInfo_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          ReducerCallInfo.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          ReducerCallInfo.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_EnergyQuanta_type_value = null;
    EnergyQuanta = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_EnergyQuanta_type_value) return _cached_EnergyQuanta_type_value;
        _cached_EnergyQuanta_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_EnergyQuanta_type_value.value.elements.push({
          name: "quanta",
          algebraicType: AlgebraicType.U128
        });
        return _cached_EnergyQuanta_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          EnergyQuanta.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          EnergyQuanta.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_TransactionUpdate_type_value = null;
    TransactionUpdate = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_TransactionUpdate_type_value)
          return _cached_TransactionUpdate_type_value;
        _cached_TransactionUpdate_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_TransactionUpdate_type_value.value.elements.push(
          {
            name: "status",
            algebraicType: UpdateStatus.getTypeScriptAlgebraicType()
          },
          {
            name: "timestamp",
            algebraicType: AlgebraicType.createTimestampType()
          },
          {
            name: "callerIdentity",
            algebraicType: AlgebraicType.createIdentityType()
          },
          {
            name: "callerConnectionId",
            algebraicType: AlgebraicType.createConnectionIdType()
          },
          {
            name: "reducerCall",
            algebraicType: ReducerCallInfo.getTypeScriptAlgebraicType()
          },
          {
            name: "energyQuantaUsed",
            algebraicType: EnergyQuanta.getTypeScriptAlgebraicType()
          },
          {
            name: "totalHostExecutionDuration",
            algebraicType: AlgebraicType.createTimeDurationType()
          }
        );
        return _cached_TransactionUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          TransactionUpdate.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          TransactionUpdate.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_TransactionUpdateLight_type_value = null;
    TransactionUpdateLight = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_TransactionUpdateLight_type_value)
          return _cached_TransactionUpdateLight_type_value;
        _cached_TransactionUpdateLight_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_TransactionUpdateLight_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "update",
            algebraicType: DatabaseUpdate.getTypeScriptAlgebraicType()
          }
        );
        return _cached_TransactionUpdateLight_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          TransactionUpdateLight.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          TransactionUpdateLight.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_IdentityToken_type_value = null;
    IdentityToken = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_IdentityToken_type_value)
          return _cached_IdentityToken_type_value;
        _cached_IdentityToken_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_IdentityToken_type_value.value.elements.push(
          {
            name: "identity",
            algebraicType: AlgebraicType.createIdentityType()
          },
          { name: "token", algebraicType: AlgebraicType.String },
          {
            name: "connectionId",
            algebraicType: AlgebraicType.createConnectionIdType()
          }
        );
        return _cached_IdentityToken_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          IdentityToken.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          IdentityToken.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_OneOffTable_type_value = null;
    OneOffTable = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_OneOffTable_type_value) return _cached_OneOffTable_type_value;
        _cached_OneOffTable_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_OneOffTable_type_value.value.elements.push(
          { name: "tableName", algebraicType: AlgebraicType.String },
          { name: "rows", algebraicType: BsatnRowList.getTypeScriptAlgebraicType() }
        );
        return _cached_OneOffTable_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          OneOffTable.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          OneOffTable.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_OneOffQueryResponse_type_value = null;
    OneOffQueryResponse = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_OneOffQueryResponse_type_value)
          return _cached_OneOffQueryResponse_type_value;
        _cached_OneOffQueryResponse_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_OneOffQueryResponse_type_value.value.elements.push(
          {
            name: "messageId",
            algebraicType: AlgebraicType.Array(AlgebraicType.U8)
          },
          {
            name: "error",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.String
            )
          },
          {
            name: "tables",
            algebraicType: AlgebraicType.Array(
              OneOffTable.getTypeScriptAlgebraicType()
            )
          },
          {
            name: "totalHostExecutionDuration",
            algebraicType: AlgebraicType.createTimeDurationType()
          }
        );
        return _cached_OneOffQueryResponse_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          OneOffQueryResponse.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          OneOffQueryResponse.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscribeRows_type_value = null;
    SubscribeRows = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscribeRows_type_value)
          return _cached_SubscribeRows_type_value;
        _cached_SubscribeRows_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscribeRows_type_value.value.elements.push(
          { name: "tableId", algebraicType: AlgebraicType.U32 },
          { name: "tableName", algebraicType: AlgebraicType.String },
          {
            name: "tableRows",
            algebraicType: TableUpdate.getTypeScriptAlgebraicType()
          }
        );
        return _cached_SubscribeRows_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscribeRows.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscribeRows.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscribeApplied_type_value = null;
    SubscribeApplied = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscribeApplied_type_value)
          return _cached_SubscribeApplied_type_value;
        _cached_SubscribeApplied_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscribeApplied_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "totalHostExecutionDurationMicros",
            algebraicType: AlgebraicType.U64
          },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() },
          {
            name: "rows",
            algebraicType: SubscribeRows.getTypeScriptAlgebraicType()
          }
        );
        return _cached_SubscribeApplied_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscribeApplied.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscribeApplied.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_UnsubscribeApplied_type_value = null;
    UnsubscribeApplied = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_UnsubscribeApplied_type_value)
          return _cached_UnsubscribeApplied_type_value;
        _cached_UnsubscribeApplied_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_UnsubscribeApplied_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "totalHostExecutionDurationMicros",
            algebraicType: AlgebraicType.U64
          },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() },
          {
            name: "rows",
            algebraicType: SubscribeRows.getTypeScriptAlgebraicType()
          }
        );
        return _cached_UnsubscribeApplied_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          UnsubscribeApplied.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          UnsubscribeApplied.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscriptionError_type_value = null;
    SubscriptionError = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscriptionError_type_value)
          return _cached_SubscriptionError_type_value;
        _cached_SubscriptionError_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscriptionError_type_value.value.elements.push(
          {
            name: "totalHostExecutionDurationMicros",
            algebraicType: AlgebraicType.U64
          },
          {
            name: "requestId",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.U32
            )
          },
          {
            name: "queryId",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.U32
            )
          },
          {
            name: "tableId",
            algebraicType: AlgebraicType.createOptionType(
              AlgebraicType.U32
            )
          },
          { name: "error", algebraicType: AlgebraicType.String }
        );
        return _cached_SubscriptionError_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscriptionError.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscriptionError.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_SubscribeMultiApplied_type_value = null;
    SubscribeMultiApplied = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_SubscribeMultiApplied_type_value)
          return _cached_SubscribeMultiApplied_type_value;
        _cached_SubscribeMultiApplied_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_SubscribeMultiApplied_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "totalHostExecutionDurationMicros",
            algebraicType: AlgebraicType.U64
          },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() },
          {
            name: "update",
            algebraicType: DatabaseUpdate.getTypeScriptAlgebraicType()
          }
        );
        return _cached_SubscribeMultiApplied_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          SubscribeMultiApplied.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          SubscribeMultiApplied.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_UnsubscribeMultiApplied_type_value = null;
    UnsubscribeMultiApplied = {
      /**
       * A function which returns this type represented as an AlgebraicType.
       * This function is derived from the AlgebraicType used to generate this type.
       */
      getTypeScriptAlgebraicType() {
        if (_cached_UnsubscribeMultiApplied_type_value)
          return _cached_UnsubscribeMultiApplied_type_value;
        _cached_UnsubscribeMultiApplied_type_value = AlgebraicType.Product({
          elements: []
        });
        _cached_UnsubscribeMultiApplied_type_value.value.elements.push(
          { name: "requestId", algebraicType: AlgebraicType.U32 },
          {
            name: "totalHostExecutionDurationMicros",
            algebraicType: AlgebraicType.U64
          },
          { name: "queryId", algebraicType: QueryId.getTypeScriptAlgebraicType() },
          {
            name: "update",
            algebraicType: DatabaseUpdate.getTypeScriptAlgebraicType()
          }
        );
        return _cached_UnsubscribeMultiApplied_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          UnsubscribeMultiApplied.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          UnsubscribeMultiApplied.getTypeScriptAlgebraicType()
        );
      }
    };
    _cached_ServerMessage_type_value = null;
    ServerMessage = {
      // Helper functions for constructing each variant of the tagged union.
      // ```
      // const foo = Foo.A(42);
      // assert!(foo.tag === "A");
      // assert!(foo.value === 42);
      // ```
      InitialSubscription: /* @__PURE__ */ __name((value) => ({
        tag: "InitialSubscription",
        value
      }), "InitialSubscription"),
      TransactionUpdate: /* @__PURE__ */ __name((value) => ({
        tag: "TransactionUpdate",
        value
      }), "TransactionUpdate"),
      TransactionUpdateLight: /* @__PURE__ */ __name((value) => ({
        tag: "TransactionUpdateLight",
        value
      }), "TransactionUpdateLight"),
      IdentityToken: /* @__PURE__ */ __name((value) => ({ tag: "IdentityToken", value }), "IdentityToken"),
      OneOffQueryResponse: /* @__PURE__ */ __name((value) => ({
        tag: "OneOffQueryResponse",
        value
      }), "OneOffQueryResponse"),
      SubscribeApplied: /* @__PURE__ */ __name((value) => ({
        tag: "SubscribeApplied",
        value
      }), "SubscribeApplied"),
      UnsubscribeApplied: /* @__PURE__ */ __name((value) => ({
        tag: "UnsubscribeApplied",
        value
      }), "UnsubscribeApplied"),
      SubscriptionError: /* @__PURE__ */ __name((value) => ({
        tag: "SubscriptionError",
        value
      }), "SubscriptionError"),
      SubscribeMultiApplied: /* @__PURE__ */ __name((value) => ({
        tag: "SubscribeMultiApplied",
        value
      }), "SubscribeMultiApplied"),
      UnsubscribeMultiApplied: /* @__PURE__ */ __name((value) => ({
        tag: "UnsubscribeMultiApplied",
        value
      }), "UnsubscribeMultiApplied"),
      getTypeScriptAlgebraicType() {
        if (_cached_ServerMessage_type_value)
          return _cached_ServerMessage_type_value;
        _cached_ServerMessage_type_value = AlgebraicType.Sum({
          variants: []
        });
        _cached_ServerMessage_type_value.value.variants.push(
          {
            name: "InitialSubscription",
            algebraicType: InitialSubscription.getTypeScriptAlgebraicType()
          },
          {
            name: "TransactionUpdate",
            algebraicType: TransactionUpdate.getTypeScriptAlgebraicType()
          },
          {
            name: "TransactionUpdateLight",
            algebraicType: TransactionUpdateLight.getTypeScriptAlgebraicType()
          },
          {
            name: "IdentityToken",
            algebraicType: IdentityToken.getTypeScriptAlgebraicType()
          },
          {
            name: "OneOffQueryResponse",
            algebraicType: OneOffQueryResponse.getTypeScriptAlgebraicType()
          },
          {
            name: "SubscribeApplied",
            algebraicType: SubscribeApplied.getTypeScriptAlgebraicType()
          },
          {
            name: "UnsubscribeApplied",
            algebraicType: UnsubscribeApplied.getTypeScriptAlgebraicType()
          },
          {
            name: "SubscriptionError",
            algebraicType: SubscriptionError.getTypeScriptAlgebraicType()
          },
          {
            name: "SubscribeMultiApplied",
            algebraicType: SubscribeMultiApplied.getTypeScriptAlgebraicType()
          },
          {
            name: "UnsubscribeMultiApplied",
            algebraicType: UnsubscribeMultiApplied.getTypeScriptAlgebraicType()
          }
        );
        return _cached_ServerMessage_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(
          writer,
          ServerMessage.getTypeScriptAlgebraicType(),
          value
        );
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(
          reader,
          ServerMessage.getTypeScriptAlgebraicType()
        );
      }
    };
    EventEmitter2 = class {
      static {
        __name(this, "EventEmitter");
      }
      #events = /* @__PURE__ */ new Map();
      on(event, callback) {
        let callbacks = this.#events.get(event);
        if (!callbacks) {
          callbacks = /* @__PURE__ */ new Set();
          this.#events.set(event, callbacks);
        }
        callbacks.add(callback);
      }
      off(event, callback) {
        const callbacks = this.#events.get(event);
        if (!callbacks) {
          return;
        }
        callbacks.delete(callback);
      }
      emit(event, ...args) {
        const callbacks = this.#events.get(event);
        if (!callbacks) {
          return;
        }
        for (const callback of callbacks) {
          callback(...args);
        }
      }
    };
    LogLevelIdentifierIcon = {
      component: "\u{1F4E6}",
      info: "\u2139\uFE0F",
      warn: "\u26A0\uFE0F",
      error: "\u274C",
      debug: "\u{1F41B}"
    };
    LogStyle = {
      component: "color: #fff; background-color: #8D6FDD; padding: 2px 5px; border-radius: 3px;",
      info: "color: #fff; background-color: #007bff; padding: 2px 5px; border-radius: 3px;",
      warn: "color: #fff; background-color: #ffc107; padding: 2px 5px; border-radius: 3px;",
      error: "color: #fff; background-color: #dc3545; padding: 2px 5px; border-radius: 3px;",
      debug: "color: #fff; background-color: #28a745; padding: 2px 5px; border-radius: 3px;"
    };
    LogTextStyle = {
      component: "color: #8D6FDD;",
      info: "color: #007bff;",
      warn: "color: #ffc107;",
      error: "color: #dc3545;",
      debug: "color: #28a745;"
    };
    stdbLogger = /* @__PURE__ */ __name((level, message) => {
      console.log(
        `%c${LogLevelIdentifierIcon[level]} ${level.toUpperCase()}%c ${message}`,
        LogStyle[level],
        LogTextStyle[level]
      );
    }, "stdbLogger");
    TableCache = class {
      static {
        __name(this, "TableCache");
      }
      rows;
      tableTypeInfo;
      emitter;
      /**
       * @param name the table name
       * @param primaryKeyCol column index designated as `#[primarykey]`
       * @param primaryKey column name designated as `#[primarykey]`
       * @param entityClass the entityClass
       */
      constructor(tableTypeInfo) {
        this.tableTypeInfo = tableTypeInfo;
        this.rows = /* @__PURE__ */ new Map();
        this.emitter = new EventEmitter2();
      }
      /**
       * @returns number of rows in the table
       */
      count() {
        return this.rows.size;
      }
      /**
       * @returns The values of the rows in the table
       */
      iter() {
        return Array.from(this.rows.values()).map(([row]) => row);
      }
      applyOperations = /* @__PURE__ */ __name((operations, ctx) => {
        const pendingCallbacks = [];
        if (this.tableTypeInfo.primaryKeyInfo !== void 0) {
          const insertMap = /* @__PURE__ */ new Map();
          const deleteMap = /* @__PURE__ */ new Map();
          for (const op of operations) {
            if (op.type === "insert") {
              const [_, prevCount] = insertMap.get(op.rowId) || [op, 0];
              insertMap.set(op.rowId, [op, prevCount + 1]);
            } else {
              const [_, prevCount] = deleteMap.get(op.rowId) || [op, 0];
              deleteMap.set(op.rowId, [op, prevCount + 1]);
            }
          }
          for (const [primaryKey, [insertOp, refCount]] of insertMap) {
            const deleteEntry = deleteMap.get(primaryKey);
            if (deleteEntry) {
              const [_, deleteCount] = deleteEntry;
              const refCountDelta = refCount - deleteCount;
              const maybeCb = this.update(
                ctx,
                primaryKey,
                insertOp.row,
                refCountDelta
              );
              if (maybeCb) {
                pendingCallbacks.push(maybeCb);
              }
              deleteMap.delete(primaryKey);
            } else {
              const maybeCb = this.insert(ctx, insertOp, refCount);
              if (maybeCb) {
                pendingCallbacks.push(maybeCb);
              }
            }
          }
          for (const [deleteOp, refCount] of deleteMap.values()) {
            const maybeCb = this.delete(ctx, deleteOp, refCount);
            if (maybeCb) {
              pendingCallbacks.push(maybeCb);
            }
          }
        } else {
          for (const op of operations) {
            if (op.type === "insert") {
              const maybeCb = this.insert(ctx, op);
              if (maybeCb) {
                pendingCallbacks.push(maybeCb);
              }
            } else {
              const maybeCb = this.delete(ctx, op);
              if (maybeCb) {
                pendingCallbacks.push(maybeCb);
              }
            }
          }
        }
        return pendingCallbacks;
      }, "applyOperations");
      update = /* @__PURE__ */ __name((ctx, rowId, newRow, refCountDelta = 0) => {
        const existingEntry = this.rows.get(rowId);
        if (!existingEntry) {
          stdbLogger(
            "error",
            `Updating a row that was not present in the cache. Table: ${this.tableTypeInfo.tableName}, RowId: ${rowId}`
          );
          return void 0;
        }
        const [oldRow, previousCount] = existingEntry;
        const refCount = Math.max(1, previousCount + refCountDelta);
        if (previousCount + refCountDelta <= 0) {
          stdbLogger(
            "error",
            `Negative reference count for in table ${this.tableTypeInfo.tableName} row ${rowId} (${previousCount} + ${refCountDelta})`
          );
          return void 0;
        }
        this.rows.set(rowId, [newRow, refCount]);
        if (previousCount === 0) {
          stdbLogger(
            "error",
            `Updating a row id in table ${this.tableTypeInfo.tableName} which was not present in the cache (rowId: ${rowId})`
          );
          return {
            type: "insert",
            table: this.tableTypeInfo.tableName,
            cb: /* @__PURE__ */ __name(() => {
              this.emitter.emit("insert", ctx, newRow);
            }, "cb")
          };
        }
        return {
          type: "update",
          table: this.tableTypeInfo.tableName,
          cb: /* @__PURE__ */ __name(() => {
            this.emitter.emit("update", ctx, oldRow, newRow);
          }, "cb")
        };
      }, "update");
      insert = /* @__PURE__ */ __name((ctx, operation, count3 = 1) => {
        const [_, previousCount] = this.rows.get(operation.rowId) || [
          operation.row,
          0
        ];
        this.rows.set(operation.rowId, [operation.row, previousCount + count3]);
        if (previousCount === 0) {
          return {
            type: "insert",
            table: this.tableTypeInfo.tableName,
            cb: /* @__PURE__ */ __name(() => {
              this.emitter.emit("insert", ctx, operation.row);
            }, "cb")
          };
        }
        return void 0;
      }, "insert");
      delete = /* @__PURE__ */ __name((ctx, operation, count3 = 1) => {
        const [_, previousCount] = this.rows.get(operation.rowId) || [
          operation.row,
          0
        ];
        if (previousCount === 0) {
          stdbLogger("warn", "Deleting a row that was not present in the cache");
          return void 0;
        }
        if (previousCount <= count3) {
          this.rows.delete(operation.rowId);
          return {
            type: "delete",
            table: this.tableTypeInfo.tableName,
            cb: /* @__PURE__ */ __name(() => {
              this.emitter.emit("delete", ctx, operation.row);
            }, "cb")
          };
        }
        this.rows.set(operation.rowId, [operation.row, previousCount - count3]);
        return void 0;
      }, "delete");
      /**
       * Register a callback for when a row is newly inserted into the database.
       *
       * ```ts
       * User.onInsert((user, reducerEvent) => {
       *   if (reducerEvent) {
       *      console.log("New user on reducer", reducerEvent, user);
       *   } else {
       *      console.log("New user received during subscription update on insert", user);
       *  }
       * });
       * ```
       *
       * @param cb Callback to be called when a new row is inserted
       */
      onInsert = /* @__PURE__ */ __name((cb) => {
        this.emitter.on("insert", cb);
      }, "onInsert");
      /**
       * Register a callback for when a row is deleted from the database.
       *
       * ```ts
       * User.onDelete((user, reducerEvent) => {
       *   if (reducerEvent) {
       *      console.log("Deleted user on reducer", reducerEvent, user);
       *   } else {
       *      console.log("Deleted user received during subscription update on update", user);
       *  }
       * });
       * ```
       *
       * @param cb Callback to be called when a new row is inserted
       */
      onDelete = /* @__PURE__ */ __name((cb) => {
        this.emitter.on("delete", cb);
      }, "onDelete");
      /**
       * Register a callback for when a row is updated into the database.
       *
       * ```ts
       * User.onInsert((user, reducerEvent) => {
       *   if (reducerEvent) {
       *      console.log("Updated user on reducer", reducerEvent, user);
       *   } else {
       *      console.log("Updated user received during subscription update on delete", user);
       *  }
       * });
       * ```
       *
       * @param cb Callback to be called when a new row is inserted
       */
      onUpdate = /* @__PURE__ */ __name((cb) => {
        this.emitter.on("update", cb);
      }, "onUpdate");
      /**
       * Remove a callback for when a row is newly inserted into the database.
       *
       * @param cb Callback to be removed
       */
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        this.emitter.off("insert", cb);
      }, "removeOnInsert");
      /**
       * Remove a callback for when a row is deleted from the database.
       *
       * @param cb Callback to be removed
       */
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        this.emitter.off("delete", cb);
      }, "removeOnDelete");
      /**
       * Remove a callback for when a row is updated into the database.
       *
       * @param cb Callback to be removed
       */
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        this.emitter.off("update", cb);
      }, "removeOnUpdate");
    };
    ClientCache = class {
      static {
        __name(this, "ClientCache");
      }
      /**
       * The tables in the database.
       */
      tables;
      constructor() {
        this.tables = /* @__PURE__ */ new Map();
      }
      /**
       * Returns the table with the given name.
       * @param name The name of the table.
       * @returns The table
       */
      getTable(name) {
        const table3 = this.tables.get(name);
        if (!table3) {
          console.error(
            "The table has not been registered for this client. Please register the table before using it. If you have registered global tables using the SpacetimeDBClient.registerTables() or `registerTable()` method, please make sure that is executed first!"
          );
          throw new Error(`Table ${name} does not exist`);
        }
        return table3;
      }
      getOrCreateTable(tableTypeInfo) {
        let table3;
        if (!this.tables.has(tableTypeInfo.tableName)) {
          table3 = new TableCache(tableTypeInfo);
          this.tables.set(tableTypeInfo.tableName, table3);
        } else {
          table3 = this.tables.get(tableTypeInfo.tableName);
        }
        return table3;
      }
    };
    __name(comparePreReleases, "comparePreReleases");
    SemanticVersion = class _SemanticVersion {
      static {
        __name(this, "_SemanticVersion");
      }
      major;
      minor;
      patch;
      preRelease;
      buildInfo;
      constructor(major, minor, patch, preRelease = null, buildInfo = null) {
        this.major = major;
        this.minor = minor;
        this.patch = patch;
        this.preRelease = preRelease;
        this.buildInfo = buildInfo;
      }
      toString() {
        let versionString = `${this.major}.${this.minor}.${this.patch}`;
        if (this.preRelease) {
          versionString += `-${this.preRelease.join(".")}`;
        }
        if (this.buildInfo) {
          versionString += `+${this.buildInfo}`;
        }
        return versionString;
      }
      compare(other) {
        if (this.major !== other.major) {
          return this.major - other.major;
        }
        if (this.minor !== other.minor) {
          return this.minor - other.minor;
        }
        if (this.patch !== other.patch) {
          return this.patch - other.patch;
        }
        if (this.preRelease && other.preRelease) {
          return comparePreReleases(this.preRelease, other.preRelease);
        }
        if (this.preRelease) {
          return -1;
        }
        if (other.preRelease) {
          return -1;
        }
        return 0;
      }
      clone() {
        return new _SemanticVersion(
          this.major,
          this.minor,
          this.patch,
          this.preRelease ? [...this.preRelease] : null,
          this.buildInfo
        );
      }
      static parseVersionString(version2) {
        const regex = /^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-([\da-zA-Z-]+(?:\.[\da-zA-Z-]+)*))?(?:\+([\da-zA-Z-]+(?:\.[\da-zA-Z-]+)*))?$/;
        const match = version2.match(regex);
        if (!match) {
          throw new Error(`Invalid version string: ${version2}`);
        }
        const major = parseInt(match[1], 10);
        const minor = parseInt(match[2], 10);
        const patch = parseInt(match[3], 10);
        const preRelease = match[4] ? match[4].split(".").map((id) => isNaN(Number(id)) ? id : Number(id)) : null;
        const buildInfo = match[5] || null;
        return new _SemanticVersion(major, minor, patch, preRelease, buildInfo);
      }
    };
    _MINIMUM_CLI_VERSION = new SemanticVersion(
      1,
      4,
      0
    );
    __name(ensureMinimumVersionOrThrow, "ensureMinimumVersionOrThrow");
    __name(versionErrorMessage, "versionErrorMessage");
    __name(decompress, "decompress");
    __name(resolveWS, "resolveWS");
    WebsocketDecompressAdapter = class _WebsocketDecompressAdapter {
      static {
        __name(this, "_WebsocketDecompressAdapter");
      }
      onclose;
      onopen;
      onmessage;
      onerror;
      #ws;
      async #handleOnMessage(msg) {
        const buffer = new Uint8Array(msg.data);
        let decompressed;
        if (buffer[0] === 0) {
          decompressed = buffer.slice(1);
        } else if (buffer[0] === 1) {
          throw new Error(
            "Brotli Compression not supported. Please use gzip or none compression in withCompression method on DbConnection."
          );
        } else if (buffer[0] === 2) {
          decompressed = await decompress(buffer.slice(1), "gzip");
        } else {
          throw new Error(
            "Unexpected Compression Algorithm. Please use `gzip` or `none`"
          );
        }
        this.onmessage?.({ data: decompressed });
      }
      #handleOnOpen(msg) {
        this.onopen?.(msg);
      }
      #handleOnError(msg) {
        this.onerror?.(msg);
      }
      #handleOnClose(msg) {
        this.onclose?.(msg);
      }
      send(msg) {
        this.#ws.send(msg);
      }
      close() {
        this.#ws.close();
      }
      constructor(ws) {
        this.onmessage = void 0;
        this.onopen = void 0;
        this.onmessage = void 0;
        this.onerror = void 0;
        ws.onmessage = this.#handleOnMessage.bind(this);
        ws.onerror = this.#handleOnError.bind(this);
        ws.onclose = this.#handleOnClose.bind(this);
        ws.onopen = this.#handleOnOpen.bind(this);
        ws.binaryType = "arraybuffer";
        this.#ws = ws;
      }
      static async createWebSocketFn({
        url,
        nameOrAddress,
        wsProtocol,
        authToken,
        compression,
        lightMode,
        confirmedReads
      }) {
        const headers2 = new Headers();
        const WS = await resolveWS();
        let temporaryAuthToken = void 0;
        if (authToken) {
          headers2.set("Authorization", `Bearer ${authToken}`);
          const tokenUrl = new URL("v1/identity/websocket-token", url);
          tokenUrl.protocol = url.protocol === "wss:" ? "https:" : "http:";
          const response = await fetch(tokenUrl, { method: "POST", headers: headers2 });
          if (response.ok) {
            const { token } = await response.json();
            temporaryAuthToken = token;
          } else {
            return Promise.reject(
              new Error(`Failed to verify token: ${response.statusText}`)
            );
          }
        }
        const databaseUrl = new URL(`v1/database/${nameOrAddress}/subscribe`, url);
        if (temporaryAuthToken) {
          databaseUrl.searchParams.set("token", temporaryAuthToken);
        }
        databaseUrl.searchParams.set(
          "compression",
          compression === "gzip" ? "Gzip" : "None"
        );
        if (lightMode) {
          databaseUrl.searchParams.set("light", "true");
        }
        if (confirmedReads !== void 0) {
          databaseUrl.searchParams.set("confirmed", confirmedReads.toString());
        }
        const ws = new WS(databaseUrl.toString(), wsProtocol);
        return new _WebsocketDecompressAdapter(ws);
      }
    };
    DbConnectionBuilder = class {
      static {
        __name(this, "DbConnectionBuilder");
      }
      /**
       * Creates a new `DbConnectionBuilder` database client and set the initial parameters.
       *
       * Users are not expected to call this constructor directly. Instead, use the static method `DbConnection.builder()`.
       *
       * @param remoteModule The remote module to use to connect to the SpacetimeDB server.
       * @param dbConnectionConstructor The constructor to use to create a new `DbConnection`.
       */
      constructor(remoteModule, dbConnectionConstructor) {
        this.remoteModule = remoteModule;
        this.dbConnectionConstructor = dbConnectionConstructor;
        this.#createWSFn = WebsocketDecompressAdapter.createWebSocketFn;
      }
      #uri;
      #nameOrAddress;
      #identity;
      #token;
      #emitter = new EventEmitter2();
      #compression = "gzip";
      #lightMode = false;
      #confirmedReads;
      #createWSFn;
      /**
       * Set the URI of the SpacetimeDB server to connect to.
       *
       * @param uri The URI of the SpacetimeDB server to connect to.
       *
       **/
      withUri(uri) {
        this.#uri = new URL(uri);
        return this;
      }
      /**
       * Set the name or Identity of the database module to connect to.
       *
       * @param nameOrAddress
       *
       * @returns The `DbConnectionBuilder` instance.
       */
      withModuleName(nameOrAddress) {
        this.#nameOrAddress = nameOrAddress;
        return this;
      }
      /**
       * Set the identity of the client to connect to the database.
       *
       * @param token The credentials to use to authenticate with SpacetimeDB. This
       * is optional. You can store the token returned by the `onConnect` callback
       * to use in future connections.
       *
       * @returns The `DbConnectionBuilder` instance.
       */
      withToken(token) {
        this.#token = token;
        return this;
      }
      withWSFn(createWSFn) {
        this.#createWSFn = createWSFn;
        return this;
      }
      /**
       * Set the compression algorithm to use for the connection.
       *
       * @param compression The compression algorithm to use for the connection.
       */
      withCompression(compression) {
        this.#compression = compression;
        return this;
      }
      /**
       * Sets the connection to operate in light mode.
       *
       * Light mode is a mode that reduces the amount of data sent over the network.
       *
       * @param lightMode The light mode for the connection.
       */
      withLightMode(lightMode) {
        this.#lightMode = lightMode;
        return this;
      }
      /**
       * Sets the connection to use confirmed reads.
       *
       * When enabled, the server will send query results only after they are
       * confirmed to be durable.
       *
       * What durable means depends on the server configuration: a single node
       * server may consider a transaction durable once it is `fsync`'ed to disk,
       * whereas a cluster may require that some number of replicas have
       * acknowledge that they have stored the transactions.
       *
       * Note that enabling confirmed reads will increase the latency between a
       * reducer call and the corresponding subscription update arriving at the
       * client.
       *
       * If this method is not called, not preference is sent to the server, and
       * the server will choose the default.
       *
       * @param confirmedReads `true` to enable confirmed reads, `false` to disable.
       */
      withConfirmedReads(confirmedReads) {
        this.#confirmedReads = confirmedReads;
        return this;
      }
      /**
       * Register a callback to be invoked upon authentication with the database.
       *
       * @param identity A unique identifier for a client connected to a database.
       * @param token The credentials to use to authenticate with SpacetimeDB.
       *
       * @returns The `DbConnectionBuilder` instance.
       *
       * The callback will be invoked with the `Identity` and private authentication `token` provided by the database to identify this connection.
       *
       * If credentials were supplied to connect, those passed to the callback will be equivalent to the ones used to connect.
       *
       * If the initial connection was anonymous, a new set of credentials will be generated by the database to identify this user.
       *
       * The credentials passed to the callback can be saved and used to authenticate the same user in future connections.
       *
       * @example
       *
       * ```ts
       * DbConnection.builder().onConnect((ctx, identity, token) => {
       *  console.log("Connected to SpacetimeDB with identity:", identity.toHexString());
       * });
       * ```
       */
      onConnect(callback) {
        this.#emitter.on("connect", callback);
        return this;
      }
      /**
       * Register a callback to be invoked upon an error.
       *
       * @example
       *
       * ```ts
       * DbConnection.builder().onConnectError((ctx, error) => {
       *   console.log("Error connecting to SpacetimeDB:", error);
       * });
       * ```
       */
      onConnectError(callback) {
        this.#emitter.on("connectError", callback);
        return this;
      }
      /**
       * Registers a callback to run when a {@link DbConnection} whose connection initially succeeded
       * is disconnected, either after a {@link DbConnection.disconnect} call or due to an error.
       *
       * If the connection ended because of an error, the error is passed to the callback.
       *
       * The `callback` will be installed on the `DbConnection` created by `build`
       * before initiating the connection, ensuring there's no opportunity for the disconnect to happen
       * before the callback is installed.
       *
       * Note that this does not trigger if `build` fails
       * or in cases where {@link DbConnectionBuilder.onConnectError} would trigger.
       * This callback only triggers if the connection closes after `build` returns successfully
       * and {@link DbConnectionBuilder.onConnect} is invoked, i.e., after the `IdentityToken` is received.
       *
       * To simplify SDK implementation, at most one such callback can be registered.
       * Calling `onDisconnect` on the same `DbConnectionBuilder` multiple times throws an error.
       *
       * Unlike callbacks registered via {@link DbConnection},
       * no mechanism is provided to unregister the provided callback.
       * This is a concession to ergonomics; there's no clean place to return a `CallbackId` from this method
       * or from `build`.
       *
       * @param {function(error?: Error): void} callback - The callback to invoke upon disconnection.
       * @throws {Error} Throws an error if called multiple times on the same `DbConnectionBuilder`.
       */
      onDisconnect(callback) {
        this.#emitter.on("disconnect", callback);
        return this;
      }
      /**
       * Builds a new `DbConnection` with the parameters set on this `DbConnectionBuilder` and attempts to connect to the SpacetimeDB server.
       *
       * @returns A new `DbConnection` with the parameters set on this `DbConnectionBuilder`.
       *
       * @example
       *
       * ```ts
       * const host = "http://localhost:3000";
       * const name_or_address = "database_name"
       * const auth_token = undefined;
       * DbConnection.builder().withUri(host).withModuleName(name_or_address).withToken(auth_token).build();
       * ```
       */
      build() {
        if (!this.#uri) {
          throw new Error("URI is required to connect to SpacetimeDB");
        }
        if (!this.#nameOrAddress) {
          throw new Error(
            "Database name or address is required to connect to SpacetimeDB"
          );
        }
        ensureMinimumVersionOrThrow(this.remoteModule.versionInfo?.cliVersion);
        return this.dbConnectionConstructor(
          new DbConnectionImpl({
            uri: this.#uri,
            nameOrAddress: this.#nameOrAddress,
            identity: this.#identity,
            token: this.#token,
            emitter: this.#emitter,
            compression: this.#compression,
            lightMode: this.#lightMode,
            confirmedReads: this.#confirmedReads,
            createWSFn: this.#createWSFn,
            remoteModule: this.remoteModule
          })
        );
      }
    };
    SubscriptionBuilderImpl = class {
      static {
        __name(this, "SubscriptionBuilderImpl");
      }
      constructor(db) {
        this.db = db;
      }
      #onApplied = void 0;
      #onError = void 0;
      /**
       * Registers `callback` to run when this query is successfully added to our subscribed set,
       * I.e. when its `SubscriptionApplied` message is received.
       *
       * The database state exposed via the `&EventContext` argument
       * includes all the rows added to the client cache as a result of the new subscription.
       *
       * The event in the `&EventContext` argument is `Event::SubscribeApplied`.
       *
       * Multiple `on_applied` callbacks for the same query may coexist.
       * No mechanism for un-registering `on_applied` callbacks is exposed.
       *
       * @param cb - Callback to run when the subscription is applied.
       * @returns The current `SubscriptionBuilder` instance.
       */
      onApplied(cb) {
        this.#onApplied = cb;
        return this;
      }
      /**
       * Registers `callback` to run when this query either:
       * - Fails to be added to our subscribed set.
       * - Is unexpectedly removed from our subscribed set.
       *
       * If the subscription had previously started and has been unexpectedly removed,
       * the database state exposed via the `&EventContext` argument contains no rows
       * from any subscriptions removed within the same error event.
       * As proposed, it must therefore contain no rows.
       *
       * The event in the `&EventContext` argument is `Event::SubscribeError`,
       * containing a dynamic error object with a human-readable description of the error
       * for diagnostic purposes.
       *
       * Multiple `on_error` callbacks for the same query may coexist.
       * No mechanism for un-registering `on_error` callbacks is exposed.
       *
       * @param cb - Callback to run when there is an error in subscription.
       * @returns The current `SubscriptionBuilder` instance.
       */
      onError(cb) {
        this.#onError = cb;
        return this;
      }
      /**
       * Subscribe to a single query. The results of the query will be merged into the client
       * cache and deduplicated on the client.
       *
       * @param query_sql A `SQL` query to subscribe to.
       *
       * @example
       *
       * ```ts
       * const subscription = connection.subscriptionBuilder().onApplied(() => {
       *   console.log("SDK client cache initialized.");
       * }).subscribe("SELECT * FROM User");
       *
       * subscription.unsubscribe();
       * ```
       */
      subscribe(query_sql) {
        const queries = Array.isArray(query_sql) ? query_sql : [query_sql];
        if (queries.length === 0) {
          throw new Error("Subscriptions must have at least one query");
        }
        return new SubscriptionHandleImpl(
          this.db,
          queries,
          this.#onApplied,
          this.#onError
        );
      }
      /**
       * Subscribes to all rows from all tables.
       *
       * This method is intended as a convenience
       * for applications where client-side memory use and network bandwidth are not concerns.
       * Applications where these resources are a constraint
       * should register more precise queries via `subscribe`
       * in order to replicate only the subset of data which the client needs to function.
       *
       * This method should not be combined with `subscribe` on the same `DbConnection`.
       * A connection may either `subscribe` to particular queries,
       * or `subscribeToAllTables`, but not both.
       * Attempting to call `subscribe`
       * on a `DbConnection` that has previously used `subscribeToAllTables`,
       * or vice versa, may misbehave in any number of ways,
       * including dropping subscriptions, corrupting the client cache, or throwing errors.
       */
      subscribeToAllTables() {
        this.subscribe("SELECT * FROM *");
      }
    };
    SubscriptionManager = class {
      static {
        __name(this, "SubscriptionManager");
      }
      subscriptions = /* @__PURE__ */ new Map();
    };
    SubscriptionHandleImpl = class {
      static {
        __name(this, "SubscriptionHandleImpl");
      }
      constructor(db, querySql, onApplied, onError) {
        this.db = db;
        this.#emitter.on(
          "applied",
          (ctx) => {
            this.#activeState = true;
            if (onApplied) {
              onApplied(ctx);
            }
          }
        );
        this.#emitter.on(
          "error",
          (ctx, error4) => {
            this.#activeState = false;
            this.#endedState = true;
            if (onError) {
              onError(ctx, error4);
            }
          }
        );
        this.#queryId = this.db.registerSubscription(this, this.#emitter, querySql);
      }
      #queryId;
      #unsubscribeCalled = false;
      #endedState = false;
      #activeState = false;
      #emitter = new EventEmitter2();
      /**
       * Consumes self and issues an `Unsubscribe` message,
       * removing this query from the client's set of subscribed queries.
       * It is only valid to call this method if `is_active()` is `true`.
       */
      unsubscribe() {
        if (this.#unsubscribeCalled) {
          throw new Error("Unsubscribe has already been called");
        }
        this.#unsubscribeCalled = true;
        this.db.unregisterSubscription(this.#queryId);
        this.#emitter.on(
          "end",
          (_ctx) => {
            this.#endedState = true;
            this.#activeState = false;
          }
        );
      }
      /**
       * Unsubscribes and also registers a callback to run upon success.
       * I.e. when an `UnsubscribeApplied` message is received.
       *
       * If `Unsubscribe` returns an error,
       * or if the `on_error` callback(s) are invoked before this subscription would end normally,
       * the `on_end` callback is not invoked.
       *
       * @param onEnd - Callback to run upon successful unsubscribe.
       */
      unsubscribeThen(onEnd) {
        if (this.#endedState) {
          throw new Error("Subscription has already ended");
        }
        if (this.#unsubscribeCalled) {
          throw new Error("Unsubscribe has already been called");
        }
        this.#unsubscribeCalled = true;
        this.db.unregisterSubscription(this.#queryId);
        this.#emitter.on(
          "end",
          (ctx) => {
            this.#endedState = true;
            this.#activeState = false;
            onEnd(ctx);
          }
        );
      }
      /**
       * True if this `SubscriptionHandle` has ended,
       * either due to an error or a call to `unsubscribe`.
       *
       * This is initially false, and becomes true when either the `on_end` or `on_error` callback is invoked.
       * A subscription which has not yet been applied is not active, but is also not ended.
       */
      isEnded() {
        return this.#endedState;
      }
      /**
       * True if this `SubscriptionHandle` is active, meaning it has been successfully applied
       * and has not since ended, either due to an error or a complete `unsubscribe` request-response pair.
       *
       * This corresponds exactly to the interval bounded at the start by the `on_applied` callback
       * and at the end by either the `on_end` or `on_error` callback.
       */
      isActive() {
        return this.#activeState;
      }
    };
    __name(callReducerFlagsToNumber, "callReducerFlagsToNumber");
    DbConnectionImpl = class {
      static {
        __name(this, "DbConnectionImpl");
      }
      /**
       * Whether or not the connection is active.
       */
      isActive = false;
      /**
       * This connection's public identity.
       */
      identity = void 0;
      /**
       * This connection's private authentication token.
       */
      token = void 0;
      /**
       * The accessor field to access the tables in the database and associated
       * callback functions.
       */
      db;
      /**
       * The accessor field to access the reducers in the database and associated
       * callback functions.
       */
      reducers;
      /**
       * The accessor field to access functions related to setting flags on
       * reducers regarding how the server should handle the reducer call and
       * the events that it sends back to the client.
       */
      setReducerFlags;
      /**
       * The `ConnectionId` of the connection to to the database.
       */
      connectionId = ConnectionId.random();
      // These fields are meant to be strictly private.
      #queryId = 0;
      #emitter;
      #reducerEmitter = new EventEmitter2();
      #onApplied;
      #remoteModule;
      #messageQueue = Promise.resolve();
      #subscriptionManager = new SubscriptionManager();
      // These fields are not part of the public API, but in a pinch you
      // could use JavaScript to access them by bypassing TypeScript's
      // private fields.
      // We use them in testing.
      clientCache;
      ws;
      wsPromise;
      constructor({
        uri,
        nameOrAddress,
        identity,
        token,
        emitter,
        remoteModule,
        createWSFn,
        compression,
        lightMode,
        confirmedReads
      }) {
        stdbLogger("info", "Connecting to SpacetimeDB WS...");
        const url = new URL(uri.toString());
        if (!/^wss?:/.test(uri.protocol)) {
          url.protocol = url.protocol === "https:" ? "wss:" : "ws:";
        }
        this.identity = identity;
        this.token = token;
        this.#remoteModule = remoteModule;
        this.#emitter = emitter;
        const connectionId = this.connectionId.toHexString();
        url.searchParams.set("connection_id", connectionId);
        this.clientCache = new ClientCache();
        this.db = this.#remoteModule.dbViewConstructor(this);
        this.setReducerFlags = this.#remoteModule.setReducerFlagsConstructor();
        this.reducers = this.#remoteModule.reducersConstructor(
          this,
          this.setReducerFlags
        );
        this.wsPromise = createWSFn({
          url,
          nameOrAddress,
          wsProtocol: "v1.bsatn.spacetimedb",
          authToken: token,
          compression,
          lightMode,
          confirmedReads
        }).then((v) => {
          this.ws = v;
          this.ws.onclose = () => {
            this.#emitter.emit("disconnect", this);
          };
          this.ws.onerror = (e3) => {
            this.#emitter.emit("connectError", this, e3);
          };
          this.ws.onopen = this.#handleOnOpen.bind(this);
          this.ws.onmessage = this.#handleOnMessage.bind(this);
          return v;
        }).catch((e3) => {
          stdbLogger("error", "Error connecting to SpacetimeDB WS");
          this.#emitter.emit("connectError", this, e3);
          return void 0;
        });
      }
      #getNextQueryId = /* @__PURE__ */ __name(() => {
        const queryId = this.#queryId;
        this.#queryId += 1;
        return queryId;
      }, "#getNextQueryId");
      // NOTE: This is very important!!! This is the actual function that
      // gets called when you call `connection.subscriptionBuilder()`.
      // The `subscriptionBuilder` function which is generated, just shadows
      // this function in the type system, but not the actual implementation!
      // Do not remove this function, or shoot yourself in the foot please.
      // It's not clear what would be a better way to do this at this exact
      // moment.
      subscriptionBuilder = /* @__PURE__ */ __name(() => {
        return new SubscriptionBuilderImpl(this);
      }, "subscriptionBuilder");
      registerSubscription(handle, handleEmitter, querySql) {
        const queryId = this.#getNextQueryId();
        this.#subscriptionManager.subscriptions.set(queryId, {
          handle,
          emitter: handleEmitter
        });
        this.#sendMessage(
          ClientMessage.SubscribeMulti({
            queryStrings: querySql,
            queryId: { id: queryId },
            // The TypeScript SDK doesn't currently track `request_id`s,
            // so always use 0.
            requestId: 0
          })
        );
        return queryId;
      }
      unregisterSubscription(queryId) {
        this.#sendMessage(
          ClientMessage.UnsubscribeMulti({
            queryId: { id: queryId },
            // The TypeScript SDK doesn't currently track `request_id`s,
            // so always use 0.
            requestId: 0
          })
        );
      }
      // This function is async because we decompress the message async
      async #processParsedMessage(message) {
        const parseRowList = /* @__PURE__ */ __name((type, tableName, rowList) => {
          const buffer = rowList.rowsData;
          const reader = new BinaryReader(buffer);
          const rows = [];
          const rowType = this.#remoteModule.tables[tableName].rowType;
          let previousOffset = 0;
          const primaryKeyInfo = this.#remoteModule.tables[tableName].primaryKeyInfo;
          while (reader.remaining > 0) {
            const row = AlgebraicType.deserializeValue(reader, rowType);
            let rowId = void 0;
            if (primaryKeyInfo !== void 0) {
              rowId = AlgebraicType.intoMapKey(
                primaryKeyInfo.colType,
                row[primaryKeyInfo.colName]
              );
            } else {
              const rowBytes = buffer.subarray(previousOffset, reader.offset);
              const asBase64 = (0, import_base64_js.fromByteArray)(rowBytes);
              rowId = asBase64;
            }
            previousOffset = reader.offset;
            rows.push({
              type,
              rowId,
              row
            });
          }
          return rows;
        }, "parseRowList");
        const parseTableUpdate = /* @__PURE__ */ __name(async (rawTableUpdate) => {
          const tableName = rawTableUpdate.tableName;
          let operations = [];
          for (const update of rawTableUpdate.updates) {
            let decompressed;
            if (update.tag === "Gzip") {
              const decompressedBuffer = await decompress(update.value, "gzip");
              decompressed = QueryUpdate.deserialize(
                new BinaryReader(decompressedBuffer)
              );
            } else if (update.tag === "Brotli") {
              throw new Error(
                "Brotli compression not supported. Please use gzip or none compression in withCompression method on DbConnection."
              );
            } else {
              decompressed = update.value;
            }
            operations = operations.concat(
              parseRowList("insert", tableName, decompressed.inserts)
            );
            operations = operations.concat(
              parseRowList("delete", tableName, decompressed.deletes)
            );
          }
          return {
            tableName,
            operations
          };
        }, "parseTableUpdate");
        const parseDatabaseUpdate = /* @__PURE__ */ __name(async (dbUpdate) => {
          const tableUpdates = [];
          for (const rawTableUpdate of dbUpdate.tables) {
            tableUpdates.push(await parseTableUpdate(rawTableUpdate));
          }
          return tableUpdates;
        }, "parseDatabaseUpdate");
        switch (message.tag) {
          case "InitialSubscription": {
            const dbUpdate = message.value.databaseUpdate;
            const tableUpdates = await parseDatabaseUpdate(dbUpdate);
            const subscriptionUpdate = {
              tag: "InitialSubscription",
              tableUpdates
            };
            return subscriptionUpdate;
          }
          case "TransactionUpdateLight": {
            const dbUpdate = message.value.update;
            const tableUpdates = await parseDatabaseUpdate(dbUpdate);
            const subscriptionUpdate = {
              tag: "TransactionUpdateLight",
              tableUpdates
            };
            return subscriptionUpdate;
          }
          case "TransactionUpdate": {
            const txUpdate = message.value;
            const identity = txUpdate.callerIdentity;
            const connectionId = ConnectionId.nullIfZero(
              txUpdate.callerConnectionId
            );
            const reducerName = txUpdate.reducerCall.reducerName;
            const args = txUpdate.reducerCall.args;
            const energyQuantaUsed = txUpdate.energyQuantaUsed;
            let tableUpdates = [];
            let errMessage = "";
            switch (txUpdate.status.tag) {
              case "Committed":
                tableUpdates = await parseDatabaseUpdate(txUpdate.status.value);
                break;
              case "Failed":
                tableUpdates = [];
                errMessage = txUpdate.status.value;
                break;
              case "OutOfEnergy":
                tableUpdates = [];
                break;
            }
            if (reducerName === "<none>") {
              const errorMessage = errMessage;
              console.error(`Received an error from the database: ${errorMessage}`);
              return;
            }
            let reducerInfo;
            if (reducerName !== "") {
              reducerInfo = {
                reducerName,
                args
              };
            }
            const transactionUpdate = {
              tag: "TransactionUpdate",
              tableUpdates,
              identity,
              connectionId,
              reducerInfo,
              status: txUpdate.status,
              energyConsumed: energyQuantaUsed.quanta,
              message: errMessage,
              timestamp: txUpdate.timestamp
            };
            return transactionUpdate;
          }
          case "IdentityToken": {
            const identityTokenMessage = {
              tag: "IdentityToken",
              identity: message.value.identity,
              token: message.value.token,
              connectionId: message.value.connectionId
            };
            return identityTokenMessage;
          }
          case "OneOffQueryResponse": {
            throw new Error(
              `TypeScript SDK never sends one-off queries, but got OneOffQueryResponse ${message}`
            );
          }
          case "SubscribeMultiApplied": {
            const parsedTableUpdates = await parseDatabaseUpdate(
              message.value.update
            );
            const subscribeAppliedMessage = {
              tag: "SubscribeApplied",
              queryId: message.value.queryId.id,
              tableUpdates: parsedTableUpdates
            };
            return subscribeAppliedMessage;
          }
          case "UnsubscribeMultiApplied": {
            const parsedTableUpdates = await parseDatabaseUpdate(
              message.value.update
            );
            const unsubscribeAppliedMessage = {
              tag: "UnsubscribeApplied",
              queryId: message.value.queryId.id,
              tableUpdates: parsedTableUpdates
            };
            return unsubscribeAppliedMessage;
          }
          case "SubscriptionError": {
            return {
              tag: "SubscriptionError",
              queryId: message.value.queryId,
              error: message.value.error
            };
          }
        }
      }
      #sendMessage(message) {
        this.wsPromise.then((wsResolved) => {
          if (wsResolved) {
            const writer = new BinaryWriter(1024);
            ClientMessage.serialize(writer, message);
            const encoded = writer.getBuffer();
            wsResolved.send(encoded);
          }
        });
      }
      /**
       * Handles WebSocket onOpen event.
       */
      #handleOnOpen() {
        this.isActive = true;
      }
      #applyTableUpdates(tableUpdates, eventContext) {
        const pendingCallbacks = [];
        for (const tableUpdate of tableUpdates) {
          const tableName = tableUpdate.tableName;
          const tableTypeInfo = this.#remoteModule.tables[tableName];
          const table3 = this.clientCache.getOrCreateTable(tableTypeInfo);
          const newCallbacks = table3.applyOperations(
            tableUpdate.operations,
            eventContext
          );
          for (const callback of newCallbacks) {
            pendingCallbacks.push(callback);
          }
        }
        return pendingCallbacks;
      }
      async #processMessage(data) {
        const serverMessage = parseValue(ServerMessage, data);
        const message = await this.#processParsedMessage(serverMessage);
        if (!message) {
          return;
        }
        switch (message.tag) {
          case "InitialSubscription": {
            const event = { tag: "SubscribeApplied" };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const { event: _, ...subscriptionEventContext } = eventContext;
            const callbacks = this.#applyTableUpdates(
              message.tableUpdates,
              eventContext
            );
            if (this.#emitter) {
              this.#onApplied?.(subscriptionEventContext);
            }
            for (const callback of callbacks) {
              callback.cb();
            }
            break;
          }
          case "TransactionUpdateLight": {
            const event = { tag: "UnknownTransaction" };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const callbacks = this.#applyTableUpdates(
              message.tableUpdates,
              eventContext
            );
            for (const callback of callbacks) {
              callback.cb();
            }
            break;
          }
          case "TransactionUpdate": {
            let reducerInfo = message.reducerInfo;
            let unknownTransaction = false;
            let reducerArgs;
            let reducerTypeInfo;
            if (!reducerInfo) {
              unknownTransaction = true;
            } else {
              reducerTypeInfo = this.#remoteModule.reducers[reducerInfo.reducerName];
              try {
                const reader = new BinaryReader(reducerInfo.args);
                reducerArgs = AlgebraicType.deserializeValue(
                  reader,
                  reducerTypeInfo.argsType
                );
              } catch {
                console.debug("Failed to deserialize reducer arguments");
                unknownTransaction = true;
              }
            }
            if (unknownTransaction) {
              const event2 = { tag: "UnknownTransaction" };
              const eventContext2 = this.#remoteModule.eventContextConstructor(
                this,
                event2
              );
              const callbacks2 = this.#applyTableUpdates(
                message.tableUpdates,
                eventContext2
              );
              for (const callback of callbacks2) {
                callback.cb();
              }
              return;
            }
            reducerInfo = reducerInfo;
            reducerTypeInfo = reducerTypeInfo;
            const reducerEvent = {
              callerIdentity: message.identity,
              status: message.status,
              callerConnectionId: message.connectionId,
              timestamp: message.timestamp,
              energyConsumed: message.energyConsumed,
              reducer: {
                name: reducerInfo.reducerName,
                args: reducerArgs
              }
            };
            const event = {
              tag: "Reducer",
              value: reducerEvent
            };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const reducerEventContext = {
              ...eventContext,
              event: reducerEvent
            };
            const callbacks = this.#applyTableUpdates(
              message.tableUpdates,
              eventContext
            );
            const argsArray = [];
            reducerTypeInfo.argsType.value.elements.forEach((element) => {
              argsArray.push(reducerArgs[element.name]);
            });
            this.#reducerEmitter.emit(
              reducerInfo.reducerName,
              reducerEventContext,
              ...argsArray
            );
            for (const callback of callbacks) {
              callback.cb();
            }
            break;
          }
          case "IdentityToken": {
            this.identity = message.identity;
            if (!this.token && message.token) {
              this.token = message.token;
            }
            this.connectionId = message.connectionId;
            this.#emitter.emit("connect", this, this.identity, this.token);
            break;
          }
          case "SubscribeApplied": {
            const subscription = this.#subscriptionManager.subscriptions.get(
              message.queryId
            );
            if (subscription === void 0) {
              stdbLogger(
                "error",
                `Received SubscribeApplied for unknown queryId ${message.queryId}.`
              );
              break;
            }
            const event = { tag: "SubscribeApplied" };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const { event: _, ...subscriptionEventContext } = eventContext;
            const callbacks = this.#applyTableUpdates(
              message.tableUpdates,
              eventContext
            );
            subscription?.emitter.emit("applied", subscriptionEventContext);
            for (const callback of callbacks) {
              callback.cb();
            }
            break;
          }
          case "UnsubscribeApplied": {
            const subscription = this.#subscriptionManager.subscriptions.get(
              message.queryId
            );
            if (subscription === void 0) {
              stdbLogger(
                "error",
                `Received UnsubscribeApplied for unknown queryId ${message.queryId}.`
              );
              break;
            }
            const event = { tag: "UnsubscribeApplied" };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const { event: _, ...subscriptionEventContext } = eventContext;
            const callbacks = this.#applyTableUpdates(
              message.tableUpdates,
              eventContext
            );
            subscription?.emitter.emit("end", subscriptionEventContext);
            this.#subscriptionManager.subscriptions.delete(message.queryId);
            for (const callback of callbacks) {
              callback.cb();
            }
            break;
          }
          case "SubscriptionError": {
            const error4 = Error(message.error);
            const event = { tag: "Error", value: error4 };
            const eventContext = this.#remoteModule.eventContextConstructor(
              this,
              event
            );
            const errorContext = {
              ...eventContext,
              event: error4
            };
            if (message.queryId !== void 0) {
              this.#subscriptionManager.subscriptions.get(message.queryId)?.emitter.emit("error", errorContext, error4);
              this.#subscriptionManager.subscriptions.delete(message.queryId);
            } else {
              console.error("Received an error message without a queryId: ", error4);
              this.#subscriptionManager.subscriptions.forEach(({ emitter }) => {
                emitter.emit("error", errorContext, error4);
              });
            }
          }
        }
      }
      /**
       * Handles WebSocket onMessage event.
       * @param wsMessage MessageEvent object.
       */
      #handleOnMessage(wsMessage) {
        this.#messageQueue = this.#messageQueue.then(() => {
          return this.#processMessage(wsMessage.data);
        });
      }
      /**
       * Call a reducer on your SpacetimeDB module.
       *
       * @param reducerName The name of the reducer to call
       * @param argsSerializer The arguments to pass to the reducer
       */
      callReducer(reducerName, argsBuffer, flags2) {
        const message = ClientMessage.CallReducer({
          reducer: reducerName,
          args: argsBuffer,
          // The TypeScript SDK doesn't currently track `request_id`s,
          // so always use 0.
          requestId: 0,
          flags: callReducerFlagsToNumber(flags2)
        });
        this.#sendMessage(message);
      }
      /**
       * Close the current connection.
       *
       * @example
       *
       * ```ts
       * const connection = DbConnection.builder().build();
       * connection.disconnect()
       * ```
       */
      disconnect() {
        this.wsPromise.then((wsResolved) => {
          if (wsResolved) {
            wsResolved.close();
          }
        });
      }
      on(eventName, callback) {
        this.#emitter.on(eventName, callback);
      }
      off(eventName, callback) {
        this.#emitter.off(eventName, callback);
      }
      onConnect(callback) {
        this.#emitter.on("connect", callback);
      }
      onDisconnect(callback) {
        this.#emitter.on("disconnect", callback);
      }
      onConnectError(callback) {
        this.#emitter.on("connectError", callback);
      }
      removeOnConnect(callback) {
        this.#emitter.off("connect", callback);
      }
      removeOnDisconnect(callback) {
        this.#emitter.off("disconnect", callback);
      }
      removeOnConnectError(callback) {
        this.#emitter.off("connectError", callback);
      }
      // Note: This is required to be public because it needs to be
      // called from the `RemoteReducers` class.
      onReducer(reducerName, callback) {
        this.#reducerEmitter.on(reducerName, callback);
      }
      // Note: This is required to be public because it needs to be
      // called from the `RemoteReducers` class.
      offReducer(reducerName, callback) {
        this.#reducerEmitter.off(reducerName, callback);
      }
    };
  }
});

// .svelte-kit/output/server/entries/endpoints/api/profile-picture/_server.ts.js
var server_ts_exports = {};
__export(server_ts_exports, {
  POST: () => POST
});
function readUint32LE(data, offset) {
  return (data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 | data[offset + 3] << 24) >>> 0;
}
function getFourCC(data, offset) {
  return String.fromCharCode(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);
}
function parseLossyDimensions(data, offset, size) {
  if (size < 10) return null;
  const frameStart = offset;
  if (data[frameStart + 3] !== 157 || data[frameStart + 4] !== 1 || data[frameStart + 5] !== 42) {
    return null;
  }
  const rawWidth = data[frameStart + 6] | (data[frameStart + 7] & 63) << 8;
  const rawHeight = data[frameStart + 8] | (data[frameStart + 9] & 63) << 8;
  return { width: rawWidth + 1, height: rawHeight + 1 };
}
function parseLosslessDimensions(data, offset, size) {
  if (size < 5) return null;
  if (data[offset] !== 47) {
    return null;
  }
  const widthMinusOne = (data[offset + 2] & 63) << 8 | data[offset + 1];
  const heightMinusOne = (data[offset + 4] & 15) << 10 | data[offset + 3] << 2 | (data[offset + 2] & 192) >> 6;
  return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}
function parseExtendedDimensions(data, offset, size) {
  if (size < 10) return null;
  const widthMinusOne = data[offset + 4] | data[offset + 5] << 8 | data[offset + 6] << 16;
  const heightMinusOne = data[offset + 7] | data[offset + 8] << 8 | data[offset + 9] << 16;
  return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}
function parseWebpDimensions(data) {
  if (data.length < 30) {
    return null;
  }
  if (getFourCC(data, 0) !== "RIFF" || getFourCC(data, 8) !== "WEBP") {
    return null;
  }
  let offset = 12;
  while (offset + 8 <= data.length) {
    const chunkId = getFourCC(data, offset);
    const chunkSize = readUint32LE(data, offset + 4);
    const chunkDataOffset = offset + 8;
    if (chunkDataOffset + chunkSize > data.length) {
      return null;
    }
    if (chunkId === "VP8 ") {
      return parseLossyDimensions(data, chunkDataOffset, chunkSize);
    }
    if (chunkId === "VP8L") {
      return parseLosslessDimensions(data, chunkDataOffset, chunkSize);
    }
    if (chunkId === "VP8X") {
      return parseExtendedDimensions(data, chunkDataOffset, chunkSize);
    }
    offset = chunkDataOffset + chunkSize + (chunkSize & 1);
  }
  return null;
}
async function subscribeAndWait(connection, queries) {
  return new Promise((resolve2, reject) => {
    let settled = false;
    connection.subscriptionBuilder().onApplied(() => {
      if (settled) return;
      settled = true;
      resolve2();
    }).onError((errorContext) => {
      if (settled) return;
      settled = true;
      const errorMessage = errorContext instanceof Error ? errorContext.message : JSON.stringify(errorContext);
      console.error("[Profile] Subscription error:", errorMessage);
      reject(new Error(`Failed to subscribe: ${errorMessage}`));
    }).subscribe(queries);
  });
}
async function connectToSpacetimeDb(env3) {
  const host = env3.SPACETIMEDB_HOST;
  const moduleName = env3.SPACETIMEDB_DB_NAME;
  const token = env3.SPACETIMEDB_ADMIN_TOKEN;
  if (!host || !moduleName) {
    console.error("[Profile] SpacetimeDB connection is not configured:", host, moduleName);
    throw error3(500, "SpacetimeDB connection is not configured");
  }
  if (!token) {
    console.error("[Profile] No admin token available for server-side SpacetimeDB connection");
    throw error3(500, "SpacetimeDB admin token is not configured");
  }
  console.log("[Profile] Connecting to SpacetimeDB:", host, moduleName, "(using admin token)");
  return new Promise((resolve2, reject) => {
    let settled = false;
    const builder = DbConnection.builder().withUri(host).withModuleName(moduleName).withToken(token).onConnect((connection, identity) => {
      if (identity === void 0) {
        if (!settled) {
          settled = true;
          reject(new Error("Unable to resolve account identity"));
        }
        return;
      }
      if (!settled) {
        settled = true;
        resolve2({ connection, identity });
      }
    }).onConnectError((_ctx, err) => {
      if (!settled) {
        settled = true;
        reject(err ?? new Error("Failed to connect to SpacetimeDB"));
      }
    }).onDisconnect((_ctx, err) => {
      if (!settled) {
        settled = true;
        reject(err ?? new Error("Disconnected before connecting to SpacetimeDB"));
      }
    });
    builder.build();
  });
}
async function resolveAccountData(connection, identity) {
  console.log(`[Profile] Starting resolve account data for identity: ${identity.toHexString()}`);
  await subscribeAndWait(connection, [
    `SELECT * FROM Account`
    // WHERE identity = 0x${identity.toHexString()}`
  ]);
  console.log("[Profile] After subscribe and wait");
  const allAccounts = Array.from(connection.db.account.iter());
  console.log(`[Profile] Accounts in cache: ${allAccounts.length}`);
  for (const acc of allAccounts) {
    console.log(`[Profile]   Account ${acc.id}: identity=${acc.identity.toHexString()}`);
  }
  const accountRow = connection.db.account.identity.find(identity);
  if (!accountRow) {
    console.error(
      `[Profile] No account is associated with this identity: ${identity.toHexString()}`
    );
    throw error3(469, "No account is associated with this identity");
  }
  const accountId = accountRow.id;
  const query = `SELECT * FROM AccountCustomization`;
  console.log("Starting subscribe for account_customization with Query: ", query);
  await subscribeAndWait(connection, [query]);
  console.log("After subscribe and wait for account_customization");
  const customization = connection.db.accountCustomization.accountId.find(accountId);
  const currentVersion = customization ? Number(customization.pfpVersion) : 0;
  return { accountId, currentVersion };
}
async function uploadProfilePictureToR2(bucket, objectKey, body2, contentType) {
  console.log(
    `[Profile] Uploading to R2 bucket, key: ${objectKey}, size: ${body2.byteLength} bytes`
  );
  const result = await bucket.put(objectKey, body2, {
    httpMetadata: {
      contentType,
      cacheControl: "public, max-age=31536000, immutable"
    }
  });
  if (!result) {
    throw new Error("R2 put returned null - upload may have failed");
  }
  console.log(`[Profile] R2 upload successful, etag: ${result.etag}`);
}
function buildProfilePictureUrl(baseUrl, accountId, version2, extension) {
  console.log("[PFP] Base URL:", baseUrl);
  if (!baseUrl?.trim() || version2 <= 0) {
    return null;
  }
  const normalizedBase = baseUrl.trim().replace(/\/$/, "");
  return `${normalizedBase}/pfp/${accountId.toString()}.${extension}?v=${version2}`;
}
async function downloadImageFromUrl(imageUrl) {
  const response = await fetch(imageUrl);
  if (!response.ok) {
    throw new Error(`Failed to download image from URL: ${response.status}`);
  }
  const buffer = await response.arrayBuffer();
  return new Uint8Array(buffer);
}
var _cached_ClockSchedule_type_value, ClockSchedule, _cached_ClockUpdate_type_value, ClockUpdate, _cached_CloseAndCycleGameTile_type_value, CloseAndCycleGameTile, _cached_Connect_type_value, Connect, _cached_Disconnect_type_value, Disconnect, _cached_IncrementPfpVersion_type_value, IncrementPfpVersion, _cached_SetUsername_type_value, SetUsername, _cached_Account_type_value, Account, _cached_UpsertAccount_type_value, UpsertAccount, _cached_AccountSeq_type_value, AccountSeq, _cached_UpsertAccountSeq_type_value, UpsertAccountSeq, _cached_InputFrame_type_value, InputFrame, _cached_AuthFrame_type_value, AuthFrame, _cached_UpsertAuthFrame_type_value, UpsertAuthFrame, _cached_BaseCfg_type_value, BaseCfg, _cached_UpsertBaseCfg_type_value, UpsertBaseCfg, _cached_InputCollector_type_value, InputCollector, _cached_UpsertInputCollector_type_value, UpsertInputCollector, _cached_UpsertInputFrame_type_value, UpsertInputFrame, _cached_LevelFileData_type_value, LevelFileData, _cached_UpsertLevelFileData_type_value, UpsertLevelFileData, AccountTableHandle, _cached_AccountCustomization_type_value, AccountCustomization, AccountCustomizationTableHandle, AccountSeqTableHandle, _cached_Admin_type_value, Admin, AdminTableHandle, AuthFrameTableHandle, BaseCfgTableHandle, _cached_Clock_type_value, Clock, ClockTableHandle, ClockScheduleTableHandle, _cached_DeterminismCheck_type_value, DeterminismCheck, DeterminismCheckTableHandle, _cached_GameCoreSnap_type_value, GameCoreSnap, GameCoreSnapTableHandle, InputCollectorTableHandle, InputFrameTableHandle, _cached_LastAuthFrameTimestamp_type_value, LastAuthFrameTimestamp, LastAuthFrameTimestampTableHandle, LevelFileDataTableHandle, _cached_Seq_type_value, Seq, SeqTableHandle, _cached_StepsSinceLastAuthFrame_type_value, StepsSinceLastAuthFrame, StepsSinceLastAuthFrameTableHandle, _cached_StepsSinceLastBatch_type_value, StepsSinceLastBatch, StepsSinceLastBatchTableHandle, REMOTE_MODULE, RemoteReducers, SetReducerFlags, RemoteTables, SubscriptionBuilder, DbConnection, PROFILE_STORAGE_PREFIX, MAX_PROFILE_VERSION, PFP_MIME, PFP_MAX_BYTES, PFP_IMAGE_SIZE, POST;
var init_server_ts = __esm({
  ".svelte-kit/output/server/entries/endpoints/api/profile-picture/_server.ts.js"() {
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
    init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
    init_performance2();
    init_exports();
    init_index_browser();
    _cached_ClockSchedule_type_value = null;
    ClockSchedule = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_ClockSchedule_type_value) return _cached_ClockSchedule_type_value;
        _cached_ClockSchedule_type_value = AlgebraicType.Product({ elements: [] });
        _cached_ClockSchedule_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U64 },
          { name: "scheduledAt", algebraicType: AlgebraicType.createScheduleAtType() }
        );
        return _cached_ClockSchedule_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, ClockSchedule.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, ClockSchedule.getTypeScriptAlgebraicType());
      }
    };
    _cached_ClockUpdate_type_value = null;
    ClockUpdate = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_ClockUpdate_type_value) return _cached_ClockUpdate_type_value;
        _cached_ClockUpdate_type_value = AlgebraicType.Product({ elements: [] });
        _cached_ClockUpdate_type_value.value.elements.push(
          { name: "schedule", algebraicType: ClockSchedule.getTypeScriptAlgebraicType() }
        );
        return _cached_ClockUpdate_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, ClockUpdate.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, ClockUpdate.getTypeScriptAlgebraicType());
      }
    };
    _cached_CloseAndCycleGameTile_type_value = null;
    CloseAndCycleGameTile = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_CloseAndCycleGameTile_type_value) return _cached_CloseAndCycleGameTile_type_value;
        _cached_CloseAndCycleGameTile_type_value = AlgebraicType.Product({ elements: [] });
        _cached_CloseAndCycleGameTile_type_value.value.elements.push(
          { name: "worldId", algebraicType: AlgebraicType.U8 }
        );
        return _cached_CloseAndCycleGameTile_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, CloseAndCycleGameTile.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, CloseAndCycleGameTile.getTypeScriptAlgebraicType());
      }
    };
    _cached_Connect_type_value = null;
    Connect = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Connect_type_value) return _cached_Connect_type_value;
        _cached_Connect_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Connect_type_value.value.elements.push();
        return _cached_Connect_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Connect.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Connect.getTypeScriptAlgebraicType());
      }
    };
    _cached_Disconnect_type_value = null;
    Disconnect = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Disconnect_type_value) return _cached_Disconnect_type_value;
        _cached_Disconnect_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Disconnect_type_value.value.elements.push();
        return _cached_Disconnect_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Disconnect.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Disconnect.getTypeScriptAlgebraicType());
      }
    };
    _cached_IncrementPfpVersion_type_value = null;
    IncrementPfpVersion = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_IncrementPfpVersion_type_value) return _cached_IncrementPfpVersion_type_value;
        _cached_IncrementPfpVersion_type_value = AlgebraicType.Product({ elements: [] });
        _cached_IncrementPfpVersion_type_value.value.elements.push();
        return _cached_IncrementPfpVersion_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, IncrementPfpVersion.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, IncrementPfpVersion.getTypeScriptAlgebraicType());
      }
    };
    _cached_SetUsername_type_value = null;
    SetUsername = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_SetUsername_type_value) return _cached_SetUsername_type_value;
        _cached_SetUsername_type_value = AlgebraicType.Product({ elements: [] });
        _cached_SetUsername_type_value.value.elements.push(
          { name: "username", algebraicType: AlgebraicType.String },
          { name: "overwriteExisting", algebraicType: AlgebraicType.Bool }
        );
        return _cached_SetUsername_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, SetUsername.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, SetUsername.getTypeScriptAlgebraicType());
      }
    };
    _cached_Account_type_value = null;
    Account = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Account_type_value) return _cached_Account_type_value;
        _cached_Account_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Account_type_value.value.elements.push(
          { name: "identity", algebraicType: AlgebraicType.createIdentityType() },
          { name: "id", algebraicType: AlgebraicType.U64 },
          { name: "isConnected", algebraicType: AlgebraicType.Bool },
          { name: "marbles", algebraicType: AlgebraicType.U32 },
          { name: "points", algebraicType: AlgebraicType.U32 },
          { name: "gold", algebraicType: AlgebraicType.U32 },
          { name: "firstLoginBonusClaimed", algebraicType: AlgebraicType.Bool },
          { name: "isMember", algebraicType: AlgebraicType.Bool },
          { name: "dailyRewardClaimStreak", algebraicType: AlgebraicType.I64 },
          { name: "lastDailyRewardClaimDay", algebraicType: AlgebraicType.I64 }
        );
        return _cached_Account_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Account.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Account.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertAccount_type_value = null;
    UpsertAccount = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertAccount_type_value) return _cached_UpsertAccount_type_value;
        _cached_UpsertAccount_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertAccount_type_value.value.elements.push(
          { name: "row", algebraicType: Account.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertAccount_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertAccount.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertAccount.getTypeScriptAlgebraicType());
      }
    };
    _cached_AccountSeq_type_value = null;
    AccountSeq = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_AccountSeq_type_value) return _cached_AccountSeq_type_value;
        _cached_AccountSeq_type_value = AlgebraicType.Product({ elements: [] });
        _cached_AccountSeq_type_value.value.elements.push(
          { name: "idS", algebraicType: AlgebraicType.U64 },
          { name: "seq", algebraicType: AlgebraicType.U64 }
        );
        return _cached_AccountSeq_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, AccountSeq.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, AccountSeq.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertAccountSeq_type_value = null;
    UpsertAccountSeq = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertAccountSeq_type_value) return _cached_UpsertAccountSeq_type_value;
        _cached_UpsertAccountSeq_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertAccountSeq_type_value.value.elements.push(
          { name: "row", algebraicType: AccountSeq.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertAccountSeq_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertAccountSeq.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertAccountSeq.getTypeScriptAlgebraicType());
      }
    };
    _cached_InputFrame_type_value = null;
    InputFrame = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_InputFrame_type_value) return _cached_InputFrame_type_value;
        _cached_InputFrame_type_value = AlgebraicType.Product({ elements: [] });
        _cached_InputFrame_type_value.value.elements.push(
          { name: "seq", algebraicType: AlgebraicType.U16 },
          { name: "inputEventsList", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
        );
        return _cached_InputFrame_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, InputFrame.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, InputFrame.getTypeScriptAlgebraicType());
      }
    };
    _cached_AuthFrame_type_value = null;
    AuthFrame = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_AuthFrame_type_value) return _cached_AuthFrame_type_value;
        _cached_AuthFrame_type_value = AlgebraicType.Product({ elements: [] });
        _cached_AuthFrame_type_value.value.elements.push(
          { name: "seq", algebraicType: AlgebraicType.U16 },
          { name: "frames", algebraicType: AlgebraicType.Array(InputFrame.getTypeScriptAlgebraicType()) }
        );
        return _cached_AuthFrame_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, AuthFrame.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, AuthFrame.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertAuthFrame_type_value = null;
    UpsertAuthFrame = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertAuthFrame_type_value) return _cached_UpsertAuthFrame_type_value;
        _cached_UpsertAuthFrame_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertAuthFrame_type_value.value.elements.push(
          { name: "row", algebraicType: AuthFrame.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertAuthFrame_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertAuthFrame.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertAuthFrame.getTypeScriptAlgebraicType());
      }
    };
    _cached_BaseCfg_type_value = null;
    BaseCfg = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_BaseCfg_type_value) return _cached_BaseCfg_type_value;
        _cached_BaseCfg_type_value = AlgebraicType.Product({ elements: [] });
        _cached_BaseCfg_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "clockIntervalSec", algebraicType: AlgebraicType.F64 },
          { name: "targetStepsPerSecond", algebraicType: AlgebraicType.U16 },
          { name: "physicsStepsPerBatch", algebraicType: AlgebraicType.U16 },
          { name: "stepsPerAuthFrame", algebraicType: AlgebraicType.U16 },
          { name: "authFrameTimeErrorThresholdSec", algebraicType: AlgebraicType.F64 },
          { name: "logInputFrameTimes", algebraicType: AlgebraicType.Bool },
          { name: "logAuthFrameTimeDiffs", algebraicType: AlgebraicType.Bool },
          { name: "gcCacheAccountTimeoutMinutes", algebraicType: AlgebraicType.F64 }
        );
        return _cached_BaseCfg_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, BaseCfg.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, BaseCfg.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertBaseCfg_type_value = null;
    UpsertBaseCfg = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertBaseCfg_type_value) return _cached_UpsertBaseCfg_type_value;
        _cached_UpsertBaseCfg_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertBaseCfg_type_value.value.elements.push(
          { name: "row", algebraicType: BaseCfg.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertBaseCfg_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertBaseCfg.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertBaseCfg.getTypeScriptAlgebraicType());
      }
    };
    _cached_InputCollector_type_value = null;
    InputCollector = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_InputCollector_type_value) return _cached_InputCollector_type_value;
        _cached_InputCollector_type_value = AlgebraicType.Product({ elements: [] });
        _cached_InputCollector_type_value.value.elements.push(
          { name: "delaySeqs", algebraicType: AlgebraicType.U16 },
          { name: "inputEventData", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
        );
        return _cached_InputCollector_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, InputCollector.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, InputCollector.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertInputCollector_type_value = null;
    UpsertInputCollector = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertInputCollector_type_value) return _cached_UpsertInputCollector_type_value;
        _cached_UpsertInputCollector_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertInputCollector_type_value.value.elements.push(
          { name: "row", algebraicType: InputCollector.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertInputCollector_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertInputCollector.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertInputCollector.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertInputFrame_type_value = null;
    UpsertInputFrame = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertInputFrame_type_value) return _cached_UpsertInputFrame_type_value;
        _cached_UpsertInputFrame_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertInputFrame_type_value.value.elements.push(
          { name: "row", algebraicType: InputFrame.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertInputFrame_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertInputFrame.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertInputFrame.getTypeScriptAlgebraicType());
      }
    };
    _cached_LevelFileData_type_value = null;
    LevelFileData = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_LevelFileData_type_value) return _cached_LevelFileData_type_value;
        _cached_LevelFileData_type_value = AlgebraicType.Product({ elements: [] });
        _cached_LevelFileData_type_value.value.elements.push(
          { name: "unityPrefabGuid", algebraicType: AlgebraicType.String },
          { name: "levelFileBinary", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
        );
        return _cached_LevelFileData_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, LevelFileData.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, LevelFileData.getTypeScriptAlgebraicType());
      }
    };
    _cached_UpsertLevelFileData_type_value = null;
    UpsertLevelFileData = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_UpsertLevelFileData_type_value) return _cached_UpsertLevelFileData_type_value;
        _cached_UpsertLevelFileData_type_value = AlgebraicType.Product({ elements: [] });
        _cached_UpsertLevelFileData_type_value.value.elements.push(
          { name: "levelFileData", algebraicType: LevelFileData.getTypeScriptAlgebraicType() }
        );
        return _cached_UpsertLevelFileData_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, UpsertLevelFileData.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, UpsertLevelFileData.getTypeScriptAlgebraicType());
      }
    };
    AccountTableHandle = class {
      static {
        __name(this, "AccountTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `identity` unique index on the table `Account`,
       * which allows point queries on the field of the same name
       * via the [`AccountIdentityUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.account.identity().find(...)`.
       *
       * Get a handle on the `identity` unique index on the table `Account`.
       */
      identity = {
        // Find the subscribed row whose `identity` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.identity, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      /**
       * Access to the `id` unique index on the table `Account`,
       * which allows point queries on the field of the same name
       * via the [`AccountIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.account.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `Account`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_AccountCustomization_type_value = null;
    AccountCustomization = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_AccountCustomization_type_value) return _cached_AccountCustomization_type_value;
        _cached_AccountCustomization_type_value = AlgebraicType.Product({ elements: [] });
        _cached_AccountCustomization_type_value.value.elements.push(
          { name: "accountId", algebraicType: AlgebraicType.U64 },
          { name: "username", algebraicType: AlgebraicType.String },
          { name: "pfpVersion", algebraicType: AlgebraicType.U8 }
        );
        return _cached_AccountCustomization_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, AccountCustomization.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, AccountCustomization.getTypeScriptAlgebraicType());
      }
    };
    AccountCustomizationTableHandle = class {
      static {
        __name(this, "AccountCustomizationTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `accountId` unique index on the table `AccountCustomization`,
       * which allows point queries on the field of the same name
       * via the [`AccountCustomizationAccountIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.accountCustomization.accountId().find(...)`.
       *
       * Get a handle on the `accountId` unique index on the table `AccountCustomization`.
       */
      accountId = {
        // Find the subscribed row whose `accountId` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.accountId, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    AccountSeqTableHandle = class {
      static {
        __name(this, "AccountSeqTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `idS` unique index on the table `AccountSeq`,
       * which allows point queries on the field of the same name
       * via the [`AccountSeqIdSUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.accountSeq.idS().find(...)`.
       *
       * Get a handle on the `idS` unique index on the table `AccountSeq`.
       */
      idS = {
        // Find the subscribed row whose `idS` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.idS, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_Admin_type_value = null;
    Admin = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Admin_type_value) return _cached_Admin_type_value;
        _cached_Admin_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Admin_type_value.value.elements.push(
          { name: "adminIdentity", algebraicType: AlgebraicType.createIdentityType() }
        );
        return _cached_Admin_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Admin.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Admin.getTypeScriptAlgebraicType());
      }
    };
    AdminTableHandle = class {
      static {
        __name(this, "AdminTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `adminIdentity` unique index on the table `Admin`,
       * which allows point queries on the field of the same name
       * via the [`AdminAdminIdentityUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.admin.adminIdentity().find(...)`.
       *
       * Get a handle on the `adminIdentity` unique index on the table `Admin`.
       */
      adminIdentity = {
        // Find the subscribed row whose `adminIdentity` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.adminIdentity, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    AuthFrameTableHandle = class {
      static {
        __name(this, "AuthFrameTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `seq` unique index on the table `AuthFrame`,
       * which allows point queries on the field of the same name
       * via the [`AuthFrameSeqUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.authFrame.seq().find(...)`.
       *
       * Get a handle on the `seq` unique index on the table `AuthFrame`.
       */
      seq = {
        // Find the subscribed row whose `seq` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.seq, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    BaseCfgTableHandle = class {
      static {
        __name(this, "BaseCfgTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `BaseCfg`,
       * which allows point queries on the field of the same name
       * via the [`BaseCfgIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.baseCfg.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `BaseCfg`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_Clock_type_value = null;
    Clock = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Clock_type_value) return _cached_Clock_type_value;
        _cached_Clock_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Clock_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "prevClockUpdate", algebraicType: AlgebraicType.createTimestampType() },
          { name: "tickTimeAccumulatorSec", algebraicType: AlgebraicType.F32 }
        );
        return _cached_Clock_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Clock.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Clock.getTypeScriptAlgebraicType());
      }
    };
    ClockTableHandle = class {
      static {
        __name(this, "ClockTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `Clock`,
       * which allows point queries on the field of the same name
       * via the [`ClockIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.clock.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `Clock`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    ClockScheduleTableHandle = class {
      static {
        __name(this, "ClockScheduleTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `ClockSchedule`,
       * which allows point queries on the field of the same name
       * via the [`ClockScheduleIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.clockSchedule.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `ClockSchedule`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_DeterminismCheck_type_value = null;
    DeterminismCheck = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_DeterminismCheck_type_value) return _cached_DeterminismCheck_type_value;
        _cached_DeterminismCheck_type_value = AlgebraicType.Product({ elements: [] });
        _cached_DeterminismCheck_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "seq", algebraicType: AlgebraicType.U16 },
          { name: "hashString", algebraicType: AlgebraicType.String }
        );
        return _cached_DeterminismCheck_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, DeterminismCheck.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, DeterminismCheck.getTypeScriptAlgebraicType());
      }
    };
    DeterminismCheckTableHandle = class {
      static {
        __name(this, "DeterminismCheckTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `DeterminismCheck`,
       * which allows point queries on the field of the same name
       * via the [`DeterminismCheckIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.determinismCheck.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `DeterminismCheck`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_GameCoreSnap_type_value = null;
    GameCoreSnap = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_GameCoreSnap_type_value) return _cached_GameCoreSnap_type_value;
        _cached_GameCoreSnap_type_value = AlgebraicType.Product({ elements: [] });
        _cached_GameCoreSnap_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "seq", algebraicType: AlgebraicType.U16 },
          { name: "binaryData", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
        );
        return _cached_GameCoreSnap_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, GameCoreSnap.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, GameCoreSnap.getTypeScriptAlgebraicType());
      }
    };
    GameCoreSnapTableHandle = class {
      static {
        __name(this, "GameCoreSnapTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `GameCoreSnap`,
       * which allows point queries on the field of the same name
       * via the [`GameCoreSnapIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.gameCoreSnap.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `GameCoreSnap`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    InputCollectorTableHandle = class {
      static {
        __name(this, "InputCollectorTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
    };
    InputFrameTableHandle = class {
      static {
        __name(this, "InputFrameTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `seq` unique index on the table `InputFrame`,
       * which allows point queries on the field of the same name
       * via the [`InputFrameSeqUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.inputFrame.seq().find(...)`.
       *
       * Get a handle on the `seq` unique index on the table `InputFrame`.
       */
      seq = {
        // Find the subscribed row whose `seq` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.seq, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_LastAuthFrameTimestamp_type_value = null;
    LastAuthFrameTimestamp = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_LastAuthFrameTimestamp_type_value) return _cached_LastAuthFrameTimestamp_type_value;
        _cached_LastAuthFrameTimestamp_type_value = AlgebraicType.Product({ elements: [] });
        _cached_LastAuthFrameTimestamp_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "lastAuthFrameTime", algebraicType: AlgebraicType.createTimestampType() }
        );
        return _cached_LastAuthFrameTimestamp_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, LastAuthFrameTimestamp.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, LastAuthFrameTimestamp.getTypeScriptAlgebraicType());
      }
    };
    LastAuthFrameTimestampTableHandle = class {
      static {
        __name(this, "LastAuthFrameTimestampTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `LastAuthFrameTimestamp`,
       * which allows point queries on the field of the same name
       * via the [`LastAuthFrameTimestampIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.lastAuthFrameTimestamp.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `LastAuthFrameTimestamp`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    LevelFileDataTableHandle = class {
      static {
        __name(this, "LevelFileDataTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `unityPrefabGuid` unique index on the table `LevelFileData`,
       * which allows point queries on the field of the same name
       * via the [`LevelFileDataUnityPrefabGuidUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.levelFileData.unityPrefabGuid().find(...)`.
       *
       * Get a handle on the `unityPrefabGuid` unique index on the table `LevelFileData`.
       */
      unityPrefabGuid = {
        // Find the subscribed row whose `unityPrefabGuid` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.unityPrefabGuid, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_Seq_type_value = null;
    Seq = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_Seq_type_value) return _cached_Seq_type_value;
        _cached_Seq_type_value = AlgebraicType.Product({ elements: [] });
        _cached_Seq_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "value", algebraicType: AlgebraicType.U16 }
        );
        return _cached_Seq_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, Seq.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, Seq.getTypeScriptAlgebraicType());
      }
    };
    SeqTableHandle = class {
      static {
        __name(this, "SeqTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `Seq`,
       * which allows point queries on the field of the same name
       * via the [`SeqIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.seq.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `Seq`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_StepsSinceLastAuthFrame_type_value = null;
    StepsSinceLastAuthFrame = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_StepsSinceLastAuthFrame_type_value) return _cached_StepsSinceLastAuthFrame_type_value;
        _cached_StepsSinceLastAuthFrame_type_value = AlgebraicType.Product({ elements: [] });
        _cached_StepsSinceLastAuthFrame_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "value", algebraicType: AlgebraicType.U16 }
        );
        return _cached_StepsSinceLastAuthFrame_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, StepsSinceLastAuthFrame.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, StepsSinceLastAuthFrame.getTypeScriptAlgebraicType());
      }
    };
    StepsSinceLastAuthFrameTableHandle = class {
      static {
        __name(this, "StepsSinceLastAuthFrameTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `StepsSinceLastAuthFrame`,
       * which allows point queries on the field of the same name
       * via the [`StepsSinceLastAuthFrameIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.stepsSinceLastAuthFrame.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `StepsSinceLastAuthFrame`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    _cached_StepsSinceLastBatch_type_value = null;
    StepsSinceLastBatch = {
      /**
      * A function which returns this type represented as an AlgebraicType.
      * This function is derived from the AlgebraicType used to generate this type.
      */
      getTypeScriptAlgebraicType() {
        if (_cached_StepsSinceLastBatch_type_value) return _cached_StepsSinceLastBatch_type_value;
        _cached_StepsSinceLastBatch_type_value = AlgebraicType.Product({ elements: [] });
        _cached_StepsSinceLastBatch_type_value.value.elements.push(
          { name: "id", algebraicType: AlgebraicType.U8 },
          { name: "value", algebraicType: AlgebraicType.U16 }
        );
        return _cached_StepsSinceLastBatch_type_value;
      },
      serialize(writer, value) {
        AlgebraicType.serializeValue(writer, StepsSinceLastBatch.getTypeScriptAlgebraicType(), value);
      },
      deserialize(reader) {
        return AlgebraicType.deserializeValue(reader, StepsSinceLastBatch.getTypeScriptAlgebraicType());
      }
    };
    StepsSinceLastBatchTableHandle = class {
      static {
        __name(this, "StepsSinceLastBatchTableHandle");
      }
      // phantom type to track the table name
      tableName;
      tableCache;
      constructor(tableCache) {
        this.tableCache = tableCache;
      }
      count() {
        return this.tableCache.count();
      }
      iter() {
        return this.tableCache.iter();
      }
      /**
       * Access to the `id` unique index on the table `StepsSinceLastBatch`,
       * which allows point queries on the field of the same name
       * via the [`StepsSinceLastBatchIdUnique.find`] method.
       *
       * Users are encouraged not to explicitly reference this type,
       * but to directly chain method calls,
       * like `ctx.db.stepsSinceLastBatch.id().find(...)`.
       *
       * Get a handle on the `id` unique index on the table `StepsSinceLastBatch`.
       */
      id = {
        // Find the subscribed row whose `id` column value is equal to `col_val`,
        // if such a row is present in the client cache.
        find: /* @__PURE__ */ __name((col_val) => {
          for (let row of this.tableCache.iter()) {
            if (deepEqual(row.id, col_val)) {
              return row;
            }
          }
        }, "find")
      };
      onInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onInsert(cb);
      }, "onInsert");
      removeOnInsert = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnInsert(cb);
      }, "removeOnInsert");
      onDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onDelete(cb);
      }, "onDelete");
      removeOnDelete = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnDelete(cb);
      }, "removeOnDelete");
      // Updates are only defined for tables with primary keys.
      onUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.onUpdate(cb);
      }, "onUpdate");
      removeOnUpdate = /* @__PURE__ */ __name((cb) => {
        return this.tableCache.removeOnUpdate(cb);
      }, "removeOnUpdate");
    };
    REMOTE_MODULE = {
      tables: {
        Account: {
          tableName: "Account",
          rowType: Account.getTypeScriptAlgebraicType(),
          primaryKey: "identity",
          primaryKeyInfo: {
            colName: "identity",
            colType: Account.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        AccountCustomization: {
          tableName: "AccountCustomization",
          rowType: AccountCustomization.getTypeScriptAlgebraicType(),
          primaryKey: "accountId",
          primaryKeyInfo: {
            colName: "accountId",
            colType: AccountCustomization.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        AccountSeq: {
          tableName: "AccountSeq",
          rowType: AccountSeq.getTypeScriptAlgebraicType(),
          primaryKey: "idS",
          primaryKeyInfo: {
            colName: "idS",
            colType: AccountSeq.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        Admin: {
          tableName: "Admin",
          rowType: Admin.getTypeScriptAlgebraicType(),
          primaryKey: "adminIdentity",
          primaryKeyInfo: {
            colName: "adminIdentity",
            colType: Admin.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        AuthFrame: {
          tableName: "AuthFrame",
          rowType: AuthFrame.getTypeScriptAlgebraicType(),
          primaryKey: "seq",
          primaryKeyInfo: {
            colName: "seq",
            colType: AuthFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        BaseCfg: {
          tableName: "BaseCfg",
          rowType: BaseCfg.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: BaseCfg.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        Clock: {
          tableName: "Clock",
          rowType: Clock.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: Clock.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        ClockSchedule: {
          tableName: "ClockSchedule",
          rowType: ClockSchedule.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: ClockSchedule.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        DeterminismCheck: {
          tableName: "DeterminismCheck",
          rowType: DeterminismCheck.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: DeterminismCheck.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        GameCoreSnap: {
          tableName: "GameCoreSnap",
          rowType: GameCoreSnap.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: GameCoreSnap.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        InputCollector: {
          tableName: "InputCollector",
          rowType: InputCollector.getTypeScriptAlgebraicType()
        },
        InputFrame: {
          tableName: "InputFrame",
          rowType: InputFrame.getTypeScriptAlgebraicType(),
          primaryKey: "seq",
          primaryKeyInfo: {
            colName: "seq",
            colType: InputFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        LastAuthFrameTimestamp: {
          tableName: "LastAuthFrameTimestamp",
          rowType: LastAuthFrameTimestamp.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: LastAuthFrameTimestamp.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        LevelFileData: {
          tableName: "LevelFileData",
          rowType: LevelFileData.getTypeScriptAlgebraicType(),
          primaryKey: "unityPrefabGuid",
          primaryKeyInfo: {
            colName: "unityPrefabGuid",
            colType: LevelFileData.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        Seq: {
          tableName: "Seq",
          rowType: Seq.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: Seq.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        StepsSinceLastAuthFrame: {
          tableName: "StepsSinceLastAuthFrame",
          rowType: StepsSinceLastAuthFrame.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: StepsSinceLastAuthFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        },
        StepsSinceLastBatch: {
          tableName: "StepsSinceLastBatch",
          rowType: StepsSinceLastBatch.getTypeScriptAlgebraicType(),
          primaryKey: "id",
          primaryKeyInfo: {
            colName: "id",
            colType: StepsSinceLastBatch.getTypeScriptAlgebraicType().value.elements[0].algebraicType
          }
        }
      },
      reducers: {
        ClockUpdate: {
          reducerName: "ClockUpdate",
          argsType: ClockUpdate.getTypeScriptAlgebraicType()
        },
        CloseAndCycleGameTile: {
          reducerName: "CloseAndCycleGameTile",
          argsType: CloseAndCycleGameTile.getTypeScriptAlgebraicType()
        },
        Connect: {
          reducerName: "Connect",
          argsType: Connect.getTypeScriptAlgebraicType()
        },
        Disconnect: {
          reducerName: "Disconnect",
          argsType: Disconnect.getTypeScriptAlgebraicType()
        },
        IncrementPfpVersion: {
          reducerName: "IncrementPfpVersion",
          argsType: IncrementPfpVersion.getTypeScriptAlgebraicType()
        },
        SetUsername: {
          reducerName: "SetUsername",
          argsType: SetUsername.getTypeScriptAlgebraicType()
        },
        UpsertAccount: {
          reducerName: "UpsertAccount",
          argsType: UpsertAccount.getTypeScriptAlgebraicType()
        },
        UpsertAccountSeq: {
          reducerName: "UpsertAccountSeq",
          argsType: UpsertAccountSeq.getTypeScriptAlgebraicType()
        },
        UpsertAuthFrame: {
          reducerName: "UpsertAuthFrame",
          argsType: UpsertAuthFrame.getTypeScriptAlgebraicType()
        },
        UpsertBaseCfg: {
          reducerName: "UpsertBaseCfg",
          argsType: UpsertBaseCfg.getTypeScriptAlgebraicType()
        },
        UpsertInputCollector: {
          reducerName: "UpsertInputCollector",
          argsType: UpsertInputCollector.getTypeScriptAlgebraicType()
        },
        UpsertInputFrame: {
          reducerName: "UpsertInputFrame",
          argsType: UpsertInputFrame.getTypeScriptAlgebraicType()
        },
        UpsertLevelFileData: {
          reducerName: "UpsertLevelFileData",
          argsType: UpsertLevelFileData.getTypeScriptAlgebraicType()
        }
      },
      versionInfo: {
        cliVersion: "1.8.0"
      },
      // Constructors which are used by the DbConnectionImpl to
      // extract type information from the generated RemoteModule.
      //
      // NOTE: This is not strictly necessary for `eventContextConstructor` because
      // all we do is build a TypeScript object which we could have done inside the
      // SDK, but if in the future we wanted to create a class this would be
      // necessary because classes have methods, so we'll keep it.
      eventContextConstructor: /* @__PURE__ */ __name((imp, event) => {
        return {
          ...imp,
          event
        };
      }, "eventContextConstructor"),
      dbViewConstructor: /* @__PURE__ */ __name((imp) => {
        return new RemoteTables(imp);
      }, "dbViewConstructor"),
      reducersConstructor: /* @__PURE__ */ __name((imp, setReducerFlags) => {
        return new RemoteReducers(imp, setReducerFlags);
      }, "reducersConstructor"),
      setReducerFlagsConstructor: /* @__PURE__ */ __name(() => {
        return new SetReducerFlags();
      }, "setReducerFlagsConstructor")
    };
    RemoteReducers = class {
      static {
        __name(this, "RemoteReducers");
      }
      constructor(connection, setCallReducerFlags) {
        this.connection = connection;
        this.setCallReducerFlags = setCallReducerFlags;
      }
      clockUpdate(schedule) {
        const __args = { schedule };
        let __writer = new BinaryWriter(1024);
        ClockUpdate.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("ClockUpdate", __argsBuffer, this.setCallReducerFlags.clockUpdateFlags);
      }
      onClockUpdate(callback) {
        this.connection.onReducer("ClockUpdate", callback);
      }
      removeOnClockUpdate(callback) {
        this.connection.offReducer("ClockUpdate", callback);
      }
      closeAndCycleGameTile(worldId) {
        const __args = { worldId };
        let __writer = new BinaryWriter(1024);
        CloseAndCycleGameTile.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("CloseAndCycleGameTile", __argsBuffer, this.setCallReducerFlags.closeAndCycleGameTileFlags);
      }
      onCloseAndCycleGameTile(callback) {
        this.connection.onReducer("CloseAndCycleGameTile", callback);
      }
      removeOnCloseAndCycleGameTile(callback) {
        this.connection.offReducer("CloseAndCycleGameTile", callback);
      }
      onConnect(callback) {
        this.connection.onReducer("Connect", callback);
      }
      removeOnConnect(callback) {
        this.connection.offReducer("Connect", callback);
      }
      onDisconnect(callback) {
        this.connection.onReducer("Disconnect", callback);
      }
      removeOnDisconnect(callback) {
        this.connection.offReducer("Disconnect", callback);
      }
      incrementPfpVersion() {
        this.connection.callReducer("IncrementPfpVersion", new Uint8Array(0), this.setCallReducerFlags.incrementPfpVersionFlags);
      }
      onIncrementPfpVersion(callback) {
        this.connection.onReducer("IncrementPfpVersion", callback);
      }
      removeOnIncrementPfpVersion(callback) {
        this.connection.offReducer("IncrementPfpVersion", callback);
      }
      setUsername(username, overwriteExisting) {
        const __args = { username, overwriteExisting };
        let __writer = new BinaryWriter(1024);
        SetUsername.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("SetUsername", __argsBuffer, this.setCallReducerFlags.setUsernameFlags);
      }
      onSetUsername(callback) {
        this.connection.onReducer("SetUsername", callback);
      }
      removeOnSetUsername(callback) {
        this.connection.offReducer("SetUsername", callback);
      }
      upsertAccount(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertAccount.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertAccount", __argsBuffer, this.setCallReducerFlags.upsertAccountFlags);
      }
      onUpsertAccount(callback) {
        this.connection.onReducer("UpsertAccount", callback);
      }
      removeOnUpsertAccount(callback) {
        this.connection.offReducer("UpsertAccount", callback);
      }
      upsertAccountSeq(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertAccountSeq.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertAccountSeq", __argsBuffer, this.setCallReducerFlags.upsertAccountSeqFlags);
      }
      onUpsertAccountSeq(callback) {
        this.connection.onReducer("UpsertAccountSeq", callback);
      }
      removeOnUpsertAccountSeq(callback) {
        this.connection.offReducer("UpsertAccountSeq", callback);
      }
      upsertAuthFrame(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertAuthFrame.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertAuthFrame", __argsBuffer, this.setCallReducerFlags.upsertAuthFrameFlags);
      }
      onUpsertAuthFrame(callback) {
        this.connection.onReducer("UpsertAuthFrame", callback);
      }
      removeOnUpsertAuthFrame(callback) {
        this.connection.offReducer("UpsertAuthFrame", callback);
      }
      upsertBaseCfg(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertBaseCfg.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertBaseCfg", __argsBuffer, this.setCallReducerFlags.upsertBaseCfgFlags);
      }
      onUpsertBaseCfg(callback) {
        this.connection.onReducer("UpsertBaseCfg", callback);
      }
      removeOnUpsertBaseCfg(callback) {
        this.connection.offReducer("UpsertBaseCfg", callback);
      }
      upsertInputCollector(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertInputCollector.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertInputCollector", __argsBuffer, this.setCallReducerFlags.upsertInputCollectorFlags);
      }
      onUpsertInputCollector(callback) {
        this.connection.onReducer("UpsertInputCollector", callback);
      }
      removeOnUpsertInputCollector(callback) {
        this.connection.offReducer("UpsertInputCollector", callback);
      }
      upsertInputFrame(row) {
        const __args = { row };
        let __writer = new BinaryWriter(1024);
        UpsertInputFrame.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertInputFrame", __argsBuffer, this.setCallReducerFlags.upsertInputFrameFlags);
      }
      onUpsertInputFrame(callback) {
        this.connection.onReducer("UpsertInputFrame", callback);
      }
      removeOnUpsertInputFrame(callback) {
        this.connection.offReducer("UpsertInputFrame", callback);
      }
      upsertLevelFileData(levelFileData) {
        const __args = { levelFileData };
        let __writer = new BinaryWriter(1024);
        UpsertLevelFileData.serialize(__writer, __args);
        let __argsBuffer = __writer.getBuffer();
        this.connection.callReducer("UpsertLevelFileData", __argsBuffer, this.setCallReducerFlags.upsertLevelFileDataFlags);
      }
      onUpsertLevelFileData(callback) {
        this.connection.onReducer("UpsertLevelFileData", callback);
      }
      removeOnUpsertLevelFileData(callback) {
        this.connection.offReducer("UpsertLevelFileData", callback);
      }
    };
    SetReducerFlags = class {
      static {
        __name(this, "SetReducerFlags");
      }
      clockUpdateFlags = "FullUpdate";
      clockUpdate(flags2) {
        this.clockUpdateFlags = flags2;
      }
      closeAndCycleGameTileFlags = "FullUpdate";
      closeAndCycleGameTile(flags2) {
        this.closeAndCycleGameTileFlags = flags2;
      }
      incrementPfpVersionFlags = "FullUpdate";
      incrementPfpVersion(flags2) {
        this.incrementPfpVersionFlags = flags2;
      }
      setUsernameFlags = "FullUpdate";
      setUsername(flags2) {
        this.setUsernameFlags = flags2;
      }
      upsertAccountFlags = "FullUpdate";
      upsertAccount(flags2) {
        this.upsertAccountFlags = flags2;
      }
      upsertAccountSeqFlags = "FullUpdate";
      upsertAccountSeq(flags2) {
        this.upsertAccountSeqFlags = flags2;
      }
      upsertAuthFrameFlags = "FullUpdate";
      upsertAuthFrame(flags2) {
        this.upsertAuthFrameFlags = flags2;
      }
      upsertBaseCfgFlags = "FullUpdate";
      upsertBaseCfg(flags2) {
        this.upsertBaseCfgFlags = flags2;
      }
      upsertInputCollectorFlags = "FullUpdate";
      upsertInputCollector(flags2) {
        this.upsertInputCollectorFlags = flags2;
      }
      upsertInputFrameFlags = "FullUpdate";
      upsertInputFrame(flags2) {
        this.upsertInputFrameFlags = flags2;
      }
      upsertLevelFileDataFlags = "FullUpdate";
      upsertLevelFileData(flags2) {
        this.upsertLevelFileDataFlags = flags2;
      }
    };
    RemoteTables = class {
      static {
        __name(this, "RemoteTables");
      }
      constructor(connection) {
        this.connection = connection;
      }
      get account() {
        return new AccountTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Account));
      }
      get accountCustomization() {
        return new AccountCustomizationTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AccountCustomization));
      }
      get accountSeq() {
        return new AccountSeqTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AccountSeq));
      }
      get admin() {
        return new AdminTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Admin));
      }
      get authFrame() {
        return new AuthFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AuthFrame));
      }
      get baseCfg() {
        return new BaseCfgTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.BaseCfg));
      }
      get clock() {
        return new ClockTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Clock));
      }
      get clockSchedule() {
        return new ClockScheduleTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.ClockSchedule));
      }
      get determinismCheck() {
        return new DeterminismCheckTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.DeterminismCheck));
      }
      get gameCoreSnap() {
        return new GameCoreSnapTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.GameCoreSnap));
      }
      get inputCollector() {
        return new InputCollectorTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.InputCollector));
      }
      get inputFrame() {
        return new InputFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.InputFrame));
      }
      get lastAuthFrameTimestamp() {
        return new LastAuthFrameTimestampTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.LastAuthFrameTimestamp));
      }
      get levelFileData() {
        return new LevelFileDataTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.LevelFileData));
      }
      get seq() {
        return new SeqTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Seq));
      }
      get stepsSinceLastAuthFrame() {
        return new StepsSinceLastAuthFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.StepsSinceLastAuthFrame));
      }
      get stepsSinceLastBatch() {
        return new StepsSinceLastBatchTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.StepsSinceLastBatch));
      }
    };
    SubscriptionBuilder = class extends SubscriptionBuilderImpl {
      static {
        __name(this, "SubscriptionBuilder");
      }
    };
    DbConnection = class extends DbConnectionImpl {
      static {
        __name(this, "DbConnection");
      }
      static builder = /* @__PURE__ */ __name(() => {
        return new DbConnectionBuilder(REMOTE_MODULE, (imp) => imp);
      }, "builder");
      subscriptionBuilder = /* @__PURE__ */ __name(() => {
        return new SubscriptionBuilder(this);
      }, "subscriptionBuilder");
    };
    PROFILE_STORAGE_PREFIX = "pfp/";
    MAX_PROFILE_VERSION = 255;
    PFP_MIME = "image/webp";
    PFP_MAX_BYTES = 512 * 1024;
    PFP_IMAGE_SIZE = 256;
    __name(readUint32LE, "readUint32LE");
    __name(getFourCC, "getFourCC");
    __name(parseLossyDimensions, "parseLossyDimensions");
    __name(parseLosslessDimensions, "parseLosslessDimensions");
    __name(parseExtendedDimensions, "parseExtendedDimensions");
    __name(parseWebpDimensions, "parseWebpDimensions");
    __name(subscribeAndWait, "subscribeAndWait");
    __name(connectToSpacetimeDb, "connectToSpacetimeDb");
    __name(resolveAccountData, "resolveAccountData");
    __name(uploadProfilePictureToR2, "uploadProfilePictureToR2");
    __name(buildProfilePictureUrl, "buildProfilePictureUrl");
    __name(downloadImageFromUrl, "downloadImageFromUrl");
    POST = /* @__PURE__ */ __name(async ({ request, platform: platform2 }) => {
      console.log("[Profile] Starting POST request");
      const env3 = platform2?.env;
      if (!env3) {
        console.error("[Profile] Platform environment is not available");
        throw error3(500, "Platform environment is not available");
      }
      const authHeader = request.headers.get("authorization") ?? request.headers.get("Authorization");
      console.log(
        `[Profile] Auth header present: ${!!authHeader}, starts with Bearer: ${authHeader?.startsWith("Bearer ")}`
      );
      if (!authHeader?.startsWith("Bearer ")) {
        console.error("[Profile] Missing or invalid auth header");
        throw error3(401, "An ID token is required to upload profile pictures");
      }
      const token = authHeader.slice(7).trim();
      if (!token) {
        console.error("[Profile] Token is empty after extracting from header");
        throw error3(401, "An ID token is required to upload profile pictures");
      }
      console.log(`[Profile] Received token (first 20 chars): ${token.substring(0, 20)}...`);
      const contentType = request.headers.get("content-type") ?? "";
      let bytes;
      let userIdentity = null;
      if (contentType.includes("multipart/form-data")) {
        const formData = await request.formData();
        const image = formData.get("image");
        const identityField = formData.get("identity");
        if (!(image instanceof File)) {
          throw error3(400, "The request must include an image file");
        }
        if (!identityField || typeof identityField !== "string") {
          throw error3(400, "The request must include an identity");
        }
        try {
          userIdentity = Identity.fromString(identityField);
        } catch {
          throw error3(400, "Invalid identity format");
        }
        if (image.type !== PFP_MIME) {
          throw error3(400, `Profile pictures must be uploaded as ${PFP_MIME}`);
        }
        if (image.size > PFP_MAX_BYTES) {
          throw error3(400, `Profile pictures must be smaller than ${PFP_MAX_BYTES} bytes`);
        }
        bytes = new Uint8Array(await image.arrayBuffer());
        const dimensions = parseWebpDimensions(bytes);
        if (!dimensions) {
          throw error3(400, "The uploaded image is not a valid WebP file");
        }
        if (dimensions.width !== PFP_IMAGE_SIZE || dimensions.height !== PFP_IMAGE_SIZE) {
          throw error3(
            400,
            `Profile pictures must be exactly ${PFP_IMAGE_SIZE}\xD7${PFP_IMAGE_SIZE} pixels`
          );
        }
      } else if (contentType.includes("application/json")) {
        const body2 = await request.json();
        if (!body2.imageUrl || typeof body2.imageUrl !== "string") {
          throw error3(400, "Request must include an imageUrl");
        }
        if (!body2.identity || typeof body2.identity !== "string") {
          throw error3(400, "Request must include an identity");
        }
        try {
          new URL(body2.imageUrl);
        } catch {
          throw error3(400, "Invalid imageUrl provided");
        }
        try {
          userIdentity = Identity.fromString(body2.identity);
        } catch {
          throw error3(400, "Invalid identity format");
        }
        console.log("[Profile] Downloading image from URL:", body2.imageUrl);
        try {
          bytes = await downloadImageFromUrl(body2.imageUrl);
        } catch (err) {
          console.error("[Profile] Failed to download image:", err);
          throw error3(400, "Failed to download image from URL");
        }
        console.log(`[Profile] Downloaded ${bytes.length} bytes from URL`);
      } else {
        throw error3(400, "Request must be multipart/form-data or application/json");
      }
      if (!userIdentity) {
        throw error3(400, "Identity is required");
      }
      console.log(`[Profile] User identity: ${userIdentity.toHexString()}`);
      let connection = null;
      let accountId = 0n;
      let currentVersion = 0;
      try {
        console.log("[Profile] Attempting to connect to SpacetimeDB with admin token...");
        const connectionResult = await connectToSpacetimeDb(env3);
        console.log(
          "[Profile] Connected to SpacetimeDB as admin, identity:",
          connectionResult.identity.toHexString()
        );
        connection = connectionResult.connection;
        const accountData = await resolveAccountData(connection, userIdentity);
        accountId = accountData.accountId;
        currentVersion = accountData.currentVersion;
        console.log(`[Profile] Resolved account ${accountId} with pfp version ${currentVersion}`);
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : String(err);
        const errorStack = err instanceof Error ? err.stack : void 0;
        console.error("[Profile] Failed to resolve account data:", errorMessage);
        console.error("[Profile] Error stack:", errorStack);
        connection?.disconnect();
        if (err && typeof err === "object" && "status" in err) {
          throw err;
        }
        throw error3(401, `Unable to verify your SpacetimeDB account: ${errorMessage}`);
      }
      if (!connection) {
        throw error3(500, "SpacetimeDB connection is not available");
      }
      const activeConnection = connection;
      const bucket = env3.MARBLES_BUCKET_BINDING;
      if (!bucket) {
        console.error("[Profile] R2 bucket binding not available");
        throw error3(500, "Profile picture storage is not configured");
      }
      const isWebp = bytes.length >= 12 && getFourCC(bytes, 8) === "WEBP";
      const extension = isWebp ? "webp" : "jpg";
      const mimeType = isWebp ? "image/webp" : "image/jpeg";
      const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.${extension}`;
      const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);
      try {
        console.log(`[Profile] Uploading ${bytes.length} bytes to R2 as ${objectKey}`);
        await uploadProfilePictureToR2(bucket, objectKey, bytes, mimeType);
        console.log("[Profile] Upload successful");
        console.log("[Profile] Calling IncrementPfpVersion reducer");
        await activeConnection.reducers.incrementPfpVersion();
        console.log("[Profile] Reducer called successfully");
      } catch (err) {
        console.error("[Profile] Failed to upload profile picture:", err);
        throw error3(500, "Failed to upload profile picture");
      } finally {
        activeConnection.disconnect();
      }
      const url = buildProfilePictureUrl(
        env3.VITE_PFP_CDN_BASE_URL,
        accountId,
        nextVersion,
        extension
      );
      console.log("[Profile] Returning profile picture URL:", url);
      return json({
        accountId: accountId.toString(),
        version: nextVersion,
        url
      });
    }, "POST");
  }
});

// .wrangler/tmp/bundle-Tn6LFy/middleware-loader.entry.ts
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// .wrangler/tmp/bundle-Tn6LFy/middleware-insertion-facade.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// .svelte-kit/cloudflare/_worker.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// .svelte-kit/output/server/index.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// .svelte-kit/output/server/chunks/environment.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var BROWSER = false;
var base = "";
var assets = base;
var app_dir = "_app";
var relative = true;
var initial = { base, assets };
function override(paths) {
  base = paths.base;
  assets = paths.assets;
}
__name(override, "override");
function reset() {
  base = initial.base;
  assets = initial.assets;
}
__name(reset, "reset");

// .svelte-kit/output/server/index.js
init_exports();
init_internal();
init_server();

// node_modules/devalue/index.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// node_modules/devalue/src/uneval.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// node_modules/devalue/src/utils.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var escaped = {
  "<": "\\u003C",
  "\\": "\\\\",
  "\b": "\\b",
  "\f": "\\f",
  "\n": "\\n",
  "\r": "\\r",
  "	": "\\t",
  "\u2028": "\\u2028",
  "\u2029": "\\u2029"
};
var DevalueError = class extends Error {
  static {
    __name(this, "DevalueError");
  }
  /**
   * @param {string} message
   * @param {string[]} keys
   */
  constructor(message, keys) {
    super(message);
    this.name = "DevalueError";
    this.path = keys.join("");
  }
};
function is_primitive(thing) {
  return Object(thing) !== thing;
}
__name(is_primitive, "is_primitive");
var object_proto_names = /* @__PURE__ */ Object.getOwnPropertyNames(
  Object.prototype
).sort().join("\0");
function is_plain_object(thing) {
  const proto = Object.getPrototypeOf(thing);
  return proto === Object.prototype || proto === null || Object.getPrototypeOf(proto) === null || Object.getOwnPropertyNames(proto).sort().join("\0") === object_proto_names;
}
__name(is_plain_object, "is_plain_object");
function get_type(thing) {
  return Object.prototype.toString.call(thing).slice(8, -1);
}
__name(get_type, "get_type");
function get_escaped_char(char) {
  switch (char) {
    case '"':
      return '\\"';
    case "<":
      return "\\u003C";
    case "\\":
      return "\\\\";
    case "\n":
      return "\\n";
    case "\r":
      return "\\r";
    case "	":
      return "\\t";
    case "\b":
      return "\\b";
    case "\f":
      return "\\f";
    case "\u2028":
      return "\\u2028";
    case "\u2029":
      return "\\u2029";
    default:
      return char < " " ? `\\u${char.charCodeAt(0).toString(16).padStart(4, "0")}` : "";
  }
}
__name(get_escaped_char, "get_escaped_char");
function stringify_string(str) {
  let result = "";
  let last_pos = 0;
  const len = str.length;
  for (let i = 0; i < len; i += 1) {
    const char = str[i];
    const replacement = get_escaped_char(char);
    if (replacement) {
      result += str.slice(last_pos, i) + replacement;
      last_pos = i + 1;
    }
  }
  return `"${last_pos === 0 ? str : result + str.slice(last_pos)}"`;
}
__name(stringify_string, "stringify_string");
function enumerable_symbols(object) {
  return Object.getOwnPropertySymbols(object).filter(
    (symbol) => Object.getOwnPropertyDescriptor(object, symbol).enumerable
  );
}
__name(enumerable_symbols, "enumerable_symbols");
var is_identifier = /^[a-zA-Z_$][a-zA-Z_$0-9]*$/;
function stringify_key(key2) {
  return is_identifier.test(key2) ? "." + key2 : "[" + JSON.stringify(key2) + "]";
}
__name(stringify_key, "stringify_key");

// node_modules/devalue/src/uneval.js
var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$";
var unsafe_chars = /[<\b\f\n\r\t\0\u2028\u2029]/g;
var reserved = /^(?:do|if|in|for|int|let|new|try|var|byte|case|char|else|enum|goto|long|this|void|with|await|break|catch|class|const|final|float|short|super|throw|while|yield|delete|double|export|import|native|return|switch|throws|typeof|boolean|default|extends|finally|package|private|abstract|continue|debugger|function|volatile|interface|protected|transient|implements|instanceof|synchronized)$/;
function uneval(value, replacer) {
  const counts = /* @__PURE__ */ new Map();
  const keys = [];
  const custom = /* @__PURE__ */ new Map();
  function walk(thing) {
    if (typeof thing === "function") {
      throw new DevalueError(`Cannot stringify a function`, keys);
    }
    if (!is_primitive(thing)) {
      if (counts.has(thing)) {
        counts.set(thing, counts.get(thing) + 1);
        return;
      }
      counts.set(thing, 1);
      if (replacer) {
        const str2 = replacer(thing, (value2) => uneval(value2, replacer));
        if (typeof str2 === "string") {
          custom.set(thing, str2);
          return;
        }
      }
      const type = get_type(thing);
      switch (type) {
        case "Number":
        case "BigInt":
        case "String":
        case "Boolean":
        case "Date":
        case "RegExp":
        case "URL":
        case "URLSearchParams":
          return;
        case "Array":
          thing.forEach((value2, i) => {
            keys.push(`[${i}]`);
            walk(value2);
            keys.pop();
          });
          break;
        case "Set":
          Array.from(thing).forEach(walk);
          break;
        case "Map":
          for (const [key2, value2] of thing) {
            keys.push(
              `.get(${is_primitive(key2) ? stringify_primitive(key2) : "..."})`
            );
            walk(value2);
            keys.pop();
          }
          break;
        case "Int8Array":
        case "Uint8Array":
        case "Uint8ClampedArray":
        case "Int16Array":
        case "Uint16Array":
        case "Int32Array":
        case "Uint32Array":
        case "Float32Array":
        case "Float64Array":
        case "BigInt64Array":
        case "BigUint64Array":
          walk(thing.buffer);
          return;
        case "ArrayBuffer":
          return;
        case "Temporal.Duration":
        case "Temporal.Instant":
        case "Temporal.PlainDate":
        case "Temporal.PlainTime":
        case "Temporal.PlainDateTime":
        case "Temporal.PlainMonthDay":
        case "Temporal.PlainYearMonth":
        case "Temporal.ZonedDateTime":
          return;
        default:
          if (!is_plain_object(thing)) {
            throw new DevalueError(
              `Cannot stringify arbitrary non-POJOs`,
              keys
            );
          }
          if (enumerable_symbols(thing).length > 0) {
            throw new DevalueError(
              `Cannot stringify POJOs with symbolic keys`,
              keys
            );
          }
          for (const key2 in thing) {
            keys.push(stringify_key(key2));
            walk(thing[key2]);
            keys.pop();
          }
      }
    }
  }
  __name(walk, "walk");
  walk(value);
  const names = /* @__PURE__ */ new Map();
  Array.from(counts).filter((entry) => entry[1] > 1).sort((a, b) => b[1] - a[1]).forEach((entry, i) => {
    names.set(entry[0], get_name(i));
  });
  function stringify3(thing) {
    if (names.has(thing)) {
      return names.get(thing);
    }
    if (is_primitive(thing)) {
      return stringify_primitive(thing);
    }
    if (custom.has(thing)) {
      return custom.get(thing);
    }
    const type = get_type(thing);
    switch (type) {
      case "Number":
      case "String":
      case "Boolean":
        return `Object(${stringify3(thing.valueOf())})`;
      case "RegExp":
        return `new RegExp(${stringify_string(thing.source)}, "${thing.flags}")`;
      case "Date":
        return `new Date(${thing.getTime()})`;
      case "URL":
        return `new URL(${stringify_string(thing.toString())})`;
      case "URLSearchParams":
        return `new URLSearchParams(${stringify_string(thing.toString())})`;
      case "Array":
        const members = (
          /** @type {any[]} */
          thing.map(
            (v, i) => i in thing ? stringify3(v) : ""
          )
        );
        const tail = thing.length === 0 || thing.length - 1 in thing ? "" : ",";
        return `[${members.join(",")}${tail}]`;
      case "Set":
      case "Map":
        return `new ${type}([${Array.from(thing).map(stringify3).join(",")}])`;
      case "Int8Array":
      case "Uint8Array":
      case "Uint8ClampedArray":
      case "Int16Array":
      case "Uint16Array":
      case "Int32Array":
      case "Uint32Array":
      case "Float32Array":
      case "Float64Array":
      case "BigInt64Array":
      case "BigUint64Array": {
        let str2 = `new ${type}`;
        if (counts.get(thing.buffer) === 1) {
          const array2 = new thing.constructor(thing.buffer);
          str2 += `([${array2}])`;
        } else {
          str2 += `([${stringify3(thing.buffer)}])`;
        }
        const a = thing.byteOffset;
        const b = a + thing.byteLength;
        if (a > 0 || b !== thing.buffer.byteLength) {
          const m = +/(\d+)/.exec(type)[1] / 8;
          str2 += `.subarray(${a / m},${b / m})`;
        }
        return str2;
      }
      case "ArrayBuffer": {
        const ui8 = new Uint8Array(thing);
        return `new Uint8Array([${ui8.toString()}]).buffer`;
      }
      case "Temporal.Duration":
      case "Temporal.Instant":
      case "Temporal.PlainDate":
      case "Temporal.PlainTime":
      case "Temporal.PlainDateTime":
      case "Temporal.PlainMonthDay":
      case "Temporal.PlainYearMonth":
      case "Temporal.ZonedDateTime":
        return `${type}.from(${stringify_string(thing.toString())})`;
      default:
        const keys2 = Object.keys(thing);
        const obj = keys2.map((key2) => `${safe_key(key2)}:${stringify3(thing[key2])}`).join(",");
        const proto = Object.getPrototypeOf(thing);
        if (proto === null) {
          return keys2.length > 0 ? `{${obj},__proto__:null}` : `{__proto__:null}`;
        }
        return `{${obj}}`;
    }
  }
  __name(stringify3, "stringify");
  const str = stringify3(value);
  if (names.size) {
    const params = [];
    const statements = [];
    const values = [];
    names.forEach((name, thing) => {
      params.push(name);
      if (custom.has(thing)) {
        values.push(
          /** @type {string} */
          custom.get(thing)
        );
        return;
      }
      if (is_primitive(thing)) {
        values.push(stringify_primitive(thing));
        return;
      }
      const type = get_type(thing);
      switch (type) {
        case "Number":
        case "String":
        case "Boolean":
          values.push(`Object(${stringify3(thing.valueOf())})`);
          break;
        case "RegExp":
          values.push(thing.toString());
          break;
        case "Date":
          values.push(`new Date(${thing.getTime()})`);
          break;
        case "Array":
          values.push(`Array(${thing.length})`);
          thing.forEach((v, i) => {
            statements.push(`${name}[${i}]=${stringify3(v)}`);
          });
          break;
        case "Set":
          values.push(`new Set`);
          statements.push(
            `${name}.${Array.from(thing).map((v) => `add(${stringify3(v)})`).join(".")}`
          );
          break;
        case "Map":
          values.push(`new Map`);
          statements.push(
            `${name}.${Array.from(thing).map(([k, v]) => `set(${stringify3(k)}, ${stringify3(v)})`).join(".")}`
          );
          break;
        case "ArrayBuffer":
          values.push(
            `new Uint8Array([${new Uint8Array(thing).join(",")}]).buffer`
          );
          break;
        default:
          values.push(
            Object.getPrototypeOf(thing) === null ? "Object.create(null)" : "{}"
          );
          Object.keys(thing).forEach((key2) => {
            statements.push(
              `${name}${safe_prop(key2)}=${stringify3(thing[key2])}`
            );
          });
      }
    });
    statements.push(`return ${str}`);
    return `(function(${params.join(",")}){${statements.join(
      ";"
    )}}(${values.join(",")}))`;
  } else {
    return str;
  }
}
__name(uneval, "uneval");
function get_name(num) {
  let name = "";
  do {
    name = chars[num % chars.length] + name;
    num = ~~(num / chars.length) - 1;
  } while (num >= 0);
  return reserved.test(name) ? `${name}0` : name;
}
__name(get_name, "get_name");
function escape_unsafe_char(c2) {
  return escaped[c2] || c2;
}
__name(escape_unsafe_char, "escape_unsafe_char");
function escape_unsafe_chars(str) {
  return str.replace(unsafe_chars, escape_unsafe_char);
}
__name(escape_unsafe_chars, "escape_unsafe_chars");
function safe_key(key2) {
  return /^[_$a-zA-Z][_$a-zA-Z0-9]*$/.test(key2) ? key2 : escape_unsafe_chars(JSON.stringify(key2));
}
__name(safe_key, "safe_key");
function safe_prop(key2) {
  return /^[_$a-zA-Z][_$a-zA-Z0-9]*$/.test(key2) ? `.${key2}` : `[${escape_unsafe_chars(JSON.stringify(key2))}]`;
}
__name(safe_prop, "safe_prop");
function stringify_primitive(thing) {
  if (typeof thing === "string") return stringify_string(thing);
  if (thing === void 0) return "void 0";
  if (thing === 0 && 1 / thing < 0) return "-0";
  const str = String(thing);
  if (typeof thing === "number") return str.replace(/^(-)?0\./, "$1.");
  if (typeof thing === "bigint") return thing + "n";
  return str;
}
__name(stringify_primitive, "stringify_primitive");

// node_modules/devalue/src/parse.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();

// node_modules/devalue/src/base64.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
function encode64(arraybuffer) {
  const dv = new DataView(arraybuffer);
  let binaryString = "";
  for (let i = 0; i < arraybuffer.byteLength; i++) {
    binaryString += String.fromCharCode(dv.getUint8(i));
  }
  return binaryToAscii(binaryString);
}
__name(encode64, "encode64");
function decode64(string) {
  const binaryString = asciiToBinary(string);
  const arraybuffer = new ArrayBuffer(binaryString.length);
  const dv = new DataView(arraybuffer);
  for (let i = 0; i < arraybuffer.byteLength; i++) {
    dv.setUint8(i, binaryString.charCodeAt(i));
  }
  return arraybuffer;
}
__name(decode64, "decode64");
var KEY_STRING = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
function asciiToBinary(data) {
  if (data.length % 4 === 0) {
    data = data.replace(/==?$/, "");
  }
  let output = "";
  let buffer = 0;
  let accumulatedBits = 0;
  for (let i = 0; i < data.length; i++) {
    buffer <<= 6;
    buffer |= KEY_STRING.indexOf(data[i]);
    accumulatedBits += 6;
    if (accumulatedBits === 24) {
      output += String.fromCharCode((buffer & 16711680) >> 16);
      output += String.fromCharCode((buffer & 65280) >> 8);
      output += String.fromCharCode(buffer & 255);
      buffer = accumulatedBits = 0;
    }
  }
  if (accumulatedBits === 12) {
    buffer >>= 4;
    output += String.fromCharCode(buffer);
  } else if (accumulatedBits === 18) {
    buffer >>= 2;
    output += String.fromCharCode((buffer & 65280) >> 8);
    output += String.fromCharCode(buffer & 255);
  }
  return output;
}
__name(asciiToBinary, "asciiToBinary");
function binaryToAscii(str) {
  let out = "";
  for (let i = 0; i < str.length; i += 3) {
    const groupsOfSix = [void 0, void 0, void 0, void 0];
    groupsOfSix[0] = str.charCodeAt(i) >> 2;
    groupsOfSix[1] = (str.charCodeAt(i) & 3) << 4;
    if (str.length > i + 1) {
      groupsOfSix[1] |= str.charCodeAt(i + 1) >> 4;
      groupsOfSix[2] = (str.charCodeAt(i + 1) & 15) << 2;
    }
    if (str.length > i + 2) {
      groupsOfSix[2] |= str.charCodeAt(i + 2) >> 6;
      groupsOfSix[3] = str.charCodeAt(i + 2) & 63;
    }
    for (let j = 0; j < groupsOfSix.length; j++) {
      if (typeof groupsOfSix[j] === "undefined") {
        out += "=";
      } else {
        out += KEY_STRING[groupsOfSix[j]];
      }
    }
  }
  return out;
}
__name(binaryToAscii, "binaryToAscii");

// node_modules/devalue/src/constants.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var UNDEFINED = -1;
var HOLE = -2;
var NAN = -3;
var POSITIVE_INFINITY = -4;
var NEGATIVE_INFINITY = -5;
var NEGATIVE_ZERO = -6;

// node_modules/devalue/src/parse.js
function parse(serialized, revivers) {
  return unflatten(JSON.parse(serialized), revivers);
}
__name(parse, "parse");
function unflatten(parsed, revivers) {
  if (typeof parsed === "number") return hydrate2(parsed, true);
  if (!Array.isArray(parsed) || parsed.length === 0) {
    throw new Error("Invalid input");
  }
  const values = (
    /** @type {any[]} */
    parsed
  );
  const hydrated = Array(values.length);
  function hydrate2(index5, standalone = false) {
    if (index5 === UNDEFINED) return void 0;
    if (index5 === NAN) return NaN;
    if (index5 === POSITIVE_INFINITY) return Infinity;
    if (index5 === NEGATIVE_INFINITY) return -Infinity;
    if (index5 === NEGATIVE_ZERO) return -0;
    if (standalone || typeof index5 !== "number") {
      throw new Error(`Invalid input`);
    }
    if (index5 in hydrated) return hydrated[index5];
    const value = values[index5];
    if (!value || typeof value !== "object") {
      hydrated[index5] = value;
    } else if (Array.isArray(value)) {
      if (typeof value[0] === "string") {
        const type = value[0];
        const reviver = revivers?.[type];
        if (reviver) {
          let i = value[1];
          if (typeof i !== "number") {
            i = values.push(value[1]) - 1;
          }
          return hydrated[index5] = reviver(hydrate2(i));
        }
        switch (type) {
          case "Date":
            hydrated[index5] = new Date(value[1]);
            break;
          case "Set":
            const set2 = /* @__PURE__ */ new Set();
            hydrated[index5] = set2;
            for (let i = 1; i < value.length; i += 1) {
              set2.add(hydrate2(value[i]));
            }
            break;
          case "Map":
            const map = /* @__PURE__ */ new Map();
            hydrated[index5] = map;
            for (let i = 1; i < value.length; i += 2) {
              map.set(hydrate2(value[i]), hydrate2(value[i + 1]));
            }
            break;
          case "RegExp":
            hydrated[index5] = new RegExp(value[1], value[2]);
            break;
          case "Object":
            hydrated[index5] = Object(value[1]);
            break;
          case "BigInt":
            hydrated[index5] = BigInt(value[1]);
            break;
          case "null":
            const obj = /* @__PURE__ */ Object.create(null);
            hydrated[index5] = obj;
            for (let i = 1; i < value.length; i += 2) {
              obj[value[i]] = hydrate2(value[i + 1]);
            }
            break;
          case "Int8Array":
          case "Uint8Array":
          case "Uint8ClampedArray":
          case "Int16Array":
          case "Uint16Array":
          case "Int32Array":
          case "Uint32Array":
          case "Float32Array":
          case "Float64Array":
          case "BigInt64Array":
          case "BigUint64Array": {
            const TypedArrayConstructor = globalThis[type];
            const typedArray = new TypedArrayConstructor(hydrate2(value[1]));
            hydrated[index5] = value[2] !== void 0 ? typedArray.subarray(value[2], value[3]) : typedArray;
            break;
          }
          case "ArrayBuffer": {
            const base64 = value[1];
            const arraybuffer = decode64(base64);
            hydrated[index5] = arraybuffer;
            break;
          }
          case "Temporal.Duration":
          case "Temporal.Instant":
          case "Temporal.PlainDate":
          case "Temporal.PlainTime":
          case "Temporal.PlainDateTime":
          case "Temporal.PlainMonthDay":
          case "Temporal.PlainYearMonth":
          case "Temporal.ZonedDateTime": {
            const temporalName = type.slice(9);
            hydrated[index5] = Temporal[temporalName].from(value[1]);
            break;
          }
          case "URL": {
            const url = new URL(value[1]);
            hydrated[index5] = url;
            break;
          }
          case "URLSearchParams": {
            const url = new URLSearchParams(value[1]);
            hydrated[index5] = url;
            break;
          }
          default:
            throw new Error(`Unknown type ${type}`);
        }
      } else {
        const array2 = new Array(value.length);
        hydrated[index5] = array2;
        for (let i = 0; i < value.length; i += 1) {
          const n2 = value[i];
          if (n2 === HOLE) continue;
          array2[i] = hydrate2(n2);
        }
      }
    } else {
      const object = {};
      hydrated[index5] = object;
      for (const key2 in value) {
        if (key2 === "__proto__") {
          throw new Error("Cannot parse an object with a `__proto__` property");
        }
        const n2 = value[key2];
        object[key2] = hydrate2(n2);
      }
    }
    return hydrated[index5];
  }
  __name(hydrate2, "hydrate");
  return hydrate2(0);
}
__name(unflatten, "unflatten");

// node_modules/devalue/src/stringify.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
function stringify(value, reducers) {
  const stringified = [];
  const indexes = /* @__PURE__ */ new Map();
  const custom = [];
  if (reducers) {
    for (const key2 of Object.getOwnPropertyNames(reducers)) {
      custom.push({ key: key2, fn: reducers[key2] });
    }
  }
  const keys = [];
  let p = 0;
  function flatten(thing) {
    if (typeof thing === "function") {
      throw new DevalueError(`Cannot stringify a function`, keys);
    }
    if (thing === void 0) return UNDEFINED;
    if (Number.isNaN(thing)) return NAN;
    if (thing === Infinity) return POSITIVE_INFINITY;
    if (thing === -Infinity) return NEGATIVE_INFINITY;
    if (thing === 0 && 1 / thing < 0) return NEGATIVE_ZERO;
    if (indexes.has(thing)) return indexes.get(thing);
    const index6 = p++;
    indexes.set(thing, index6);
    for (const { key: key2, fn } of custom) {
      const value2 = fn(thing);
      if (value2) {
        stringified[index6] = `["${key2}",${flatten(value2)}]`;
        return index6;
      }
    }
    let str = "";
    if (is_primitive(thing)) {
      str = stringify_primitive2(thing);
    } else {
      const type = get_type(thing);
      switch (type) {
        case "Number":
        case "String":
        case "Boolean":
          str = `["Object",${stringify_primitive2(thing)}]`;
          break;
        case "BigInt":
          str = `["BigInt",${thing}]`;
          break;
        case "Date":
          const valid = !isNaN(thing.getDate());
          str = `["Date","${valid ? thing.toISOString() : ""}"]`;
          break;
        case "URL":
          str = `["URL",${stringify_string(thing.toString())}]`;
          break;
        case "URLSearchParams":
          str = `["URLSearchParams",${stringify_string(thing.toString())}]`;
          break;
        case "RegExp":
          const { source: source2, flags: flags2 } = thing;
          str = flags2 ? `["RegExp",${stringify_string(source2)},"${flags2}"]` : `["RegExp",${stringify_string(source2)}]`;
          break;
        case "Array":
          str = "[";
          for (let i = 0; i < thing.length; i += 1) {
            if (i > 0) str += ",";
            if (i in thing) {
              keys.push(`[${i}]`);
              str += flatten(thing[i]);
              keys.pop();
            } else {
              str += HOLE;
            }
          }
          str += "]";
          break;
        case "Set":
          str = '["Set"';
          for (const value2 of thing) {
            str += `,${flatten(value2)}`;
          }
          str += "]";
          break;
        case "Map":
          str = '["Map"';
          for (const [key2, value2] of thing) {
            keys.push(
              `.get(${is_primitive(key2) ? stringify_primitive2(key2) : "..."})`
            );
            str += `,${flatten(key2)},${flatten(value2)}`;
            keys.pop();
          }
          str += "]";
          break;
        case "Int8Array":
        case "Uint8Array":
        case "Uint8ClampedArray":
        case "Int16Array":
        case "Uint16Array":
        case "Int32Array":
        case "Uint32Array":
        case "Float32Array":
        case "Float64Array":
        case "BigInt64Array":
        case "BigUint64Array": {
          const typedArray = thing;
          str = '["' + type + '",' + flatten(typedArray.buffer);
          const a = thing.byteOffset;
          const b = a + thing.byteLength;
          if (a > 0 || b !== typedArray.buffer.byteLength) {
            const m = +/(\d+)/.exec(type)[1] / 8;
            str += `,${a / m},${b / m}`;
          }
          str += "]";
          break;
        }
        case "ArrayBuffer": {
          const arraybuffer = thing;
          const base64 = encode64(arraybuffer);
          str = `["ArrayBuffer","${base64}"]`;
          break;
        }
        case "Temporal.Duration":
        case "Temporal.Instant":
        case "Temporal.PlainDate":
        case "Temporal.PlainTime":
        case "Temporal.PlainDateTime":
        case "Temporal.PlainMonthDay":
        case "Temporal.PlainYearMonth":
        case "Temporal.ZonedDateTime":
          str = `["${type}",${stringify_string(thing.toString())}]`;
          break;
        default:
          if (!is_plain_object(thing)) {
            throw new DevalueError(
              `Cannot stringify arbitrary non-POJOs`,
              keys
            );
          }
          if (enumerable_symbols(thing).length > 0) {
            throw new DevalueError(
              `Cannot stringify POJOs with symbolic keys`,
              keys
            );
          }
          if (Object.getPrototypeOf(thing) === null) {
            str = '["null"';
            for (const key2 in thing) {
              keys.push(stringify_key(key2));
              str += `,${stringify_string(key2)},${flatten(thing[key2])}`;
              keys.pop();
            }
            str += "]";
          } else {
            str = "{";
            let started = false;
            for (const key2 in thing) {
              if (started) str += ",";
              started = true;
              keys.push(stringify_key(key2));
              str += `${stringify_string(key2)}:${flatten(thing[key2])}`;
              keys.pop();
            }
            str += "}";
          }
      }
    }
    stringified[index6] = str;
    return index6;
  }
  __name(flatten, "flatten");
  const index5 = flatten(value);
  if (index5 < 0) return `${index5}`;
  return `[${stringified.join(",")}]`;
}
__name(stringify, "stringify");
function stringify_primitive2(thing) {
  const type = typeof thing;
  if (type === "string") return stringify_string(thing);
  if (thing instanceof String) return stringify_string(thing.toString());
  if (thing === void 0) return UNDEFINED.toString();
  if (thing === 0 && 1 / thing < 0) return NEGATIVE_ZERO.toString();
  if (type === "bigint") return `["BigInt","${thing}"]`;
  return String(thing);
}
__name(stringify_primitive2, "stringify_primitive");

// .svelte-kit/output/server/index.js
init_exports2();
init_utils3();

// .svelte-kit/output/server/chunks/internal.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
init_chunks();
init_equality();
init_clsx();
init_context();
var public_env = {};
function set_private_env(environment) {
}
__name(set_private_env, "set_private_env");
function set_public_env(environment) {
  public_env = environment;
}
__name(set_public_env, "set_public_env");
function effect_update_depth_exceeded() {
  {
    throw new Error(`https://svelte.dev/e/effect_update_depth_exceeded`);
  }
}
__name(effect_update_depth_exceeded, "effect_update_depth_exceeded");
function hydration_failed() {
  {
    throw new Error(`https://svelte.dev/e/hydration_failed`);
  }
}
__name(hydration_failed, "hydration_failed");
function state_descriptors_fixed() {
  {
    throw new Error(`https://svelte.dev/e/state_descriptors_fixed`);
  }
}
__name(state_descriptors_fixed, "state_descriptors_fixed");
function state_prototype_fixed() {
  {
    throw new Error(`https://svelte.dev/e/state_prototype_fixed`);
  }
}
__name(state_prototype_fixed, "state_prototype_fixed");
function state_unsafe_mutation() {
  {
    throw new Error(`https://svelte.dev/e/state_unsafe_mutation`);
  }
}
__name(state_unsafe_mutation, "state_unsafe_mutation");
function svelte_boundary_reset_onerror() {
  {
    throw new Error(`https://svelte.dev/e/svelte_boundary_reset_onerror`);
  }
}
__name(svelte_boundary_reset_onerror, "svelte_boundary_reset_onerror");
function hydration_mismatch(location) {
  {
    console.warn(`https://svelte.dev/e/hydration_mismatch`);
  }
}
__name(hydration_mismatch, "hydration_mismatch");
function svelte_boundary_reset_noop() {
  {
    console.warn(`https://svelte.dev/e/svelte_boundary_reset_noop`);
  }
}
__name(svelte_boundary_reset_noop, "svelte_boundary_reset_noop");
var hydrating = false;
function set_hydrating(value) {
  hydrating = value;
}
__name(set_hydrating, "set_hydrating");
var hydrate_node;
function set_hydrate_node(node) {
  if (node === null) {
    hydration_mismatch();
    throw HYDRATION_ERROR;
  }
  return hydrate_node = node;
}
__name(set_hydrate_node, "set_hydrate_node");
function hydrate_next() {
  return set_hydrate_node(
    /** @type {TemplateNode} */
    /* @__PURE__ */ get_next_sibling(hydrate_node)
  );
}
__name(hydrate_next, "hydrate_next");
function next(count3 = 1) {
  if (hydrating) {
    var i = count3;
    var node = hydrate_node;
    while (i--) {
      node = /** @type {TemplateNode} */
      /* @__PURE__ */ get_next_sibling(node);
    }
    hydrate_node = node;
  }
}
__name(next, "next");
function skip_nodes(remove = true) {
  var depth = 0;
  var node = hydrate_node;
  while (true) {
    if (node.nodeType === COMMENT_NODE) {
      var data = (
        /** @type {Comment} */
        node.data
      );
      if (data === HYDRATION_END) {
        if (depth === 0) return node;
        depth -= 1;
      } else if (data === HYDRATION_START || data === HYDRATION_START_ELSE) {
        depth += 1;
      }
    }
    var next2 = (
      /** @type {TemplateNode} */
      /* @__PURE__ */ get_next_sibling(node)
    );
    if (remove) node.remove();
    node = next2;
  }
}
__name(skip_nodes, "skip_nodes");
var tracing_mode_flag = false;
var component_context = null;
function set_component_context(context3) {
  component_context = context3;
}
__name(set_component_context, "set_component_context");
function push2(props, runes = false, fn) {
  component_context = {
    p: component_context,
    i: false,
    c: null,
    e: null,
    s: props,
    x: null,
    l: null
  };
}
__name(push2, "push");
function pop2(component5) {
  var context3 = (
    /** @type {ComponentContext} */
    component_context
  );
  var effects = context3.e;
  if (effects !== null) {
    context3.e = null;
    for (var fn of effects) {
      create_user_effect(fn);
    }
  }
  context3.i = true;
  component_context = context3.p;
  return (
    /** @type {T} */
    {}
  );
}
__name(pop2, "pop");
function is_runes() {
  return true;
}
__name(is_runes, "is_runes");
var micro_tasks = [];
function run_micro_tasks() {
  var tasks = micro_tasks;
  micro_tasks = [];
  run_all(tasks);
}
__name(run_micro_tasks, "run_micro_tasks");
function queue_micro_task(fn) {
  if (micro_tasks.length === 0 && !is_flushing_sync) {
    var tasks = micro_tasks;
    queueMicrotask(() => {
      if (tasks === micro_tasks) run_micro_tasks();
    });
  }
  micro_tasks.push(fn);
}
__name(queue_micro_task, "queue_micro_task");
function flush_tasks() {
  while (micro_tasks.length > 0) {
    run_micro_tasks();
  }
}
__name(flush_tasks, "flush_tasks");
function handle_error(error4) {
  var effect = active_effect;
  if (effect === null) {
    active_reaction.f |= ERROR_VALUE;
    return error4;
  }
  if ((effect.f & EFFECT_RAN) === 0) {
    if ((effect.f & BOUNDARY_EFFECT) === 0) {
      throw error4;
    }
    effect.b.error(error4);
  } else {
    invoke_error_boundary(error4, effect);
  }
}
__name(handle_error, "handle_error");
function invoke_error_boundary(error4, effect) {
  while (effect !== null) {
    if ((effect.f & BOUNDARY_EFFECT) !== 0) {
      try {
        effect.b.error(error4);
        return;
      } catch (e3) {
        error4 = e3;
      }
    }
    effect = effect.parent;
  }
  throw error4;
}
__name(invoke_error_boundary, "invoke_error_boundary");
var batches = /* @__PURE__ */ new Set();
var current_batch = null;
var batch_values = null;
var queued_root_effects = [];
var last_scheduled_effect = null;
var is_flushing = false;
var is_flushing_sync = false;
var Batch = class _Batch {
  static {
    __name(this, "Batch");
  }
  committed = false;
  /**
   * The current values of any sources that are updated in this batch
   * They keys of this map are identical to `this.#previous`
   * @type {Map<Source, any>}
   */
  current = /* @__PURE__ */ new Map();
  /**
   * The values of any sources that are updated in this batch _before_ those updates took place.
   * They keys of this map are identical to `this.#current`
   * @type {Map<Source, any>}
   */
  previous = /* @__PURE__ */ new Map();
  /**
   * When the batch is committed (and the DOM is updated), we need to remove old branches
   * and append new ones by calling the functions added inside (if/each/key/etc) blocks
   * @type {Set<() => void>}
   */
  #commit_callbacks = /* @__PURE__ */ new Set();
  /**
   * If a fork is discarded, we need to destroy any effects that are no longer needed
   * @type {Set<(batch: Batch) => void>}
   */
  #discard_callbacks = /* @__PURE__ */ new Set();
  /**
   * The number of async effects that are currently in flight
   */
  #pending = 0;
  /**
   * The number of async effects that are currently in flight, _not_ inside a pending boundary
   */
  #blocking_pending = 0;
  /**
   * A deferred that resolves when the batch is committed, used with `settled()`
   * TODO replace with Promise.withResolvers once supported widely enough
   * @type {{ promise: Promise<void>, resolve: (value?: any) => void, reject: (reason: unknown) => void } | null}
   */
  #deferred = null;
  /**
   * Deferred effects (which run after async work has completed) that are DIRTY
   * @type {Effect[]}
   */
  #dirty_effects = [];
  /**
   * Deferred effects that are MAYBE_DIRTY
   * @type {Effect[]}
   */
  #maybe_dirty_effects = [];
  /**
   * A set of branches that still exist, but will be destroyed when this batch
   * is committed — we skip over these during `process`
   * @type {Set<Effect>}
   */
  skipped_effects = /* @__PURE__ */ new Set();
  is_fork = false;
  /**
   *
   * @param {Effect[]} root_effects
   */
  process(root_effects) {
    queued_root_effects = [];
    this.apply();
    var target = {
      parent: null,
      effect: null,
      effects: [],
      render_effects: [],
      block_effects: []
    };
    for (const root2 of root_effects) {
      this.#traverse_effect_tree(root2, target);
    }
    if (!this.is_fork) {
      this.#resolve();
    }
    if (this.#blocking_pending > 0 || this.is_fork) {
      this.#defer_effects(target.effects);
      this.#defer_effects(target.render_effects);
      this.#defer_effects(target.block_effects);
    } else {
      current_batch = null;
      flush_queued_effects(target.render_effects);
      flush_queued_effects(target.effects);
      this.#deferred?.resolve();
    }
    batch_values = null;
  }
  /**
   * Traverse the effect tree, executing effects or stashing
   * them for later execution as appropriate
   * @param {Effect} root
   * @param {EffectTarget} target
   */
  #traverse_effect_tree(root2, target) {
    root2.f ^= CLEAN;
    var effect = root2.first;
    while (effect !== null) {
      var flags2 = effect.f;
      var is_branch = (flags2 & (BRANCH_EFFECT | ROOT_EFFECT)) !== 0;
      var is_skippable_branch = is_branch && (flags2 & CLEAN) !== 0;
      var skip = is_skippable_branch || (flags2 & INERT) !== 0 || this.skipped_effects.has(effect);
      if ((effect.f & BOUNDARY_EFFECT) !== 0 && effect.b?.is_pending()) {
        target = {
          parent: target,
          effect,
          effects: [],
          render_effects: [],
          block_effects: []
        };
      }
      if (!skip && effect.fn !== null) {
        if (is_branch) {
          effect.f ^= CLEAN;
        } else if ((flags2 & EFFECT) !== 0) {
          target.effects.push(effect);
        } else if (is_dirty(effect)) {
          if ((effect.f & BLOCK_EFFECT) !== 0) target.block_effects.push(effect);
          update_effect(effect);
        }
        var child = effect.first;
        if (child !== null) {
          effect = child;
          continue;
        }
      }
      var parent = effect.parent;
      effect = effect.next;
      while (effect === null && parent !== null) {
        if (parent === target.effect) {
          this.#defer_effects(target.effects);
          this.#defer_effects(target.render_effects);
          this.#defer_effects(target.block_effects);
          target = /** @type {EffectTarget} */
          target.parent;
        }
        effect = parent.next;
        parent = parent.parent;
      }
    }
  }
  /**
   * @param {Effect[]} effects
   */
  #defer_effects(effects) {
    for (const e3 of effects) {
      const target = (e3.f & DIRTY) !== 0 ? this.#dirty_effects : this.#maybe_dirty_effects;
      target.push(e3);
      set_signal_status(e3, CLEAN);
    }
  }
  /**
   * Associate a change to a given source with the current
   * batch, noting its previous and current values
   * @param {Source} source
   * @param {any} value
   */
  capture(source2, value) {
    if (!this.previous.has(source2)) {
      this.previous.set(source2, value);
    }
    if ((source2.f & ERROR_VALUE) === 0) {
      this.current.set(source2, source2.v);
      batch_values?.set(source2, source2.v);
    }
  }
  activate() {
    current_batch = this;
    this.apply();
  }
  deactivate() {
    current_batch = null;
    batch_values = null;
  }
  flush() {
    this.activate();
    if (queued_root_effects.length > 0) {
      flush_effects();
      if (current_batch !== null && current_batch !== this) {
        return;
      }
    } else if (this.#pending === 0) {
      this.process([]);
    }
    this.deactivate();
  }
  discard() {
    for (const fn of this.#discard_callbacks) fn(this);
    this.#discard_callbacks.clear();
  }
  #resolve() {
    if (this.#blocking_pending === 0) {
      for (const fn of this.#commit_callbacks) fn();
      this.#commit_callbacks.clear();
    }
    if (this.#pending === 0) {
      this.#commit();
    }
  }
  #commit() {
    if (batches.size > 1) {
      this.previous.clear();
      var previous_batch_values = batch_values;
      var is_earlier = true;
      var dummy_target = {
        parent: null,
        effect: null,
        effects: [],
        render_effects: [],
        block_effects: []
      };
      for (const batch of batches) {
        if (batch === this) {
          is_earlier = false;
          continue;
        }
        const sources = [];
        for (const [source2, value] of this.current) {
          if (batch.current.has(source2)) {
            if (is_earlier && value !== batch.current.get(source2)) {
              batch.current.set(source2, value);
            } else {
              continue;
            }
          }
          sources.push(source2);
        }
        if (sources.length === 0) {
          continue;
        }
        const others = [...batch.current.keys()].filter((s3) => !this.current.has(s3));
        if (others.length > 0) {
          const marked = /* @__PURE__ */ new Set();
          const checked = /* @__PURE__ */ new Map();
          for (const source2 of sources) {
            mark_effects(source2, others, marked, checked);
          }
          if (queued_root_effects.length > 0) {
            current_batch = batch;
            batch.apply();
            for (const root2 of queued_root_effects) {
              batch.#traverse_effect_tree(root2, dummy_target);
            }
            queued_root_effects = [];
            batch.deactivate();
          }
        }
      }
      current_batch = null;
      batch_values = previous_batch_values;
    }
    this.committed = true;
    batches.delete(this);
  }
  /**
   *
   * @param {boolean} blocking
   */
  increment(blocking) {
    this.#pending += 1;
    if (blocking) this.#blocking_pending += 1;
  }
  /**
   *
   * @param {boolean} blocking
   */
  decrement(blocking) {
    this.#pending -= 1;
    if (blocking) this.#blocking_pending -= 1;
    this.revive();
  }
  revive() {
    for (const e3 of this.#dirty_effects) {
      set_signal_status(e3, DIRTY);
      schedule_effect(e3);
    }
    for (const e3 of this.#maybe_dirty_effects) {
      set_signal_status(e3, MAYBE_DIRTY);
      schedule_effect(e3);
    }
    this.#dirty_effects = [];
    this.#maybe_dirty_effects = [];
    this.flush();
  }
  /** @param {() => void} fn */
  oncommit(fn) {
    this.#commit_callbacks.add(fn);
  }
  /** @param {(batch: Batch) => void} fn */
  ondiscard(fn) {
    this.#discard_callbacks.add(fn);
  }
  settled() {
    return (this.#deferred ??= deferred()).promise;
  }
  static ensure() {
    if (current_batch === null) {
      const batch = current_batch = new _Batch();
      batches.add(current_batch);
      if (!is_flushing_sync) {
        _Batch.enqueue(() => {
          if (current_batch !== batch) {
            return;
          }
          batch.flush();
        });
      }
    }
    return current_batch;
  }
  /** @param {() => void} task */
  static enqueue(task) {
    queue_micro_task(task);
  }
  apply() {
    return;
  }
};
function flushSync(fn) {
  var was_flushing_sync = is_flushing_sync;
  is_flushing_sync = true;
  try {
    var result;
    if (fn) ;
    while (true) {
      flush_tasks();
      if (queued_root_effects.length === 0) {
        current_batch?.flush();
        if (queued_root_effects.length === 0) {
          last_scheduled_effect = null;
          return (
            /** @type {T} */
            result
          );
        }
      }
      flush_effects();
    }
  } finally {
    is_flushing_sync = was_flushing_sync;
  }
}
__name(flushSync, "flushSync");
function flush_effects() {
  var was_updating_effect = is_updating_effect;
  is_flushing = true;
  try {
    var flush_count = 0;
    set_is_updating_effect(true);
    while (queued_root_effects.length > 0) {
      var batch = Batch.ensure();
      if (flush_count++ > 1e3) {
        var updates, entry;
        if (BROWSER) ;
        infinite_loop_guard();
      }
      batch.process(queued_root_effects);
      old_values.clear();
    }
  } finally {
    is_flushing = false;
    set_is_updating_effect(was_updating_effect);
    last_scheduled_effect = null;
  }
}
__name(flush_effects, "flush_effects");
function infinite_loop_guard() {
  try {
    effect_update_depth_exceeded();
  } catch (error4) {
    invoke_error_boundary(error4, last_scheduled_effect);
  }
}
__name(infinite_loop_guard, "infinite_loop_guard");
var eager_block_effects = null;
function flush_queued_effects(effects) {
  var length = effects.length;
  if (length === 0) return;
  var i = 0;
  while (i < length) {
    var effect = effects[i++];
    if ((effect.f & (DESTROYED | INERT)) === 0 && is_dirty(effect)) {
      eager_block_effects = /* @__PURE__ */ new Set();
      update_effect(effect);
      if (effect.deps === null && effect.first === null && effect.nodes_start === null) {
        if (effect.teardown === null && effect.ac === null) {
          unlink_effect(effect);
        } else {
          effect.fn = null;
        }
      }
      if (eager_block_effects?.size > 0) {
        old_values.clear();
        for (const e3 of eager_block_effects) {
          if ((e3.f & (DESTROYED | INERT)) !== 0) continue;
          const ordered_effects = [e3];
          let ancestor = e3.parent;
          while (ancestor !== null) {
            if (eager_block_effects.has(ancestor)) {
              eager_block_effects.delete(ancestor);
              ordered_effects.push(ancestor);
            }
            ancestor = ancestor.parent;
          }
          for (let j = ordered_effects.length - 1; j >= 0; j--) {
            const e22 = ordered_effects[j];
            if ((e22.f & (DESTROYED | INERT)) !== 0) continue;
            update_effect(e22);
          }
        }
        eager_block_effects.clear();
      }
    }
  }
  eager_block_effects = null;
}
__name(flush_queued_effects, "flush_queued_effects");
function mark_effects(value, sources, marked, checked) {
  if (marked.has(value)) return;
  marked.add(value);
  if (value.reactions !== null) {
    for (const reaction of value.reactions) {
      const flags2 = reaction.f;
      if ((flags2 & DERIVED) !== 0) {
        mark_effects(
          /** @type {Derived} */
          reaction,
          sources,
          marked,
          checked
        );
      } else if ((flags2 & (ASYNC | BLOCK_EFFECT)) !== 0 && (flags2 & DIRTY) === 0 && // we may have scheduled this one already
      depends_on(reaction, sources, checked)) {
        set_signal_status(reaction, DIRTY);
        schedule_effect(
          /** @type {Effect} */
          reaction
        );
      }
    }
  }
}
__name(mark_effects, "mark_effects");
function depends_on(reaction, sources, checked) {
  const depends = checked.get(reaction);
  if (depends !== void 0) return depends;
  if (reaction.deps !== null) {
    for (const dep of reaction.deps) {
      if (sources.includes(dep)) {
        return true;
      }
      if ((dep.f & DERIVED) !== 0 && depends_on(
        /** @type {Derived} */
        dep,
        sources,
        checked
      )) {
        checked.set(
          /** @type {Derived} */
          dep,
          true
        );
        return true;
      }
    }
  }
  checked.set(reaction, false);
  return false;
}
__name(depends_on, "depends_on");
function schedule_effect(signal) {
  var effect = last_scheduled_effect = signal;
  while (effect.parent !== null) {
    effect = effect.parent;
    var flags2 = effect.f;
    if (is_flushing && effect === active_effect && (flags2 & BLOCK_EFFECT) !== 0 && (flags2 & HEAD_EFFECT) === 0) {
      return;
    }
    if ((flags2 & (ROOT_EFFECT | BRANCH_EFFECT)) !== 0) {
      if ((flags2 & CLEAN) === 0) return;
      effect.f ^= CLEAN;
    }
  }
  queued_root_effects.push(effect);
}
__name(schedule_effect, "schedule_effect");
function createSubscriber(start) {
  let subscribers = 0;
  let version2 = source(0);
  let stop;
  return () => {
    if (effect_tracking()) {
      get(version2);
      render_effect(() => {
        if (subscribers === 0) {
          stop = untrack(() => start(() => increment(version2)));
        }
        subscribers += 1;
        return () => {
          queue_micro_task(() => {
            subscribers -= 1;
            if (subscribers === 0) {
              stop?.();
              stop = void 0;
              increment(version2);
            }
          });
        };
      });
    }
  };
}
__name(createSubscriber, "createSubscriber");
var flags = EFFECT_TRANSPARENT | EFFECT_PRESERVED | BOUNDARY_EFFECT;
function boundary(node, props, children) {
  new Boundary(node, props, children);
}
__name(boundary, "boundary");
var Boundary = class {
  static {
    __name(this, "Boundary");
  }
  /** @type {Boundary | null} */
  parent;
  #pending = false;
  /** @type {TemplateNode} */
  #anchor;
  /** @type {TemplateNode | null} */
  #hydrate_open = hydrating ? hydrate_node : null;
  /** @type {BoundaryProps} */
  #props;
  /** @type {((anchor: Node) => void)} */
  #children;
  /** @type {Effect} */
  #effect;
  /** @type {Effect | null} */
  #main_effect = null;
  /** @type {Effect | null} */
  #pending_effect = null;
  /** @type {Effect | null} */
  #failed_effect = null;
  /** @type {DocumentFragment | null} */
  #offscreen_fragment = null;
  /** @type {TemplateNode | null} */
  #pending_anchor = null;
  #local_pending_count = 0;
  #pending_count = 0;
  #is_creating_fallback = false;
  /**
   * A source containing the number of pending async deriveds/expressions.
   * Only created if `$effect.pending()` is used inside the boundary,
   * otherwise updating the source results in needless `Batch.ensure()`
   * calls followed by no-op flushes
   * @type {Source<number> | null}
   */
  #effect_pending = null;
  #effect_pending_subscriber = createSubscriber(() => {
    this.#effect_pending = source(this.#local_pending_count);
    return () => {
      this.#effect_pending = null;
    };
  });
  /**
   * @param {TemplateNode} node
   * @param {BoundaryProps} props
   * @param {((anchor: Node) => void)} children
   */
  constructor(node, props, children) {
    this.#anchor = node;
    this.#props = props;
    this.#children = children;
    this.parent = /** @type {Effect} */
    active_effect.b;
    this.#pending = !!this.#props.pending;
    this.#effect = block(() => {
      active_effect.b = this;
      if (hydrating) {
        const comment = this.#hydrate_open;
        hydrate_next();
        const server_rendered_pending = (
          /** @type {Comment} */
          comment.nodeType === COMMENT_NODE && /** @type {Comment} */
          comment.data === HYDRATION_START_ELSE
        );
        if (server_rendered_pending) {
          this.#hydrate_pending_content();
        } else {
          this.#hydrate_resolved_content();
        }
      } else {
        var anchor = this.#get_anchor();
        try {
          this.#main_effect = branch(() => children(anchor));
        } catch (error4) {
          this.error(error4);
        }
        if (this.#pending_count > 0) {
          this.#show_pending_snippet();
        } else {
          this.#pending = false;
        }
      }
      return () => {
        this.#pending_anchor?.remove();
      };
    }, flags);
    if (hydrating) {
      this.#anchor = hydrate_node;
    }
  }
  #hydrate_resolved_content() {
    try {
      this.#main_effect = branch(() => this.#children(this.#anchor));
    } catch (error4) {
      this.error(error4);
    }
    this.#pending = false;
  }
  #hydrate_pending_content() {
    const pending = this.#props.pending;
    if (!pending) {
      return;
    }
    this.#pending_effect = branch(() => pending(this.#anchor));
    Batch.enqueue(() => {
      var anchor = this.#get_anchor();
      this.#main_effect = this.#run(() => {
        Batch.ensure();
        return branch(() => this.#children(anchor));
      });
      if (this.#pending_count > 0) {
        this.#show_pending_snippet();
      } else {
        pause_effect(
          /** @type {Effect} */
          this.#pending_effect,
          () => {
            this.#pending_effect = null;
          }
        );
        this.#pending = false;
      }
    });
  }
  #get_anchor() {
    var anchor = this.#anchor;
    if (this.#pending) {
      this.#pending_anchor = create_text();
      this.#anchor.before(this.#pending_anchor);
      anchor = this.#pending_anchor;
    }
    return anchor;
  }
  /**
   * Returns `true` if the effect exists inside a boundary whose pending snippet is shown
   * @returns {boolean}
   */
  is_pending() {
    return this.#pending || !!this.parent && this.parent.is_pending();
  }
  has_pending_snippet() {
    return !!this.#props.pending;
  }
  /**
   * @param {() => Effect | null} fn
   */
  #run(fn) {
    var previous_effect = active_effect;
    var previous_reaction = active_reaction;
    var previous_ctx = component_context;
    set_active_effect(this.#effect);
    set_active_reaction(this.#effect);
    set_component_context(this.#effect.ctx);
    try {
      return fn();
    } catch (e3) {
      handle_error(e3);
      return null;
    } finally {
      set_active_effect(previous_effect);
      set_active_reaction(previous_reaction);
      set_component_context(previous_ctx);
    }
  }
  #show_pending_snippet() {
    const pending = (
      /** @type {(anchor: Node) => void} */
      this.#props.pending
    );
    if (this.#main_effect !== null) {
      this.#offscreen_fragment = document.createDocumentFragment();
      this.#offscreen_fragment.append(
        /** @type {TemplateNode} */
        this.#pending_anchor
      );
      move_effect(this.#main_effect, this.#offscreen_fragment);
    }
    if (this.#pending_effect === null) {
      this.#pending_effect = branch(() => pending(this.#anchor));
    }
  }
  /**
   * Updates the pending count associated with the currently visible pending snippet,
   * if any, such that we can replace the snippet with content once work is done
   * @param {1 | -1} d
   */
  #update_pending_count(d) {
    if (!this.has_pending_snippet()) {
      if (this.parent) {
        this.parent.#update_pending_count(d);
      }
      return;
    }
    this.#pending_count += d;
    if (this.#pending_count === 0) {
      this.#pending = false;
      if (this.#pending_effect) {
        pause_effect(this.#pending_effect, () => {
          this.#pending_effect = null;
        });
      }
      if (this.#offscreen_fragment) {
        this.#anchor.before(this.#offscreen_fragment);
        this.#offscreen_fragment = null;
      }
    }
  }
  /**
   * Update the source that powers `$effect.pending()` inside this boundary,
   * and controls when the current `pending` snippet (if any) is removed.
   * Do not call from inside the class
   * @param {1 | -1} d
   */
  update_pending_count(d) {
    this.#update_pending_count(d);
    this.#local_pending_count += d;
    if (this.#effect_pending) {
      internal_set(this.#effect_pending, this.#local_pending_count);
    }
  }
  get_effect_pending() {
    this.#effect_pending_subscriber();
    return get(
      /** @type {Source<number>} */
      this.#effect_pending
    );
  }
  /** @param {unknown} error */
  error(error4) {
    var onerror = this.#props.onerror;
    let failed = this.#props.failed;
    if (this.#is_creating_fallback || !onerror && !failed) {
      throw error4;
    }
    if (this.#main_effect) {
      destroy_effect(this.#main_effect);
      this.#main_effect = null;
    }
    if (this.#pending_effect) {
      destroy_effect(this.#pending_effect);
      this.#pending_effect = null;
    }
    if (this.#failed_effect) {
      destroy_effect(this.#failed_effect);
      this.#failed_effect = null;
    }
    if (hydrating) {
      set_hydrate_node(
        /** @type {TemplateNode} */
        this.#hydrate_open
      );
      next();
      set_hydrate_node(skip_nodes());
    }
    var did_reset = false;
    var calling_on_error = false;
    const reset2 = /* @__PURE__ */ __name(() => {
      if (did_reset) {
        svelte_boundary_reset_noop();
        return;
      }
      did_reset = true;
      if (calling_on_error) {
        svelte_boundary_reset_onerror();
      }
      Batch.ensure();
      this.#local_pending_count = 0;
      if (this.#failed_effect !== null) {
        pause_effect(this.#failed_effect, () => {
          this.#failed_effect = null;
        });
      }
      this.#pending = this.has_pending_snippet();
      this.#main_effect = this.#run(() => {
        this.#is_creating_fallback = false;
        return branch(() => this.#children(this.#anchor));
      });
      if (this.#pending_count > 0) {
        this.#show_pending_snippet();
      } else {
        this.#pending = false;
      }
    }, "reset");
    var previous_reaction = active_reaction;
    try {
      set_active_reaction(null);
      calling_on_error = true;
      onerror?.(error4, reset2);
      calling_on_error = false;
    } catch (error22) {
      invoke_error_boundary(error22, this.#effect && this.#effect.parent);
    } finally {
      set_active_reaction(previous_reaction);
    }
    if (failed) {
      queue_micro_task(() => {
        this.#failed_effect = this.#run(() => {
          Batch.ensure();
          this.#is_creating_fallback = true;
          try {
            return branch(() => {
              failed(
                this.#anchor,
                () => error4,
                () => reset2
              );
            });
          } catch (error22) {
            invoke_error_boundary(
              error22,
              /** @type {Effect} */
              this.#effect.parent
            );
            return null;
          } finally {
            this.#is_creating_fallback = false;
          }
        });
      });
    }
  }
};
function destroy_derived_effects(derived) {
  var effects = derived.effects;
  if (effects !== null) {
    derived.effects = null;
    for (var i = 0; i < effects.length; i += 1) {
      destroy_effect(
        /** @type {Effect} */
        effects[i]
      );
    }
  }
}
__name(destroy_derived_effects, "destroy_derived_effects");
function get_derived_parent_effect(derived) {
  var parent = derived.parent;
  while (parent !== null) {
    if ((parent.f & DERIVED) === 0) {
      return (
        /** @type {Effect} */
        parent
      );
    }
    parent = parent.parent;
  }
  return null;
}
__name(get_derived_parent_effect, "get_derived_parent_effect");
function execute_derived(derived) {
  var value;
  var prev_active_effect = active_effect;
  set_active_effect(get_derived_parent_effect(derived));
  {
    try {
      derived.f &= ~WAS_MARKED;
      destroy_derived_effects(derived);
      value = update_reaction(derived);
    } finally {
      set_active_effect(prev_active_effect);
    }
  }
  return value;
}
__name(execute_derived, "execute_derived");
function update_derived(derived) {
  var value = execute_derived(derived);
  if (!derived.equals(value)) {
    derived.v = value;
    derived.wv = increment_write_version();
  }
  if (is_destroying_effect) {
    return;
  }
  if (batch_values !== null) {
    if (effect_tracking()) {
      batch_values.set(derived, derived.v);
    }
  } else {
    var status = (derived.f & CONNECTED) === 0 ? MAYBE_DIRTY : CLEAN;
    set_signal_status(derived, status);
  }
}
__name(update_derived, "update_derived");
var eager_effects = /* @__PURE__ */ new Set();
var old_values = /* @__PURE__ */ new Map();
var eager_effects_deferred = false;
function source(v, stack) {
  var signal = {
    f: 0,
    // TODO ideally we could skip this altogether, but it causes type errors
    v,
    reactions: null,
    equals,
    rv: 0,
    wv: 0
  };
  return signal;
}
__name(source, "source");
// @__NO_SIDE_EFFECTS__
function state(v, stack) {
  const s3 = source(v);
  push_reaction_value(s3);
  return s3;
}
__name(state, "state");
// @__NO_SIDE_EFFECTS__
function mutable_source(initial_value, immutable2 = false, trackable = true) {
  const s3 = source(initial_value);
  if (!immutable2) {
    s3.equals = safe_equals;
  }
  return s3;
}
__name(mutable_source, "mutable_source");
function set(source2, value, should_proxy = false) {
  if (active_reaction !== null && // since we are untracking the function inside `$inspect.with` we need to add this check
  // to ensure we error if state is set inside an inspect effect
  (!untracking || (active_reaction.f & EAGER_EFFECT) !== 0) && is_runes() && (active_reaction.f & (DERIVED | BLOCK_EFFECT | ASYNC | EAGER_EFFECT)) !== 0 && !current_sources?.includes(source2)) {
    state_unsafe_mutation();
  }
  let new_value = should_proxy ? proxy(value) : value;
  return internal_set(source2, new_value);
}
__name(set, "set");
function internal_set(source2, value) {
  if (!source2.equals(value)) {
    var old_value = source2.v;
    if (is_destroying_effect) {
      old_values.set(source2, value);
    } else {
      old_values.set(source2, old_value);
    }
    source2.v = value;
    var batch = Batch.ensure();
    batch.capture(source2, old_value);
    if ((source2.f & DERIVED) !== 0) {
      if ((source2.f & DIRTY) !== 0) {
        execute_derived(
          /** @type {Derived} */
          source2
        );
      }
      set_signal_status(source2, (source2.f & CONNECTED) !== 0 ? CLEAN : MAYBE_DIRTY);
    }
    source2.wv = increment_write_version();
    mark_reactions(source2, DIRTY);
    if (active_effect !== null && (active_effect.f & CLEAN) !== 0 && (active_effect.f & (BRANCH_EFFECT | ROOT_EFFECT)) === 0) {
      if (untracked_writes === null) {
        set_untracked_writes([source2]);
      } else {
        untracked_writes.push(source2);
      }
    }
    if (!batch.is_fork && eager_effects.size > 0 && !eager_effects_deferred) {
      flush_eager_effects();
    }
  }
  return value;
}
__name(internal_set, "internal_set");
function flush_eager_effects() {
  eager_effects_deferred = false;
  const inspects = Array.from(eager_effects);
  for (const effect of inspects) {
    if ((effect.f & CLEAN) !== 0) {
      set_signal_status(effect, MAYBE_DIRTY);
    }
    if (is_dirty(effect)) {
      update_effect(effect);
    }
  }
  eager_effects.clear();
}
__name(flush_eager_effects, "flush_eager_effects");
function increment(source2) {
  set(source2, source2.v + 1);
}
__name(increment, "increment");
function mark_reactions(signal, status) {
  var reactions = signal.reactions;
  if (reactions === null) return;
  var length = reactions.length;
  for (var i = 0; i < length; i++) {
    var reaction = reactions[i];
    var flags2 = reaction.f;
    var not_dirty = (flags2 & DIRTY) === 0;
    if (not_dirty) {
      set_signal_status(reaction, status);
    }
    if ((flags2 & DERIVED) !== 0) {
      var derived = (
        /** @type {Derived} */
        reaction
      );
      batch_values?.delete(derived);
      if ((flags2 & WAS_MARKED) === 0) {
        if (flags2 & CONNECTED) {
          reaction.f |= WAS_MARKED;
        }
        mark_reactions(derived, MAYBE_DIRTY);
      }
    } else if (not_dirty) {
      if ((flags2 & BLOCK_EFFECT) !== 0) {
        if (eager_block_effects !== null) {
          eager_block_effects.add(
            /** @type {Effect} */
            reaction
          );
        }
      }
      schedule_effect(
        /** @type {Effect} */
        reaction
      );
    }
  }
}
__name(mark_reactions, "mark_reactions");
function proxy(value) {
  if (typeof value !== "object" || value === null || STATE_SYMBOL in value) {
    return value;
  }
  const prototype = get_prototype_of(value);
  if (prototype !== object_prototype && prototype !== array_prototype) {
    return value;
  }
  var sources = /* @__PURE__ */ new Map();
  var is_proxied_array = is_array(value);
  var version2 = /* @__PURE__ */ state(0);
  var parent_version = update_version;
  var with_parent = /* @__PURE__ */ __name((fn) => {
    if (update_version === parent_version) {
      return fn();
    }
    var reaction = active_reaction;
    var version22 = update_version;
    set_active_reaction(null);
    set_update_version(parent_version);
    var result = fn();
    set_active_reaction(reaction);
    set_update_version(version22);
    return result;
  }, "with_parent");
  if (is_proxied_array) {
    sources.set("length", /* @__PURE__ */ state(
      /** @type {any[]} */
      value.length
    ));
  }
  return new Proxy(
    /** @type {any} */
    value,
    {
      defineProperty(_, prop, descriptor) {
        if (!("value" in descriptor) || descriptor.configurable === false || descriptor.enumerable === false || descriptor.writable === false) {
          state_descriptors_fixed();
        }
        var s3 = sources.get(prop);
        if (s3 === void 0) {
          s3 = with_parent(() => {
            var s22 = /* @__PURE__ */ state(descriptor.value);
            sources.set(prop, s22);
            return s22;
          });
        } else {
          set(s3, descriptor.value, true);
        }
        return true;
      },
      deleteProperty(target, prop) {
        var s3 = sources.get(prop);
        if (s3 === void 0) {
          if (prop in target) {
            const s22 = with_parent(() => /* @__PURE__ */ state(UNINITIALIZED));
            sources.set(prop, s22);
            increment(version2);
          }
        } else {
          set(s3, UNINITIALIZED);
          increment(version2);
        }
        return true;
      },
      get(target, prop, receiver) {
        if (prop === STATE_SYMBOL) {
          return value;
        }
        var s3 = sources.get(prop);
        var exists = prop in target;
        if (s3 === void 0 && (!exists || get_descriptor(target, prop)?.writable)) {
          s3 = with_parent(() => {
            var p = proxy(exists ? target[prop] : UNINITIALIZED);
            var s22 = /* @__PURE__ */ state(p);
            return s22;
          });
          sources.set(prop, s3);
        }
        if (s3 !== void 0) {
          var v = get(s3);
          return v === UNINITIALIZED ? void 0 : v;
        }
        return Reflect.get(target, prop, receiver);
      },
      getOwnPropertyDescriptor(target, prop) {
        var descriptor = Reflect.getOwnPropertyDescriptor(target, prop);
        if (descriptor && "value" in descriptor) {
          var s3 = sources.get(prop);
          if (s3) descriptor.value = get(s3);
        } else if (descriptor === void 0) {
          var source2 = sources.get(prop);
          var value2 = source2?.v;
          if (source2 !== void 0 && value2 !== UNINITIALIZED) {
            return {
              enumerable: true,
              configurable: true,
              value: value2,
              writable: true
            };
          }
        }
        return descriptor;
      },
      has(target, prop) {
        if (prop === STATE_SYMBOL) {
          return true;
        }
        var s3 = sources.get(prop);
        var has = s3 !== void 0 && s3.v !== UNINITIALIZED || Reflect.has(target, prop);
        if (s3 !== void 0 || active_effect !== null && (!has || get_descriptor(target, prop)?.writable)) {
          if (s3 === void 0) {
            s3 = with_parent(() => {
              var p = has ? proxy(target[prop]) : UNINITIALIZED;
              var s22 = /* @__PURE__ */ state(p);
              return s22;
            });
            sources.set(prop, s3);
          }
          var value2 = get(s3);
          if (value2 === UNINITIALIZED) {
            return false;
          }
        }
        return has;
      },
      set(target, prop, value2, receiver) {
        var s3 = sources.get(prop);
        var has = prop in target;
        if (is_proxied_array && prop === "length") {
          for (var i = value2; i < /** @type {Source<number>} */
          s3.v; i += 1) {
            var other_s = sources.get(i + "");
            if (other_s !== void 0) {
              set(other_s, UNINITIALIZED);
            } else if (i in target) {
              other_s = with_parent(() => /* @__PURE__ */ state(UNINITIALIZED));
              sources.set(i + "", other_s);
            }
          }
        }
        if (s3 === void 0) {
          if (!has || get_descriptor(target, prop)?.writable) {
            s3 = with_parent(() => /* @__PURE__ */ state(void 0));
            set(s3, proxy(value2));
            sources.set(prop, s3);
          }
        } else {
          has = s3.v !== UNINITIALIZED;
          var p = with_parent(() => proxy(value2));
          set(s3, p);
        }
        var descriptor = Reflect.getOwnPropertyDescriptor(target, prop);
        if (descriptor?.set) {
          descriptor.set.call(receiver, value2);
        }
        if (!has) {
          if (is_proxied_array && typeof prop === "string") {
            var ls = (
              /** @type {Source<number>} */
              sources.get("length")
            );
            var n2 = Number(prop);
            if (Number.isInteger(n2) && n2 >= ls.v) {
              set(ls, n2 + 1);
            }
          }
          increment(version2);
        }
        return true;
      },
      ownKeys(target) {
        get(version2);
        var own_keys = Reflect.ownKeys(target).filter((key22) => {
          var source3 = sources.get(key22);
          return source3 === void 0 || source3.v !== UNINITIALIZED;
        });
        for (var [key2, source2] of sources) {
          if (source2.v !== UNINITIALIZED && !(key2 in target)) {
            own_keys.push(key2);
          }
        }
        return own_keys;
      },
      setPrototypeOf() {
        state_prototype_fixed();
      }
    }
  );
}
__name(proxy, "proxy");
var $window;
var first_child_getter;
var next_sibling_getter;
function init_operations() {
  if ($window !== void 0) {
    return;
  }
  $window = window;
  var element_prototype = Element.prototype;
  var node_prototype = Node.prototype;
  var text_prototype = Text.prototype;
  first_child_getter = get_descriptor(node_prototype, "firstChild").get;
  next_sibling_getter = get_descriptor(node_prototype, "nextSibling").get;
  if (is_extensible(element_prototype)) {
    element_prototype.__click = void 0;
    element_prototype.__className = void 0;
    element_prototype.__attributes = null;
    element_prototype.__style = void 0;
    element_prototype.__e = void 0;
  }
  if (is_extensible(text_prototype)) {
    text_prototype.__t = void 0;
  }
}
__name(init_operations, "init_operations");
function create_text(value = "") {
  return document.createTextNode(value);
}
__name(create_text, "create_text");
// @__NO_SIDE_EFFECTS__
function get_first_child(node) {
  return first_child_getter.call(node);
}
__name(get_first_child, "get_first_child");
// @__NO_SIDE_EFFECTS__
function get_next_sibling(node) {
  return next_sibling_getter.call(node);
}
__name(get_next_sibling, "get_next_sibling");
function clear_text_content(node) {
  node.textContent = "";
}
__name(clear_text_content, "clear_text_content");
function without_reactive_context(fn) {
  var previous_reaction = active_reaction;
  var previous_effect = active_effect;
  set_active_reaction(null);
  set_active_effect(null);
  try {
    return fn();
  } finally {
    set_active_reaction(previous_reaction);
    set_active_effect(previous_effect);
  }
}
__name(without_reactive_context, "without_reactive_context");
function push_effect(effect, parent_effect) {
  var parent_last = parent_effect.last;
  if (parent_last === null) {
    parent_effect.last = parent_effect.first = effect;
  } else {
    parent_last.next = effect;
    effect.prev = parent_last;
    parent_effect.last = effect;
  }
}
__name(push_effect, "push_effect");
function create_effect(type, fn, sync, push22 = true) {
  var parent = active_effect;
  if (parent !== null && (parent.f & INERT) !== 0) {
    type |= INERT;
  }
  var effect = {
    ctx: component_context,
    deps: null,
    nodes_start: null,
    nodes_end: null,
    f: type | DIRTY | CONNECTED,
    first: null,
    fn,
    last: null,
    next: null,
    parent,
    b: parent && parent.b,
    prev: null,
    teardown: null,
    transitions: null,
    wv: 0,
    ac: null
  };
  if (sync) {
    try {
      update_effect(effect);
      effect.f |= EFFECT_RAN;
    } catch (e22) {
      destroy_effect(effect);
      throw e22;
    }
  } else if (fn !== null) {
    schedule_effect(effect);
  }
  if (push22) {
    var e3 = effect;
    if (sync && e3.deps === null && e3.teardown === null && e3.nodes_start === null && e3.first === e3.last && // either `null`, or a singular child
    (e3.f & EFFECT_PRESERVED) === 0) {
      e3 = e3.first;
      if ((type & BLOCK_EFFECT) !== 0 && (type & EFFECT_TRANSPARENT) !== 0 && e3 !== null) {
        e3.f |= EFFECT_TRANSPARENT;
      }
    }
    if (e3 !== null) {
      e3.parent = parent;
      if (parent !== null) {
        push_effect(e3, parent);
      }
      if (active_reaction !== null && (active_reaction.f & DERIVED) !== 0 && (type & ROOT_EFFECT) === 0) {
        var derived = (
          /** @type {Derived} */
          active_reaction
        );
        (derived.effects ??= []).push(e3);
      }
    }
  }
  return effect;
}
__name(create_effect, "create_effect");
function effect_tracking() {
  return active_reaction !== null && !untracking;
}
__name(effect_tracking, "effect_tracking");
function create_user_effect(fn) {
  return create_effect(EFFECT | USER_EFFECT, fn, false);
}
__name(create_user_effect, "create_user_effect");
function component_root(fn) {
  Batch.ensure();
  const effect = create_effect(ROOT_EFFECT | EFFECT_PRESERVED, fn, true);
  return (options2 = {}) => {
    return new Promise((fulfil) => {
      if (options2.outro) {
        pause_effect(effect, () => {
          destroy_effect(effect);
          fulfil(void 0);
        });
      } else {
        destroy_effect(effect);
        fulfil(void 0);
      }
    });
  };
}
__name(component_root, "component_root");
function render_effect(fn, flags2 = 0) {
  return create_effect(RENDER_EFFECT | flags2, fn, true);
}
__name(render_effect, "render_effect");
function block(fn, flags2 = 0) {
  var effect = create_effect(BLOCK_EFFECT | flags2, fn, true);
  return effect;
}
__name(block, "block");
function branch(fn, push22 = true) {
  return create_effect(BRANCH_EFFECT | EFFECT_PRESERVED, fn, true, push22);
}
__name(branch, "branch");
function execute_effect_teardown(effect) {
  var teardown = effect.teardown;
  if (teardown !== null) {
    const previously_destroying_effect = is_destroying_effect;
    const previous_reaction = active_reaction;
    set_is_destroying_effect(true);
    set_active_reaction(null);
    try {
      teardown.call(null);
    } finally {
      set_is_destroying_effect(previously_destroying_effect);
      set_active_reaction(previous_reaction);
    }
  }
}
__name(execute_effect_teardown, "execute_effect_teardown");
function destroy_effect_children(signal, remove_dom = false) {
  var effect = signal.first;
  signal.first = signal.last = null;
  while (effect !== null) {
    const controller2 = effect.ac;
    if (controller2 !== null) {
      without_reactive_context(() => {
        controller2.abort(STALE_REACTION);
      });
    }
    var next2 = effect.next;
    if ((effect.f & ROOT_EFFECT) !== 0) {
      effect.parent = null;
    } else {
      destroy_effect(effect, remove_dom);
    }
    effect = next2;
  }
}
__name(destroy_effect_children, "destroy_effect_children");
function destroy_block_effect_children(signal) {
  var effect = signal.first;
  while (effect !== null) {
    var next2 = effect.next;
    if ((effect.f & BRANCH_EFFECT) === 0) {
      destroy_effect(effect);
    }
    effect = next2;
  }
}
__name(destroy_block_effect_children, "destroy_block_effect_children");
function destroy_effect(effect, remove_dom = true) {
  var removed = false;
  if ((remove_dom || (effect.f & HEAD_EFFECT) !== 0) && effect.nodes_start !== null && effect.nodes_end !== null) {
    remove_effect_dom(
      effect.nodes_start,
      /** @type {TemplateNode} */
      effect.nodes_end
    );
    removed = true;
  }
  destroy_effect_children(effect, remove_dom && !removed);
  remove_reactions(effect, 0);
  set_signal_status(effect, DESTROYED);
  var transitions = effect.transitions;
  if (transitions !== null) {
    for (const transition of transitions) {
      transition.stop();
    }
  }
  execute_effect_teardown(effect);
  var parent = effect.parent;
  if (parent !== null && parent.first !== null) {
    unlink_effect(effect);
  }
  effect.next = effect.prev = effect.teardown = effect.ctx = effect.deps = effect.fn = effect.nodes_start = effect.nodes_end = effect.ac = null;
}
__name(destroy_effect, "destroy_effect");
function remove_effect_dom(node, end) {
  while (node !== null) {
    var next2 = node === end ? null : (
      /** @type {TemplateNode} */
      /* @__PURE__ */ get_next_sibling(node)
    );
    node.remove();
    node = next2;
  }
}
__name(remove_effect_dom, "remove_effect_dom");
function unlink_effect(effect) {
  var parent = effect.parent;
  var prev = effect.prev;
  var next2 = effect.next;
  if (prev !== null) prev.next = next2;
  if (next2 !== null) next2.prev = prev;
  if (parent !== null) {
    if (parent.first === effect) parent.first = next2;
    if (parent.last === effect) parent.last = prev;
  }
}
__name(unlink_effect, "unlink_effect");
function pause_effect(effect, callback, destroy = true) {
  var transitions = [];
  pause_children(effect, transitions, true);
  run_out_transitions(transitions, () => {
    if (destroy) destroy_effect(effect);
    if (callback) callback();
  });
}
__name(pause_effect, "pause_effect");
function run_out_transitions(transitions, fn) {
  var remaining = transitions.length;
  if (remaining > 0) {
    var check = /* @__PURE__ */ __name(() => --remaining || fn(), "check");
    for (var transition of transitions) {
      transition.out(check);
    }
  } else {
    fn();
  }
}
__name(run_out_transitions, "run_out_transitions");
function pause_children(effect, transitions, local) {
  if ((effect.f & INERT) !== 0) return;
  effect.f ^= INERT;
  if (effect.transitions !== null) {
    for (const transition of effect.transitions) {
      if (transition.is_global || local) {
        transitions.push(transition);
      }
    }
  }
  var child = effect.first;
  while (child !== null) {
    var sibling = child.next;
    var transparent = (child.f & EFFECT_TRANSPARENT) !== 0 || // If this is a branch effect without a block effect parent,
    // it means the parent block effect was pruned. In that case,
    // transparency information was transferred to the branch effect.
    (child.f & BRANCH_EFFECT) !== 0 && (effect.f & BLOCK_EFFECT) !== 0;
    pause_children(child, transitions, transparent ? local : false);
    child = sibling;
  }
}
__name(pause_children, "pause_children");
function move_effect(effect, fragment) {
  var node = effect.nodes_start;
  var end = effect.nodes_end;
  while (node !== null) {
    var next2 = node === end ? null : (
      /** @type {TemplateNode} */
      /* @__PURE__ */ get_next_sibling(node)
    );
    fragment.append(node);
    node = next2;
  }
}
__name(move_effect, "move_effect");
var is_updating_effect = false;
function set_is_updating_effect(value) {
  is_updating_effect = value;
}
__name(set_is_updating_effect, "set_is_updating_effect");
var is_destroying_effect = false;
function set_is_destroying_effect(value) {
  is_destroying_effect = value;
}
__name(set_is_destroying_effect, "set_is_destroying_effect");
var active_reaction = null;
var untracking = false;
function set_active_reaction(reaction) {
  active_reaction = reaction;
}
__name(set_active_reaction, "set_active_reaction");
var active_effect = null;
function set_active_effect(effect) {
  active_effect = effect;
}
__name(set_active_effect, "set_active_effect");
var current_sources = null;
function push_reaction_value(value) {
  if (active_reaction !== null && true) {
    if (current_sources === null) {
      current_sources = [value];
    } else {
      current_sources.push(value);
    }
  }
}
__name(push_reaction_value, "push_reaction_value");
var new_deps = null;
var skipped_deps = 0;
var untracked_writes = null;
function set_untracked_writes(value) {
  untracked_writes = value;
}
__name(set_untracked_writes, "set_untracked_writes");
var write_version = 1;
var read_version = 0;
var update_version = read_version;
function set_update_version(value) {
  update_version = value;
}
__name(set_update_version, "set_update_version");
function increment_write_version() {
  return ++write_version;
}
__name(increment_write_version, "increment_write_version");
function is_dirty(reaction) {
  var flags2 = reaction.f;
  if ((flags2 & DIRTY) !== 0) {
    return true;
  }
  if (flags2 & DERIVED) {
    reaction.f &= ~WAS_MARKED;
  }
  if ((flags2 & MAYBE_DIRTY) !== 0) {
    var dependencies = reaction.deps;
    if (dependencies !== null) {
      var length = dependencies.length;
      for (var i = 0; i < length; i++) {
        var dependency = dependencies[i];
        if (is_dirty(
          /** @type {Derived} */
          dependency
        )) {
          update_derived(
            /** @type {Derived} */
            dependency
          );
        }
        if (dependency.wv > reaction.wv) {
          return true;
        }
      }
    }
    if ((flags2 & CONNECTED) !== 0 && // During time traveling we don't want to reset the status so that
    // traversal of the graph in the other batches still happens
    batch_values === null) {
      set_signal_status(reaction, CLEAN);
    }
  }
  return false;
}
__name(is_dirty, "is_dirty");
function schedule_possible_effect_self_invalidation(signal, effect, root2 = true) {
  var reactions = signal.reactions;
  if (reactions === null) return;
  if (current_sources?.includes(signal)) {
    return;
  }
  for (var i = 0; i < reactions.length; i++) {
    var reaction = reactions[i];
    if ((reaction.f & DERIVED) !== 0) {
      schedule_possible_effect_self_invalidation(
        /** @type {Derived} */
        reaction,
        effect,
        false
      );
    } else if (effect === reaction) {
      if (root2) {
        set_signal_status(reaction, DIRTY);
      } else if ((reaction.f & CLEAN) !== 0) {
        set_signal_status(reaction, MAYBE_DIRTY);
      }
      schedule_effect(
        /** @type {Effect} */
        reaction
      );
    }
  }
}
__name(schedule_possible_effect_self_invalidation, "schedule_possible_effect_self_invalidation");
function update_reaction(reaction) {
  var previous_deps = new_deps;
  var previous_skipped_deps = skipped_deps;
  var previous_untracked_writes = untracked_writes;
  var previous_reaction = active_reaction;
  var previous_sources = current_sources;
  var previous_component_context = component_context;
  var previous_untracking = untracking;
  var previous_update_version = update_version;
  var flags2 = reaction.f;
  new_deps = /** @type {null | Value[]} */
  null;
  skipped_deps = 0;
  untracked_writes = null;
  active_reaction = (flags2 & (BRANCH_EFFECT | ROOT_EFFECT)) === 0 ? reaction : null;
  current_sources = null;
  set_component_context(reaction.ctx);
  untracking = false;
  update_version = ++read_version;
  if (reaction.ac !== null) {
    without_reactive_context(() => {
      reaction.ac.abort(STALE_REACTION);
    });
    reaction.ac = null;
  }
  try {
    reaction.f |= REACTION_IS_UPDATING;
    var fn = (
      /** @type {Function} */
      reaction.fn
    );
    var result = fn();
    var deps = reaction.deps;
    if (new_deps !== null) {
      var i;
      remove_reactions(reaction, skipped_deps);
      if (deps !== null && skipped_deps > 0) {
        deps.length = skipped_deps + new_deps.length;
        for (i = 0; i < new_deps.length; i++) {
          deps[skipped_deps + i] = new_deps[i];
        }
      } else {
        reaction.deps = deps = new_deps;
      }
      if (is_updating_effect && effect_tracking() && (reaction.f & CONNECTED) !== 0) {
        for (i = skipped_deps; i < deps.length; i++) {
          (deps[i].reactions ??= []).push(reaction);
        }
      }
    } else if (deps !== null && skipped_deps < deps.length) {
      remove_reactions(reaction, skipped_deps);
      deps.length = skipped_deps;
    }
    if (is_runes() && untracked_writes !== null && !untracking && deps !== null && (reaction.f & (DERIVED | MAYBE_DIRTY | DIRTY)) === 0) {
      for (i = 0; i < /** @type {Source[]} */
      untracked_writes.length; i++) {
        schedule_possible_effect_self_invalidation(
          untracked_writes[i],
          /** @type {Effect} */
          reaction
        );
      }
    }
    if (previous_reaction !== null && previous_reaction !== reaction) {
      read_version++;
      if (untracked_writes !== null) {
        if (previous_untracked_writes === null) {
          previous_untracked_writes = untracked_writes;
        } else {
          previous_untracked_writes.push(.../** @type {Source[]} */
          untracked_writes);
        }
      }
    }
    if ((reaction.f & ERROR_VALUE) !== 0) {
      reaction.f ^= ERROR_VALUE;
    }
    return result;
  } catch (error4) {
    return handle_error(error4);
  } finally {
    reaction.f ^= REACTION_IS_UPDATING;
    new_deps = previous_deps;
    skipped_deps = previous_skipped_deps;
    untracked_writes = previous_untracked_writes;
    active_reaction = previous_reaction;
    current_sources = previous_sources;
    set_component_context(previous_component_context);
    untracking = previous_untracking;
    update_version = previous_update_version;
  }
}
__name(update_reaction, "update_reaction");
function remove_reaction(signal, dependency) {
  let reactions = dependency.reactions;
  if (reactions !== null) {
    var index5 = index_of.call(reactions, signal);
    if (index5 !== -1) {
      var new_length = reactions.length - 1;
      if (new_length === 0) {
        reactions = dependency.reactions = null;
      } else {
        reactions[index5] = reactions[new_length];
        reactions.pop();
      }
    }
  }
  if (reactions === null && (dependency.f & DERIVED) !== 0 && // Destroying a child effect while updating a parent effect can cause a dependency to appear
  // to be unused, when in fact it is used by the currently-updating parent. Checking `new_deps`
  // allows us to skip the expensive work of disconnecting and immediately reconnecting it
  (new_deps === null || !new_deps.includes(dependency))) {
    set_signal_status(dependency, MAYBE_DIRTY);
    if ((dependency.f & CONNECTED) !== 0) {
      dependency.f ^= CONNECTED;
      dependency.f &= ~WAS_MARKED;
    }
    destroy_derived_effects(
      /** @type {Derived} **/
      dependency
    );
    remove_reactions(
      /** @type {Derived} **/
      dependency,
      0
    );
  }
}
__name(remove_reaction, "remove_reaction");
function remove_reactions(signal, start_index) {
  var dependencies = signal.deps;
  if (dependencies === null) return;
  for (var i = start_index; i < dependencies.length; i++) {
    remove_reaction(signal, dependencies[i]);
  }
}
__name(remove_reactions, "remove_reactions");
function update_effect(effect) {
  var flags2 = effect.f;
  if ((flags2 & DESTROYED) !== 0) {
    return;
  }
  set_signal_status(effect, CLEAN);
  var previous_effect = active_effect;
  var was_updating_effect = is_updating_effect;
  active_effect = effect;
  is_updating_effect = true;
  try {
    if ((flags2 & BLOCK_EFFECT) !== 0) {
      destroy_block_effect_children(effect);
    } else {
      destroy_effect_children(effect);
    }
    execute_effect_teardown(effect);
    var teardown = update_reaction(effect);
    effect.teardown = typeof teardown === "function" ? teardown : null;
    effect.wv = write_version;
    var dep;
    if (BROWSER && tracing_mode_flag && (effect.f & DIRTY) !== 0 && effect.deps !== null) ;
  } finally {
    is_updating_effect = was_updating_effect;
    active_effect = previous_effect;
  }
}
__name(update_effect, "update_effect");
function get(signal) {
  var flags2 = signal.f;
  var is_derived = (flags2 & DERIVED) !== 0;
  if (active_reaction !== null && !untracking) {
    var destroyed = active_effect !== null && (active_effect.f & DESTROYED) !== 0;
    if (!destroyed && !current_sources?.includes(signal)) {
      var deps = active_reaction.deps;
      if ((active_reaction.f & REACTION_IS_UPDATING) !== 0) {
        if (signal.rv < read_version) {
          signal.rv = read_version;
          if (new_deps === null && deps !== null && deps[skipped_deps] === signal) {
            skipped_deps++;
          } else if (new_deps === null) {
            new_deps = [signal];
          } else if (!new_deps.includes(signal)) {
            new_deps.push(signal);
          }
        }
      } else {
        (active_reaction.deps ??= []).push(signal);
        var reactions = signal.reactions;
        if (reactions === null) {
          signal.reactions = [active_reaction];
        } else if (!reactions.includes(active_reaction)) {
          reactions.push(active_reaction);
        }
      }
    }
  }
  if (is_destroying_effect) {
    if (old_values.has(signal)) {
      return old_values.get(signal);
    }
    if (is_derived) {
      var derived = (
        /** @type {Derived} */
        signal
      );
      var value = derived.v;
      if ((derived.f & CLEAN) === 0 && derived.reactions !== null || depends_on_old_values(derived)) {
        value = execute_derived(derived);
      }
      old_values.set(derived, value);
      return value;
    }
  } else if (is_derived) {
    derived = /** @type {Derived} */
    signal;
    if (batch_values?.has(derived)) {
      return batch_values.get(derived);
    }
    if (is_dirty(derived)) {
      update_derived(derived);
    }
    if (is_updating_effect && effect_tracking() && (derived.f & CONNECTED) === 0) {
      reconnect(derived);
    }
  } else if (batch_values?.has(signal)) {
    return batch_values.get(signal);
  }
  if ((signal.f & ERROR_VALUE) !== 0) {
    throw signal.v;
  }
  return signal.v;
}
__name(get, "get");
function reconnect(derived) {
  if (derived.deps === null) return;
  derived.f ^= CONNECTED;
  for (const dep of derived.deps) {
    (dep.reactions ??= []).push(derived);
    if ((dep.f & DERIVED) !== 0 && (dep.f & CONNECTED) === 0) {
      reconnect(
        /** @type {Derived} */
        dep
      );
    }
  }
}
__name(reconnect, "reconnect");
function depends_on_old_values(derived) {
  if (derived.v === UNINITIALIZED) return true;
  if (derived.deps === null) return false;
  for (const dep of derived.deps) {
    if (old_values.has(dep)) {
      return true;
    }
    if ((dep.f & DERIVED) !== 0 && depends_on_old_values(
      /** @type {Derived} */
      dep
    )) {
      return true;
    }
  }
  return false;
}
__name(depends_on_old_values, "depends_on_old_values");
function untrack(fn) {
  var previous_untracking = untracking;
  try {
    untracking = true;
    return fn();
  } finally {
    untracking = previous_untracking;
  }
}
__name(untrack, "untrack");
var STATUS_MASK = -7169;
function set_signal_status(signal, status) {
  signal.f = signal.f & STATUS_MASK | status;
}
__name(set_signal_status, "set_signal_status");
var all_registered_events = /* @__PURE__ */ new Set();
var root_event_handles = /* @__PURE__ */ new Set();
var last_propagated_event = null;
function handle_event_propagation(event) {
  var handler_element = this;
  var owner_document = (
    /** @type {Node} */
    handler_element.ownerDocument
  );
  var event_name = event.type;
  var path = event.composedPath?.() || [];
  var current_target = (
    /** @type {null | Element} */
    path[0] || event.target
  );
  last_propagated_event = event;
  var path_idx = 0;
  var handled_at = last_propagated_event === event && event.__root;
  if (handled_at) {
    var at_idx = path.indexOf(handled_at);
    if (at_idx !== -1 && (handler_element === document || handler_element === /** @type {any} */
    window)) {
      event.__root = handler_element;
      return;
    }
    var handler_idx = path.indexOf(handler_element);
    if (handler_idx === -1) {
      return;
    }
    if (at_idx <= handler_idx) {
      path_idx = at_idx;
    }
  }
  current_target = /** @type {Element} */
  path[path_idx] || event.target;
  if (current_target === handler_element) return;
  define_property(event, "currentTarget", {
    configurable: true,
    get() {
      return current_target || owner_document;
    }
  });
  var previous_reaction = active_reaction;
  var previous_effect = active_effect;
  set_active_reaction(null);
  set_active_effect(null);
  try {
    var throw_error;
    var other_errors = [];
    while (current_target !== null) {
      var parent_element = current_target.assignedSlot || current_target.parentNode || /** @type {any} */
      current_target.host || null;
      try {
        var delegated = current_target["__" + event_name];
        if (delegated != null && (!/** @type {any} */
        current_target.disabled || // DOM could've been updated already by the time this is reached, so we check this as well
        // -> the target could not have been disabled because it emits the event in the first place
        event.target === current_target)) {
          delegated.call(current_target, event);
        }
      } catch (error4) {
        if (throw_error) {
          other_errors.push(error4);
        } else {
          throw_error = error4;
        }
      }
      if (event.cancelBubble || parent_element === handler_element || parent_element === null) {
        break;
      }
      current_target = parent_element;
    }
    if (throw_error) {
      for (let error4 of other_errors) {
        queueMicrotask(() => {
          throw error4;
        });
      }
      throw throw_error;
    }
  } finally {
    event.__root = handler_element;
    delete event.currentTarget;
    set_active_reaction(previous_reaction);
    set_active_effect(previous_effect);
  }
}
__name(handle_event_propagation, "handle_event_propagation");
function assign_nodes(start, end) {
  var effect = (
    /** @type {Effect} */
    active_effect
  );
  if (effect.nodes_start === null) {
    effect.nodes_start = start;
    effect.nodes_end = end;
  }
}
__name(assign_nodes, "assign_nodes");
function mount(component5, options2) {
  return _mount(component5, options2);
}
__name(mount, "mount");
function hydrate(component5, options2) {
  init_operations();
  options2.intro = options2.intro ?? false;
  const target = options2.target;
  const was_hydrating = hydrating;
  const previous_hydrate_node = hydrate_node;
  try {
    var anchor = (
      /** @type {TemplateNode} */
      /* @__PURE__ */ get_first_child(target)
    );
    while (anchor && (anchor.nodeType !== COMMENT_NODE || /** @type {Comment} */
    anchor.data !== HYDRATION_START)) {
      anchor = /** @type {TemplateNode} */
      /* @__PURE__ */ get_next_sibling(anchor);
    }
    if (!anchor) {
      throw HYDRATION_ERROR;
    }
    set_hydrating(true);
    set_hydrate_node(
      /** @type {Comment} */
      anchor
    );
    const instance = _mount(component5, { ...options2, anchor });
    set_hydrating(false);
    return (
      /**  @type {Exports} */
      instance
    );
  } catch (error4) {
    if (error4 instanceof Error && error4.message.split("\n").some((line) => line.startsWith("https://svelte.dev/e/"))) {
      throw error4;
    }
    if (error4 !== HYDRATION_ERROR) {
      console.warn("Failed to hydrate: ", error4);
    }
    if (options2.recover === false) {
      hydration_failed();
    }
    init_operations();
    clear_text_content(target);
    set_hydrating(false);
    return mount(component5, options2);
  } finally {
    set_hydrating(was_hydrating);
    set_hydrate_node(previous_hydrate_node);
  }
}
__name(hydrate, "hydrate");
var document_listeners = /* @__PURE__ */ new Map();
function _mount(Component, { target, anchor, props = {}, events, context: context3, intro = true }) {
  init_operations();
  var registered_events = /* @__PURE__ */ new Set();
  var event_handle = /* @__PURE__ */ __name((events2) => {
    for (var i = 0; i < events2.length; i++) {
      var event_name = events2[i];
      if (registered_events.has(event_name)) continue;
      registered_events.add(event_name);
      var passive = is_passive_event(event_name);
      target.addEventListener(event_name, handle_event_propagation, { passive });
      var n2 = document_listeners.get(event_name);
      if (n2 === void 0) {
        document.addEventListener(event_name, handle_event_propagation, { passive });
        document_listeners.set(event_name, 1);
      } else {
        document_listeners.set(event_name, n2 + 1);
      }
    }
  }, "event_handle");
  event_handle(array_from(all_registered_events));
  root_event_handles.add(event_handle);
  var component5 = void 0;
  var unmount2 = component_root(() => {
    var anchor_node = anchor ?? target.appendChild(create_text());
    boundary(
      /** @type {TemplateNode} */
      anchor_node,
      {
        pending: /* @__PURE__ */ __name(() => {
        }, "pending")
      },
      (anchor_node2) => {
        if (context3) {
          push2({});
          var ctx = (
            /** @type {ComponentContext} */
            component_context
          );
          ctx.c = context3;
        }
        if (events) {
          props.$$events = events;
        }
        if (hydrating) {
          assign_nodes(
            /** @type {TemplateNode} */
            anchor_node2,
            null
          );
        }
        component5 = Component(anchor_node2, props) || {};
        if (hydrating) {
          active_effect.nodes_end = hydrate_node;
          if (hydrate_node === null || hydrate_node.nodeType !== COMMENT_NODE || /** @type {Comment} */
          hydrate_node.data !== HYDRATION_END) {
            hydration_mismatch();
            throw HYDRATION_ERROR;
          }
        }
        if (context3) {
          pop2();
        }
      }
    );
    return () => {
      for (var event_name of registered_events) {
        target.removeEventListener(event_name, handle_event_propagation);
        var n2 = (
          /** @type {number} */
          document_listeners.get(event_name)
        );
        if (--n2 === 0) {
          document.removeEventListener(event_name, handle_event_propagation);
          document_listeners.delete(event_name);
        } else {
          document_listeners.set(event_name, n2);
        }
      }
      root_event_handles.delete(event_handle);
      if (anchor_node !== anchor) {
        anchor_node.parentNode?.removeChild(anchor_node);
      }
    };
  });
  mounted_components.set(component5, unmount2);
  return component5;
}
__name(_mount, "_mount");
var mounted_components = /* @__PURE__ */ new WeakMap();
function unmount(component5, options2) {
  const fn = mounted_components.get(component5);
  if (fn) {
    mounted_components.delete(component5);
    return fn(options2);
  }
  return Promise.resolve();
}
__name(unmount, "unmount");
function asClassComponent$1(component5) {
  return class extends Svelte4Component {
    /** @param {any} options */
    constructor(options2) {
      super({
        component: component5,
        ...options2
      });
    }
  };
}
__name(asClassComponent$1, "asClassComponent$1");
var Svelte4Component = class {
  static {
    __name(this, "Svelte4Component");
  }
  /** @type {any} */
  #events;
  /** @type {Record<string, any>} */
  #instance;
  /**
   * @param {ComponentConstructorOptions & {
   *  component: any;
   * }} options
   */
  constructor(options2) {
    var sources = /* @__PURE__ */ new Map();
    var add_source = /* @__PURE__ */ __name((key2, value) => {
      var s3 = /* @__PURE__ */ mutable_source(value, false, false);
      sources.set(key2, s3);
      return s3;
    }, "add_source");
    const props = new Proxy(
      { ...options2.props || {}, $$events: {} },
      {
        get(target, prop) {
          return get(sources.get(prop) ?? add_source(prop, Reflect.get(target, prop)));
        },
        has(target, prop) {
          if (prop === LEGACY_PROPS) return true;
          get(sources.get(prop) ?? add_source(prop, Reflect.get(target, prop)));
          return Reflect.has(target, prop);
        },
        set(target, prop, value) {
          set(sources.get(prop) ?? add_source(prop, value), value);
          return Reflect.set(target, prop, value);
        }
      }
    );
    this.#instance = (options2.hydrate ? hydrate : mount)(options2.component, {
      target: options2.target,
      anchor: options2.anchor,
      props,
      context: options2.context,
      intro: options2.intro ?? false,
      recover: options2.recover
    });
    if (!options2?.props?.$$host || options2.sync === false) {
      flushSync();
    }
    this.#events = props.$$events;
    for (const key2 of Object.keys(this.#instance)) {
      if (key2 === "$set" || key2 === "$destroy" || key2 === "$on") continue;
      define_property(this, key2, {
        get() {
          return this.#instance[key2];
        },
        /** @param {any} value */
        set(value) {
          this.#instance[key2] = value;
        },
        enumerable: true
      });
    }
    this.#instance.$set = /** @param {Record<string, any>} next */
    (next2) => {
      Object.assign(props, next2);
    };
    this.#instance.$destroy = () => {
      unmount(this.#instance);
    };
  }
  /** @param {Record<string, any>} props */
  $set(props) {
    this.#instance.$set(props);
  }
  /**
   * @param {string} event
   * @param {(...args: any[]) => any} callback
   * @returns {any}
   */
  $on(event, callback) {
    this.#events[event] = this.#events[event] || [];
    const cb = /* @__PURE__ */ __name((...args) => callback.call(this, ...args), "cb");
    this.#events[event].push(cb);
    return () => {
      this.#events[event] = this.#events[event].filter(
        /** @param {any} fn */
        (fn) => fn !== cb
      );
    };
  }
  $destroy() {
    this.#instance.$destroy();
  }
};
var read_implementation = null;
function set_read_implementation(fn) {
  read_implementation = fn;
}
__name(set_read_implementation, "set_read_implementation");
function asClassComponent(component5) {
  const component_constructor = asClassComponent$1(component5);
  const _render = /* @__PURE__ */ __name((props, { context: context3 } = {}) => {
    const result = render(component5, { props, context: context3 });
    const munged = Object.defineProperties(
      /** @type {LegacyRenderResult & PromiseLike<LegacyRenderResult>} */
      {},
      {
        css: {
          value: { code: "", map: null }
        },
        head: {
          get: /* @__PURE__ */ __name(() => result.head, "get")
        },
        html: {
          get: /* @__PURE__ */ __name(() => result.body, "get")
        },
        then: {
          /**
           * this is not type-safe, but honestly it's the best I can do right now, and it's a straightforward function.
           *
           * @template TResult1
           * @template [TResult2=never]
           * @param { (value: LegacyRenderResult) => TResult1 } onfulfilled
           * @param { (reason: unknown) => TResult2 } onrejected
           */
          value: /* @__PURE__ */ __name((onfulfilled, onrejected) => {
            {
              const user_result = onfulfilled({
                css: munged.css,
                head: munged.head,
                html: munged.html
              });
              return Promise.resolve(user_result);
            }
          }, "value")
        }
      }
    );
    return munged;
  }, "_render");
  component_constructor.render = _render;
  return component_constructor;
}
__name(asClassComponent, "asClassComponent");
function Root($$renderer, $$props) {
  $$renderer.component(($$renderer2) => {
    let {
      stores: stores2,
      page: page2,
      constructors,
      components = [],
      form,
      data_0 = null,
      data_1 = null
    } = $$props;
    {
      setContext("__svelte__", stores2);
    }
    {
      stores2.page.set(page2);
    }
    const Pyramid_1 = constructors[1];
    if (constructors[1]) {
      $$renderer2.push("<!--[-->");
      const Pyramid_0 = constructors[0];
      $$renderer2.push(`<!---->`);
      Pyramid_0($$renderer2, {
        data: data_0,
        form,
        params: page2.params,
        children: /* @__PURE__ */ __name(($$renderer3) => {
          $$renderer3.push(`<!---->`);
          Pyramid_1($$renderer3, { data: data_1, form, params: page2.params });
          $$renderer3.push(`<!---->`);
        }, "children"),
        $$slots: { default: true }
      });
      $$renderer2.push(`<!---->`);
    } else {
      $$renderer2.push("<!--[!-->");
      const Pyramid_0 = constructors[0];
      $$renderer2.push(`<!---->`);
      Pyramid_0($$renderer2, { data: data_0, form, params: page2.params });
      $$renderer2.push(`<!---->`);
    }
    $$renderer2.push(`<!--]--> `);
    {
      $$renderer2.push("<!--[!-->");
    }
    $$renderer2.push(`<!--]-->`);
  });
}
__name(Root, "Root");
var root = asClassComponent(Root);
var options = {
  app_template_contains_nonce: false,
  async: false,
  csp: { "mode": "auto", "directives": { "upgrade-insecure-requests": false, "block-all-mixed-content": false }, "reportOnly": { "upgrade-insecure-requests": false, "block-all-mixed-content": false } },
  csrf_check_origin: true,
  csrf_trusted_origins: [],
  embedded: false,
  env_public_prefix: "PUBLIC_",
  env_private_prefix: "",
  hash_routing: false,
  hooks: null,
  // added lazily, via `get_hooks`
  preload_strategy: "modulepreload",
  root,
  service_worker: false,
  service_worker_options: void 0,
  templates: {
    app: /* @__PURE__ */ __name(({ head: head2, body: body2, assets: assets2, nonce, env: env3 }) => '<!doctype html>\r\n<html lang="en">\r\n	<head>\r\n		<meta charset="utf-8" />\r\n		<meta name="viewport" content="width=device-width, initial-scale=1.0" />\r\n		' + head2 + '\r\n	</head>\r\n	<body data-sveltekit-preload-data="hover">\r\n		<div style="display: contents">' + body2 + "</div>\r\n	</body>\r\n</html>\r\n\r\n", "app"),
    error: /* @__PURE__ */ __name(({ status, message }) => '<!doctype html>\n<html lang="en">\n	<head>\n		<meta charset="utf-8" />\n		<title>' + message + `</title>

		<style>
			body {
				--bg: white;
				--fg: #222;
				--divider: #ccc;
				background: var(--bg);
				color: var(--fg);
				font-family:
					system-ui,
					-apple-system,
					BlinkMacSystemFont,
					'Segoe UI',
					Roboto,
					Oxygen,
					Ubuntu,
					Cantarell,
					'Open Sans',
					'Helvetica Neue',
					sans-serif;
				display: flex;
				align-items: center;
				justify-content: center;
				height: 100vh;
				margin: 0;
			}

			.error {
				display: flex;
				align-items: center;
				max-width: 32rem;
				margin: 0 1rem;
			}

			.status {
				font-weight: 200;
				font-size: 3rem;
				line-height: 1;
				position: relative;
				top: -0.05rem;
			}

			.message {
				border-left: 1px solid var(--divider);
				padding: 0 0 0 1rem;
				margin: 0 0 0 1rem;
				min-height: 2.5rem;
				display: flex;
				align-items: center;
			}

			.message h1 {
				font-weight: 400;
				font-size: 1em;
				margin: 0;
			}

			@media (prefers-color-scheme: dark) {
				body {
					--bg: #222;
					--fg: #ddd;
					--divider: #666;
				}
			}
		</style>
	</head>
	<body>
		<div class="error">
			<span class="status">` + status + '</span>\n			<div class="message">\n				<h1>' + message + "</h1>\n			</div>\n		</div>\n	</body>\n</html>\n", "error")
  },
  version_hash: "1jgtt99"
};
async function get_hooks() {
  let handle;
  let handleFetch;
  let handleError;
  let handleValidationError;
  let init2;
  let reroute;
  let transport;
  return {
    handle,
    handleFetch,
    handleError,
    handleValidationError,
    init: init2,
    reroute,
    transport
  };
}
__name(get_hooks, "get_hooks");

// .svelte-kit/output/server/chunks/shared.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
init_utils3();
var INVALIDATED_PARAM = "x-sveltekit-invalidated";
var TRAILING_SLASH_PARAM = "x-sveltekit-trailing-slash";
function stringify2(data, transport) {
  const encoders = Object.fromEntries(Object.entries(transport).map(([k, v]) => [k, v.encode]));
  return stringify(data, encoders);
}
__name(stringify2, "stringify");
function parse_remote_arg(string, transport) {
  if (!string) return void 0;
  const json_string = text_decoder2.decode(
    // no need to add back `=` characters, atob can handle it
    base64_decode(string.replaceAll("-", "+").replaceAll("_", "/"))
  );
  const decoders = Object.fromEntries(Object.entries(transport).map(([k, v]) => [k, v.decode]));
  return parse(json_string, decoders);
}
__name(parse_remote_arg, "parse_remote_arg");
function create_remote_cache_key(id, payload) {
  return id + "/" + payload;
}
__name(create_remote_cache_key, "create_remote_cache_key");

// .svelte-kit/output/server/index.js
var import_cookie = __toESM(require_cookie(), 1);
var set_cookie_parser = __toESM(require_set_cookie(), 1);
function with_resolvers() {
  let resolve2;
  let reject;
  const promise = new Promise((res, rej) => {
    resolve2 = res;
    reject = rej;
  });
  return { promise, resolve: resolve2, reject };
}
__name(with_resolvers, "with_resolvers");
var NULL_BODY_STATUS = [101, 103, 204, 205, 304];
var IN_WEBCONTAINER2 = !!globalThis.process?.versions?.webcontainer;
var SVELTE_KIT_ASSETS = "/_svelte_kit_assets";
var ENDPOINT_METHODS = ["GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS", "HEAD"];
var PAGE_METHODS = ["GET", "POST", "HEAD"];
function negotiate(accept, types) {
  const parts = [];
  accept.split(",").forEach((str, i) => {
    const match = /([^/ \t]+)\/([^; \t]+)[ \t]*(?:;[ \t]*q=([0-9.]+))?/.exec(str);
    if (match) {
      const [, type, subtype, q = "1"] = match;
      parts.push({ type, subtype, q: +q, i });
    }
  });
  parts.sort((a, b) => {
    if (a.q !== b.q) {
      return b.q - a.q;
    }
    if (a.subtype === "*" !== (b.subtype === "*")) {
      return a.subtype === "*" ? 1 : -1;
    }
    if (a.type === "*" !== (b.type === "*")) {
      return a.type === "*" ? 1 : -1;
    }
    return a.i - b.i;
  });
  let accepted;
  let min_priority = Infinity;
  for (const mimetype of types) {
    const [type, subtype] = mimetype.split("/");
    const priority = parts.findIndex(
      (part) => (part.type === type || part.type === "*") && (part.subtype === subtype || part.subtype === "*")
    );
    if (priority !== -1 && priority < min_priority) {
      accepted = mimetype;
      min_priority = priority;
    }
  }
  return accepted;
}
__name(negotiate, "negotiate");
function is_content_type(request, ...types) {
  const type = request.headers.get("content-type")?.split(";", 1)[0].trim() ?? "";
  return types.includes(type.toLowerCase());
}
__name(is_content_type, "is_content_type");
function is_form_content_type(request) {
  return is_content_type(
    request,
    "application/x-www-form-urlencoded",
    "multipart/form-data",
    "text/plain"
  );
}
__name(is_form_content_type, "is_form_content_type");
function coalesce_to_error(err) {
  return err instanceof Error || err && /** @type {any} */
  err.name && /** @type {any} */
  err.message ? (
    /** @type {Error} */
    err
  ) : new Error(JSON.stringify(err));
}
__name(coalesce_to_error, "coalesce_to_error");
function normalize_error(error22) {
  return (
    /** @type {import('../exports/internal/index.js').Redirect | HttpError | SvelteKitError | Error} */
    error22
  );
}
__name(normalize_error, "normalize_error");
function get_status(error22) {
  return error22 instanceof HttpError || error22 instanceof SvelteKitError ? error22.status : 500;
}
__name(get_status, "get_status");
function get_message(error22) {
  return error22 instanceof SvelteKitError ? error22.text : "Internal Error";
}
__name(get_message, "get_message");
var escape_html_attr_dict = {
  "&": "&amp;",
  '"': "&quot;"
  // Svelte also escapes < because the escape function could be called inside a `noscript` there
  // https://github.com/sveltejs/svelte/security/advisories/GHSA-8266-84wp-wv5c
  // However, that doesn't apply in SvelteKit
};
var escape_html_dict = {
  "&": "&amp;",
  "<": "&lt;"
};
var surrogates = (
  // high surrogate without paired low surrogate
  "[\\ud800-\\udbff](?![\\udc00-\\udfff])|[\\ud800-\\udbff][\\udc00-\\udfff]|[\\udc00-\\udfff]"
);
var escape_html_attr_regex = new RegExp(
  `[${Object.keys(escape_html_attr_dict).join("")}]|` + surrogates,
  "g"
);
var escape_html_regex = new RegExp(
  `[${Object.keys(escape_html_dict).join("")}]|` + surrogates,
  "g"
);
function escape_html2(str, is_attr) {
  const dict = is_attr ? escape_html_attr_dict : escape_html_dict;
  const escaped_str = str.replace(is_attr ? escape_html_attr_regex : escape_html_regex, (match) => {
    if (match.length === 2) {
      return match;
    }
    return dict[match] ?? `&#${match.charCodeAt(0)};`;
  });
  return escaped_str;
}
__name(escape_html2, "escape_html");
function method_not_allowed(mod, method) {
  return text(`${method} method not allowed`, {
    status: 405,
    headers: {
      // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/405
      // "The server must generate an Allow header field in a 405 status code response"
      allow: allowed_methods(mod).join(", ")
    }
  });
}
__name(method_not_allowed, "method_not_allowed");
function allowed_methods(mod) {
  const allowed = ENDPOINT_METHODS.filter((method) => method in mod);
  if ("GET" in mod && !("HEAD" in mod)) {
    allowed.push("HEAD");
  }
  return allowed;
}
__name(allowed_methods, "allowed_methods");
function get_global_name(options2) {
  return `__sveltekit_${options2.version_hash}`;
}
__name(get_global_name, "get_global_name");
function static_error_page(options2, status, message) {
  let page2 = options2.templates.error({ status, message: escape_html2(message) });
  return text(page2, {
    headers: { "content-type": "text/html; charset=utf-8" },
    status
  });
}
__name(static_error_page, "static_error_page");
async function handle_fatal_error(event, state2, options2, error22) {
  error22 = error22 instanceof HttpError ? error22 : coalesce_to_error(error22);
  const status = get_status(error22);
  const body2 = await handle_error_and_jsonify(event, state2, options2, error22);
  const type = negotiate(event.request.headers.get("accept") || "text/html", [
    "application/json",
    "text/html"
  ]);
  if (event.isDataRequest || type === "application/json") {
    return json(body2, {
      status
    });
  }
  return static_error_page(options2, status, body2.message);
}
__name(handle_fatal_error, "handle_fatal_error");
async function handle_error_and_jsonify(event, state2, options2, error22) {
  if (error22 instanceof HttpError) {
    return { message: "Unknown Error", ...error22.body };
  }
  const status = get_status(error22);
  const message = get_message(error22);
  return await with_request_store(
    { event, state: state2 },
    () => options2.hooks.handleError({ error: error22, event, status, message })
  ) ?? { message };
}
__name(handle_error_and_jsonify, "handle_error_and_jsonify");
function redirect_response(status, location) {
  const response = new Response(void 0, {
    status,
    headers: { location }
  });
  return response;
}
__name(redirect_response, "redirect_response");
function clarify_devalue_error(event, error22) {
  if (error22.path) {
    return `Data returned from \`load\` while rendering ${event.route.id} is not serializable: ${error22.message} (${error22.path}). If you need to serialize/deserialize custom types, use transport hooks: https://svelte.dev/docs/kit/hooks#Universal-hooks-transport.`;
  }
  if (error22.path === "") {
    return `Data returned from \`load\` while rendering ${event.route.id} is not a plain object`;
  }
  return error22.message;
}
__name(clarify_devalue_error, "clarify_devalue_error");
function serialize_uses(node) {
  const uses = {};
  if (node.uses && node.uses.dependencies.size > 0) {
    uses.dependencies = Array.from(node.uses.dependencies);
  }
  if (node.uses && node.uses.search_params.size > 0) {
    uses.search_params = Array.from(node.uses.search_params);
  }
  if (node.uses && node.uses.params.size > 0) {
    uses.params = Array.from(node.uses.params);
  }
  if (node.uses?.parent) uses.parent = 1;
  if (node.uses?.route) uses.route = 1;
  if (node.uses?.url) uses.url = 1;
  return uses;
}
__name(serialize_uses, "serialize_uses");
function has_prerendered_path(manifest2, pathname) {
  return manifest2._.prerendered_routes.has(pathname) || pathname.at(-1) === "/" && manifest2._.prerendered_routes.has(pathname.slice(0, -1));
}
__name(has_prerendered_path, "has_prerendered_path");
function format_server_error(status, error22, event) {
  const formatted_text = `
\x1B[1;31m[${status}] ${event.request.method} ${event.url.pathname}\x1B[0m`;
  if (status === 404) {
    return formatted_text;
  }
  return `${formatted_text}
${error22.stack}`;
}
__name(format_server_error, "format_server_error");
function get_node_type(node_id) {
  const parts = node_id?.split("/");
  const filename = parts?.at(-1);
  if (!filename) return "unknown";
  const dot_parts = filename.split(".");
  return dot_parts.slice(0, -1).join(".");
}
__name(get_node_type, "get_node_type");
async function render_endpoint(event, event_state, mod, state2) {
  const method = (
    /** @type {import('types').HttpMethod} */
    event.request.method
  );
  let handler = mod[method] || mod.fallback;
  if (method === "HEAD" && !mod.HEAD && mod.GET) {
    handler = mod.GET;
  }
  if (!handler) {
    return method_not_allowed(mod, method);
  }
  const prerender = mod.prerender ?? state2.prerender_default;
  if (prerender && (mod.POST || mod.PATCH || mod.PUT || mod.DELETE)) {
    throw new Error("Cannot prerender endpoints that have mutative methods");
  }
  if (state2.prerendering && !state2.prerendering.inside_reroute && !prerender) {
    if (state2.depth > 0) {
      throw new Error(`${event.route.id} is not prerenderable`);
    } else {
      return new Response(void 0, { status: 204 });
    }
  }
  event_state.is_endpoint_request = true;
  try {
    const response = await with_request_store(
      { event, state: event_state },
      () => handler(
        /** @type {import('@sveltejs/kit').RequestEvent<Record<string, any>>} */
        event
      )
    );
    if (!(response instanceof Response)) {
      throw new Error(
        `Invalid response from route ${event.url.pathname}: handler should return a Response object`
      );
    }
    if (state2.prerendering && (!state2.prerendering.inside_reroute || prerender)) {
      const cloned = new Response(response.clone().body, {
        status: response.status,
        statusText: response.statusText,
        headers: new Headers(response.headers)
      });
      cloned.headers.set("x-sveltekit-prerender", String(prerender));
      if (state2.prerendering.inside_reroute && prerender) {
        cloned.headers.set(
          "x-sveltekit-routeid",
          encodeURI(
            /** @type {string} */
            event.route.id
          )
        );
        state2.prerendering.dependencies.set(event.url.pathname, { response: cloned, body: null });
      } else {
        return cloned;
      }
    }
    return response;
  } catch (e3) {
    if (e3 instanceof Redirect) {
      return new Response(void 0, {
        status: e3.status,
        headers: { location: e3.location }
      });
    }
    throw e3;
  }
}
__name(render_endpoint, "render_endpoint");
function is_endpoint_request(event) {
  const { method, headers: headers2 } = event.request;
  if (ENDPOINT_METHODS.includes(method) && !PAGE_METHODS.includes(method)) {
    return true;
  }
  if (method === "POST" && headers2.get("x-sveltekit-action") === "true") return false;
  const accept = event.request.headers.get("accept") ?? "*/*";
  return negotiate(accept, ["*", "text/html"]) !== "text/html";
}
__name(is_endpoint_request, "is_endpoint_request");
function compact(arr) {
  return arr.filter(
    /** @returns {val is NonNullable<T>} */
    (val) => val != null
  );
}
__name(compact, "compact");
var DATA_SUFFIX = "/__data.json";
var HTML_DATA_SUFFIX = ".html__data.json";
function has_data_suffix2(pathname) {
  return pathname.endsWith(DATA_SUFFIX) || pathname.endsWith(HTML_DATA_SUFFIX);
}
__name(has_data_suffix2, "has_data_suffix");
function add_data_suffix2(pathname) {
  if (pathname.endsWith(".html")) return pathname.replace(/\.html$/, HTML_DATA_SUFFIX);
  return pathname.replace(/\/$/, "") + DATA_SUFFIX;
}
__name(add_data_suffix2, "add_data_suffix");
function strip_data_suffix2(pathname) {
  if (pathname.endsWith(HTML_DATA_SUFFIX)) {
    return pathname.slice(0, -HTML_DATA_SUFFIX.length) + ".html";
  }
  return pathname.slice(0, -DATA_SUFFIX.length);
}
__name(strip_data_suffix2, "strip_data_suffix");
var ROUTE_SUFFIX = "/__route.js";
function has_resolution_suffix2(pathname) {
  return pathname.endsWith(ROUTE_SUFFIX);
}
__name(has_resolution_suffix2, "has_resolution_suffix");
function add_resolution_suffix2(pathname) {
  return pathname.replace(/\/$/, "") + ROUTE_SUFFIX;
}
__name(add_resolution_suffix2, "add_resolution_suffix");
function strip_resolution_suffix2(pathname) {
  return pathname.slice(0, -ROUTE_SUFFIX.length);
}
__name(strip_resolution_suffix2, "strip_resolution_suffix");
var noop_span = {
  spanContext() {
    return noop_span_context;
  },
  setAttribute() {
    return this;
  },
  setAttributes() {
    return this;
  },
  addEvent() {
    return this;
  },
  setStatus() {
    return this;
  },
  updateName() {
    return this;
  },
  end() {
    return this;
  },
  isRecording() {
    return false;
  },
  recordException() {
    return this;
  },
  addLink() {
    return this;
  },
  addLinks() {
    return this;
  }
};
var noop_span_context = {
  traceId: "",
  spanId: "",
  traceFlags: 0
};
async function record_span({ name, attributes: attributes2, fn }) {
  {
    return fn(noop_span);
  }
}
__name(record_span, "record_span");
function is_action_json_request(event) {
  const accept = negotiate(event.request.headers.get("accept") ?? "*/*", [
    "application/json",
    "text/html"
  ]);
  return accept === "application/json" && event.request.method === "POST";
}
__name(is_action_json_request, "is_action_json_request");
async function handle_action_json_request(event, event_state, options2, server2) {
  const actions = server2?.actions;
  if (!actions) {
    const no_actions_error = new SvelteKitError(
      405,
      "Method Not Allowed",
      `POST method not allowed. No form actions exist for ${"this page"}`
    );
    return action_json(
      {
        type: "error",
        error: await handle_error_and_jsonify(event, event_state, options2, no_actions_error)
      },
      {
        status: no_actions_error.status,
        headers: {
          // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/405
          // "The server must generate an Allow header field in a 405 status code response"
          allow: "GET"
        }
      }
    );
  }
  check_named_default_separate(actions);
  try {
    const data = await call_action(event, event_state, actions);
    if (BROWSER) ;
    if (data instanceof ActionFailure) {
      return action_json({
        type: "failure",
        status: data.status,
        // @ts-expect-error we assign a string to what is supposed to be an object. That's ok
        // because we don't use the object outside, and this way we have better code navigation
        // through knowing where the related interface is used.
        data: stringify_action_response(
          data.data,
          /** @type {string} */
          event.route.id,
          options2.hooks.transport
        )
      });
    } else {
      return action_json({
        type: "success",
        status: data ? 200 : 204,
        // @ts-expect-error see comment above
        data: stringify_action_response(
          data,
          /** @type {string} */
          event.route.id,
          options2.hooks.transport
        )
      });
    }
  } catch (e3) {
    const err = normalize_error(e3);
    if (err instanceof Redirect) {
      return action_json_redirect(err);
    }
    return action_json(
      {
        type: "error",
        error: await handle_error_and_jsonify(
          event,
          event_state,
          options2,
          check_incorrect_fail_use(err)
        )
      },
      {
        status: get_status(err)
      }
    );
  }
}
__name(handle_action_json_request, "handle_action_json_request");
function check_incorrect_fail_use(error22) {
  return error22 instanceof ActionFailure ? new Error('Cannot "throw fail()". Use "return fail()"') : error22;
}
__name(check_incorrect_fail_use, "check_incorrect_fail_use");
function action_json_redirect(redirect) {
  return action_json({
    type: "redirect",
    status: redirect.status,
    location: redirect.location
  });
}
__name(action_json_redirect, "action_json_redirect");
function action_json(data, init2) {
  return json(data, init2);
}
__name(action_json, "action_json");
function is_action_request(event) {
  return event.request.method === "POST";
}
__name(is_action_request, "is_action_request");
async function handle_action_request(event, event_state, server2) {
  const actions = server2?.actions;
  if (!actions) {
    event.setHeaders({
      // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/405
      // "The server must generate an Allow header field in a 405 status code response"
      allow: "GET"
    });
    return {
      type: "error",
      error: new SvelteKitError(
        405,
        "Method Not Allowed",
        `POST method not allowed. No form actions exist for ${"this page"}`
      )
    };
  }
  check_named_default_separate(actions);
  try {
    const data = await call_action(event, event_state, actions);
    if (BROWSER) ;
    if (data instanceof ActionFailure) {
      return {
        type: "failure",
        status: data.status,
        data: data.data
      };
    } else {
      return {
        type: "success",
        status: 200,
        // @ts-expect-error this will be removed upon serialization, so `undefined` is the same as omission
        data
      };
    }
  } catch (e3) {
    const err = normalize_error(e3);
    if (err instanceof Redirect) {
      return {
        type: "redirect",
        status: err.status,
        location: err.location
      };
    }
    return {
      type: "error",
      error: check_incorrect_fail_use(err)
    };
  }
}
__name(handle_action_request, "handle_action_request");
function check_named_default_separate(actions) {
  if (actions.default && Object.keys(actions).length > 1) {
    throw new Error(
      "When using named actions, the default action cannot be used. See the docs for more info: https://svelte.dev/docs/kit/form-actions#named-actions"
    );
  }
}
__name(check_named_default_separate, "check_named_default_separate");
async function call_action(event, event_state, actions) {
  const url = new URL(event.request.url);
  let name = "default";
  for (const param of url.searchParams) {
    if (param[0].startsWith("/")) {
      name = param[0].slice(1);
      if (name === "default") {
        throw new Error('Cannot use reserved action name "default"');
      }
      break;
    }
  }
  const action = actions[name];
  if (!action) {
    throw new SvelteKitError(404, "Not Found", `No action with name '${name}' found`);
  }
  if (!is_form_content_type(event.request)) {
    throw new SvelteKitError(
      415,
      "Unsupported Media Type",
      `Form actions expect form-encoded data \u2014 received ${event.request.headers.get(
        "content-type"
      )}`
    );
  }
  return record_span({
    name: "sveltekit.form_action",
    attributes: {
      "http.route": event.route.id || "unknown"
    },
    fn: /* @__PURE__ */ __name(async (current2) => {
      const traced_event = merge_tracing(event, current2);
      const result = await with_request_store(
        { event: traced_event, state: event_state },
        () => action(traced_event)
      );
      if (result instanceof ActionFailure) {
        current2.setAttributes({
          "sveltekit.form_action.result.type": "failure",
          "sveltekit.form_action.result.status": result.status
        });
      }
      return result;
    }, "fn")
  });
}
__name(call_action, "call_action");
function uneval_action_response(data, route_id, transport) {
  const replacer = /* @__PURE__ */ __name((thing) => {
    for (const key2 in transport) {
      const encoded = transport[key2].encode(thing);
      if (encoded) {
        return `app.decode('${key2}', ${uneval(encoded, replacer)})`;
      }
    }
  }, "replacer");
  return try_serialize(data, (value) => uneval(value, replacer), route_id);
}
__name(uneval_action_response, "uneval_action_response");
function stringify_action_response(data, route_id, transport) {
  const encoders = Object.fromEntries(
    Object.entries(transport).map(([key2, value]) => [key2, value.encode])
  );
  return try_serialize(data, (value) => stringify(value, encoders), route_id);
}
__name(stringify_action_response, "stringify_action_response");
function try_serialize(data, fn, route_id) {
  try {
    return fn(data);
  } catch (e3) {
    const error22 = (
      /** @type {any} */
      e3
    );
    if (data instanceof Response) {
      throw new Error(
        `Data returned from action inside ${route_id} is not serializable. Form actions need to return plain objects or fail(). E.g. return { success: true } or return fail(400, { message: "invalid" });`
      );
    }
    if ("path" in error22) {
      let message = `Data returned from action inside ${route_id} is not serializable: ${error22.message}`;
      if (error22.path !== "") message += ` (data.${error22.path})`;
      throw new Error(message);
    }
    throw error22;
  }
}
__name(try_serialize, "try_serialize");
function create_async_iterator() {
  let resolved = -1;
  let returned = -1;
  const deferred2 = [];
  return {
    iterate: /* @__PURE__ */ __name((transform = (x) => x) => {
      return {
        [Symbol.asyncIterator]() {
          return {
            next: /* @__PURE__ */ __name(async () => {
              const next2 = deferred2[++returned];
              if (!next2) return { value: null, done: true };
              const value = await next2.promise;
              return { value: transform(value), done: false };
            }, "next")
          };
        }
      };
    }, "iterate"),
    add: /* @__PURE__ */ __name((promise) => {
      deferred2.push(with_resolvers());
      void promise.then((value) => {
        deferred2[++resolved].resolve(value);
      });
    }, "add")
  };
}
__name(create_async_iterator, "create_async_iterator");
function server_data_serializer(event, event_state, options2) {
  let promise_id = 1;
  let max_nodes = -1;
  const iterator = create_async_iterator();
  const global = get_global_name(options2);
  function get_replacer(index5) {
    return /* @__PURE__ */ __name(function replacer(thing) {
      if (typeof thing?.then === "function") {
        const id = promise_id++;
        const promise = thing.then(
          /** @param {any} data */
          (data) => ({ data })
        ).catch(
          /** @param {any} error */
          async (error22) => ({
            error: await handle_error_and_jsonify(event, event_state, options2, error22)
          })
        ).then(
          /**
           * @param {{data: any; error: any}} result
           */
          async ({ data, error: error22 }) => {
            let str;
            try {
              str = uneval(error22 ? [, error22] : [data], replacer);
            } catch {
              error22 = await handle_error_and_jsonify(
                event,
                event_state,
                options2,
                new Error(`Failed to serialize promise while rendering ${event.route.id}`)
              );
              data = void 0;
              str = uneval([, error22], replacer);
            }
            return {
              index: index5,
              str: `${global}.resolve(${id}, ${str.includes("app.decode") ? `(app) => ${str}` : `() => ${str}`})`
            };
          }
        );
        iterator.add(promise);
        return `${global}.defer(${id})`;
      } else {
        for (const key2 in options2.hooks.transport) {
          const encoded = options2.hooks.transport[key2].encode(thing);
          if (encoded) {
            return `app.decode('${key2}', ${uneval(encoded, replacer)})`;
          }
        }
      }
    }, "replacer");
  }
  __name(get_replacer, "get_replacer");
  const strings = (
    /** @type {string[]} */
    []
  );
  return {
    set_max_nodes(i) {
      max_nodes = i;
    },
    add_node(i, node) {
      try {
        if (!node) {
          strings[i] = "null";
          return;
        }
        const payload = { type: "data", data: node.data, uses: serialize_uses(node) };
        if (node.slash) payload.slash = node.slash;
        strings[i] = uneval(payload, get_replacer(i));
      } catch (e3) {
        e3.path = e3.path.slice(1);
        throw new Error(clarify_devalue_error(
          event,
          /** @type {any} */
          e3
        ));
      }
    },
    get_data(csp) {
      const open = `<script${csp.script_needs_nonce ? ` nonce="${csp.nonce}"` : ""}>`;
      const close = `<\/script>
`;
      return {
        data: `[${compact(max_nodes > -1 ? strings.slice(0, max_nodes) : strings).join(",")}]`,
        chunks: promise_id > 1 ? iterator.iterate(({ index: index5, str }) => {
          if (max_nodes > -1 && index5 >= max_nodes) {
            return "";
          }
          return open + str + close;
        }) : null
      };
    }
  };
}
__name(server_data_serializer, "server_data_serializer");
function server_data_serializer_json(event, event_state, options2) {
  let promise_id = 1;
  const iterator = create_async_iterator();
  const reducers = {
    ...Object.fromEntries(
      Object.entries(options2.hooks.transport).map(([key2, value]) => [key2, value.encode])
    ),
    /** @param {any} thing */
    Promise: /* @__PURE__ */ __name((thing) => {
      if (typeof thing?.then !== "function") {
        return;
      }
      const id = promise_id++;
      let key2 = "data";
      const promise = thing.catch(
        /** @param {any} e */
        async (e3) => {
          key2 = "error";
          return handle_error_and_jsonify(
            event,
            event_state,
            options2,
            /** @type {any} */
            e3
          );
        }
      ).then(
        /** @param {any} value */
        async (value) => {
          let str;
          try {
            str = stringify(value, reducers);
          } catch {
            const error22 = await handle_error_and_jsonify(
              event,
              event_state,
              options2,
              new Error(`Failed to serialize promise while rendering ${event.route.id}`)
            );
            key2 = "error";
            str = stringify(error22, reducers);
          }
          return `{"type":"chunk","id":${id},"${key2}":${str}}
`;
        }
      );
      iterator.add(promise);
      return id;
    }, "Promise")
  };
  const strings = (
    /** @type {string[]} */
    []
  );
  return {
    add_node(i, node) {
      try {
        if (!node) {
          strings[i] = "null";
          return;
        }
        if (node.type === "error" || node.type === "skip") {
          strings[i] = JSON.stringify(node);
          return;
        }
        strings[i] = `{"type":"data","data":${stringify(node.data, reducers)},"uses":${JSON.stringify(
          serialize_uses(node)
        )}${node.slash ? `,"slash":${JSON.stringify(node.slash)}` : ""}}`;
      } catch (e3) {
        e3.path = "data" + e3.path;
        throw new Error(clarify_devalue_error(
          event,
          /** @type {any} */
          e3
        ));
      }
    },
    get_data() {
      return {
        data: `{"type":"data","nodes":[${strings.join(",")}]}
`,
        chunks: promise_id > 1 ? iterator.iterate() : null
      };
    }
  };
}
__name(server_data_serializer_json, "server_data_serializer_json");
async function load_server_data({ event, event_state, state: state2, node, parent }) {
  if (!node?.server) return null;
  let is_tracking = true;
  const uses = {
    dependencies: /* @__PURE__ */ new Set(),
    params: /* @__PURE__ */ new Set(),
    parent: false,
    route: false,
    url: false,
    search_params: /* @__PURE__ */ new Set()
  };
  const load2 = node.server.load;
  const slash = node.server.trailingSlash;
  if (!load2) {
    return { type: "data", data: null, uses, slash };
  }
  const url = make_trackable(
    event.url,
    () => {
      if (is_tracking) {
        uses.url = true;
      }
    },
    (param) => {
      if (is_tracking) {
        uses.search_params.add(param);
      }
    }
  );
  if (state2.prerendering) {
    disable_search(url);
  }
  const result = await record_span({
    name: "sveltekit.load",
    attributes: {
      "sveltekit.load.node_id": node.server_id || "unknown",
      "sveltekit.load.node_type": get_node_type(node.server_id),
      "http.route": event.route.id || "unknown"
    },
    fn: /* @__PURE__ */ __name(async (current2) => {
      const traced_event = merge_tracing(event, current2);
      const result2 = await with_request_store(
        { event: traced_event, state: event_state },
        () => load2.call(null, {
          ...traced_event,
          fetch: /* @__PURE__ */ __name((info3, init2) => {
            new URL(info3 instanceof Request ? info3.url : info3, event.url);
            return event.fetch(info3, init2);
          }, "fetch"),
          /** @param {string[]} deps */
          depends: /* @__PURE__ */ __name((...deps) => {
            for (const dep of deps) {
              const { href } = new URL(dep, event.url);
              uses.dependencies.add(href);
            }
          }, "depends"),
          params: new Proxy(event.params, {
            get: /* @__PURE__ */ __name((target, key2) => {
              if (is_tracking) {
                uses.params.add(key2);
              }
              return target[
                /** @type {string} */
                key2
              ];
            }, "get")
          }),
          parent: /* @__PURE__ */ __name(async () => {
            if (is_tracking) {
              uses.parent = true;
            }
            return parent();
          }, "parent"),
          route: new Proxy(event.route, {
            get: /* @__PURE__ */ __name((target, key2) => {
              if (is_tracking) {
                uses.route = true;
              }
              return target[
                /** @type {'id'} */
                key2
              ];
            }, "get")
          }),
          url,
          untrack(fn) {
            is_tracking = false;
            try {
              return fn();
            } finally {
              is_tracking = true;
            }
          }
        })
      );
      return result2;
    }, "fn")
  });
  return {
    type: "data",
    data: result ?? null,
    uses,
    slash
  };
}
__name(load_server_data, "load_server_data");
async function load_data({
  event,
  event_state,
  fetched,
  node,
  parent,
  server_data_promise,
  state: state2,
  resolve_opts,
  csr
}) {
  const server_data_node = await server_data_promise;
  const load2 = node?.universal?.load;
  if (!load2) {
    return server_data_node?.data ?? null;
  }
  const result = await record_span({
    name: "sveltekit.load",
    attributes: {
      "sveltekit.load.node_id": node.universal_id || "unknown",
      "sveltekit.load.node_type": get_node_type(node.universal_id),
      "http.route": event.route.id || "unknown"
    },
    fn: /* @__PURE__ */ __name(async (current2) => {
      const traced_event = merge_tracing(event, current2);
      return await with_request_store(
        { event: traced_event, state: event_state },
        () => load2.call(null, {
          url: event.url,
          params: event.params,
          data: server_data_node?.data ?? null,
          route: event.route,
          fetch: create_universal_fetch(event, state2, fetched, csr, resolve_opts),
          setHeaders: event.setHeaders,
          depends: /* @__PURE__ */ __name(() => {
          }, "depends"),
          parent,
          untrack: /* @__PURE__ */ __name((fn) => fn(), "untrack"),
          tracing: traced_event.tracing
        })
      );
    }, "fn")
  });
  return result ?? null;
}
__name(load_data, "load_data");
function create_universal_fetch(event, state2, fetched, csr, resolve_opts) {
  const universal_fetch = /* @__PURE__ */ __name(async (input, init2) => {
    const cloned_body = input instanceof Request && input.body ? input.clone().body : null;
    const cloned_headers = input instanceof Request && [...input.headers].length ? new Headers(input.headers) : init2?.headers;
    let response = await event.fetch(input, init2);
    const url = new URL(input instanceof Request ? input.url : input, event.url);
    const same_origin = url.origin === event.url.origin;
    let dependency;
    if (same_origin) {
      if (state2.prerendering) {
        dependency = { response, body: null };
        state2.prerendering.dependencies.set(url.pathname, dependency);
      }
    } else if (url.protocol === "https:" || url.protocol === "http:") {
      const mode = input instanceof Request ? input.mode : init2?.mode ?? "cors";
      if (mode === "no-cors") {
        response = new Response("", {
          status: response.status,
          statusText: response.statusText,
          headers: response.headers
        });
      } else {
        const acao = response.headers.get("access-control-allow-origin");
        if (!acao || acao !== event.url.origin && acao !== "*") {
          throw new Error(
            `CORS error: ${acao ? "Incorrect" : "No"} 'Access-Control-Allow-Origin' header is present on the requested resource`
          );
        }
      }
    }
    let teed_body;
    const proxy2 = new Proxy(response, {
      get(response2, key2, _receiver) {
        async function push_fetched(body2, is_b64) {
          const status_number = Number(response2.status);
          if (isNaN(status_number)) {
            throw new Error(
              `response.status is not a number. value: "${response2.status}" type: ${typeof response2.status}`
            );
          }
          fetched.push({
            url: same_origin ? url.href.slice(event.url.origin.length) : url.href,
            method: event.request.method,
            request_body: (
              /** @type {string | ArrayBufferView | undefined} */
              input instanceof Request && cloned_body ? await stream_to_string(cloned_body) : init2?.body
            ),
            request_headers: cloned_headers,
            response_body: body2,
            response: response2,
            is_b64
          });
        }
        __name(push_fetched, "push_fetched");
        if (key2 === "body") {
          if (response2.body === null) {
            return null;
          }
          if (teed_body) {
            return teed_body;
          }
          const [a, b] = response2.body.tee();
          void (async () => {
            let result = new Uint8Array();
            for await (const chunk of a) {
              const combined = new Uint8Array(result.length + chunk.length);
              combined.set(result, 0);
              combined.set(chunk, result.length);
              result = combined;
            }
            if (dependency) {
              dependency.body = new Uint8Array(result);
            }
            void push_fetched(base64_encode(result), true);
          })();
          return teed_body = b;
        }
        if (key2 === "arrayBuffer") {
          return async () => {
            const buffer = await response2.arrayBuffer();
            const bytes = new Uint8Array(buffer);
            if (dependency) {
              dependency.body = bytes;
            }
            if (buffer instanceof ArrayBuffer) {
              await push_fetched(base64_encode(bytes), true);
            }
            return buffer;
          };
        }
        async function text2() {
          const body2 = await response2.text();
          if (body2 === "" && NULL_BODY_STATUS.includes(response2.status)) {
            await push_fetched(void 0, false);
            return void 0;
          }
          if (!body2 || typeof body2 === "string") {
            await push_fetched(body2, false);
          }
          if (dependency) {
            dependency.body = body2;
          }
          return body2;
        }
        __name(text2, "text2");
        if (key2 === "text") {
          return text2;
        }
        if (key2 === "json") {
          return async () => {
            const body2 = await text2();
            return body2 ? JSON.parse(body2) : void 0;
          };
        }
        return Reflect.get(response2, key2, response2);
      }
    });
    if (csr) {
      const get2 = response.headers.get;
      response.headers.get = (key2) => {
        const lower = key2.toLowerCase();
        const value = get2.call(response.headers, lower);
        if (value && !lower.startsWith("x-sveltekit-")) {
          const included = resolve_opts.filterSerializedResponseHeaders(lower, value);
          if (!included) {
            throw new Error(
              `Failed to get response header "${lower}" \u2014 it must be included by the \`filterSerializedResponseHeaders\` option: https://svelte.dev/docs/kit/hooks#Server-hooks-handle (at ${event.route.id})`
            );
          }
        }
        return value;
      };
    }
    return proxy2;
  }, "universal_fetch");
  return (input, init2) => {
    const response = universal_fetch(input, init2);
    response.catch(() => {
    });
    return response;
  };
}
__name(create_universal_fetch, "create_universal_fetch");
async function stream_to_string(stream) {
  let result = "";
  const reader = stream.getReader();
  while (true) {
    const { done, value } = await reader.read();
    if (done) {
      break;
    }
    result += text_decoder2.decode(value);
  }
  return result;
}
__name(stream_to_string, "stream_to_string");
function hash(...values) {
  let hash2 = 5381;
  for (const value of values) {
    if (typeof value === "string") {
      let i = value.length;
      while (i) hash2 = hash2 * 33 ^ value.charCodeAt(--i);
    } else if (ArrayBuffer.isView(value)) {
      const buffer = new Uint8Array(value.buffer, value.byteOffset, value.byteLength);
      let i = buffer.length;
      while (i) hash2 = hash2 * 33 ^ buffer[--i];
    } else {
      throw new TypeError("value must be a string or TypedArray");
    }
  }
  return (hash2 >>> 0).toString(36);
}
__name(hash, "hash");
var replacements2 = {
  "<": "\\u003C",
  "\u2028": "\\u2028",
  "\u2029": "\\u2029"
};
var pattern = new RegExp(`[${Object.keys(replacements2).join("")}]`, "g");
function serialize_data(fetched, filter, prerendering = false) {
  const headers2 = {};
  let cache_control = null;
  let age = null;
  let varyAny = false;
  for (const [key2, value] of fetched.response.headers) {
    if (filter(key2, value)) {
      headers2[key2] = value;
    }
    if (key2 === "cache-control") cache_control = value;
    else if (key2 === "age") age = value;
    else if (key2 === "vary" && value.trim() === "*") varyAny = true;
  }
  const payload = {
    status: fetched.response.status,
    statusText: fetched.response.statusText,
    headers: headers2,
    body: fetched.response_body
  };
  const safe_payload = JSON.stringify(payload).replace(pattern, (match) => replacements2[match]);
  const attrs = [
    'type="application/json"',
    "data-sveltekit-fetched",
    `data-url="${escape_html2(fetched.url, true)}"`
  ];
  if (fetched.is_b64) {
    attrs.push("data-b64");
  }
  if (fetched.request_headers || fetched.request_body) {
    const values = [];
    if (fetched.request_headers) {
      values.push([...new Headers(fetched.request_headers)].join(","));
    }
    if (fetched.request_body) {
      values.push(fetched.request_body);
    }
    attrs.push(`data-hash="${hash(...values)}"`);
  }
  if (!prerendering && fetched.method === "GET" && cache_control && !varyAny) {
    const match = /s-maxage=(\d+)/g.exec(cache_control) ?? /max-age=(\d+)/g.exec(cache_control);
    if (match) {
      const ttl = +match[1] - +(age ?? "0");
      attrs.push(`data-ttl="${ttl}"`);
    }
  }
  return `<script ${attrs.join(" ")}>${safe_payload}<\/script>`;
}
__name(serialize_data, "serialize_data");
var s = JSON.stringify;
function sha256(data) {
  if (!key[0]) precompute();
  const out = init.slice(0);
  const array2 = encode(data);
  for (let i = 0; i < array2.length; i += 16) {
    const w = array2.subarray(i, i + 16);
    let tmp;
    let a;
    let b;
    let out0 = out[0];
    let out1 = out[1];
    let out2 = out[2];
    let out3 = out[3];
    let out4 = out[4];
    let out5 = out[5];
    let out6 = out[6];
    let out7 = out[7];
    for (let i2 = 0; i2 < 64; i2++) {
      if (i2 < 16) {
        tmp = w[i2];
      } else {
        a = w[i2 + 1 & 15];
        b = w[i2 + 14 & 15];
        tmp = w[i2 & 15] = (a >>> 7 ^ a >>> 18 ^ a >>> 3 ^ a << 25 ^ a << 14) + (b >>> 17 ^ b >>> 19 ^ b >>> 10 ^ b << 15 ^ b << 13) + w[i2 & 15] + w[i2 + 9 & 15] | 0;
      }
      tmp = tmp + out7 + (out4 >>> 6 ^ out4 >>> 11 ^ out4 >>> 25 ^ out4 << 26 ^ out4 << 21 ^ out4 << 7) + (out6 ^ out4 & (out5 ^ out6)) + key[i2];
      out7 = out6;
      out6 = out5;
      out5 = out4;
      out4 = out3 + tmp | 0;
      out3 = out2;
      out2 = out1;
      out1 = out0;
      out0 = tmp + (out1 & out2 ^ out3 & (out1 ^ out2)) + (out1 >>> 2 ^ out1 >>> 13 ^ out1 >>> 22 ^ out1 << 30 ^ out1 << 19 ^ out1 << 10) | 0;
    }
    out[0] = out[0] + out0 | 0;
    out[1] = out[1] + out1 | 0;
    out[2] = out[2] + out2 | 0;
    out[3] = out[3] + out3 | 0;
    out[4] = out[4] + out4 | 0;
    out[5] = out[5] + out5 | 0;
    out[6] = out[6] + out6 | 0;
    out[7] = out[7] + out7 | 0;
  }
  const bytes = new Uint8Array(out.buffer);
  reverse_endianness(bytes);
  return btoa(String.fromCharCode(...bytes));
}
__name(sha256, "sha256");
var init = new Uint32Array(8);
var key = new Uint32Array(64);
function precompute() {
  function frac(x) {
    return (x - Math.floor(x)) * 4294967296;
  }
  __name(frac, "frac");
  let prime = 2;
  for (let i = 0; i < 64; prime++) {
    let is_prime = true;
    for (let factor = 2; factor * factor <= prime; factor++) {
      if (prime % factor === 0) {
        is_prime = false;
        break;
      }
    }
    if (is_prime) {
      if (i < 8) {
        init[i] = frac(prime ** (1 / 2));
      }
      key[i] = frac(prime ** (1 / 3));
      i++;
    }
  }
}
__name(precompute, "precompute");
function reverse_endianness(bytes) {
  for (let i = 0; i < bytes.length; i += 4) {
    const a = bytes[i + 0];
    const b = bytes[i + 1];
    const c2 = bytes[i + 2];
    const d = bytes[i + 3];
    bytes[i + 0] = d;
    bytes[i + 1] = c2;
    bytes[i + 2] = b;
    bytes[i + 3] = a;
  }
}
__name(reverse_endianness, "reverse_endianness");
function encode(str) {
  const encoded = text_encoder2.encode(str);
  const length = encoded.length * 8;
  const size = 512 * Math.ceil((length + 65) / 512);
  const bytes = new Uint8Array(size / 8);
  bytes.set(encoded);
  bytes[encoded.length] = 128;
  reverse_endianness(bytes);
  const words = new Uint32Array(bytes.buffer);
  words[words.length - 2] = Math.floor(length / 4294967296);
  words[words.length - 1] = length;
  return words;
}
__name(encode, "encode");
var array = new Uint8Array(16);
function generate_nonce() {
  crypto.getRandomValues(array);
  return btoa(String.fromCharCode(...array));
}
__name(generate_nonce, "generate_nonce");
var quoted = /* @__PURE__ */ new Set([
  "self",
  "unsafe-eval",
  "unsafe-hashes",
  "unsafe-inline",
  "none",
  "strict-dynamic",
  "report-sample",
  "wasm-unsafe-eval",
  "script"
]);
var crypto_pattern = /^(nonce|sha\d\d\d)-/;
var BaseProvider = class {
  static {
    __name(this, "BaseProvider");
  }
  /** @type {boolean} */
  #use_hashes;
  /** @type {boolean} */
  #script_needs_csp;
  /** @type {boolean} */
  #script_src_needs_csp;
  /** @type {boolean} */
  #script_src_elem_needs_csp;
  /** @type {boolean} */
  #style_needs_csp;
  /** @type {boolean} */
  #style_src_needs_csp;
  /** @type {boolean} */
  #style_src_attr_needs_csp;
  /** @type {boolean} */
  #style_src_elem_needs_csp;
  /** @type {import('types').CspDirectives} */
  #directives;
  /** @type {import('types').Csp.Source[]} */
  #script_src;
  /** @type {import('types').Csp.Source[]} */
  #script_src_elem;
  /** @type {import('types').Csp.Source[]} */
  #style_src;
  /** @type {import('types').Csp.Source[]} */
  #style_src_attr;
  /** @type {import('types').Csp.Source[]} */
  #style_src_elem;
  /** @type {string} */
  #nonce;
  /**
   * @param {boolean} use_hashes
   * @param {import('types').CspDirectives} directives
   * @param {string} nonce
   */
  constructor(use_hashes, directives, nonce) {
    this.#use_hashes = use_hashes;
    this.#directives = directives;
    const d = this.#directives;
    this.#script_src = [];
    this.#script_src_elem = [];
    this.#style_src = [];
    this.#style_src_attr = [];
    this.#style_src_elem = [];
    const effective_script_src = d["script-src"] || d["default-src"];
    const script_src_elem = d["script-src-elem"];
    const effective_style_src = d["style-src"] || d["default-src"];
    const style_src_attr = d["style-src-attr"];
    const style_src_elem = d["style-src-elem"];
    const needs_csp = /* @__PURE__ */ __name((directive) => !!directive && !directive.some((value) => value === "unsafe-inline"), "needs_csp");
    this.#script_src_needs_csp = needs_csp(effective_script_src);
    this.#script_src_elem_needs_csp = needs_csp(script_src_elem);
    this.#style_src_needs_csp = needs_csp(effective_style_src);
    this.#style_src_attr_needs_csp = needs_csp(style_src_attr);
    this.#style_src_elem_needs_csp = needs_csp(style_src_elem);
    this.#script_needs_csp = this.#script_src_needs_csp || this.#script_src_elem_needs_csp;
    this.#style_needs_csp = this.#style_src_needs_csp || this.#style_src_attr_needs_csp || this.#style_src_elem_needs_csp;
    this.script_needs_nonce = this.#script_needs_csp && !this.#use_hashes;
    this.style_needs_nonce = this.#style_needs_csp && !this.#use_hashes;
    this.#nonce = nonce;
  }
  /** @param {string} content */
  add_script(content) {
    if (!this.#script_needs_csp) return;
    const source2 = this.#use_hashes ? `sha256-${sha256(content)}` : `nonce-${this.#nonce}`;
    if (this.#script_src_needs_csp) {
      this.#script_src.push(source2);
    }
    if (this.#script_src_elem_needs_csp) {
      this.#script_src_elem.push(source2);
    }
  }
  /** @param {string} content */
  add_style(content) {
    if (!this.#style_needs_csp) return;
    const source2 = this.#use_hashes ? `sha256-${sha256(content)}` : `nonce-${this.#nonce}`;
    if (this.#style_src_needs_csp) {
      this.#style_src.push(source2);
    }
    if (this.#style_src_attr_needs_csp) {
      this.#style_src_attr.push(source2);
    }
    if (this.#style_src_elem_needs_csp) {
      const sha256_empty_comment_hash = "sha256-9OlNO0DNEeaVzHL4RZwCLsBHA8WBQ8toBp/4F5XV2nc=";
      const d = this.#directives;
      if (d["style-src-elem"] && !d["style-src-elem"].includes(sha256_empty_comment_hash) && !this.#style_src_elem.includes(sha256_empty_comment_hash)) {
        this.#style_src_elem.push(sha256_empty_comment_hash);
      }
      if (source2 !== sha256_empty_comment_hash) {
        this.#style_src_elem.push(source2);
      }
    }
  }
  /**
   * @param {boolean} [is_meta]
   */
  get_header(is_meta = false) {
    const header = [];
    const directives = { ...this.#directives };
    if (this.#style_src.length > 0) {
      directives["style-src"] = [
        ...directives["style-src"] || directives["default-src"] || [],
        ...this.#style_src
      ];
    }
    if (this.#style_src_attr.length > 0) {
      directives["style-src-attr"] = [
        ...directives["style-src-attr"] || [],
        ...this.#style_src_attr
      ];
    }
    if (this.#style_src_elem.length > 0) {
      directives["style-src-elem"] = [
        ...directives["style-src-elem"] || [],
        ...this.#style_src_elem
      ];
    }
    if (this.#script_src.length > 0) {
      directives["script-src"] = [
        ...directives["script-src"] || directives["default-src"] || [],
        ...this.#script_src
      ];
    }
    if (this.#script_src_elem.length > 0) {
      directives["script-src-elem"] = [
        ...directives["script-src-elem"] || [],
        ...this.#script_src_elem
      ];
    }
    for (const key2 in directives) {
      if (is_meta && (key2 === "frame-ancestors" || key2 === "report-uri" || key2 === "sandbox")) {
        continue;
      }
      const value = (
        /** @type {string[] | true} */
        directives[key2]
      );
      if (!value) continue;
      const directive = [key2];
      if (Array.isArray(value)) {
        value.forEach((value2) => {
          if (quoted.has(value2) || crypto_pattern.test(value2)) {
            directive.push(`'${value2}'`);
          } else {
            directive.push(value2);
          }
        });
      }
      header.push(directive.join(" "));
    }
    return header.join("; ");
  }
};
var CspProvider = class extends BaseProvider {
  static {
    __name(this, "CspProvider");
  }
  get_meta() {
    const content = this.get_header(true);
    if (!content) {
      return;
    }
    return `<meta http-equiv="content-security-policy" content="${escape_html2(content, true)}">`;
  }
};
var CspReportOnlyProvider = class extends BaseProvider {
  static {
    __name(this, "CspReportOnlyProvider");
  }
  /**
   * @param {boolean} use_hashes
   * @param {import('types').CspDirectives} directives
   * @param {string} nonce
   */
  constructor(use_hashes, directives, nonce) {
    super(use_hashes, directives, nonce);
    if (Object.values(directives).filter((v) => !!v).length > 0) {
      const has_report_to = directives["report-to"]?.length ?? 0 > 0;
      const has_report_uri = directives["report-uri"]?.length ?? 0 > 0;
      if (!has_report_to && !has_report_uri) {
        throw Error(
          "`content-security-policy-report-only` must be specified with either the `report-to` or `report-uri` directives, or both"
        );
      }
    }
  }
};
var Csp = class {
  static {
    __name(this, "Csp");
  }
  /** @readonly */
  nonce = generate_nonce();
  /** @type {CspProvider} */
  csp_provider;
  /** @type {CspReportOnlyProvider} */
  report_only_provider;
  /**
   * @param {import('./types.js').CspConfig} config
   * @param {import('./types.js').CspOpts} opts
   */
  constructor({ mode, directives, reportOnly }, { prerender }) {
    const use_hashes = mode === "hash" || mode === "auto" && prerender;
    this.csp_provider = new CspProvider(use_hashes, directives, this.nonce);
    this.report_only_provider = new CspReportOnlyProvider(use_hashes, reportOnly, this.nonce);
  }
  get script_needs_nonce() {
    return this.csp_provider.script_needs_nonce || this.report_only_provider.script_needs_nonce;
  }
  get style_needs_nonce() {
    return this.csp_provider.style_needs_nonce || this.report_only_provider.style_needs_nonce;
  }
  /** @param {string} content */
  add_script(content) {
    this.csp_provider.add_script(content);
    this.report_only_provider.add_script(content);
  }
  /** @param {string} content */
  add_style(content) {
    this.csp_provider.add_style(content);
    this.report_only_provider.add_style(content);
  }
};
function exec(match, params, matchers) {
  const result = {};
  const values = match.slice(1);
  const values_needing_match = values.filter((value) => value !== void 0);
  let buffered = 0;
  for (let i = 0; i < params.length; i += 1) {
    const param = params[i];
    let value = values[i - buffered];
    if (param.chained && param.rest && buffered) {
      value = values.slice(i - buffered, i + 1).filter((s22) => s22).join("/");
      buffered = 0;
    }
    if (value === void 0) {
      if (param.rest) result[param.name] = "";
      continue;
    }
    if (!param.matcher || matchers[param.matcher](value)) {
      result[param.name] = value;
      const next_param = params[i + 1];
      const next_value = values[i + 1];
      if (next_param && !next_param.rest && next_param.optional && next_value && param.chained) {
        buffered = 0;
      }
      if (!next_param && !next_value && Object.keys(result).length === values_needing_match.length) {
        buffered = 0;
      }
      continue;
    }
    if (param.optional && param.chained) {
      buffered++;
      continue;
    }
    return;
  }
  if (buffered) return;
  return result;
}
__name(exec, "exec");
function generate_route_object(route, url, manifest2) {
  const { errors, layouts, leaf } = route;
  const nodes = [...errors, ...layouts.map((l) => l?.[1]), leaf[1]].filter((n2) => typeof n2 === "number").map((n2) => `'${n2}': () => ${create_client_import(manifest2._.client.nodes?.[n2], url)}`).join(",\n		");
  return [
    `{
	id: ${s(route.id)}`,
    `errors: ${s(route.errors)}`,
    `layouts: ${s(route.layouts)}`,
    `leaf: ${s(route.leaf)}`,
    `nodes: {
		${nodes}
	}
}`
  ].join(",\n	");
}
__name(generate_route_object, "generate_route_object");
function create_client_import(import_path, url) {
  if (!import_path) return "Promise.resolve({})";
  if (import_path[0] === "/") {
    return `import('${import_path}')`;
  }
  if (assets !== "") {
    return `import('${assets}/${import_path}')`;
  }
  let path = get_relative_path(url.pathname, `${base}/${import_path}`);
  if (path[0] !== ".") path = `./${path}`;
  return `import('${path}')`;
}
__name(create_client_import, "create_client_import");
async function resolve_route(resolved_path, url, manifest2) {
  if (!manifest2._.client.routes) {
    return text("Server-side route resolution disabled", { status: 400 });
  }
  let route = null;
  let params = {};
  const matchers = await manifest2._.matchers();
  for (const candidate of manifest2._.client.routes) {
    const match = candidate.pattern.exec(resolved_path);
    if (!match) continue;
    const matched = exec(match, candidate.params, matchers);
    if (matched) {
      route = candidate;
      params = decode_params(matched);
      break;
    }
  }
  return create_server_routing_response(route, params, url, manifest2).response;
}
__name(resolve_route, "resolve_route");
function create_server_routing_response(route, params, url, manifest2) {
  const headers2 = new Headers({
    "content-type": "application/javascript; charset=utf-8"
  });
  if (route) {
    const csr_route = generate_route_object(route, url, manifest2);
    const body2 = `${create_css_import(route, url, manifest2)}
export const route = ${csr_route}; export const params = ${JSON.stringify(params)};`;
    return { response: text(body2, { headers: headers2 }), body: body2 };
  } else {
    return { response: text("", { headers: headers2 }), body: "" };
  }
}
__name(create_server_routing_response, "create_server_routing_response");
function create_css_import(route, url, manifest2) {
  const { errors, layouts, leaf } = route;
  let css = "";
  for (const node of [...errors, ...layouts.map((l) => l?.[1]), leaf[1]]) {
    if (typeof node !== "number") continue;
    const node_css = manifest2._.client.css?.[node];
    for (const css_path of node_css ?? []) {
      css += `'${assets || base}/${css_path}',`;
    }
  }
  if (!css) return "";
  return `${create_client_import(
    /** @type {string} */
    manifest2._.client.start,
    url
  )}.then(x => x.load_css([${css}]));`;
}
__name(create_css_import, "create_css_import");
var updated = {
  ...readable(false),
  check: /* @__PURE__ */ __name(() => false, "check")
};
async function render_response({
  branch: branch2,
  fetched,
  options: options2,
  manifest: manifest2,
  state: state2,
  page_config,
  status,
  error: error22 = null,
  event,
  event_state,
  resolve_opts,
  action_result,
  data_serializer
}) {
  if (state2.prerendering) {
    if (options2.csp.mode === "nonce") {
      throw new Error('Cannot use prerendering if config.kit.csp.mode === "nonce"');
    }
    if (options2.app_template_contains_nonce) {
      throw new Error("Cannot use prerendering if page template contains %sveltekit.nonce%");
    }
  }
  const { client } = manifest2._;
  const modulepreloads = new Set(client.imports);
  const stylesheets5 = new Set(client.stylesheets);
  const fonts5 = new Set(client.fonts);
  const link_headers = /* @__PURE__ */ new Set();
  const link_tags = /* @__PURE__ */ new Set();
  const inline_styles = /* @__PURE__ */ new Map();
  let rendered;
  const form_value = action_result?.type === "success" || action_result?.type === "failure" ? action_result.data ?? null : null;
  let base$1 = base;
  let assets$1 = assets;
  let base_expression = s(base);
  {
    if (!state2.prerendering?.fallback) {
      const segments = event.url.pathname.slice(base.length).split("/").slice(2);
      base$1 = segments.map(() => "..").join("/") || ".";
      base_expression = `new URL(${s(base$1)}, location).pathname.slice(0, -1)`;
      if (!assets || assets[0] === "/" && assets !== SVELTE_KIT_ASSETS) {
        assets$1 = base$1;
      }
    } else if (options2.hash_routing) {
      base_expression = "new URL('.', location).pathname.slice(0, -1)";
    }
  }
  if (page_config.ssr) {
    const props = {
      stores: {
        page: writable(null),
        navigating: writable(null),
        updated
      },
      constructors: await Promise.all(
        branch2.map(({ node }) => {
          if (!node.component) {
            throw new Error(`Missing +page.svelte component for route ${event.route.id}`);
          }
          return node.component();
        })
      ),
      form: form_value
    };
    let data2 = {};
    for (let i = 0; i < branch2.length; i += 1) {
      data2 = { ...data2, ...branch2[i].data };
      props[`data_${i}`] = data2;
    }
    props.page = {
      error: error22,
      params: (
        /** @type {Record<string, any>} */
        event.params
      ),
      route: event.route,
      status,
      url: event.url,
      data: data2,
      form: form_value,
      state: {}
    };
    const render_opts = {
      context: /* @__PURE__ */ new Map([
        [
          "__request__",
          {
            page: props.page
          }
        ]
      ])
    };
    const fetch2 = globalThis.fetch;
    try {
      if (BROWSER) ;
      rendered = await with_request_store({ event, state: event_state }, async () => {
        if (relative) override({ base: base$1, assets: assets$1 });
        const maybe_promise = options2.root.render(props, render_opts);
        const rendered2 = options2.async && "then" in maybe_promise ? (
          /** @type {ReturnType<typeof options.root.render> & Promise<any>} */
          maybe_promise.then((r3) => r3)
        ) : maybe_promise;
        if (options2.async) {
          reset();
        }
        const { head: head22, html: html2, css } = options2.async ? await rendered2 : rendered2;
        return { head: head22, html: html2, css };
      });
    } finally {
      reset();
    }
    for (const { node } of branch2) {
      for (const url of node.imports) modulepreloads.add(url);
      for (const url of node.stylesheets) stylesheets5.add(url);
      for (const url of node.fonts) fonts5.add(url);
      if (node.inline_styles && !client.inline) {
        Object.entries(await node.inline_styles()).forEach(([k, v]) => inline_styles.set(k, v));
      }
    }
  } else {
    rendered = { head: "", html: "", css: { code: "", map: null } };
  }
  let head2 = "";
  let body2 = rendered.html;
  const csp = new Csp(options2.csp, {
    prerender: !!state2.prerendering
  });
  const prefixed = /* @__PURE__ */ __name((path) => {
    if (path.startsWith("/")) {
      return base + path;
    }
    return `${assets$1}/${path}`;
  }, "prefixed");
  const style = client.inline ? client.inline?.style : Array.from(inline_styles.values()).join("\n");
  if (style) {
    const attributes2 = [];
    if (csp.style_needs_nonce) attributes2.push(` nonce="${csp.nonce}"`);
    csp.add_style(style);
    head2 += `
	<style${attributes2.join("")}>${style}</style>`;
  }
  for (const dep of stylesheets5) {
    const path = prefixed(dep);
    const attributes2 = ['rel="stylesheet"'];
    if (inline_styles.has(dep)) {
      attributes2.push("disabled", 'media="(max-width: 0)"');
    } else {
      if (resolve_opts.preload({ type: "css", path })) {
        link_headers.add(`<${encodeURI(path)}>; rel="preload"; as="style"; nopush`);
      }
    }
    head2 += `
		<link href="${path}" ${attributes2.join(" ")}>`;
  }
  for (const dep of fonts5) {
    const path = prefixed(dep);
    if (resolve_opts.preload({ type: "font", path })) {
      const ext = dep.slice(dep.lastIndexOf(".") + 1);
      link_tags.add(`<link rel="preload" as="font" type="font/${ext}" href="${path}" crossorigin>`);
      link_headers.add(
        `<${encodeURI(path)}>; rel="preload"; as="font"; type="font/${ext}"; crossorigin; nopush`
      );
    }
  }
  const global = get_global_name(options2);
  const { data, chunks } = data_serializer.get_data(csp);
  if (page_config.ssr && page_config.csr) {
    body2 += `
			${fetched.map(
      (item) => serialize_data(item, resolve_opts.filterSerializedResponseHeaders, !!state2.prerendering)
    ).join("\n			")}`;
  }
  if (page_config.csr) {
    const route = manifest2._.client.routes?.find((r3) => r3.id === event.route.id) ?? null;
    if (client.uses_env_dynamic_public && state2.prerendering) {
      modulepreloads.add(`${app_dir}/env.js`);
    }
    if (!client.inline) {
      const included_modulepreloads = Array.from(modulepreloads, (dep) => prefixed(dep)).filter(
        (path) => resolve_opts.preload({ type: "js", path })
      );
      for (const path of included_modulepreloads) {
        link_headers.add(`<${encodeURI(path)}>; rel="modulepreload"; nopush`);
        if (options2.preload_strategy !== "modulepreload") {
          head2 += `
		<link rel="preload" as="script" crossorigin="anonymous" href="${path}">`;
        } else {
          link_tags.add(`<link rel="modulepreload" href="${path}">`);
        }
      }
    }
    if (state2.prerendering && link_tags.size > 0) {
      head2 += Array.from(link_tags).map((tag) => `
		${tag}`).join("");
    }
    if (manifest2._.client.routes && state2.prerendering && !state2.prerendering.fallback) {
      const pathname = add_resolution_suffix2(event.url.pathname);
      state2.prerendering.dependencies.set(
        pathname,
        create_server_routing_response(route, event.params, new URL(pathname, event.url), manifest2)
      );
    }
    const blocks = [];
    const load_env_eagerly = client.uses_env_dynamic_public && state2.prerendering;
    const properties = [`base: ${base_expression}`];
    if (assets) {
      properties.push(`assets: ${s(assets)}`);
    }
    if (client.uses_env_dynamic_public) {
      properties.push(`env: ${load_env_eagerly ? "null" : s(public_env)}`);
    }
    if (chunks) {
      blocks.push("const deferred = new Map();");
      properties.push(`defer: (id) => new Promise((fulfil, reject) => {
							deferred.set(id, { fulfil, reject });
						})`);
      let app_declaration = "";
      if (Object.keys(options2.hooks.transport).length > 0) {
        if (client.inline) {
          app_declaration = `const app = __sveltekit_${options2.version_hash}.app.app;`;
        } else if (client.app) {
          app_declaration = `const app = await import(${s(prefixed(client.app))});`;
        } else {
          app_declaration = `const { app } = await import(${s(prefixed(client.start))});`;
        }
      }
      const prelude = app_declaration ? `${app_declaration}
							const [data, error] = fn(app);` : `const [data, error] = fn();`;
      properties.push(`resolve: async (id, fn) => {
							${prelude}

							const try_to_resolve = () => {
								if (!deferred.has(id)) {
									setTimeout(try_to_resolve, 0);
									return;
								}
								const { fulfil, reject } = deferred.get(id);
								deferred.delete(id);
								if (error) reject(error);
								else fulfil(data);
							}
							try_to_resolve();
						}`);
    }
    blocks.push(`${global} = {
						${properties.join(",\n						")}
					};`);
    const args = ["element"];
    blocks.push("const element = document.currentScript.parentElement;");
    if (page_config.ssr) {
      const serialized = { form: "null", error: "null" };
      if (form_value) {
        serialized.form = uneval_action_response(
          form_value,
          /** @type {string} */
          event.route.id,
          options2.hooks.transport
        );
      }
      if (error22) {
        serialized.error = uneval(error22);
      }
      const hydrate2 = [
        `node_ids: [${branch2.map(({ node }) => node.index).join(", ")}]`,
        `data: ${data}`,
        `form: ${serialized.form}`,
        `error: ${serialized.error}`
      ];
      if (status !== 200) {
        hydrate2.push(`status: ${status}`);
      }
      if (manifest2._.client.routes) {
        if (route) {
          const stringified = generate_route_object(route, event.url, manifest2).replaceAll(
            "\n",
            "\n							"
          );
          hydrate2.push(`params: ${uneval(event.params)}`, `server_route: ${stringified}`);
        }
      } else if (options2.embedded) {
        hydrate2.push(`params: ${uneval(event.params)}`, `route: ${s(event.route)}`);
      }
      const indent = "	".repeat(load_env_eagerly ? 7 : 6);
      args.push(`{
${indent}	${hydrate2.join(`,
${indent}	`)}
${indent}}`);
    }
    const { remote_data: remote_cache } = event_state;
    let serialized_remote_data = "";
    if (remote_cache) {
      const remote = {};
      for (const [info3, cache] of remote_cache) {
        if (!info3.id) continue;
        for (const key2 in cache) {
          remote[create_remote_cache_key(info3.id, key2)] = await cache[key2];
        }
      }
      const replacer = /* @__PURE__ */ __name((thing) => {
        for (const key2 in options2.hooks.transport) {
          const encoded = options2.hooks.transport[key2].encode(thing);
          if (encoded) {
            return `app.decode('${key2}', ${uneval(encoded, replacer)})`;
          }
        }
      }, "replacer");
      serialized_remote_data = `${global}.data = ${uneval(remote, replacer)};

						`;
    }
    const boot = client.inline ? `${client.inline.script}

					${serialized_remote_data}${global}.app.start(${args.join(", ")});` : client.app ? `Promise.all([
						import(${s(prefixed(client.start))}),
						import(${s(prefixed(client.app))})
					]).then(([kit, app]) => {
						${serialized_remote_data}kit.start(app, ${args.join(", ")});
					});` : `import(${s(prefixed(client.start))}).then((app) => {
						${serialized_remote_data}app.start(${args.join(", ")})
					});`;
    if (load_env_eagerly) {
      blocks.push(`import(${s(`${base$1}/${app_dir}/env.js`)}).then(({ env }) => {
						${global}.env = env;

						${boot.replace(/\n/g, "\n	")}
					});`);
    } else {
      blocks.push(boot);
    }
    if (options2.service_worker) {
      let opts = "";
      if (options2.service_worker_options != null) {
        const service_worker_options = { ...options2.service_worker_options };
        opts = `, ${s(service_worker_options)}`;
      }
      blocks.push(`if ('serviceWorker' in navigator) {
						addEventListener('load', function () {
							navigator.serviceWorker.register('${prefixed("service-worker.js")}'${opts});
						});
					}`);
    }
    const init_app = `
				{
					${blocks.join("\n\n					")}
				}
			`;
    csp.add_script(init_app);
    body2 += `
			<script${csp.script_needs_nonce ? ` nonce="${csp.nonce}"` : ""}>${init_app}<\/script>
		`;
  }
  const headers2 = new Headers({
    "x-sveltekit-page": "true",
    "content-type": "text/html"
  });
  if (state2.prerendering) {
    const http_equiv = [];
    const csp_headers = csp.csp_provider.get_meta();
    if (csp_headers) {
      http_equiv.push(csp_headers);
    }
    if (state2.prerendering.cache) {
      http_equiv.push(`<meta http-equiv="cache-control" content="${state2.prerendering.cache}">`);
    }
    if (http_equiv.length > 0) {
      head2 = http_equiv.join("\n") + head2;
    }
  } else {
    const csp_header = csp.csp_provider.get_header();
    if (csp_header) {
      headers2.set("content-security-policy", csp_header);
    }
    const report_only_header = csp.report_only_provider.get_header();
    if (report_only_header) {
      headers2.set("content-security-policy-report-only", report_only_header);
    }
    if (link_headers.size) {
      headers2.set("link", Array.from(link_headers).join(", "));
    }
  }
  head2 += rendered.head;
  const html = options2.templates.app({
    head: head2,
    body: body2,
    assets: assets$1,
    nonce: (
      /** @type {string} */
      csp.nonce
    ),
    env: public_env
  });
  const transformed = await resolve_opts.transformPageChunk({
    html,
    done: true
  }) || "";
  if (!chunks) {
    headers2.set("etag", `"${hash(transformed)}"`);
  }
  return !chunks ? text(transformed, {
    status,
    headers: headers2
  }) : new Response(
    new ReadableStream({
      async start(controller2) {
        controller2.enqueue(text_encoder2.encode(transformed + "\n"));
        for await (const chunk of chunks) {
          if (chunk.length) controller2.enqueue(text_encoder2.encode(chunk));
        }
        controller2.close();
      },
      type: "bytes"
    }),
    {
      headers: headers2
    }
  );
}
__name(render_response, "render_response");
var PageNodes = class {
  static {
    __name(this, "PageNodes");
  }
  data;
  /**
   * @param {Array<import('types').SSRNode | undefined>} nodes
   */
  constructor(nodes) {
    this.data = nodes;
  }
  layouts() {
    return this.data.slice(0, -1);
  }
  page() {
    return this.data.at(-1);
  }
  validate() {
    for (const layout of this.layouts()) {
      if (layout) {
        validate_layout_server_exports(
          layout.server,
          /** @type {string} */
          layout.server_id
        );
        validate_layout_exports(
          layout.universal,
          /** @type {string} */
          layout.universal_id
        );
      }
    }
    const page2 = this.page();
    if (page2) {
      validate_page_server_exports(
        page2.server,
        /** @type {string} */
        page2.server_id
      );
      validate_page_exports(
        page2.universal,
        /** @type {string} */
        page2.universal_id
      );
    }
  }
  /**
   * @template {'prerender' | 'ssr' | 'csr' | 'trailingSlash'} Option
   * @param {Option} option
   * @returns {Value | undefined}
   */
  #get_option(option) {
    return this.data.reduce(
      (value, node) => {
        return node?.universal?.[option] ?? node?.server?.[option] ?? value;
      },
      /** @type {Value | undefined} */
      void 0
    );
  }
  csr() {
    return this.#get_option("csr") ?? true;
  }
  ssr() {
    return this.#get_option("ssr") ?? true;
  }
  prerender() {
    return this.#get_option("prerender") ?? false;
  }
  trailing_slash() {
    return this.#get_option("trailingSlash") ?? "never";
  }
  get_config() {
    let current2 = {};
    for (const node of this.data) {
      if (!node?.universal?.config && !node?.server?.config) continue;
      current2 = {
        ...current2,
        // TODO: should we override the server config value with the universal value similar to other page options?
        ...node?.universal?.config,
        ...node?.server?.config
      };
    }
    return Object.keys(current2).length ? current2 : void 0;
  }
  should_prerender_data() {
    return this.data.some(
      // prerender in case of trailingSlash because the client retrieves that value from the server
      (node) => node?.server?.load || node?.server?.trailingSlash !== void 0
    );
  }
};
async function respond_with_error({
  event,
  event_state,
  options: options2,
  manifest: manifest2,
  state: state2,
  status,
  error: error22,
  resolve_opts
}) {
  if (event.request.headers.get("x-sveltekit-error")) {
    return static_error_page(
      options2,
      status,
      /** @type {Error} */
      error22.message
    );
  }
  const fetched = [];
  try {
    const branch2 = [];
    const default_layout = await manifest2._.nodes[0]();
    const nodes = new PageNodes([default_layout]);
    const ssr = nodes.ssr();
    const csr = nodes.csr();
    const data_serializer = server_data_serializer(event, event_state, options2);
    if (ssr) {
      state2.error = true;
      const server_data_promise = load_server_data({
        event,
        event_state,
        state: state2,
        node: default_layout,
        // eslint-disable-next-line @typescript-eslint/require-await
        parent: /* @__PURE__ */ __name(async () => ({}), "parent")
      });
      const server_data = await server_data_promise;
      data_serializer.add_node(0, server_data);
      const data = await load_data({
        event,
        event_state,
        fetched,
        node: default_layout,
        // eslint-disable-next-line @typescript-eslint/require-await
        parent: /* @__PURE__ */ __name(async () => ({}), "parent"),
        resolve_opts,
        server_data_promise,
        state: state2,
        csr
      });
      branch2.push(
        {
          node: default_layout,
          server_data,
          data
        },
        {
          node: await manifest2._.nodes[1](),
          // 1 is always the root error
          data: null,
          server_data: null
        }
      );
    }
    return await render_response({
      options: options2,
      manifest: manifest2,
      state: state2,
      page_config: {
        ssr,
        csr
      },
      status,
      error: await handle_error_and_jsonify(event, event_state, options2, error22),
      branch: branch2,
      fetched,
      event,
      event_state,
      resolve_opts,
      data_serializer
    });
  } catch (e3) {
    if (e3 instanceof Redirect) {
      return redirect_response(e3.status, e3.location);
    }
    return static_error_page(
      options2,
      get_status(e3),
      (await handle_error_and_jsonify(event, event_state, options2, e3)).message
    );
  }
}
__name(respond_with_error, "respond_with_error");
async function handle_remote_call(event, state2, options2, manifest2, id) {
  return record_span({
    name: "sveltekit.remote.call",
    attributes: {},
    fn: /* @__PURE__ */ __name((current2) => {
      const traced_event = merge_tracing(event, current2);
      return with_request_store(
        { event: traced_event, state: state2 },
        () => handle_remote_call_internal(traced_event, state2, options2, manifest2, id)
      );
    }, "fn")
  });
}
__name(handle_remote_call, "handle_remote_call");
async function handle_remote_call_internal(event, state2, options2, manifest2, id) {
  const [hash2, name, additional_args] = id.split("/");
  const remotes = manifest2._.remotes;
  if (!remotes[hash2]) error3(404);
  const module = await remotes[hash2]();
  const fn = module.default[name];
  if (!fn) error3(404);
  const info3 = fn.__;
  const transport = options2.hooks.transport;
  event.tracing.current.setAttributes({
    "sveltekit.remote.call.type": info3.type,
    "sveltekit.remote.call.name": info3.name
  });
  let form_client_refreshes;
  try {
    if (info3.type === "query_batch") {
      if (event.request.method !== "POST") {
        throw new SvelteKitError(
          405,
          "Method Not Allowed",
          `\`query.batch\` functions must be invoked via POST request, not ${event.request.method}`
        );
      }
      const { payloads } = await event.request.json();
      const args = payloads.map((payload2) => parse_remote_arg(payload2, transport));
      const get_result = await with_request_store({ event, state: state2 }, () => info3.run(args));
      const results = await Promise.all(
        args.map(async (arg, i) => {
          try {
            return { type: "result", data: get_result(arg, i) };
          } catch (error22) {
            return {
              type: "error",
              error: await handle_error_and_jsonify(event, state2, options2, error22),
              status: error22 instanceof HttpError || error22 instanceof SvelteKitError ? error22.status : 500
            };
          }
        })
      );
      return json(
        /** @type {RemoteFunctionResponse} */
        {
          type: "result",
          result: stringify2(results, transport)
        }
      );
    }
    if (info3.type === "form") {
      if (event.request.method !== "POST") {
        throw new SvelteKitError(
          405,
          "Method Not Allowed",
          `\`form\` functions must be invoked via POST request, not ${event.request.method}`
        );
      }
      if (!is_form_content_type(event.request)) {
        throw new SvelteKitError(
          415,
          "Unsupported Media Type",
          `\`form\` functions expect form-encoded data \u2014 received ${event.request.headers.get(
            "content-type"
          )}`
        );
      }
      const form_data = await event.request.formData();
      form_client_refreshes = /** @type {string[]} */
      JSON.parse(
        /** @type {string} */
        form_data.get("sveltekit:remote_refreshes") ?? "[]"
      );
      form_data.delete("sveltekit:remote_refreshes");
      if (additional_args) {
        form_data.set("sveltekit:id", decodeURIComponent(additional_args));
      }
      const fn2 = info3.fn;
      const data2 = await with_request_store({ event, state: state2 }, () => fn2(form_data));
      return json(
        /** @type {RemoteFunctionResponse} */
        {
          type: "result",
          result: stringify2(data2, transport),
          refreshes: data2.issues ? {} : await serialize_refreshes(form_client_refreshes)
        }
      );
    }
    if (info3.type === "command") {
      const { payload: payload2, refreshes } = await event.request.json();
      const arg = parse_remote_arg(payload2, transport);
      const data2 = await with_request_store({ event, state: state2 }, () => fn(arg));
      return json(
        /** @type {RemoteFunctionResponse} */
        {
          type: "result",
          result: stringify2(data2, transport),
          refreshes: await serialize_refreshes(refreshes)
        }
      );
    }
    const payload = info3.type === "prerender" ? additional_args : (
      /** @type {string} */
      // new URL(...) necessary because we're hiding the URL from the user in the event object
      new URL(event.request.url).searchParams.get("payload")
    );
    const data = await with_request_store(
      { event, state: state2 },
      () => fn(parse_remote_arg(payload, transport))
    );
    return json(
      /** @type {RemoteFunctionResponse} */
      {
        type: "result",
        result: stringify2(data, transport)
      }
    );
  } catch (error22) {
    if (error22 instanceof Redirect) {
      return json(
        /** @type {RemoteFunctionResponse} */
        {
          type: "redirect",
          location: error22.location,
          refreshes: await serialize_refreshes(form_client_refreshes ?? [])
        }
      );
    }
    const status = error22 instanceof HttpError || error22 instanceof SvelteKitError ? error22.status : 500;
    return json(
      /** @type {RemoteFunctionResponse} */
      {
        type: "error",
        error: await handle_error_and_jsonify(event, state2, options2, error22),
        status
      },
      {
        // By setting a non-200 during prerendering we fail the prerender process (unless handleHttpError handles it).
        // Errors at runtime will be passed to the client and are handled there
        status: state2.prerendering ? status : void 0,
        headers: {
          "cache-control": "private, no-store"
        }
      }
    );
  }
  async function serialize_refreshes(client_refreshes) {
    const refreshes = state2.refreshes ?? {};
    for (const key2 of client_refreshes) {
      if (refreshes[key2] !== void 0) continue;
      const [hash3, name2, payload] = key2.split("/");
      const loader = manifest2._.remotes[hash3];
      const fn2 = (await loader?.())?.default?.[name2];
      if (!fn2) error3(400, "Bad Request");
      refreshes[key2] = with_request_store(
        { event, state: state2 },
        () => fn2(parse_remote_arg(payload, transport))
      );
    }
    if (Object.keys(refreshes).length === 0) {
      return void 0;
    }
    return stringify2(
      Object.fromEntries(
        await Promise.all(
          Object.entries(refreshes).map(async ([key2, promise]) => [key2, await promise])
        )
      ),
      transport
    );
  }
  __name(serialize_refreshes, "serialize_refreshes");
}
__name(handle_remote_call_internal, "handle_remote_call_internal");
async function handle_remote_form_post(event, state2, manifest2, id) {
  return record_span({
    name: "sveltekit.remote.form.post",
    attributes: {},
    fn: /* @__PURE__ */ __name((current2) => {
      const traced_event = merge_tracing(event, current2);
      return with_request_store(
        { event: traced_event, state: state2 },
        () => handle_remote_form_post_internal(traced_event, state2, manifest2, id)
      );
    }, "fn")
  });
}
__name(handle_remote_form_post, "handle_remote_form_post");
async function handle_remote_form_post_internal(event, state2, manifest2, id) {
  const [hash2, name, action_id] = id.split("/");
  const remotes = manifest2._.remotes;
  const module = await remotes[hash2]?.();
  let form = (
    /** @type {RemoteForm<any, any>} */
    module?.default[name]
  );
  if (!form) {
    event.setHeaders({
      // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/405
      // "The server must generate an Allow header field in a 405 status code response"
      allow: "GET"
    });
    return {
      type: "error",
      error: new SvelteKitError(
        405,
        "Method Not Allowed",
        `POST method not allowed. No form actions exist for ${"this page"}`
      )
    };
  }
  if (action_id) {
    form = with_request_store({ event, state: state2 }, () => form.for(JSON.parse(action_id)));
  }
  try {
    const form_data = await event.request.formData();
    const fn = (
      /** @type {RemoteInfo & { type: 'form' }} */
      /** @type {any} */
      form.__.fn
    );
    if (action_id && !form_data.has("id")) {
      form_data.set("sveltekit:id", decodeURIComponent(action_id));
    }
    await with_request_store({ event, state: state2 }, () => fn(form_data));
    return {
      type: "success",
      status: 200
    };
  } catch (e3) {
    const err = normalize_error(e3);
    if (err instanceof Redirect) {
      return {
        type: "redirect",
        status: err.status,
        location: err.location
      };
    }
    return {
      type: "error",
      error: check_incorrect_fail_use(err)
    };
  }
}
__name(handle_remote_form_post_internal, "handle_remote_form_post_internal");
function get_remote_id(url) {
  return url.pathname.startsWith(`${base}/${app_dir}/remote/`) && url.pathname.replace(`${base}/${app_dir}/remote/`, "");
}
__name(get_remote_id, "get_remote_id");
function get_remote_action(url) {
  return url.searchParams.get("/remote");
}
__name(get_remote_action, "get_remote_action");
var MAX_DEPTH = 10;
async function render_page(event, event_state, page2, options2, manifest2, state2, nodes, resolve_opts) {
  if (state2.depth > MAX_DEPTH) {
    return text(`Not found: ${event.url.pathname}`, {
      status: 404
      // TODO in some cases this should be 500. not sure how to differentiate
    });
  }
  if (is_action_json_request(event)) {
    const node = await manifest2._.nodes[page2.leaf]();
    return handle_action_json_request(event, event_state, options2, node?.server);
  }
  try {
    const leaf_node = (
      /** @type {import('types').SSRNode} */
      nodes.page()
    );
    let status = 200;
    let action_result = void 0;
    if (is_action_request(event)) {
      const remote_id = get_remote_action(event.url);
      if (remote_id) {
        action_result = await handle_remote_form_post(event, event_state, manifest2, remote_id);
      } else {
        action_result = await handle_action_request(event, event_state, leaf_node.server);
      }
      if (action_result?.type === "redirect") {
        return redirect_response(action_result.status, action_result.location);
      }
      if (action_result?.type === "error") {
        status = get_status(action_result.error);
      }
      if (action_result?.type === "failure") {
        status = action_result.status;
      }
    }
    const should_prerender = nodes.prerender();
    if (should_prerender) {
      const mod = leaf_node.server;
      if (mod?.actions) {
        throw new Error("Cannot prerender pages with actions");
      }
    } else if (state2.prerendering) {
      return new Response(void 0, {
        status: 204
      });
    }
    state2.prerender_default = should_prerender;
    const should_prerender_data = nodes.should_prerender_data();
    const data_pathname = add_data_suffix2(event.url.pathname);
    const fetched = [];
    const ssr = nodes.ssr();
    const csr = nodes.csr();
    if (ssr === false && !(state2.prerendering && should_prerender_data)) {
      if (BROWSER && action_result && !event.request.headers.has("x-sveltekit-action")) ;
      return await render_response({
        branch: [],
        fetched,
        page_config: {
          ssr: false,
          csr
        },
        status,
        error: null,
        event,
        event_state,
        options: options2,
        manifest: manifest2,
        state: state2,
        resolve_opts,
        data_serializer: server_data_serializer(event, event_state, options2)
      });
    }
    const branch2 = [];
    let load_error = null;
    const data_serializer = server_data_serializer(event, event_state, options2);
    const data_serializer_json = state2.prerendering && should_prerender_data ? server_data_serializer_json(event, event_state, options2) : null;
    const server_promises = nodes.data.map((node, i) => {
      if (load_error) {
        throw load_error;
      }
      return Promise.resolve().then(async () => {
        try {
          if (node === leaf_node && action_result?.type === "error") {
            throw action_result.error;
          }
          const server_data = await load_server_data({
            event,
            event_state,
            state: state2,
            node,
            parent: /* @__PURE__ */ __name(async () => {
              const data = {};
              for (let j = 0; j < i; j += 1) {
                const parent = await server_promises[j];
                if (parent) Object.assign(data, parent.data);
              }
              return data;
            }, "parent")
          });
          if (node) {
            data_serializer.add_node(i, server_data);
          }
          data_serializer_json?.add_node(i, server_data);
          return server_data;
        } catch (e3) {
          load_error = /** @type {Error} */
          e3;
          throw load_error;
        }
      });
    });
    const load_promises = nodes.data.map((node, i) => {
      if (load_error) throw load_error;
      return Promise.resolve().then(async () => {
        try {
          return await load_data({
            event,
            event_state,
            fetched,
            node,
            parent: /* @__PURE__ */ __name(async () => {
              const data = {};
              for (let j = 0; j < i; j += 1) {
                Object.assign(data, await load_promises[j]);
              }
              return data;
            }, "parent"),
            resolve_opts,
            server_data_promise: server_promises[i],
            state: state2,
            csr
          });
        } catch (e3) {
          load_error = /** @type {Error} */
          e3;
          throw load_error;
        }
      });
    });
    for (const p of server_promises) p.catch(() => {
    });
    for (const p of load_promises) p.catch(() => {
    });
    for (let i = 0; i < nodes.data.length; i += 1) {
      const node = nodes.data[i];
      if (node) {
        try {
          const server_data = await server_promises[i];
          const data = await load_promises[i];
          branch2.push({ node, server_data, data });
        } catch (e3) {
          const err = normalize_error(e3);
          if (err instanceof Redirect) {
            if (state2.prerendering && should_prerender_data) {
              const body2 = JSON.stringify({
                type: "redirect",
                location: err.location
              });
              state2.prerendering.dependencies.set(data_pathname, {
                response: text(body2),
                body: body2
              });
            }
            return redirect_response(err.status, err.location);
          }
          const status2 = get_status(err);
          const error22 = await handle_error_and_jsonify(event, event_state, options2, err);
          while (i--) {
            if (page2.errors[i]) {
              const index5 = (
                /** @type {number} */
                page2.errors[i]
              );
              const node2 = await manifest2._.nodes[index5]();
              let j = i;
              while (!branch2[j]) j -= 1;
              data_serializer.set_max_nodes(j + 1);
              const layouts = compact(branch2.slice(0, j + 1));
              const nodes2 = new PageNodes(layouts.map((layout) => layout.node));
              return await render_response({
                event,
                event_state,
                options: options2,
                manifest: manifest2,
                state: state2,
                resolve_opts,
                page_config: {
                  ssr: nodes2.ssr(),
                  csr: nodes2.csr()
                },
                status: status2,
                error: error22,
                branch: layouts.concat({
                  node: node2,
                  data: null,
                  server_data: null
                }),
                fetched,
                data_serializer
              });
            }
          }
          return static_error_page(options2, status2, error22.message);
        }
      } else {
        branch2.push(null);
      }
    }
    if (state2.prerendering && data_serializer_json) {
      let { data, chunks } = data_serializer_json.get_data();
      if (chunks) {
        for await (const chunk of chunks) {
          data += chunk;
        }
      }
      state2.prerendering.dependencies.set(data_pathname, {
        response: text(data),
        body: data
      });
    }
    return await render_response({
      event,
      event_state,
      options: options2,
      manifest: manifest2,
      state: state2,
      resolve_opts,
      page_config: {
        csr,
        ssr
      },
      status,
      error: null,
      branch: ssr === false ? [] : compact(branch2),
      action_result,
      fetched,
      data_serializer: ssr === false ? server_data_serializer(event, event_state, options2) : data_serializer
    });
  } catch (e3) {
    return await respond_with_error({
      event,
      event_state,
      options: options2,
      manifest: manifest2,
      state: state2,
      status: 500,
      error: e3,
      resolve_opts
    });
  }
}
__name(render_page, "render_page");
function once2(fn) {
  let done = false;
  let result;
  return () => {
    if (done) return result;
    done = true;
    return result = fn();
  };
}
__name(once2, "once");
async function render_data(event, event_state, route, options2, manifest2, state2, invalidated_data_nodes, trailing_slash) {
  if (!route.page) {
    return new Response(void 0, {
      status: 404
    });
  }
  try {
    const node_ids = [...route.page.layouts, route.page.leaf];
    const invalidated = invalidated_data_nodes ?? node_ids.map(() => true);
    let aborted = false;
    const url = new URL(event.url);
    url.pathname = normalize_path(url.pathname, trailing_slash);
    const new_event = { ...event, url };
    const functions = node_ids.map((n2, i) => {
      return once2(async () => {
        try {
          if (aborted) {
            return (
              /** @type {import('types').ServerDataSkippedNode} */
              {
                type: "skip"
              }
            );
          }
          const node = n2 == void 0 ? n2 : await manifest2._.nodes[n2]();
          return load_server_data({
            event: new_event,
            event_state,
            state: state2,
            node,
            parent: /* @__PURE__ */ __name(async () => {
              const data2 = {};
              for (let j = 0; j < i; j += 1) {
                const parent = (
                  /** @type {import('types').ServerDataNode | null} */
                  await functions[j]()
                );
                if (parent) {
                  Object.assign(data2, parent.data);
                }
              }
              return data2;
            }, "parent")
          });
        } catch (e3) {
          aborted = true;
          throw e3;
        }
      });
    });
    const promises = functions.map(async (fn, i) => {
      if (!invalidated[i]) {
        return (
          /** @type {import('types').ServerDataSkippedNode} */
          {
            type: "skip"
          }
        );
      }
      return fn();
    });
    let length = promises.length;
    const nodes = await Promise.all(
      promises.map(
        (p, i) => p.catch(async (error22) => {
          if (error22 instanceof Redirect) {
            throw error22;
          }
          length = Math.min(length, i + 1);
          return (
            /** @type {import('types').ServerErrorNode} */
            {
              type: "error",
              error: await handle_error_and_jsonify(event, event_state, options2, error22),
              status: error22 instanceof HttpError || error22 instanceof SvelteKitError ? error22.status : void 0
            }
          );
        })
      )
    );
    const data_serializer = server_data_serializer_json(event, event_state, options2);
    for (let i = 0; i < nodes.length; i++) data_serializer.add_node(i, nodes[i]);
    const { data, chunks } = data_serializer.get_data();
    if (!chunks) {
      return json_response(data);
    }
    return new Response(
      new ReadableStream({
        async start(controller2) {
          controller2.enqueue(text_encoder2.encode(data));
          for await (const chunk of chunks) {
            controller2.enqueue(text_encoder2.encode(chunk));
          }
          controller2.close();
        },
        type: "bytes"
      }),
      {
        headers: {
          // we use a proprietary content type to prevent buffering.
          // the `text` prefix makes it inspectable
          "content-type": "text/sveltekit-data",
          "cache-control": "private, no-store"
        }
      }
    );
  } catch (e3) {
    const error22 = normalize_error(e3);
    if (error22 instanceof Redirect) {
      return redirect_json_response(error22);
    } else {
      return json_response(await handle_error_and_jsonify(event, event_state, options2, error22), 500);
    }
  }
}
__name(render_data, "render_data");
function json_response(json2, status = 200) {
  return text(typeof json2 === "string" ? json2 : JSON.stringify(json2), {
    status,
    headers: {
      "content-type": "application/json",
      "cache-control": "private, no-store"
    }
  });
}
__name(json_response, "json_response");
function redirect_json_response(redirect) {
  return json_response(
    /** @type {import('types').ServerRedirectNode} */
    {
      type: "redirect",
      location: redirect.location
    }
  );
}
__name(redirect_json_response, "redirect_json_response");
var INVALID_COOKIE_CHARACTER_REGEX = /[\x00-\x1F\x7F()<>@,;:"/[\]?={} \t]/;
function validate_options(options2) {
  if (options2?.path === void 0) {
    throw new Error("You must specify a `path` when setting, deleting or serializing cookies");
  }
}
__name(validate_options, "validate_options");
function generate_cookie_key(domain2, path, name) {
  return `${domain2 || ""}${path}?${encodeURIComponent(name)}`;
}
__name(generate_cookie_key, "generate_cookie_key");
function get_cookies(request, url) {
  const header = request.headers.get("cookie") ?? "";
  const initial_cookies = (0, import_cookie.parse)(header, { decode: /* @__PURE__ */ __name((value) => value, "decode") });
  let normalized_url;
  const new_cookies = /* @__PURE__ */ new Map();
  const defaults = {
    httpOnly: true,
    sameSite: "lax",
    secure: url.hostname === "localhost" && url.protocol === "http:" ? false : true
  };
  const cookies = {
    // The JSDoc param annotations appearing below for get, set and delete
    // are necessary to expose the `cookie` library types to
    // typescript users. `@type {import('@sveltejs/kit').Cookies}` above is not
    // sufficient to do so.
    /**
     * @param {string} name
     * @param {import('cookie').CookieParseOptions} [opts]
     */
    get(name, opts) {
      const best_match = Array.from(new_cookies.values()).filter((c2) => {
        return c2.name === name && domain_matches(url.hostname, c2.options.domain) && path_matches(url.pathname, c2.options.path);
      }).sort((a, b) => b.options.path.length - a.options.path.length)[0];
      if (best_match) {
        return best_match.options.maxAge === 0 ? void 0 : best_match.value;
      }
      const req_cookies = (0, import_cookie.parse)(header, { decode: opts?.decode });
      const cookie = req_cookies[name];
      return cookie;
    },
    /**
     * @param {import('cookie').CookieParseOptions} [opts]
     */
    getAll(opts) {
      const cookies2 = (0, import_cookie.parse)(header, { decode: opts?.decode });
      const lookup = /* @__PURE__ */ new Map();
      for (const c2 of new_cookies.values()) {
        if (domain_matches(url.hostname, c2.options.domain) && path_matches(url.pathname, c2.options.path)) {
          const existing = lookup.get(c2.name);
          if (!existing || c2.options.path.length > existing.options.path.length) {
            lookup.set(c2.name, c2);
          }
        }
      }
      for (const c2 of lookup.values()) {
        cookies2[c2.name] = c2.value;
      }
      return Object.entries(cookies2).map(([name, value]) => ({ name, value }));
    },
    /**
     * @param {string} name
     * @param {string} value
     * @param {import('./page/types.js').Cookie['options']} options
     */
    set(name, value, options2) {
      const illegal_characters = name.match(INVALID_COOKIE_CHARACTER_REGEX);
      if (illegal_characters) {
        console.warn(
          `The cookie name "${name}" will be invalid in SvelteKit 3.0 as it contains ${illegal_characters.join(
            " and "
          )}. See RFC 2616 for more details https://datatracker.ietf.org/doc/html/rfc2616#section-2.2`
        );
      }
      validate_options(options2);
      set_internal(name, value, { ...defaults, ...options2 });
    },
    /**
     * @param {string} name
     *  @param {import('./page/types.js').Cookie['options']} options
     */
    delete(name, options2) {
      validate_options(options2);
      cookies.set(name, "", { ...options2, maxAge: 0 });
    },
    /**
     * @param {string} name
     * @param {string} value
     *  @param {import('./page/types.js').Cookie['options']} options
     */
    serialize(name, value, options2) {
      validate_options(options2);
      let path = options2.path;
      if (!options2.domain || options2.domain === url.hostname) {
        if (!normalized_url) {
          throw new Error("Cannot serialize cookies until after the route is determined");
        }
        path = resolve(normalized_url, path);
      }
      return (0, import_cookie.serialize)(name, value, { ...defaults, ...options2, path });
    }
  };
  function get_cookie_header(destination, header2) {
    const combined_cookies = {
      // cookies sent by the user agent have lowest precedence
      ...initial_cookies
    };
    for (const cookie of new_cookies.values()) {
      if (!domain_matches(destination.hostname, cookie.options.domain)) continue;
      if (!path_matches(destination.pathname, cookie.options.path)) continue;
      const encoder = cookie.options.encode || encodeURIComponent;
      combined_cookies[cookie.name] = encoder(cookie.value);
    }
    if (header2) {
      const parsed = (0, import_cookie.parse)(header2, { decode: /* @__PURE__ */ __name((value) => value, "decode") });
      for (const name in parsed) {
        combined_cookies[name] = parsed[name];
      }
    }
    return Object.entries(combined_cookies).map(([name, value]) => `${name}=${value}`).join("; ");
  }
  __name(get_cookie_header, "get_cookie_header");
  const internal_queue = [];
  function set_internal(name, value, options2) {
    if (!normalized_url) {
      internal_queue.push(() => set_internal(name, value, options2));
      return;
    }
    let path = options2.path;
    if (!options2.domain || options2.domain === url.hostname) {
      path = resolve(normalized_url, path);
    }
    const cookie_key = generate_cookie_key(options2.domain, path, name);
    const cookie = { name, value, options: { ...options2, path } };
    new_cookies.set(cookie_key, cookie);
  }
  __name(set_internal, "set_internal");
  function set_trailing_slash(trailing_slash) {
    normalized_url = normalize_path(url.pathname, trailing_slash);
    internal_queue.forEach((fn) => fn());
  }
  __name(set_trailing_slash, "set_trailing_slash");
  return { cookies, new_cookies, get_cookie_header, set_internal, set_trailing_slash };
}
__name(get_cookies, "get_cookies");
function domain_matches(hostname, constraint) {
  if (!constraint) return true;
  const normalized = constraint[0] === "." ? constraint.slice(1) : constraint;
  if (hostname === normalized) return true;
  return hostname.endsWith("." + normalized);
}
__name(domain_matches, "domain_matches");
function path_matches(path, constraint) {
  if (!constraint) return true;
  const normalized = constraint.endsWith("/") ? constraint.slice(0, -1) : constraint;
  if (path === normalized) return true;
  return path.startsWith(normalized + "/");
}
__name(path_matches, "path_matches");
function add_cookies_to_headers(headers2, cookies) {
  for (const new_cookie of cookies) {
    const { name, value, options: options2 } = new_cookie;
    headers2.append("set-cookie", (0, import_cookie.serialize)(name, value, options2));
    if (options2.path.endsWith(".html")) {
      const path = add_data_suffix2(options2.path);
      headers2.append("set-cookie", (0, import_cookie.serialize)(name, value, { ...options2, path }));
    }
  }
}
__name(add_cookies_to_headers, "add_cookies_to_headers");
function create_fetch({ event, options: options2, manifest: manifest2, state: state2, get_cookie_header, set_internal }) {
  const server_fetch = /* @__PURE__ */ __name(async (info3, init2) => {
    const original_request = normalize_fetch_input(info3, init2, event.url);
    let mode = (info3 instanceof Request ? info3.mode : init2?.mode) ?? "cors";
    let credentials = (info3 instanceof Request ? info3.credentials : init2?.credentials) ?? "same-origin";
    return options2.hooks.handleFetch({
      event,
      request: original_request,
      fetch: /* @__PURE__ */ __name(async (info22, init3) => {
        const request = normalize_fetch_input(info22, init3, event.url);
        const url = new URL(request.url);
        if (!request.headers.has("origin")) {
          request.headers.set("origin", event.url.origin);
        }
        if (info22 !== original_request) {
          mode = (info22 instanceof Request ? info22.mode : init3?.mode) ?? "cors";
          credentials = (info22 instanceof Request ? info22.credentials : init3?.credentials) ?? "same-origin";
        }
        if ((request.method === "GET" || request.method === "HEAD") && (mode === "no-cors" && url.origin !== event.url.origin || url.origin === event.url.origin)) {
          request.headers.delete("origin");
        }
        if (url.origin !== event.url.origin) {
          if (`.${url.hostname}`.endsWith(`.${event.url.hostname}`) && credentials !== "omit") {
            const cookie = get_cookie_header(url, request.headers.get("cookie"));
            if (cookie) request.headers.set("cookie", cookie);
          }
          return fetch(request);
        }
        const prefix = assets || base;
        const decoded = decodeURIComponent(url.pathname);
        const filename = (decoded.startsWith(prefix) ? decoded.slice(prefix.length) : decoded).slice(1);
        const filename_html = `${filename}/index.html`;
        const is_asset = manifest2.assets.has(filename) || filename in manifest2._.server_assets;
        const is_asset_html = manifest2.assets.has(filename_html) || filename_html in manifest2._.server_assets;
        if (is_asset || is_asset_html) {
          const file = is_asset ? filename : filename_html;
          if (state2.read) {
            const type = is_asset ? manifest2.mimeTypes[filename.slice(filename.lastIndexOf("."))] : "text/html";
            return new Response(state2.read(file), {
              headers: type ? { "content-type": type } : {}
            });
          } else if (read_implementation && file in manifest2._.server_assets) {
            const length = manifest2._.server_assets[file];
            const type = manifest2.mimeTypes[file.slice(file.lastIndexOf("."))];
            return new Response(read_implementation(file), {
              headers: {
                "Content-Length": "" + length,
                "Content-Type": type
              }
            });
          }
          return await fetch(request);
        }
        if (has_prerendered_path(manifest2, base + decoded)) {
          return await fetch(request);
        }
        if (credentials !== "omit") {
          const cookie = get_cookie_header(url, request.headers.get("cookie"));
          if (cookie) {
            request.headers.set("cookie", cookie);
          }
          const authorization = event.request.headers.get("authorization");
          if (authorization && !request.headers.has("authorization")) {
            request.headers.set("authorization", authorization);
          }
        }
        if (!request.headers.has("accept")) {
          request.headers.set("accept", "*/*");
        }
        if (!request.headers.has("accept-language")) {
          request.headers.set(
            "accept-language",
            /** @type {string} */
            event.request.headers.get("accept-language")
          );
        }
        const response = await internal_fetch(request, options2, manifest2, state2);
        const set_cookie = response.headers.get("set-cookie");
        if (set_cookie) {
          for (const str of set_cookie_parser.splitCookiesString(set_cookie)) {
            const { name, value, ...options3 } = set_cookie_parser.parseString(str, {
              decodeValues: false
            });
            const path = options3.path ?? (url.pathname.split("/").slice(0, -1).join("/") || "/");
            set_internal(name, value, {
              path,
              encode: /* @__PURE__ */ __name((value2) => value2, "encode"),
              .../** @type {import('cookie').CookieSerializeOptions} */
              options3
            });
          }
        }
        return response;
      }, "fetch")
    });
  }, "server_fetch");
  return (input, init2) => {
    const response = server_fetch(input, init2);
    response.catch(() => {
    });
    return response;
  };
}
__name(create_fetch, "create_fetch");
function normalize_fetch_input(info3, init2, url) {
  if (info3 instanceof Request) {
    return info3;
  }
  return new Request(typeof info3 === "string" ? new URL(info3, url) : info3, init2);
}
__name(normalize_fetch_input, "normalize_fetch_input");
async function internal_fetch(request, options2, manifest2, state2) {
  if (request.signal) {
    if (request.signal.aborted) {
      throw new DOMException("The operation was aborted.", "AbortError");
    }
    let remove_abort_listener = /* @__PURE__ */ __name(() => {
    }, "remove_abort_listener");
    const abort_promise = new Promise((_, reject) => {
      const on_abort = /* @__PURE__ */ __name(() => {
        reject(new DOMException("The operation was aborted.", "AbortError"));
      }, "on_abort");
      request.signal.addEventListener("abort", on_abort, { once: true });
      remove_abort_listener = /* @__PURE__ */ __name(() => request.signal.removeEventListener("abort", on_abort), "remove_abort_listener");
    });
    const result = await Promise.race([
      respond(request, options2, manifest2, {
        ...state2,
        depth: state2.depth + 1
      }),
      abort_promise
    ]);
    remove_abort_listener();
    return result;
  } else {
    return await respond(request, options2, manifest2, {
      ...state2,
      depth: state2.depth + 1
    });
  }
}
__name(internal_fetch, "internal_fetch");
var body;
var etag;
var headers;
function get_public_env(request) {
  body ??= `export const env=${JSON.stringify(public_env)}`;
  etag ??= `W/${Date.now()}`;
  headers ??= new Headers({
    "content-type": "application/javascript; charset=utf-8",
    etag
  });
  if (request.headers.get("if-none-match") === etag) {
    return new Response(void 0, { status: 304, headers });
  }
  return new Response(body, { headers });
}
__name(get_public_env, "get_public_env");
var default_transform = /* @__PURE__ */ __name(({ html }) => html, "default_transform");
var default_filter = /* @__PURE__ */ __name(() => false, "default_filter");
var default_preload = /* @__PURE__ */ __name(({ type }) => type === "js" || type === "css", "default_preload");
var page_methods = /* @__PURE__ */ new Set(["GET", "HEAD", "POST"]);
var allowed_page_methods = /* @__PURE__ */ new Set(["GET", "HEAD", "OPTIONS"]);
var respond = propagate_context(internal_respond);
async function internal_respond(request, options2, manifest2, state2) {
  const url = new URL(request.url);
  const is_route_resolution_request = has_resolution_suffix2(url.pathname);
  const is_data_request = has_data_suffix2(url.pathname);
  const remote_id = get_remote_id(url);
  {
    const request_origin = request.headers.get("origin");
    if (remote_id) {
      if (request.method !== "GET" && request_origin !== url.origin) {
        const message = "Cross-site remote requests are forbidden";
        return json({ message }, { status: 403 });
      }
    } else if (options2.csrf_check_origin) {
      const forbidden = is_form_content_type(request) && (request.method === "POST" || request.method === "PUT" || request.method === "PATCH" || request.method === "DELETE") && request_origin !== url.origin && (!request_origin || !options2.csrf_trusted_origins.includes(request_origin));
      if (forbidden) {
        const message = `Cross-site ${request.method} form submissions are forbidden`;
        const opts = { status: 403 };
        if (request.headers.get("accept") === "application/json") {
          return json({ message }, opts);
        }
        return text(message, opts);
      }
    }
  }
  if (options2.hash_routing && url.pathname !== base + "/" && url.pathname !== "/[fallback]") {
    return text("Not found", { status: 404 });
  }
  let invalidated_data_nodes;
  if (is_route_resolution_request) {
    url.pathname = strip_resolution_suffix2(url.pathname);
  } else if (is_data_request) {
    url.pathname = strip_data_suffix2(url.pathname) + (url.searchParams.get(TRAILING_SLASH_PARAM) === "1" ? "/" : "") || "/";
    url.searchParams.delete(TRAILING_SLASH_PARAM);
    invalidated_data_nodes = url.searchParams.get(INVALIDATED_PARAM)?.split("").map((node) => node === "1");
    url.searchParams.delete(INVALIDATED_PARAM);
  } else if (remote_id) {
    url.pathname = request.headers.get("x-sveltekit-pathname") ?? base;
    url.search = request.headers.get("x-sveltekit-search") ?? "";
  }
  const headers2 = {};
  const { cookies, new_cookies, get_cookie_header, set_internal, set_trailing_slash } = get_cookies(
    request,
    url
  );
  const event_state = {
    prerendering: state2.prerendering,
    transport: options2.hooks.transport,
    handleValidationError: options2.hooks.handleValidationError,
    tracing: {
      record_span
    },
    is_in_remote_function: false
  };
  const event = {
    cookies,
    // @ts-expect-error `fetch` needs to be created after the `event` itself
    fetch: null,
    getClientAddress: state2.getClientAddress || (() => {
      throw new Error(
        `${"@sveltejs/adapter-cloudflare"} does not specify getClientAddress. Please raise an issue`
      );
    }),
    locals: {},
    params: {},
    platform: state2.platform,
    request,
    route: { id: null },
    setHeaders: /* @__PURE__ */ __name((new_headers) => {
      for (const key2 in new_headers) {
        const lower = key2.toLowerCase();
        const value = new_headers[key2];
        if (lower === "set-cookie") {
          throw new Error(
            "Use `event.cookies.set(name, value, options)` instead of `event.setHeaders` to set cookies"
          );
        } else if (lower in headers2) {
          throw new Error(`"${key2}" header is already set`);
        } else {
          headers2[lower] = value;
          if (state2.prerendering && lower === "cache-control") {
            state2.prerendering.cache = /** @type {string} */
            value;
          }
        }
      }
    }, "setHeaders"),
    url,
    isDataRequest: is_data_request,
    isSubRequest: state2.depth > 0,
    isRemoteRequest: !!remote_id
  };
  event.fetch = create_fetch({
    event,
    options: options2,
    manifest: manifest2,
    state: state2,
    get_cookie_header,
    set_internal
  });
  if (state2.emulator?.platform) {
    event.platform = await state2.emulator.platform({
      config: {},
      prerender: !!state2.prerendering?.fallback
    });
  }
  let resolved_path = url.pathname;
  if (!remote_id) {
    const prerendering_reroute_state = state2.prerendering?.inside_reroute;
    try {
      if (state2.prerendering) state2.prerendering.inside_reroute = true;
      resolved_path = await options2.hooks.reroute({ url: new URL(url), fetch: event.fetch }) ?? url.pathname;
    } catch {
      return text("Internal Server Error", {
        status: 500
      });
    } finally {
      if (state2.prerendering) state2.prerendering.inside_reroute = prerendering_reroute_state;
    }
  }
  try {
    resolved_path = decode_pathname(resolved_path);
  } catch {
    return text("Malformed URI", { status: 400 });
  }
  if (resolved_path !== url.pathname && !state2.prerendering?.fallback && has_prerendered_path(manifest2, resolved_path)) {
    const url2 = new URL(request.url);
    url2.pathname = is_data_request ? add_data_suffix2(resolved_path) : is_route_resolution_request ? add_resolution_suffix2(resolved_path) : resolved_path;
    const response = await fetch(url2, request);
    const headers22 = new Headers(response.headers);
    if (headers22.has("content-encoding")) {
      headers22.delete("content-encoding");
      headers22.delete("content-length");
    }
    return new Response(response.body, {
      headers: headers22,
      status: response.status,
      statusText: response.statusText
    });
  }
  let route = null;
  if (base && !state2.prerendering?.fallback) {
    if (!resolved_path.startsWith(base)) {
      return text("Not found", { status: 404 });
    }
    resolved_path = resolved_path.slice(base.length) || "/";
  }
  if (is_route_resolution_request) {
    return resolve_route(resolved_path, new URL(request.url), manifest2);
  }
  if (resolved_path === `/${app_dir}/env.js`) {
    return get_public_env(request);
  }
  if (!remote_id && resolved_path.startsWith(`/${app_dir}`)) {
    const headers22 = new Headers();
    headers22.set("cache-control", "public, max-age=0, must-revalidate");
    return text("Not found", { status: 404, headers: headers22 });
  }
  if (!state2.prerendering?.fallback) {
    const matchers = await manifest2._.matchers();
    for (const candidate of manifest2._.routes) {
      const match = candidate.pattern.exec(resolved_path);
      if (!match) continue;
      const matched = exec(match, candidate.params, matchers);
      if (matched) {
        route = candidate;
        event.route = { id: route.id };
        event.params = decode_params(matched);
        break;
      }
    }
  }
  let resolve_opts = {
    transformPageChunk: default_transform,
    filterSerializedResponseHeaders: default_filter,
    preload: default_preload
  };
  let trailing_slash = "never";
  try {
    const page_nodes = route?.page ? new PageNodes(await load_page_nodes(route.page, manifest2)) : void 0;
    if (route && !remote_id) {
      if (url.pathname === base || url.pathname === base + "/") {
        trailing_slash = "always";
      } else if (page_nodes) {
        if (BROWSER) ;
        trailing_slash = page_nodes.trailing_slash();
      } else if (route.endpoint) {
        const node = await route.endpoint();
        trailing_slash = node.trailingSlash ?? "never";
        if (BROWSER) ;
      }
      if (!is_data_request) {
        const normalized = normalize_path(url.pathname, trailing_slash);
        if (normalized !== url.pathname && !state2.prerendering?.fallback) {
          return new Response(void 0, {
            status: 308,
            headers: {
              "x-sveltekit-normalize": "1",
              location: (
                // ensure paths starting with '//' are not treated as protocol-relative
                (normalized.startsWith("//") ? url.origin + normalized : normalized) + (url.search === "?" ? "" : url.search)
              )
            }
          });
        }
      }
      if (state2.before_handle || state2.emulator?.platform) {
        let config2 = {};
        let prerender = false;
        if (route.endpoint) {
          const node = await route.endpoint();
          config2 = node.config ?? config2;
          prerender = node.prerender ?? prerender;
        } else if (page_nodes) {
          config2 = page_nodes.get_config() ?? config2;
          prerender = page_nodes.prerender();
        }
        if (state2.before_handle) {
          state2.before_handle(event, config2, prerender);
        }
        if (state2.emulator?.platform) {
          event.platform = await state2.emulator.platform({ config: config2, prerender });
        }
      }
    }
    set_trailing_slash(trailing_slash);
    if (state2.prerendering && !state2.prerendering.fallback && !state2.prerendering.inside_reroute) {
      disable_search(url);
    }
    const response = await record_span({
      name: "sveltekit.handle.root",
      attributes: {
        "http.route": event.route.id || "unknown",
        "http.method": event.request.method,
        "http.url": event.url.href,
        "sveltekit.is_data_request": is_data_request,
        "sveltekit.is_sub_request": event.isSubRequest
      },
      fn: /* @__PURE__ */ __name(async (root_span) => {
        const traced_event = {
          ...event,
          tracing: {
            enabled: false,
            root: root_span,
            current: root_span
          }
        };
        return await with_request_store(
          { event: traced_event, state: event_state },
          () => options2.hooks.handle({
            event: traced_event,
            resolve: /* @__PURE__ */ __name((event2, opts) => {
              return record_span({
                name: "sveltekit.resolve",
                attributes: {
                  "http.route": event2.route.id || "unknown"
                },
                fn: /* @__PURE__ */ __name((resolve_span) => {
                  return with_request_store(
                    null,
                    () => resolve2(merge_tracing(event2, resolve_span), page_nodes, opts).then(
                      (response2) => {
                        for (const key2 in headers2) {
                          const value = headers2[key2];
                          response2.headers.set(
                            key2,
                            /** @type {string} */
                            value
                          );
                        }
                        add_cookies_to_headers(response2.headers, new_cookies.values());
                        if (state2.prerendering && event2.route.id !== null) {
                          response2.headers.set("x-sveltekit-routeid", encodeURI(event2.route.id));
                        }
                        resolve_span.setAttributes({
                          "http.response.status_code": response2.status,
                          "http.response.body.size": response2.headers.get("content-length") || "unknown"
                        });
                        return response2;
                      }
                    )
                  );
                }, "fn")
              });
            }, "resolve")
          })
        );
      }, "fn")
    });
    if (response.status === 200 && response.headers.has("etag")) {
      let if_none_match_value = request.headers.get("if-none-match");
      if (if_none_match_value?.startsWith('W/"')) {
        if_none_match_value = if_none_match_value.substring(2);
      }
      const etag2 = (
        /** @type {string} */
        response.headers.get("etag")
      );
      if (if_none_match_value === etag2) {
        const headers22 = new Headers({ etag: etag2 });
        for (const key2 of [
          "cache-control",
          "content-location",
          "date",
          "expires",
          "vary",
          "set-cookie"
        ]) {
          const value = response.headers.get(key2);
          if (value) headers22.set(key2, value);
        }
        return new Response(void 0, {
          status: 304,
          headers: headers22
        });
      }
    }
    if (is_data_request && response.status >= 300 && response.status <= 308) {
      const location = response.headers.get("location");
      if (location) {
        return redirect_json_response(new Redirect(
          /** @type {any} */
          response.status,
          location
        ));
      }
    }
    return response;
  } catch (e3) {
    if (e3 instanceof Redirect) {
      const response = is_data_request || remote_id ? redirect_json_response(e3) : route?.page && is_action_json_request(event) ? action_json_redirect(e3) : redirect_response(e3.status, e3.location);
      add_cookies_to_headers(response.headers, new_cookies.values());
      return response;
    }
    return await handle_fatal_error(event, event_state, options2, e3);
  }
  async function resolve2(event2, page_nodes, opts) {
    try {
      if (opts) {
        resolve_opts = {
          transformPageChunk: opts.transformPageChunk || default_transform,
          filterSerializedResponseHeaders: opts.filterSerializedResponseHeaders || default_filter,
          preload: opts.preload || default_preload
        };
      }
      if (options2.hash_routing || state2.prerendering?.fallback) {
        return await render_response({
          event: event2,
          event_state,
          options: options2,
          manifest: manifest2,
          state: state2,
          page_config: { ssr: false, csr: true },
          status: 200,
          error: null,
          branch: [],
          fetched: [],
          resolve_opts,
          data_serializer: server_data_serializer(event2, event_state, options2)
        });
      }
      if (remote_id) {
        return await handle_remote_call(event2, event_state, options2, manifest2, remote_id);
      }
      if (route) {
        const method = (
          /** @type {import('types').HttpMethod} */
          event2.request.method
        );
        let response2;
        if (is_data_request) {
          response2 = await render_data(
            event2,
            event_state,
            route,
            options2,
            manifest2,
            state2,
            invalidated_data_nodes,
            trailing_slash
          );
        } else if (route.endpoint && (!route.page || is_endpoint_request(event2))) {
          response2 = await render_endpoint(event2, event_state, await route.endpoint(), state2);
        } else if (route.page) {
          if (!page_nodes) {
            throw new Error("page_nodes not found. This should never happen");
          } else if (page_methods.has(method)) {
            response2 = await render_page(
              event2,
              event_state,
              route.page,
              options2,
              manifest2,
              state2,
              page_nodes,
              resolve_opts
            );
          } else {
            const allowed_methods2 = new Set(allowed_page_methods);
            const node = await manifest2._.nodes[route.page.leaf]();
            if (node?.server?.actions) {
              allowed_methods2.add("POST");
            }
            if (method === "OPTIONS") {
              response2 = new Response(null, {
                status: 204,
                headers: {
                  allow: Array.from(allowed_methods2.values()).join(", ")
                }
              });
            } else {
              const mod = [...allowed_methods2].reduce(
                (acc, curr) => {
                  acc[curr] = true;
                  return acc;
                },
                /** @type {Record<string, any>} */
                {}
              );
              response2 = method_not_allowed(mod, method);
            }
          }
        } else {
          throw new Error("Route is neither page nor endpoint. This should never happen");
        }
        if (request.method === "GET" && route.page && route.endpoint) {
          const vary = response2.headers.get("vary")?.split(",")?.map((v) => v.trim().toLowerCase());
          if (!(vary?.includes("accept") || vary?.includes("*"))) {
            response2 = new Response(response2.body, {
              status: response2.status,
              statusText: response2.statusText,
              headers: new Headers(response2.headers)
            });
            response2.headers.append("Vary", "Accept");
          }
        }
        return response2;
      }
      if (state2.error && event2.isSubRequest) {
        const headers22 = new Headers(request.headers);
        headers22.set("x-sveltekit-error", "true");
        return await fetch(request, { headers: headers22 });
      }
      if (state2.error) {
        return text("Internal Server Error", {
          status: 500
        });
      }
      if (state2.depth === 0) {
        if (BROWSER && event2.url.pathname === "/.well-known/appspecific/com.chrome.devtools.json") ;
        return await respond_with_error({
          event: event2,
          event_state,
          options: options2,
          manifest: manifest2,
          state: state2,
          status: 404,
          error: new SvelteKitError(404, "Not Found", `Not found: ${event2.url.pathname}`),
          resolve_opts
        });
      }
      if (state2.prerendering) {
        return text("not found", { status: 404 });
      }
      const response = await fetch(request);
      return new Response(response.body, response);
    } catch (e3) {
      return await handle_fatal_error(event2, event_state, options2, e3);
    } finally {
      event2.cookies.set = () => {
        throw new Error("Cannot use `cookies.set(...)` after the response has been generated");
      };
      event2.setHeaders = () => {
        throw new Error("Cannot use `setHeaders(...)` after the response has been generated");
      };
    }
  }
  __name(resolve2, "resolve2");
}
__name(internal_respond, "internal_respond");
function load_page_nodes(page2, manifest2) {
  return Promise.all([
    // we use == here rather than === because [undefined] serializes as "[null]"
    ...page2.layouts.map((n2) => n2 == void 0 ? n2 : manifest2._.nodes[n2]()),
    manifest2._.nodes[page2.leaf]()
  ]);
}
__name(load_page_nodes, "load_page_nodes");
function propagate_context(fn) {
  return async (req, ...rest) => {
    {
      return fn(req, ...rest);
    }
  };
}
__name(propagate_context, "propagate_context");
function filter_env(env3, allowed, disallowed) {
  return Object.fromEntries(
    Object.entries(env3).filter(
      ([k]) => k.startsWith(allowed) && (disallowed === "" || !k.startsWith(disallowed))
    )
  );
}
__name(filter_env, "filter_env");
function set_app(value) {
}
__name(set_app, "set_app");
var init_promise;
var current = null;
var Server = class {
  static {
    __name(this, "Server");
  }
  /** @type {import('types').SSROptions} */
  #options;
  /** @type {import('@sveltejs/kit').SSRManifest} */
  #manifest;
  /** @param {import('@sveltejs/kit').SSRManifest} manifest */
  constructor(manifest2) {
    this.#options = options;
    this.#manifest = manifest2;
    if (IN_WEBCONTAINER2) {
      const respond2 = this.respond.bind(this);
      this.respond = async (...args) => {
        const { promise, resolve: resolve2 } = (
          /** @type {PromiseWithResolvers<void>} */
          with_resolvers()
        );
        const previous = current;
        current = promise;
        await previous;
        return respond2(...args).finally(resolve2);
      };
    }
  }
  /**
   * @param {import('@sveltejs/kit').ServerInitOptions} opts
   */
  async init({ env: env3, read }) {
    const { env_public_prefix, env_private_prefix } = this.#options;
    set_private_env(filter_env(env3, env_private_prefix, env_public_prefix));
    set_public_env(filter_env(env3, env_public_prefix, env_private_prefix));
    if (read) {
      const wrapped_read = /* @__PURE__ */ __name((file) => {
        const result = read(file);
        if (result instanceof ReadableStream) {
          return result;
        } else {
          return new ReadableStream({
            async start(controller2) {
              try {
                const stream = await Promise.resolve(result);
                if (!stream) {
                  controller2.close();
                  return;
                }
                const reader = stream.getReader();
                while (true) {
                  const { done, value } = await reader.read();
                  if (done) break;
                  controller2.enqueue(value);
                }
                controller2.close();
              } catch (error22) {
                controller2.error(error22);
              }
            }
          });
        }
      }, "wrapped_read");
      set_read_implementation(wrapped_read);
    }
    await (init_promise ??= (async () => {
      try {
        const module = await get_hooks();
        this.#options.hooks = {
          handle: module.handle || (({ event, resolve: resolve2 }) => resolve2(event)),
          handleError: module.handleError || (({ status, error: error22, event }) => {
            const error_message = format_server_error(
              status,
              /** @type {Error} */
              error22,
              event
            );
            console.error(error_message);
          }),
          handleFetch: module.handleFetch || (({ request, fetch: fetch2 }) => fetch2(request)),
          handleValidationError: module.handleValidationError || (({ issues }) => {
            console.error("Remote function schema validation failed:", issues);
            return { message: "Bad Request" };
          }),
          reroute: module.reroute || (() => {
          }),
          transport: module.transport || {}
        };
        set_app({
          decoders: module.transport ? Object.fromEntries(Object.entries(module.transport).map(([k, v]) => [k, v.decode])) : {}
        });
        if (module.init) {
          await module.init();
        }
      } catch (e3) {
        {
          throw e3;
        }
      }
    })());
  }
  /**
   * @param {Request} request
   * @param {import('types').RequestOptions} options
   */
  async respond(request, options2) {
    return respond(request, this.#options, this.#manifest, {
      ...options2,
      error: false,
      depth: 0
    });
  }
};

// .svelte-kit/cloudflare-tmp/manifest.js
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var manifest = (() => {
  function __memo(fn) {
    let value;
    return () => value ??= value = fn();
  }
  __name(__memo, "__memo");
  return {
    appDir: "_app",
    appPath: "_app",
    assets: /* @__PURE__ */ new Set(["unity-webgl/Build/unity-webgl.data.unityweb", "unity-webgl/Build/unity-webgl.framework.js.unityweb", "unity-webgl/Build/unity-webgl.loader.js", "unity-webgl/Build/unity-webgl.wasm.unityweb", "unity-webgl/TemplateData/webmemd-icon.png"]),
    mimeTypes: { ".js": "text/javascript", ".png": "image/png" },
    _: {
      client: { start: "_app/immutable/entry/start.COquKCCi.js", app: "_app/immutable/entry/app.CfW62mSe.js", imports: ["_app/immutable/entry/start.COquKCCi.js", "_app/immutable/chunks/CligCAuS.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/C_Sh5Hij.js", "_app/immutable/entry/app.CfW62mSe.js", "_app/immutable/chunks/H8HRPmpP.js", "_app/immutable/chunks/DWDlLD0t.js", "_app/immutable/chunks/DFqY95ai.js", "_app/immutable/chunks/C_Sh5Hij.js", "_app/immutable/chunks/yg97jW04.js"], stylesheets: [], fonts: [], uses_env_dynamic_public: false },
      nodes: [
        __memo(() => Promise.resolve().then(() => (init__(), __exports))),
        __memo(() => Promise.resolve().then(() => (init__2(), __exports2))),
        __memo(() => Promise.resolve().then(() => (init__3(), __exports3))),
        __memo(() => Promise.resolve().then(() => (init__4(), __exports4)))
      ],
      remotes: {},
      routes: [
        {
          id: "/",
          pattern: /^\/$/,
          params: [],
          page: { layouts: [0], errors: [1], leaf: 2 },
          endpoint: null
        },
        {
          id: "/api/profile-picture",
          pattern: /^\/api\/profile-picture\/?$/,
          params: [],
          page: null,
          endpoint: __memo(() => Promise.resolve().then(() => (init_server_ts(), server_ts_exports)))
        },
        {
          id: "/game",
          pattern: /^\/game\/?$/,
          params: [],
          page: { layouts: [0], errors: [1], leaf: 3 },
          endpoint: null
        }
      ],
      prerendered_routes: /* @__PURE__ */ new Set([]),
      matchers: /* @__PURE__ */ __name(async () => {
        return {};
      }, "matchers"),
      server_assets: {}
    }
  };
})();
var prerendered = /* @__PURE__ */ new Set([]);
var base_path = "";

// .svelte-kit/cloudflare/_worker.js
import { env as env2 } from "cloudflare:workers";
async function e(e3, t2) {
  let n2 = "string" != typeof t2 && "HEAD" === t2.method;
  n2 && (t2 = new Request(t2, { method: "GET" }));
  let r3 = await e3.match(t2);
  return n2 && r3 && (r3 = new Response(null, r3)), r3;
}
__name(e, "e");
function t(e3, t2, n2, o2) {
  return ("string" == typeof t2 || "GET" === t2.method) && r2(n2) && (n2.headers.has("Set-Cookie") && (n2 = new Response(n2.body, n2)).headers.append("Cache-Control", "private=Set-Cookie"), o2.waitUntil(e3.put(t2, n2.clone()))), n2;
}
__name(t, "t");
var n = /* @__PURE__ */ new Set([200, 203, 204, 300, 301, 404, 405, 410, 414, 501]);
function r2(e3) {
  if (!n.has(e3.status)) return false;
  if (~(e3.headers.get("Vary") || "").indexOf("*")) return false;
  let t2 = e3.headers.get("Cache-Control") || "";
  return !/(private|no-cache|no-store)/i.test(t2);
}
__name(r2, "r");
function o(n2) {
  return async function(r3, o2) {
    let a = await e(n2, r3);
    if (a) return a;
    o2.defer((e3) => {
      t(n2, r3, e3, o2);
    });
  };
}
__name(o, "o");
var s2 = caches.default;
var c = t.bind(0, s2);
var r22 = e.bind(0, s2);
var e2 = o.bind(0, s2);
var server = new Server(manifest);
var app_path = `/${manifest.appPath}`;
var immutable = `${app_path}/immutable/`;
var version_file = `${app_path}/version.json`;
var origin;
var initialized = server.init({
  // @ts-expect-error env contains environment variables and bindings
  env: env2,
  read: /* @__PURE__ */ __name(async (file) => {
    const url = `${origin}/${file}`;
    const response = await /** @type {{ ASSETS: { fetch: typeof fetch } }} */
    env2.ASSETS.fetch(
      url
    );
    if (!response.ok) {
      throw new Error(
        `read(...) failed: could not fetch ${url} (${response.status} ${response.statusText})`
      );
    }
    return response.body;
  }, "read")
});
var worker_default = {
  /**
   * @param {Request} req
   * @param {{ ASSETS: { fetch: typeof fetch } }} env
   * @param {ExecutionContext} ctx
   * @returns {Promise<Response>}
   */
  async fetch(req, env22, ctx) {
    if (!origin) {
      origin = new URL(req.url).origin;
      await initialized;
    }
    let pragma = req.headers.get("cache-control") || "";
    let res = !pragma.includes("no-cache") && await r22(req);
    if (res) return res;
    let { pathname, search } = new URL(req.url);
    try {
      pathname = decodeURIComponent(pathname);
    } catch {
    }
    const stripped_pathname = pathname.replace(/\/$/, "");
    let is_static_asset = false;
    const filename = stripped_pathname.slice(base_path.length + 1);
    if (filename) {
      is_static_asset = manifest.assets.has(filename) || manifest.assets.has(filename + "/index.html") || filename in manifest._.server_assets || filename + "/index.html" in manifest._.server_assets;
    }
    let location = pathname.at(-1) === "/" ? stripped_pathname : pathname + "/";
    if (is_static_asset || prerendered.has(pathname) || pathname === version_file || pathname.startsWith(immutable)) {
      res = await env22.ASSETS.fetch(req);
    } else if (location && prerendered.has(location)) {
      if (search) location += search;
      res = new Response("", {
        status: 308,
        headers: {
          location
        }
      });
    } else {
      res = await server.respond(req, {
        platform: {
          env: env22,
          ctx,
          context: ctx,
          // deprecated in favor of ctx
          // @ts-expect-error webworker types from worktop are not compatible with Cloudflare Workers types
          caches,
          // @ts-expect-error the type is correct but ts is confused because platform.cf uses the type from index.ts while req.cf uses the type from index.d.ts
          cf: req.cf
        },
        getClientAddress() {
          return (
            /** @type {string} */
            req.headers.get("cf-connecting-ip")
          );
        }
      });
    }
    pragma = res.headers.get("cache-control") || "";
    return pragma && res.status < 400 ? c(req, res, ctx) : res;
  }
};

// node_modules/wrangler/templates/middleware/middleware-ensure-req-body-drained.ts
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var drainBody = /* @__PURE__ */ __name(async (request, env3, _ctx, middlewareCtx) => {
  try {
    return await middlewareCtx.next(request, env3);
  } finally {
    try {
      if (request.body !== null && !request.bodyUsed) {
        const reader = request.body.getReader();
        while (!(await reader.read()).done) {
        }
      }
    } catch (e3) {
      console.error("Failed to drain the unused request body.", e3);
    }
  }
}, "drainBody");
var middleware_ensure_req_body_drained_default = drainBody;

// .wrangler/tmp/bundle-Tn6LFy/middleware-insertion-facade.js
var __INTERNAL_WRANGLER_MIDDLEWARE__ = [
  middleware_ensure_req_body_drained_default
];
var middleware_insertion_facade_default = worker_default;

// node_modules/wrangler/templates/middleware/common.ts
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_process();
init_virtual_unenv_global_polyfill_cloudflare_unenv_preset_node_console();
init_performance2();
var __facade_middleware__ = [];
function __facade_register__(...args) {
  __facade_middleware__.push(...args.flat());
}
__name(__facade_register__, "__facade_register__");
function __facade_invokeChain__(request, env3, ctx, dispatch, middlewareChain) {
  const [head2, ...tail] = middlewareChain;
  const middlewareCtx = {
    dispatch,
    next(newRequest, newEnv) {
      return __facade_invokeChain__(newRequest, newEnv, ctx, dispatch, tail);
    }
  };
  return head2(request, env3, ctx, middlewareCtx);
}
__name(__facade_invokeChain__, "__facade_invokeChain__");
function __facade_invoke__(request, env3, ctx, dispatch, finalMiddleware) {
  return __facade_invokeChain__(request, env3, ctx, dispatch, [
    ...__facade_middleware__,
    finalMiddleware
  ]);
}
__name(__facade_invoke__, "__facade_invoke__");

// .wrangler/tmp/bundle-Tn6LFy/middleware-loader.entry.ts
var __Facade_ScheduledController__ = class ___Facade_ScheduledController__ {
  constructor(scheduledTime, cron, noRetry) {
    this.scheduledTime = scheduledTime;
    this.cron = cron;
    this.#noRetry = noRetry;
  }
  static {
    __name(this, "__Facade_ScheduledController__");
  }
  #noRetry;
  noRetry() {
    if (!(this instanceof ___Facade_ScheduledController__)) {
      throw new TypeError("Illegal invocation");
    }
    this.#noRetry();
  }
};
function wrapExportedHandler(worker) {
  if (__INTERNAL_WRANGLER_MIDDLEWARE__ === void 0 || __INTERNAL_WRANGLER_MIDDLEWARE__.length === 0) {
    return worker;
  }
  for (const middleware of __INTERNAL_WRANGLER_MIDDLEWARE__) {
    __facade_register__(middleware);
  }
  const fetchDispatcher = /* @__PURE__ */ __name(function(request, env3, ctx) {
    if (worker.fetch === void 0) {
      throw new Error("Handler does not export a fetch() function.");
    }
    return worker.fetch(request, env3, ctx);
  }, "fetchDispatcher");
  return {
    ...worker,
    fetch(request, env3, ctx) {
      const dispatcher = /* @__PURE__ */ __name(function(type, init2) {
        if (type === "scheduled" && worker.scheduled !== void 0) {
          const controller2 = new __Facade_ScheduledController__(
            Date.now(),
            init2.cron ?? "",
            () => {
            }
          );
          return worker.scheduled(controller2, env3, ctx);
        }
      }, "dispatcher");
      return __facade_invoke__(request, env3, ctx, dispatcher, fetchDispatcher);
    }
  };
}
__name(wrapExportedHandler, "wrapExportedHandler");
function wrapWorkerEntrypoint(klass) {
  if (__INTERNAL_WRANGLER_MIDDLEWARE__ === void 0 || __INTERNAL_WRANGLER_MIDDLEWARE__.length === 0) {
    return klass;
  }
  for (const middleware of __INTERNAL_WRANGLER_MIDDLEWARE__) {
    __facade_register__(middleware);
  }
  return class extends klass {
    #fetchDispatcher = /* @__PURE__ */ __name((request, env3, ctx) => {
      this.env = env3;
      this.ctx = ctx;
      if (super.fetch === void 0) {
        throw new Error("Entrypoint class does not define a fetch() function.");
      }
      return super.fetch(request);
    }, "#fetchDispatcher");
    #dispatcher = /* @__PURE__ */ __name((type, init2) => {
      if (type === "scheduled" && super.scheduled !== void 0) {
        const controller2 = new __Facade_ScheduledController__(
          Date.now(),
          init2.cron ?? "",
          () => {
          }
        );
        return super.scheduled(controller2);
      }
    }, "#dispatcher");
    fetch(request) {
      return __facade_invoke__(
        request,
        this.env,
        this.ctx,
        this.#dispatcher,
        this.#fetchDispatcher
      );
    }
  };
}
__name(wrapWorkerEntrypoint, "wrapWorkerEntrypoint");
var WRAPPED_ENTRY;
if (typeof middleware_insertion_facade_default === "object") {
  WRAPPED_ENTRY = wrapExportedHandler(middleware_insertion_facade_default);
} else if (typeof middleware_insertion_facade_default === "function") {
  WRAPPED_ENTRY = wrapWorkerEntrypoint(middleware_insertion_facade_default);
}
var middleware_loader_entry_default = WRAPPED_ENTRY;
export {
  __INTERNAL_WRANGLER_MIDDLEWARE__,
  middleware_loader_entry_default as default
};
/*! Bundled license information:

cookie/index.js:
  (*!
   * cookie
   * Copyright(c) 2012-2014 Roman Shtylman
   * Copyright(c) 2015 Douglas Christopher Wilson
   * MIT Licensed
   *)
*/
//# sourceMappingURL=_worker.js.map
