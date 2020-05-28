namespace Fighter

open System
open System.Reactive.Linq
open System.Reactive.Concurrency
open System.Threading
open System.Runtime.CompilerServices

[<Extension>]
type Observable =
    [<Extension>]
    static member EveryUpdate() =
        let scheduler = SynchronizationContextScheduler(SynchronizationContext.Current)
        Observable.Interval(TimeSpan.FromSeconds(0.01), scheduler)
        //Applicationfs.instance.deltaTime.DistinctUntilChanged().subsc//.Buffer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), scheduler)