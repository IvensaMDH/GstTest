using Gst;
using Gst.App;
using System;

var cmd = $"videotestsrc pattern=snow ! video/x-raw,width=1280,height=720,format=BGR ! appsink name=appsink1";

Application.Init();

var pipeline = Parse.Launch(cmd) as Pipeline;
var appsink = new AppSink(pipeline.GetChildByName("appsink1").Handle)
{
    WaitOnEos = true,
    Drop = true,
    Sync = true,
    Qos = true,
    EmitSignals = true,
};
appsink.NewSample += OnNewSample;

pipeline.SetState(State.Playing);

Console.ReadKey();

static void OnNewSample(object o, NewSampleArgs args)
{
    var appsink = o as AppSink;

    using var sample = appsink.TryPullSample(0);
    using var caps = sample.Caps;
    using var cap = caps[0];
    using var buffer = sample.Buffer;

    buffer.Map(out MapInfo map, MapFlags.Read);
    Console.WriteLine($"Sample received!");
    sample.Buffer.Unmap(map);

    GC.Collect(); //Even forcing GC does not work...
}