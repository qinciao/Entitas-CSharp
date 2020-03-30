using System;

namespace Entitas.VisualDebugging.Unity
{

    [Flags]
    public enum SystemInterfaceFlags
    {
        None = 0,
        IInitializeSystem = 1 << 1,
        IExecuteSystem = 1 << 2,
        ICleanupSystem = 1 << 3,
        ITearDownSystem = 1 << 4,
        IReactiveSystem = 1 << 5,
        IUpdateSystem = 1 << 6,
        ILateUpdateSystem = 1 << 7,
        IFixedUpdateSystem = 1 << 8
    }

    public class SystemInfo
    {

        public ISystem system { get { return _system; } }
        public string systemName { get { return _systemName; } }

        public bool isInitializeSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem; } }
        public bool isExecuteSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem; } }
        public bool isCleanupSystems { get { return (_interfaceFlags & SystemInterfaceFlags.ICleanupSystem) == SystemInterfaceFlags.ICleanupSystem; } }
        public bool isTearDownSystems { get { return (_interfaceFlags & SystemInterfaceFlags.ITearDownSystem) == SystemInterfaceFlags.ITearDownSystem; } }
        public bool isReactiveSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem; } }
        public bool isUpdateSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IUpdateSystem) == SystemInterfaceFlags.IUpdateSystem; } }
        public bool isLateUpdateSystems { get { return (_interfaceFlags & SystemInterfaceFlags.ILateUpdateSystem) == SystemInterfaceFlags.ILateUpdateSystem; } }
        public bool isFixedUpdateSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IFixedUpdateSystem) == SystemInterfaceFlags.IFixedUpdateSystem; } }

        public double initializationDuration { get; set; }

        public double accumulatedExecutionDuration { get { return _accumulatedExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration { get { return _executionDurationsCount == 0 ? 0 : _accumulatedExecutionDuration / _executionDurationsCount; } }

        public double accumulatedUpdateDuration { get { return _accumulatedUpdateDuration; } }
        public double minUpdateDuration { get { return _minUpdateDuration; } }
        public double maxUpdateDuration { get { return _maxUpdateDuration; } }
        public double averageUpdateDuration { get { return _updateDurationsCount == 0 ? 0 : _accumulatedUpdateDuration / _updateDurationsCount; } }

        public double accumulatedLateUpdateDuration { get { return _accumulatedLateUpdateDuration; } }
        public double minLateUpdateDuration { get { return _minLateUpdateDuration; } }
        public double maxLateUpdateDuration { get { return _maxLateUpdateDuration; } }
        public double averageLateUpdateDuration { get { return _lateUpdateDurationsCount == 0 ? 0 : _accumulatedLateUpdateDuration / _lateUpdateDurationsCount; } }

        public double accumulatedFixedUpdateDuration { get { return _accumulatedFixedUpdateDuration; } }
        public double minFixedUpdateDuration { get { return _minFixedUpdateDuration; } }
        public double maxFixedUpdateDuration { get { return _maxFixedUpdateDuration; } }
        public double averageFixedUpdateDuration { get { return _fixedUpdateDurationsCount == 0 ? 0 : _accumulatedFixedUpdateDuration / _fixedUpdateDurationsCount; } }

        public double accumulatedCleanupDuration { get { return _accumulatedCleanupDuration; } }
        public double minCleanupDuration { get { return _minCleanupDuration; } }
        public double maxCleanupDuration { get { return _maxCleanupDuration; } }
        public double averageCleanupDuration { get { return _cleanupDurationsCount == 0 ? 0 : _accumulatedCleanupDuration / _cleanupDurationsCount; } }

        public double cleanupDuration { get; set; }
        public double teardownDuration { get; set; }

        public bool areAllParentsActive { get { return parentSystemInfo == null || (parentSystemInfo.isActive && parentSystemInfo.areAllParentsActive); } }

        public SystemInfo parentSystemInfo;
        public bool isActive;

        readonly ISystem _system;
        readonly SystemInterfaceFlags _interfaceFlags;
        readonly string _systemName;

        double _accumulatedExecutionDuration;
        double _minExecutionDuration;
        double _maxExecutionDuration;
        int _executionDurationsCount;

        double _accumulatedUpdateDuration;
        double _minUpdateDuration;
        double _maxUpdateDuration;
        int _updateDurationsCount;

        double _accumulatedLateUpdateDuration;
        double _minLateUpdateDuration;
        double _maxLateUpdateDuration;
        int _lateUpdateDurationsCount;

        double _accumulatedFixedUpdateDuration;
        double _minFixedUpdateDuration;
        double _maxFixedUpdateDuration;
        int _fixedUpdateDurationsCount;

        double _accumulatedCleanupDuration;
        double _minCleanupDuration;
        double _maxCleanupDuration;
        int _cleanupDurationsCount;

        public SystemInfo(ISystem system)
        {
            _system = system;
            _interfaceFlags = getInterfaceFlags(system);

            var debugSystem = system as DebugSystems;
            _systemName = debugSystem != null
                ? debugSystem.name
                : system.GetType().Name.RemoveSystemSuffix();

            isActive = true;
        }

        public void AddExecutionDuration(double executionDuration)
        {
            if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0)
            {
                _minExecutionDuration = executionDuration;
            }
            if (executionDuration > _maxExecutionDuration)
            {
                _maxExecutionDuration = executionDuration;
            }

            _accumulatedExecutionDuration += executionDuration;
            _executionDurationsCount += 1;
        }

        public void AddUpdateDuration(double updateDuration)
        {
            if (updateDuration < _minUpdateDuration || _minUpdateDuration == 0.0)
                _minUpdateDuration = updateDuration;
            if (updateDuration > _maxUpdateDuration)
                _maxUpdateDuration = updateDuration;
            _accumulatedUpdateDuration += updateDuration;
            ++_updateDurationsCount;
        }

        public void AddLateUpdateDuration(double lateUpdateDuration)
        {
            if (lateUpdateDuration < _minLateUpdateDuration || _minLateUpdateDuration == 0.0)
                _minLateUpdateDuration = lateUpdateDuration;
            if (lateUpdateDuration > _maxLateUpdateDuration)
                _maxLateUpdateDuration = lateUpdateDuration;
            _accumulatedLateUpdateDuration += lateUpdateDuration;
            ++_lateUpdateDurationsCount;
        }

        public void AddFixedUpdateDuration(double fixedUpdateDuration)
        {
            if (fixedUpdateDuration < _minFixedUpdateDuration || _minFixedUpdateDuration == 0.0)
                _minFixedUpdateDuration = fixedUpdateDuration;
            if (fixedUpdateDuration > _maxFixedUpdateDuration)
                _maxFixedUpdateDuration = fixedUpdateDuration;
            _accumulatedFixedUpdateDuration += fixedUpdateDuration;
            ++_fixedUpdateDurationsCount;
        }

        public void AddCleanupDuration(double cleanupDuration)
        {
            if (cleanupDuration < _minCleanupDuration || _minCleanupDuration == 0)
            {
                _minCleanupDuration = cleanupDuration;
            }
            if (cleanupDuration > _maxCleanupDuration)
            {
                _maxCleanupDuration = cleanupDuration;
            }

            _accumulatedCleanupDuration += cleanupDuration;
            _cleanupDurationsCount += 1;
        }

        public void ResetDurations()
        {
            _accumulatedExecutionDuration = 0;
            _executionDurationsCount = 0;

            _accumulatedCleanupDuration = 0;
            _cleanupDurationsCount = 0;
        }

        public void ResetUpdateDurations()
        {
            _accumulatedUpdateDuration = 0;
            _updateDurationsCount = 0;
        }

        public void ResetLateUpdateDurations()
        {
            _accumulatedLateUpdateDuration = 0;
            _lateUpdateDurationsCount = 0;
        }

        public void ResetFixedUpdateDurations()
        {
            _accumulatedFixedUpdateDuration = 0;
            _fixedUpdateDurationsCount = 0;
        }

        static SystemInterfaceFlags getInterfaceFlags(ISystem system)
        {
            var flags = SystemInterfaceFlags.None;
            if (system is IInitializeSystem)
            {
                flags |= SystemInterfaceFlags.IInitializeSystem;
            }
            if (system is IReactiveSystem)
            {
                flags |= SystemInterfaceFlags.IReactiveSystem;
            }
            else if (system is IExecuteSystem)
            {
                flags |= SystemInterfaceFlags.IExecuteSystem;
            }
            if (system is ICleanupSystem)
            {
                flags |= SystemInterfaceFlags.ICleanupSystem;
            }
            if (system is ITearDownSystem)
            {
                flags |= SystemInterfaceFlags.ITearDownSystem;
            }
            if (system is IUpdateSystem)
            {
                flags |= SystemInterfaceFlags.IUpdateSystem;
            }
            if (system is ILateUpdateSystem)
            {
                flags |= SystemInterfaceFlags.ILateUpdateSystem;
            }
            if (system is IFixedUpdateSystem)
            {
                flags |= SystemInterfaceFlags.IFixedUpdateSystem;
            }

            return flags;
        }
    }
}
