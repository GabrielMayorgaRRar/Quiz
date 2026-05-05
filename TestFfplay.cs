using System;
using System.Diagnostics;
using System.Threading.Tasks;

class TestFfplay {
    static void TestMain() {
        var p = Process.Start(new ProcessStartInfo {
            FileName = "ffplay",
            Arguments = "-nodisp -autoexit https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
            UseShellExecute = false
        });
        p?.WaitForExit();
        Console.WriteLine("Done.");
    }
}
