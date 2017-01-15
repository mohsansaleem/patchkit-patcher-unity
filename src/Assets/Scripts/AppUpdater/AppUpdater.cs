﻿using PatchKit.Unity.Patcher.AppData.Local;
using PatchKit.Unity.Patcher.AppData.Remote;
using PatchKit.Unity.Patcher.Cancellation;
using PatchKit.Unity.Patcher.Debug;

namespace PatchKit.Unity.Patcher.AppUpdater
{
    public class AppUpdater
    {
        private static readonly DebugLogger DebugLogger = new DebugLogger(typeof(AppUpdater));

        private readonly IAppUpdaterStrategyResolver _strategyResolver;

        private bool _patchHasBeenCalled;

        public readonly AppUpdaterContext Context;

        public AppUpdater(App app, AppUpdaterConfiguration configuration) : this(
            new AppUpdaterStrategyResolver(), new AppUpdaterContext(app, configuration))
        {
        }

        public AppUpdater(IAppUpdaterStrategyResolver strategyResolver, AppUpdaterContext context)
        {
            AssertChecks.ArgumentNotNull(strategyResolver, "strategyResolver");
            AssertChecks.ArgumentNotNull(context, "context");

            DebugLogger.LogConstructor();

            _strategyResolver = strategyResolver;
            Context = context;
        }

        public void Patch(CancellationToken cancellationToken)
        {
            AssertChecks.MethodCalledOnlyOnce(ref _patchHasBeenCalled, "Patch");

            DebugLogger.Log("Patching.");

            var strategy = _strategyResolver.Resolve(Context);
            strategy.Patch(cancellationToken);
        }
    }
}