using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity
{

    public enum AvgResetInterval
    {
        Always = 1,
        VeryFast = 30,
        Fast = 60,
        Normal = 120,
        Slow = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems
    {

        public static AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        public int totalInitializeSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _initializeSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalInitializeSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalExecuteSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _executeSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalExecuteSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalUpdateSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _updateSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalUpdateSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalLateUpdateSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _lateUpdateSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalLateUpdateSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalFixedUpdateSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _fixedUpdateSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalFixedUpdateSystemsCount : 1;
                }
                return total;
            }

        }

        public int totalCleanupSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _cleanupSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalCleanupSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalTearDownSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _tearDownSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalTearDownSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _systems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalSystemsCount : 1;
                }
                return total;
            }
        }

        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }
        public int updateSystemsCount { get { return _updateSystems.Count; } }
        public int lateUpdateSystemsCount { get { return _lateUpdateSystems.Count; } }
        public int fixedUpdateSystemsCount { get { return _fixedUpdateSystems.Count; } }
        public int cleanupSystemsCount { get { return _cleanupSystems.Count; } }
        public int tearDownSystemsCount { get { return _tearDownSystems.Count; } }

        public string name { get { return _name; } }
        public GameObject gameObject { get { return _gameObject; } }
        public SystemInfo systemInfo { get { return _systemInfo; } }

        public double executeDuration { get { return _executeDuration; } }
        public double updateDuration { get { return _updateDuration; } }
        public double lateUpdateDuration { get { return _lateUpdateDuration; } }
        public double fixedUpdateDuration { get { return _fixedUpdateDuration; } }
        public double cleanupDuration { get { return _cleanupDuration; } }

        public SystemInfo[] initializeSystemInfos { get { return _initializeSystemInfos.ToArray(); } }
        public SystemInfo[] executeSystemInfos { get { return _executeSystemInfos.ToArray(); } }
        public SystemInfo[] updateSystemInfos { get { return _updateSystemInfos.ToArray(); } }
        public SystemInfo[] lateUpdateSystemInfos { get { return _lateUpdateSystemInfos.ToArray(); } }
        public SystemInfo[] fixedUpdateSystemInfos { get { return _fixedUpdateSystemInfos.ToArray(); } }
        public SystemInfo[] cleanupSystemInfos { get { return _cleanupSystemInfos.ToArray(); } }
        public SystemInfo[] tearDownSystemInfos { get { return _tearDownSystemInfos.ToArray(); } }

        public bool paused;

        string _name;

        List<ISystem> _systems;
        GameObject _gameObject;
        SystemInfo _systemInfo;

        List<SystemInfo> _initializeSystemInfos;
        List<SystemInfo> _executeSystemInfos;
        List<SystemInfo> _updateSystemInfos;
        List<SystemInfo> _lateUpdateSystemInfos;
        List<SystemInfo> _fixedUpdateSystemInfos;
        List<SystemInfo> _cleanupSystemInfos;
        List<SystemInfo> _tearDownSystemInfos;

        Stopwatch _stopwatch;

        double _executeDuration;
        double _updateDuration;
        double _lateUpdateDuration;
        double _fixedUpdateDuration;
        double _cleanupDuration;

        public DebugSystems(string name)
        {
            initialize(name);
        }

        protected DebugSystems(bool noInit)
        {
        }

        protected void initialize(string name)
        {
            _name = name;
            _gameObject = new GameObject(name);
            _gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);

            _systemInfo = new SystemInfo(this);

            _systems = new List<ISystem>();
            _initializeSystemInfos = new List<SystemInfo>();
            _executeSystemInfos = new List<SystemInfo>();
            _updateSystemInfos = new List<SystemInfo>();
            _lateUpdateSystemInfos = new List<SystemInfo>();
            _fixedUpdateSystemInfos = new List<SystemInfo>();
            _cleanupSystemInfos = new List<SystemInfo>();
            _tearDownSystemInfos = new List<SystemInfo>();

            _stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system)
        {
            _systems.Add(system);

            SystemInfo childSystemInfo;

            var debugSystems = system as DebugSystems;
            if (debugSystems != null)
            {
                childSystemInfo = debugSystems.systemInfo;
                debugSystems.gameObject.transform.SetParent(_gameObject.transform, false);
            }
            else
            {
                childSystemInfo = new SystemInfo(system);
            }

            childSystemInfo.parentSystemInfo = _systemInfo;

            if (childSystemInfo.isInitializeSystems)
            {
                _initializeSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isExecuteSystems || childSystemInfo.isReactiveSystems)
            {
                _executeSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isCleanupSystems)
            {
                _cleanupSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isTearDownSystems)
            {
                _tearDownSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isUpdateSystems)
            {
                _updateSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isLateUpdateSystems)
            {
                _lateUpdateSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.isFixedUpdateSystems)
            {
                _fixedUpdateSystemInfos.Add(childSystemInfo);
            }

            return base.Add(system);
        }

        public void ResetDurations()
        {
            foreach (var systemInfo in _executeSystemInfos)
            {
                systemInfo.ResetDurations();
            }

            foreach (var system in _systems)
            {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null)
                {
                    debugSystems.ResetDurations();
                }
            }
        }

        public void ResetUpdateDurations()
        {
            foreach (var systemInfo in _updateSystemInfos)
            {
                systemInfo.ResetUpdateDurations();
            }

            foreach (var system in _systems)
            {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null)
                {
                    debugSystems.ResetUpdateDurations();
                }
            }
        }

        public void ResetLateUpdateDurations()
        {
            foreach (var systemInfo in _lateUpdateSystemInfos)
            {
                systemInfo.ResetUpdateDurations();
            }

            foreach (var system in _systems)
            {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null)
                {
                    debugSystems.ResetUpdateDurations();
                }
            }
        }

        public void ResetFixedUpdateDurations()
        {
            foreach (var systemInfo in _fixedUpdateSystemInfos)
            {
                systemInfo.ResetFixedUpdateDurations();
            }

            foreach (var system in _systems)
            {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null)
                {
                    debugSystems.ResetFixedUpdateDurations();
                }
            }
        }

        public override void Initialize()
        {
            for (int i = 0; i < _initializeSystems.Count; i++)
            {
                var systemInfo = _initializeSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _initializeSystems[i].Initialize();
                    _stopwatch.Stop();
                    systemInfo.initializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }

        public override void Execute()
        {
            if (!paused)
            {
                StepExecute();
            }
        }

        public override void Update()
        {
            if (paused)
                return;
            StepUpdate();
        }

        public override void LateUpdate()
        {
            if (paused)
                return;
            StepLateUpdate();
        }

        public override void FixedUpdate()
        {
            if (paused)
                return;
            StepFixedUpdate();
        }

        public override void Cleanup()
        {
            if (!paused)
            {
                StepCleanup();
            }
        }

        public void StepExecute()
        {
            _executeDuration = 0;
            if (Time.frameCount % (int)avgResetInterval == 0)
            {
                ResetDurations();
            }
            for (int i = 0; i < _executeSystems.Count; i++)
            {
                var systemInfo = _executeSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _executeSystems[i].Execute();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _executeDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }
        }

        public void StepUpdate()
        {
            _updateDuration = 0.0;
            if (Time.frameCount % (int)DebugSystems.avgResetInterval == 0)
                ResetUpdateDurations();
            for (int index = 0; index < _updateSystems.Count; ++index)
            {
                SystemInfo updateSystemInfo = _updateSystemInfos[index];
                if (updateSystemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _updateSystems[index].Update();
                    _stopwatch.Stop();
                    double totalMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;
                    _updateDuration += totalMilliseconds;
                    updateSystemInfo.AddUpdateDuration(totalMilliseconds);
                }
            }
        }

        public void StepLateUpdate()
        {
            _lateUpdateDuration = 0.0;
            if (Time.frameCount % (int)DebugSystems.avgResetInterval == 0)
                ResetLateUpdateDurations();
            for (int index = 0; index < _lateUpdateSystems.Count; ++index)
            {
                SystemInfo lateUpdateSystemInfo = _lateUpdateSystemInfos[index];
                if (lateUpdateSystemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _lateUpdateSystems[index].LateUpdate();
                    _stopwatch.Stop();
                    double totalMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;
                    _lateUpdateDuration += totalMilliseconds;
                    lateUpdateSystemInfo.AddLateUpdateDuration(totalMilliseconds);
                }
            }
        }

        public void StepFixedUpdate()
        {
            _fixedUpdateDuration = 0.0;
            if (Time.frameCount % (int)DebugSystems.avgResetInterval == 0)
                ResetFixedUpdateDurations();
            for (int index = 0; index < _fixedUpdateSystems.Count; ++index)
            {
                SystemInfo fixedUpdateSystemInfo = _fixedUpdateSystemInfos[index];
                if (fixedUpdateSystemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _fixedUpdateSystems[index].FixedUpdate();
                    _stopwatch.Stop();
                    double totalMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;
                    _fixedUpdateDuration += totalMilliseconds;
                    fixedUpdateSystemInfo.AddFixedUpdateDuration(totalMilliseconds);
                }
            }
        }

        public void StepCleanup()
        {
            _cleanupDuration = 0;
            for (int i = 0; i < _cleanupSystems.Count; i++)
            {
                var systemInfo = _cleanupSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _cleanupSystems[i].Cleanup();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _cleanupDuration += duration;
                    systemInfo.AddCleanupDuration(duration);
                }
            }
        }

        public override void TearDown()
        {
            for (int i = 0; i < _tearDownSystems.Count; i++)
            {
                var systemInfo = _tearDownSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _tearDownSystems[i].TearDown();
                    _stopwatch.Stop();
                    systemInfo.teardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }
    }
}
